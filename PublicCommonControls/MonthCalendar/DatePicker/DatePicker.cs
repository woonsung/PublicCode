using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;


namespace PublicCommonControls.WCalendar
{
    [Designer(typeof(Design.DatePickerControlDesigner))]
    [ToolboxBitmap(typeof(DateTimePicker))]
    [DefaultEvent("ValueChanged")]
    public class DatePicker : Control
    {
        private static readonly ButtonColors[] ColorArray;
        private readonly ToolStripControlHost monthCalendarHost;
        private readonly MonthCalendar monthCalendar;
        private readonly DatePickerDateTextBox dateTextBox;
        private CustomToolStripDropDown dropDown;
        private Color borderColor;
        private Color focusedBorderColor;
        private Rectangle buttonBounds;
        private ComboButtonState buttonState;
        private bool isDropped;
        private bool cancelClosing;
        private bool isFocused;
        private Color buttonBackColor;
        private Color buttonPushedColor;
        private Color buttonMouseOver;
        private float imagePercentage;
        static DatePicker()
        {
            ColorArray = new[]
            {
                new ButtonColors
                {
                   TL = Color.FromArgb(209, 224, 253),
                   BL = Color.FromArgb(171, 193, 244),
                   BB = Color.FromArgb(183, 198, 241),
                   BR = Color.FromArgb(176, 197, 242),
                   TRR = Color.FromArgb(188, 204, 243),
                   TR = Color.FromArgb(175, 197, 244),
                   BS = Color.FromArgb(225, 234, 254),
                   BE = Color.FromArgb(174, 200, 247)
                },
                new ButtonColors
                {
                   TL = Color.FromArgb(180, 199, 235),
                   BL = Color.FromArgb(135, 160, 222),
                   BB = Color.FromArgb(147, 167, 223),
                   BR = Color.FromArgb(140, 167, 222),
                   TRR = Color.FromArgb(155, 175, 224),
                   TR = Color.FromArgb(138, 166, 227),
                   BS = Color.FromArgb(253, 255, 255),
                   BE = Color.FromArgb(185, 218, 251)
                },
                new ButtonColors
                {
                   TL = Color.FromArgb(162, 172, 220),
                   BL = Color.FromArgb(115, 129, 217),
                   BB = Color.FromArgb(185, 201, 243),
                   BR = Color.FromArgb(176, 179, 242),
                   TRR = Color.FromArgb(188, 204, 243),
                   TR = Color.FromArgb(119, 134, 217),
                   BS = Color.FromArgb(110, 142, 241),
                   BE = Color.FromArgb(210, 222, 235)
                }
            };
        }
        public DatePicker()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            this.borderColor = Color.FromArgb(127, 157, 185);
            this.focusedBorderColor = Color.Blue;
            this.Width = 101;
            this.Height = 22;
            this.AllowPromptAsInput = false;
            this.monthCalendar = new MonthCalendar { SelectionMode = MonthCalendarSelectionMode.Day };
            this.monthCalendar.KeyPress += MonthCalendarKeyPress;
            this.dateTextBox = new DatePickerDateTextBox(this)
            {
                Font = this.Font,
                Location = new Point(2, 2),
                Date = DateTime.Today,
                Width = this.Width - 19,
                Height = 18,
                MinDate = this.monthCalendar.MinDate,
                MaxDate = this.monthCalendar.MaxDate
            };
            this.dateTextBox.CheckDate += DateTextBoxCheckDate;
            this.dateTextBox.GotFocus += FocusChanged;
            this.dateTextBox.LostFocus += FocusChanged;
            this.Controls.Add(this.dateTextBox);
            this.monthCalendar.DateSelected += MonthCalendarDateSelected;
            this.monthCalendar.ActiveDateChanged += MonthCalendarActiveDateChanged;
            this.monthCalendar.DateClicked += MonthCalendarDateClicked;
            this.monthCalendar.InternalDateSelected += MonthCalendarInternalDateSelected;
            this.monthCalendar.GotFocus += FocusChanged;
            this.monthCalendar.LostFocus += FocusChanged;

            this.monthCalendarHost = new ToolStripControlHost(this.monthCalendar);
            this.monthCalendarHost.LostFocus += MonthCalendarHostLostFocus;
            this.monthCalendarHost.GotFocus += FocusChanged;

            this.dropDown = new CustomToolStripDropDown
            {
                DropShadowEnabled = true
            };
            this.dropDown.Closing += DropDownClosing;
            this.dropDown.GotFocus += FocusChanged;
            this.dropDown.LostFocus += FocusChanged;
            this.dropDown.Items.Add(this.monthCalendarHost);
            this.monthCalendar.MonthMenu.OwnerItem = this.monthCalendarHost;
            this.monthCalendar.YearMenu.OwnerItem = this.monthCalendarHost;
            this.monthCalendar.MonthMenu.ItemClicked += MenuItemClicked;
            this.monthCalendar.YearMenu.ItemClicked += MenuItemClicked;
            this.BackColor = SystemColors.Window;
            this.ClosePickerOnDayClick = true;
            this.buttonBackColor = Color.White;
            this.buttonMouseOver = Color.FromArgb(0, 122, 204);
            this.buttonPushedColor = Color.FromArgb(100, 100, 100);
            this.imagePercentage = 1f;
        }

        [Category("Action")]
        [Description("Is raised when the date value changed.")]
        public event EventHandler<CheckDateEventArgs> ValueChanged;
        [Category("Action")]
        [Description("Is raised when the mouse is over an date.")]
        public event EventHandler<ActiveDateChangedEventArgs> ActiveDateChanged;

        private enum ComboButtonState
        {
            Normal = 0,
            Hot,
            Pressed,
            None,
        }
        [Description("Sets the border color.")]
        [DefaultValue(typeof(Color), "127,157,185")]
        [Category("Appearance")]
        public Color BorderColor
        {
            get
            {
                return this.borderColor;
            }
            set
            {
                if (value == this.borderColor || value.IsEmpty)
                    return;
                this.borderColor = value;
                this.Refresh();
            }
        }
        [Description("The currently selected date.")]
        [Category("Behavior")]
        public DateTime Value
        {
            get
            {
                return this.monthCalendar.SelectionRange.Start;
            }
            set
            {
                if (this.monthCalendar.SelectionStart == value || value < this.MinDate || value > this.MaxDate)
                    return;
                this.monthCalendar.SelectionStart = value;
                this.dateTextBox.Date = value;
                this.monthCalendar.EnsureSelectedDateIsVisible();
            }
        }
        [Description("The minimum selectable date.")]
        [Category("Behavior")]
        public DateTime MinDate
        {
            get
            {
                return this.monthCalendar.MinDate;
            }
            set
            {
                this.monthCalendar.MinDate = value;
                this.dateTextBox.MinDate = this.monthCalendar.MinDate;
            }
        }
        [Description("The maximum selectable date.")]
        [Category("Behavior")]
        public DateTime MaxDate
        {
            get
            {
                return this.monthCalendar.MaxDate;
            }
            set
            {
                this.monthCalendar.MaxDate = value;
                this.dateTextBox.MaxDate = this.monthCalendar.MaxDate;
            }
        }
        [Category("Appearance")]
        [Description("Percentage of button image.")]
        [DefaultValue(1f)]
        public float ImagePercentage
        {
            get { return imagePercentage; }
            set
            {
                imagePercentage = value;
                this.Invalidate();
            }
        }
        [Category("Appearance")]
        [Description("The backcolor of button.")]
        [DefaultValue(typeof(Color), "White")]
        public Color ButtonBackColor
        {
            get { return this.buttonBackColor; }
            set
            {
                this.buttonBackColor = value;
                this.Invalidate();
            }
        }
        [Category("Appearance")]
        [Description("The of backcolor button when mouse over.")]
        [DefaultValue(typeof(Color), "0, 122, 204")]
        public Color ButtonMouseOverColor
        {
            get { return this.buttonMouseOver; }
            set
            {
                this.buttonMouseOver = value;
                this.Invalidate();
            }
        }
        [Category("Appearance")]
        [Description("The backcolor of button when pushed.")]
        [DefaultValue(typeof(Color), "100, 100, 100")]
        public Color ButtonPhushedBackColor
        {
            get { return this.buttonPushedColor; }
            set
            {
                this.buttonPushedColor = value;
                this.Invalidate();
            }
        }
        [Category("Appearance")]
        [Description("The backcolor for invalid dates in the text portion.")]
        [DefaultValue(typeof(Color), "Red")]
        public Color InvalidBackColor
        {
            get { return this.dateTextBox.InvalidBackColor; }
            set { this.dateTextBox.InvalidBackColor = value; }
        }
        [Category("Appearance")]
        [Description("The text color for invalid dates in th text portion.")]
        public Color InvalidForeColor
        {
            get { return this.dateTextBox.InvalidForeColor; }
            set { this.dateTextBox.InvalidForeColor = value; }
        }
        [Category("Appearance")]
        [Description("The border color if the control is focused.")]
        [DefaultValue(typeof(Color), "Blue")]
        public Color FocusedBorderColor
        {
            get
            {
                return this.focusedBorderColor;
            }
            set
            {
                if (value == this.focusedBorderColor || value.IsEmpty)
                    return;
                this.focusedBorderColor = value;
                if (this.Focused)
                    Invalidate();
            }
        }
        [Category("Apperance")]
        [Description("The color table for the picker part.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MonthCalendarColorTable PickerColorTable
        {
            get
            {
                return this.monthCalendar.ColorTable;
            }
            set
            {
                this.monthCalendar.ColorTable = value;
            }
        }
        [Category("Appearance")]
        [Description("The font for the days in the picker.")]
        public Font PickerDayFont
        {
            get { return this.monthCalendar.Font; }
            set { this.monthCalendar.Font = value; }
        }
        [Category("Appearance")]
        [Description("The font for the picker header.")]
        public Font PickerHeaderFont
        {
            get
            {
                return this.monthCalendar.HeaderFont;
            }
            set
            {
                this.monthCalendar.HeaderFont = value;
            }
        }
        [Category("Appearance")]
        [Description("The font for the picker footer.")]
        public Font PickerFooterFont
        {
            get { return this.monthCalendar.FooterFont; }
            set { this.monthCalendar.FooterFont = value; }
        }
        [Category("Appearance")]
        [Description("The font for the picker day header.")]
        public Font PickerDayHeaderFont
        {
            get { return this.monthCalendar.DayHeaderFont; }
            set { this.monthCalendar.DayHeaderFont = value; }
        }
        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        [Description("Determines the text alignment for the days in the picker.")]
        [Category("Appearance")]
        public ContentAlignment PickerDayTextAlignment
        {
            get { return this.monthCalendar.DayTextAlignment; }
            set { this.monthCalendar.DayTextAlignment = value; }
        }
        [Description("The bolded dates in the picker.")]
        public List<DateTime> PickerBoldedDates
        {
            get { return this.monthCalendar.BoldedDates; }
            set { this.monthCalendar.BoldedDates = value; }
        }
        [Description("The bolded dates in th caledar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BoldedDatesCollection BoldedDatesCollection
        {
            get
            {
                return this.monthCalendar.BoldedDatesCollection;
            }
        }
        [Description("The bold date categories in the calendar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BoldedDateCategoryCollection BoldedDateCategoryCollection
        {
            get
            {
                return this.monthCalendar.BoldedDateCategoryCollection;
            }
        }
        [Category("Behavior")]
        [Description("The culture used by the DatePicker.")]
        [TypeConverter(typeof(Design.CultureInfoCustomTypeConverter))]
        public CultureInfo Culture
        {
            get
            {
               return this.monthCalendar.Culture;
            }
            set
            {
                if (value == null || value.IsNeutralCulture)
                    return;
                this.monthCalendar.Culture = value;
                this.MinDate = this.monthCalendar.MinDate;
                this.MaxDate = this.monthCalendar.MaxDate;
                this.RightToLeft = this.monthCalendar.UseRTL ? RightToLeft.Yes : RightToLeft.Inherit;
                this.Invalidate();
            }
        }
        [Category("Behavior")]
        [Description("The calendar used by the MonthCalendar.")]
        [Editor(typeof(Design.MonthCalendarCalendarUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(Design.MonthCalendarCalendarTypeConverter))]
        public Calendar CultureCalendar
        {
            get
            {
                return this.monthCalendar.CultureCalendar;
            }
            set
            {
                this.monthCalendar.CultureCalendar = value;
                this.MinDate = this.monthCalendar.MinDate;
                this.MaxDate = this.monthCalendar.MaxDate;
                this.Invalidate();
            }
        }
        [TypeConverter(typeof(Design.MonthCalendarNamesProviderTypeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Behavior")]
        [Description("Culture dependent settings for month/day names and day formatting.")]
        public ICustomFormatProvider FormatProvider
        {
            get { return this.monthCalendar.FormatProvider; }
            set { this.monthCalendar.FormatProvider = value; }
        }
        [Category("Appearance")]
        [Description("Show the week header in the picker.")]
        [DefaultValue(false)]
        public bool ShowPickerWeekHeader
        {
            get { return this.monthCalendar.ShowWeekHeader; }
            set { this.monthCalendar.ShowWeekHeader = value; }
        }
        [Category("Behavior")]
        [Description("Whether to close the picker on clicking a day or not (regardless whether the day is already selected).")]
        [DefaultValue(false)]
        public bool ClosePickerOnDayClick { get; set; }
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indicates whether to use the shortest or the abbreviated day names in the day header of the picker.")]
        public bool UseShortestDayNames
        {
            get { return this.monthCalendar.UseShortestDayNames; }
            set { this.monthCalendar.UseShortestDayNames = value; }
        }
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indicates whether to use the native digits as specified by the current Culture property.")]
        public bool UseNativeDigits
        {
            get { return this.monthCalendar.UseNativeDigits; }
            set { this.monthCalendar.UseNativeDigits = value; }
        }
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Allows the input to be the current date separator and tries to parse the date after the editing of the date finished.")]
        public bool AllowPromptAsInput { get; set; }

        [Category("Appearance")]
        [Description("The picker dimension.")]
        [DefaultValue(typeof(Size), "1,1")]
        public Size PickerDimension
        {
            get { return this.monthCalendar.CalendarDimensions; }
            set { this.monthCalendar.CalendarDimensions = value; }
        }
        public override bool Focused
        {
            get { return base.Focused || this.dateTextBox.Focused || this.monthCalendar.Focused || this.monthCalendarHost.Focused || this.dropDown.Focused; }
        }
        public sealed override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                this.dateTextBox.BackColor = value;
            }
        }
        public sealed override Font Font
        {
            get => base.Font;
            set => base.Font = value;
        }
        internal MonthCalendar Picker
        {
            get { return this.monthCalendar; }
        }
        internal void SwitchPickerState()
        {
            if(this.isDropped)
            {
                this.buttonState = ComboButtonState.Hot;
                this.isDropped = false;
                this.dropDown.Close(ToolStripDropDownCloseReason.CloseCalled);
                this.Focus();
            }
            else
            {
                if (this.buttonState == ComboButtonState.Pressed)
                    this.buttonState = ComboButtonState.Hot;
                else if (this.buttonState == ComboButtonState.None)
                    this.buttonState = ComboButtonState.Hot;
                else
                {
                    this.buttonState = ComboButtonState.Pressed;
                    this.Refresh();
                    this.ShowDropDown();
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(this.dropDown != null)
                {
                    this.dropDown.Closing -= this.DropDownClosing;
                    this.dropDown.GotFocus -= this.FocusChanged;
                    this.dropDown.LostFocus -= this.FocusChanged;
                    this.dropDown.Dispose();
                    this.dropDown = null;
                }
                this.monthCalendar.DateSelected -= this.MonthCalendarDateSelected;
                this.monthCalendar.InternalDateSelected -= this.MonthCalendarInternalDateSelected;
                this.monthCalendar.ActiveDateChanged -= this.MonthCalendarActiveDateChanged;
                this.monthCalendar.DateClicked -= this.MonthCalendarDateClicked;
                this.monthCalendar.GotFocus -= this.FocusChanged;
                this.monthCalendar.LostFocus -= this.FocusChanged;
                this.monthCalendar.MonthMenu.ItemClicked -= MenuItemClicked;
                this.monthCalendar.YearMenu.ItemClicked -= MenuItemClicked;
                this.monthCalendarHost.LostFocus -= this.MonthCalendarHostLostFocus;
                this.monthCalendar.GotFocus -= this.FocusChanged;
                this.monthCalendar.Dispose();
                this.monthCalendarHost.Dispose();
                this.dateTextBox.CheckDate -= this.DateTextBoxCheckDate;
                this.dateTextBox.GotFocus -= this.FocusChanged;
                this.dateTextBox.LostFocus -= this.FocusChanged;
                this.dateTextBox.Dispose();
            }
            base.Dispose(disposing);
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (width < 19)
                width = 19;
            height = this.MeasureControlSize();
            if (this.dateTextBox != null)
                this.dateTextBox.Size = new Size(Math.Max(width - this.Height, 0), height - 4);
            base.SetBoundsCore(x, y, width, height, specified);
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if(keyData == Keys.Space && !this.dateTextBox.InEditMode)
            {
                this.SwitchPickerState();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = this.ClientRectangle;
            rect.Width--;
            rect.Height--;
            e.Graphics.Clear(this.Enabled ? this.BackColor : SystemColors.Window);
            using (Pen p = new Pen(this.Focused ? this.focusedBorderColor : MonthCalendarAbstractRenderer.GetGrayColor(this.Enabled, this.borderColor)))
            {
                e.Graphics.DrawRectangle(p, rect);
            }
            Rectangle btnRect = rect;
            btnRect.X = rect.Right - rect.Height;
            btnRect.Width = rect.Height;
            btnRect.Height = rect.Height;
            this.buttonBounds = btnRect;
            DrawButton(e.Graphics, btnRect, this.buttonState, this.Enabled);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            if(!this.isDropped)
            {
                this.buttonState = ComboButtonState.Hot;
                this.Refresh();
            }
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            //if(this.isDropped)
            //{
                this.buttonState = ComboButtonState.Normal;
                this.Invalidate();
            //}
            base.OnMouseLeave(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            if(e.Button == MouseButtons.Left && this.buttonBounds.Contains(e.Location))
            {
                this.SwitchPickerState();
                this.Refresh();
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if(e.Button == MouseButtons.None && !this.isDropped)
            {
                ComboButtonState st = this.buttonState;
                this.buttonState = this.buttonBounds.Contains(e.Location) ? ComboButtonState.Hot : ComboButtonState.Normal;
                if (st != this.buttonState)
                    this.Invalidate();
            }
        }
        protected override void OnLostFocus(EventArgs e)
        {
            if(!this.isDropped)
            {
                this.buttonState = ComboButtonState.Normal;
                this.Invalidate();
            }
            if(!this.Focused)
                base.OnLostFocus(e);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.Invalidate();
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.dateTextBox.Font = this.Font;
            this.Height = Math.Max(22, this.MeasureControlSize());
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            if (this.dateTextBox != null)
                this.dateTextBox.ForeColor = this.ForeColor;
            base.OnForeColorChanged(e);
        }
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.Invalidate();
        }
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            this.dateTextBox.RightToLeft = this.RightToLeft;
            this.dateTextBox.Refresh();
        }
        protected virtual void OnValueChanged(CheckDateEventArgs e)
        {
            if (this.ValueChanged != null)
                this.ValueChanged(this, e);
        }
        protected virtual void OnActiveDateChanged(ActiveDateChangedEventArgs e)
        {
            if (this.ActiveDateChanged != null)
                this.ActiveDateChanged(this, e);
        }
        private void DrawButton(Graphics g, Rectangle rect, ComboButtonState buttonstate, bool enabled)
        {
            if (g == null || rect.IsEmpty)
                return;
            RectangleF r = rect;
            r.X = r.X + 1;
            r.Y = r.Y + 1;
            r.Width = r.Width - 1;
            r.Height = r.Height - 1;
            if(!enabled)
                g.FillRectangle(new SolidBrush(SystemColors.InactiveBorder), r);
            else
            {
                switch (buttonstate)
                {
                    case ComboButtonState.Hot:
                        g.FillRectangle(new SolidBrush(buttonMouseOver), r);
                        break;
                    case ComboButtonState.Pressed:
                        g.FillRectangle(new SolidBrush(buttonPushedColor), r);
                        break;
                    default:
                        g.FillRectangle(new SolidBrush(buttonBackColor), r);
                        break;
                }
            }
            using (Pen p = new Pen(Color.Black))
            {
                g.DrawRectangle(p, new Rectangle(rect.X + 2, rect.Y + 1, rect.Width - 2, rect.Height - 2));
            }
            r.X += rect.Height * (1 - imagePercentage) / 2;
            r.Y += rect.Height * (1 - imagePercentage) / 2;
            r.Width -= rect.Height * (1 - imagePercentage);
            r.Height -= rect.Height * (1 - imagePercentage);

            using (Bitmap icon = PublicCommonControls.Properties.Resources.NewIcon_Calendar_Black)
            using (Bitmap icon_white = PublicCommonControls.Properties.Resources.NewIcon_Calendar_White)
            using (Bitmap icon_diabled = PublicCommonControls.Properties.Resources.NewIcon_Calendar_Disabled)
            {
                if (enabled)
                {
                    switch (buttonstate)
                    {
                        case ComboButtonState.Hot:
                        case ComboButtonState.Pressed:
                            g.DrawImage(icon_white, r);
                            break;
                        default:
                            g.DrawImage(icon, r);
                            break;
                    }

                }
                else
                {
                    g.DrawImage(icon_diabled, r);
                }
            }

        }
        private void DropDownClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (this.cancelClosing)
            {
                this.cancelClosing = false;
                e.Cancel = true;
            }
            else
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.CloseCalled)
                {
                    this.buttonState = ComboButtonState.Hot;
                    this.Invalidate();
                }
                else
                {
                    this.isDropped = false;
                }
            }
        }
        private void MenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.cancelClosing = true;
        }
        private void MonthCalendarHostLostFocus(object sender, EventArgs e)
        {
            if(this.isDropped)
            {
                this.buttonState = ComboButtonState.None;
                this.dropDown.Close(ToolStripDropDownCloseReason.AppFocusChange);
            }
            this.FocusChanged(this, EventArgs.Empty);
        }
        private void MonthCalendarDateSelected(object sender, DateRangeEventArgs e)
        {
            this.buttonState = ComboButtonState.Normal;
            this.dropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
            this.dateTextBox.Date = e.Start;
        }
        private void MonthCalendarInternalDateSelected(object sender, DateEventArgs e)
        {
            this.dateTextBox.Date = e.Date;
        }
        private void MonthCalendarActiveDateChanged(object sender, ActiveDateChangedEventArgs e)
        {
            this.OnActiveDateChanged(e);
        }
        private void MonthCalendarDateClicked(object sender, DateEventArgs e)
        {
            if(this.ClosePickerOnDayClick)
            {
                this.buttonState = ComboButtonState.Normal;
                this.dropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
            }
        }
        private void DateTextBoxCheckDate(object sneder, CheckDateEventArgs e)
        {
            this.monthCalendar.SelectionRange = new SelectionRange(e.Date, e.Date);
            this.monthCalendar.EnsureSelectedDateIsVisible();
            CheckDateEventArgs newArgs = new CheckDateEventArgs(e.Date, this.IsValidDate(e.Date));
            this.OnValueChanged(newArgs);
            e.IsValid = newArgs.IsValid;
        }
        private void MonthCalendarKeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Space)
            {
                this.SwitchPickerState();
                e.Handled = true;
            }
        }
        private void FocusChanged(object sender, EventArgs e)
        {
            if(this.isFocused != this.Focused)
            {
                this.isFocused = this.Focused;
                this.Invalidate();
            }
        }
        private void ShowDropDown()
        {
            if(this.dropDown != null)
            {
                this.isDropped = true;
                this.monthCalendar.EnsureSelectedDateIsVisible();
                int x = 0, y = this.Height;
                if(this.RightToLeft == RightToLeft.Yes)
                {
                    x = this.monthCalendar.Size.Width + Math.Abs(this.monthCalendar.Size.Width - this.Width);
                }
                this.dropDown.Show(this, x, y);
                this.monthCalendar.Focus();
            }
        }
        private bool IsValidDate(DateTime date)
        {
            return date >= this.MinDate && date <= this.MaxDate;
        }
        private int MeasureControlSize()
        {
            using (Graphics g = this.CreateGraphics())
                return this.MeasureControlSize(g);
        }
        private int MeasureControlSize(Graphics g)
        {
            if (g == null)
                return 22;
            return Size.Round(g.MeasureString(DateTime.Today.ToShortDateString(), this.Font)).Height + 8;
        }
        private bool ShouldSerializeCulture()
        {
            return this.Picker.ShouldSerializeCulture();
        }
        private void ResetCulture()
        {
            this.Picker.ResetCulture();
        }
        private bool ShouldSerializeCultureCalendar()
        {
            return this.Picker.ShouldSerializeCultureCalendar();
        }
        private void ResetCultureCalendar()
        {
            this.Picker.ResetCultureCalendar();
        }
        private bool ShouldSerializeMindate()
        {
            return this.Picker.ShoulSerializeMindate();
        }
        private void ResetMindate()
        {
            this.Picker.ResetMinDate();
            this.dateTextBox.MinDate = this.monthCalendar.MinDate;
        }
        private bool ShouldSerializeMaxDate()
        {
            return this.Picker.ShouldSerializeMaxDate();
        }
        private void ResetMaxDate()
        {
            this.Picker.ResetMaxDate();
            this.dateTextBox.MaxDate = this.monthCalendar.MaxDate;
        }
        private bool ShouldSerializeValue()
        {
            return this.Value != DateTime.Today;
        }
        private void ResetValue()
        {
            this.Value = DateTime.Today;
        }
        private bool ShouldSerializeInvalidForeColor()
        {
            return this.InvalidForeColor != this.ForeColor;
        }
        private void ResetInvalidForeColor()
        {
            this.InvalidForeColor = this.ForeColor;
        }
        private bool ShouldSerializePickerColorTable()
        {
            return this.monthCalendar.ShouldSerializeColorTable();
        }
        private void ResetPickerColorTable()
        {
            this.monthCalendar.ResetColorTable();
            this.Invalidate();
        }
        private bool ShouldSerializeBackColor()
        {
            return this.BackColor != SystemColors.Window;
        }
        private new void ResetBackColor()
        {
            this.BackColor = SystemColors.Window;
        }
        private bool ShouldSerializePickerHeaderFont()
        {
            return this.PickerHeaderFont.Name != "Segoe UI" || !this.PickerHeaderFont.Size.Equals(9f) || this.PickerHeaderFont.Style != FontStyle.Regular;
        }
        private void ResetPickerHeaderFont()
        {
            this.PickerHeaderFont.Dispose();
            this.PickerHeaderFont = new Font("Segoe UI", 9f, FontStyle.Regular);
        }
        private bool ShouldSerializePickerFooterFont()
        {
            return this.PickerFooterFont.Name != "Arial" || !this.PickerFooterFont.Size.Equals(9f) || this.PickerFooterFont.Style != FontStyle.Bold;
        }
        private void ResetPickerFooterFont()
        {
            this.PickerFooterFont.Dispose();
            this.PickerFooterFont = new Font("Arial", 9f, FontStyle.Bold);
        }
        private bool ShouldSerializePickerDayHeaderFont()
        {
            return this.PickerDayHeaderFont.Name != "Segoe UI" || !this.PickerDayHeaderFont.Size.Equals(8f) || this.PickerDayHeaderFont.Style != FontStyle.Regular;
        }
        private void ResetPickerDayHeaderFont()
        {
            this.PickerDayHeaderFont.Dispose();
            this.PickerDayHeaderFont = new Font("Segoe UI", 8f, FontStyle.Regular);
        }
        private bool ShouldSerializePickerDayFont()
        {
            return this.PickerDayFont.Name != "Microsoft Sans Serif" || !this.PickerDayFont.Size.Equals(8.25f) || this.PickerDayFont.Style != FontStyle.Regular;
        }
        private void ResetPickerDayFont()
        {
            this.PickerDayFont.Dispose();
            this.PickerDayFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
        }
        private bool ShouldSerializePickerBoldedDates()
        {
            return this.PickerBoldedDates.Count != 0;
        }
        private void ResetPickerBoldedDates()
        {
            this.PickerBoldedDates = null;
            if (this.isDropped)
                this.monthCalendar.Refresh();
        }

        private struct ButtonColors
        {
            public Color TL { get; set; }
            public Color BL { get; set; }
            public Color BB { get; set; }
            public Color BR { get; set; }
            public Color TRR { get; set; }
            public Color TR { get; set; }
            public Color BS { get; set; }
            public Color BE { get; set; }
        }
        private class CustomToolStripDropDown :ToolStripDropDown
        {
            protected override Padding DefaultPadding
            {
                get { return new Padding(0, 0, 0, -2); }
            }
        }

    }
}
