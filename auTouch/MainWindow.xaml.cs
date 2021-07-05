using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace auTouch
{
    public partial class MainWindow : Window
    {
        MouseSimulator ms = new MouseSimulator();
        List<Window> dots = new List<Window>();
        List<DotPorperty> dotPorperties = new List<DotPorperty>();
        private int index = 0;
        DotPorperty dp = new DotPorperty();

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            foreach(var dot in dots)
            {
                dot.Close();
            }
        }

        private void BtnClick(object sender, RoutedEventArgs e)
        {       
            //var p = window.PointToScreen(new Point(0, 0));
            //p.X += window.Width / 2; 
            //p.Y += window.Height / 2;

            //ms.SetCursorPosition(p);
        }

        private void Dot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window window = sender as Window;
            dp = dotPorperties.Where( i => (i.Name == window.Name)).Select(i => i).FirstOrDefault();
            this.DataContext = dp;
        }

        private void Delete_Dot(Window dot)
        {
            dots.Remove(dot);
            Thread.Sleep(100);
            dot.Close();
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ((Window)sender).DragMove();
            }
        }

        private void Delete_Dot(object sender, RoutedEventArgs e)
        {
            Window dot = dots.Where(i => (i.Name == dp.Name)).Select(i => i).FirstOrDefault();
            if (dot == null) return;
            dotPorperties.Remove(dp);
            dots.Remove(dot);
            dot.Close();
            this.DataContext = null;
        }

        private void Btn_Create_Click(object sender, RoutedEventArgs e)
        {
            Dot dot = new Dot();
            DotPorperty dotPorperty = new DotPorperty();
            dot.Topmost = true;
            dot.Name = "dot_" + index++;
            dot.MouseRightButtonDown += Dot_MouseRightButtonDown;
            dotPorperty.Name = dot.Name;
            dotPorperties.Add(dotPorperty);
            dots.Add(dot);
            dot.Show();
        }
    }  
}
