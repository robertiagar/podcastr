using PodcastR.Login;
using System;
using System.Threading.Tasks;

namespace PodcastR.Interfaces
{
    public interface IAuthenticationService
    {
        Task<TokenResponse> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string password);
    }
}
