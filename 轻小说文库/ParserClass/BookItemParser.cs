using HtmlAgilityPack;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace 轻小说文库 {
	public class BookItemParser {
		private static BookItemParser instance;
		/// <summary>
		/// 获取一个BookItemParser的实例
		/// </summary>
		public static BookItemParser Instance {
			get {
				if (instance == null) {
					instance = new BookItemParser();
				}
				return instance;
			}
		}
		private BookItemParser() { }
		public void GetBookItems(string htmlPage, ObservableCollection<BookItem> bookItems, string kind) {
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(htmlPage);
			var contentNodes = htmlDoc.GetElementbyId("content");
			HtmlNode targetNode = null;
			foreach (var item in contentNodes.Elements("table")) {
				if (item.GetAttributeValue("class", "f") == "grid") {
					targetNode = item;
				}
			}
			if (kind == "normal") {
				NormalGetBookItems(bookItems, targetNode);
			}
			else if (kind == "special") {
				SpecialGetBookItemsAsync(bookItems, targetNode);
			}
		}

		private static async void SpecialGetBookItemsAsync(ObservableCollection<BookItem> bookItems, HtmlNode targetNode) {
			foreach (var trNode in targetNode.Descendants("tr")) {
				foreach (var tdNode in trNode.Descendants("td")) {
					if (tdNode.GetAttributeValue("class", "f") == "even") {
						foreach (var aNode in tdNode.Descendants("a")) {
							string href = aNode.GetAttributeValue("href", "f");
							var bookItem = new BookItem() {
								Interlinkage = href,
								Title = aNode.InnerText.Split('(')[0].Replace("\r\n", " ").Trim(),
							};
							bookItems.Add(bookItem);
							break;
						}
						break;
					}
				}
			}
			foreach (var bookItem in bookItems) {
				var htmlPage = await HTMLParser.Instance.GetHtml(bookItem.Interlinkage);
				if (htmlPage == null) {
					MainPage.TipsTextBlock.Text = "网络或服务器故障！";
					MainPage.TipsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
				}
				else {
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(htmlPage);
					var contentNodes = htmlDoc.GetElementbyId("content");
					var tableNodes = contentNodes.Descendants("table");
					foreach (var tableNode in tableNodes) {
						//应对某些属性没有的情况，比如甘城光辉游乐园没有最近更新
						var trNode = tableNode.ChildNodes[3];
						try { bookItem.Classification = trNode.ChildNodes[1].InnerText; } catch { bookItem.Classification = "文库分类："; }
						try { bookItem.Author = trNode.ChildNodes[3].InnerText; } catch { bookItem.Author = "小说作者："; }
						try { bookItem.State = trNode.ChildNodes[5].InnerText; } catch { bookItem.State = "文章状态："; }
						try { bookItem.LastUpdate = trNode.ChildNodes[7].InnerText; } catch { bookItem.LastUpdate = "最后更新："; }
						break;
					}
					foreach (var imgNode in contentNodes.ChildNodes[1].ChildNodes[7].Descendants("img")) {
						bookItem.CoverUri = imgNode.Attributes["src"].Value;
						break;
					}
				}
			}
		}

		private static void NormalGetBookItems(ObservableCollection<BookItem> bookItems, HtmlNode targetNode) {
			foreach (var tdNode in targetNode.Descendants("td"))//此层循环只循环一次
			{
				foreach (var node in tdNode.ChildNodes) {
					if (node.Name == "div") {
						var bookItem = new BookItem() { CoverUri = node.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["src"].Value };
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
