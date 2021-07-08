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
using System.Windows.Interop;

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
        private bool isRunning = false;
        private Key hotkey = Key.F10;

        // 回傳快捷鍵的 VK code
        private uint VK_Hotkey
        {
            get
            {
                return (uint)KeyInterop.VirtualKeyFromKey(hotkey);
            }
        }
        private string hotkeyName
        {
            get
            {
                return hotkey.ToString();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Closing += MainWindow_Closing;
            currentModeDot.dp.Name = "Current Cuorsor";
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isRunning = false;
            Inverse_Button_State();
            Show_Dots();
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            Delete_Dot();
        }

        private void Btn_Create_Click(object sender, RoutedEventArgs e)
        {
            currentDot = Create_Dot();
            this.DataContext = currentDot.dp;
        }

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear_Dots();
        }

        private void Btn_Hotkey_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();
            window.Width = 240;
            window.Height = 150;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ResizeMode = ResizeMode.NoResize;
            TextBlock textBlock = new TextBlock()
            {
                Text = hotkeyName,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 40
            };
            window.Content = textBlock;
            window.KeyDown += Set_Hotkey_Window_KeyDown;
            window.Show();
        }

        // 設定快捷鍵
        private void Set_Hotkey_Window_KeyDown(object sender, KeyEventArgs e)
        {
            hotkey = e.Key;
            Window window = sender as Window;
            TextBlock textBlock = window.Content as TextBlock;
            textBlock.Text = hotkeyName;
            UnregisterHotKey();
            RegisterHotKey();
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
                if (dots.Count == 0) return;
                Hide_Dots();
                Run_Mode_SelfDefine();
            }

            bw.RunWorkerAsync();
            isRunning = true;
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
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private Dot Create_Dot()
        {
            Dot dot = new();
            dot.Topmost = true;
            dot.Name = "Dot_" + index++;
            dot.MouseLeftButtonDown += Dot_MouseLeftButtonDown;
            dot.dp.Name = dot.Name;
            dots.Add(dot);
            dot.Show();
            return dot;
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

        // 當前位置邏輯
        private void Dowork_Current(object sender, DoWorkEventArgs e)
        {
            if (currentDot.dp.Count == 0) // Infinite Click
            {
                while (true)
                {
                    if (bw.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    currentDot.Click_Event();
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
                    currentDot.Click_Event();
                    Thread.Sleep(currentDot.dp.Interval);
                }
            }
        }

        // 自定義邏輯
        private void Dowork_SelfDefine(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                foreach (Dot dot in dots)
                {
                    dot.Run(bw, e);
                    Thread.Sleep(5);
                }
                if (bw.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                Thread.Sleep(10);
            }
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

        private void Hide_Dots()
        {
            foreach (Dot dot in dots)
            {
                dot.Visibility = Visibility.Collapsed;
            }
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
            bw.DoWork += Dowork_Current;
        }

        private void Run_Mode_SelfDefine()
        {
            Initialize_BackgroundWorker();
            bw.DoWork += Dowork_SelfDefine;
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

        private void Show_Dots()
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

            // 避免字串為空
            if (textBox.Text == "")
            {
                textBox.Text = "0";
                textBox.CaretIndex = textBox.Text.Length;
            }
        }


        // 背景讀取快捷鍵
        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        private const int HOTKEY_ID = 9000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            //const uint MOD_CTRL = 0x0002;
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, 0, VK_Hotkey))
            {
                // handle error
            }
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            if (isRunning)
            {
                Btn_Stop.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                Btn_Run.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
    }
}
