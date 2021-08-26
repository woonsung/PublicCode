using System;
using System.Linq;
using System.ComponentModel;
using System.Globalization;


namespace PublicCommonControls.WCalendar
{
    public class MonthCalendarFormatProvider : ICustomFormatProvider
    {
        private DateTimeFormatInfo dtfi;
        private NumberFormatInfo nfi;
        private Calendar calendar;
        private string[] monthNames;
        private string[] abbrMonthNames;
        private string[] dayNames;
        private string[] abbrDayNames;
        private string[] shortestDayNames;


        public MonthCalendarFormatProvider(CultureInfo ci, Calendar cal, bool rtlCulture)
        {
            if (ci == null)
                throw new ArgumentNullException("ci", "parameter 'ci' cannot be null.");
            this.DateTimeFormat = ci.DateTimeFormat;
            this.nfi = ci.NumberFormat;
            this.Calendar = cal;
            this.IsRTLLanguage = rtlCulture;
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTimeFormatInfo DateTimeFormat
        {
            get
            { return this.dtfi; }
            set
            {
                if (value == null)
                    return;
                this.dtfi = value;
                this.calendar = value.Calendar;
                this.dayNames = dtfi.DayNames;
                this.abbrDayNames = dtfi.AbbreviatedDayNames;
                this.shortestDayNames = dtfi.ShortestDayNames;
                this.FirstDayOfWeek = dtfi.FirstDayOfWeek;
                this.monthNames = dtfi.MonthNames;
                this.abbrMonthNames = dtfi.AbbreviatedMonthNames;
                this.DateSeparator = dtfi.DateSeparator;
                this.ShortDatePattern = dtfi.ShortDatePattern;
                this.LongDatePattern = dtfi.LongDatePattern;
                this.MonthDayPattern = dtfi.MonthDayPattern;
                this.YearMonthPattern = dtfi.YearMonthPattern;
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Calendar Calendar
        {
            get { return this.calendar; }
            set
            {
                this.calendar = value ?? this.dtfi.Calendar;
            }
        }
        [Description("The full month names.")]
        public virtual string[] MonthNames
        {
            get
            {
                return this.monthNames;
            }
            set
            {
                if (value == null || value.Length < 12)
                    return;
                if (value.Where((t, i) => string.IsNullOrEmpty(t) && i != 12).Any())
                    return;
                this.monthNames = value;
            }
        }
        [Description("The abbreviated month names.")]
        public virtual string[] AbbreviatedMonthNames
        {
            get
            {
                return this.abbrMonthNames;
            }
            set
            {
                if (value == null || value.Length < 12)
                    return;
                if (value.Where((t, i) => string.IsNullOrEmpty(t) && i != 12).Any())
                    return;
                this.abbrMonthNames = value;
            }
        }
        [Description("The date separator.")]
        public virtual string DateSeparator { get; set; }
        [Description("The short date pattern.")]
        public virtual string ShortDatePattern { get; set; }
        [Description("The long date pattern.")]
        public virtual string LongDatePattern { get; set; }
        [Description("The month day pattern.")]
        public virtual string MonthDayPattern { get; set; }
        [Description("the year month pattern.")]
        public virtual string YearMonthPattern { get; set; }
        [Description("The full day names.")]
        public virtual string[] DayNames
        {
            get
            {
                return this.dayNames;
            }
            set
            {
                if (value == null || value.Length != 7)
                    return;
                if (value.Any(string.IsNullOrEmpty))
                    return;
                this.dayNames = value;
            }
        }
        [Description("The abbreviated day names.")]
        public virtual string[] AbbreviatedDayNames
        {
            get
            {
                return this.abbrDayNames;
            }
            set
            {
                if (value == null || value.Length != 7)
                    return;
                if (value.Any(string.IsNullOrEmpty))
                    return;
                this.abbrDayNames = value;
            }
        }
        [Description("The shortest day names.")]
        public virtual string[] ShortestDayNames
        {
            get
            {
                return this.shortestDayNames;
            }
            set
            {
                if (value == null || value.Length != 7)
                    return;
                if (value.Any(string.IsNullOrEmpty))
                    return;
                this.shortestDayNames = value;
            }
        }
        [Description("The first day of the week.")]
        public virtual DayOfWeek FirstDayOfWeek { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsRTLLanguage { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MonthCalendar MonthCalendar { get; set; }
        public virtual string GetMonthName(int year, int month)
        {
            if (!this.CheckMonthAndYear(year, month))
                return string.Empty;
            if (this.calendar.GetType() == typeof(HebrewCalendar))
                return this.calendar.GetMonthsInYear(year) == 13 ? this.monthNames[month - 1] : this.monthNames[month == 12 ? 11 : month - 1];
            return this.monthNames[month - 1];
        }
        public virtual string GetAbbreviatedMonthName(int year, int month)
        {
            if (!this.CheckMonthAndYear(year, month))
                return string.Empty;
            if (this.calendar.GetType() == typeof(HebrewCalendar))
                return this.calendar.GetMonthsInYear(year) == 13 ? this.abbrMonthNames[month - 1] : this.abbrMonthNames[month == 12 ? 11 : month - 1];
            return this.abbrMonthNames[month - 1];
        }
        public virtual int GetMonthsInYear(int year)
        {
            return this.calendar.GetMonthsInYear(year);
        }

        public virtual string GetEraName(int era)
        {
            return this.dtfi.GetEraName(era);
        }
        public virtual string GetDayName(DayOfWeek dayofweek)
        {
            return this.dayNames[(int)dayofweek];
        }
        public virtual string GetAbbreviatedDayName(DayOfWeek dayofweek)
        {
            return this.abbrDayNames[(int)dayofweek];
        }
        public virtual string GetShortestDayName(DayOfWeek dayofweek)
        {
            return this.shortestDayNames[(int)dayofweek];
        }
        protected virtual bool CheckMonthAndYear(int year, int month)
        {
            MonthCalendarEraRange[] ranges = this.MonthCalendar.EraRanges;
            bool validYear = false;
            foreach (var range in ranges)
            {
                int minYear = this.calendar.GetYear(range.MinDate);
                int maxYear = this.calendar.GetYear(range.MaxDate);
                if (year >= minYear && year <= maxYear)
                    validYear = true;
            }
            return validYear && month > 0 && month < 14;
        }
        private bool ShouldSerializeMonthNames()
        {
            return ShouldSerialize(this.dtfi.MonthNames, this.monthNames);
        }
        private void ResetMonthNames()
        {
            this.monthNames = this.dtfi.MonthNames;
        }
        private bool ShouldSerializeAbbreviatedMonthNames()
        {
            return ShouldSerialize(this.dtfi.AbbreviatedMonthNames, this.abbrMonthNames);
        }
        private void ResetAbbreviatedMonthNames()
        {
            this.abbrMonthNames = this.dtfi.AbbreviatedMonthNames;
        }
        private bool ShouldSerializeDateSeparator()
        {
            return this.DateSeparator != this.dtfi.DateSeparator;
        }
        private void ResetDateSeparator()
        {
            this.DateSeparator = this.dtfi.DateSeparator;
        }
        private bool ShouldSerializeShortDatePattern()
        {
            return this.ShortDatePattern != this.dtfi.ShortDatePattern;
        }
        private void ResetShortDatePattern()
        {
            this.ShortDatePattern = this.dtfi.ShortDatePattern;
        }
        private bool ShouldSerializeLongDatePattern()
        {
            return this.LongDatePattern != this.dtfi.LongDatePattern;
        }
        private void ResetLongDatePattern()
        {
            this.LongDatePattern = this.dtfi.LongDatePattern;
        }
        private bool ShouldSerializeMonthDayPattern()
        {
            return this.MonthDayPattern != this.dtfi.MonthDayPattern;
        }
        private void ResetMonthDayPattern()
        {
            this.MonthDayPattern = this.dtfi.MonthDayPattern;
        }
        private bool ShouldSerializeYearMonthPattern()
        {
            return this.YearMonthPattern != this.dtfi.YearMonthPattern;
        }
        private void ResetYearMonthPattern()
        {
            this.YearMonthPattern = this.dtfi.YearMonthPattern;
        }
        private bool ShouldSerializeDayNames()
        {
            return ShouldSerialize(this.dtfi.DayNames, this.dayNames);
        }
        private void ResetDayNames()
        {
            this.dayNames = this.dtfi.DayNames;
        }
        private bool ShouldSerializeAbbreviatedDayNames()
        {
            return ShouldSerialize(this.dtfi.AbbreviatedDayNames, this.abbrDayNames);
        }
        private void ResetAbbreviatedDayNames()
        {
            this.abbrDayNames = this.dtfi.AbbreviatedDayNames;
        }
        private bool ShouldSerializeShortestDayNames()
        {
            return ShouldSerialize(this.dtfi.ShortestDayNames, this.shortestDayNames);
        }
        private void ResetShortestDayNames()
        {
            this.shortestDayNames = this.dtfi.ShortestDayNames;
        }
        private bool ShouldSerializeFirstDayOfWeek()
        {
            return this.dtfi.FirstDayOfWeek != this.FirstDayOfWeek;
        }
        private void ResetFirstDayOfWeek()
        {
            this.FirstDayOfWeek = this.dtfi.FirstDayOfWeek;
        }
        private static bool ShouldSerialize(string[] source, string[] custom)
        {
            return source == null || custom == null || source.Length != custom.Length || source.Where((t, i) => t != custom[i]).Any();
        }

    }
}
