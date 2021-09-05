using System;

namespace UnityModdingTools.Abstractions
{
    public interface IElementAttribute : IProjectComponent
    {
        string Value { get; set; }
    }
}
