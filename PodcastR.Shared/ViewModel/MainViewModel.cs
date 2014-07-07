using GalaSoft.MvvmLight;
using PodcastR.Core.Entities;
using PodcastR.Core.Extensions;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
#if WINDOWS_APP
using PodcastR.Flyouts;
#endif
using PodcastR.Services;
using PodcastR.Interfaces;


namespace PodcastR.ViewModel
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
        private ObservableCollection<PodcastViewModel> _podcasts;
        private ObservableCollection<EpisodeViewModel> _episodes;
        private ObservableCollection<EpisodeViewModel> _downloads;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IPodcastsService service, INotificationService notificationService)
        {
            this.podcastsService = service;
            this.notificationService = notificationService;
            this._podcasts = new ObservableCollection<PodcastViewModel>();
            this._episodes = new ObservableCollection<EpisodeViewModel>();
            this._downloads = new ObservableCollection<EpisodeViewModel>();
            this._NowPlaying = new NowPlayingViewModel();
            this.AddPodcastCommand = new RelayCommand(async () => await AddPodcastAsync());
            this.AddToPlaylistCommand = new RelayCommand(() => AddToPlaylist(), () => CanAddToPlaylist());
            this.AddToDownloadsCommand = new RelayCommand(async () => await AddToDownloads(), () => CanAddToDownloadlist());
#if WINDOWS_APP
            this.ShowPlaylistFlyoutCommand = new RelayCommand(() => ShowPlaylist());
            this.ShowDownloadsFlyoutCommand = new RelayCommand(() => ShowDownloads());
#endif
        }

        public IList<PodcastViewModel> Podcasts
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

        public IList<EpisodeViewModel> Playlist
        {
            get { return App.Playlist; }
        }

        public ICommand AddPodcastCommand { get; private set; }
        public ICommand AddToPlaylistCommand { get; private set; }
        public ICommand AddToDownloadsCommand { get; private set; }
        public ICommand ShowPlaylistFlyoutCommand { get; private set; }
        public ICommand ShowDownloadsFlyoutCommand { get; private set; }

        public async Task LoadPodcastsAsync()
        {
            //allPodcasts = (await PodcastService.GetSubscriptions(0)).Select(p => new PodcastViewModel(p)).ToList();
            allPodcasts = (await podcastsService.GetPodcastsAsync()).Select(p => new PodcastViewModel(p)).ToList();
            if (allPodcasts != null)
            {
                var allEpisodes = allPodcasts.SelectMany(p => p.Episodes).OrderByDescending(e => e.Episode.Published).Take(12).ToList();

                foreach (var podcast in allPodcasts.Take(6))
                {
                    _podcasts.Add(podcast);
                }

                foreach (var episode in allEpisodes.Take(12))
                {
                    _episodes.Add(episode);
                }
            }

            await notificationService.RegisterNotificationsAsync();
        }

        public async Task LoadNewEpisodesAsync()
        {
            int newEpisodesCount = 0;
            var allEpisodes = allPodcasts.SelectMany(p => p.Episodes).ToList();
            var episodes = allEpisodes.ToList();
            var newEpisodes = await PodcastService.CheckForNewEpisodes(allPodcasts.Select(p => p.Podcast));
            episodes.AddRange(from episode in newEpisodes
                              select new EpisodeViewModel(episode));
            foreach (var episode in episodes)
            {
                if (!allEpisodes.Contains(episode))
                {
                    allEpisodes.Add(episode);
                    newEpisodesCount++;
                }
            }
            allEpisodes = allEpisodes.OrderByDescending(e => e.Episode.Published).ToList();
            var episodesToAdd = allEpisodes.Take(newEpisodesCount).ToList();
            foreach (var episode in episodesToAdd)
            {
                _episodes.Insert(episodesToAdd.Count - newEpisodesCount, episode);
                newEpisodesCount--;
            }
            await SavePodcastsAsync();
        }

        public async Task AddPodcastAsync()
        {
            var podcast = await podcastsService.GetPodcastAsync(FeedUri);
            if (podcast != null)
            {
                var podcastVm = new PodcastViewModel(podcast);
                allPodcasts.Add(podcastVm);
                _podcasts.Add(podcastVm);
                var allEpisodes = allPodcasts.SelectMany(p => p.Episodes).ToList();

                foreach (var episode in podcast.Episodes.OrderBy(e => e.Published).ToList())
                {
                    allEpisodes.Add(new EpisodeViewModel(episode));
                }
                allEpisodes = allEpisodes.OrderByDescending(e => e.Episode.Published).ToList();

                var episodesNotDisplayed = allEpisodes.Take(12).Except(_episodes).ToList();
                var newEpisodesCount = episodesNotDisplayed.Count;

                foreach (var episode in episodesNotDisplayed)
                {
                    _episodes.Insert(episodesNotDisplayed.Count - newEpisodesCount, episode);
                    newEpisodesCount--;
                }

                await SavePodcastsAsync();
            }
        }

        public async Task SavePodcastsAsync()
        {
            await PodcastService.SavePodcastsToLocalStorage(allPodcasts.Select(p => p.Podcast).ToList());
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
                if (SelectedEpisode != null)
                {
                    IsOpen = true;
                }
                else
                {
                    IsOpen = false;
                }
            }
        }

        private NowPlayingViewModel _NowPlaying;
        private IList<PodcastViewModel> allPodcasts;
        private IPodcastsService podcastsService;
        private INotificationService notificationService;

        public NowPlayingViewModel NowPlaying
        {
            get { return _NowPlaying; }
            set
            {
                Set<NowPlayingViewModel>(() => NowPlaying, ref _NowPlaying, value);
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
                SelectedEpisode.IsDownloading = true;
                _downloads.Add(SelectedEpisode);
                await PodcastDownloaderService.DownloadPodcastEpisodeAsync(
                    SelectedEpisode.Episode,
                    SelectedEpisode.DownloadCallback,
                    errorCallback: SelectedEpisode.ErrorCallback);
                await SavePodcastsAsync();
            }
        }

        public bool CanAddToDownloadlist()
        {
            return SelectedEpisode != null;
        }

#if WINDOWS_APP
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
#endif
        public IList<PodcastViewModel> AllPodcasts
        {
            get { return allPodcasts; }
        }
    }
}