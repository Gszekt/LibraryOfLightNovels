using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 轻小说文库 {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class NovelContentPage : Page {
		public NovelContentPage() {
			this.InitializeComponent();
		}
		protected override void OnNavigatedTo(NavigationEventArgs e) {
			var parameters = e.Parameter as List<object>;
			var isNovelText = (bool)parameters[1];
			if (isNovelText) {
				illustrationsListView.Visibility = Visibility.Collapsed;
				novelContentScrollViewer.Visibility = Visibility.Visible;
				novelContentTextBlock.Text = parameters[0] as string;
				novelContentScrollViewer.ChangeView(0, 0, 0);
			}
			else {
				illustrationsListView.Visibility = Visibility.Visible;
				novelContentScrollViewer.Visibility = Visibility.Collapsed;
				illustrationsListView.ItemsSource = parameters[0] as ObservableCollection<Illustration>;
			}
			MainPage.ProgressRing.IsActive = false;
			MainPage.ProgressRing.Visibility = Visibility.Collapsed;
		}
	}
}
