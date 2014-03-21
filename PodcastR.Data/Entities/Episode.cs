﻿using PodcastR.Data.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PodcastR.Data.Entities
{
    public class Episode
    {
        public Episode()
        {

        }

        public Episode(System.Xml.Linq.XElement element, Podcast podcast)
        {
            XNamespace itunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";
            Name = element.Element("title").Value;
            Path = new Uri(element.Element("enclosure").Attribute("url").Value);
            Author = element.Element(itunes + "author").Value;
            IsLocal = false;
            Published = element.Element("pubDate").Value.ToDateTime();
            ImageUrl = element.Element(itunes + "image") != null ? new Uri(element.Element(itunes + "image").Attribute("href").Value) : podcast.ImageUrl;
            Podcast = podcast;
        }

        public string Name { get; set; }
        public Uri Path { get; set; }
        public bool IsLocal { get; set; }
        public string Author { get; set; }
        public DateTime Published { get; set; }
        public Uri ImageUrl { get; set; }

        [JsonIgnore]
        public virtual Podcast Podcast { get; set; }
    }
}
