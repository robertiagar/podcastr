using PodcastR.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Security.Credentials;
using Windows.Storage;

namespace PodcastR.Services
{
    public class SettingsService : ISettingsService
    {
        #region strings
        private readonly string ACCESS_TOKEN = "AccessToken";
        private readonly string EXPIRES_AT = "ExpiresAt";
        private readonly string USERNAME = "Username";
        private readonly string PASSWORD = "Password";
        #endregion

        private ApplicationDataContainer _localSettings;
        private PasswordVault _vault;

        public SettingsService()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
            _vault = new PasswordVault();
        }

        public string AccessToken
        {
            get
            {
                return _localSettings.Values[ACCESS_TOKEN] as string;
            }
            set
            {
                _localSettings.Values[ACCESS_TOKEN] = value;
            }
        }

        public DateTime ExpiresAt
        {
            get
            {
                return DateTime.Parse(_localSettings.Values[EXPIRES_AT].ToString());
            }
            set
            {
                _localSettings.Values[EXPIRES_AT] = value.ToString();
            }
        }

        public string Username
        {
            get
            {
                return _localSettings.Values[USERNAME] as string;
            }
            set
            {
                _localSettings.Values[USERNAME] = value;
            }
        }

        public string Password
        {
            get
            {
                return _vault.Retrieve(PASSWORD, _localSettings.Values[USERNAME] as string).Password;
            }
            set
            {
                _vault.Add(new PasswordCredential
                {
                    Password = value,
                    Resource = PASSWORD,
                    UserName = _localSettings.Values[USERNAME] as string
                });
            }
        }


        public void ClearSettings()
        {
            _localSettings.Values.Clear();
        }
    }
}
