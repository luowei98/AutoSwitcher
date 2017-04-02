using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSwitcher.Input.Api
{
    public enum KeyboardControlKeys
    {
        Undefined = 0,
        [Description("Left Control")]
        LCtrl = 0xA2, //VK_LCONTROL
        [Description("Left Alt")]
        LAlt = 0xA4, //VK_LMENU,
        [Description("Left Win")]
        LWin = 0x5B //VK_LWIN
    }

    public enum KeyboardVirtualKeys
    {
        Undefined = 0,
        [Description("Tab")]
        Tab = 0x09, //VK_TAB
        [Description("~")]
        Console = 0xC0, //~
        [Description("Escape")]
        Escape = 0x1B //VK_ESCAPE
    }
}
