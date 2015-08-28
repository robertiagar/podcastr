using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PodcastR.Interfaces
{
    public interface INavigationService
    {
        void Navigate(Type type);
        void Navigate(Type type, object parameter);
        void Navigate(string type);
        void Navigate(string type, object parameter);
        void GoBack();
    }
}
