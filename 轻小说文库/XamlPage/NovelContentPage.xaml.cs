using HtmlAgilityPack;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 轻小说文库 {
	/// <summary>
	/// 小说内容和插图显示页面
	/// </summary>
	public sealed partial class NovelContentPage : Page {
		private ObservableCollection<Illustration> Illustrations { get; set; }
		private HtmlDocument htmlDoc;

		public NovelContentPage() {
			this.InitializeComponent();
			htmlDoc = new HtmlDocument();
			Illustrations = new ObservableCollection<Illustration>();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e) {
			var novelContentUri = e.Parameter as string;
			var htmlPage = await HTMLParser.Instance.GetHtml(novelContentUri);
			if (htmlPage == null) {
				MainPage.TipsTextBlock.Text = "网络或服务器故障！";
				MainPage.TipsStackPanel.Visibility = Visibility.Visible;
				return;
			}

			htmlDoc.LoadHtml(htmlPage);
			var title = htmlDoc.GetElementbyId("title").InnerText;
			if (title.Contains("插图")) {
				illustrationsListView.Visibility = Visibility.Visible;
				novelContentScrollViewer.Visibility = Visibility.Collapsed;
				IllustrationUriParser.Instance.GetIllustrationsAsync(htmlPage, Illustrations);
			}
			else {
				illustrationsListView.Visibility = Visibility.Collapsed;
				novelContentScrollViewer.Visibility = Visibility.Visible;
				novelContentTextBlock.Text = NovelTextParser.Instance.GetNovelText(htmlPage);
				novelContentScrollViewer.ChangeView(null, 0, null);
			}

			MainPage.ProgressRing.IsActive = false;
			MainPage.ProgressRing.Visibility = Visibility.Collapsed;
		}

		// 
		protected override void OnKeyDown(KeyRoutedEventArgs e) {
			novelContentScrollViewer.ChangeView(null, novelContentScrollViewer.VerticalOffset + 2, null);
			base.OnKeyDown(e);
		}

		private void OnUpKeyDown(object sender, KeyRoutedEventArgs e) {
			novelContentScrollViewer.ChangeView(null, novelContentScrollViewer.VerticalOffset + 2, null);
		}

	}
}
