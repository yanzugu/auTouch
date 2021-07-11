using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace auTouch
{
    public partial class Dot : Window
    {
        public readonly DotPorperty dp = new();
        MouseSimulator ms = new();
        Point point;
        DateTime dt1 = new();
        DateTime dt2;
        TimeSpan ts;

        public Dot()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.MouseUp += Dot_MouseUp;
            this.Loaded += Dot_Loaded;
        }

        private void Dot_Loaded(object sender, RoutedEventArgs e)
        {
            point = Get_Point();
            Index.Text = dp.Name.Split('_').Length == 2 ? dp.Name.Split('_')[1] : "";
        }

        // 取得當前位置
        private Point Get_Point()
        {
            var p = this.PointToScreen(new Point(0, 0));
            p.X += this.Width / 2 + 4;
            p.Y += this.Height / 2 + 4;
            return p;
        }

        private void Dot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                point = Get_Point();
            }
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

        public void Run(BackgroundWorker bw, DoWorkEventArgs e)
        {
            if (dp.Count == 0)
            {
                dt2 = DateTime.Now;
                ts = dt2.Subtract(dt1);
                if (ts.TotalMilliseconds > dp.Interval)
                {
                    dt1 = DateTime.Now;
                    ms.SetCursorPosition(point);
                    Click_Event();
                }

            }
            else
            {
                dt2 = DateTime.Now;
                ts = dt2.Subtract(dt1);
                if (ts.TotalMilliseconds > dp.Interval)
                {
                    dt1 = DateTime.Now;
                    ms.SetCursorPosition(point);
                    Click_Event();
                }
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
