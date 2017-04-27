using HtmlAgilityPack;
using System.Collections.ObjectModel;

namespace 轻小说文库
{
    public class IllustrationParser
    {
        private IllustrationParser(){}
        private static IllustrationParser instance;

        public static IllustrationParser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IllustrationParser();
                }
                return instance;
            }
        }

        public ObservableCollection<Illustration> GetIllustrations(string htmlPage)
        {
            var illustrations = new ObservableCollection<Illustration>();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlPage);
            var contentNode = htmlDoc.DocumentNode;
            foreach (var imageNode in contentNode.Descendants("img"))
            {
                Illustration illustration = new Illustration();
                illustration.ImageUri = imageNode.Attributes["src"].Value;
                illustrations.Add(illustration);
            }
            return illustrations;
        }
    }
}
