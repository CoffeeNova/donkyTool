using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Me.Catx.Native
{
    /**/
    /// <summary>
    /// Get the given window's information.
    /// <para>Useage: </para>
    /// <para>WindowInfo wi = new WindowInfo(wndHandle); </para>
    /// <para>Rectangle rect = wi.WindowRect;</para>
    /// <para>String title = wi.WindowText</para>
    /// </summary>
    /// TODO: Add more window info
    public class WindowInfo
    {
        private IntPtr m_hWnd;

        private Rectangle m_wndRect;
        /// <summary>
        /// Window Rect
        /// </summary>
        public Rectangle WindowRect
        {
            get { return m_wndRect; }
        }

        private String m_wndText;
        /// <summary>
        /// Window Text
        /// </summary>
        public String WindowText
        {
            get { return m_wndText; }
        }

        private string m_clsName;
        /// <summary>
        /// Class Name
        /// </summary>
        public String ClassName
        {
            get { return m_clsName; }
        }

        public WindowInfo(IntPtr wndHandle)
        {
            m_hWnd = wndHandle;
            GetWndRect();
            GetWndText();
            GetClsName();
        }

        private void GetWndRect()
        {
            WinAPI.RECT rct = new WinAPI.RECT();
            WinAPI.GetWindowRect(m_hWnd, ref rct);

            m_wndRect = new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top);

        }

        private void GetWndText()
        {
            int len = WinAPI.GetWindowTextLength(m_hWnd);
            StringBuilder sb = new StringBuilder(len + 1);
            WinAPI.GetWindowText(m_hWnd, sb, sb.Capacity);

            m_wndText = sb.ToString();
        }

        private void GetClsName()
        {
            StringBuilder sb = new StringBuilder(255);
            WinAPI.GetClassName(m_hWnd, sb, sb.Capacity);

            m_clsName = sb.ToString();
        }

        /// <summary>
        /// Set the window penetrable.
        /// </summary>
        public void SetPenetrable(int alpha)
        {
            uint intExTemp = WinAPI.GetWindowLong(m_hWnd, WinAPI.GWL_EXSTYLE);
            uint oldGWLEx = WinAPI.SetWindowLong(m_hWnd, WinAPI.GWL_EXSTYLE, WinAPI.WS_EX_TRANSPARENT | WinAPI.WS_EX_LAYERED);

            WinAPI.SetLayeredWindowAttributes(m_hWnd, 0, alpha, WinAPI.LWA_ALPHA);
        }
    }
}
