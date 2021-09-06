using System;
using System.Collections.Generic;
using System.Text;
using UnityModdingTools.Common.Configuration;
using UnityModdingTools.Common.Models;

namespace UnityModdingTools.Common
{
    public class ReferenceFilterHandler
    {
        public static readonly char FilterORSeparator = ';';
        public static readonly string Wildcard = "*";
        public bool IsMatch(ReferenceManagerFilterSetting filterSetting, ReferenceModel reference)
        {
            string? relativeDirectory = filterSetting.RelativeDirectory;
            if (!CheckPath(relativeDirectory, reference))
                return false;
            string? includeFilter = filterSetting.IncludeFilter;
            return CheckFilter(includeFilter, reference);
        }

        public static bool CheckPath(string? dirFilter, ReferenceModel reference)
        {
            if (dirFilter != null)
            {
                if (!reference.HintPath?.StartsWith(dirFilter) ?? true)
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
                foreach (var part in filterParts)
                {
                    if (string.IsNullOrWhiteSpace(part))
                        continue;

                    bool match = true;
                    bool wildcardStart = part.StartsWith(Wildcard);
                    bool wildcardEnd = part.EndsWith(Wildcard);
                    string filter = part.Replace(Wildcard,"");
                    if (wildcardStart)
                    {
                        match = refName.EndsWith(filter);
                    }
                    if (wildcardEnd && match)
                    {
                        match = refName.StartsWith(filter);
                    }
                    if(!(wildcardStart || wildcardEnd))
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
