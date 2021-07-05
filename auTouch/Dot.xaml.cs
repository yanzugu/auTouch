using System.Windows;
using System.Windows.Input;

namespace auTouch
{
    public partial class Dot : Window
    {
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
    }
}
