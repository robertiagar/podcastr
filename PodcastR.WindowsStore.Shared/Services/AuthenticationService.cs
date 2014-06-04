using Newtonsoft.Json;
using PodcastR.Common;
using PodcastR.Interfaces;
using PodcastR.Login;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace PodcastR.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private static HttpClient _client;
        private ISettingsService settings;

        public AuthenticationService(ISettingsService settings)
        {
            _client = new HttpClient();
            this.settings = settings;
        }

        public async Task<TokenResponse> LoginAsync(string username, string password)
        {
            var values = new Dictionary<string, string>();
            values.Add("grant_type", "password");
            values.Add("username", username);
            values.Add("password", password);

            var form = new HttpFormUrlEncodedContent(values);

            var response = await _client.PostAsync(new Uri(Constants.ApplicationUri + "token"), form);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();

                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseJson);

                return tokenResponse;
            }

            return null;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            var values = new
            {
                Email = username,
                Password = password,
                ConfirmPassword = password
            };

            var valuesJson = JsonConvert.SerializeObject(values);

            var content = new HttpStringContent(valuesJson, UnicodeEncoding.Utf8, "application/json");

            var response = await _client.PostAsync(new Uri(Constants.ApplicationUri + "api/Account/Register"), content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}
