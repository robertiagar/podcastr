using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace PodcastR.WindowsStore.Converters
{
    public class IsPlayingToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isPlaying = (bool)value;
            var icon = new SymbolIcon();
            if (isPlaying)
            {
                icon.Symbol = Symbol.Pause;
            }
            else
            {
                icon.Symbol = Symbol.Play;
            }
            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
