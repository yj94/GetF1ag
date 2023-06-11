using System;
using System.Runtime.InteropServices;

namespace GetF1ag
{
    class Program
    {
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point point);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, IntPtr lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }

        private static IntPtr previousHWnd = IntPtr.Zero;

        static void Main(string[] args)
        {
            Console.Title = "GetF1ag Tool By YJ";
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(@"
                       ______     __  _________              __  __    __
                      / ____/__  / /_/ ____<  /___ _____ _   \ \/ /   / /
                     / / __/ _ \/ __/ /_   / / __ `/ __ `/____\  /_  / / 
                    / /_/ /  __/ /_/ __/  / / /_/ / /_/ /_____/ / /_/ /  
                    \____/\___/\__/_/    /_/\__,_/\__, /     /_/\____/   
                                                 /____/                  
            ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("正在实时获取窗口句柄和内容...");
            Console.WriteLine("请将鼠标移动到指定的窗口上。");
            Console.WriteLine("按下 Ctrl + C 结束程序。");

            try
            {
                while (true)
                {
                    Point mousePos;
                    GetCursorPos(out mousePos);

                    IntPtr hWnd = WindowFromPoint(mousePos);
                    if (hWnd != previousHWnd)
                    {
                        string windowText = GetWindowText(hWnd);
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("窗口句柄: 0x" + hWnd.ToString("X"));
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("窗口内容: " + windowText);

                        previousHWnd = hWnd;
                    }

                    System.Threading.Thread.Sleep(500); // 每隔 500 毫秒更新一次
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生异常,请联系作者: " + ex.Message);
            }
        }

        private static string GetWindowText(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            IntPtr buffer = Marshal.AllocCoTaskMem((length + 1) * 2);
            GetWindowText(hWnd, buffer, length + 1);
            string windowText = Marshal.PtrToStringUni(buffer);
            Marshal.FreeCoTaskMem(buffer);
            return windowText;
        }

        private static int GetWindowTextLength(IntPtr hWnd)
        {
            return GetWindowTextLengthW(hWnd);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLengthW(IntPtr hWnd);
    }
}
