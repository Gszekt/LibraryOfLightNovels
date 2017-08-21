using HtmlAgilityPack;
using System.Collections.ObjectModel;
using 轻小说文库.ParserClass;

namespace 轻小说文库 {
	public class IllustrationUriParser {
		private IllustrationUriParser() { }
		private static IllustrationUriParser instance;

		public static IllustrationUriParser Instance {
			get {
				if (instance == null) {
					instance = new IllustrationUriParser();
				}
				return instance;
			}
		}

		public async void GetIllustrationsAsync(string htmlPage,ObservableCollection<Illustration> illustrations) {
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(htmlPage);
			var contentNode = htmlDoc.DocumentNode;
			foreach (var imageNode in contentNode.Descendants("img")) {
				var illustration = new Illustration {
					Image = await ImageHelper.GetImage(new System.Uri(imageNode.Attributes["src"].Value),"illustration")
				};
				illustrations.Add(illustration);
			}
		}
	}
}
