using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Xaml.Data;

namespace PodcastR.Converters
{
    public class BackgroundTransferStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var status = (BackgroundTransferStatus)value;
            switch (status)
            {
                case BackgroundTransferStatus.Canceled:
                    return "Download canceled.";
                case BackgroundTransferStatus.Completed:
                    return "Download completed.";
                case BackgroundTransferStatus.Error:
                    return "Donwload error.";
                case BackgroundTransferStatus.Idle:
                    return "Download idle.";
                case BackgroundTransferStatus.PausedByApplication:
                    return "Download paused.";
                case BackgroundTransferStatus.PausedCostedNetwork:
                    return "Download paused.";
                case BackgroundTransferStatus.PausedNoNetwork:
                    return "Download paused.";
                case BackgroundTransferStatus.Running:
                    return "Downloading.";
                default:
                    return "Unknown state.";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
