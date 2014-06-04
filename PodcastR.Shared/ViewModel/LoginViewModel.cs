using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PodcastR.Interfaces;
using PodcastR.Login;
using PodcastR.Services;
using PodcastR.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PodcastR.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private IAuthenticationService service;
        private ISettingsService settingsService;
        private INavigationService navigationService;

        public LoginViewModel(IAuthenticationService service, ISettingsService settingsService, INavigationService navigationService)
        {
            this.service = service;
            this.settingsService = settingsService;
            this.navigationService = navigationService;
            this.Account = new LoginAccount();
            this.LoginCommand = new RelayCommand(async () => await LoginAsync());
            this.RegisterCommand = new RelayCommand(() => Register());
        }

        public ICommand LoginCommand { get; private set; }
        public ICommand RegisterCommand { get; private set; }
        public LoginAccount Account { get; set; }

        private async Task LoginAsync()
        {
            var token = await service.LoginAsync(this.Account.Username, this.Account.Password);
            if (token != null)
            {
                settingsService.AccessToken = token.AccessToken;
                settingsService.ExpiresAt = token.ExpiresAt;
                settingsService.Username = this.Account.Username;
                settingsService.Password = this.Account.Password;

                navigationService.Navigate(typeof(HubPage));
                
            }
        }

        private void Register()
        {
            navigationService.Navigate(typeof(RegisterPage));
        }
    }
}
