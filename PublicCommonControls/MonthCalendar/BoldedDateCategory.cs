using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PublicCommonControls.WCalendar
{
    public struct BoldedDateCategory
    {
        private string name;
        public BoldedDateCategory(string name)
            : this()
        {
            if (string.IsNullOrEmpty(name))
                throw new InvalidOperationException("parameter 'name' is invalid");
            this.name = name.Trim();
            this.GradientMode = LinearGradientMode.Vertical;
        }
        public static BoldedDateCategory Empty
        {
            get
            {
                return new BoldedDateCategory
                {
                    BackColorStart = Color.Empty,
                    BackColorEnd = Color.Empty,
                    GradientMode = LinearGradientMode.Vertical,
                    ForeColor = Color.Empty,
                    name = string.Empty
                };
            }
        }
        public string Name
        {
            get { return this.name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    this.name = value.Trim();
            }
        }
        public Color ForeColor { get; set; }
        public Color BackColorStart { get; set; }
        public Color BackColorEnd { get; set; }
        public LinearGradientMode GradientMode { get; set; }
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(this.name) || (this.ForeColor == Color.Empty && this.BackColorStart == Color.Empty);
            }
        }
        public override string ToString()
        {
            return this.name;
        }
    }
}
