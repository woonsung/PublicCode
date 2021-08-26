using System;
using System.Collections.Generic;
using System.Linq;

namespace PublicCommonControls.WCalendar
{
    public class BoldedDateCategoryCollection : List<BoldedDateCategory>
    {
        private readonly MonthCalendar parent;
        public BoldedDateCategoryCollection(MonthCalendar parent)
        {
            this.parent = parent;
        }
        public new void Add(BoldedDateCategory category)
        {
            if (!this.CanAddItem(category))
                return;
            base.Add(category);
        }
        public new void AddRange(IEnumerable<BoldedDateCategory> types)
        {
            if (types == null)
                return;
            types.ToList().ForEach(this.Add);
        }
        public new bool Remove(BoldedDateCategory item)
        {
            if (base.Remove(item))
            {
                this.parent.BoldedDatesCollection.RemoveAll(d => d.Category.Equals(item));
                return true;
            }
            return false;
        }
        public new void RemoveAt(int index)
        {
            if (index < 0 || index >= this.Count)
                return;
            this.Remove(this[index]);
        }
        public new int RemoveAll(Predicate<BoldedDateCategory> match)
        {
            if (match == null)
                return 0;
            var matches = this.FindAll(match);
            if (matches.Count != 0)
                matches.ForEach(t => this.Remove(t));
            return matches.Count;
        }
        public new void RemoveRange(int index, int count)
        {
            if (index < 0 || index + count > this.Count)
                return;
            var items = this.GetRange(index, count);
            items.ForEach(t => this.Remove(t));
        }
        public new void Clear()
        {
            this.parent.BoldedDatesCollection.Clear();
            base.Clear();
        }
        public new void Insert(int index, BoldedDateCategory item)
        {
            if (!this.CanAddItem(item))
                return;
            base.Insert(index, item);
        }
        public new void InsertRange(int index, IEnumerable<BoldedDateCategory> items)
        {
            if (items == null)
                return;
            var list = items.ToList();
            if (list.Any(d => !this.CanAddItem(d)))
                return;
            base.InsertRange(index, list);
        }
        private bool CanAddItem(BoldedDateCategory category)
        {
            return !category.IsEmpty && !this.Exists(t => string.Compare(category.Name, t.Name, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}
