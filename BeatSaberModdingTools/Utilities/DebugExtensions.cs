using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.Utilities
{
    public static class DebugExtensions
    {
        public static string[] GetProperties(this object obj)
        {
            return obj?.GetType().GetProperties().Select(p =>
            {
                string name = p.Name;
                string value = null;
                try
                {
                    value = p.GetValue(obj)?.ToString() ?? "NULL";
                }
                catch (Exception ex)
                {
                    value = $"ERROR: {ex.Message}";
                }
                return $"{name}: {value}";
            }).ToArray() ?? Array.Empty<string>();
        }
        public static Type[] GetInterfaces(this object obj)
        {
            return obj?.GetType().GetInterfaces();
        }
    }
}
