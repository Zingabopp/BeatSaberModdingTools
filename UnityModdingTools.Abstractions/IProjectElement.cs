using System;

namespace UnityModdingTools.Abstractions
{
    public interface IProjectElement : IProjectComponent
    {
        IElementAttribute? this[string s] { get; set; }
        string Name { get; }

        public bool IsConditioned { get; }

        string? Condition { get; set; }
    }
}
