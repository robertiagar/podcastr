using PodcastR.Data.Entities;
using PodcastR.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace PodcastR.Data.Services
{
    public static class PodcastDownloaderService
    {
        private static Dictionary<Episode, DownloadOperation> downloads = new Dictionary<Episode, DownloadOperation>();
        private static Dictionary<Episode, CancellationTokenSource> cancellationTokenSources = new Dictionary<Episode, CancellationTokenSource>();

        public static void PauseDownload(Episode episode)
        {
            var downloadOperation = downloads[episode];
            if (downloadOperation.Progress.Status == BackgroundTransferStatus.Running)
            {
                downloadOperation.Pause();
            }
        }

        public static void ResumeDownload(Episode episode)
        {
            var downloadOperation = downloads[episode];
            if (downloadOperation.Progress.Status == BackgroundTransferStatus.PausedByApplication)
            {
                downloadOperation.Resume();
            }
        }

        public static void CancelDownload(Episode episode)
        {
            var cancellationTokenSource = cancellationTokenSources[episode];
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSources.Remove(episode);
        }

        public static void CancelAllDownloads()
        {
            foreach (var cancellationTokenSource in cancellationTokenSources.ToList())
            {
                cancellationTokenSource.Value.Cancel();
                cancellationTokenSource.Value.Dispose();
                cancellationTokenSources.Remove(cancellationTokenSource.Key);
            }
        }

        public static int NumberOfDownloads
        {
            get { return downloads.Count; }
        }

        public static async Task<Episode> DownloadPodcastEpisodeAsync(Episode episode, Action<DownloadOperation> callback, CancellationTokenSource cts, Action errorCallback = null)
        {
            var appFolder = await KnownFolders.MusicLibrary.CreateFolderAsync("PodcastR", CreationCollisionOption.OpenIfExists);
            var podcastFolder = await appFolder.CreateFolderAsync(episode.Podcast.Name, CreationCollisionOption.OpenIfExists);
            try
            {
                var extension = episode.Path.AbsolutePath.GetExtension();
                var fileName = episode.Name + extension;
                var episodeFile = await podcastFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                var backgroundDownloader = new BackgroundDownloader();
                var downloadOperation = backgroundDownloader.CreateDownload(episode.Path, episodeFile);
                var progress = new Progress<DownloadOperation>(callback);
                downloads.Add(episode, downloadOperation);
                cancellationTokenSources.Add(episode, cts);

                await downloadOperation.StartAsync().AsTask(cts.Token, progress);

                episode.IsLocal = true;
                episode.Path = new Uri(episodeFile.Path);
                return episode;
            }
            catch
            {
                if (errorCallback != null)
                    errorCallback();
                return episode;
            }
        }
    }
}
