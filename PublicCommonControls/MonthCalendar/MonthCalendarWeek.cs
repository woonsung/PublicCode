using System;
using System.Drawing;

namespace PublicCommonControls.WCalendar
{
    public class MonthCalendarWeek
    {
        public MonthCalendarWeek(MonthCalendarMonth month, int weekNumber, DateTime start, DateTime end)
        {
            this.WeekNumber = weekNumber;
            this.Start = start;
            this.End = end;
            this.Month = month;
        }
        public MonthCalendarMonth Month { get; private set; }
        public MonthCalendar MonthCalendar
        {
            get { return this.Month.MonthCalendar; }
        }
        public int WeekNumber { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public Rectangle Bounds { get; set; }
        public bool Visible { get; set; }
    }
}
