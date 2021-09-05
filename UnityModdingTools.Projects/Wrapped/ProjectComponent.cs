using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityModdingTools.Abstractions;

namespace UnityModdingTools.Projects
{
    public abstract class ProjectComponent : IProjectComponent
    {
        public IProjectComponent? Parent { get; private set; }

        public IEnumerable<IProjectComponent> Content => throw new NotImplementedException();

        public XObject XObject { get; }

        public ComponentState State { get; private set; }

        public void SetState(ComponentState state)
        {
            State = state;
        }

        protected ProjectComponent(IProjectComponent? parent, XObject node)
        {
            Parent = parent;
            XObject = node;
        }
    }
}
