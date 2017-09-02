using HtmlAgilityPack;
using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using 轻小说文库.ParserClass;

namespace 轻小说文库 {
	public class BookItemParser {
		private static HtmlDocument htmlDoc1;
		private static HtmlDocument htmlDoc2;
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

		private BookItemParser() {
			htmlDoc1 = new HtmlDocument();
			htmlDoc2 = new HtmlDocument();
		}

		public void GetBookItems(string htmlPage, ObservableCollection<BookItem> bookItems, string kind) {
			htmlDoc1.LoadHtml(htmlPage);
			var contentNodes = htmlDoc1.GetElementbyId("content");
			HtmlNode targetNode = null;
			foreach (var item in contentNodes.Elements("table")) {
				if (item.GetAttributeValue("class", "f") == "grid") {
					targetNode = item;
				}
			}
			if (kind == "normal") {
				NormalGetBookItemsAsync(bookItems, targetNode);
			}
			else if (kind == "special") {
				SpecialGetBookItemsAsync(bookItems, targetNode);
			}
		}

		/// <summary>
		/// 获取收藏的书籍
		/// </summary>
		/// <param name="bookItems">保存书籍的列表</param>
		/// <param name="targetNode">HTML源码</param>
		private async void SpecialGetBookItemsAsync(ObservableCollection<BookItem> bookItems, HtmlNode targetNode) {
			var collectedNovels = new System.Text.StringBuilder();
			foreach (var tr1Node in targetNode.Descendants("tr")) {
				foreach (var tdNode in tr1Node.Descendants("td")) {
					if (tdNode.GetAttributeValue("class", "f") == "even") {
						foreach (var aNode in tdNode.Descendants("a")) {
							string href = aNode.GetAttributeValue("href", "f");
							var bookItem = new BookItem() {
								Interlinkage = href,
								Title = aNode.InnerText.Split('(')[0].Replace("\r\n", " ").Trim(),
							};
							bookItem.DelID = href.Substring(href.LastIndexOf('=') + 1);
							bookItem.BID = href.Substring(href.IndexOf('=') + 1, href.IndexOf('&') - href.IndexOf('=') - 1);
							var htmlPage = await HTMLParser.Instance.GetHtml(bookItem.Interlinkage);
							if (htmlPage == null) {
								await MainPage.PopMessageDialog("网络或服务器故障！");
							}
							else {
								htmlDoc2.LoadHtml(htmlPage);
								var contentNodes = htmlDoc2.GetElementbyId("content");
								var tableNodes = contentNodes.Descendants("table");
								foreach (var tableNode in tableNodes) {
									//应对某些属性没有的情况，比如甘城光辉游乐园没有最近更新
									var tr2Node = tableNode.ChildNodes[3];
									try { bookItem.Classification = tr2Node.ChildNodes[1].InnerText; } catch { bookItem.Classification = "文库分类："; }
									try { bookItem.Author = tr2Node.ChildNodes[3].InnerText; } catch { bookItem.Author = "小说作者："; }
									try { bookItem.State = tr2Node.ChildNodes[5].InnerText; } catch { bookItem.State = "文章状态："; }
									try { bookItem.LastUpdate = tr2Node.ChildNodes[7].InnerText; } catch { bookItem.LastUpdate = "最后更新："; }
									break;
								}
								foreach (var imgNode in contentNodes.ChildNodes[1].ChildNodes[7].Descendants("img")) {
									var coverUri = imgNode.Attributes["src"].Value;
									bookItem.Cover = await ImageHelper.GetImage(new System.Uri(coverUri), "cover");
									break;
								}
							}
							collectedNovels.Append(bookItem.BID + ";");
							bookItems.Add(bookItem);
							break;
						}
						break;
					}
				}
			}
			try {
				var localFolder = ApplicationData.Current.LocalCacheFolder;
				var file = await localFolder.CreateFileAsync("collectedNovels.txt", CreationCollisionOption.OpenIfExists);
				await FileIO.WriteTextAsync(file, collectedNovels.ToString());
			}
			catch { }
		}

		/// <summary>
		/// 获取书籍
		/// </summary>
		/// <param name="bookItems">保存书籍的列表</param>
		/// <param name="targetNode">HTML源码</param>
		private async void NormalGetBookItemsAsync(ObservableCollection<BookItem> bookItems, HtmlNode targetNode) {
			foreach (var tdNode in targetNode.Descendants("td"))//此层循环只循环一次
			{
				foreach (var node in tdNode.ChildNodes) {
					if (node.Name == "div") {
						var bookItem = new BookItem();
						var coverUri = node.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["src"].Value;
						bookItem.Cover = await ImageHelper.GetImage(new System.Uri(coverUri), "cover");
						var temps = node.ChildNodes[1].ChildNodes[1].Attributes["title"].Value.Split('(');
						bookItem.Title = temps[0].Replace("\r\n", " ").Trim();//标题
						bookItem.Interlinkage = node.ChildNodes[1].ChildNodes[1].Attributes["href"].Value;
						temps = bookItem.Interlinkage.Split('/');
						bookItem.BID = temps[temps.Length - 1].Remove(temps[temps.Length - 1].Length - 4);
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
