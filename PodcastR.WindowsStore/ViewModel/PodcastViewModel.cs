using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastR.WindowsStore.ViewModel
{
    public class PodcastViewModel : ViewModelBase
    {
        private string _Name;
        private Uri _ImageUrl;
        private ObservableCollection<EpisodeViewModel> _episodes;

        public PodcastViewModel()
        {
            _episodes = new ObservableCollection<EpisodeViewModel>();
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                Set<string>(() => Name, ref _Name, value);
            }
        }

        public Uri ImageUrl
        {
            get { return _ImageUrl; }
            set
            {
                Set<Uri>(() => ImageUrl, ref _ImageUrl, value);
            }
        }

        public IList<EpisodeViewModel> Episodes
        {
            get { return _episodes; }
        }
    }
}
