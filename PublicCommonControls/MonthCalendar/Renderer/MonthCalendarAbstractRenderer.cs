using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PublicCommonControls.WCalendar
{
    public abstract class MonthCalendarAbstractRenderer
    {
        private readonly MonthCalendar calendar;
        protected MonthCalendarAbstractRenderer(MonthCalendar calendar)
        {
            if (calendar == null)
                throw new ArgumentNullException("calendar", "Parameter 'calendar' cannot be null.");
            this.calendar = calendar;
            this.ColorTable = new MonthCalendarColorTable();
        }
        public MonthCalendarColorTable ColorTable { get; set; }
        public static void FillBackground(Graphics g, GraphicsPath path, Color colorStart, Color colorEnd, LinearGradientMode? mode)
        {
            if (path == null)
                throw new ArgumentNullException("path", "parameter 'path' cannot be null.");
            RectangleF rect = path.GetBounds();
            if (!CheckParams(g, path.GetBounds()) || colorStart == Color.Empty)
                return;
            if (colorEnd == Color.Empty)
            {
                if (colorStart != Color.Transparent)
                {
                    using (SolidBrush brush = new SolidBrush(colorStart))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            else
            {
                rect.Height += 2;
                rect.Y--;
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, colorStart, colorEnd, mode.GetValueOrDefault(LinearGradientMode.Vertical)))
                {
                    g.FillPath(brush, path);
                }
            }
        }
        protected static bool CheckParams(Graphics g, RectangleF rect)
        {
            if (g == null)
                throw new ArgumentNullException("g");
            if (rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect))
                return false;
            return true;
        }
        protected static StringFormat GetStringAlignment(ContentAlignment align)
        {
            StringFormat format = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap);
            switch(align)
            {
                case ContentAlignment.TopLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Far;
                    break;
            }
            return format;
        }
        protected static void FillBackground(Graphics g, Rectangle rect, Color colorStart, Color colorEnd, LinearGradientMode? mode)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddRectangle(rect);
                FillBackground(g, path, colorStart, colorEnd, mode);
            }
        }
        public virtual void DrawControlBackground(Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException("g");
            FillBackground(g, this.calendar.ClientRectangle, this.ColorTable.BackgroundGradientBegin, this.ColorTable.BackgroundGradientEnd, this.ColorTable.BackgroundGradientMode);
        }
        public virtual void DrawTitleBackground(Graphics g, MonthCalendarMonth month, MonthCalendarHeaderState state)
        {
            if (!CheckParams(g, month.TitleBounds))
                return;
            Color backStart, backEnd;
            LinearGradientMode? mode;
            if(state == MonthCalendarHeaderState.Default)
            {
                backStart = this.ColorTable.HeaderGradientBegin;
                backEnd = this.ColorTable.HeaderGradientEnd;
                mode = this.ColorTable.HeaderGradientMode;
            }
            else
            {
                backStart = this.ColorTable.HeaderActiveGradientBegin;
                backEnd = this.ColorTable.HeaderActiveGradientEnd;
                mode = this.ColorTable.HeaderActiveGradientMode;
            }
            this.FillBackgroundInternal(g, month.TitleBounds, backStart, backEnd, mode);
        }
        public virtual void DrawMonthBodyBackground(Graphics g, MonthCalendarMonth month)
        {
            if (!CheckParams(g, month.MonthBounds))
                return;
            FillBackground(g, month.MonthBounds, this.ColorTable.MonthBodyGradientBegin,
                this.ColorTable.MonthBodyGradientEnd, this.ColorTable.MonthBodyGradientMode);
        }
        public virtual void DrawDayHeaderBackground(Graphics g, MonthCalendarMonth month)
        {
            if (!CheckParams(g, month.DayNamesBounds))
                return;
            FillBackground(g, month.DayNamesBounds, this.ColorTable.DayHeaderGradientBegin,
                this.ColorTable.DayHeaderGradientEnd, this.ColorTable.DayHeaderGradientMode);
        }
        public virtual void DrawWeekHeaderBackground(Graphics g, MonthCalendarMonth month)
        {
            if (!CheckParams(g, month.WeekBounds))
                return;
            FillBackground(g, month.WeekBounds, this.ColorTable.WeekHeaderGradientBegin,
                this.ColorTable.WeekHeaderGradientEnd, this.ColorTable.WeekHeaderGradientMode);
        }
        public virtual void DrawFooterBackground(Graphics g, Rectangle footerBounds, bool active)
        {
            if (!CheckParams(g, footerBounds))
                return;
            MonthCalendarColorTable colors = this.ColorTable;
            if(active)
            {
                FillBackground(g, footerBounds, colors.FooterActiveGradientBegin,
                    colors.FooterActiveGradientEnd, colors.FooterActiveGradientMode);
            }
            else
            {
                FillBackground(g, footerBounds, colors.FooterGradientBegin,
                    colors.FooterGradientEnd, colors.FooterGradientMode);
            }
        }
        public abstract void DrawMonthHeader(Graphics g, MonthCalendarMonth calMonth, MonthCalendarHeaderState state);
        public abstract void DrawDay(Graphics g, MonthCalendarDay day);
        public abstract void DrawDayHeader(Graphics g, Rectangle rect);
        public abstract void DrawFooter(Graphics g, Rectangle rect, bool active);
        public abstract void DrawWeekHeaderItem(Graphics g, MonthCalendarWeek week);
        public abstract void DrawWeekHeaderSeparator(Graphics g, Rectangle rect);

        internal static Color GetGrayColor(bool enabled, Color baseColor)
        {
            if (baseColor.IsEmpty || enabled)
                return baseColor;
            float lumi = (.3F * baseColor.R) + (.59F * baseColor.G) + (.11F * baseColor.B);
            return Color.FromArgb((int)lumi, (int)lumi, (int)lumi);
        }
        internal void FillBackgroundInternal(Graphics g, Rectangle rect, Color start, Color end, LinearGradientMode? mode)
        {
            if(!this.calendar.Enabled)
            {
                float lumiStart = (.3F * start.R) * (.59F * start.G) * (.11F * start.B);
                float lumiEnd = (.3F * end.R) * (.59F * end.G) * (.11F * end.B);
                if (!start.IsEmpty)
                    start = Color.FromArgb((int)lumiStart, (int)lumiStart, (int)lumiStart);
                if (!end.IsEmpty)
                    end = Color.FromArgb((int)lumiEnd, (int)lumiEnd, (int)lumiEnd);
            }
            FillBackground(g, rect, start, end, mode);
        }
    }

}
