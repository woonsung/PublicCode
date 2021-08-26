using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace PublicCommonControls.WCalendar.Design
{
    [ToolboxItem(false)]
    internal class FlagCheckedListBox : CheckedListBox
    {
        private bool isUpdatingCheckStates;
        private Type enumType;
        private Enum enumValue;
        public FlagCheckedListBox()
        {
            this.CheckOnClick = true;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Enum EnumValue
        {
            get
            {
                object e = Enum.ToObject(this.enumType, this.GetCurrentValue());
                return (Enum)e;
            }
            set
            {
                Items.Clear();
                this.enumValue = value;
                this.enumType = value.GetType();
                this.FillEnumMembers();
                this.ApplyEnumValue();
            }
        }
        protected override void OnItemCheck(ItemCheckEventArgs e)
        {
            base.OnItemCheck(e);
            if (this.isUpdatingCheckStates)
                return;
            FlagCheckedListBoxItem item = Items[e.Index] as FlagCheckedListBoxItem;
            this.UpdateCheckedItems(item, e.NewValue);
        }
        private void Add(int v, string c)
        {
            FlagCheckedListBoxItem item = new FlagCheckedListBoxItem(v, c);
            Items.Add(item);
        }
        private int GetCurrentValue()
        {
            return (from object t in this.Items select t as FlagCheckedListBoxItem)
                .Where((item, i) => this.GetItemChecked(i) && item != null)
                .Aggregate(0, (current, item) => current | item.Value);
        }
        private void UpdateCheckedItems(int value)
        {
            this.isUpdatingCheckStates = true;
            for(int i = 0; i < Items.Count; i++)
            {
                FlagCheckedListBoxItem item = Items[i] as FlagCheckedListBoxItem;
                if (item == null)
                    continue;
                if (item.Value == 0)
                    SetItemChecked(i, value == 0);
                else
                {
                    SetItemChecked(i, (item.Value & value) == item.Value && item.Value != 0);
                }
            }
            this.isUpdatingCheckStates = false;
        }
        private void UpdateCheckedItems(FlagCheckedListBoxItem composite, CheckState cs)
        {
            if (composite.Value == 0)
                this.UpdateCheckedItems(0);
            int sum = (from object t in this.Items select t as FlagCheckedListBoxItem)
                .Where((Items, i) => Items != null && this.GetItemChecked(i))
                .Aggregate(0, (current, item) => current | item.Value);
            if (cs == CheckState.Unchecked)
                sum = sum & (~composite.Value);
            else
                sum |= composite.Value;
            this.UpdateCheckedItems(sum);
        }
        private void FillEnumMembers()
        {
            foreach(string name in Enum.GetNames(this.enumType))
            {
                object val = Enum.Parse(this.enumType, name);
                int intVal = (int)Convert.ChangeType(val, typeof(int));
                this.Add(intVal, name);
            }
            this.Height = this.GetItemHeight(0) * 8;
        }
        private void ApplyEnumValue()
        {
            int intval = (int)Convert.ChangeType(this.enumType, typeof(int));
            this.UpdateCheckedItems(intval);
        }


        private class FlagCheckedListBoxItem
        {
            public FlagCheckedListBoxItem(int value, string caption)
            {
                this.Value = value;
                this.Caption = caption;
            }
            public int Value { get; set; }
            public string Caption { get; set; }

            public override string ToString()
            {
                return base.ToString();
            }
        }

    }
}
