using Microsoft.ServiceBus.Notifications;
using PodcastR.ApiCore.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace PodcastR.WebJobs
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = ApplicationDbContext.Create();

            var users = context.Users.ToList();

            var hub = NotificationHubClient.CreateClientFromConnectionString(
                "Endpoint=sb://podcastr-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=9uwYunpSW4DaBAO5/Q6RJmBjE24rmh8oiMdr2rIUVwY=",
                "podcastr");

            while (true)
            {
                foreach (var user in users)
                {
                    var list = user.Podcasts.ToList();

                    Console.Write(JsonConvert.SerializeObject(list[0]));

                    var notification = new Dictionary<string, string>(){
                        {"Test_text", list[0].Name}
                    };

                    hub.SendTemplateNotificationAsync(notification, user.Email).Wait();
                }

                Thread.Sleep(new TimeSpan(0, 60, 0));
            }
        }
    }
}
