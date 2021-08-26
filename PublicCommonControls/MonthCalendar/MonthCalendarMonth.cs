using System;
using System.Collections.Generic;
using System.Drawing;

namespace PublicCommonControls.WCalendar
{
    public class MonthCalendarMonth
    {
        private Point location;
        private DateTime firstVisibleDate = DateTime.MinValue;
        private DateTime lastVisibleDate = DateTime.MaxValue;
        public MonthCalendarMonth(MonthCalendar monthCal, DateTime date)
        {
            this.MonthCalendar = monthCal;
            this.Date = date;
            this.location = new Point(0, 0);
            MonthCalendarDate dt = new MonthCalendarDate(monthCal.CultureCalendar, date).FirstOfMonth.GetFirstDayInWeek(monthCal.FormatProvider);
            List<MonthCalendarDay> dayList = new List<MonthCalendarDay>();
            List<MonthCalendarWeek> weekList = new List<MonthCalendarWeek>();
            int dayAdjust = 0;
            while(dt.AddDays(dayAdjust).DayOfWeek != monthCal.FormatProvider.FirstDayOfWeek)
            {
                dayAdjust++;
            }
            int d = dayAdjust != 0 ? 8 - dayAdjust : 0;
            for(int i  = dayAdjust; i < 42 + dayAdjust; i++, dt = dt.AddDays(1))
            {
                MonthCalendarDay day = new MonthCalendarDay(this, dt.Date);
                dayList.Add(day);
                if(day.Visible)
                {
                    if (this.firstVisibleDate == DateTime.MinValue)
                        this.firstVisibleDate = dt.Date;
                    if (!day.TrailingDate)
                        this.lastVisibleDate = dt.Date;
                }
                if(i == dayAdjust || ((i - d) %7) == 0)
                {
                    DateTime weekEnd = dt.GetEndDateOfWeek(monthCal.FormatProvider).Date;
                    int weekNumEnd = DateMethods.GetWeekOfYear(monthCal.Culture, monthCal.CultureCalendar, weekEnd);
                    weekList.Add(new MonthCalendarWeek(this, weekNumEnd, dt.Date, weekEnd));
                }
                if(dt.Date == monthCal.CultureCalendar.MaxSupportedDateTime.Date)
                {
                    break;
                }
            }
            this.Days = dayList.ToArray();
            this.Weeks = weekList.ToArray();
        }
        public int Index { get; set; }
        public Rectangle Bounds
        {
            get { return new Rectangle(this.location, this.Size); }
        }
        public MonthCalendar MonthCalendar { get; private set; }
        public DateTime Date { get; private set; }
        public Point Location
        {
            get { return this.location; }
            set
            {
                this.location = value;
                this.CalculateProportions(value);
            }
        }
        public MonthCalendarDay[] Days { get; private set; }
        public Rectangle DayNamesBounds { get; set; }
        public MonthCalendarWeek[] Weeks { get; set; }
        public Rectangle TitleBounds { get; set; }
        public Rectangle TitleMonthBounds { get; set; }
        public Rectangle TitleYearBounds { get; set; }
        public Rectangle WeekBounds { get; set; }
        public Rectangle MonthBounds { get; set; }
        public Size Size
        {
            get { return this.MonthCalendar.MonthSize; }
        }
        public DateTime FirstVisibleDate
        {
            get { return this.lastVisibleDate; }
        }
        public DateTime LastVisibleDate
        {
            get { return this.lastVisibleDate; }
        }
        internal bool DrawLeftButton
        {
            get
            {
                return this.MonthCalendar.UseRTL ? this.Index == this.MonthCalendar.CalendarDimensions.Width - 1 : this.Index == 0;
            }
        }
        internal bool DrawRightButton
        {
            get
            {
                return this.MonthCalendar.UseRTL ? this.Index == 0 : this.Index == this.MonthCalendar.CalendarDimensions.Width - 1;
            }
        }
        public MonthCalendarHitTest HitTest(Point loc)
        {
            if (this.TitleBounds.Contains(loc))
            {
                DateTime dt = this.MonthCalendar.CultureCalendar.GetEra(this.Date) != this.MonthCalendar.CultureCalendar.GetEra(this.firstVisibleDate)
                                ? this.firstVisibleDate : this.Date;
                if (this.TitleMonthBounds.Contains(loc))
                    return new MonthCalendarHitTest(dt, MonthCalendarHitType.MonthName, this.TitleMonthBounds, this.TitleBounds);
                if (this.TitleYearBounds.Contains(loc))
                    return new MonthCalendarHitTest(dt, MonthCalendarHitType.MonthYear, this.TitleYearBounds, this.TitleBounds);
                return new MonthCalendarHitTest(this.Date, MonthCalendarHitType.Header, this.TitleBounds);
            }
            if(this.WeekBounds.Contains(loc))
            {
                foreach(MonthCalendarWeek week in this.Weeks)
                {
                    if(week.Visible && week.Bounds.Contains(loc))
                        return new MonthCalendarHitTest(week.Start, MonthCalendarHitType.Week, week.Bounds);
                }
            }
            else if(this.MonthBounds.Contains(loc))
            {
                foreach(MonthCalendarDay day in this.Days)
                {
                    if (day.Bounds.Contains(loc) && day.Visible)
                        return new MonthCalendarHitTest(day.Date, MonthCalendarHitType.Day, day.Bounds);
                }
            }
            else if (this.DayNamesBounds.Contains(loc))
            {
                int dayWidth = this.MonthCalendar.DaySize.Width;
                Rectangle dayNameBounds = this.DayNamesBounds;
                dayNameBounds.Width = dayWidth;
                for(int i = 0; i < 7; i++)
                {
                    if (dayNameBounds.Contains(loc))
                        return new MonthCalendarHitTest(this.Days[i].Date, MonthCalendarHitType.DayName, dayNameBounds);
                    dayNameBounds.X += dayWidth;
                }
            }
            return MonthCalendarHitTest.Empty;
        }
        public bool ContainsDate(DateTime date)
        {
            return date >= this.firstVisibleDate && date <= this.lastVisibleDate;
        }

        private void CalculateProportions(Point loc)
        {
            this.TitleBounds = new Rectangle(loc, this.MonthCalendar.HeaderSize);
            bool useRTL = this.MonthCalendar.UseRTL;
            int adjustX = this.MonthCalendar.WeekNumberSize.Width;
            int dayWidth = this.MonthCalendar.DaySize.Width;
            int dayHeight = this.MonthCalendar.DaySize.Height;
            int weekRectAdjust = 0;
            if(useRTL)
            {
                weekRectAdjust = dayWidth * 7;
                adjustX = 0;
            }
            this.DayNamesBounds = new Rectangle(new Point(loc.X + adjustX, loc.Y + this.TitleBounds.Height),
                this.MonthCalendar.DayNamesSize);
            Rectangle weekNumberRect = new Rectangle(loc.X + weekRectAdjust, loc.Y + this.TitleBounds.Height
                + this.DayNamesBounds.Height, this.MonthCalendar.WeekNumberSize.Width, dayHeight);
            Rectangle weekBounds = weekNumberRect;
            Rectangle monthRect = new Rectangle(loc.X + adjustX, loc.Y + this.TitleBounds.Height + this.DayNamesBounds.Height,
                dayWidth * 7, dayHeight * 6);
            int startX = monthRect.X;
            adjustX = dayWidth;
            if(useRTL)
            {
                startX = monthRect.Right - dayWidth;
                adjustX = -dayWidth;
            }
            int x = startX;
            int y = monthRect.Y;
            int j = 0;
            if(this.Days.Length > 0 && new MonthCalendarDate(this.MonthCalendar.CultureCalendar, this.Days[0].Date).DayOfWeek != this.MonthCalendar.FormatProvider.FirstDayOfWeek)
            {
                DayOfWeek currentDayOfWeek = this.MonthCalendar.FormatProvider.FirstDayOfWeek;
                DayOfWeek dayOfWeek = new MonthCalendarDate(this.MonthCalendar.CultureCalendar, this.Days[0].Date).DayOfWeek;
                while(currentDayOfWeek != dayOfWeek)
                {
                    x += adjustX;
                    if((j + 1) % 7 == 0)
                    {
                        x = startX;
                        y += dayHeight;
                    }
                    int nextDay = (int)currentDayOfWeek + 1;
                    if (nextDay > 6)
                        nextDay = 0;
                    currentDayOfWeek = (DayOfWeek)nextDay;
                    j++;
                }
            }
            for(int i = 0; i < this.Days.Length; i++ , j++)
            {
                this.Days[i].Bounds = new Rectangle(x, y, dayWidth, dayHeight);
                if(i % 7 ==0)
                {
                    bool visible = this.Days[i].Visible || this.Days[Math.Min(i + 6, this.Days.Length - 1)].Visible;
                    this.Weeks[i / 7].Bounds = weekNumberRect;
                    this.Weeks[i / 7].Visible = visible;
                    if (visible)
                        weekBounds = Rectangle.Union(weekBounds, weekNumberRect);
                    weekNumberRect.Y += dayHeight;
                }
                x += adjustX;
                if((j + 1) % 7 == 0)
                {
                    x = startX;
                    y += dayHeight;
                }
            }
            monthRect.Y = weekBounds.Y;
            monthRect.Height = weekBounds.Height;
            this.MonthBounds = monthRect;
            this.WeekBounds = weekBounds;
        }
    }
}
