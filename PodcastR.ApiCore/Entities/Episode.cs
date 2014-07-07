using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PodcastR.ApiCore.Entities
{
    public class Episode
    {

        private static DateTime ToDateTime(string input)
        {
            var index = input.LastIndexOf(" ");
            var stringResult = input.Remove(index).Replace("Thurs", "Thu");
            var dateResult = DateTime.Parse(stringResult);
            return dateResult;
        }
        

        public Episode()
        {

        }

        public Episode(System.Xml.Linq.XElement element, Podcast podcast)
        {
            XNamespace itunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";
            Name = element.Element("title").Value;
            WebPath = element.Element("enclosure").Attribute("url").Value;
            Author = element.Element(itunes + "author").Value;
            Published = ToDateTime(element.Element("pubDate").Value);
            ImageUrl = element.Element(itunes + "image") != null ? element.Element(itunes + "image").Attribute("href").Value : podcast.ImageUrl;
            Summary = element.Element(itunes + "summary") != null ? element.Element(itunes + "summary").Value : null;
            Description = element.Element("description") != null ? element.Element("description").Value : null;
            Podcast = podcast;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string WebPath { get; set; }
        public string Author { get; set; }
        public DateTime Published { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public virtual Podcast Podcast { get; set; }
    }
}
