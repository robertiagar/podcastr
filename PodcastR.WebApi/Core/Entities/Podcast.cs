using Newtonsoft.Json;
using PodcastR.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace PodcastR.WebApi.Core.Entities
{
    public class Podcast
    {
        public Podcast()
        {
            this.Episodes = new List<Episode>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string FeedUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual IList<Episode> Episodes { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
    }
}