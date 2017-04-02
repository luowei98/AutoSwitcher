﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoSwitcher.Window.Api
{
    public static class WindowEntryFactory
    {
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        public static WindowEntry Create(IntPtr hWnd)
        {
            var windowTitle = GetWindowTitle(hWnd);

            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);

            var iconHandle = WindowIcon.GetAppIcon(hWnd);
            var isVisible = !IsIconic(hWnd);

            return new WindowEntry
            {
                HWnd = hWnd,
                Title = windowTitle,
                ProcessId = processId,
                IconHandle = iconHandle,
                IsVisible = isVisible
            };
        }

        private static string GetWindowTitle(IntPtr hWnd)
        {
            var lLength = GetWindowTextLength(hWnd);
            if (lLength == 0)
                return null;

            var builder = new StringBuilder(lLength);
            GetWindowText(hWnd, builder, lLength + 1);
            return builder.ToString();
        }
    }
}
