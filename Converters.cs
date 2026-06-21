using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PDFCompress
{
    internal class CompressToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isConverted)
            {
                if (isConverted == true)
                    return Brushes.DarkGreen;
                else
                {
                    return Brushes.DarkRed;
                }
            }

            return Brushes.Black; // Fallback color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class IsCompressedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null) {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
