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
    public class InstallTypeImageConverter : IValueConverter
    {
        private const string SteamIconPath = "BeatSaberModTemplates.Icons.Steam.png";
        private const string OculusIconPath = "BeatSaberModTemplates.Icons.Oculus.png";
        private const string ManualIconPath = "BeatSaberModTemplates.Icons.Manual.png";

        static readonly Lazy<BitmapImage> SteamIcon = new Lazy<BitmapImage>(() => LoadImageFromResource(SteamIconPath));
        static readonly Lazy<BitmapImage> OculusIcon = new Lazy<BitmapImage>(() => LoadImageFromResource(OculusIconPath));
        static readonly Lazy<BitmapImage> ManualIcon = new Lazy<BitmapImage>(() => LoadImageFromResource(ManualIconPath));

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
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value is BitmapImage image)
            {
                switch (image.UriSource.OriginalString)
                {
                    case SteamIconPath:
                        return InstallType.Steam;
                    case OculusIconPath:
                        return InstallType.Oculus;
                    default:
                        return InstallType.Manual;
                }
            }
            return InstallType.Manual;
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
