using System;

namespace AutoSwitcher.Window.Api
{
    public interface IWindowEntry
    {
        IntPtr HWnd { get; set; }
        uint ProcessId { get; set; }
        string ProcessName { get; set; }
        string Title { get; set; }
        IntPtr IconHandle { get; set; }
        bool IsVisible { get; set; }

        bool Focus();
        bool IsForeground();
        bool IsSameWindow(IWindowEntry other);
    }
}
