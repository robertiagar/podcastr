using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PodcastR.Interfaces;
using PodcastR.Login;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PodcastR.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private IAuthenticationService service;
        private INavigationService navigation;
        private ISettingsService settings;
        public RegisterViewModel(IAuthenticationService service, INavigationService navigation, ISettingsService settings)
        {
            this.service = service;
            this.navigation = navigation;
            this.settings = settings;
            Account = new RegisterAccount();

            this.RegisterCommand = new RelayCommand(async () => await RegisterAsync());
            this.GoBackCommand = new RelayCommand(() =>
                {
                    navigation.GoBack();
                });
        }

        public RegisterAccount Account { get; set; }
        public ICommand RegisterCommand { get; private set; }
        public ICommand GoBackCommand { get; private set; }

        private async Task RegisterAsync()
        {
            var account = Account;
            var registered = false;
            if (account.Password == account.ConfirmPassword)
            {
                registered = await service.RegisterAsync(account.Username, account.Password);
            }
            if (registered)
            {
                settings.Username = account.Username;
                settings.Password = account.Password;
                var token = await service.LoginAsync(account.Username, account.Password);
                if (token != null)
                {
                    settings.AccessToken = token.AccessToken;
                    settings.ExpiresAt = token.ExpiresAt;

                    navigation.Navigate(typeof(HubPage));
                }
            }
        }
    }
}
