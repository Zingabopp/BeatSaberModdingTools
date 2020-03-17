using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_Tests.BuildTasks
{

    public class TestLogger
    {
        public List<string> Messages = new List<string>();
        public void LogErrorFromException(Exception ex)
        {
            string message = $"{ex.Message}";
            Messages.Add(message);
            Console.Write(message);
        }
        public void LogMessage(MessageImportance importance, string message, params object[] messageArgs)
        {
            string msg = string.Format($"{importance.ToString()}: {message}", messageArgs);
            Messages.Add(msg);
            Console.WriteLine(msg);
        }
        public void LogError(string subcategory, string errorCode, string helpKeyword, string file,
            int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
        {
            string msg = string.Format($"ERROR: {subcategory}.{errorCode} | {file}({lineNumber}-{endLineNumber}:{columnNumber}-{endColumnNumber}): {message}", messageArgs);
            Messages.Add(msg);
            Console.WriteLine(msg);
        }
        public void LogWarning(string subcategory, string warningCode, string helpKeyword, string file,
            int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
        {
            string msg = string.Format($"Warning: {subcategory}.{warningCode} | {file}({lineNumber}-{endLineNumber}:{columnNumber}-{endColumnNumber}): {message}", messageArgs);
            Messages.Add(msg);
            Console.WriteLine(msg);
        }
    }
}
