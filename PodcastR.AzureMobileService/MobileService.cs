using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace PodcastR.AzureMobileService
{
    public static class MobileService
    {
        private static MobileServiceClient m_mobileService;
        private static MobileServiceUser m_user;

        static MobileService()
        {
            m_mobileService = new Microsoft.WindowsAzure.MobileServices.MobileServiceClient("https://podcastr.azure-mobile.net/", "EwKwZVDKoqoIFjgiRhUxRPiscVfeel63");
        }

        public static async Task LoginAsync(SelectedLogin login)
        {
            switch (login)
            {
                case SelectedLogin.Facebook:
                    //await LoginFacebookAsync();
                    break;
                case SelectedLogin.Microsoft:
                    await LoginMicrosoftAsync();
                    break;
                case SelectedLogin.Twitter:
                    //await LoginTwitterAsync();
                    break;
                case SelectedLogin.Google:
                    //await LoginGoogleAsync();
                    break;
                default:
                    break;
            }
        }

        private static async Task LoginMicrosoftAsync()
        {
            while (m_user == null)
            {
                string message;
                try
                {
                    m_user = await m_mobileService.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount, true);
                    message = string.Format("You are now logged in - {0}", m_user.UserId);
                }
                catch (InvalidOperationException)
                {
                    message = "You must log in. Login Required";
                }

                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }

        public enum SelectedLogin
        {
            Facebook,
            Microsoft,
            Twitter,
            Google
        }
    }
}
