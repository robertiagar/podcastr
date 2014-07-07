using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PodcastR.Interfaces
{
    public interface INotificationService
    {
        Task RegisterNotificationsAsync();

        Task RegisterNotificationsForPodcastAsync(IEnumerable<int> podcastIds);
    }
}
