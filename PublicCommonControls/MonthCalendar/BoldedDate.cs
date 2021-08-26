using System;

namespace PublicCommonControls.WCalendar
{
    public struct BoldedDate
    {
        public BoldedDateCategory Category { get; set; }
        public DateTime Value { get; set; }
        public bool IsEmpty
        {
            get { return this.Category.IsEmpty || Value == DateTime.MinValue; }
        }
    }
}
