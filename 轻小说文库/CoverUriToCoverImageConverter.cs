using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace 轻小说文库 {
	public class CoverUriToCoverImageConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			if (value == null) {
				value = "http://www.wenku8.com/other/hexie.jpg";
			}
			return new BitmapImage(new Uri((string)value, UriKind.Absolute));
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotImplementedException();
		}
	}
}
