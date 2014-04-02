using GalaSoft.MvvmLight;
using PodcastR.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PodcastR.WindowsStore.ViewModel
{
    public class NowPlayingViewModel : ViewModelBase
    {
        private Episode _NowPlaying;
        public Episode Episode
        {
            get { return _NowPlaying; }
            set
            {
                Set<Episode>(() => Episode, ref _NowPlaying, value);
            }
        }

        private TimeSpan _ElapsedTime;
        public TimeSpan ElapsedTime
        {
            get { return _ElapsedTime; }
            set
            {
                Set<TimeSpan>(() => ElapsedTime, ref _ElapsedTime, value);
            }
        }

        private TimeSpan _TotalTime;
        public TimeSpan TotalTime
        {
            get { return _TotalTime; }
            set
            {
                Set<TimeSpan>(() => TotalTime, ref _TotalTime, value);
            }
        }
    }
}
