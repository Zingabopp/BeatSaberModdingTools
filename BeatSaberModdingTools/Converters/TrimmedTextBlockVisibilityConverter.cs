using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BeatSaberModdingTools.Converters
{
    /// <summary>
    /// Automatically sets the visibility of a tooltip based on if the text was trimmed.
    /// Source: https://stackoverflow.com/a/22714677
    /// </summary>
    public class TrimmedTextBlockVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return Visibility.Hidden;

            FrameworkElement textBlock = (FrameworkElement)value;

            textBlock.Measure(new System.Windows.Size(Double.PositiveInfinity, Double.PositiveInfinity));

            if (textBlock.ActualWidth < textBlock.DesiredSize.Width)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
