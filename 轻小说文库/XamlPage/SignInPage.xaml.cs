using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 轻小说文库
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignInPage : Page
    {
        public SignInPage()
        {
            this.InitializeComponent();
        }

        private void signInButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer appData = ApplicationData.Current.LocalSettings;
            if (!(appData.Values.ContainsKey("UserName")))
            {
                appData.Values.Add("UserName", userNameTextBox.Text);
            }
            if (!(appData.Values.ContainsKey("Password")))
            {
                appData.Values.Add("Password", passwordPasswordBox.Password);
            }
            HTMLParser.Password = appData.Values["Password"] as string;
            HTMLParser.UserName = appData.Values["UserName"] as string;
            signInGrid.Visibility = Visibility.Collapsed;
            MainPage.MainSplitView.IsPaneOpen = true;
        }
    }
}
