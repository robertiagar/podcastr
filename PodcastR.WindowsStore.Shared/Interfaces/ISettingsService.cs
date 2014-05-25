using PodcastR.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastR.Interfaces
{
    public interface ISettingsService
    {
        void SaveTokenDetails(Token token);
        void SaveAccountDetails(Account account);
    }
}
