using PodcastR.Data.Entities;
using PodcastR.WindowsStore.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Hub App template is documented at http://go.microsoft.com/fwlink/?LinkId=321221

namespace PodcastR.WindowsStore
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static SystemMediaTransportControls systemControls;
        private static DisplayRequest displayRequestManager;
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        public static MediaElement Player { get; set; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                // create the media element for background audio
                rootFrame.Style = Resources["RootFrameStyle"] as Style;
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(HubPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        public static void SetUpBackgroundAudio()
        {
            var rootGrid = VisualTreeHelper.GetChild(Window.Current.Content, 0);
            App.Player = (MediaElement)VisualTreeHelper.GetChild(rootGrid, 0);

            systemControls = SystemMediaTransportControls.GetForCurrentView();
            systemControls.IsPlayEnabled = true;
            systemControls.IsPauseEnabled = true;
            systemControls.IsStopEnabled = true;
            systemControls.IsEnabled = true;
            systemControls.IsNextEnabled = true;
            systemControls.IsPreviousEnabled = true;
            systemControls.DisplayUpdater.Type = MediaPlaybackType.Music;
            systemControls.ButtonPressed += systemControls_ButtonPressed;
            displayRequestManager = new DisplayRequest();
        }

        private static async void systemControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Next:
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Player.Pause();
                        systemControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    });
                    break;
                case SystemMediaTransportControlsButton.Play:
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, ()=>{
                        Player.Play();
                        systemControls.PlaybackStatus= MediaPlaybackStatus.Playing;
                    });
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    break;
                case SystemMediaTransportControlsButton.Stop:
                    break;
                default:
                    break;
            }
        }

        public static void UpdateSystemControls(Episode episode)
        {
            if (systemControls != null)
            {
                systemControls.DisplayUpdater.Thumbnail = RandomAccessStreamReference.CreateFromUri(episode.ImageUrl);
                systemControls.DisplayUpdater.MusicProperties.AlbumArtist = episode.Podcast.Name;
                systemControls.DisplayUpdater.MusicProperties.Artist = episode.Author;
                systemControls.DisplayUpdater.MusicProperties.Title = episode.Name;
                systemControls.DisplayUpdater.Update();
                systemControls.PlaybackStatus = MediaPlaybackStatus.Playing;
            }
        }
    }
}
