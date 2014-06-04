using PodcastR.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Background;

namespace PodcastR.Services
{
    public class NotificationService : INotificationService
    {
        public void RegisterBackgroundTask()
        {
            var taskRegistered = false;
            var taskName = "UpdatePodcastsTask";

            var tasks = Windows.ApplicationModel.Background.BackgroundTaskRegistration.AllTasks;

            foreach (var task in tasks)
            {
                var currentTask = task.Value;

                if (currentTask.Name == taskName)
                {
                    taskRegistered = true;
                    break;
                }
            }

            if (!taskRegistered)
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = taskName;
                builder.TaskEntryPoint = "PodcastR.BackgroundTasks.UpdatePodcastsTask";
                builder.SetTrigger(new PushNotificationTrigger());

                var task = builder.Register();
            }
        }
    }
}
