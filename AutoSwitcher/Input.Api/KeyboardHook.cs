using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutoSwitcher.Input.Api
{
    public class KeyboardHook : IDisposable
    {
        private readonly Action callback;

        [StructLayout(LayoutKind.Sequential)]
        public struct Kbdllhookstruct
        {
            public int VkCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public IntPtr Extra;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod,
            uint dwThreadId);

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
        private const int WH_KEYBOARD_LL = 13;
        private static IntPtr _hookID = IntPtr.Zero;
        // ReSharper restore UnusedMember.Local
        // ReSharper restore InconsistentNaming


        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static KeyboardHook Hook(Action callback)
        {
            return new KeyboardHook(callback);
        }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly LowLevelKeyboardProc proc;

        private KeyboardHook(Action callback)
        {
            this.callback = callback;

            // ReSharper disable once RedundantDelegateCreation
            proc = new LowLevelKeyboardProc(HookCallback);

            using (var curProcess = Process.GetCurrentProcess())
            {
                using (var curModule = curProcess.MainModule)
                {
                    _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
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

            var keyInfo = (Kbdllhookstruct)Marshal.PtrToStructure(lParam, typeof(Kbdllhookstruct));
            if (keyInfo.VkCode == 0)
                return CallNextHookEx(_hookID, nCode, wParam, lParam);

            callback();
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}