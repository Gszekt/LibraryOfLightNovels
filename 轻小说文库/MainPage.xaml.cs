﻿using System;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
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
		public static ProgressRing ProgressRing { get; set; }
		private string currentBookClassification;

		public MainPage() {
			this.InitializeComponent();

			MainSplitView = menuSplitView;
			ContentFrame = splitViewContentFrame;
			TitleTextBlock = titleTextBlock;
			ProgressRing = progressBar;
			CheckIdAndNameAsync();

			this.AddHandler(KeyDownEvent, new KeyEventHandler(OnBackKeyDown), true);
		}

		/// <summary>
		/// 处理键盘退格键，返回上一页
		/// </summary>
		private void OnBackKeyDown(object sender, KeyRoutedEventArgs e) {
			if (e.Key is Windows.System.VirtualKey.Back && splitViewContentFrame.CanGoBack) {
				ProgressRing.IsActive = true;
				ProgressRing.Visibility = Visibility.Visible;
				splitViewContentFrame.GoBack();
			}
		}

		/// <summary>
		/// 检查ID和password是否已经存在
		/// </summary>
		public async void CheckIdAndNameAsync() {
			var appData = ApplicationData.Current.LocalSettings;
			if (!appData.Values.ContainsKey("UserName") || appData.Values["UserName"] == null) {
				ProgressRing.IsActive = true;
				ProgressRing.Visibility = Visibility.Visible;
				ContentFrame.Navigate(typeof(SignInPage));
			}
			else {
				var password = appData.Values["Password"] as string;
				var userName = appData.Values["UserName"] as string;
				if (await HTMLParser.SetHttpClientInstanceAsync(userName, password)) {
					ProgressRing.IsActive = true;
					ProgressRing.Visibility = Visibility.Visible;
					currentBookClassification = "updatedNovelsButton";
					ContentFrame.Navigate(typeof(BookItemsPage), new Button { Name = "updatedNovelsButton" });
				}
				else {
					await PopMessageDialog("网络或服务器故障！");
				}
			}
		}

		public static async System.Threading.Tasks.Task PopMessageDialog(string message) {
			var dialog = new ContentDialog() {
				Content = message,
				IsPrimaryButtonEnabled = true,
				PrimaryButtonText = "确定",
				FullSizeDesired = false,
				Background = new Windows.UI.Xaml.Media.SolidColorBrush(Color.FromArgb(191, 243, 243, 248)),
				BorderThickness= new Thickness(0.0)
				
			};
			await dialog.ShowAsync();
		}

		/// <summary>
		/// 汉堡按钮事件处理 控制SplitView.Pane的开关
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuButton_Click(object sender, RoutedEventArgs e) {
			menuSplitView.IsPaneOpen = !menuSplitView.IsPaneOpen;
		}

		/// <summary>
		/// 处理书籍种类按钮的点击事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BookClassificationButton_Click(object sender, RoutedEventArgs e) {
			if (Window.Current.Bounds.Width <= 1200) {
				menuSplitView.IsPaneOpen = false;
			}
			if ((sender as Button).Name != currentBookClassification) {
				MainPage.ProgressRing.IsActive = true;
				MainPage.ProgressRing.Visibility = Visibility.Visible;
				splitViewContentFrame.Navigate(typeof(BookItemsPage), sender);
				MainPage.ContentFrame.BackStack.Clear();
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
				currentBookClassification = (sender as Button).Name;
			}
		}

		/// <summary>
		/// 设置 按钮事件处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SettingsButton_Click(object sender, RoutedEventArgs e) {

		}

		/// <summary>
		/// 账户 按钮事件处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AccountButton_Click(object sender, RoutedEventArgs e) {

		}

	}
}
