using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace UnityModdingTools.Abstractions
{
    public interface IProjectComponent
    {
        IProjectComponent? Parent { get; }
        IEnumerable<IProjectComponent> Content { get; }
        XObject XObject { get; }
        ComponentState State { get; }
        void SetState(ComponentState state);
    }
}
