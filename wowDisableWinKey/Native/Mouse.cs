using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Me.Catx.Native
{
    /// <summary>
    /// Simulate mouse events
    /// </summary>
    public class Mouse
    {
        public static void MoveTo(int x, int y)
        {
            WinAPI.SetCursorPos(x, y);
        }

        public static void LeftClick(int x, int y)
        {
            WinAPI.SetCursorPos(x, y);
            WinAPI.mouse_event(MouseEventFlag.LEFTDOWN, x, y, 0, 0);
            WinAPI.mouse_event(MouseEventFlag.LEFTUP, x, y, 0, 0);
        }
    }
}
