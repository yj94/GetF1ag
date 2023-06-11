using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GetF1ag
{
    class Program
    {

        // 声明设置控制台窗口属性的函数
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        // 定义标准输入句柄和控制台模式常量
        private const int STD_INPUT_HANDLE = -10;
        private const uint ENABLE_QUICK_EDIT_MODE = 0x0040;

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



        static void Main(string[] args)
        {
            int previousHWnd = 0;

            // 获取标准输入句柄
            IntPtr stdInHandle = GetStdHandle(STD_INPUT_HANDLE);

            // 获取当前控制台模式
            GetConsoleMode(stdInHandle, out uint consoleMode);

            // 禁用快速编辑模式
            consoleMode &= ~ENABLE_QUICK_EDIT_MODE;

            // 设置新的控制台模式
            SetConsoleMode(stdInHandle, consoleMode);

            // 开启快速编辑模式
            //consoleMode |= ENABLE_QUICK_EDIT_MODE;

            // 设置新的控制台模式
            //SetConsoleMode(stdInHandle, consoleMode);

            Console.CursorVisible = false; // 禁用控制台窗口的光标
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
            Console.WriteLine("                         正在实时获取窗口句柄和内容...");
            Console.WriteLine("                         请将鼠标移动到指定的窗口上。");
            Console.WriteLine("                         获取的内容存储在GetF1ag.txt中。");
            Console.WriteLine("                         按下 Ctrl + C 结束程序。");
            Console.Out.Flush();
            try
            {
                while (true)
                {
                    Point mousePos;
                    GetCursorPos(out mousePos);

                    IntPtr hWnd = WindowFromPoint(mousePos);
                    string windowText = GetWindowText(hWnd);
                    using (var writer = new StreamWriter("GetF1ag.txt", true))
                    {
                        if (previousHWnd != (int)hWnd)
                        {
                            //输出到控制台
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("                         窗口句柄: 0x" + hWnd.ToString("X"));
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("                         窗口内容: " + windowText);
                            //输出到文件
                            writer.WriteLine("窗口句柄: 0x" + hWnd.ToString("X"));
                            writer.WriteLine("窗口内容: " + windowText);
                        }
                        previousHWnd = (int)hWnd;
                        //Console.WriteLine("之前：" + previousHWnd);
                        //Console.WriteLine("现在：" + (int)hWnd);
                        Console.Out.Flush();
                    }
                    System.Threading.Thread.Sleep(300); // 每隔 300 毫秒更新一次
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("                         发生异常,请联系作者: " + ex.Message);
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
