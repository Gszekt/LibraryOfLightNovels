using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 轻小说文库
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookItemsPage : Page
    {
        private static int pageNum;
        private ObservableCollection<BookItem> bookItems;
        public string requestURI { get; private set; }

        public BookItemsPage()
        {
            this.InitializeComponent();
            bookItems = new ObservableCollection<BookItem>();
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var button = e.Parameter as Button;
            switch (button.Name)
            {
                case "hotNovelsButton":
                    requestURI = BookClassificationURI.hotNovelsURI;
                    MainPage.TitleTextBlock.Text = "热门轻小说";
                    break;
                case "animatedNovelsButton":
                    requestURI = BookClassificationURI.animatedNovelsURI;
                    MainPage.TitleTextBlock.Text = "动画化作品";
                    break;
                case "updatedNovelsButton":
                    requestURI = BookClassificationURI.updatedNovelsURI;
                    MainPage.TitleTextBlock.Text = "今日更新";
                    break;
                case "newNovelsButton":
                    requestURI = BookClassificationURI.newNovelsURI;
                    MainPage.TitleTextBlock.Text = "新书一览";
                    break;
                case "completeNovelsButton":
                    requestURI = BookClassificationURI.completeNovelsURI;
                    MainPage.TitleTextBlock.Text = "完结全本";
                    break;
                default:
                    break;
            }
            pageNum = 1;
            if (!HTMLParser.IsInstanceReady)
            {
                OnNavigatedTo(e);
            }
            else
            {
                var pageHtml = await HTMLParser.Instance.GetHtml(requestURI + "&page=" + pageNum++);
                try
                {
                    bookItems.Clear();
                    BookItemParser.Instance.GetBookItems(pageHtml, bookItems);
                    BookItemsGridView.ItemsSource = bookItems;
                }
                catch (System.Exception)
                {
                    MainPage.TipsTextBlock.Text = "出错！请检查您的网络！";
                    MainPage.TipsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
        }

        private void BookItemsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BookItem bookItem = BookItemsGridView.SelectedItem as BookItem;
            if (bookItem != null)
            {
                MainPage.ContentFrame.Navigate(typeof(BookDetalsPage), bookItem);
            }
        }
        bool flag = true;
        private async void BookItemsScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (BookItemsScrollViewer.VerticalOffset > BookItemsScrollViewer.ScrollableHeight - 10 && flag)
            {
                flag = false;
                var pageHtml = await HTMLParser.Instance.GetHtml(requestURI + "&page=" + pageNum++);
                flag = true;
                BookItemParser.Instance.GetBookItems(pageHtml, bookItems);
                BookItemsGridView.ItemsSource = bookItems;
            }
        }
    }
    public class BookClassificationURI
    {
        public static string hotNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=allvisit";
        public static string animatedNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=anime";
        public static string updatedNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=lastupdate";
        public static string newNovelsURI = "http://www.wenku8.com/modules/article/toplist.php?sort=postdate";
        public static string completeNovelsURI = "http://www.wenku8.com/modules/article/articlelist.php?fullflag=1";
    }
}
