using PodcastR.Data.Entities;
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
        public static async Task DownloadPodcastEpisodeAsync(Episode episode, Action<DownloadOperation> callback)
        {
            var appFolder = await KnownFolders.MusicLibrary.CreateFolderAsync("PodcastR", CreationCollisionOption.OpenIfExists);
            var podcastFolder = await appFolder.CreateFolderAsync(episode.Podcast.Name, CreationCollisionOption.OpenIfExists);
            try
            {
                var episodeFile = await podcastFolder.CreateFileAsync(episode.Name + ".mp3", CreationCollisionOption.FailIfExists);
                var backgroundDownloader = new BackgroundDownloader();
                var downloadOperation = backgroundDownloader.CreateDownload(episode.Path, episodeFile);
                var progress = new Progress<DownloadOperation>(callback);

                await downloadOperation.StartAsync().AsTask(progress);
            }
            catch
            {

            }
        }
    }
}
