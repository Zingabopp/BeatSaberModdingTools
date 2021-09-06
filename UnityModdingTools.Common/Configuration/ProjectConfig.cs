using Newtonsoft.Json;

namespace UnityModdingTools.Common.Configuration
{
    public class ProjectConfig : ConfigBase
    {
        private string? _gameDirProperty;
        /// <summary>
        /// Build property name to use for setting the game directory.
        /// </summary>
        [JsonProperty(nameof(GameDirectoryProperty))]
        public string? GameDirectoryProperty
        {
            get { return _gameDirProperty; }
            set
            {
                if (_gameDirProperty == value)
                    return;
                _gameDirProperty = value;
                NotifyPropertyChanged();
            }
        }

        public string? GetGameDirectoryProperty()
        {
            string? prop = GameDirectoryProperty;
            if (!string.IsNullOrWhiteSpace(prop))
                return prop;
            else if (Parent is ProjectConfig parent)
                return parent.GetGameDirectoryProperty();
            else
                return null;
        }
    }
}
