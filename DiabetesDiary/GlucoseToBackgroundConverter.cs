using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

//
// Colors the background in entry log based on glucose value
//
namespace DiabetesDiary
{
    /// <summary>
    /// Gets background color to be used for a given glucose level
    /// </summary>
    class GlucoseToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int glucose = (int)value;
                if (glucose < 70) return Brushes.Red;
                else if (glucose < 90) return Brushes.Orange;
                else if (glucose < 121) return Brushes.Green;
                else if (glucose < 160) return Brushes.Yellow;
                else if (glucose < 240) return Brushes.Orange;
                else return Brushes.Red;
            }
            //value is not an integer. Do not throw an exception
            // in the converter, but return something that is obviously wrong
            return Brushes.Pink;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
