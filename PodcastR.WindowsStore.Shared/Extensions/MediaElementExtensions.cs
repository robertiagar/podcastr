using Microsoft.Practices.ServiceLocation;
using PodcastR.Core.Entities;
using PodcastR.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PodcastR.Extensions
{
    public static class MediaElementExtensions
    {
        private static MainViewModel _viewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
        private static DispatcherTimer timer;
        private static int absvalue;

        private static void SetupTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
        }

        private static void timer_Tick(object sender, object e)
        {
            if (_viewModel != null && _viewModel.NowPlaying != null)
            {
                absvalue = (int)Math.Round(App.Player.NaturalDuration.TimeSpan.TotalSeconds, MidpointRounding.AwayFromZero);
                _viewModel.NowPlaying.ElapsedTime = TimeSpan.FromSeconds(App.Player.Position.TotalSeconds);
                _viewModel.NowPlaying.TotalTime = TimeSpan.FromSeconds(absvalue);
            }
        }

        public static void Play(this MediaElement element, EpisodeViewModel episode)
        {
            if (App.Playlist == null)
                App.Playlist = new[] { episode };
            if (!App.Playlist.Contains(episode))
                App.Playlist.Add(episode);
            Play(episode);
        }

        private static void Play(EpisodeViewModel episode)
        {
            foreach (var ep in App.Playlist)
            {
                ep.PlayCommand.Label = "Play";
                ep.PlayCommand.Symbol = Symbol.Play;
            }
            if (episode.Episode.Path.StartsWith("http://") ||
                episode.Episode.Path.StartsWith("https://"))
                PlayFromNetwork(episode);
            else
                PlayLocalEpisode(episode);

            SetupTimer();
            timer.Start();
        }

        private static async void PlayLocalEpisode(EpisodeViewModel episode)
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(episode.Episode.Path);
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                App.Player.SetSource(stream, file.ContentType);
                App.UpdateSystemControls(episode.Episode);
                _viewModel.NowPlaying.Episode = episode;
                _viewModel.NowPlaying.Episode.PlayCommand.Symbol = Symbol.Pause;
                _viewModel.NowPlaying.Episode.PlayCommand.Label = "Pause";
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
            _viewModel.NowPlaying.Episode = episode;
            _viewModel.NowPlaying.Episode.PlayCommand.Symbol = Symbol.Pause;
            _viewModel.NowPlaying.Episode.PlayCommand.Label = "Pause";
        }
    }
}
