using Newtonsoft.Json;
using PodcastR.Common;
using PodcastR.Core.Entities;
using PodcastR.Interfaces;
using PodcastR.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace PodcastR.Services
{
    public class PodcastsService : IPodcastsService
    {
        private static HttpClient _client;
        private ISettingsService service;

        public PodcastsService(ISettingsService service)
        {
            this.service = service;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", service.AccessToken));
        }

        public async Task<IEnumerable<Podcast>> GetPodcastsAsync()
        {
            var podcastsJson = await _client.GetStringAsync(new Uri(Constants.ApplicationUri + "api/Podcasts"));

            var podcasts = JsonConvert.DeserializeObject<IEnumerable<Podcast>>(podcastsJson);

            return podcasts;
        }

        public async Task<Podcast> GetPodcastAsync(int podcastId)
        {
            var podcastsJson = await _client.GetStringAsync(new Uri(Constants.ApplicationUri + "api/Podcasts/" + podcastId));

            var podcast = JsonConvert.DeserializeObject<Podcast>(podcastsJson);

            return podcast;
        }

        public async Task<Podcast> GetPodcastAsync(string podcastUrl)
        {
            var content = new HttpStringContent(string.Format("\"{0}\"", podcastUrl), UnicodeEncoding.Utf8, "application/json");

            var response = await _client.PostAsync(new Uri(Constants.ApplicationUri + "api/Podcasts"), content);

            if (response.IsSuccessStatusCode)
            {
                var podcastJson = await response.Content.ReadAsStringAsync();

                var podcast = JsonConvert.DeserializeObject<Podcast>(podcastJson);

                return podcast;
            }

            return null;
        }

        public async Task<IEnumerable<Episode>> GetLatestEpisodesAsync(int podcastId, DateTime lastEpisodeDate)
        {
            var response = await _client.GetAsync(new Uri(Constants.ApplicationUri + "api/LatestEpisodes?" + string.Format("podcastId={0}&lastEpisodeDate=", podcastId, lastEpisodeDate)));

            if (response.IsSuccessStatusCode)
            {
                var episodesJson = await response.Content.ReadAsStringAsync();

                var episodes = JsonConvert.DeserializeObject<IEnumerable<Episode>>(episodesJson);

                return episodes;
            }

            return null;
        }
    }
}
