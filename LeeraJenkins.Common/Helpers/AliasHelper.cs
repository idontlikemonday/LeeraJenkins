using System;
using System.Linq;

namespace LeeraJenkins.Common.Helpers
{
    public class AliasHelper
    {
        public static bool Contains(string value, string aliasesString)
        {
            var aliases = aliasesString.Split(new char[] { ',', ' ' });
            return aliases.Contains(value?.ToLower());
        }
    }
}
