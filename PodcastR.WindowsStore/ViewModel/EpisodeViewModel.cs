﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using PodcastR.Data.Entities;
using PodcastR.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Networking.BackgroundTransfer;

namespace PodcastR.WindowsStore.ViewModel
{
    public class EpisodeViewModel : ViewModelBase
    {
        public EpisodeViewModel(Episode episode)
        {
            this.Episode = episode;
            this.CancellationTokenSource = new CancellationTokenSource();
            this.ToggleEpsidoeLocationCommand = new RelayCommand(async () => await this.ToggleEpisodeLocationAsync());
            this.IsLocal = episode.IsLocal;
        }


        private ulong _Percent;
        private ulong _TotalBytesToDownload;
        private ulong _BytesReceived;
        private bool _IsDownloading;
        private bool _IsLocal;
        private BackgroundTransferStatus _Status;

        public ICommand ToggleEpsidoeLocationCommand { get; private set; }

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

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public void ErrorCallback()
        {

        }

        private async Task ToggleEpisodeLocationAsync()
        {
            this.IsDownloading = true;
            await PodcastDownloaderService.ToggleEpisodeLocationAsync(this.Episode, 
                DownloadCallback, 
                CancellationTokenSource, 
                ErrorCallback);
            this.IsDownloading = false;
            this.IsLocal = Episode.IsLocal;
            await ServiceLocator.Current.GetInstance<MainViewModel>().SavePodcastsAsync();
        }
    }
}
