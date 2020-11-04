using System;

namespace LeeraJenkins.Model.Calendar
{
    public class CalendarElement
    {
        public DateTime? Day { get; set; }
        public int WeekNum { get; set; }
        public bool IsDay { get; set; }
        public string Value { get; set; }
    }
}
