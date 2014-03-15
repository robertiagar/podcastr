using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastR.WindowsStore.ViewModel
{
    public class EpisodeViewModel : ViewModelBase
    {
        private string _Name;
        private string _Author;
        private Uri _Path;
        private Uri _PodcastImageUrl;
        private string _PodcastName;
        private bool _IsLocal;

        public string Name
        {
            get { return _Name; }
            set
            {
                Set<string>(() => Name, ref _Name, value);
            }
        }

        public string Author
        {
            get { return _Author; }
            set
            {
                Set<string>(() => Author, ref _Author, value);
            }
        }

        public Uri Path
        {
            get { return _Path; }
            set
            {
                Set<Uri>(() => Path, ref _Path, value);
            }
        }

        public string PodcastName
        {
            get { return _PodcastName; }
            set
            {
                Set<string>(() => PodcastName, ref _PodcastName, value);
            }
        }

        public Uri PodcastImageUrl
        {
            get { return _PodcastImageUrl; }
            set
            {
                Set<Uri>(() => PodcastImageUrl, ref _PodcastImageUrl, value);
            }
        }

        public bool IsLocal
        {
            get { return _IsLocal; }
            set
            {
                Set<bool>(() => IsLocal, ref _IsLocal, value);
            }
        }
    }
}
