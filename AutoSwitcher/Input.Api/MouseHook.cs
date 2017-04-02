using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutoSwitcher.Input.Api
{
    public class MouseHook : IDisposable
    {
        private readonly Action callback;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local
        private const int HC_ACTION = 0;
        private const int WH_MOUSE_LL = 14;
        private static IntPtr _hookID = IntPtr.Zero;
        // ReSharper restore UnusedMember.Local
        // ReSharper restore InconsistentNaming


        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static MouseHook Hook(Action callback)
        {
            return new MouseHook(callback);
        }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private static LowLevelMouseProc proc;

        private MouseHook(Action callback)
        {
            this.callback = callback;

            // ReSharper disable once RedundantDelegateCreation
            proc = new LowLevelMouseProc(HookCallback);

            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                _hookID = SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode != HC_ACTION)
                return CallNextHookEx(_hookID, nCode, wParam, lParam);

            callback();

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}