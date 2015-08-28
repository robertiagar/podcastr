using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PodcastR.ApiCore.Entities;
using PodcastR.WebApi.Infrastructure;
using PodcastR.ApiCore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;

namespace PodcastR.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api")]
    public class PodcastsController : BaseApiController
    {
        /// <summary>
        /// Gets the current users Podcasts.
        /// </summary>
        /// <returns>A collection of <see cref="Podcast">Podcasts</see>.</returns>
        public async Task<IEnumerable<Podcast>> Get()
        {
            var user = (await UserManager.FindByIdAsync(User.Identity.GetUserId()));

            return user.Podcasts.ToList();
        }

        public async Task<Podcast> Get(int id)
        {
            return await DbContext.Podcasts.Where(p => p.Id == id).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Podcast>> Get(string name)
        {
            return await DbContext.Podcasts.Where(p => p.Name.ToLower().Contains(name.ToLower())).ToListAsync();
        }

        [Route("LatestEpisodes")]
        public async Task<IEnumerable<Episode>> GetLatestEpisodes(int podcastId, DateTime lastEpisodeDate)
        {
            var podcast = await DbContext.Podcasts.Where(p => p.Id == podcastId).SingleOrDefaultAsync();
            return podcast.Episodes.Where(e => e.Published > lastEpisodeDate).ToList();
        }

        public async Task<IHttpActionResult> Post([FromBody]string podcastUrl)
        {
            if (string.IsNullOrEmpty(podcastUrl))
            {
                return BadRequest("PodcastUrl cannot be empty.");
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return null;
            }
            try
            {
                var httpClient = new HttpClient();
                var xml = await httpClient.GetStringAsync(podcastUrl);
                var xmlDocument = XDocument.Parse(xml);

                var channelTitle = xmlDocument.Element("rss").Element("channel").Element("title").Value;
                XNamespace itunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";
                var elements = xmlDocument.Element("rss").Element("channel").Elements("item");
                var description = xmlDocument.Element("rss").Element("channel").Element("description").Value;
                var imageUrl = xmlDocument.Element("rss").Element("channel").Element(itunes + "image").Attribute("href").Value;
                var author = xmlDocument.Element("rss").Element("channel").Element(itunes + "author").Value;

                var podcast = new Podcast
                {
                    FeedUrl = podcastUrl,
                    Name = channelTitle,
                    ImageUrl = imageUrl,
                    Author = author,
                    Description = description,
                    DateAdded = DateTime.Now,
                };

                var existingPodcast = DbContext.Podcasts.Where(p => p.Name == podcast.Name && p.FeedUrl == podcast.FeedUrl).SingleOrDefault();

                if (existingPodcast == null)
                {

                    var episodes = new List<Episode>();

                    foreach (var element in elements)
                    {
                        var episode = new Episode(element, podcast);
                        episodes.Add(episode);
                    }
                    podcast.Episodes = episodes.OrderByDescending(episode => episode.Published).ToList();

                    DbContext.Podcasts.Add(podcast);
                    var x = await DbContext.SaveChangesAsync();

                    user.Podcasts.Add(podcast);
                    var y = await UserManager.UpdateAsync(user);

                    if (x != 0 && y.Succeeded)
                    {
                        return Json<Podcast>(podcast);
                    }
                    else
                    {
                        return BadRequest("Error saving podcast");
                    }
                }
                else
                {
                    user.Podcasts.Add(existingPodcast);

                    var y = await UserManager.UpdateAsync(user);
                    if (y.Succeeded)
                    {
                        return Json<Podcast>(existingPodcast);
                    }
                    else
                    {
                        return BadRequest("Error saving podcast");
                    }
                }
            }
            catch
            {
                return InternalServerError(new ApplicationException("Error processing podcast url."));
            }
        }
    }
}
