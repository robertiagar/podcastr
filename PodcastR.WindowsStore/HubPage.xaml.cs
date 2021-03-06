﻿using PodcastR.Core.Entities;
using PodcastR.Extensions;
using PodcastR.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PodcastR.ViewModel;


// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=321224

namespace PodcastR
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private System.Threading.CancellationTokenSource cts = new System.Threading.CancellationTokenSource();

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var viewModel = (MainViewModel)this.DataContext;
            if (this.DataContext != null && !viewModel.Podcasts.Any())
            {
                await viewModel.LoadPodcastsAsync();
                await viewModel.SavePodcastsAsync();
            }

            //var episode = await PodcastR.Data.Services.PodcastDownloaderService.DownloadPodcastEpisodeAsync(podcastsFromLocal[0].Episodes[2], p =>
            //{
            //    if (p.Progress.TotalBytesToReceive > 0)
            //    {
            //        defaultViewModel["Progress"] = p.Progress.BytesReceived * 100 / p.Progress.TotalBytesToReceive;
            //    }
            //    if (int.Parse(defaultViewModel["Progress"].ToString()) == 100)
            //    {
            //        defaultViewModel["Progress"] = "Downlod succesfull.";
            //    }
            //}, cts, errorCallback: () =>
            //{
            //    defaultViewModel["Progress"] = "Download cancelled or something went wrong.";
            //});
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var viewModel = (MainViewModel)this.DataContext;
            var podcasts = viewModel.AllPodcasts.OrderBy(p => p.Podcast.Name).ToList();
            var episodes = podcasts.SelectMany(p => p.Episodes).OrderByDescending(ep => ep.Episode.Published).ToList();
            if (section.Header.Equals("Subscriptions"))
            {
                this.Frame.Navigate(typeof(PodcastsPage), podcasts);
            }
            if (section.Header.Equals("Episodes"))
            {
                this.Frame.Navigate(typeof(EpisodesPage), episodes);
            }
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var podcast = e.ClickedItem as PodcastViewModel;
            var episode = e.ClickedItem as EpisodeViewModel;
            if (podcast != null)
                this.Frame.Navigate(typeof(PodcastPage), podcast);
            if (episode != null)
            {
                //this.Frame.Navigate(typeof(EpisodePage), episode);
                var viewModel = (MainViewModel)this.DataContext;
                viewModel.NowPlaying.Episode = episode;
                viewModel.NowPlaying.IsPlaying = true;
                App.Player.Play(episode);
            }
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
