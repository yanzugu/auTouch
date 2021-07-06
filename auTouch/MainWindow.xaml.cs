using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Diagnostics;

namespace auTouch
{
    public partial class MainWindow : Window
    {
        readonly MouseSimulator ms = new();
        readonly List<Dot> dots = new();

        private BackgroundWorker bw;
        private int index = 0;  // 用來命名: dot_{index}
        private Dot target; // 選定的 Dot

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Closing += MainWindow_Closing;
            this.KeyDown += MainWindow_KeyDown;
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            Delete_Dot();
        }

        private void Btn_Create_Click(object sender, RoutedEventArgs e)
        {
            Create_Dot();
        }

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear_Dots();
        }

        private void Btn_Run_Click(object sender, RoutedEventArgs e)
        {
            bw = new();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;

            if (RB_Current.IsChecked == true)
            {
                int count = 0, interval = 0;
                if (!int.TryParse(TB_Count.Text, out count) || !int.TryParse(TB_Interval.Text, out interval))
                    return;

                Btn_Stop.IsEnabled = true;
                Btn_Run.IsEnabled = false;

                bw.DoWork += ((sender, e) =>
                {
                    if (count < 0)
                    {
                        while (true)
                        {
                            ms.MouseLeftClickEvent();
                            Thread.Sleep(interval);
                            if (bw.CancellationPending == true)
                            {
                                e.Cancel = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            ms.MouseLeftClickEvent();
                            Thread.Sleep(interval);
                            if (bw.CancellationPending == true)
                            {
                                e.Cancel = true;
                                break;
                            }
                        }
                    }
                });
                bw.RunWorkerAsync();
            }
            else
            {
            }
        }

        private void Btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            Btn_Stop.IsEnabled = false;
            Btn_Run.IsEnabled = true;
            if (bw != null && bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
            }
        }

        private void Check_Number(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9 ^-]+");
            e.Handled = (regex.IsMatch(e.Text) || String.IsNullOrWhiteSpace(e.Text));
        }

        private void Create_Dot()
        {
            Dot dot = new();
            dot.Topmost = true;
            dot.Name = "dot_" + index++;
            dot.MouseLeftButtonDown += Dot_MouseLeftButtonDown;
            dot.dp.Name = dot.Name;
            dots.Add(dot);
            dot.Show();
        }

        private void Clear_Dots()
        {
            foreach (var dot in dots)
            {
                dot.Close();
            }
            dots.Clear();
            this.DataContext = null;
        }

        // 設定個別 Dot 屬性
        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Dot dot = sender as Dot;
            target = dot;
            this.DataContext = target.dp;
        }

        private void Delete_Dot()
        {
            if (target == null) return;
            dots.Remove(target);
            target.Close();
            this.DataContext = null;
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ((Window)sender).DragMove();
            }
        }

        private Point Get_Dot_Point(Dot dot)
        {
            Point point = dot.PointToScreen(new Point(0, 0));
            point.X += dot.Width / 2 + 3;
            point.Y += dot.Height / 2 + 3;

            return point;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.F5))
            {
                Btn_Run.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // 程式關閉時關閉所有的 Dots
            foreach (Dot dot in dots)
            {
                dot.Close();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 移除字串中的空白
            TextBox textBox = sender as TextBox;
            if (textBox.Text != textBox.Text.Replace(" ", ""))
            {
                textBox.Text = textBox.Text.Replace(" ", "");
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
    }
}
