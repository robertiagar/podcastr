using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PodcastR.Core.Extensions
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

        public static string RemoveIllegalPathChars(this string input)
        {
            string illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return input.Replace(illegal, "");
        }
    }
}
