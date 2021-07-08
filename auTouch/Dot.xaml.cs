using System.Windows;
using System.Windows.Input;

namespace auTouch
{
    public partial class Dot : Window
    {
        public readonly DotPorperty dp = new();
        MouseSimulator ms = new();

        public Dot()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ((Window)sender).DragMove();             
            }
        }

        public void Click_Event()
        {
            switch (dp.EventType)
            {
                case ClickEventType.Left:
                    ms.MouseLeftClickEvent();
                    break;
                case ClickEventType.Right:
                    ms.MouseRightClickEvent();
                    break;
            }
        }
    }

    public enum ClickEventType
    {
        Left,
        Right,
        Middle
    }

}
