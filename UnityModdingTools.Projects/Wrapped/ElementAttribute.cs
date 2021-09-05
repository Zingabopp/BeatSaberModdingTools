using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityModdingTools.Abstractions;

namespace UnityModdingTools.Projects
{
    public class ElementAttribute : ProjectComponent, IElementAttribute
    {
        private readonly XAttribute XML;
        public string Value
        {
            get => XML.Value;
            set => XML.Value = value;
        }

        public ElementAttribute(IProjectComponent? parent, XObject node) 
            : base(parent, node)
        {
            XML = node as XAttribute ?? throw new ArgumentException("node must be an XAttribute.", nameof(node));
        }

    }
}
