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
		public string requestURI { get; private set; }

		public BookItemsPage() {
			this.InitializeComponent();
			BookItems = new ObservableCollection<BookItem>();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e) {
			if (e.NavigationMode == NavigationMode.New) {
				var button = e.Parameter as Button;
				switch (button.Name) {
					case "hotNovelsButton":
						requestURI = BookClassificationURI.hotNovelsURI;
						kind = "normal";
						localTitle = MainPage.TitleTextBlock.Text = "热门轻小说";
						break;
					case "animatedNovelsButton":
						requestURI = BookClassificationURI.animatedNovelsURI;
						kind = "normal";
						localTitle = MainPage.TitleTextBlock.Text = "动画化作品";
						break;
					case "updatedNovelsButton":
						requestURI = BookClassificationURI.updatedNovelsURI;
						kind = "normal";
						localTitle = MainPage.TitleTextBlock.Text = "今日更新";
						break;
					case "newNovelsButton":
						requestURI = BookClassificationURI.newNovelsURI;
						kind = "normal";
						localTitle = MainPage.TitleTextBlock.Text = "新书一览";
						break;
					case "completeNovelsButton":
						requestURI = BookClassificationURI.completeNovelsURI;
						kind = "normal";
						localTitle = MainPage.TitleTextBlock.Text = "完结全本";
						break;
					case "favouriteNovelsButton":
						requestURI = BookClassificationURI.favouriteNovelsURI;
						kind = "special";
						localTitle = MainPage.TitleTextBlock.Text = "特别收藏";
						break;
					default:
						break;
				}
				pageNum = 1;
				string htmlPage = null;
				if (kind == "normal") {
					htmlPage = await HTMLParser.Instance.GetHtml(requestURI + "&page=" + pageNum++);
				}
				else {
					htmlPage = await HTMLParser.Instance.GetHtml(requestURI);
				}
				if (htmlPage == null) {
					MainPage.TipsTextBlock.Text = "网络或服务器故障！";
					MainPage.TipsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
				}
				else {
					try {
						BookItems.Clear();
						await BookItemParser.Instance.GetBookItemsAsync(htmlPage, BookItems, kind);
						MainPage.ProgressRing.IsActive = false;
						MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
					}
					catch (System.Exception) {
						MainPage.TipsTextBlock.Text = "出错！";
						MainPage.TipsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
					}
				}
			}
			else if (e.NavigationMode == NavigationMode.Back) {
				MainPage.TitleTextBlock.Text = localTitle;
				MainPage.ProgressRing.IsActive = false;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
			}
		}

		private void BookItemsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			BookItem bookItem = BookItemsGridView.SelectedItem as BookItem;
			if (bookItem != null) {
				MainPage.ContentFrame.Navigate(typeof(BookDetalsPage), bookItem);
				MainPage.ProgressRing.IsActive = true;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
			}
		}

		bool flag = true;
		private async void BookItemsScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e) {
			if (BookItemsScrollViewer.VerticalOffset > BookItemsScrollViewer.ScrollableHeight - 10 && flag) {
				flag = false;
				MainPage.ProgressRing.IsActive = true;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
				var htmlPage = await HTMLParser.Instance.GetHtml(requestURI + "&page=" + pageNum++);
				if (htmlPage == null) {
					MainPage.TipsTextBlock.Text = "网络或服务器故障！";
					MainPage.TipsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
				}
				else {
					await BookItemParser.Instance.GetBookItemsAsync(htmlPage, BookItems, "normal");
				}
				flag = true;
				MainPage.ProgressRing.IsActive = false;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
			}
		}

		private void BookItemsGridView_ItemClick(object sender, ItemClickEventArgs e) {
			var bookItem = e.ClickedItem as BookItem;
			if (bookItem != null) {
				MainPage.ContentFrame.Navigate(typeof(BookDetalsPage), bookItem);
				MainPage.ProgressRing.IsActive = true;
				MainPage.ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
			}
		}
	}
	public class BookClassificationURI {
		public static string hotNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=allvisit";
		public static string animatedNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=anime";
		public static string updatedNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=lastupdate";
		public static string newNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=postdate";
		public static string completeNovelsURI = "http://www.wenku8.com/modules/article/articlelist.php?fullflag=1";
		public static string favouriteNovelsURI = "http://www.wenku8.com/modules/article/bookcase.php";
	}
}
