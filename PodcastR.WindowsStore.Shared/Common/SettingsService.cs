using PodcastR.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Windows.Security.Credentials;
using Windows.Storage;
using PodcastR.Login;

namespace PodcastR.Common
{
    public class SettingsService : ISettingsService
    {
        private static readonly string USERNAME_KEY = "Username";
        private static readonly string TOKEN_KEY = "Token";
        private static readonly string PASSWORD_KEY = "Password";
        private static readonly string RESOURCE = "PodcastR.Password";

        public SettingsService()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
            _vault = new PasswordVault();
        }

        private ApplicationDataContainer _localSettings;
        private PasswordVault _vault;

        public void SaveTokenDetails(Token token)
        {
            _localSettings.Values[USERNAME_KEY] = token.UserName;
            _localSettings.Values[TOKEN_KEY] = token.AccessToken;
        }

        public void SaveAccountDetails(Account account)
        {
            _localSettings.Values[USERNAME_KEY] = account.Username;
            var existingCredentials = _vault.FindAllByUserName(account.Username);
            if (!existingCredentials.Any())
            {
                _vault.Add(new PasswordCredential(RESOURCE, account.Username, account.Password));
            }
        }
    }
}
