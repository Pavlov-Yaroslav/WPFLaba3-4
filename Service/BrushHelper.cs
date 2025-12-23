using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1.Service
{
    public static class BrushHelper
    {
        public static Brush GetBrush(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return Brushes.Black;

            var converter = new BrushConverter();

            if (converter.IsValid(color))
            {
                return (Brush)converter.ConvertFromString(
                    null,
                    CultureInfo.InvariantCulture,
                    color);
            }

            return Brushes.Black;
        }
    }

}
