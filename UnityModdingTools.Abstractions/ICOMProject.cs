using System;
using System.Collections.Generic;
using System.Text;

namespace UnityModdingTools.Abstractions
{
    public interface ICOMProject
    {
        bool IsDirty { get; }
        void Unload();
        void Load();
    }
}
