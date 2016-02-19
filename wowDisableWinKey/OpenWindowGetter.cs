using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace wowDisableWinKey
{
    /// <summary>Contains functionality to get all the open windows.</summary>
    /// http://www.tcx.be/blog/2006/list-open-windows/
    public static class OpenWindowGetter
    {
        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<IntPtr, string> GetAltTabWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate(IntPtr hWnd, int lParam)
            {
                //if (hWnd == shellWindow) return true;
                //if (!IsWindowVisible(hWnd)) return true;

                if (!KeepWindowHandleInAltTabList(hWnd))
                    return true;
                int length = GetWindowTextLength(hWnd);
                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<IntPtr> GetAltTabWindowsHandles()
        {
            IntPtr shellWindow = GetShellWindow();
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate(IntPtr hWnd, int lParam)
            {
                //if (hWnd == shellWindow) return true;
                //if (!IsWindowVisible(hWnd)) return true;

                if (!KeepWindowHandleInAltTabList(hWnd))
                    return true;
                windows.Add(hWnd);
                return true;

            }, 0);

            return windows;
        }
        /// <summary>
        /// Returns a pair that contains the handle and title of the window
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <returns>A pair that contains the handle and title of the window</returns>
        public static KeyValuePair<IntPtr, string> GetWindow(IntPtr hWnd)
        {
            KeyValuePair<IntPtr, string> window = new KeyValuePair<IntPtr, string>(IntPtr.Zero, "");

            if (!KeepWindowHandleInAltTabList(hWnd))
                return window;
            int length = GetWindowTextLength(hWnd);
            StringBuilder builder = new StringBuilder(length);
            GetWindowText(hWnd, builder, length + 1);
         
            return new KeyValuePair<IntPtr, string>(hWnd, builder.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static bool KeepWindowHandleInAltTabList(IntPtr window)
        {
            if (window == GetShellWindow()|| GetWindowTextLength(window)==0)   //Desktop or without title
                return false;

            //uint processId = 0;
            //Interop.GetWindowThreadProcessId(window, out processId);
            //System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById((int)processId);
            //if (process.MainModule.FileName.EndsWith("ShellExperienceHost.exe"))
            //    return false;

            //http://stackoverflow.com/questions/210504/enumerate-windows-like-alt-tab-does
            //http://blogs.msdn.com/oldnewthing/archive/2007/10/08/5351207.aspx
            //1. For each visible window, walk up its owner chain until you find the root owner. 
            //2. Then walk back down the visible last active popup chain until you find a visible window.
            //3. If you're back to where you're started, (look for exceptions) then put the window in the Alt+Tab list.
            IntPtr root = GetAncestor(window, GaFlags.GA_ROOTOWNER);

            if (GetLastVisibleActivePopUpOfWindow(root) == window)
            {
                Me.Catx.Native.WindowInfo wi = new Me.Catx.Native.WindowInfo(window);
               
                if (wi.ClassName == "Shell_TrayWnd" ||                          //Windows taskbar
                    wi.ClassName == "DV2ControlHost" ||                         //Windows startmenu, if open
                    (wi.ClassName == "Button" && wi.WindowText == "Start") ||   //Windows startmenu-button.
                    wi.ClassName == "MsgrIMEWindowClass" ||                     //Live messenger's notifybox i think
                    wi.ClassName == "SysShadow" ||                              //Live messenger's shadow-hack
                    wi.ClassName.StartsWith("WMP9MediaBarFlyout"))             //WMP's "now playing" taskbar-toolbar
                    return false;

                return true;
            }
            return false;
        }
        /// <summary>
        /// http://stackoverflow.com/questions/210504/enumerate-windows-like-alt-tab-does
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        private static IntPtr GetLastVisibleActivePopUpOfWindow(IntPtr window)
        {

            IntPtr lastPopUp = GetLastActivePopup(window);
            if (IsWindowVisible(lastPopUp))
                return lastPopUp;
            else if (lastPopUp == window)
                return IntPtr.Zero;
            else
                return GetLastVisibleActivePopUpOfWindow(lastPopUp);
        }
        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern IntPtr GetAncestor(IntPtr hwnd, GaFlags flags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        enum GaFlags
        {
            /// <summary>
            /// Retrieves the parent window. This does not include the owner, as it does with the GetParent function. 
            /// </summary>
            GA_PARENT = 1,
            /// <summary>
            /// Retrieves the root window by walking the chain of parent windows.
            /// </summary>
            GA_ROOT = 2,
            /// <summary>
            /// Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent. 
            /// </summary>
            GA_ROOTOWNER = 3
        }
    }
}
