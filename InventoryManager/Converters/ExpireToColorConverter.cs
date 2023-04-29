using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace InventoryManager.Converters
{
    public class ExpireToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color colorSelected = Color.White;
            var now = DateTime.Now;
            if(value is DateTime exp)
            {
                if (now.Year - exp.Year > 0 && now.Month - exp.Month > 5)
                    return Color.Green;
                if (now.Year - exp.Year == 0 && now.Month - exp.Month < 5 )
                    colorSelected = Color.Yellow;
                if (now.Year - exp.Year <= 0 && now.Month - exp.Month <= 0)
                    colorSelected = Color.Red;
            }
            return colorSelected;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTime.Now;
        }
    }
}
