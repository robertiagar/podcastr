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
        public static void Play(this MediaElement element, Episode episode)
        {
            App.Playlist = new[] { episode };
            App.Player.Source = episode.Path;
            App.Player.Play();
            App.UpdateSystemControls(episode);
        }

        public static void Play(this MediaElement element,Episode episode, MainViewModel viewModel)
        {
            App.Playlist = new[] { episode };
            App.Player.Source = episode.Path;
            App.Player.Play();
            App.UpdateSystemControls(episode);
            viewModel.NowPlaying = episode;
        }
    }
}
