using System;
using System.Drawing;

namespace PublicCommonControls.WCalendar
{
    public class MonthCalendarHitTest
    {
        public static readonly MonthCalendarHitTest Empty = new MonthCalendarHitTest();
        private Rectangle invalidateBounds = Rectangle.Empty;
        public MonthCalendarHitTest()
            : this(DateTime.MinValue, MonthCalendarHitType.None, Rectangle.Empty, Rectangle.Empty)
        {

        }
        public MonthCalendarHitTest(DateTime date, MonthCalendarHitType type, Rectangle bounds)
            : this(date, type, bounds, Rectangle.Empty)
        {

        }
        public MonthCalendarHitTest(DateTime date, MonthCalendarHitType type, Rectangle bounds, Rectangle invalidateBounds)
        {
            this.Date = date;
            this.Type = type;
            this.Bounds = bounds;
            this.invalidateBounds = invalidateBounds;
        }
        public DateTime Date { get; set; }
        public MonthCalendarHitType Type { get; private set;}
        public Rectangle Bounds { get; private set; }
        public Rectangle InvalidateBounds
        {
            get
            {
                if (this.invalidateBounds.IsEmpty)
                    return this.Bounds;
                return this.invalidateBounds;
            }
            internal set
            {
                this.invalidateBounds = value;
            }
        }
        public bool IsEmpty
        {
            get { return this.Type == MonthCalendarHitType.None || this.Date == DateTime.MinValue || this.Bounds.IsEmpty; }
        }


    }
}
