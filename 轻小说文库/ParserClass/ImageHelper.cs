using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace 轻小说文库.ParserClass {
	// 参照http://blog.csdn.net/lindexi_gd/article/details/53425673
	class ImageHelper {
		private static string kind;

		/// <summary>
		/// 获取图片，如果本地为缓存，则从网络下载并保存值本地
		/// </summary>
		/// <param name="uri">图片Uri</param>
		/// <returns>Uri对应的BitmapImage类型的图片对象</returns>
		public static async Task<BitmapImage> GetImage(Uri uri, string imageKind) {
			kind = imageKind;
			return await GetLoacalFolderImage(uri) ?? await GetHttpImage(uri);
		}

		// 从本地获取图片
		private static async Task<BitmapImage> GetLoacalFolderImage(Uri uri) {
			var folder = await GetImageFolder();
			var imageName = GetImgName(uri.AbsolutePath);
			try {
				var file = await folder.GetFileAsync(imageName);
				using (var stream = await file.OpenAsync(FileAccessMode.Read)) {
					var img = new BitmapImage();
					await img.SetSourceAsync(stream);
					return img;
				}
			}
			catch (Exception) {
				return null;
			}
		}

		// 从网络获取图片
		private static async Task<BitmapImage> GetHttpImage(Uri uri) {
			try {
				var http = new Windows.Web.Http.HttpClient();
				var buffer = await http.GetBufferAsync(uri);
				var img = new BitmapImage();
				using (IRandomAccessStream stream = new InMemoryRandomAccessStream()) {
					await stream.WriteAsync(buffer);
					stream.Seek(0);
					await img.SetSourceAsync(stream);
					await StorageImageFolder(stream, uri);
					return img;
				}
			}
			catch (Exception) {
				return null;
			}
		}

		// 保存图片到本地
		private static async Task StorageImageFolder(IRandomAccessStream stream, Uri uri) {
			var folder = await GetImageFolder();
			var imageName = GetImgName(uri.AbsolutePath);
			try {
				var file = await folder.CreateFileAsync(imageName);
				await FileIO.WriteBytesAsync(file, await ConvertIRandomAccessStreamByte(stream));
			}
			catch (Exception) {
			}
		}

		// 获取图片的名称
		private static string GetImgName(string uri) {
			var subs = uri.Split('/');
			return subs[subs.Length - 1];
		}

		private static async Task<byte[]> ConvertIRandomAccessStreamByte(IRandomAccessStream stream) {
			var read = new DataReader(stream.GetInputStreamAt(0));
			await read.LoadAsync((uint)stream.Size);
			var temp = new byte[stream.Size];
			read.ReadBytes(temp);
			return temp;
		}

		// 获取保存图片的文件夹
		private static async Task<StorageFolder> GetImageFolder() {
			string folderName = "imageCache";
			StorageFolder folder = null;
			//从本地获取文件夹
			if (kind == "cover") {
				folder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
			}
			else if (kind == "illustration") {
				folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
			}
			return folder;
		}

		// 计算Md5
		//private static string Md5(string str) {
		//	HashAlgorithmProvider hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
		//	CryptographicHash cryptographic = hashAlgorithm.CreateHash();
		//	IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
		//	cryptographic.Append(buffer);
		//	return CryptographicBuffer.EncodeToHexString(cryptographic.GetValueAndReset());
		//}
	}
}
