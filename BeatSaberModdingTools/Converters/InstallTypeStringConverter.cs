using BeatSaberModdingTools.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BeatSaberModdingTools.Converters
{
    public class InstallTypeStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InstallType installType)
            {
                switch (installType)
                {
                    case InstallType.Steam:
                        return "Steam";
                    case InstallType.Oculus:
                        return "Oculus";
                    case InstallType.Manual:
                        return "Manual";
                    default:
                        break;
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value is string typeStr)
            {
                switch (typeStr)
                {
                    case "Steam":
                        return InstallType.Steam;
                    case "Oculus":
                        return InstallType.Oculus;
                    case "Manual":
                        return InstallType.Manual;
                }
            }
            return InstallType.Manual;
        }
    }
}
