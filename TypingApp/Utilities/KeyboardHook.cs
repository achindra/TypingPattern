using System;
using System.Windows.Input;
using System.Runtime.InteropServices;
using TypingApp.Utilities;

namespace TypingApp
{
    class KeyboardHook : IDisposable
    {
        private GlobalHooksHelper.HookDelegate kbHookDelegate;
        private IntPtr kbHookHandle = IntPtr.Zero;
        private const Int32 WH_KEYBOARD_LL = 13;
        private const Int32 WM_KEYDOWN = 0x0100;
        private const Int32 WM_SYSKEYDOWN = 0x0104;
        private bool disposed = false;

        public event EventHandler<KeyPressedArgs> KeyPressEvent;

        private IntPtr KeyboardHookDelegate(Int32 nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (null != KeyPressEvent)
                {
                    KeyPressEvent(this, new KeyPressedArgs(KeyInterop.KeyFromVirtualKey(vkCode)));
                }
            }

            return GlobalHooksHelper.CallNextHookEx(kbHookHandle, nCode, wParam, lParam);
        }

        public KeyboardHook()
        {
            kbHookDelegate = KeyboardHookDelegate;
            kbHookHandle = GlobalHooksHelper.SetWindowsHookEx(WH_KEYBOARD_LL, kbHookDelegate, IntPtr.Zero, 0);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (kbHookHandle != IntPtr.Zero)
                {
                    GlobalHooksHelper.UnhookWindowsHookEx(kbHookHandle);
                }
                disposed = true;
            }
        }

        ~KeyboardHook()
        {
            Dispose(false);
        }
    }
}
