using System;
using System.Collections.Generic;
using System.Text;

namespace UnityModdingTools.Common.Models
{
    public class ReferenceModel
    {
        public string Name { get; }
        public string? HintPath { get; set; }
        /// <summary>
        /// CopyLocal
        /// </summary>
        public CopyLocal Private { get; set; }
        public string? Condition { get; set; }
        public bool StartedInProject { get; set; }
        public ReferenceModel(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            HintPath = null;
            Private = CopyLocal.Undefined;
            Condition = null;
            StartedInProject = false;
        }

        public ReferenceModel(string name, bool startedInProject)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            HintPath = null;
            Private = CopyLocal.Undefined;
            Condition = null;
            StartedInProject = startedInProject;
        }

        public override bool Equals(object? obj)
        {
            return obj is ReferenceModel model &&
                   Name == model.Name &&
                   HintPath == model.HintPath &&
                   Private == model.Private;
        }

        public override int GetHashCode()
        {
            int hashCode = 1926118681;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string?>.Default.GetHashCode(HintPath);
            hashCode = hashCode * -1521134295 + Private.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            if (Condition != null)
                return Name + " | " + Condition;
            else
                return Name;
        }

        public static bool operator ==(ReferenceModel left, ReferenceModel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReferenceModel left, ReferenceModel right)
        {
            return !(left == right);
        }
    }

    public enum CopyLocal
    {
        Undefined,
        True,
        False
    }
}
