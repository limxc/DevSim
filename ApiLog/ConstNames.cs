using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLog
{
    internal static class ConstNames
    {
        internal const string DBPath = "DB/ApiLogs.db";
        internal static readonly string[] IgnorePostfixs = [".css", ".js", ".json", ".map", ".png", ".ico"];
        internal static readonly string[] IgnorePrefixs = ["/_", "/swagger", "/apilog", ".map"];
    }
}