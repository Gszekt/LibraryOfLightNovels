using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace 轻小说文库 {
	public class HTMLParser {
		private static HttpClient httpClientInstance;

		/// <summary>
		/// 获取一个HTMLParser的实例
		/// </summary>
		private static HTMLParser instance;
		public static HTMLParser Instance {
			get {
				if (instance == null) {
					instance = new HTMLParser();
				}
				return instance;
			}
		}

		/// <summary>
		/// 获得HttpClient实例，此实例带有Cookie，用于之后的访问
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static async Task<bool> SetHttpClientInstanceAsync(string userName, string password) {
			httpClientInstance = new HttpClient();
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Encoding encodingGBK = Encoding.GetEncoding(936);

			var uri = new Uri("http://www.wenku8.com/login.php?do=submit&jumpurl=%2Fother%2Fhexie.php");
			httpClientInstance.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:51.0) Gecko/20100101 Firefox/51.0");
			httpClientInstance.DefaultRequestHeaders.Add("Referer", "http://www.wenku8.com/login.php?do=submit&jumpurl=%2Fother%2Fhexie.php");
			httpClientInstance.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
			httpClientInstance.DefaultRequestHeaders.Add("Host", "www.wenku8.com");
			var httpResponse = new HttpResponseMessage();
			try {
				//以下内容为PostData的内容
				var paramList = new List<KeyValuePair<string, string>>();
				paramList.Add(new KeyValuePair<string, string>("username", userName));
				paramList.Add(new KeyValuePair<string, string>("password", password));
				paramList.Add(new KeyValuePair<string, string>("usecookie", "0"));
				paramList.Add(new KeyValuePair<string, string>("action", "login"));
				paramList.Add(new KeyValuePair<string, string>("submit", "%26%23160%3B%B5%C7%26%23160%3B%26%23160%3B%C2%BC%26%23160%3B"));

				httpResponse = await httpClientInstance.PostAsync(uri, new FormUrlEncodedContent(paramList));
				httpResponse.EnsureSuccessStatusCode();
				return true;
			}
			catch {
				return false;
			}
		}

		public static HttpClient GetHttpClientInstance() {
			return httpClientInstance;
		}

		/// <summary>
		/// 获取目标URI的HTML源码
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public async Task<string> GetHtml(string url) {
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Encoding encodingGBK = Encoding.GetEncoding(936);
			try {
				var httpResponseBody = await httpClientInstance.GetByteArrayAsync(url);
				return encodingGBK.GetString(httpResponseBody);
			}
			catch (Exception) {
				// 理论上应该返回特征码，之后补上
				return null;
			}
		}
	}

}
