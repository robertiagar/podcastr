using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastR.Interfaces
{
    public interface ISettingsService
    {
        string AccessToken { get; set; }
        DateTime ExpiresAt { get; set; }
        string Username { get; set; }
        string Password { get; set; }

        void ClearSettings();
    }
}
