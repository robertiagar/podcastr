using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PodcastR.WebApi.Core.Entities;
using PodcastR.WebApi.Models;
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
    public class PodcastsController : ApiController
    {
        private ApplicationDbContext _dbContext;
        private ApplicationUserManager _userManager;

        public PodcastsController()
        {
        }

        public PodcastsController(ApplicationDbContext context, ApplicationUserManager manager)
        {
            DbContext = context;
            UserManager = manager;
        }

        public ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext ?? Request.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _dbContext = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// Gets the current users Podcast.
        /// </summary>
        /// <returns>A collection of <see cref="Podcast">Podcasts</see>.</returns>
        public async Task<IEnumerable<Podcast>> Get()
        {
            var user = (await UserManager.FindByIdAsync(User.Identity.GetUserId()));

            return await DbContext.Podcasts.Where(p => p.User.Id == user.Id).ToListAsync();
        }

        public async Task<IHttpActionResult> Post(string podcastUrl)
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
                    return BadRequest("error saving podcast");
                }
            }
            catch
            {
                return InternalServerError(new ApplicationException("Error processing podcast url."));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager.Dispose();
                DbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
