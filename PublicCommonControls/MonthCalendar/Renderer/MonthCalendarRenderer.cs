using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace PublicCommonControls.WCalendar
{
    public class MonthCalendarRenderer : MonthCalendarAbstractRenderer
    {
        private readonly MonthCalendar monthCal;
        public MonthCalendarRenderer(MonthCalendar calendar)
            : base(calendar)
        {
            this.monthCal = calendar;
        }
        public override void DrawMonthHeader(Graphics g, MonthCalendarMonth calMonth, MonthCalendarHeaderState state)
        {
            if (calMonth == null || !CheckParams(g, calMonth.TitleBounds))
                return;
            Rectangle rect = calMonth.TitleBounds;
            MonthCalendarDate date = new MonthCalendarDate(monthCal.CultureCalendar, calMonth.Date);
            MonthCalendarDate firstVisible = new MonthCalendarDate(monthCal.CultureCalendar, calMonth.FirstVisibleDate);
            string month;
            int year;
            if(firstVisible.Era != date.Era)
            {
                month = this.monthCal.FormatProvider.GetMonthName(firstVisible.Year, firstVisible.Month);
                year = firstVisible.Year;
            }
            else
            {
                month = this.monthCal.FormatProvider.GetMonthName(date.Year, date.Month);
                year = date.Year;
            }
            string yearString = this.monthCal.UseNativeDigits ? DateMethods.GetNativeNumberString(year, this.monthCal.Culture.NumberFormat.NativeDigits, false)
                : year.ToString(CultureInfo.CurrentUICulture);
            Font headerFont = this.monthCal.HeaderFont;
            Font boldFont = new Font(headerFont.FontFamily, headerFont.SizeInPoints, FontStyle.Bold);
            SizeF monthSize = g.MeasureString(month, boldFont);
            SizeF yearSize = g.MeasureString(yearString, boldFont);
            float maxHeight = Math.Max(monthSize.Height, yearSize.Height);
            int width = (int)monthSize.Width + (int)yearSize.Width + 7;
            int arrowLeftX = rect.X + 6;
            int arrowRightX = rect.Right - 6;
            int arrowY = rect.Y + (rect.Height / 2) - 4;
            int x = Math.Max(0, rect.X + (rect.Width / 2) + 1 - (width / 2));
            int y = Math.Max(0, rect.Y + (rect.Height / 2) + 1 - (((int)maxHeight + 1) / 2));
            calMonth.TitleMonthBounds = new Rectangle(x, y, (int)monthSize.Width + 1, (int)maxHeight + 1);
            calMonth.TitleYearBounds = new Rectangle(x + calMonth.TitleMonthBounds.Width + 7, y, (int)yearSize.Width + 1, (int)maxHeight + 1);
            Point[] arrowLeft = new[] { new Point(arrowLeftX, arrowY + 4), new Point(arrowLeftX + 4, arrowY),
                                        new Point(arrowLeftX + 4, arrowY + 8), new Point(arrowLeftX, arrowY + 4) };
            Point[] arrowRight = new[] {new Point(arrowRightX, arrowY + 4), new Point(arrowRightX - 4, arrowY),
                                        new Point(arrowRightX - 4, arrowY + 8), new Point(arrowRightX, arrowY +4)};
            MonthCalendarColorTable colorTable = this.ColorTable;
            using (SolidBrush brush = new SolidBrush(colorTable.HeaderText), brushOver = new SolidBrush(colorTable.HeaderActiveText)
                , brushSelected = new SolidBrush(colorTable.HeaderSelectedText))
            {
                Rectangle monthRect = calMonth.TitleMonthBounds;
                Rectangle yearRect = calMonth.TitleYearBounds;
                Font monthFont = headerFont;
                Font yearFont = headerFont;
                SolidBrush monthBrush = brush, yearBrush = brush;
                if (state == MonthCalendarHeaderState.YearSelected)
                {
                    yearBrush = brushSelected;
                    yearFont = boldFont;
                    yearRect.Width += 4;
                }
                else if (state == MonthCalendarHeaderState.YearActive)
                    yearBrush = brushOver;
                if (state == MonthCalendarHeaderState.MonthNameSelected)
                {
                    monthBrush = brushSelected;
                    monthFont = boldFont;
                    monthRect.Width += 4;
                }
                else if (state == MonthCalendarHeaderState.MonthNameActive)
                    monthBrush = brushOver;
                g.DrawString(month, monthFont, monthBrush, monthRect);
                g.DrawString(yearString, yearFont, yearBrush, yearRect);
            }
            boldFont.Dispose();
            if(calMonth.DrawLeftButton)
            {
                Color arrowColor = this.monthCal.LeftButtonState == ButtonState.Normal ? GetGrayColor(this.monthCal.Enabled, colorTable.HeaderArrow) : colorTable.HeaderActiveArrow;
                this.monthCal.SetLeftArrowRect(new Rectangle(rect.X, rect.Y, 15, rect.Height));
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLines(arrowLeft);
                    using (SolidBrush brush = new SolidBrush(arrowColor))
                        g.FillPath(brush, path);
                    using (Pen p = new Pen(arrowColor))
                        g.DrawPath(p, path);
                }
            }
            if(calMonth.DrawRightButton)
            {
                Color arrowColor = this.monthCal.RightButtonState == ButtonState.Normal ? GetGrayColor(this.monthCal.Enabled, colorTable.HeaderArrow) : colorTable.HeaderActiveArrow;
                this.monthCal.SetRightArrowRect(new Rectangle(rect.Right - 15, rect.Y, 15, rect.Height));
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLines(arrowRight);
                    using (SolidBrush brush = new SolidBrush(arrowColor))
                        g.FillPath(brush, path);
                    using (Pen p = new Pen(arrowColor))
                        g.DrawPath(p, path);
                }
            }
        }
        public override void DrawDay(Graphics g, MonthCalendarDay day)
        {
            if (!CheckParams(g, day.Bounds))
                return;
            MonthCalendarColorTable colors = this.ColorTable;
            Rectangle rect = day.Bounds;
            var boldDate = this.monthCal.BoldedDatesCollection.Find(d => d.Value.Date == day.Date.Date);
            if (day.MouseOver)
                FillBackground(g, rect, colors.DayActiveGradientBegin, colors.DayActiveGradientEnd, colors.DaySelectedGradientMode);
            else if (day.Selected)
                this.FillBackgroundInternal(g, rect, colors.DaySelectedGradientBegin, colors.DaySelectedGradientEnd, colors.DaySelectedGradientMode);
            else if (!boldDate.IsEmpty && boldDate.Category.BackColorStart != Color.Empty && boldDate.Category.BackColorStart != Color.Transparent)
            {
                FillBackground(
                    g,
                    rect,
                    boldDate.Category.BackColorStart, boldDate.Category.BackColorEnd.IsEmpty || boldDate.Category.BackColorEnd == Color.Transparent ? boldDate.Category.BackColorStart : boldDate.Category.BackColorEnd,
                    boldDate.Category.GradientMode
                    );
            }
            List<DateTime> boldedDates = this.monthCal.GetBoldedDates();
            bool bold = boldedDates.Contains(day.Date) || !boldDate.IsEmpty;

            using (StringFormat format = GetStringAlignment(this.monthCal.DayTextAlignment))
            {
                Color textColor = bold ? (boldDate.IsEmpty || boldDate.Category.ForeColor == Color.Empty || boldDate.Category.ForeColor == Color.Transparent ? colors.DayTextBold : boldDate.Category.ForeColor)
                    : (day.Selected ? colors.DaySelectedText : (day.MouseOver ? colors.DayActiveText : (day.TrailingDate ? colors.DayTrailingText : colors.DayText)));
                using (SolidBrush brush = new SolidBrush(textColor))
                {
                    using (Font font = new Font(this.monthCal.Font.FontFamily, this.monthCal.Font.SizeInPoints, FontStyle.Bold))
                    {
                        Rectangle textRect = day.Bounds;
                        textRect.Width -= 2;
                        bool useBoldFont = day.Date == DateTime.Today || bold;
                        var calDate = new MonthCalendarDate(monthCal.CultureCalendar, day.Date);
                        string dayString = this.monthCal.UseNativeDigits ? DateMethods.GetNativeNumberString(calDate.Day, this.monthCal.Culture.NumberFormat.NativeDigits, false)
                            : calDate.Day.ToString(this.monthCal.Culture);
                        if (this.monthCal.Enabled)
                            g.DrawString(dayString, (useBoldFont ? font : this.monthCal.Font), brush, textRect, format);
                        else
                            ControlPaint.DrawStringDisabled(g, dayString, (useBoldFont ? font : this.monthCal.Font), Color.Transparent, textRect, format);
                    }
                }
            }
            if(day.Date == DateTime.Today)
            {
                rect.Height -= 1;
                rect.Width -= 2;
                Color borderColor = day.Selected ? colors.DaySelectedTodayCircleBorder : (day.MouseOver ? colors.DayActiveTodayCircleBorder : colors.DayTodayCircleBorder);
                using (Pen p = new Pen(borderColor))
                {
                    g.DrawRectangle(p, rect);
                    rect.Offset(1, 0);
                    g.DrawRectangle(p, rect);
                }
            }
        }
        public override void DrawDayHeader(Graphics g, Rectangle rect)
        {
            int dayWidth = this.monthCal.DaySize.Width;
            if (!CheckParams(g, rect) || dayWidth <= 0)
                return;
            List<string> names = new List<string>(DateMethods.GetDayNames(this.monthCal.FormatProvider, this.monthCal.UseShortestDayNames ? 2 : 1));
            if (this.monthCal.UseRTL)
                names.Reverse();
            Rectangle dayRect = rect;
            dayRect.Width = dayWidth;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                using (SolidBrush brush = new SolidBrush(this.ColorTable.DayHeaderText))
                {
                    names.ForEach(day =>
                    {
                        if (this.monthCal.Enabled)
                            g.DrawString(day, this.monthCal.DayHeaderFont, brush, dayRect, format);
                        else
                            ControlPaint.DrawStringDisabled(g, day, this.monthCal.DayHeaderFont, Color.Transparent, dayRect, format);
                        dayRect.X += dayWidth;
                    });
                }
            }
            using (Pen p = new Pen(GetGrayColor(this.monthCal.Enabled, this.ColorTable.MonthSeparator)))
            {
                g.DrawLine(p, rect.X, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
            }
        }
        public override void DrawFooter(Graphics g, Rectangle rect, bool active)
        {
            if (!CheckParams(g, rect))
                return;
            string dateString = new MonthCalendarDate(this.monthCal.CultureCalendar, DateTime.Today).ToString(
                null, null, this.monthCal.FormatProvider, this.monthCal.UseNativeDigits ? this.monthCal.Culture.NumberFormat.NativeDigits : null);
            SizeF dateSize = g.MeasureString(dateString, this.monthCal.FooterFont);
            Rectangle todayRect = rect;
            todayRect.X += 2;
            if (this.monthCal.UseRTL)
                todayRect.X = rect.Right - 20;
            todayRect.Y = rect.Y + (rect.Height / 2) - 5;
            todayRect.Height = 11;
            todayRect.Width = 18;
            using (Pen p = new Pen(this.ColorTable.FooterTodayCircleBorder))
            {
                g.DrawRectangle(p, todayRect);
            }
            int y = rect.Y + (rect.Height / 2) - ((int)dateSize.Height / 2);
            Rectangle dateRect;
            if (this.monthCal.UseRTL)
                dateRect = new Rectangle(rect.X + 1, y, todayRect.Left - rect.X, (int)dateSize.Height + 1);
            else
                dateRect = new Rectangle(todayRect.Right + 2, y, rect.Width - todayRect.Width, (int)dateSize.Height + 1);
            using (StringFormat format = GetStringAlignment(this.monthCal.UseRTL ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft))
            {
                using (SolidBrush brush = new SolidBrush(active ? this.ColorTable.FooterActiveText : GetGrayColor(this.monthCal.Enabled, this.ColorTable.FooterText)))
                {
                    g.DrawString("Today", this.monthCal.FooterFont, brush, dateRect, format);
                }
            }
        }
        public override void DrawWeekHeaderItem(Graphics g, MonthCalendarWeek week)
        {
            if (!CheckParams(g, week.Bounds))
                return;
            var weekString = this.monthCal.UseNativeDigits ? DateMethods.GetNativeNumberString(week.WeekNumber, this.monthCal.Culture.NumberFormat.NativeDigits, false)
                : week.WeekNumber.ToString(CultureInfo.CurrentUICulture);
            using (StringFormat format = GetStringAlignment(this.monthCal.DayTextAlignment))
            {
                format.Alignment = StringAlignment.Center;
                using (SolidBrush brush = new SolidBrush(this.ColorTable.WeekHeaderText))
                {
                    if (this.monthCal.Enabled)
                        g.DrawString(weekString, this.monthCal.Font, brush, week.Bounds, format);
                    else
                        ControlPaint.DrawStringDisabled(g, weekString, this.monthCal.Font, Color.Transparent, week.Bounds, format);
                }
            }
        }
        public override void DrawWeekHeaderSeparator(Graphics g, Rectangle rect)
        {
            if (!CheckParams(g, rect) || !this.monthCal.ShowWeekHeader)
                return;
            using (Pen p = new Pen(GetGrayColor(this.monthCal.Enabled, this.ColorTable.MonthSeparator)))
            {
                if (this.monthCal.UseRTL)
                    g.DrawLine(p, rect.X, rect.Y - 1, rect.X, rect.Bottom - 1);
                else
                    g.DrawLine(p, rect.Right - 1, rect.Y - 1, rect.Right - 1, rect.Bottom - 1);
            }
        }
    }
}