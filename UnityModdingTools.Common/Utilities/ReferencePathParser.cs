using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace UnityModdingTools.Common.Utilities
{

    public class ReferencePathParser
    {
        public static readonly string RelativeDirTag = "{RELATIVEDIRS}";
        public static readonly string PathRegexTemplate =
            @$"^(.*?)[\\/]*({RelativeDirTag})?[/\\]*?([^/\\]*?)(\.dll|$)";
        //public static readonly string PathRegexTemplate_NoRelativeDirs = @"(.+\\)?()(.*?)(\.dll)?$";
        public readonly Regex Regex;
        public readonly string Pattern;

        public ReferencePathParser(params string[]? relativeDirs)
        {
            Pattern = MakeRegexPattern(relativeDirs);
            Regex = new Regex(Pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Attempts to parse an assembly path.
        /// Can be fully qualified path, relative path, or relative starting with a '$(ProjectProperty)'.
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public ParseResult Parse(string assemblyPath)
        {
            if (string.IsNullOrWhiteSpace(assemblyPath))
                throw new ArgumentNullException(nameof(assemblyPath));
            Match match = Regex.Match(assemblyPath);
            return new ParseResult(match);
        }

        public static ParseResult Parse(string assemblyPath, string[]? relativeDirs)
        {
            string pattern = MakeRegexPattern(relativeDirs);
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = regex.Match(assemblyPath);
            return new ParseResult(match);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeDirs"></param>
        /// <returns></returns>
        public static string MakeRegexPattern(params string[]? relativeDirs)
        {
            //if (relativeDirs == null || relativeDirs.Length == 0)
            //    return PathRegexTemplate_NoRelativeDirs;
            string regexString = PathRegexTemplate;
            if (relativeDirs != null && relativeDirs.Length > 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var dir in relativeDirs)
                {
                    if (builder.Length > 0)
                        builder.Append('|');
                    builder.Append($"{FormatRegexStr(dir, true)}");
                }
                regexString = regexString.Replace(RelativeDirTag, builder.ToString());
            }
            else
                regexString = regexString.Replace(RelativeDirTag, "");
            return regexString;
        }

        private static string FormatRegexStr(string input, bool trimDirSeparators)
        {
            string output = input;
            if (trimDirSeparators)
                output = output.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            output = output.Replace("\\", "[\\\\/]")
                .Replace(".", "\\.")
                .Replace("*", "\\");
            return output;
        }
    }


    public enum PathParserGroups
    {
        Whole = 0,
        GamePath = 1,
        RelativePath = 2,
        AssemblyName = 3,
        Extension = 4
    }

    public struct ParseResult
    {
        public static ParseResult Fail() => new ParseResult(false, null, null, null, null, null);
        /// <summary>
        /// True if there was a successful match. If false, all other fields are null.
        /// </summary>
        public readonly bool Success;
        /// <summary>
        /// The whole matched string.
        /// </summary>
        public readonly string? Match;
        /// <summary>
        /// If there were relative directories, this matches everything before those.
        /// Else this is the directory of the assembly.
        /// </summary>
        public readonly string? GamePath;
        /// <summary>
        /// Relative directory matched, if any.
        /// </summary>
        public readonly string? RelativePath;
        /// <summary>
        /// Name of the assembly without file extension.
        /// </summary>
        public readonly string? AssemblyName;
        /// <summary>
        /// Assembly extension, includes starting '.' (.dll). 
        /// </summary>
        public readonly string? Extension;

        public ParseResult(bool success, string? match, string? gamePath, string? relativePath, string? assemblyName, string? extension)
        {
            Success = success;
            Match = match;
            GamePath = NullIfEmpty(gamePath);
            RelativePath = NullIfEmpty(relativePath);
            AssemblyName = NullIfEmpty(assemblyName);
            Extension = NullIfEmpty(extension);
        }

        private static string? NullIfEmpty(string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            return str;
        }

        public ParseResult(Match match)
            : this(match.Success,
                  match.Groups[(int)PathParserGroups.Whole].Value,
                  match.Groups[(int)PathParserGroups.GamePath].Value,
                  match.Groups[(int)PathParserGroups.RelativePath].Value,
                  match.Groups[(int)PathParserGroups.AssemblyName].Value,
                  match.Groups[(int)PathParserGroups.Extension].Value)
        {

        }
    }
}
