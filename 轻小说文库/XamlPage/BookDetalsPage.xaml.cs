using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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
				bookDetailsGrid.DataContext = Bookitem;
				localTitle = MainPage.TitleTextBlock.Text = Bookitem.Title;
				var htmlPage = await HTMLParser.Instance.GetHtml(Bookitem.Interlinkage);
				if (htmlPage == null) {
					await MainPage.PopMessageDialog("网络或服务器故障！");
					return;
				}
				HtmlDoc.LoadHtml(htmlPage);
				var contentNode = HtmlDoc.GetElementbyId("content");
				var summary = contentNode.ChildNodes[1].ChildNodes[7].ChildNodes[1].ChildNodes[3].ChildNodes[13].InnerText;
				readLinkage = contentNode.ChildNodes[1].ChildNodes[11].ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes[0].Attributes["href"].Value;
				Bookitem.Summary = summary.Replace("&nbsp;", " ");
				Bookitem.ReadLinkage = readLinkage;
				try {
					var localFolder = ApplicationData.Current.LocalCacheFolder;
					var file = await localFolder.GetFileAsync("collectedNovels.txt");
					var collectedNovels = await FileIO.ReadTextAsync(file);
					if (collectedNovels.Contains(Bookitem.BID)) {
						CollectButton.Content = "取消收藏";
					}
					else {
						CollectButton.Content = "添加收藏";
					}
				}
				catch (Exception) {
					CollectButton.Content = "添加收藏";
				}

				summaryTextBlock.Text = Bookitem.Summary;

				htmlPage = await HTMLParser.Instance.GetHtml(Bookitem.ReadLinkage);
				if (htmlPage == null) {
					await MainPage.PopMessageDialog("已取消收藏！");
				}
				else {
					BookIndexes.Clear();
					BookIndexParser.Instance.GetBookIndexes(htmlPage, BookIndexes);
					MainPage.ProgressRing.IsActive = false;
					MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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
				this.Frame.Navigate(typeof(ChapterIndexPage), parameters);
			}
		}

		// 收藏按钮的点击处理
		private async void CollectButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
			if ((sender as Button).Content.Equals("添加收藏")) {
				await HTMLParser.Instance.GetHtml(BaseURIs.collectNovelURI + Bookitem.BID);
				await MainPage.PopMessageDialog("收藏成功！");
				CollectButton.Content = "取消收藏";
			}
			else {
				await HTMLParser.Instance.GetHtml(BaseURIs.deleteNovelURI + Bookitem.DelID);
				await MainPage.PopMessageDialog("已取消收藏！");
				CollectButton.Content = "添加收藏";
			}
		}

		// 推荐按钮的点击处理
		private async void RecommendButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
			var htmlPage = await HTMLParser.Instance.GetHtml(BaseURIs.voteNovelURI + Bookitem.BID);
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(htmlPage);
			foreach (var node in htmlDoc.DocumentNode.Descendants()) {
				try {
					if (node.Attributes["class"].Value == "blockcontent") {
						await MainPage.PopMessageDialog(node.InnerText);
						break;
					}
				}
				catch (Exception) {
					// 当节点没有属性时， node.Attributes["class"]会抛出异常，不做处理，继续处理下一个节点
				}
			}
		}
	}
}
