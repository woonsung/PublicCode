using System;
using System.Drawing;

namespace PublicCommonControls.WCalendar
{
    public class MonthCalendarDay
    {
        public MonthCalendarDay(MonthCalendarMonth month, DateTime date)
        {
            this.Month = month;
            this.Date = date;
            this.MonthCalendar = month.MonthCalendar;
        }
        public Rectangle Bounds { get; set; }
        public DateTime Date { get; set; }
        public MonthCalendarMonth Month { get; private set; }
        public MonthCalendar MonthCalendar { get; private set; }
        public bool Selected
        {
            get { return this.MonthCalendar.IsSelcted(this.Date); }
        }
        public bool MouseOver
        {
            get
            {
                return this.Date == this.MonthCalendar.MouseOverDay;
            }
        }
        public bool TrailingDate
        {
            get
            {
                return this.MonthCalendar.CultureCalendar.GetMonth(this.Date) != this.MonthCalendar.CultureCalendar.GetMonth(this.Month.Date);
            }
        }
        public bool Visible
        {
            get
            {
                if (this.Date == this.MonthCalendar.ViewStart && this.MonthCalendar.ViewStart == this.MonthCalendar.MinDate)
                    return true;
                return this.Date >= this.MonthCalendar.MinDate && this.Date <= this.MonthCalendar.MaxDate && !(this.TrailingDate &&
                    this.Date >= this.MonthCalendar.ViewStart && this.Date <= this.MonthCalendar.ViewEnd);
            }
        }

    }
}
