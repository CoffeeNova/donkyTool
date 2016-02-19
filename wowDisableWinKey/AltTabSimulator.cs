using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Gma.UserActivityMonitor;

namespace wowDisableWinKey
{
    //http://csharpindepth.com/articles/general/singleton.aspx - about singltons

    public sealed class AltTabSimulator
    {
        private SystemProcessHookForm systemProcessHook;
        private static readonly Lazy<AltTabSimulator> instance = new Lazy<AltTabSimulator>(() => new AltTabSimulator());

        public static AltTabSimulator Instance
        {
            get { return instance.Value; }
        }

        private SwitchTo tabOrWindow;
        public SwitchTo TabOrWindow
        {
            get { return tabOrWindow; }
            set { tabOrWindow = value; }
        }
        private List<IntPtr> altTabList;
        public List<IntPtr> AltTabList
        {
            get { return altTabList; }
        }

        private bool started = false;
        public bool Started
        {
            get { return started; }
        }

        private  bool suspended = true;
        public  bool Suspended
        {
            get { return suspended; }
        }

        private AltTabSimulator() 
        {
            systemProcessHook = new SystemProcessHookForm();

        }

        public void Start()
        {
            if (!started && suspended)
            {
                RefreshAltTabList();
                systemProcessHook.WindowEvent += systemProcessHook_WindowEvent;
                HookManager.ForegroundChanged += HookManager_ForegroundChanged;
                started = true;
                suspended = false;
            }
        }

        public void Suspend()
        {
            if (!suspended && started)
            {
                ClearAltTabList();
                systemProcessHook.WindowEvent -= systemProcessHook_WindowEvent;
                HookManager.ForegroundChanged -= HookManager_ForegroundChanged;
                suspended = true;
                started = false;
            }
        }
        /// <summary>
        ///Обновляет список видимых окон, как в списке альт таба (почти как, еще добавлет закрытые окна, окторые в вин10 почему-то остаются висеть в процессах, типа calc.exe)
        /// </summary>
        private void RefreshAltTabList()
        {
            altTabList = OpenWindowGetter.GetAltTabWindowsHandles(); 
        }
        private void ClearAltTabList()
        {
            altTabList.Clear();
        }
        private void HookManager_ForegroundChanged(object sender, EventArgs e)
        {
            bool newWindow = true;
            IntPtr fore = (IntPtr)sender;

            try
            {
                // try to find new foreground window in alt tab list
                foreach (IntPtr hWnd in altTabList)
                    if (hWnd == fore)
                    {
                        newWindow = false;
                        break;
                    }
                //Thread.Sleep(10);
                //if (newWindow && OpenWindowGetter.KeepWindowHandleInAltTabList(fore))
                //{
                //    altTabList.Insert(0, fore);
                //    Console.WriteLine(WowDisableWinKeyTools.GetWindowTitle(fore) + " " + fore.ToString());
                //}
                if (!newWindow)
                {
                    IntPtr windowHWnd = altTabList.Find(x => x == fore);
                    if (windowHWnd != IntPtr.Zero)
                    {
                        altTabList.Remove(windowHWnd);
                        altTabList.Insert(0, windowHWnd);
                        tabOrWindow = SwitchTo.Window;
                    }
                }
                //check if window exists, remove from list if not

            }
            catch { }

        }

        private void systemProcessHook_WindowEvent(object sender, IntPtr handle, Interop.ShellEvents shell)
        {
            if (shell == Interop.ShellEvents.HSHELL_WINDOWDESTROYED)
                if (altTabList.Remove(handle))
                    Console.WriteLine("hwnd:{0}, title:{1} removed. altTabList cound ={2}", handle.ToString(), WowDisableWinKeyTools.GetWindowTitle(handle), altTabList.Count.ToString());
            if (shell == Interop.ShellEvents.HSHELL_WINDOWCREATED && OpenWindowGetter.KeepWindowHandleInAltTabList(handle))
            {
                altTabList.Insert(0, handle);
                Console.WriteLine("hwnd:{0}, title:{1} inserted. altTabList cound ={2}", handle.ToString(), WowDisableWinKeyTools.GetWindowTitle(handle), altTabList.Count.ToString());
                tabOrWindow = SwitchTo.Window;
            }
        }
        private void Dispose()
        {
            try
            {
                systemProcessHook.WindowEvent -= systemProcessHook_WindowEvent;
                HookManager.ForegroundChanged -= HookManager_ForegroundChanged;
            }
            catch { }
        }
        ~AltTabSimulator()
        {
            Dispose();
        }
    }

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
            if (window == GetShellWindow() || GetWindowTextLength(window) == 0)   //Desktop or without title
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
