using Microsoft.ServiceBus.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastR.WebJobs
{
    class Program
    {
        static void Main(string[] args)
        {
            var hub = NotificationHubClient.CreateClientFromConnectionString(
                "Endpoint=sb://podcastr-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=9uwYunpSW4DaBAO5/Q6RJmBjE24rmh8oiMdr2rIUVwY=",
                "podcastr");

            var notification = new Dictionary<string, string>(){
                {"Test_text","This is just a test!"}
            };

            hub.SendTemplateNotificationAsync(notification).Wait();
        }
    }
}
