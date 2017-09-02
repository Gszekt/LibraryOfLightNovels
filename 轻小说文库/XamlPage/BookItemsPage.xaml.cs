using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 轻小说文库 {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class BookItemsPage : Page {
		private static int pageNum;
		private static string kind;
		private static string localTitle;
		private ObservableCollection<BookItem> BookItems { get; set; }
		public string RequestURI { get; private set; }

		public BookItemsPage() {
			this.InitializeComponent();
			BookItems = new ObservableCollection<BookItem>();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e) {
			if (e.NavigationMode == NavigationMode.New) {
				var button = e.Parameter as Button;
				switch (button.Name) {
					case "hotNovelsButton":
						RequestURI = BaseURIs.hotNovelsURI;
						kind = "normal";
						localTitle = MainPage.TitleTextBlock.Text = "热门轻小说";
						break;
					case "animatedNovelsButton":
						RequestURI = BaseURIs.animatedNovelsURI;
						kind = "normal";
						localTitle = MainPage.TitleTextBlock.Text = "动画化作品";
						break;
					case "updatedNovelsButton":
						RequestURI = BaseURIs.updatedNovelsURI;
						kind = "normal";
						localTitle = MainPage.TitleTextBlock.Text = "今日更新";
						break;
					case "newNovelsButton":
						RequestURI = BaseURIs.newNovelsURI;
						kind = "normal";
						localTitle = MainPage.TitleTextBlock.Text = "新书一览";
						break;
					case "completeNovelsButton":
						RequestURI = BaseURIs.completeNovelsURI;
						kind = "normal";
						localTitle = MainPage.TitleTextBlock.Text = "完结全本";
						break;
					case "favouriteNovelsButton":
						RequestURI = BaseURIs.favouriteNovelsURI;
						kind = "special";
						localTitle = MainPage.TitleTextBlock.Text = "特别收藏";
						break;
					default:
						break;
				}
				pageNum = 1;
				string htmlPage = null;
				if (kind == "normal") {
					htmlPage = await HTMLParser.Instance.GetHtml(RequestURI + "&page=" + pageNum++);
				}
				else {
					htmlPage = await HTMLParser.Instance.GetHtml(RequestURI);
				}
				if (htmlPage == null) {
					await MainPage.PopMessageDialog("网络或服务器故障！");
					return;
				}
				try {
					BookItems.Clear();
					BookItemParser.Instance.GetBookItems(htmlPage, BookItems, kind);
					MainPage.ProgressRing.IsActive = false;
					MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				}
				catch (Exception) {
					await MainPage.PopMessageDialog("网络或服务器故障！");
				}
			}
			else if (e.NavigationMode == NavigationMode.Back) {
				MainPage.TitleTextBlock.Text = localTitle;
				MainPage.ProgressRing.IsActive = false;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
			}
		}

		bool flag = true;
		private async void BookItemsScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e) {
			if (kind == "special") {
				return;
			}
			if (BookItemsScrollViewer.VerticalOffset > BookItemsScrollViewer.ScrollableHeight - 10 && flag) {
				flag = false;
				MainPage.ProgressRing.IsActive = true;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
				var htmlPage = await HTMLParser.Instance.GetHtml(RequestURI + "&page=" + pageNum++);
				if (htmlPage == null) {
					await MainPage.PopMessageDialog("网络或服务器故障！");
				}
				else {
					BookItemParser.Instance.GetBookItems(htmlPage, BookItems, "normal");
				}
				flag = true;
				MainPage.ProgressRing.IsActive = false;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
			}
		}

		// BookItem的点击响应
		private void BookItemsGridView_ItemClick(object sender, ItemClickEventArgs e) {
			if (e.ClickedItem is BookItem) {
				this.Frame.Navigate(typeof(BookDetalsPage), e.ClickedItem);
				MainPage.ProgressRing.IsActive = true;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
			}
		}
	}

	// 程序中用到的所有URI
	public class BaseURIs {
		public static string hotNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=allvisit";
		public static string animatedNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=anime";
		public static string updatedNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=lastupdate";
		public static string newNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=postdate";
		public static string completeNovelsURI = "http://www.wenku8.com/modules/article/articlelist.php?fullflag=1";
		public static string favouriteNovelsURI = "http://www.wenku8.com/modules/article/bookcase.php";

		public static string collectNovelURI = "http://www.wenku8.com/modules/article/addbookcase.php?bid=";
		public static string deleteNovelURI = "http://www.wenku8.com/modules/article/bookcase.php?delid=";
		public static string voteNovelURI = "http://www.wenku8.com/modules/article/uservote.php?id=";
	}
}
