﻿using HtmlAgilityPack;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 轻小说文库 {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class BookDetalsPage : Page {
		internal string readLinkage;
		internal List<object> parameters;
		private static string localTitle;
		private HtmlDocument HtmlDoc { get; set; }
		private BookItem Bookitem { get; set; }
		private ObservableCollection<BookIndex> BookIndexes { get; set; }

		public BookDetalsPage() {
			this.InitializeComponent();
			HtmlDoc = new HtmlDocument();
			parameters = new List<object>();
			BookIndexes = new ObservableCollection<BookIndex>();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e) {
			if (e.NavigationMode == NavigationMode.New) {
				Bookitem = e.Parameter as BookItem;
				localTitle = MainPage.TitleTextBlock.Text = Bookitem.Title;
				var htmlPage = await HTMLParser.Instance.GetHtml(Bookitem.Interlinkage);
				if (htmlPage == null) {
					MainPage.TipsTextBlock.Text = "网络或服务器故障！";
					MainPage.TipsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
				}
				else {
					HtmlDoc.LoadHtml(htmlPage);
					var contentNode = HtmlDoc.GetElementbyId("content");
					var summary = contentNode.ChildNodes[1].ChildNodes[7].ChildNodes[1].ChildNodes[3].ChildNodes[13].InnerText;
					readLinkage = contentNode.ChildNodes[1].ChildNodes[11].ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes[0].Attributes["href"].Value;
					Bookitem.Summary = summary.Replace("&nbsp;", " ");
					Bookitem.ReadLinkage = readLinkage;

					summaryTextBlock.Text = Bookitem.Summary;
					bookDetailsGrid.DataContext = Bookitem;

					htmlPage = await HTMLParser.Instance.GetHtml(Bookitem.ReadLinkage);
					if (htmlPage == null) {
						MainPage.TipsTextBlock.Text = "网络或服务器故障！";
						MainPage.TipsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
					}
					else {
						BookIndexes.Clear();
						BookIndexParser.Instance.GetBookIndexes(htmlPage, BookIndexes);
						MainPage.ProgressRing.IsActive = false;
						MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
					}
				}
			}
			else if (e.NavigationMode == NavigationMode.Back) {
				MainPage.TitleTextBlock.Text = localTitle;
				MainPage.ProgressRing.IsActive = false;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
			}
		}

		private void VolumnIndexGridView_ItemClick(object sender, ItemClickEventArgs e) {
			parameters.Clear();
			var bookIndex = (BookIndex)e.ClickedItem;
			if (bookIndex != null) {
				parameters.Add(bookIndex);
				parameters.Add(readLinkage);
				MainPage.ProgressRing.IsActive = true;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
				MainPage.ContentFrame.Navigate(typeof(ChapterIndexPage), parameters);
			}
		}
	}
}
