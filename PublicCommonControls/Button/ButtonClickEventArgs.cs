using System.ComponentModel;

namespace PublicCommonControls.WButton
{
    public class ButtonClickEventArgs : CancelEventArgs
    {
        public ButtonClickEventArgs(ButtonClickEventArgs other)
            : this()
        {
            this.Pushed = other.Pushed;
            this.Text = other.Text;
            this.ButtonPressed = other.ButtonPressed;
            this.Cancel = other.Cancel;
        }
        public ButtonClickEventArgs(string text, bool value, EnButtonPressed btn = EnButtonPressed.Whole)
            : this()
        {
            this.Pushed = value;
            this.Text = text;
            this.ButtonPressed = btn;
            this.Cancel = false;
        }
        protected ButtonClickEventArgs()
        {

        }
        public EnButtonPressed ButtonPressed { get; protected set; }
        public bool Pushed { get; protected set; }
        public string Text { get; protected set; }
    }
}
