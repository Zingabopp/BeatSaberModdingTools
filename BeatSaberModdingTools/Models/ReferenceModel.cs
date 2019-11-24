using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.Models
{
    public class ReferenceModel
    {
        public string Name { get; private set; }
        public string HintPath { get; private set; }
        public ReferenceModel(string name, string hintPath = "")
        {
            Name = name;

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
