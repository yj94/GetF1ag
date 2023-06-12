using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Automation;
/*
 免责声明：
本程序仅供学习和技术研究之用。使用本程序所获取的任何窗口内容或其他信息应遵守适用法律和规定，并获得合法的授权。未经授权或滥用本程序可能涉及侵犯隐私、违反法律或其他相关规定。开发者不承担任何由使用本程序所引起的直接或间接责任。

使用本程序时，请确保遵守以下准则：

尊重他人的隐私和合法权益；
仅用于合法和道德目的；
不违反任何适用的法律、法规或规定；
在使用前事先获得相关授权。
请注意，本免责声明并不能覆盖所有情况，对于使用本程序所引发的任何问题或后果，开发者不承担任何责任。使用本程序的责任完全由使用者自行承担。
在使用本程序之前，请务必审慎考虑，并确保你的行为合法、合规且符合道德规范。
请根据你的具体情况对免责声明进行修改和适应，以确保其与你的应用和使用场景相符合，并符合适用的法律和规定。
*/
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
            int screenWidth = Console.WindowWidth;

            // 计算居中的位置
            int leftMargin = (screenWidth / 2);

            // 设置光标位置并输出内容
            Console.SetCursorPosition(leftMargin, Console.CursorTop);
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
            Console.WriteLine("正在实时获取窗口句柄和内容...");
            Console.WriteLine("请将鼠标移动到指定的窗口上。");
            Console.WriteLine("获取的内容存储在GetF1ag.txt中。");
            Console.WriteLine("按下 Ctrl + C 结束程序。");
            Console.Out.Flush();
            try
            {
                while (true)
                {
                    Point mousePos;
                    GetCursorPos(out mousePos);

                    IntPtr hWnd = WindowFromPoint(mousePos);
                    string windowText = GetWindowText(hWnd);
                    string windowMoreText = GetControlText(hWnd);
                    windowMoreText = windowMoreText.Replace(Environment.NewLine, " ");
                    using (var writer = new StreamWriter("GetF1ag.txt", true))
                    {
                        if (previousHWnd != (int)hWnd)
                        {
                            //输出到控制台
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine(DateTime.Now.ToString() + "->窗口句柄: 0x" + hWnd.ToString("X"));
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine(DateTime.Now.ToString() + "->窗口内容: " + windowText);
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.WriteLine(DateTime.Now.ToString() + "->遍历内容: " + windowMoreText);
                            //输出到文件
                            writer.WriteLine(DateTime.Now.ToString() + "->窗口句柄: 0x" + hWnd.ToString("X"));
                            writer.WriteLine(DateTime.Now.ToString() + "->窗口内容: " + windowText);
                            writer.WriteLine(DateTime.Now.ToString() + "->遍历内容: " + windowMoreText);
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
                Console.WriteLine("发生异常,请联系作者: " + ex.Message);
            }
        }
        private static string GetControlText(IntPtr hWnd)
        {
            AutomationElement element = AutomationElement.FromHandle(hWnd);

            string controlText = string.Empty;
            try
            {
                GetControlTextRecursive(element, ref controlText);
            }
            catch (Exception ex)
            {
                controlText = $"遍历失败： {ex.Message}";
            }

            return controlText;
        }

        private static void GetControlTextRecursive(AutomationElement element, ref string controlText)
        {
            if (element == null)
                return;

            try
            {
                if (element.Current.ControlType == ControlType.Edit || element.Current.ControlType == ControlType.Document)
                {
                    ValuePattern valuePattern = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                    if (valuePattern != null)
                    {
                        string text = valuePattern.Current.Value;
                        controlText += text + Environment.NewLine;
                    }
                    else
                    {
                        controlText += "不支持的模式：" + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                controlText += "获取内容时发生异常：" + ex.Message + Environment.NewLine;
            }

            // 递归遍历子控件
            AutomationElementCollection children = element.FindAll(TreeScope.Children, Condition.TrueCondition);
            foreach (AutomationElement child in children)
            {
                GetControlTextRecursive(child, ref controlText);
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
