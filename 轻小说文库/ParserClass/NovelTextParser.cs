using HtmlAgilityPack;

namespace 轻小说文库
{
    public class NovelTextParser
    {
        private static NovelTextParser instance;

        public static NovelTextParser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NovelTextParser();
                }
                return instance;
            }
        }
        private NovelTextParser(){}

        public string GetNovelText(string htmlPage)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlPage);
            var contentNode = htmlDoc.GetElementbyId("content");
            var novelText = contentNode.InnerText.Replace("&nbsp;", "  ");
            return novelText;
        }
    }
}
