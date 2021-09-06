using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityModdingTools.Common.Configuration
{
    public class ReferenceManagerGameFilters : ConfigBase
    {
        private string _gameId;
        [JsonProperty(nameof(GameId))]
        public string GameId
        {
            get { return _gameId; }
            set
            {
                if (_gameId == value)
                    return;
                _gameId = value ?? throw new ArgumentNullException(nameof(GameId));
                NotifyPropertyChanged();
            }
        }
        private Dictionary<string, ReferenceManagerFilterSetting> _settings;
        [JsonProperty(nameof(Settings))]
        public IEnumerable<ReferenceManagerFilterSetting> Settings => _settings.Values.AsEnumerable();
        public void Remove(ReferenceManagerFilterSetting setting)
        {
            if (_settings.Remove(setting.Name))
            {
                RemoveChild(setting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Thrown if attempting to add a GameId that already exists.</exception>
        public void Add(ReferenceManagerFilterSetting setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));
            _settings.Add(setting.Name, setting);
            AddChild(setting);
        }

        public bool TryGetGameSetting(string gameId, out ReferenceManagerFilterSetting setting)
        {
            return _settings.TryGetValue(gameId, out setting);
        }

        public ReferenceManagerGameFilters(
            [JsonProperty(nameof(GameId))] string gameId, 
            [JsonProperty(nameof(Settings))] IEnumerable<ReferenceManagerFilterSetting> settings)
        {
            _gameId = gameId;
            _settings = new Dictionary<string, ReferenceManagerFilterSetting>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in settings)
            {
                _settings.Add(s.Name, s);
                AddChild(s);
            }
        }
    }
}
