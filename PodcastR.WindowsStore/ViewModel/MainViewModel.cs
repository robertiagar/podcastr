using GalaSoft.MvvmLight;
using PodcastR.Data.Entities;
using PodcastR.Data.Services;
using PodcastR.Data.Extensions;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using PodcastR.WindowsStore.Flyouts;

namespace PodcastR.WindowsStore.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : StickyAppBarViewModel
    {
        private ObservableCollection<Podcast> _podcasts;
        private ObservableCollection<EpisodeViewModel> _episodes;
        private ObservableCollection<EpisodeViewModel> _downloads;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            this._podcasts = new ObservableCollection<Podcast>();
            this._episodes = new ObservableCollection<EpisodeViewModel>();
            this._downloads = new ObservableCollection<EpisodeViewModel>();
            this.AddPodcastCommand = new RelayCommand(async () => await AddPodcastAsync());
            this.ClearPodcastsCommand = new RelayCommand(async () => { ClearPodcasts(); await SavePodcastsAsync(); });
            this.AddToPlaylistCommand = new RelayCommand(() => AddToPlaylist(), () => CanAddToPlaylist());
            this.AddToDownloadsCommand = new RelayCommand(async () => await AddToDownloads(), () => CanAddToDownloadlist());
            this.ShowPlaylistFlyoutCommand = new RelayCommand(() => ShowPlaylist());
            this.ShowDownloadsFlyoutCommand = new RelayCommand(() => ShowDownloads());
        }

        public IList<Podcast> Podcasts
        {
            get { return _podcasts; }
        }

        public IList<EpisodeViewModel> Episodes
        {
            get { return _episodes; }
        }

        public IList<EpisodeViewModel> Downloads
        {
            get { return _downloads; }
        }

        public ICommand AddPodcastCommand { get; private set; }
        public ICommand ClearPodcastsCommand { get; private set; }
        public ICommand AddToPlaylistCommand { get; private set; }
        public ICommand AddToDownloadsCommand { get; private set; }
        public ICommand ShowPlaylistFlyoutCommand { get; private set; }
        public ICommand ShowDownloadsFlyoutCommand { get; private set; }

        public async Task LoadPodcastsAsync()
        {
            var podcasts = (await PodcastService.GetSubscriptions(6));
            if (podcasts != null)
            {
                var episodes = podcasts.SelectMany(p => p.Episodes).OrderByDescending(e => e.Published).Take(12);

                foreach (var podcast in podcasts)
                {
                    _podcasts.Add(podcast);
                }

                foreach (var episode in episodes)
                {
                    _episodes.Add(new EpisodeViewModel(episode));
                }
            }
        }

        public async Task LoadNewEpisodesAsync()
        {
            var episodes = _episodes.ToList();
            var newEpisodes = await PodcastService.CheckForNewEpisodes(_podcasts);
            episodes.AddRange(from episode in newEpisodes
                              select new EpisodeViewModel(episode));
            foreach (var episode in episodes.OrderBy(e => e.Episode.Published))
            {
                if (!_episodes.Contains(episode))
                    _episodes.Insert(0, episode);
            }
        }

        public async Task AddPodcastAsync()
        {
            var podcast = await PodcastService.LoadPodcastAsync(FeedUri);
            _podcasts.Add(podcast);
            foreach (var episode in podcast.Episodes.Take(5).OrderBy(e => e.Published).ToList())
            {
                int i = 0;
                while (i != _episodes.Count && _episodes[i].Episode.Published < episode.Published)
                    i++;

                _episodes.Insert(i, new EpisodeViewModel(episode));
            }
            await SavePodcastsAsync();
        }

        public void ClearPodcasts()
        {
            _podcasts.Clear();
        }

        public async Task SavePodcastsAsync()
        {
            await PodcastService.SavePodcastsToLocalStorage(_podcasts);
        }

        private string _FeedUri;
        public string FeedUri
        {
            get { return _FeedUri; }
            set
            {
                Set<string>(() => FeedUri, ref _FeedUri, value);
            }
        }

        private EpisodeViewModel _SelectedEpisode;
        public EpisodeViewModel SelectedEpisode
        {
            get { return _SelectedEpisode; }
            set
            {
                Set<EpisodeViewModel>(() => SelectedEpisode, ref _SelectedEpisode, value);
                ((RelayCommand)AddToPlaylistCommand).RaiseCanExecuteChanged();
                ((RelayCommand)AddToDownloadsCommand).RaiseCanExecuteChanged();
            }
        }

        private Episode _NowPlaying;
        public Episode NowPlaying
        {
            get { return _NowPlaying; }
            set
            {
                Set<Episode>(() => NowPlaying, ref _NowPlaying, value);
            }
        }

        private void AddToPlaylist()
        {
            if (!App.Playlist.Contains(SelectedEpisode))
            {
                App.Playlist.Add(SelectedEpisode);
                App.UpdatePreviosNextButtons();
            }
        }

        private bool CanAddToPlaylist()
        {
            return SelectedEpisode != null;
        }

        public async Task AddToDownloads()
        {
            if (!_downloads.Contains(SelectedEpisode))
            {
                _downloads.Add(SelectedEpisode);
                await PodcastDownloaderService.DownloadPodcastEpisodeAsync(
                    SelectedEpisode.Episode,
                    SelectedEpisode.DownloadCallback,
                    SelectedEpisode.CancellationTokenSource,
                    SelectedEpisode.ErrorCallback);
                await SavePodcastsAsync();
            }
        }

        public bool CanAddToDownloadlist()
        {
            return SelectedEpisode != null;
        }

        private void ShowPlaylist()
        {
            var playlistFlyout = new PlaylistFlyout();
            playlistFlyout.Width = 650;
            playlistFlyout.ShowIndependent();
        }

        private void ShowDownloads()
        {
            var downloadsFlyout = new DownloadsFlyout();
            downloadsFlyout.Width = 650;
            downloadsFlyout.ShowIndependent();
        }
    }
}