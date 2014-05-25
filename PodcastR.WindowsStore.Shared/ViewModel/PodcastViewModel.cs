using GalaSoft.MvvmLight;
using PodcastR.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastR.ViewModel
{
    public class PodcastViewModel : ViewModelBase
    {
        private ObservableCollection<EpisodeViewModel> _episodes;
        private Podcast _Podcast;
        public PodcastViewModel(Podcast podcast)
        {
            this.Podcast = podcast;
            this._episodes = new ObservableCollection<EpisodeViewModel>();
            var episodes = podcast.Episodes.Select(ep => new EpisodeViewModel(ep));
            foreach (var episode in episodes)
            {
                _episodes.Add(episode);
            }
        }

        public Podcast Podcast
        {
            get { return _Podcast; }
            set
            {
                Set<Podcast>(() => Podcast, ref _Podcast, value);
            }
        }

        public IList<EpisodeViewModel> Episodes
        {
            get { return _episodes; }
        }
    }
}
