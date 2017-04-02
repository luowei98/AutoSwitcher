using System.Collections.Generic;

namespace AutoSwitcher.Window.Api
{
    public class WindowsList
    {
        public WindowsList(IList<IWindowEntry> windows)
        {
            Windows = windows;
        }

        public IList<IWindowEntry> Windows { get; private set; }
    }
}
