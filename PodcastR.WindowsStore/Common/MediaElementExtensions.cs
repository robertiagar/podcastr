using Microsoft.Practices.ServiceLocation;
using PodcastR.Data.Entities;
using PodcastR.WindowsStore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace PodcastR.WindowsStore.Common
{
    public static class MediaElementExtensions
    {
        public static MainViewModel _viewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
        public static void Play(this MediaElement element, EpisodeViewModel episode)
        {
            if (App.Playlist == null)
                App.Playlist = new[] { episode };
            if (!App.Playlist.Contains(episode))
                App.Playlist.Add(episode);
            Play(episode);
        }

        public static void Play(this MediaElement element, IEnumerable<EpisodeViewModel> episodes)
        {
            if (App.Playlist == null)
                App.Playlist = episodes.ToList();
            var episode = App.Playlist[0];
            Play(episode);
        }

        private static void Play(EpisodeViewModel episode)
        {
            if (episode.Episode.Path.StartsWith("http://") ||
                episode.Episode.Path.StartsWith("https://"))
                PlayFromNetwork(episode);
            else
                PlayLocalEpisode(episode);
        }        

        private static async void PlayLocalEpisode(EpisodeViewModel episode)
        {
            try
            {
                var file  = await StorageFile.GetFileFromPathAsync(episode.Episode.Path);
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                App.Player.SetSource(stream, file.ContentType);
                App.UpdateSystemControls(episode.Episode);
                _viewModel.NowPlaying = episode.Episode;
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    ;
                }
            }
        }

        private static void PlayFromNetwork(EpisodeViewModel episode)
        {
            App.Player.Source = new Uri(episode.Episode.Path);
            App.Position = App.Playlist.IndexOf(episode);
            App.Player.Play();
            App.UpdateSystemControls(episode.Episode);
            _viewModel.NowPlaying = episode.Episode;
        }
    }
}
