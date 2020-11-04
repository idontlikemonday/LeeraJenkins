using System;
using System.Collections.Generic;
using System.Linq;
using LeeraJenkins.Model.Calendar;
using LeeraJenkins.Resources;

namespace LeeraJenkins.Logic.Helpers
{
    public static class CalendarHelper
    {
        public static List<List<CalendarElement>> GetMonthCalendarElementsAsMatrix(int year, int month)
        {
            var calendarElements = new List<List<CalendarElement>>();
            calendarElements.Add(GetDaysOfWeek());

            var days = GetMonthDays(year, month);
            int weekNum = 1;
            for (int i = 0; i < days.Count; i++)
            {
                days[i].WeekNum = weekNum;
                if (days[i]?.Day?.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekNum++;
                }
            }

            AdjustFirstAndLastWeeks(days);

            var weeks = days.GroupBy(d => d.WeekNum).ToDictionary(x => x.Key, y => y.Select(d => d));
            foreach (var week in weeks)
            {
                var elements = week.Value
                    .Select(d => d)
                    .ToList();
                calendarElements.Add(elements);
            }

            return calendarElements;
        }

        private static void AdjustFirstAndLastWeeks(List<CalendarElement> days)
        {
            var firstWeekDays = days.Where(d => d.WeekNum == 1);
            for (int i = firstWeekDays.Count(); i < 7; i++)
            {
                days.Insert(0, new CalendarElement()
                {
                    Day = null,
                    WeekNum = 1,
                    IsDay = false,
                    Value = " "
                });
            }
            var maxWeekNum = days.Max(d => d.WeekNum);
            var lastWeekDays = days.Where(d => d.WeekNum == maxWeekNum);
            for (int i = lastWeekDays.Count(); i < 7; i++)
            {
                days.Add(new CalendarElement()
                {
                    Day = null,
                    WeekNum = maxWeekNum,
                    IsDay = false,
                    Value = " "
                });
            }
        }

        private static List<CalendarElement> GetNextDays(int count)
        {
            var days = new List<CalendarElement>();
            for (int i = 0; i < count; i++)
            {
                var day = new CalendarElement() { Day = DateTime.Now.AddDays(i) };
                days.Add(day);
            }

            return days;
        }

        private static List<CalendarElement> GetMonthDays(int year, int month)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var daysDuration = (lastDayOfMonth - firstDayOfMonth).Days + 1;

            var days = new List<CalendarElement>();
            for (int i = 0; i < daysDuration; i++)
            {
                var day = new CalendarElement() { Day = firstDayOfMonth.AddDays(i) };
                days.Add(day);
            }

            return days;
        }

        private static List<CalendarElement> GetDaysOfWeek()
        {
            var result = new List<CalendarElement>();
            var daysOfWeek = Misc.DaysOfWeek.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var day in daysOfWeek)
            {
                result.Add(
                    new CalendarElement()
                    {
                        IsDay = false,
                        Value = day
                    }
                );
            }

            return result;
        }
    }
}
