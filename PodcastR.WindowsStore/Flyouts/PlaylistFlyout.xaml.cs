﻿using PodcastR.Common;
using PodcastR.Extensions;
using PodcastR.Core.Entities;
using PodcastR.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace PodcastR.Flyouts
{
    public sealed partial class PlaylistFlyout : SettingsFlyout
    {
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public PlaylistFlyout()
        {
            this.InitializeComponent();
        }

        private void SettingsFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            this.DefaultViewModel["Playlist"] = App.Playlist;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            App.Player.Play(e.ClickedItem as EpisodeViewModel);
        }
    }
}
