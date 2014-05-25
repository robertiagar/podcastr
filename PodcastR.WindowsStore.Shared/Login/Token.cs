using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastR.Login
{
    public class Token
    {
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public string UserName { get; set; }
    }
}
