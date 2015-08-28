using PodcastR.ApiCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastR.ApiCore.Comparers
{
    public class EpisodeComparer : IEqualityComparer<Episode>
    {
        public bool Equals(Episode x, Episode y)
        {
            return x.Name == y.Name && x.Author == y.Author && x.Description == y.Description;
        }

        public int GetHashCode(Episode obj)
        {
            return obj.WebPath.GetHashCode();
        }
    }
}
