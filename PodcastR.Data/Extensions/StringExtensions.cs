using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastR.Data.Extensions
{
    public static class StringExtensions
    {
        public static string GetExtension(this string input)
        {
            var index = input.LastIndexOf('.');
            return input.Substring(index);
        }

        public static DateTime ToDateTime(this string input)
        {
            var index = input.LastIndexOf(" ");
            var stringResult = input.Remove(index).Replace("Thurs", "Thu");
            var dateResult = DateTime.Parse(stringResult);
            return dateResult;
        }
    }
}
