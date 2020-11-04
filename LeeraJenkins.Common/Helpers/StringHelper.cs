using System;

namespace LeeraJenkins.Common.Helpers
{
    public static class StringHelper
    {
        public static bool IsNotSame(string value1, string value2)
        {
            if (value1 == null && value2 == null)
            {
                return true;
            }
            if (String.IsNullOrWhiteSpace(value1) != String.IsNullOrWhiteSpace(value2))
            {
                return true;
            }
            return !value2.ToLower().Equals(value1?.ToLower());
        }
    }
}
