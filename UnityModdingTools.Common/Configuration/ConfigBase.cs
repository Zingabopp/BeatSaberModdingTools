using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace UnityModdingTools.Common.Configuration
{
    public abstract class ConfigBase : INotifyPropertyChanged
    {
        public bool IsDirty { get; private set; }
        private ConfigBase? _parent;
        public ConfigBase? Parent
        {
            get => _parent;
            protected set
            {
                if (this == value || _parent == value)
                    return;
                if (_parent != null)
                    _parent.DirtyReset -= OnDirtyReset;
                _parent = value;
                if (_parent != null)
                    _parent.DirtyReset += OnDirtyReset;
            }
        }

        public void AddChild(ConfigBase config)
            => ReplaceChild(null, config);

        public void RemoveChild(ConfigBase config)
            => ReplaceChild(config, null);

        public void ReplaceChild(ConfigBase? oldConfig, ConfigBase? newConfig)
        {
            if (oldConfig != null)
                oldConfig.Parent = null;
            if (newConfig != null)
                newConfig.Parent = this;
        }

        protected void ResetDirty()
        {
            IsDirty = false;
            DirtyReset?.Invoke(this, EventArgs.Empty);
        }

        protected event EventHandler? DirtyReset;
        private void OnDirtyReset(object sender, EventArgs e)
        {
            IsDirty = false;
        }
        public event EventHandler? ConfigChanged;
        protected void RaiseConfigChanged()
        {
            IsDirty = true;
            ConfigChanged?.Invoke(this, EventArgs.Empty);
            Parent?.RaiseConfigChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            RaiseConfigChanged();
        }
    }
}
