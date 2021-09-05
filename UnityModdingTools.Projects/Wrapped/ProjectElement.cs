using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityModdingTools.Abstractions;

namespace UnityModdingTools.Projects
{
    public class ProjectElement : ProjectComponent, IProjectElement
    {
        private Dictionary<string, IElementAttribute> _attributes = new Dictionary<string, IElementAttribute>();
        public IElementAttribute? this[string s]
        {
            get
            {
                if (_attributes.TryGetValue(s, out IElementAttribute value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                if (value != null)
                    _attributes[s] = value;
            }
        }

        public string Name { get; private set; }

        public string? Condition
        {
            get => this["Condition"]?.Value;
            set
            {
                if(value == null)
                {
                    _attributes.Remove("Condition");
                    var x = XElement.Attribute("Condition");
                    x?.Remove();
                }
                else if(_attributes.TryGetValue("Condition", out IElementAttribute condition))
                {
                    condition.Value = value;
                }
                else
                {
                    var x = new XAttribute("Condition", value);
                    var a = new ElementAttribute(this, x); 
                    XElement.Add(x);
                    this["Condition"] = a;
                }
            }
        }

        public bool IsConditioned => Condition != null && Condition.Length > 0;
        private readonly XElement XElement;
        public ProjectElement(IProjectComponent? parent, XObject node)
            : base(parent, node)
        {
            XElement = node as XElement ?? throw new ArgumentException("Node must be an XElement", nameof(node));
            Name = XElement.Name.LocalName;

        }
    }
}
