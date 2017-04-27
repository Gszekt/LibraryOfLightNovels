using HtmlAgilityPack;
using System.Collections.ObjectModel;

namespace 轻小说文库
{
    public class BookItemParser
    {
        private static BookItemParser instance;
        /// <summary>
        /// 获取一个BookItemParser的实例
        /// </summary>
        public static BookItemParser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BookItemParser();
                }
                return instance;
            }            
        }
        private BookItemParser(){}
        public void GetBookItems(string htmlPage , ObservableCollection<BookItem> bookItems)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlPage);
            HtmlNode contentNodes = htmlDoc.GetElementbyId("content");
            HtmlNode tableNode = null;
            foreach (var item in contentNodes.Elements("table"))
            {
                if (item.GetAttributeValue("class", "f") == "grid")
                {
                    tableNode = item;
                }
            }
            foreach (var tdNode in tableNode.Descendants("td"))//此层循环只循环一次
            {
                foreach (var node in tdNode.ChildNodes)
                {
                    if (node.Name == "div")
                    {
                        var bookItem = new BookItem();
                        bookItem.CoverUri = node.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["src"].Value;
                        var temps = node.ChildNodes[1].ChildNodes[1].Attributes["title"].Value.Split('(');
                        bookItem.Title = temps[0].Replace("\r\n", " ").Trim();//标题
                        bookItem.Interlinkage = node.ChildNodes[1].ChildNodes[1].Attributes["href"].Value;
                        temps = node.ChildNodes[3].ChildNodes[3].InnerText.Split('/');
                        bookItem.Author = temps[0];//作者
                        bookItem.Classification = temps[1];//分类
                        bookItem.State = temps[2];//状态
                        temps = node.ChildNodes[3].ChildNodes[5].InnerText.Split('/');
                        bookItem.LastUpdate = temps[0];//最近更新
                        bookItem.Summary = node.ChildNodes[3].ChildNodes[7].InnerText;
                        bookItems.Add(bookItem);
                    }
                }
            }
        }
    }
}
