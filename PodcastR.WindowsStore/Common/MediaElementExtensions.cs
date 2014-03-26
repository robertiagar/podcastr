using Microsoft.Practices.ServiceLocation;
using PodcastR.Data.Entities;
using PodcastR.WindowsStore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace PodcastR.WindowsStore.Common
{
    public static class MediaElementExtensions
    {
        public static MainViewModel _viewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
        public static void Play(this MediaElement element, Episode episode)
        {
            if (App.Playlist == null)
                App.Playlist = new[] { episode };
            if (!App.Playlist.Contains(episode))
                App.Playlist.Add(episode);
            App.Player.Source = episode.Path;
            App.Position = App.Playlist.IndexOf(episode);
            App.Player.Play();
            App.UpdateSystemControls(episode);
            if (_viewModel != null)
            {
                _viewModel.NowPlaying = episode;
            }
        }

        public static void Play(this MediaElement element, Episode episode, MainViewModel viewModel)
        {
            _viewModel = viewModel;
            if (App.Playlist == null)
                App.Playlist = new[] { episode };
            if (!App.Playlist.Contains(episode))
                App.Playlist.Add(episode);
            App.Player.Source = episode.Path;
            App.Player.Play();
            App.Position = App.Playlist.IndexOf(episode);
            App.UpdateSystemControls(episode);
            _viewModel.NowPlaying = episode;
        }

        public static void Play(this MediaElement element, IEnumerable<Episode> episodes, MainViewModel viewModel)
        {
            _viewModel = viewModel;
            if (App.Playlist == null)
                App.Playlist = episodes.ToList();
            var episode = App.Playlist[0];
            App.Position = 0;
            App.Player.Source = episode.Path;
            App.Player.Play();
            App.UpdateSystemControls(episode);
            _viewModel.NowPlaying = episode;
        }
    }
}
