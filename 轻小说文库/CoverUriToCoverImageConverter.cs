using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using 轻小说文库.ParserClass;

namespace 轻小说文库 {
	public class CoverUriToCoverImageConverter : IValueConverter {
		private static HttpClient httpClient;
		//private static WriteableBitmap writeableBitMap;
		//private static Stream stream2WriteableBitMap;

		public CoverUriToCoverImageConverter() {
			httpClient = new HttpClient();
		}

		public object Convert(object value, Type targetType, object parameter, string language) {
			// 如果 value==null ，从本地加载代表图片空缺的图片

			// 检测本地存储中是否有本图片，如果有，从本地加载

			// 如果本地没有，下载图片并保存

			if (value == null) {
				value = "http://www.wenku8.com/other/hexie.jpg";
			}

			//GetWriteableBitmapAsync((string)value);
			//var image = new BitmapImage(new Uri((string)value, UriKind.Absolute));
			//return image;
			BitmapImage image = null;
			//ImageHelper.GetImage(new Uri((string)value));
			return image;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotImplementedException();
		}

		private static async void GetWriteableBitmapAsync(string uri) {
			try {
				var imgBuffer = await httpClient.GetBufferAsync(new Uri(uri));
				if (imgBuffer != null) {
					var bitmapImg = new BitmapImage();
					WriteableBitmap writeableBitMap = null;
					Stream stream2WriteableBitMap = null;
					using (var stream = new InMemoryRandomAccessStream()) {
						stream2WriteableBitMap = stream.AsStreamForWrite();
						stream2WriteableBitMap.Write(imgBuffer.ToArray(), 0, (int)imgBuffer.Length);
						stream2WriteableBitMap.Flush();
						stream.Seek(0);
						bitmapImg.SetSource(stream);
						writeableBitMap = new WriteableBitmap(bitmapImg.PixelWidth, bitmapImg.PixelHeight);
						stream.Seek(0);
						writeableBitMap.SetSource(stream);
						SaveImageAsync(writeableBitMap, uri);
						//return writeableBitMap;
					}
				}
				else {
					//return null;
				}
			}
			catch (Exception) {
				//return null;
			}
		}
		private static async void SaveImageAsync(WriteableBitmap image, string filename) {
			try {
				if (image == null) {
					return;
				}
				Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
				if (filename.EndsWith("jpg"))
					BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
				else if (filename.EndsWith("png"))
					BitmapEncoderGuid = BitmapEncoder.PngEncoderId;
				else if (filename.EndsWith("bmp"))
					BitmapEncoderGuid = BitmapEncoder.BmpEncoderId;
				else if (filename.EndsWith("tiff"))
					BitmapEncoderGuid = BitmapEncoder.TiffEncoderId;
				else if (filename.EndsWith("gif"))
					BitmapEncoderGuid = BitmapEncoder.GifEncoderId;
				StorageFolder _local_folder = KnownFolders.PicturesLibrary;
				var folder = await _local_folder.CreateFolderAsync("images_cache", CreationCollisionOption.OpenIfExists);
				var subs = filename.Split('/');
				var name = subs[subs.Length - 1];
				var file = await folder.CreateFileAsync(name, CreationCollisionOption.FailIfExists);

				using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite)) {
					BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);
					Stream pixelStream = image.PixelBuffer.AsStream();
					byte[] pixels = new byte[pixelStream.Length];
					await pixelStream.ReadAsync(pixels, 0, pixels.Length);
					encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
							  (uint)image.PixelWidth,
							  (uint)image.PixelHeight,
							  96.0,
							  96.0,
							  pixels);
					await encoder.FlushAsync();
				}

			}
			catch {

			}
		}

	}
}
