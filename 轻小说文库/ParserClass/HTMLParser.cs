using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace 轻小说文库
{
    public class HTMLParser
    {
        private static bool isInstanceReady = false;
        public static bool IsInstanceReady
        {
            get
            {   return isInstanceReady; }
            private set
            {   isInstanceReady = value; }
        }
        /// <summary>
        /// 用户名
        /// </summary>
        private static string userName;
        public static string UserName
        {
            private get { return userName; }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    instance = new HTMLParser();
                }
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public static string Password { private get; set; }

        private static HttpClient HttpClientInstance { get; set; }
        /// <summary>
        /// 获取一个HTMLParser的实例
        /// </summary>
        private static HTMLParser instance;
        public static HTMLParser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HTMLParser();
                }
                return instance;
            }
        }

        private HTMLParser()
        {
            HttpClientInstance = new HttpClient();
            if (UserName != null && Password != null)
            {
                IsInstanceReady = false;
                SetHttpClientInstance(UserName, Password);
                IsInstanceReady = true;
            }
        }
        /// <summary>
        /// 获得HttpClient实例，此实例带有Cookie，用于之后的访问
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async void SetHttpClientInstance(string userName, string password)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encodingGBK = Encoding.GetEncoding(936);

            var uri = new Uri("http://www.wenku8.com/login.php?do=submit&jumpurl=%2Fother%2Fhexie.php");
            HttpClientInstance.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:51.0) Gecko/20100101 Firefox/51.0");
            HttpClientInstance.DefaultRequestHeaders.Add("Referer", "http://www.wenku8.com/login.php?do=submit&jumpurl=%2Fother%2Fhexie.php");
            HttpClientInstance.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            HttpClientInstance.DefaultRequestHeaders.Add("Host", "www.wenku8.com");
            var httpResponse = new HttpResponseMessage();
            try
            {
                //以下内容为PostData的内容
                var paramList = new List<KeyValuePair<string, string>>();
                paramList.Add(new KeyValuePair<string, string>("username", userName));
                paramList.Add(new KeyValuePair<string, string>("password", password));
                paramList.Add(new KeyValuePair<string, string>("usecookie", "0"));
                paramList.Add(new KeyValuePair<string, string>("action", "login"));
                paramList.Add(new KeyValuePair<string, string>("submit", "%26%23160%3B%B5%C7%26%23160%3B%26%23160%3B%C2%BC%26%23160%3B"));

                httpResponse = await HttpClientInstance.PostAsync(uri, new FormUrlEncodedContent(paramList));
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                MainPage.TipsTextBlock.Text = "出错！请检查您的网络！";
                MainPage.TipsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        /// <summary>
        /// 获取目标URI的HTML源码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetHtml(string url)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encodingGBK = Encoding.GetEncoding(936);
            try
            {
                var httpResponseBody = await HttpClientInstance.GetByteArrayAsync(url);
                return encodingGBK.GetString(httpResponseBody);
            }
            catch (Exception)
            {
                MainPage.TipsTextBlock.Text = "出错！请检查您的网络！";
                MainPage.TipsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                return null;
            }    
        }
    }

}
