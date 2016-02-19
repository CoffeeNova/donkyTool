using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Me.Catx.Native
{
    /// <summary>
    /// Find a window.
    /// <para>Usage:</para>
    /// <para>hanlde = (new FindWindow(hwndParent, className)).FoundHandle;</para>
    /// 
    /// <para>Set hwndParent = IntPtr.Zero to use Desktop as the parent. </para>
    /// </summary>
    public class FindWindow
    {
        private string m_szClassName; // class name to look for
        private string m_szWndName;   // window name to look for

        private IntPtr m_hWnd; // HWND if found
        public IntPtr FoundHandle
        {
            get { return m_hWnd; }
        }

        public FindWindow(IntPtr hwndParent, string className)
        {
            m_hWnd = IntPtr.Zero;
            m_szClassName = className;
            m_szWndName = string.Empty;
            FindChildClassHwnd(hwndParent, IntPtr.Zero);
        }

        public FindWindow(IntPtr hwndParent, string className, string wndName)
        {
            m_hWnd = IntPtr.Zero;
            m_szClassName = (className == "") ? null : className;
            m_szWndName = (wndName == "") ? null : wndName;
            FindChildClassHwnd(hwndParent, IntPtr.Zero);
        }

        /**/
        /// <summary>
        /// Find the child window, if found m_classname will be assigned 
        /// </summary>
        /// <param name="hwndParent">parent's handle</param>
        /// <param name="lParam">the application value, nonuse</param>
        /// <returns>found or not found</returns>
        //The C++ code is that  lParam is the instance of FindWindow class , if found assign the instance's m_hWnd
        private bool FindChildClassHwnd(IntPtr hwndParent, IntPtr lParam)
        {
            WinAPI.EnumWindowProc childProc = new WinAPI.EnumWindowProc(FindChildClassHwnd);
            IntPtr hwnd = WinAPI.FindWindowEx(hwndParent, IntPtr.Zero, this.m_szClassName, this.m_szWndName);
            if (hwnd != IntPtr.Zero)
            {
                this.m_hWnd = hwnd; // found: save it
                return false; // stop enumerating
            }
            WinAPI.EnumChildWindows(hwndParent, childProc, IntPtr.Zero); // recurse  redo FindChildClassHwnd
            return true;// keep looking
        }
    }
}
