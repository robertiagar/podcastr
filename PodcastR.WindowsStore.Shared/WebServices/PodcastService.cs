using PodcastR.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastR.WebServices
{
    public class PodcastService : IPodcastService
    {
        public System.Threading.Tasks.Task<IEnumerable<Core.Entities.Podcast>> GetPodcastsAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Core.Entities.Podcast> GetPodcastAsync(int podcastId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Core.Entities.Podcast> GetPodcastAsync(string podcastUrl)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<IEnumerable<Core.Entities.Episode>> GetLatestEpisodesAsync(int podcastId, DateTime lastEpisodeDate)
        {
            throw new NotImplementedException();
        }
    }
}
