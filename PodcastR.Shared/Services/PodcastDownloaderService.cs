using PodcastR.Core.Entities;
using PodcastR.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace PodcastR.Services
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
            downloads.Remove(episode);
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

        public static async Task<Episode> DownloadPodcastEpisodeAsync(Episode episode, Action<DownloadOperation> callback, Action errorCallback = null)
        {
            var appFolder = await KnownFolders.MusicLibrary.CreateFolderAsync("PodcastR", CreationCollisionOption.OpenIfExists);
            var podcastFolder = await appFolder.CreateFolderAsync(episode.Podcast.Name, CreationCollisionOption.OpenIfExists);
            try
            {
                var uri = new Uri(episode.WebPath);
                var extension = uri.AbsolutePath.GetExtension();
                var fileName = (episode.Name + extension).RemoveIllegalPathChars().Replace(":", "");
                var episodeFile = await podcastFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                var backgroundDownloader = new BackgroundDownloader();
                var downloadOperation = backgroundDownloader.CreateDownload(uri, episodeFile);
                var progress = new Progress<DownloadOperation>(callback);
                downloads.Add(episode, downloadOperation);
                var cts = new CancellationTokenSource();
                cancellationTokenSources.Add(episode, cts);

                await downloadOperation.StartAsync().AsTask(cts.Token, progress);

                downloads.Remove(episode);
                cancellationTokenSources.Remove(episode);
                episode.IsLocal = true;
                episode.WebPath = episodeFile.Path;
                return episode;
            }
            catch
            {
                if (errorCallback != null)
                    errorCallback();
                return episode;
            }
        }


        public static async Task DeletePodcastEpisodeAsync(Episode episode)
        {
            var file = await StorageFile.GetFileFromPathAsync(episode.WebPath);
            await file.DeleteAsync();
            episode.WebPath = episode.WebPath;
            episode.IsLocal = false; ;
        }
    }
}
