using Microsoft.ServiceBus.Notifications;
using Newtonsoft.Json;
using PodcastR.ApiCore.Comparers;
using PodcastR.ApiCore.Entities;
using PodcastR.ApiCore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PodcastR.WebJobs
{
    class Program
    {
        static void Main(string[] args)
        {
            Timer timer = new Timer(async (o) => await ProccesPodcasts());
            timer.Change(new TimeSpan(0, 0, 10), new TimeSpan(0, 5, 0));

            while (true)
            {
                Thread.Sleep(new TimeSpan(0, 60, 0));
            }
        }

        private static async Task ProccesPodcasts()
        {
            var hub = NotificationHubClient.CreateClientFromConnectionString(
                "Endpoint=sb://podcastr-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=9uwYunpSW4DaBAO5/Q6RJmBjE24rmh8oiMdr2rIUVwY=",
                "podcastr");

            var context = ApplicationDbContext.Create();

            Console.WriteLine("Succesfully created db context @: {0}", DateTime.Now);

            var users = await context.Users.Include("Podcasts").ToListAsync();

            Console.WriteLine("Succesfully got {0} users @: {1}", users.Count, DateTime.Now);

            var comparer = new EpisodeComparer();

            var newEpisodes = new List<Episode>();

            foreach (var user in users)
            {
                var podcasts = user.Podcasts;

                Console.WriteLine("Succesfully got {0} podcasts for user '{1}' @: {2}", podcasts.Count, user.Email, DateTime.Now);

                var episodesForUser = await CheckForNewEpisodesAsync(podcasts);

                if (episodesForUser.Any())
                {
                    Console.WriteLine("Succesfully got {0} new episodes for @: {1}", episodesForUser.Count, DateTime.Now);

                    var toastNotification = new Dictionary<string, string>()
                    {
                        { "EpisodesCount" , episodesForUser.Count.ToString() },
                    };

                    await hub.SendTemplateNotificationAsync(toastNotification, user.Email);

                    Console.WriteLine("Succesfully sent notification to user {0}!", user.Email);

                    var intersect = newEpisodes.Intersect(episodesForUser, comparer);

                    foreach (var episode in intersect)
                    {
                        newEpisodes.Remove(episode);
                    }

                    newEpisodes.AddRange(episodesForUser);

                    Console.WriteLine("Successfuly updated {0} episodes for user {1}!", episodesForUser.Count, user.Email);
                }
                else
                {
                    Console.WriteLine("No new episodes for user {0}", user.Email);
                }
            }

            if (newEpisodes.Any())
            {
                context.Episodes.AddRange(newEpisodes);

                var x = await context.SaveChangesAsync();

                if (x != 0)
                {
                    Console.WriteLine("Succesfully added {0} new episodes for all users", x);
                }
            }
            else
            {
                Console.WriteLine("No new episodes found!");
            }

            Console.WriteLine("Succesfully processed!");
        }

        private static async Task<IList<Episode>> CheckForNewEpisodesAsync(IEnumerable<Podcast> podcasts)
        {
            var result = new List<Episode>();
            foreach (var podcast in podcasts.ToList())
            {
                try
                {
                    var httpClient = new HttpClient();
                    var xml = await httpClient.GetStringAsync(podcast.FeedUrl);
                    var xmlDocument = XDocument.Parse(xml);

                    var elements = xmlDocument.Element("rss").Element("channel").Elements("item");
                    foreach (var element in elements)
                    {
                        var episode = new Episode(element, podcast);
                        if (!podcast.Episodes.Any(e => string.Compare(e.WebPath, episode.WebPath) == 0))
                        {
                            podcast.Episodes.Insert(0, episode);
                            result.Add(episode);
                        }
                    }
                }
                catch
                {

                }
            }

            return result.OrderBy(e => e.Published).ToList();
        }
    }
}
