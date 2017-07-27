using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace 轻小说文库 {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page {
		public static Frame ContentFrame { get; set; }
		public static TextBlock TitleTextBlock { get; set; }
		public static SplitView MainSplitView { get; set; }
		public static TextBlock TipsTextBlock { get; set; }
		public static StackPanel TipsStackPanel { get; set; }
		public static ProgressRing ProgressRing { get; set; }

		public MainPage() {
			this.InitializeComponent();

			TipsStackPanel = tipsStackPanel;
			TipsTextBlock = tipsTextBlock;
			MainSplitView = menuSplitView;
			ContentFrame = splitViewContentFrame;
			TitleTextBlock = titleTextBlock;
			ProgressRing = progressBar;
			SetAppViewBackButton();
			SetTitleBarView();
			CheckIdAndNameAsync();

			this.AddHandler(KeyDownEvent, new KeyEventHandler(BackKeysDown), true);
		}

		private void BackKeysDown(object sender, KeyRoutedEventArgs e) {
			if (e.Key is Windows.System.VirtualKey.Back && splitViewContentFrame.CanGoBack) {
				MainPage.ProgressRing.IsActive = true;
				splitViewContentFrame.GoBack();
			}
		}

		/// <summary>
		/// 设置标题栏颜色
		/// </summary>
		private void SetTitleBarView() {
			var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
			titleBar.BackgroundColor = Color.FromArgb(255, 0, 122, 204);
			titleBar.ButtonBackgroundColor = Color.FromArgb(255, 0, 122, 204);
			titleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 0, 122, 224);
			titleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 0, 122, 184);
			titleBar.ButtonInactiveBackgroundColor = Color.FromArgb(255, 0, 122, 224);
			titleBar.InactiveBackgroundColor = Color.FromArgb(255, 0, 122, 224);
		}

		/// <summary>
		/// 检查ID和password是否已经存在
		/// </summary>
		public async System.Threading.Tasks.Task CheckIdAndNameAsync() {
			var appData = ApplicationData.Current.LocalSettings;
			if (!appData.Values.ContainsKey("UserName") || appData.Values["UserName"] == null) {
				ContentFrame.Navigate(typeof(SignInPage));
			}
			else {
				var password = appData.Values["Password"] as string;
				var userName = appData.Values["UserName"] as string;
				if (await HTMLParser.SetHttpClientInstanceAsync(userName, password)) {
					ContentFrame.Navigate(typeof(BookItemsPage),
						new Button { Name = "updatedNovelsButton" });
				}
				else {
					TipsTextBlock.Text = "网络或服务器故障！";
					TipsStackPanel.Visibility = Visibility.Visible;
				}
			}
		}

		/// <summary>
		/// 设置标题栏返回键
		/// </summary>
		private void SetAppViewBackButton() {
			SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = splitViewContentFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
			splitViewContentFrame.Navigated += OnNavigated;
		}

		/// <summary>
		/// 控制标题栏返回键的可见性
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnNavigated(object sender, NavigationEventArgs e) {
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ((Frame)sender).CanGoBack ?
				AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
		}

		/// <summary>
		/// 标题栏返回键 事件处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BackRequested(object sender, BackRequestedEventArgs e) {
			if (splitViewContentFrame.CanGoBack) {
				MainPage.ProgressRing.IsActive = true;
				splitViewContentFrame.GoBack();
			}
		}

		/// <summary>
		/// 汉堡按钮事件处理 控制SplitView.Pane的开关
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuButton_Click(object sender, RoutedEventArgs e) {
			menuSplitView.IsPaneOpen = !menuSplitView.IsPaneOpen;
		}

		/// <summary>
		/// 处理书籍种类按钮的点击事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bookClassificationButton_Click(object sender, RoutedEventArgs e) {
			if (Window.Current.Bounds.Width <= 1200) {
				menuSplitView.IsPaneOpen = false;
			}
			MainPage.ProgressRing.IsActive = true;
			splitViewContentFrame.Navigate(typeof(BookItemsPage), sender);
		}

		/// <summary>
		/// 设置 按钮事件处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void settingsButton_Click(object sender, RoutedEventArgs e) {

		}

		/// <summary>
		/// 账户 按钮事件处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AccountButton_Click(object sender, RoutedEventArgs e) {

		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			TipsTextBlock.Text = "";
			TipsStackPanel.Visibility = Visibility.Collapsed;
		}
	}
}
