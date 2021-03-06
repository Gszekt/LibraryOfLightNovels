﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 轻小说文库 {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ChapterIndexPage : Page {
		internal List<object> linkageAndTitles;
		internal List<object> parameters;

		private static string localTitle;

		public ChapterIndexPage() {
			this.InitializeComponent();
			parameters = new List<object>();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e) {
			if (e.NavigationMode == NavigationMode.New) {
				linkageAndTitles = e.Parameter as List<object>;
				localTitle = MainPage.TitleTextBlock.Text = (linkageAndTitles[0] as BookIndex).VolumnTitle;
				chapterIndexListView.ItemsSource = (linkageAndTitles[0] as BookIndex).ChapterTitles;
				MainPage.ProgressRing.IsActive = false;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
			}
			else if (e.NavigationMode == NavigationMode.Back) {
				MainPage.TitleTextBlock.Text = localTitle;
				MainPage.ProgressRing.IsActive = false;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
			}
		}

		private void ChapterIndexListView_ItemClick(object sender, ItemClickEventArgs e) {
			var titleAndLinkage = (TitleAndLinkage)e.ClickedItem;
			if (titleAndLinkage != null) {
				var novelContentUri = (linkageAndTitles[1] as string).Replace("index.htm", titleAndLinkage.InterLinkage);
				MainPage.ProgressRing.IsActive = true;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
				this.Frame.Navigate(typeof(NovelContentPage), novelContentUri);
			}
		}
	}
}
