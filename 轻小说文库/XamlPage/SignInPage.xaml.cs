using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 轻小说文库 {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SignInPage : Page {
		public SignInPage() {
			this.InitializeComponent();
		}

		private async void SignInButton_ClickAsync(object sender, RoutedEventArgs e) {
			ApplicationDataContainer appData = ApplicationData.Current.LocalSettings;
			if (!(appData.Values.ContainsKey("UserName"))) {
				appData.Values.Add("UserName", userNameTextBox.Text);
			}
			if (!(appData.Values.ContainsKey("Password"))) {
				appData.Values.Add("Password", passwordPasswordBox.Password);
			}
			signInGrid.Visibility = Visibility.Collapsed;
			if (await HTMLParser.SetHttpClientInstanceAsync(userNameTextBox.Text, passwordPasswordBox.Password)) {
				this.Frame.Navigate(typeof(BookItemsPage),
					new Button { Name = "updatedNovelsButton" });
			}
			else {
				MainPage.TipsTextBlock.Text = "网络或服务器故障！";
				MainPage.TipsStackPanel.Visibility = Visibility.Visible;
			}
		}

		protected override void OnNavigatedTo(NavigationEventArgs e) {
			MainPage.ProgressRing.IsActive = false;
			MainPage.ProgressRing.Visibility = Visibility.Collapsed;
		}
	}
}
