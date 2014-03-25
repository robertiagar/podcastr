using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> list)
        {
            var observable = new ObservableCollection<T>();
            foreach (var item in list)
                observable.Add(item);

            return observable;
        }
    }
}
