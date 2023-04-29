using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using draw = System.Drawing;

namespace InventoryManager.Converters
{
    public class ExpireToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush selectedColor = new SolidColorBrush(Color.FromArgb(draw.Color.Transparent.A, draw.Color.Transparent.R, draw.Color.Transparent.G, draw.Color.Transparent.B));
            if (value is DateTime exp)
            {
                var timeleft = exp.Subtract(DateTime.Now).Days;
                if (timeleft < 120 && timeleft > 0)
                    selectedColor = new SolidColorBrush(Color.FromRgb(draw.Color.Yellow.R, draw.Color.Yellow.G, draw.Color.Yellow.B));
                else if (timeleft <= 0)
                    selectedColor = new SolidColorBrush(Color.FromRgb(draw.Color.IndianRed.R, draw.Color.IndianRed.G, draw.Color.IndianRed.B));
                else selectedColor = new SolidColorBrush(Color.FromArgb(draw.Color.Transparent.A, draw.Color.Transparent.R, draw.Color.Transparent.G, draw.Color.Transparent.B));
            }
            return selectedColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
