using PodcastR.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PodcastR.Interfaces
{
    public interface IPodcastService
    {
        Task<IEnumerable<Podcast>> GetPodcastsAsync();
        Task<Podcast> GetPodcastAsync(int podcastId);
        Task<Podcast> GetPodcastAsync(string podcastUrl);
        Task<IEnumerable<Episode>> GetLatestEpisodesAsync(int podcastId, DateTime lastEpisodeDate);
    }
}
