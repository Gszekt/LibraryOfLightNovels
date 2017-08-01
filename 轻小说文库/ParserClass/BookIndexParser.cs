using HtmlAgilityPack;
using System.Collections.ObjectModel;

namespace 轻小说文库 {
	public class BookIndexParser {
		private static BookIndexParser instance;
		/// <summary>
		/// 获取一个BookindexParser的实例
		/// </summary>
		public static BookIndexParser Instance {
			get {
				if (instance == null) {
					instance = new BookIndexParser();
				}
				return instance;
			}
		}
		public void GetBookIndexes(string htmlPage, ObservableCollection<BookIndex> bookIndexes) {
			var bookIndex = new BookIndex();
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(htmlPage);
			var contentNode = htmlDoc.DocumentNode;
			foreach (var tableNode in contentNode.Descendants("table")) {
				foreach (var trNode in tableNode.Descendants("tr")) {
					if (trNode.ChildNodes.Count < 4) {
						bookIndex = new BookIndex();
						var stringSplits = trNode.InnerText.Split(' ');
						bookIndex.VolumnTitle = stringSplits[4].Trim();
						bookIndexes.Add(bookIndex);
					}
					else {
						foreach (var td in trNode.ChildNodes) {
							if (td.Name == "td" && td.InnerText != "&nbsp;") {
								var titleAndLinkage = new TitleAndLinkage();
								titleAndLinkage.ChapterTitle = td.InnerText.Replace("&#9450;", "⓪");
								titleAndLinkage.InterLinkage = td.FirstChild.Attributes["href"].Value;
								bookIndex.ChapterTitles.Add(titleAndLinkage);
							}
						}
					}
				}
			}
		}
	}
}
