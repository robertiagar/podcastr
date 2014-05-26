using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace PodcastR.Services
{
    public static class SettingsService
    {
        #region strings
        private static readonly string ACCESS_TOKEN = "AccessToken";
        #endregion

        private static ApplicationDataContainer _localSettings;


        static SettingsService()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
        }

        public static string AccessToken
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
    }
}
