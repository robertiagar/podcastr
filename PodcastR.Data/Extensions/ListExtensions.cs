using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastR.Data.Extensions
{
    public static class ListExtensions
    {
        public static string ToJson<T>(this IList<T> list)
        {
            return JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.None);
        }
    }
}
