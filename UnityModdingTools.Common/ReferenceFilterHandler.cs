using System;
using System.IO;
using UnityModdingTools.Common.Configuration;
using UnityModdingTools.Common.Models;
using UnityModdingTools.Common.Utilities;

namespace UnityModdingTools.Common
{
    public class ReferenceFilterHandler
    {
        public readonly ReferencePathParser PathParser;

        public static readonly char FilterORSeparator = ';';
        public static readonly string Wildcard = "*";

        public ReferenceFilterHandler(params string[] relativeDirs)
        {
            PathParser = new ReferencePathParser(relativeDirs);
        }



        public bool IsMatch(ReferenceManagerFilterSetting filterSetting, ReferenceModel reference)
        {
            string? relativeDirectory = filterSetting.RelativeDirectory;
            if (!CheckPath(relativeDirectory, reference))
                return false;
            string? includeFilter = filterSetting.IncludeFilter;
            return CheckFilter(includeFilter, reference);
        }

        public bool CheckPath(string? dirFilter, ReferenceModel reference)
        {
            if (dirFilter != null)
            {
                // There is a directory filter, but no path in the reference
                if (string.IsNullOrWhiteSpace(reference.HintPath))
                    return false;
#pragma warning disable CS8604 // Possible null reference argument.
                ParseResult match = PathParser.Parse(reference.HintPath);
#pragma warning restore CS8604 // Possible null reference argument.
                dirFilter = dirFilter.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (!dirFilter.Equals(match.RelativePath?.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckFilter(string? includeFilter, ReferenceModel reference)
        {
            if (string.IsNullOrWhiteSpace(includeFilter))
                return false;
            if (includeFilter == "*")
                return true;
            string refName = reference.Name;
            if (includeFilter != null)
            {
                string[] filterParts = includeFilter.Split(FilterORSeparator);
                foreach (string? part in filterParts)
                {
                    if (string.IsNullOrWhiteSpace(part))
                        continue;

                    bool match = true;
                    bool wildcardStart = part.StartsWith(Wildcard);
                    bool wildcardEnd = part.EndsWith(Wildcard);
                    string filter = part.Replace(Wildcard, "");
                    if (wildcardStart)
                    {
                        match = refName.EndsWith(filter);
                    }
                    if (wildcardEnd && match)
                    {
                        match = refName.StartsWith(filter);
                    }
                    if (!(wildcardStart || wildcardEnd))
                    {
                        match = refName.Equals(filter, StringComparison.OrdinalIgnoreCase);
                    }
                    if (match)
                        return true;
                }
            }
            return false;
        }
    }
}
