using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using PodcastR.Core.Entities;
using PodcastR.Services;
using PodcastR.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using PodcastR.Common;

namespace PodcastR.ViewModel
{
    public class EpisodeViewModel : ViewModelBase
    {
        public EpisodeViewModel(Episode episode)
        {
            this.Episode = episode;
            this.IsLocal = episode.IsLocal;
            this.ToggleEpisodeLocationCommand = new SymbolCommand(new GalaSoft.MvvmLight.Command.RelayCommand(async () => await this.ToggleEpisodeLocationAsync()), this.IsLocal ? Symbol.Delete : Symbol.Download, this.IsLocal ? "Delete" : "Download");
            this.PlayCommand = new SymbolCommand(new GalaSoft.MvvmLight.Command.RelayCommand(() => this.Play()), Symbol.Play, "Play");
        }

        private void Play()
        {
            App.Player.Play(this);
        }

        private ulong _Percent;
        private ulong _TotalBytesToDownload;
        private ulong _BytesReceived;
        private bool _IsDownloading;
        private bool _IsLocal;
        private BackgroundTransferStatus _Status;

        public SymbolCommand ToggleEpisodeLocationCommand { get; private set; }
        public SymbolCommand PlayCommand { get; set; }

        public Episode Episode { get; set; }

        public ulong Percent
        {
            get { return _Percent; }
            set
            {
                Set<ulong>(() => Percent, ref _Percent, value);
            }
        }

        public ulong TotalBytesToReceive
        {
            get { return _TotalBytesToDownload; }
            set
            {
                Set<ulong>(() => TotalBytesToReceive, ref _TotalBytesToDownload, value);
            }
        }

        public ulong BytesReceived
        {
            get { return _BytesReceived; }
            set
            {
                Set<ulong>(() => BytesReceived, ref _BytesReceived, value);
            }
        }

        public BackgroundTransferStatus Status
        {
            get { return _Status; }
            set
            {
                Set<BackgroundTransferStatus>(() => Status, ref _Status, value);
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

        public void DownloadCallback(DownloadOperation operation)
        {
            var progress = operation.Progress;
            if (progress.TotalBytesToReceive > 0)
            {
                Percent = progress.BytesReceived * 100 / progress.TotalBytesToReceive;
                BytesReceived = progress.BytesReceived;
                TotalBytesToReceive = progress.TotalBytesToReceive;
            }
            if (Percent == 100)
            {
                IsDownloading = false;
            }
            Status = progress.Status;
        }

        public bool IsDownloading
        {
            get { return _IsDownloading; }
            set
            {
                Set<bool>(() => IsDownloading, ref _IsDownloading, value);
            }
        }

        public void ErrorCallback()
        {

        }

        private async Task ToggleEpisodeLocationAsync()
        {
            var delete = false;
            var download = true;
            var deleteDialog = new MessageDialog(string.Format("Are you sure you want to delete {0} from {1}?\n\nNote: This will delete the file from your local storage, not remove the episode from the feed.", Episode.Name, Episode.Podcast.Name));
            deleteDialog.Commands.Add(new UICommand("Ok", p =>
            {
                delete = true;
            }));
            deleteDialog.Commands.Add(new UICommand("Cancel", p =>
            {
                delete = false;
            }));
            deleteDialog.CancelCommandIndex = 1;
            deleteDialog.DefaultCommandIndex = 0;
            deleteDialog.Title = string.Format("Delete?", Episode.Name);

            var cancelDownloadDialog = new MessageDialog(string.Format("Are you sure you want to cancel the download of {0} from {1}?", Episode.Name, Episode.Podcast.Name));
            cancelDownloadDialog.Commands.Add(new UICommand("Ok", p =>
            {
                download = false;
            }));
            cancelDownloadDialog.Commands.Add(new UICommand("Cancel", p =>
            {
                download = true;
            }));
            cancelDownloadDialog.CancelCommandIndex = 1;
            cancelDownloadDialog.DefaultCommandIndex = 0;
            cancelDownloadDialog.Title = string.Format("Cancel?", Episode.Name);

            if (this.Episode.IsLocal)
            {
                await deleteDialog.ShowAsync();
                if (delete)
                {
                    await PodcastDownloaderService.DeletePodcastEpisodeAsync(this.Episode);
                }
            }

            if (!this.Episode.IsLocal)
            {
                if (this.IsDownloading)
                {
                    await cancelDownloadDialog.ShowAsync();
                }

                if (download)
                {
                    if (!this.IsDownloading)
                    {
                        this.IsDownloading = true;
                        this.ToggleEpisodeLocationCommand.Symbol = Symbol.Cancel;
                        this.ToggleEpisodeLocationCommand.Label = "Cancel";
                        ServiceLocator.Current.GetInstance<MainViewModel>().Downloads.Add(this);
                        await PodcastDownloaderService.DownloadPodcastEpisodeAsync(this.Episode,
                            this.DownloadCallback,
                            this.ErrorCallback);
                        this.IsDownloading = false;
                        this.IsLocal = Episode.IsLocal;
                        this.ToggleEpisodeLocationCommand.Label = Episode.IsLocal ? "Delete" : "Download";
                        this.ToggleEpisodeLocationCommand.Symbol = Episode.IsLocal ? Symbol.Delete : Symbol.Download;
                        this.Percent = 0;
                        await ServiceLocator.Current.GetInstance<MainViewModel>().SavePodcastsAsync();
                        ServiceLocator.Current.GetInstance<MainViewModel>().Downloads.Remove(this);
                    }
                }
                else
                {
                    PodcastDownloaderService.CancelDownload(this.Episode);
                }
            }
        }
    }
}
