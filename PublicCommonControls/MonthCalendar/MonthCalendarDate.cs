using System;
using System.Text;
using System.Globalization;

namespace PublicCommonControls.WCalendar
{
    internal class MonthCalendarDate : ICloneable
    {
        private Calendar calendar = CultureInfo.CurrentUICulture.Calendar;
        private DateTime date = DateTime.MinValue;
        private MonthCalendarDate firstOfMonth;

        public MonthCalendarDate(Calendar cal, DateTime date)
        {
            if (cal == null)
                throw new ArgumentNullException("cal", "parameter 'cal' cannot be null");
            if (date < cal.MinSupportedDateTime)
                date = cal.MinSupportedDateTime;
            if (date > cal.MaxSupportedDateTime)
                date = cal.MaxSupportedDateTime;
            this.Year = cal.GetYear(date);
            this.Month = cal.GetMonth(date);
            this.Day = cal.GetDayOfMonth(date);
            this.Era = cal.GetEra(date);
            this.calendar = cal;
        }
        public DateTime Date
        {
            get
            {
                if (this.date == DateTime.MinValue)
                    this.date = this.calendar.ToDateTime(this.Year, this.Month, this.Day, 0, 0, 0, 0, this.Era);
                return this.date;
            }
            
        }
        public int Day { get; protected set; }
        public int Month { get; protected set; }
        public int Year { get; protected set; }
        public int Era { get; protected set; }
        public DayOfWeek DayOfWeek
        {
            get { return this.calendar.GetDayOfWeek(this.Date); }
        }
        public MonthCalendarDate FirstOfMonth
        {
            get
            {
                if(this.firstOfMonth == null)
                {
                    if (this.Date == this.calendar.MinSupportedDateTime.Date)
                        this.firstOfMonth = this.Clone() as MonthCalendarDate;
                    else
                        this.firstOfMonth = new MonthCalendarDate(this.calendar, this.calendar.ToDateTime(this.Year, this.Month, 1, 0, 0, 0, this.Era));
                }
                return this.firstOfMonth;
            }
        }
        public int DaysInMonth
        {
            get { return DateMethods.GetDaysInMonth(this); }
        }
        public Calendar Calendar
        {
            get { return this.calendar; }
            set
            {
                if (value != null)
                    this.calendar = value;
            }
        }
        public MonthCalendarDate GetFirstDayInWeek (ICustomFormatProvider provider)
        {
            DayOfWeek firstDayOfWeek = provider.FirstDayOfWeek;
            MonthCalendarDate dt = (MonthCalendarDate)this.Clone();
            while (dt.DayOfWeek != firstDayOfWeek && dt.Date > this.calendar.MinSupportedDateTime)
                dt = dt.AddDays(-1);
            return dt;
        }
        public MonthCalendarDate GetEndDateOfWeek(ICustomFormatProvider provider)
        {
            DayOfWeek currentDayOfWeek = this.DayOfWeek;
            DayOfWeek firstDayOrWeek = provider.FirstDayOfWeek;
            if (currentDayOfWeek == firstDayOrWeek)
                return this.AddDays(6);
            int d = (int)currentDayOfWeek;
            int daysToAdd = -1;
            while(currentDayOfWeek != firstDayOrWeek)
            {
                daysToAdd++;
                if (++d > 6)
                    d = 0;
                currentDayOfWeek = (DayOfWeek)d;
            }
            return this.AddDays(daysToAdd);
        }
        public MonthCalendarDate AddMonth(int months)
        {
            return new MonthCalendarDate(this.calendar, this.InternalAddMonths(months));
        }
        public MonthCalendarDate AddDays(int days)
        {
            DateTime dt = this.Date;
            int sign = Math.Sign(days);
            days = Math.Abs(days);
            int daysAdded = 0;
            while (((dt > this.calendar.MinSupportedDateTime && sign == -1) || (dt < this.calendar.MaxSupportedDateTime.Date && sign == 1)) && days != daysAdded)
            {
                dt = this.calendar.AddDays(dt, sign);
                daysAdded++;
            }
            return new MonthCalendarDate(this.calendar, dt);
        }
        public override string ToString()
        {
            return this.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentUICulture.DateTimeFormat);
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
        public string ToString(IFormatProvider fp)
        {
            return this.ToString(null, fp);
        }
        public string ToString(string format, IFormatProvider fp, ICustomFormatProvider nameProvider = null, string[] nativeDigits = null)
        {
            if(fp != null)
            {
                DateTimeFormatInfo dtfi = DateTimeFormatInfo.GetInstance(fp);
                if (dtfi != null)
                    return this.ToString(format, dtfi, nameProvider, nativeDigits);
            }
            return this.ToString(format, null, nameProvider, nativeDigits);
        }
        public static DateTime GetFirstOfMonth(Calendar cal, int year, int month)
        {
            if (cal == null)
                return new DateTime(year, month, 1);
            return new DateTime(year, month, 1, cal);
        }
        private DateTime InternalAddMonths(int months)
        {
            try
            {
                DateTime dt = this.Date;
                int monthsToAdd = Math.Abs(months);
                int sign = Math.Sign(months);
                int counter = 0;
                while (counter != monthsToAdd)
                {
                    dt = this.calendar.AddMonths(dt, sign);
                    counter++;
                }
                return dt;
            }
            catch { }
            return months < 0 ? this.calendar.MinSupportedDateTime.Date : this.calendar.MaxSupportedDateTime.Date;
        }
        private string ToString(string format, DateTimeFormatInfo dtfi, ICustomFormatProvider nameProvider, string[] nativeDigits = null)
        {
            if (dtfi == null)
                dtfi = CultureInfo.CurrentUICulture.DateTimeFormat;
            if (string.IsNullOrEmpty(format))
                format = nameProvider != null ? nameProvider.ShortDatePattern : dtfi.ShortDatePattern;
            else if(format.Length == 1)
            {
                switch(format[0])
                {
                    case 'D':
                        format = nameProvider != null ? nameProvider.LongDatePattern : dtfi.LongDatePattern;
                        break;
                    case 'm':
                    case 'M':
                        format = nameProvider != null ? nameProvider.MonthDayPattern : dtfi.MonthDayPattern;
                        break;
                    case 'y':
                    case 'Y':
                        format = nameProvider != null ? nameProvider.YearMonthPattern : dtfi.YearMonthPattern;
                        break;
                    default:
                        format = nameProvider != null ? nameProvider.ShortDatePattern : dtfi.ShortDatePattern;
                        break;
                }
            }
            format = format.Replace(nameProvider != null ? nameProvider.DateSeparator : dtfi.DateSeparator, "/");
            StringBuilder sb = new StringBuilder();
            Calendar c = nameProvider != null ? nameProvider.Calendar : dtfi.Calendar;
            int i = 0;
            while(i < format.Length)
            {
                int tokLen;
                char ch = format[i];
                switch (ch)
                {
                    case 'd':
                        tokLen = CountChar(format, i, ch);
                        sb.Append(tokLen <= 2 ? DateMethods.GetNumberString(this.Day, nativeDigits, tokLen == 2) : GetDayName(c.GetDayOfWeek(this.Date), dtfi, nameProvider, tokLen == 3));
                        break;
                    case 'M':
                        tokLen = CountChar(format, i, ch);
                        sb.Append(tokLen <= 2 ? DateMethods.GetNumberString(this.Month, nativeDigits, tokLen == 2) : GetMonthName(this.Month, this.Year, dtfi, nameProvider, tokLen == 3));
                        break;
                    case 'y':
                        tokLen = CountChar(format, i, ch);
                        sb.Append(tokLen < 2 ? DateMethods.GetNumberString(this.Year % 100, nativeDigits, true) : DateMethods.GetNumberString(this.Year, nativeDigits, false));
                        break;
                    case 'g':
                        tokLen = CountChar(format, i, ch);
                        sb.Append(nameProvider != null ? nameProvider.GetEraName(c.GetEra(this.Date)) : dtfi.GetEraName(c.GetEra(this.Date)));
                        break;
                    case '/':
                        tokLen = CountChar(format, i, ch);
                        sb.Append(nameProvider != null ? nameProvider.DateSeparator : dtfi.DateSeparator);
                        break;
                    case '\'':
                        tokLen = 1;
                        break;
                    default:
                        tokLen = 1;
                        sb.Append(ch.ToString(CultureInfo.CurrentUICulture));
                        break;
                }
                i += tokLen;
            }
            return sb.ToString();
        }
        private static int CountChar(string fmt, int p , char c)
        {
            int len = fmt.Length;
            int index = p + 1;
            while((index < len) && fmt[index] == c)
                index++;
            return index - p;
        }
        private static string GetDayName(DayOfWeek dayofWeek, DateTimeFormatInfo info, ICustomFormatProvider nameProvider, bool abbreviated)
        {
            if (nameProvider != null)
                return abbreviated ? nameProvider.GetAbbreviatedDayName(dayofWeek) : nameProvider.GetDayName(dayofWeek);
            return abbreviated ? info.GetAbbreviatedDayName(dayofWeek) : info.GetDayName(dayofWeek);
        }
        private static string GetMonthName(int month, int year, DateTimeFormatInfo info, ICustomFormatProvider nameProvider, bool abbreviated)
        {
            if (nameProvider != null)
                return abbreviated ? nameProvider.GetAbbreviatedMonthName(year, month) : nameProvider.GetMonthName(year, month);
            return abbreviated ? info.GetAbbreviatedMonthName(month) : info.GetMonthName(month);
        }
    }
}
