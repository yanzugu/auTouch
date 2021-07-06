using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace auTouch
{
    public partial class Dot : Window
    {
        public DotPorperty dp;

        public Dot()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            dp = new();
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
