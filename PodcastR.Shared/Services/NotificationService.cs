using Microsoft.WindowsAzure.Messaging;
using PodcastR.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Networking.PushNotifications;

namespace PodcastR.Services
{
    public class NotificationService : INotificationService
    {
        private NotificationHub hub;
        private string username;

        public NotificationService(ISettingsService settings)
        {
            hub = new NotificationHub("podcastr", "Endpoint=sb://podcastr-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=ink/oq3X+ZMDTu29EWjRHp+U2AtmgNP4IQP+DiXQhOY=");
            username = settings.Username;
        }

        public async Task RegisterNotificationsAsync()
        {
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            var template = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text01);
            var text = template.GetElementsByTagName("text");

            text[0].AppendChild(template.CreateTextNode("$(Test_text)"));

            await hub.RegisterTemplateAsync(channel.Uri, template, "test", new[] { username });
        }

        public async Task RegisterNotificationsForPodcastAsync(IEnumerable<int> podcastIds)
        {
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            var podcasts = podcastIds.Select(i => i.ToString());
            var template = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text01);
            var text = template.GetElementsByTagName("text");

            text[0].AppendChild(template.CreateTextNode("$Test_text"));

            await hub.RegisterTemplateAsync(channel.Uri, template, "test", new[] { username });
        }
    }
}
