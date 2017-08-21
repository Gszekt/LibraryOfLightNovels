using HtmlAgilityPack;

namespace 轻小说文库 {
	public class NovelTextParser {
		private static NovelTextParser instance;
		private static HtmlDocument htmlDoc;

		public static NovelTextParser Instance {
			get {
				if (instance == null) {
					instance = new NovelTextParser();
				}
				return instance;
			}
		}
		private NovelTextParser() {
			htmlDoc = new HtmlDocument();
		}

		// 从HTML源码中解析出小说内容
		public string GetNovelText(string htmlPage) {
			htmlDoc.LoadHtml(htmlPage);
			var contentNode = htmlDoc.GetElementbyId("content");
			return contentNode.InnerText.Replace("&nbsp;", "  ");
		}
	}
}
