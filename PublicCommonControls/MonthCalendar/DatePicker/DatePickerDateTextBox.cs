using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PublicCommonControls.WCalendar
{
    internal class DatePickerDateTextBox :  Control
    {
        private readonly InputDateTextBox inputBox;
        private readonly DatePicker datePicker;
        private bool inEditMode;
        private bool isValidDate = true;
        private SelectedDatePart selectedPart = SelectedDatePart.None;
        private RectangleF dayBounds;
        private RectangleF monthBounds;
        private RectangleF yearBounds;
        private DateTime currentDate;
        private Color invalidDateBackColor;
        private Color invalidDateForeColor;
        private int dayPartIndex;
        private int monthPartIndex;
        private int yearPartIndex;
        public DatePickerDateTextBox(DatePicker picker)
        {
            if (picker == null)
                throw new ArgumentNullException("picker", "parameter 'picker' cannot be null.");
            this.datePicker = picker;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Selectable | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.currentDate = DateTime.Today;
            this.invalidDateBackColor = Color.Red;
            this.invalidDateForeColor = this.ForeColor;
            this.inputBox = new InputDateTextBox(this)
            {
                Visible = false,
                Multiline = true,
                ShortcutsEnabled = false
            };
            this.inputBox.FinishedEditing += InputBoxFinishedEditing;
            this.Controls.Add(this.inputBox);
        }
        private DatePickerDateTextBox()
        {

        }
        public event EventHandler<CheckDateEventArgs> CheckDate;
        private enum SelectedDatePart
        {
            Day,
            Month,
            Year,
            None
        }
        public DateTime Date
        {
            get
            {
                return this.currentDate;
            }
            set
            {
                if (value == this.currentDate)
                    return;
                if (value < this.MinDate)
                    value = this.MinDate;
                else if (value > this.MaxDate)
                    value = this.MaxDate;
                this.SetNewDate(value);
                this.Invalidate();
            }
        }
        public Color InvalidBackColor
        {
            get
            {
                return this.invalidDateBackColor;
            }
            set
            {
                if (value.IsEmpty || value == this.invalidDateBackColor)
                    return;
                this.invalidDateBackColor = value;
                this.Invalidate();
            }
        }
        public Color InvalidForeColor
        {
            get
            {
                return this.invalidDateForeColor;
            }
            set
            {
                if (value.IsEmpty || value == this.invalidDateForeColor)
                    return;
                this.invalidDateForeColor = value;
                this.Invalidate();
            }
        }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public CultureInfo Culture
        {
            get { return this.datePicker.Culture; }
        }
        public override bool Focused
        {
            get { return base.Focused || this.inputBox.Focused; }
        }
        public sealed override Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }
        public bool InEditMode
        {
            get { return this.inEditMode; }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            int dayDist = (int)Math.Min(Math.Abs(this.dayBounds.Left - e.Location.X), Math.Abs(this.dayBounds.Right - e.Location.X));
            int monthDist = (int)Math.Min(Math.Abs(this.monthBounds.Left - e.Location.X), Math.Abs(this.monthBounds.Right - e.Location.X));
            int yearDist = (int)Math.Min(Math.Abs(this.yearBounds.Left - e.Location.X), Math.Abs(this.yearBounds.Right - e.Location.X));
            int min = Math.Min(dayDist, Math.Min(monthDist, yearDist));
            if (this.dayBounds.Contains(e.Location) || min == dayDist)
                this.selectedPart = SelectedDatePart.Day;
            else if (this.monthBounds.Contains(e.Location) || min == monthDist)
                this.selectedPart = SelectedDatePart.Month;
            else if (this.yearBounds.Contains(e.Location) || min == yearDist)
                this.selectedPart = SelectedDatePart.Year;
            this.Refresh();
            base.OnMouseDown(e);
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Left || keyData == Keys.Right)
            {
                this.SetDatePart(keyData == Keys.Left ? this.RightToLeft == RightToLeft.No : this.RightToLeft == RightToLeft.Yes);
                return true;
            }
            Calendar cal = this.datePicker.Picker.CultureCalendar;
            MonthCalendarDate dt = new MonthCalendarDate(cal, this.currentDate);
            DateTime date = this.Date;
            if (keyData == Keys.Up || keyData == Keys.Down)
            {
                bool up = keyData == Keys.Up;
                switch (this.selectedPart)
                {
                    case SelectedDatePart.Day:
                        {
                            int day = dt.Day + (up ? 1 : -1);
                            int daysInMonth = DateMethods.GetDaysInMonth(dt);
                            if (day > daysInMonth)
                                day = 1;
                            else if (day < 1)
                                day = daysInMonth;
                            date = new DateTime(dt.Year, dt.Month, day, cal);
                            break;
                        }
                    case SelectedDatePart.Month:
                        {
                            int day = dt.Day;
                            int month = dt.Month + (up ? 1 : -1);
                            int monthsInYear = cal.GetMonthsInYear(dt.Year);
                            if (month > monthsInYear)
                                month = 1;
                            else if (month < 1)
                                month = monthsInYear;
                            DateTime newDate = new DateTime(dt.Year, month, 1, cal);
                            dt = new MonthCalendarDate(cal, newDate);
                            int daysInMonth = DateMethods.GetDaysInMonth(dt);
                            newDate = daysInMonth < day ? cal.AddDays(newDate, daysInMonth - 1) : cal.AddDays(newDate, day - 1);
                            date = newDate;
                            break;
                        }
                    case SelectedDatePart.Year:
                        {
                            int year = dt.Year + (up ? 1 : -1);
                            int minYear = cal.GetYear(this.MinDate);
                            int maxYear = cal.GetYear(this.MaxDate);
                            year = Math.Max(minYear, Math.Min(year, maxYear));
                            int yearDiff = year - dt.Year;
                            date = cal.AddYears(this.currentDate, yearDiff);
                            break;
                        }
                }
                this.Date = date < this.MinDate ? this.MinDate : (date > this.MaxDate ? this.MaxDate : date);
                this.Refresh();
                return true;
            }
            if (keyData == Keys.Home || keyData == Keys.End)
            {
                bool first = keyData == Keys.Home;
                switch (this.selectedPart)
                {
                    case SelectedDatePart.Day:
                        {
                            date = first ? new DateTime(dt.Year, dt.Month, 1, cal) : new DateTime(dt.Year, dt.Month, DateMethods.GetDaysInMonth(dt), cal);
                            break;
                        }
                    case SelectedDatePart.Month:
                        {
                            int day = dt.Day;
                            date = first ? new DateTime(dt.Year, 1, 1, cal) : new DateTime(dt.Year, cal.GetMonthsInYear(dt.Year), 1, cal);
                            int daysInMonth = DateMethods.GetDaysInMonth(dt);
                            date = day > daysInMonth ? cal.AddDays(date, daysInMonth - 1) : cal.AddDays(date, day - 1);
                            break;
                        }
                    case SelectedDatePart.Year:
                        {
                            date = first ? this.MinDate.Date : this.MaxDate.Date;
                            break;
                        }
                }
                this.Date = date < this.MinDate ? this.MinDate : (date > this.MaxDate ? this.MaxDate : date);
                this.Refresh();
                return true;
            }
            if(keyData == Keys.Space && !this.inEditMode)
            {
                this.datePicker.SwitchPickerState();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            if ((e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9))
            {
                e.SuppressKeyPress = true;
            }
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = true;
            if (!char.IsDigit(e.KeyChar))
                return;
            if(!this.inEditMode)
            {
                this.inEditMode = true;
                this.Refresh();
                string keyCharString = e.KeyChar.ToString(CultureInfo.InvariantCulture);
                if(this.datePicker.UseNativeDigits)
                {
                    int number = int.Parse(keyCharString);
                    keyCharString = DateMethods.GetNativeNumberString(number, this.datePicker.Culture.NumberFormat.NativeDigits, false);
                }
                this.inputBox.Font = this.Font;
                this.inputBox.Location = new Point(0, 2);
                this.inputBox.Size = this.Size;
                this.inputBox.Text = keyCharString;
                this.inputBox.Visible = true;
                this.inputBox.SelectionStart = 1;
                this.inputBox.SelectionLength = 0;
                this.inputBox.BringToFront();
                this.inputBox.Focus();
            }
            this.Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if(this.inEditMode)
            {
                e.Graphics.Clear(this.BackColor);
                base.OnPaint(e);
                return;
            }
            e.Graphics.Clear(this.Enabled ? (this.isValidDate ? this.BackColor : this.invalidDateBackColor) : SystemColors.Window);
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoClip | StringFormatFlags.NoWrap))
            {
                format.LineAlignment = StringAlignment.Center;
                if (this.RightToLeft == RightToLeft.Yes)
                    format.Alignment = StringAlignment.Far;
                using (SolidBrush foreBrush = new SolidBrush(this.Enabled ? (this.isValidDate ? this.ForeColor : this.invalidDateForeColor) : SystemColors.GrayText),
                    selectedBrush = new SolidBrush(SystemColors.HighlightText), selectedBack = new SolidBrush(SystemColors.Highlight))
                {
                    MonthCalendar cal = this.datePicker.Picker;
                    ICustomFormatProvider provider = cal.FormatProvider;
                    MonthCalendarDate date = new MonthCalendarDate(cal.CultureCalendar, this.currentDate);
                    DatePatternParser parser = new DatePatternParser(provider.ShortDatePattern, provider);
                    string dateString = parser.ParsePattern(date, this.datePicker.UseNativeDigits ? this.datePicker.Culture.NumberFormat.NativeDigits : null);
                    this.dayPartIndex = parser.DayPartIndex;
                    this.monthPartIndex = parser.MonthPartIndex;
                    this.yearPartIndex = parser.YearPartIndex;
                    List<CharacterRange> rangeList = new List<CharacterRange>();
                    int dayIndex = parser.DayIndex;
                    int monthIndex = parser.MonthIndex;
                    int yearIndex = parser.YearIndex;
                    if (!string.IsNullOrEmpty(parser.DayString))
                        rangeList.Add(new CharacterRange(dayIndex, parser.DayString.Length));
                    if (!string.IsNullOrEmpty(parser.MonthString))
                        rangeList.Add(new CharacterRange(monthIndex, parser.MonthString.Length));
                    if (!string.IsNullOrEmpty(parser.YearString))
                        rangeList.Add(new CharacterRange(yearIndex, parser.YearString.Length));
                    format.SetMeasurableCharacterRanges(rangeList.ToArray());
                    Rectangle layoutRect = this.ClientRectangle;
                    e.Graphics.DrawString(dateString, this.Font, foreBrush, layoutRect, format);
                    Region[] dateRegions = e.Graphics.MeasureCharacterRanges(dateString, this.Font, layoutRect, format);
                    this.dayBounds = dateRegions[0].GetBounds(e.Graphics);
                    this.monthBounds = dateRegions[1].GetBounds(e.Graphics);
                    this.yearBounds = dateRegions[2].GetBounds(e.Graphics);
                    if(this.selectedPart == SelectedDatePart.Day)
                    {
                        e.Graphics.FillRectangle(selectedBack, this.dayBounds.X, this.dayBounds.Y - 2, this.dayBounds.Width + 1, this.dayBounds.Height + 1);
                        e.Graphics.DrawString(parser.DayString, this.Font, selectedBrush, this.dayBounds.X - 1, this.dayBounds.Y - 1);
                    }
                    if(this.selectedPart == SelectedDatePart.Month)
                    {
                        e.Graphics.FillRectangle(selectedBack, this.monthBounds.X, this.monthBounds.Y - 2, this.monthBounds.Width + 1, this.monthBounds.Height + 1);
                        e.Graphics.DrawString(parser.MonthString, this.Font, selectedBrush, this.monthBounds.X - 1, this.monthBounds.Y - 1);
                    }
                    if(this.selectedPart == SelectedDatePart.Year)
                    {
                        e.Graphics.FillRectangle(selectedBack, this.yearBounds.X, this.yearBounds.Y - 2, this.yearBounds.Width + 1, this.yearBounds.Height + 1);
                        e.Graphics.DrawString(parser.YearString, this.Font, selectedBrush, this.yearBounds.X - 1, this.yearBounds.Y - 1);
                    }
                }
            }
            base.OnPaint(e);
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                this.inputBox.FinishedEditing -= this.InputBoxFinishedEditing;
                this.inputBox.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.selectedPart = SelectedDatePart.Day;
            this.Refresh();
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.selectedPart = SelectedDatePart.None;
            this.Refresh();
        }
        protected override void OnBackColorChanged(EventArgs e)
        {
            if (this.inputBox != null)
                this.inputBox.BackColor = this.BackColor;
            base.OnBackColorChanged(e);
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            if (this.inputBox != null)
                this.inputBox.ForeColor = this.ForeColor;
            base.OnForeColorChanged(e);
        }
        private static bool IsValidDay(int day, DateTime date, Calendar cal)
        {
            return day >= 1 && day <= DateMethods.GetDaysInMonth(new MonthCalendarDate(cal, date));
        }
        private static bool IsValidDay(string day, DateTime date, Calendar cal)
        {
            return IsValidDay(string.IsNullOrEmpty(day) ? 0 : int.Parse(day), date, cal);
        }
        private static bool IsValidMonth(int month, int year, Calendar cal)
        {
            year = cal.ToFourDigitYear(year);
            return month >= 1 && month <= cal.GetMonthsInYear(year);
        }
        private static bool IsValidMonth(string month, int year, Calendar cal)
        {
            return IsValidMonth(string.IsNullOrEmpty(month) ? 0 : int.Parse(month), year, cal);
        }
        private static bool IsValidYear(int year, Calendar cal, int era)
        {
            int minYear = cal.GetYear(cal.MinSupportedDateTime.Date);
            int maxYear = cal.GetYear(cal.MaxSupportedDateTime.Date);
            if(cal.Eras.Length > 1)
            {
                DateTime? minDate = null, maxDate = null;
                DateTime date = cal.MinSupportedDateTime;
                while(date < cal.MaxSupportedDateTime.Date)
                {
                    int e = cal.GetEra(date);
                    if(e == era)
                    {
                        if(minDate == null)
                        {
                            minDate = date;
                        }
                        maxDate = date;
                    }
                    date = cal.AddDays(date, 1);
                }
                minYear = cal.GetYear(minDate.GetValueOrDefault(cal.MinSupportedDateTime.Date));
                maxYear = cal.GetYear(maxDate.GetValueOrDefault(cal.MaxSupportedDateTime.Date));
            }
            year = cal.ToFourDigitYear(year);
            return year >= minYear && year <= maxYear;
        }
        private void SetDatePart(bool left)
        {
            int index = -1;
            switch (this.selectedPart)
            {
                case SelectedDatePart.Day:
                    {
                        index = this.dayPartIndex;
                        break;
                    }
                case SelectedDatePart.Month:
                    {
                        index = this.monthPartIndex;
                        break;
                    }
                case SelectedDatePart.Year:
                    {
                        index = this.yearPartIndex;
                        break;
                    }
            }
            if (index != -1)
            {
                this.selectedPart = this.GetNextSelectedPart(index, left);
            }
            this.Refresh();
        }
        private SelectedDatePart GetNextSelectedPart(int currentIndex, bool left)
        {
            int newIndex = currentIndex + (left ? -1 : 1);
            if (newIndex < 0)
                newIndex = 2;
            else if (newIndex > 2)
                newIndex = 0;
            if (this.dayPartIndex == newIndex)
                return SelectedDatePart.Day;
            if (this.monthPartIndex == newIndex)
                return SelectedDatePart.Month;
            if (this.yearPartIndex == newIndex)
                return SelectedDatePart.Year;
            return SelectedDatePart.None;
        }
        private void SetNewDate(DateTime date)
        {
            this.currentDate = date;
            if(this.CheckDate != null)
            {
                CheckDateEventArgs checkEventArgs = new CheckDateEventArgs(date, true);
                this.CheckDate(this, checkEventArgs);
                this.isValidDate = checkEventArgs.IsValid;
                this.Invalidate();
            }
        }
        private void InputBoxFinishedEditing(object sender, EventArgs e)
        {
            this.inputBox.Visible = false;
            string inputStr = this.inputBox.GetCurrentText();
            bool containsSeparator = inputStr.Contains(this.datePicker.FormatProvider.DateSeparator);
            string aggregate = string.Empty;
            Dictionary<int, string> dic;
            if (containsSeparator)
            {
                aggregate = this.datePicker.FormatProvider.DateSeparator;
                dic = new Dictionary<int, string>
                {
                    { this.yearPartIndex, @"(?<year>\d{2,4})" },
                    { this.dayPartIndex, @"(?<day>\d\d?)" },
                    { this.monthPartIndex, @"(?<month>\d\d?)" }
                };
            }
            else
            {
                int yearLength = inputStr.Length == 8 ? 4 : 2;
                dic = new Dictionary<int, string>
                {
                     { this.yearPartIndex, string.Format(@"(?<year>\d{{{0}}})", yearLength) },
                     { this.dayPartIndex, @"(?<day>\d\d)" },
                     { this.monthPartIndex, @"(?<month>\d\d)" }
                };
            }
            var sortedKeys = dic.Keys.ToList();
            sortedKeys.Sort();
            string regexParttern = sortedKeys.ConvertAll(i => dic[i]).Aggregate((s1, s2) => s1 + aggregate + s2);
            var match = System.Text.RegularExpressions.Regex.Match(inputStr, regexParttern);
            var groups = match.Groups;
            var dayString = groups["day"].Value;
            var monthString = groups["month"].Value;
            var yearString = groups["year"].Value;
            if(match.Success && !string.IsNullOrEmpty(dayString) && !string.IsNullOrEmpty(monthString) && !string.IsNullOrEmpty(yearString))
            {
                int year = int.Parse(yearString);
                Calendar cal = this.datePicker.Picker.CultureCalendar;
                year = cal.ToFourDigitYear(year);
                if(IsValidYear(year, cal, cal.GetEra(DateTime.Today)) && IsValidMonth(monthString, year, cal))
                {
                    DateTime date = new DateTime(year, int.Parse(monthString), 1, cal);
                    if(IsValidDay(dayString, date, cal))
                    {
                        this.Date = cal.AddDays(date, int.Parse(dayString) - 1);
                    }
                }
            }
            this.inEditMode = false;
            this.Focus();
        }

        private class InputDateTextBox : TextBox
        {
            private readonly DatePickerDateTextBox parent;
            public InputDateTextBox(DatePickerDateTextBox parent)
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");
                this.parent = parent;
                this.BorderStyle = BorderStyle.None;
                this.BackColor = parent.BackColor;
            }
            public event EventHandler FinishedEditing;

            public sealed override Color BackColor
            {
                get => base.BackColor; set => base.BackColor = value;
            }
            public string GetCurrentText()
            {
                var input = this.Text;
                if (string.IsNullOrEmpty(input))
                    return string.Empty;
                if (this.parent.datePicker.UseNativeDigits)
                    return input.ToList().ConvertAll(this.GetArabicNumeralString).Aggregate((s1, s2) => s1 + s2);
                return input;
            }
            protected override bool ProcessDialogKey(Keys keyData)
            {
                if(keyData == Keys.Enter || keyData == Keys.Tab || keyData == Keys.Escape)
                {
                    if (keyData == Keys.Escape)
                        this.Text = string.Empty;
                    this.RaiseFinishedEditing();
                    return true;
                }
                return base.ProcessDialogKey(keyData);
            }
            protected override void OnKeyDown(KeyEventArgs e)
            {
                if(this.parent.datePicker.AllowPromptAsInput)
                {
                    base.OnKeyDown(e);
                    return;
                }
                e.Handled = true;
                if((e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)) 
                {
                    switch(e.KeyCode)
                    {
                        case Keys.Back:
                        case Keys.Left:
                        case Keys.Right:
                        case Keys.Home:
                        case Keys.End:
                            {
                                e.Handled = false;
                                break;
                            }
                        default:
                            {
                                e.SuppressKeyPress = true;
                                break;
                            }
                    }
                }
                base.OnKeyDown(e);
            }
            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                bool isNumber = char.IsNumber(e.KeyChar);
                bool isSeparator = this.parent.datePicker.FormatProvider.DateSeparator.Contains(e.KeyChar);
                bool textContainsSeparator = this.Text.Contains(this.parent.datePicker.FormatProvider.DateSeparator);
                int txtLength = textContainsSeparator ? 10 : 8;
                if(isSeparator)
                {
                    if(this.Text.Length == txtLength && e.KeyChar != '\b')
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        base.OnKeyPress(e);
                    }
                    return;
                }
                if((!isNumber || this.Text.Length == txtLength) && e.KeyChar != '\b')
                {
                    e.Handled = true;
                    return;
                }
                if(this.parent.datePicker.UseNativeDigits && isNumber)
                {
                    int number = int.Parse(e.KeyChar.ToString(CultureInfo.InvariantCulture));
                    string nativeNumber = DateMethods.GetNativeNumberString(number, this.parent.datePicker.Culture.NumberFormat.NativeDigits, false);
                    e.KeyChar = nativeNumber[0];
                }
                base.OnKeyPress(e);
            }
            protected override void OnLostFocus(EventArgs e)
            {
                base.OnLostFocus(e);
                this.RaiseFinishedEditing();
            }
            private void RaiseFinishedEditing()
            {
                EventHandler handler = this.FinishedEditing;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
            private string GetArabicNumeralString(char nativeDigit)
            {
                var nativeDigits = this.parent.datePicker.Culture.NumberFormat.NativeDigits;
                for(int i= 0; i < 10; i++)
                {
                    if (nativeDigit == nativeDigits[i][0])
                        return i.ToString(CultureInfo.InvariantCulture);
                }
                return nativeDigit.ToString(CultureInfo.CurrentUICulture);
            }
        }
        private class DatePatternParser
        {
            private readonly ICustomFormatProvider provider;
            private readonly string pattern = string.Empty;
            private string dayString = string.Empty;
            private string dayNameString = string.Empty;
            private string monthString = string.Empty;
            private string yearString = string.Empty;
            private string eraString = string.Empty;
            private int dayPartIndex = -1;
            private int monthPartIndex = -1;
            private int yearPartIndex = -1;
            private int dayIndex = -1;
            private int monthIndex = -1;
            private int yearIndex = -1;
            private bool isDayNumber;
            private bool isMonthNumber;

            public DatePatternParser(string pattern, ICustomFormatProvider provider)
            {
                if (string.IsNullOrEmpty(pattern))
                    throw new InvalidOperationException("parameter 'pattern' cannot be null or empty.");
                if (provider == null)
                    throw new ArgumentNullException("provider", "parameter 'provider' cannot be null.");
                this.provider = provider;
                this.pattern = pattern;
            }
            public string DayString
            {
                get { return this.dayString; }
            }
            public string DayNameString
            {
                get { return this.dayNameString; }
            }
            public string MonthString
            {
                get { return this.monthString; }
            }
            public string YearString
            {
                get { return this.yearString; }
            }
            public string EraString
            {
                get { return this.eraString; }
            }
            public bool IsDayNumber
            {
                get { return this.isDayNumber; }
            }
            public bool IsMonthNumber
            {
                get { return this.isMonthNumber; }
            }
            public int DayPartIndex
            {
                get { return this.dayPartIndex; }
            }
            public int MonthPartIndex
            {
                get { return this.monthPartIndex; }
            }
            public int YearPartIndex
            {
                get { return this.yearPartIndex; }
            }
            public int DayIndex
            {
                get { return this.dayIndex; }
            }
            public int MonthIndex
            {
                get { return this.monthIndex; }
            }
            public int YearIndex
            {
                get { return this.yearIndex; }
            }
            public string ParsePattern(MonthCalendarDate date, string[] nativeDigits = null)
            {
                string format = this.pattern.Replace(provider.DateSeparator, "/");
                StringBuilder sb = new StringBuilder();
                Calendar c = provider.Calendar;
                int i = 0;
                int index = 0;
                while(i < format.Length)
                {
                    int tokLen;
                    char ch = format[i];
                    string currentString;
                    switch (ch)
                    {
                        case 'd':
                            {
                                tokLen = CountChar(format, i, ch);
                                if (tokLen <= 2)
                                {
                                    currentString = DateMethods.GetNumberString(date.Day, nativeDigits, tokLen == 2);
                                    this.isDayNumber = true;
                                    this.dayString = currentString;
                                    this.dayPartIndex = index++;
                                    this.dayIndex = sb.Length;
                                }
                                else
                                {
                                    currentString = tokLen == 3 ? provider.GetAbbreviatedDayName(c.GetDayOfWeek(date.Date)) : provider.GetDayName(c.GetDayOfWeek(date.Date));
                                    this.dayNameString = currentString;
                                }
                                sb.Append(currentString);
                                break;
                            }
                        case 'M':
                            {
                                tokLen = CountChar(format, i, ch);
                                if (tokLen <= 2)
                                {
                                    currentString = DateMethods.GetNumberString(date.Month, nativeDigits, tokLen == 2);
                                    this.isMonthNumber = true;
                                }
                                else
                                {
                                    currentString = tokLen == 3 ? provider.GetAbbreviatedMonthName(date.Year, date.Month) : provider.GetMonthName(date.Year, date.Month);
                                }
                                this.monthPartIndex = index++;
                                this.monthIndex = sb.Length;
                                this.monthString = currentString;
                                sb.Append(currentString);
                                break;
                            }
                        case 'y':
                            {
                                tokLen = CountChar(format, i, ch);
                                int year = tokLen <= 2 ? date.Year % 100 : date.Year;
                                currentString = DateMethods.GetNumberString(year, nativeDigits, tokLen <= 2);
                                this.yearString = currentString;
                                this.yearPartIndex = index++;
                                this.yearIndex = sb.Length;
                                sb.Append(currentString);
                                break;
                            }
                        case 'g':
                            {
                                tokLen = CountChar(format, i, ch);
                                currentString = provider.GetEraName(c.GetEra(date.Date));
                                this.eraString = currentString;
                                sb.Append(currentString);
                                break;
                            }
                        case '/':
                            {
                                tokLen = CountChar(format, i, ch);
                                sb.Append(provider.DateSeparator);
                                break;
                            }
                        default:
                            {
                                tokLen = 1;
                                sb.Append(ch.ToString(CultureInfo.CurrentUICulture));
                                break;
                            }
                    }
                    i += tokLen;
                }
                return sb.ToString();
            }
            private static int CountChar(string format, int p, char c)
            {
                int len = format.Length;
                int i = p + 1;
                while((i < len) && (format[i] == c))
                {
                    i++;
                }
                return i - p;
            }
        }
    }
}
