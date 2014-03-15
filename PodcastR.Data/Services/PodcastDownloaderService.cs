using PodcastR.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace PodcastR.Data.Services
{
    public static class PodcastDownloaderService
    {
        public static async Task<int> DownloadPodcastEpisodeAsync(Episode episode, IProgress<int> progress)
        {
            var appFolder = await KnownFolders.MusicLibrary.CreateFolderAsync("PodcastR", CreationCollisionOption.OpenIfExists);
            var podcastFolder = await appFolder.CreateFolderAsync(episode.Podcast.Name, CreationCollisionOption.OpenIfExists);
            try
            {
                var episodeFile = await podcastFolder.CreateFileAsync(episode.Name + ".mp3", CreationCollisionOption.FailIfExists);
            }
            catch
            {

            }

            return null;
        }
    }
}
