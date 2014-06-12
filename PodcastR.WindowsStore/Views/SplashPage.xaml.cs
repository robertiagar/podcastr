using GalaSoft.MvvmLight.Ioc;
using PodcastR.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PodcastR.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashPage : Page
    {
        private ISettingsService settings;
        private IAuthenticationService authentication;
        private INavigationService navigation;
        public SplashPage()
        {
            navigation = SimpleIoc.Default.GetInstance<INavigationService>();
            authentication = SimpleIoc.Default.GetInstance<IAuthenticationService>();
            settings = SimpleIoc.Default.GetInstance<ISettingsService>();
            settings.ClearSettings();
            this.InitializeComponent();
            Loaded +=  async (s, e) =>
            {
                if (settings.Username != null && settings.Password != null)
                {
                    if (settings.AccessToken != null)
                    {
                        if (settings.ExpiresAt > DateTime.Now)
                        {
                            navigation.Navigate(typeof(HubPage));
                        }
                        else
                        {
                            await LoginAsync();
                        }
                    }
                    else
                    {
                        await LoginAsync();
                    }
                }
                else
                {
                    navigation.Navigate(typeof(LoginPage));
                }
            };
        }

        private async Task LoginAsync()
        {
            var token = await authentication.LoginAsync(settings.Username, settings.Password);
            if (token != null)
            {
                settings.AccessToken = token.AccessToken;
                settings.ExpiresAt = token.ExpiresAt;

                navigation.Navigate(typeof(HubPage));
            }
        }
    }
}
