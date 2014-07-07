using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastR.Login
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(".expires")]
        public DateTime ExpiresAt { get; set; }
    }
}
