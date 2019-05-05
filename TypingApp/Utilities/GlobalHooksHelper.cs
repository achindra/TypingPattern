using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TypingApp.Utilities
{
    class GlobalHooksHelper
    {
        public delegate IntPtr HookDelegate(Int32 Code, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, Int32 nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("User32.dll")]
        public static extern IntPtr SetWindowsHookEx(Int32 idHook, HookDelegate lpfn, IntPtr hMod, Int32 dwThreadId);
    }
}
