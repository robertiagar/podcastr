using GalaSoft.MvvmLight;
using PodcastR.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;

namespace PodcastR.WindowsStore.ViewModel
{
    public class EpisodeViewModel : ViewModelBase
    {
        public EpisodeViewModel(Episode episode)
        {
            this.Episode = episode;
            this.CancellationTokenSource = new CancellationTokenSource();
        }

        private ulong _Percent;
        private ulong _TotalBytesToDownload;
        private ulong _BytesReceived;
        private BackgroundTransferStatus _Status;

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

        public void DownloadCallback(DownloadOperation operation)
        {
            var progress = operation.Progress;
            if (progress.TotalBytesToReceive > 0)
            {
                Percent = progress.BytesReceived * 100 / progress.TotalBytesToReceive;
                BytesReceived = progress.BytesReceived;
                TotalBytesToReceive = progress.TotalBytesToReceive;
            }
            Status = progress.Status;
        }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        internal void ErrorCallback()
        {
            throw new NotImplementedException();
        }
    }
}
