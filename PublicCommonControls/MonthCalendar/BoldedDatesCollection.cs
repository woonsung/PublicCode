using System.Collections.Generic;
using System.Linq;

namespace PublicCommonControls.WCalendar
{
    public class BoldedDatesCollection : List<BoldedDate>
    {
        public new void Add(BoldedDate date)
        {
            if (!this.CanAddItem(date))
                return;
            base.Add(date);
        }
        public new void AddRange(IEnumerable<BoldedDate> dates)
        {
            if (dates == null)
                return;
            dates.ToList().ForEach(this.Add);
        }
        public new void Insert(int index, BoldedDate item)
        {
            if (!this.CanAddItem(item))
                return;
            base.Insert(index, item);
        }
        public new void InsertRange(int index, IEnumerable<BoldedDate> items)
        {
            if (items == null)
                return;
            var list = items.ToList();
            if (list.Any(d => !this.CanAddItem(d)))
                return;
            base.InsertRange(index, list);
        }
        private bool CanAddItem(BoldedDate date)
        {
            return !date.Category.IsEmpty && !this.Exists(d => d.Value.Date == date.Value.Date);
        }
    }
}
