using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace PodcastR.WindowsStore.Converters
{
    public class IsLocalToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isLocal = (bool)value;
            var symbolIcon = new SymbolIcon();
            if (isLocal)
            {
                symbolIcon.Symbol = Symbol.Delete;
            }
            else
            {
                symbolIcon.Symbol = Symbol.Download;
            }
            return symbolIcon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
