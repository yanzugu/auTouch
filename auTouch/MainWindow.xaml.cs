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
        readonly Dot currentModeDot = new();

        private BackgroundWorker bw;
        private int index = 0;  // 用來命名: dot_{index}
        private Dot currentDot; // 當前選定的 Dot

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Closing += MainWindow_Closing;
            currentModeDot.dp.Name = "Current Cuorsor";
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Inverse_Button_State();
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
            Inverse_Button_State();

            if (RB_Current.IsChecked == true)
            {
                Run_Mode_Current();
            }
            else
            {
                Run_Mode_SelfDefine();
            }

            bw.RunWorkerAsync();
        }

        private void Btn_Stop_Click(object sender, RoutedEventArgs e)
        {
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
            dot.Name = "Dot_" + index++;
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

        private void Dowork_Current_Click(object sender, DoWorkEventArgs e)
        {
            if (currentDot.dp.Count == -1) // Infinite Click
            {
                while (true)
                {
                    if (bw.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    ms.MouseLeftClickEvent();
                    Thread.Sleep(currentDot.dp.Interval);
                }
            }
            else
            {
                for (int i = 0; i < currentDot.dp.Count; i++)
                {
                    if (bw.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    ms.MouseLeftClickEvent();
                    Thread.Sleep(currentDot.dp.Interval);
                }
            }
        }

        private void Dowork_SelfDefine_Click(object sender, DoWorkEventArgs e)
        {
            MessageBox.Show("");
        }

        // 設定個別 Dot 屬性
        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Dot dot = sender as Dot;
            currentDot = dot;
            this.DataContext = currentDot.dp;
        }

        private void Delete_Dot()
        {
            if (currentDot == null) return;
            dots.Remove(currentDot);
            currentDot.Close();
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

        // 對調 Run / Stop 按鈕狀態
        private void Inverse_Button_State()
        {
            Btn_Run.IsEnabled = !Btn_Run.IsEnabled;
            Btn_Stop.IsEnabled = !Btn_Stop.IsEnabled;
        }

        private void Initialize_BackgroundWorker()
        {
            bw = new();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // 程式關閉時關閉所有的 Dots
            foreach (Dot dot in dots)
            {
                dot.Close();
            }
            currentModeDot.Close();
        }

        private void Run_Mode_Current()
        {
            Initialize_BackgroundWorker();
            bw.DoWork += Dowork_Current_Click;
        }

        private void Run_Mode_SelfDefine()
        {
            Initialize_BackgroundWorker();
            bw.DoWork += Dowork_SelfDefine_Click;
        }
        
        private void RB_Current_Checked(object sender, RoutedEventArgs e)
        {
            currentDot = currentModeDot;
            this.DataContext = currentDot.dp;
        }

        private void RB_Current_Unchecked(object sender, RoutedEventArgs e)
        {
            currentDot = null;
            this.DataContext = null;
        }

        private void RB_SelfDefine_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (Dot dot in dots)
            {
                dot.Visibility = Visibility.Collapsed;
            }
        }

        private void RB_SelfDefine_Checked(object sender, RoutedEventArgs e)
        {
            foreach (Dot dot in dots)
            {
                dot.Visibility = Visibility.Visible;
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
