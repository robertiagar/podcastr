using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PodcastR.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PodcastR.WindowsStore.ViewModel
{
    public class NowPlayingViewModel : ViewModelBase
    {
        private bool _IsPlaying;
        private EpisodeViewModel _Episode;
        private TimeSpan _ElapsedTime;
        private TimeSpan _TotalTime;

        public NowPlayingViewModel()
        {
            this.PlayCommand = new RelayCommand(() => this.Play(), () => this.CanPlay());
            if (App.Player != null)
                this.IsPlaying = App.Player.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing;
        }

        public bool IsPlaying
        {
            get { return _IsPlaying; }
            set
            {
                Set<bool>(() => IsPlaying, ref _IsPlaying, value);
            }
        }

        public ICommand PlayCommand { get; private set; }

        public EpisodeViewModel Episode
        {
            get { return _Episode; }
            set
            {
                Set<EpisodeViewModel>(() => Episode, ref _Episode, value);
                ((RelayCommand)PlayCommand).RaiseCanExecuteChanged();
            }
        }

        public TimeSpan ElapsedTime
        {
            get { return _ElapsedTime; }
            set
            {
                Set<TimeSpan>(() => ElapsedTime, ref _ElapsedTime, value);
            }
        }

        public TimeSpan TotalTime
        {
            get { return _TotalTime; }
            set
            {
                Set<TimeSpan>(() => TotalTime, ref _TotalTime, value);
            }
        }

        private void Play()
        {
            if (App.Player.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
            {
                App.Player.Pause();
                IsPlaying = false;
            }
            else
            {
                App.Player.Play();
                IsPlaying = true;
            }
        }

        private bool CanPlay()
        {
            return Episode != null;
        }
    }
}
