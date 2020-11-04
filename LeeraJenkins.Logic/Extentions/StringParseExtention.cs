using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LeeraJenkins.Logic.Extentions
{
    public static class StringParseExtention
    {
        /// <param name="dateString">String example: 15 АВГУСТА ЧЕТВЕРГ - ОФИЦИАЛЬНЫЙ ИГРОВЕЧЕР</param>
        public static DateTime TryGetDate(this string dateString)
        {
            var dateRegex = new Regex(@"(\d{1,2})\s+(\w+)");
            var date = dateRegex.Match(dateString);

            DateTime result;
            if (DateTime.TryParseExact(date.Value.ToLower(), "dd MMMM", new CultureInfo("ru-RU"), DateTimeStyles.None, out result))
            {
                return result;
            }

            return new DateTime();
        }

        /// <param name="timeString">String example: 19:00</param>
        public static DateTime TryGetTime(this string timeString)
        {
            var timeRegex = new Regex(@"\d{1,2}:\d{1,2}");
            var time = timeRegex.Match(timeString);

            DateTime result;
            if (DateTime.TryParseExact(time.Value.ToLower(), "HH:mm", new CultureInfo("ru-RU"), DateTimeStyles.None, out result))
            {
                return result;
            }

            return new DateTime();
        }
    }
}
