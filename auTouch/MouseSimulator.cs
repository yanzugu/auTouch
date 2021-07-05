using System;
using System.Windows;
using System.Runtime.InteropServices;

namespace auTouch
{
    class MouseSimulator
    {
        [Flags]
        private enum MouseEventFlag : uint //設定滑鼠動作的鍵值
        {
            Move = 0x0001,               //發生移動
            LeftDown = 0x0002,           //滑鼠按下左鍵
            LeftUp = 0x0004,             //滑鼠鬆開左鍵
            RightDown = 0x0008,          //滑鼠按下右鍵
            RightUp = 0x0010,            //滑鼠鬆開右鍵
            MiddleDown = 0x0020,         //滑鼠按下中鍵
            MiddleUp = 0x0040,           //滑鼠鬆開中鍵
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,              //滑鼠輪被移動
            VirtualDesk = 0x4000,        //虛擬桌面
            Absolute = 0x8000
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);

        public void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public void SetCursorPosition(double x, double y)
        {
            SetCursorPos((int)x, (int)y);
        }

        public void SetCursorPosition(Point point)
        {
            SetCursorPos((int)point.X, (int)point.Y);
        }

        public void MouseLeftClickEvent(int dx, int dy)
        {
            SetCursorPosition(dx, dy);
            mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, dx, dy, 0, UIntPtr.Zero);
        }

        public void MouseLeftClickEvent()
        {
            mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        public void MouseRightClickEvent(int dx, int dy)
        {
            SetCursorPosition(dx, dy);
            mouse_event(MouseEventFlag.RightDown | MouseEventFlag.RightUp, dx, dy, 0, UIntPtr.Zero);
        }

        public void MouseRightClickEvent()
        {
            mouse_event(MouseEventFlag.RightDown | MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
        }
    }
}
