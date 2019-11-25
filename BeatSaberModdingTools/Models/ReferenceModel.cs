using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BeatSaberModdingTools.Models
{
    public class ReferenceModel
    {
        public string Name { get; private set; }
        public string HintPath { get; private set; }
        public XElement ParentGroup { get; private set; }
        public ReferenceModel(string name, XElement parentGroup, string hintPath = "")
        {
            Name = name;
            ParentGroup = parentGroup;
            HintPath = hintPath;
        }

        public string ToString(int padding)
        {
            string retVal = Name;
            if (!string.IsNullOrEmpty(HintPath))
                retVal = retVal.PadRight(padding) + " | " + HintPath;
            return retVal;
        }

        public override string ToString()
        {
            return ToString(0);
        }
    }
}
