using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 轻小说文库
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChapterIndexPage : Page
    {
        internal List<object> linkageAndTitles;
        internal List<object> parameters;

        public ChapterIndexPage()
        {
            this.InitializeComponent();
            parameters = new List<object>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            linkageAndTitles = e.Parameter as List<object>;
            MainPage.TitleTextBlock.Text = (linkageAndTitles[0] as BookIndex).VolumnTitle;
            chapterIndexListView.ItemsSource = (linkageAndTitles[0] as BookIndex).ChapterTitles;
			MainPage.ProgressRing.IsActive = false;
		}

        private async void chapterIndexListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TitleAndLinkage titleAndLinkage = chapterIndexListView.SelectedItem as TitleAndLinkage;
            if (titleAndLinkage != null)
            {
                parameters.Clear();
                var novelContentUri = (linkageAndTitles[1] as string).Replace("index.htm", titleAndLinkage.InterLinkage);
                var htmlPage = await HTMLParser.Instance.GetHtml(novelContentUri);
                if (htmlPage == null)
                {
                    MainPage.TipsTextBlock.Text = "网络或服务器故障！";
                    MainPage.TipsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else if (titleAndLinkage.ChapterTitle.Contains("插图"))
                {
                    var illustrations = IllustrationParser.Instance.GetIllustrations(htmlPage);
                    parameters.Add(illustrations);
                    parameters.Add(false);
                    MainPage.ContentFrame.Navigate(typeof(NovelContentPage), parameters);
					MainPage.ProgressRing.IsActive = true;
				}
                else
                {
                    var novelText = NovelTextParser.Instance.GetNovelText(htmlPage);
                    parameters.Add(novelText);
                    parameters.Add(true);
                    MainPage.ContentFrame.Navigate(typeof(NovelContentPage), parameters);
					MainPage.ProgressRing.IsActive = true;
				}
            }
        }
    }
}
