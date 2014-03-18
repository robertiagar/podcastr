using Newtonsoft.Json;
using PodcastR.Data.Entities;
using PodcastR.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

namespace PodcastR.Data.Services
{
    public class PodcastService
    {
        private static readonly string PodcastsFilename = "podcasts.json";
        private static IList<Podcast> podcasts = new List<Podcast>();

        private static async Task CheckForNewEpisodes(Podcast podcast)
        {
            var httpClient = new HttpClient();
            var xml = await httpClient.GetStringAsync(podcast.FeedUrl);
            var xmlDocument = XDocument.Parse(xml);

            var elements = xmlDocument.Element("rss").Element("channel").Elements("item");
            foreach (var element in elements)
            {
                var episode = new Episode(element);
                if (!podcast.Episodes.Any(e => string.Compare(e.Name, episode.Name) == 0))
                {
                    podcast.Episodes.Add(episode);
                }
            }
        }

        private static async Task<IList<Podcast>> GetPodcastsFromStorageAsync()
        {
            StorageFile file = await ApplicationData.Current.RoamingFolder.GetFileAsync(PodcastsFilename);
            if (file != null)
            {
                var podcastsJson = await FileIO.ReadTextAsync(file);
                var podcasts = JsonConvert.DeserializeObject<IList<Podcast>>(podcastsJson);
                foreach (var podcast in podcasts)
                {
                    foreach (var episode in podcast.Episodes)
                    {
                        episode.Podcast = podcast;
                    }
                }
                return podcasts;
            }

            return null;
        }

        public static async Task<IList<Episode>> GetLatestEpisodesAsync(int numberOfEpisodes)
        {
            var podcasts = await GetPodcastsFromStorageAsync();
            foreach (var podcast in podcasts)
            {
                await CheckForNewEpisodes(podcast);
            }

            IList<Episode> episodes;
            if (numberOfEpisodes != 0)
                episodes = podcasts.SelectMany(p => p.Episodes).OrderByDescending(e => e.Published).Take(numberOfEpisodes).ToList();
            else
                episodes = podcasts.SelectMany(p => p.Episodes).OrderByDescending(e => e.Published).ToList();

            return episodes;
        }

        public static async Task<IList<Podcast>> GetSubscriptions(int numberOfSubscriptions)
        {
            var podcasts = await GetPodcastsFromStorageAsync();

            IList<Podcast> result = null;
            if (numberOfSubscriptions != 0)
                result = podcasts.OrderByDescending(p => p.DateAdded).Take(numberOfSubscriptions).ToList();
            else
                result = podcasts.OrderByDescending(p => p.DateAdded).ToList();

            return result;
        }

        public static async Task<IList<Episode>> GetPodcastEpisodesFromStorageAsync(Podcast podcast)
        {
            var podcasts = await GetPodcastsFromStorageAsync();
            return podcasts.Where(p => p.Name == podcast.Name).SingleOrDefault().Episodes;
        }

        public static async Task<Podcast> LoadPodcastAsync(string url)
        {
            var podcast = await LoadPodcastAsync(new Uri(url));
            return podcast;
        }

        public static async Task<Podcast> LoadPodcastAsync(Uri url)
        {
            var httpClient = new HttpClient();
            var xml = await httpClient.GetStringAsync(url);
            var xmlDocument = XDocument.Parse(xml);

            var channelTitle = xmlDocument.Element("rss").Element("channel").Element("title").Value;
            XNamespace itunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";
            var elements = xmlDocument.Element("rss").Element("channel").Elements("item");
            var description = xmlDocument.Element("rss").Element("channel").Element("description").Value;
            var imageUrl = xmlDocument.Element("rss").Element("channel").Element(itunes + "image").Attribute("href").Value;
            var author = xmlDocument.Element("rss").Element("channel").Element(itunes + "author").Value;

            var podcast = new Podcast
            {
                FeedUrl = url,
                Name = channelTitle,
                ImageUrl = new Uri(imageUrl),
                Author = author,
                Description = description,
                DateAdded = DateTime.Now
            };

            var episodes = new List<Episode>();

            foreach (var element in elements)
            {
                var episode = new Episode(element);
                episode.Podcast = podcast;
                episodes.Add(episode);
            }
            podcast.Episodes = episodes;

            return podcast;
        }

        public static async Task SavePodcastsToLocalStorage(IList<Podcast> podcasts)
        {
            var podcastsJson = podcasts.ToJson();
            if (!string.IsNullOrEmpty(podcastsJson))
            {
                StorageFile file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(PodcastsFilename, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, podcastsJson);
            }
        }
    }
}
