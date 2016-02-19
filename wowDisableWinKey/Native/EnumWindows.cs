using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Me.Catx.Native
{
    /// <summary>
    /// Get hWnd of all top level windows.
    /// <para>Usage:</para>
    /// <para>EnumWindows ew = new EnumWindows();</para>
    /// <para>ew.Enum();</para>
    /// <para>listOfhWnds = ew.hWnds;</para>
    /// </summary>
    public class EnumWindows
    {
        List<IntPtr> m_hWnds;

        /// <summary>
        /// hWnds of top level windows
        /// </summary>
        public List<IntPtr> hWnds
        {
            get { return m_hWnds; }
        }

        public EnumWindows()
        {
            m_hWnds = new List<IntPtr>();
        }

        bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam)
        {
            m_hWnds.Add(hWnd);
            return true;
        }

        /// <summary>
        /// Call EnumWindow
        /// </summary>
        public void Enum()
        {
            WinAPI.EnumWindows(new WinAPI.EnumWindowsProc(EnumWindowsProc), IntPtr.Zero);
        }
    }
}
