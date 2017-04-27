using HtmlAgilityPack;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 轻小说文库
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookDetalsPage : Page
    {
        internal string readLinkage;
        internal List<object> parameters;

        public BookDetalsPage()
        {
            this.InitializeComponent();
            parameters = new List<object>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var bookItem = e.Parameter as BookItem;
            MainPage.TitleTextBlock.Text = bookItem.Title;
            var htmlPage = await HTMLParser.Instance.GetHtml(bookItem.Interlinkage);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlPage);
            var contentNode = htmlDoc.GetElementbyId("content");
            var summary = contentNode.ChildNodes[1].ChildNodes[7].ChildNodes[1].ChildNodes[3].ChildNodes[9].InnerText;
            readLinkage = contentNode.ChildNodes[1].ChildNodes[11].ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes[0].Attributes["href"].Value;
            bookItem.Summary = summary.Replace("&nbsp;", " ");
            bookItem.ReadLinkage = readLinkage;

            bookDetailsGrid.DataContext = bookItem;
            summaryTextBlock.Text = bookItem.Summary;

            htmlPage = await HTMLParser.Instance.GetHtml(bookItem.ReadLinkage);
            var bookIndexes = BookIndexParser.Instance.GetBookIndexes(htmlPage);
            volumnIndexGridView.ItemsSource = bookIndexes;
        }

        private void volumnIndexGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            parameters.Clear();
            var bookIndex = volumnIndexGridView.SelectedItem as BookIndex;
            if (bookIndex != null)
            {
                parameters.Add(bookIndex);
                parameters.Add(readLinkage);
                MainPage.ContentFrame.Navigate(typeof(ChapterIndexPage), parameters);
            }
        }        
    }
}
