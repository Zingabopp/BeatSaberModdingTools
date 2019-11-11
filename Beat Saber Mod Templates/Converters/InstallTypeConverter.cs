using BeatSaberModTemplates.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BeatSaberModTemplates.Converters
{
    public class InstallTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InstallType installType)
                return installType.ToString();
            return "ERROR";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            switch(value.ToString().ToLower())
            {
                case "steam":
                    return InstallType.Steam;
                case "oculus":
                    return InstallType.Oculus;
            }
            return null;
        }
    }
}
