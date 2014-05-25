using Newtonsoft.Json;
using PodcastR.Interfaces;
using PodcastR.Login;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.Web.Http;

namespace PodcastR.WebServices
{
    public class PodcastRWebClient
    {
        #region Configuration strings
        private static readonly string API_URL = "http://localhost:13754/";
        private static readonly string ACCESS_TOKEN_KEY = "AccessToken";
        private static readonly string ACCESS_TOKEN_EXPIRES_KEY = "Expires";
        #endregion

        private HttpClient _client;
        private PasswordVault _vault;
        private ISettingsService _settingsService;
        public PodcastRWebClient(ISettingsService settingsService)
        {
            _client = new HttpClient();
            _vault = new PasswordVault();
            _settingsService = settingsService;
        }

        public async Task LoginAsync(string username, string password)
        {
            var account = new Account()
            {
                Username = username,
                Password = password
            };

            _settingsService.SaveAccountDetails(account);

            var content = new HttpStringContent(string.Format("grant_type=password&username={0}&password={1}", username, password));

            var response = await _client.PostAsync(new Uri(API_URL + "token"),content);

            if (response.StatusCode == HttpStatusCode.Ok)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var token = JsonConvert.DeserializeObject<Token>(responseString);

                _settingsService.SaveTokenDetails(token);
            }
        }
    }
}
