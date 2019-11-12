using BeatSaberModTemplates.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace BeatSaberModTemplates.Converters
{
    public class InstallTypeConverter : IValueConverter
    {
        static InstallTypeConverter()
        {
            
        }

        static Lazy<BitmapImage> SteamIcon = new Lazy<BitmapImage>(() => LoadImageFromResource("BeatSaberModTemplates.Icons.Steam.png"));
        static Lazy<BitmapImage> OculusIcon;
        static Lazy<BitmapImage> ManualIcon;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InstallType installType)
            {
                switch (installType)
                {
                    case InstallType.Steam:
                        return SteamIcon?.Value;
                    case InstallType.Oculus:
                        return OculusIcon?.Value;
                    case InstallType.Manual:
                        return ManualIcon?.Value;
                    default:
                        break;
                }
            }
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

        private static BitmapImage LoadImageFromResource(string path)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
