using System;
using System.Globalization;
using System.Text.RegularExpressions;
using LeeraJenkins.Common.Helpers;
using LeeraJenkins.Resources;

namespace LeeraJenkins.Common.Extentions
{
    public static class StringExtention
    {
        public static string GetTgName(this string fullName)
        {
            var tgNameRegex = new Regex(@"@\w+");
            var tgNameMatch = tgNameRegex.Match(fullName);
            return tgNameMatch.Value;
        }

        public static string ToFirstUpperLetter(this string fullName)
        {
            if (String.IsNullOrEmpty(fullName))
            {
                return String.Empty;
            }
            else if (fullName.Length == 1)
            {
                return fullName.ToUpper();
            }
            else
            {
                return $"{ Char.ToUpper(fullName[0]) }{ fullName.Substring(1) }";
            }
        }

        public static bool IsLogicallyEmptyAsPlayer(this string value)
        {
            if (value == null)
            {
                return true;
            }
            return String.IsNullOrWhiteSpace(value) || AliasHelper.Contains(value, Aliases.EmptySlots);
        }

        public static string ToTableDayRowFormat(this DateTime day)
        {
            var result = day.ToString("dd MMMM, dddd", new CultureInfo("ru-RU"));
            return result;
        }
    }
}
