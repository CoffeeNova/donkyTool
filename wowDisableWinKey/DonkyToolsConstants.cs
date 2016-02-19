using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Drawing;
using NWTweak;
using System.ComponentModel;
using System.Diagnostics;

namespace wowDisableWinKey
{
    using Tools = WowDisableWinKeyTools;

    internal static class Const
    {
        internal const bool use_Hook_Not_RegisterHotKey = true; // Which method will be used for changing keyboard layout
        //internal const byte VK_TAB = 0x09;
        //internal const byte VK_CONTROL = 0x11;
        //internal const byte VK_MENU = 0x12;
        //internal const byte VK_ESCAPE = 0x1B;
        //internal const byte V = 0x56;
        internal const int KEYEVENTF_EXTENDEDKEY = 0x01;
        internal const int KEYEVENTF_KEYUP = 0x02;
        internal const int UIA_ControlTypePropertyId = 30003;
        internal const int UIA_EditControlTypeId = 50004;
        internal const int KEY_REGISTER_ID = 0;
        internal const int SHIFTKEY_REGISTER_ID = 1;
        internal const int CTRLKEY_REGISTER_ID = 2;
        internal const int ALTKEY_REGISTER_ID = 3;
        internal const int WINKEY_REGISTER_ID = 3;
        internal const int SW_RESTORE = 9;
        internal const int DddRawTargetPath = 0x00000001;
        internal const int MAX_ALLOWED_MESSAGES = 10;
        internal const int WM_GETTEXT = 0x000D;
        internal const int WM_GETTEXTLENGTH = 0x000E;
        internal const int WM_LBUTTONDOWN = 0x0201;
        internal const int WM_LBUTTONUP = 0x0202;
        internal const int WM_LBUTTONDBLCLK = 0x0203;
        internal const int WM_MOUSEACTIVATE = 0x0021;
        internal const int WM_HOTKEY = 0x312;
        internal const int WM_KEYDOWN = 0x100;
        internal const int WM_KEYUP = 0x101;
        internal const int WM_SYSKEYDOWN = 0x104;
        internal const int WM_SYSKEYUP = 0x105;
        internal const int WM_SYSCHAR = 0x106;
        internal const int WM_SYSDEADCHAR = 0x107;
        internal const int WM_INPUTLANGCHANGEREQUEST = 0x0050;
        internal const int WM_INPUTLANGCHANGE = 0x0051;
        internal const int MK_LBUTTON = 0x0001;
        internal const int MK_RBUTTON = 0x0002;
        internal const int MA_ACTIVATE = 1;
        internal const int MA_ACTIVATEANDEAT = 2;
        internal const int BM_CLICK = 0x00F5;
        internal const int CTRL_F = 0xA20046; //ctrl+F 
        internal const int MAXTITLE = 255;
        internal const int ENG_LANG_KEYB_LAYOUT = 1033;
        internal const int RUS_LANG_KEYB_LAYOUT = 1049;
        internal const uint FileAnyAccess = 0;
        internal const uint MethodBuffered = 0;
        internal const uint FileDeviceKeyboard = 0x0000000b;
        internal const uint KLF_ACTIVATE = 0x00000001;
        internal const uint KLF_REORDER = 0x00000008;
        internal const uint KLF_SUBSTITUTE_OK = 0x00000002;
        internal const uint KLF_NOTELLSHELL = 0x00000080;
        internal const uint KLF_REPLACELANG = 0x00000010;
        internal const uint KLF_SETFORPROCESS = 0x00000100;
        internal const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        internal const uint MOUSEEVENTF_LEFTUP = 0x04;
        internal const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        internal const uint MOUSEEVENTF_RIGHTUP = 0x10;
        internal const uint MOUSEEVENTF_ABSOLUTE = 0x00008000;



        internal const string DEFAUL_WOW_PROCESS_NAME = "Wow-64";
        internal const string DEFAULT_BROWSER_PROCESS_NAME = "iexplore";
        internal const string CHROME_PROCESS_NAME = "chrome";
        internal const string OPERA_PROCESS_NAME = "opera";
        internal const string FIREFOX_PROCESS_NAME = "firefox";
        internal const string TOR_PROCESS_NAME = "firefox";
        internal const string IE_PROCESS_NAME = "iexplore";
        internal const string SETTINGS_LOCATION = @"SOFTWARE\WowDisableWinKey";
        internal const string WOW_PROCESS_KEY_NAME = @"WowProcessName";
        internal const string CHROME_PROCESS_KEYNAME = @"ChromeProcessName";
        internal const string BLOCK_WINKEY = "BlockWinKey";
        internal const string DISABLE_ALL = "DisableAll";
        internal const string ADR_BAR_LANG = "AdressBarLanguage";
        internal const string CAPS_SWITCH = "CapsSwitch";
        internal const string WSS = "wss";
        internal const string WSS_CHROME = "wss_gc";
        internal const string WSS_OPERA = "wss_o";
        internal const string WSS_FIREFOX = "wss_ff";
        internal const string WSS_TOR = "wss_tor";
        internal const string WSS_IE = "wss_ie";
        internal const string CHROME_CLASS_NAME = "Chrome_WidgetWin_1";
        internal const string FIREFOX_CLASS_NAME = "";
        internal const string IE_CLASS_NAME = "";
        internal const string BROWSER_PROCESS_KEYNAME = "BrowserProcess";
        internal const string WEBSKYPE_MODIFIER_CHROME = "wsgc_mod";
        internal const string WEBSKYPE_KEY_CHROME = "wsgc_key";
        internal const string WEBSKYPE_MODIFIER_OPERA = "wso_mod";
        internal const string WEBSKYPE_KEY_OPERA = "wso_key";
        internal const string WEBSKYPE_MODIFIER_FIREFOX = "wsff_mod";
        internal const string WEBSKYPE_KEY_FIREFOX = "wsff_key";
        internal const string WEBSKYPE_MODIFIER_TOR = "wst_mod";
        internal const string WEBSKYPE_KEY_TOR = "wst_key";
        internal const string WEBSKYPE_MODIFIER_IE = "wsie_mod";
        internal const string WEBSKYPE_KEY_IE = "wsie_key";
        internal const string WEBSKYPE_BROWSERS = "wsBrowsers";
        internal const string NOTIFY_ICON_TEXT = "donky\'s tool";
        internal const string SHELL_TRAYWND = "Shell_TrayWnd";
        internal const Keys WEBSKYPE_DEFAULT_BIND = Keys.Z;
        internal const GBC_KeyModifier WEBSKYPE_DEFAULT_MODIFIER = GBC_KeyModifier.Alt;
        //internal const string GOOGLE_CHROME_

    }
    internal static class WowDisableWinKeyTools
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="keyName"></param>
        /// <param name="keyLocation"></param>
        /// <param name="keyDefaultValue"></param>
        internal static void CheckRegistrySettings(ref string keyValue, string keyName, string keyLocation, string keyDefaultValue)
        {
            if (RegistryWorker.GetKeyValue<string>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName) != null)
                keyValue = RegistryWorker.GetKeyValue<string>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName);
            else if (RegistryWorker.WriteKeyValue(RegistryWorker.WhichRoot.HKEY_LOCAL_MACHINE, keyLocation, keyName, Microsoft.Win32.RegistryValueKind.String, keyDefaultValue))
                keyValue = keyDefaultValue;
            else
            {
                MessageBox.Show("UNABLE TO USE REGKEY HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName + " PLEASE TRY TO RUN PROGRAM WITH ADMIN RIGHTS");
                System.Environment.Exit(0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="keyName"></param>
        /// <param name="keyLocation"></param>
        /// <param name="keyDefaultValue"></param>
        internal static void CheckRegistrySettings(ref bool keyValue, string keyName, string keyLocation, bool keyDefaultValue)
        {
            if (RegistryWorker.GetKeyValue<string>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName) != null)
                keyValue = RegistryWorker.GetKeyValue<string>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName) == "true" ? true : false;
            else if (RegistryWorker.WriteKeyValue(RegistryWorker.WhichRoot.HKEY_LOCAL_MACHINE, keyLocation, keyName, Microsoft.Win32.RegistryValueKind.String, keyDefaultValue == true ? "true" : "false"))
                keyValue = keyDefaultValue;
            else
            {
                MessageBox.Show("UNABLE TO USE REGKEY HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName + " PLEASE TRY TO RUN PROGRAM WITH ADMIN RIGHTS");
                System.Environment.Exit(0);
            }
        }
        internal static bool CheckRegistrySettings(bool keyValue, string keyName, string keyLocation, bool keyDefaultValue)
        {
            if (RegistryWorker.GetKeyValue<string>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName) != null)
                keyValue = RegistryWorker.GetKeyValue<string>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName) == "true" ? true : false;
            else if (RegistryWorker.WriteKeyValue(RegistryWorker.WhichRoot.HKEY_LOCAL_MACHINE, keyLocation, keyName, Microsoft.Win32.RegistryValueKind.String, keyDefaultValue == true ? "true" : "false"))
                keyValue = keyDefaultValue;
            else
            {
                MessageBox.Show("UNABLE TO USE REGKEY HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName + " PLEASE TRY TO RUN PROGRAM WITH ADMIN RIGHTS");
                System.Environment.Exit(0);
            }
            return keyValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="keyName"></param>
        /// <param name="keyLocation"></param>
        /// <param name="keyDefaultValue"></param>
        internal static void CheckRegistrySettings(ref Keys keyValue, string keyName, string keyLocation, Keys keyDefaultValue)
        {
            try
            {
                if (RegistryWorker.GetKeyValue<string>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName) != null)
                    keyValue = ConvertFromString(RegistryWorker.GetKeyValue<string>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName));
                else if (RegistryWorker.WriteKeyValue(RegistryWorker.WhichRoot.HKEY_LOCAL_MACHINE, keyLocation, keyName, Microsoft.Win32.RegistryValueKind.String, keyDefaultValue.ToString()))
                    keyValue = keyDefaultValue;
                else
                {
                    MessageBox.Show("UNABLE TO USE REGKEY HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName + " PLEASE TRY TO RUN PROGRAM WITH ADMIN RIGHTS");
                    System.Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("WRONG VALUE AT HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName);
                System.Environment.Exit(0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="keyName"></param>
        /// <param name="keyLocation"></param>
        internal static void CheckRegistrySettings(ref List<GBC_KeyModifier> keyValue, string keyName, string keyLocation, List<GBC_KeyModifier> keyDefaultValue)
        {
            Func<List<GBC_KeyModifier>, string[]> fu = (gbc_list) =>
            {
                string[] test = new string[gbc_list.Count];
                for (int i = 0; i < gbc_list.Count; i++)
                    test[i] = gbc_list[i].ToString();
                return test;
            };

            if (RegistryWorker.GetKeyValue<string[]>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName) != null)
            {
                string[] keyValueReg = RegistryWorker.GetKeyValue<string[]>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName);
                Func<string, GBC_KeyModifier> func = (value) =>
                {
                    GBC_KeyModifier mod;
                    #region switch-case shit
                    switch (value)
                    {
                        case "Shift":
                            mod = GBC_KeyModifier.Shift;
                            break;
                        case "Control":
                            mod = GBC_KeyModifier.Control;
                            break;
                        case "Alt":
                            mod = GBC_KeyModifier.Alt;
                            break;
                        case "Winkey":
                            mod = GBC_KeyModifier.WinKey;
                            break;
                        case "ShiftAlt":
                            mod = GBC_KeyModifier.ShiftAlt;
                            break;
                        case "ShiftControl":
                            mod = GBC_KeyModifier.ShiftControl;
                            break;
                        case "ShiftControlALt":
                            mod = GBC_KeyModifier.ShiftControlAlt;
                            break;
                        case "None":
                            mod = GBC_KeyModifier.None;
                            break;
                        case "ControlAlt":
                            mod = GBC_KeyModifier.ControlAlt;
                            break;
                        default:
                            mod = GBC_KeyModifier.None;
                            break;
                    }
                    #endregion
                    return mod;

                };
                keyValue.Clear();
                foreach (string keyV in keyValueReg)
                    keyValue.Add(func(keyV));
            }
            else if (RegistryWorker.WriteKeyValue(RegistryWorker.WhichRoot.HKEY_LOCAL_MACHINE, keyLocation, keyName, Microsoft.Win32.RegistryValueKind.MultiString, fu(keyDefaultValue)))
                keyValue = keyDefaultValue;
            else
            {
                MessageBox.Show("UNABLE TO USE REGKEY HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName + " PLEASE TRY TO RUN PROGRAM WITH ADMIN RIGHTS");
                System.Environment.Exit(0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="keyName"></param>
        /// <param name="keyLocation"></param>
        /// <param name="keyDefaultValue"></param>
        internal static void CheckRegistrySettings(ref List<InternetBrowser> keyValue, string keyName, string keyLocation, InternetBrowser keyDefaultValue)
        {
            if (RegistryWorker.GetKeyValue<string>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName) != null)
            {
                string[] keyValueReg = RegistryWorker.GetKeyValue<string[]>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName);
                foreach (string keyV in keyValueReg)
                    keyValue.Add(BrowserFromProgid(keyV));
            }
            else if (RegistryWorker.WriteKeyValue(RegistryWorker.WhichRoot.HKEY_LOCAL_MACHINE, keyLocation, keyName, Microsoft.Win32.RegistryValueKind.MultiString, new string[] { keyDefaultValue.ToString() }))
                keyValue.Add(keyDefaultValue);
            else
            {
                MessageBox.Show("UNABLE TO USE REGKEY HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName + " PLEASE TRY TO RUN PROGRAM WITH ADMIN RIGHTS");
                System.Environment.Exit(0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastValue"></param>
        /// <param name="keyName"></param>
        /// <param name="keyLocation"></param>
        /// <param name="saveValue"></param>
        internal static void SaveRegistrySettings(ref string lastValue, string keyName, string keyLocation, string saveValue)
        {
            if (RegistryWorker.WriteKeyValue(RegistryWorker.WhichRoot.HKEY_LOCAL_MACHINE, keyLocation, keyName, Microsoft.Win32.RegistryValueKind.String, saveValue))
                lastValue = saveValue;
            else
            {
                MessageBox.Show("UNABLE TO USE REGKEY HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName + " PLEASE TRY TO RUN PROGRAM WITH ADMIN RIGHTS");
                System.Environment.Exit(0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastValue"></param>
        /// <param name="keyName"></param>
        /// <param name="keyLocation"></param>
        /// <param name="saveValue"></param>
        internal static void SaveRegistrySettings(ref bool lastValue, string keyName, string keyLocation, bool saveValue)
        {
            if (lastValue == saveValue)
                return;
            if (RegistryWorker.WriteKeyValue(RegistryWorker.WhichRoot.HKEY_LOCAL_MACHINE, keyLocation, keyName, Microsoft.Win32.RegistryValueKind.String, saveValue == true ? "true" : "false"))
                lastValue = saveValue;
            else
            {
                MessageBox.Show("UNABLE TO USE REGKEY HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName + " PLEASE TRY TO RUN PROGRAM WITH ADMIN RIGHTS");
                System.Environment.Exit(0);
            }
        }
        internal static bool SaveRegistrySettings(bool lastValue, string keyName, string keyLocation, bool saveValue)
        {
            if (lastValue == saveValue)
                return lastValue;
            if (RegistryWorker.WriteKeyValue(RegistryWorker.WhichRoot.HKEY_LOCAL_MACHINE, keyLocation, keyName, Microsoft.Win32.RegistryValueKind.String, saveValue == true ? "true" : "false"))
                lastValue = saveValue;
            else
            {
                MessageBox.Show("UNABLE TO USE REGKEY HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName + " PLEASE TRY TO RUN PROGRAM WITH ADMIN RIGHTS");
                System.Environment.Exit(0);
            }
            return lastValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastValue"></param>
        /// <param name="keyName"></param>
        /// <param name="keyLocation"></param>
        /// <param name="saveValue"></param>
        internal static void SaveRegistrySettings(ref Keys lastValue, string keyName, string keyLocation, Keys saveValue)
        {
            if (lastValue == saveValue)
                return;
            if (RegistryWorker.WriteKeyValue(RegistryWorker.WhichRoot.HKEY_LOCAL_MACHINE, keyLocation, keyName, Microsoft.Win32.RegistryValueKind.String, saveValue.ToString()))
                lastValue = saveValue;
            else
            {
                MessageBox.Show("UNABLE TO USE REGKEY HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName + " PLEASE TRY TO RUN PROGRAM WITH ADMIN RIGHTS");
                System.Environment.Exit(0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastValue"></param>
        /// <param name="keyName"></param>
        /// <param name="keyLocation"></param>
        /// <param name="saveValue"></param>
        internal static void SaveRegistrySettings(ref List<GBC_KeyModifier> lastValue, string keyName, string keyLocation, List<GBC_KeyModifier> saveValue)
        {
            if (lastValue == saveValue)
                return;
            if (RegistryWorker.WriteKeyValue(RegistryWorker.WhichRoot.HKEY_LOCAL_MACHINE, keyLocation, keyName, Microsoft.Win32.RegistryValueKind.MultiString, saveValue.Select(x => { return x.ToString(); }).ToArray()))
                lastValue = saveValue;
            else
            {
                MessageBox.Show("UNABLE TO USE REGKEY HKEY_LOCAL_MACHINE\\" + keyLocation + "\\" + keyName + " PLEASE TRY TO RUN PROGRAM WITH ADMIN RIGHTS");
                System.Environment.Exit(0);
            }
        }
        /// <summary>
        /// Возвращает список дескрипторов окон, имя класса которых className
        /// </summary>
        /// <param name="processID">id процесса</param>
        /// <param name="className">имя класса по которому производится выборка</param>
        /// <returns></returns>
        internal static List<IntPtr> GetWidgetWindowHandles(int processID, string className)
        {
            //get all windows handles
            List<IntPtr> rootWindows = GetRootWindowsOfProcess(processID);
            // find the handles witch contains widget window
            AutomationElement rootWindowAE;
            List<IntPtr> widgetHandles = new List<IntPtr>();
            foreach (IntPtr handle in rootWindows)
            {
                rootWindowAE = AutomationElement.FromHandle(handle);
                if (rootWindowAE == null)
                    continue;
                if (rootWindowAE.Current.ClassName == className)
                {
                    widgetHandles.Add(handle);
                }
            }
            return widgetHandles;
        }
        internal static string GetWindowTitle(IntPtr hwnd)
        {
            StringBuilder sb = new StringBuilder();
            int longi = Interop.GetWindowTextLength(hwnd) + 1;
            sb.Capacity = longi;
            Interop.GetWindowText(hwnd, sb, sb.Capacity);
            return sb.ToString();
        }
        internal static List<IntPtr> GetRootWindowsOfProcess(int pid)
        {
            List<IntPtr> rootWindows = GetChildWindows(IntPtr.Zero);
            List<IntPtr> dsProcRootWindows = new List<IntPtr>();
            foreach (IntPtr hWnd in rootWindows)
            {
                uint lpdwProcessId;
                Interop.GetWindowThreadProcessId(hWnd, out lpdwProcessId);
                if (lpdwProcessId == pid)
                    dsProcRootWindows.Add(hWnd);
            }
            return dsProcRootWindows;
        }

        internal static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                Interop.Win32Callback childProc = new Interop.Win32Callback(EnumWindow);
                Interop.EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        internal static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            //  You can modify this to check to see if you want to cancel the operation, then return a null here
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        internal static bool RestoreMinimizedWindow(IntPtr hWnd)
        {

            var placement = GetPlacement(hWnd);
            if (placement.showCmd == ShowWindowCommands.Minimized)
            {
                Interop.ShowWindow(hWnd, ShowWindowEnum.Restore);
                return true;
            }
            return false;
        }

        internal static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            Interop.GetWindowPlacement(hwnd, ref placement);
            return placement;
        }
        /// <summary>
        /// Simulate a click //- //- //- //
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <param name="handle"></param>
        internal static void SimulateClickUIAutomation(AutomationElement child, AutomationElement parent, IntPtr handle)
        {
            //get new rectangle relatively parent window
            System.Windows.Rect clickZone = BoundingRectangleUIElement(child, parent);

            Point clickPoint = CountRectangleCenter(clickZone);
            //System.Windows.Point webSkypeTabClickPoint = child.GetClickablePoint();
            int xPointTabitem = clickPoint.X;
            int yPointTabitem = clickPoint.Y;

            //create lParam
            int point = (int)clickPoint.Y << 16 | (int)clickPoint.X;
            //point = 10 << 16 | 110;

            Interop.PostMessage(handle, Const.WM_LBUTTONDOWN, (IntPtr)Const.MK_LBUTTON, (IntPtr)point);
            Interop.PostMessage(handle, Const.WM_LBUTTONUP, IntPtr.Zero, (IntPtr)point);
        }
        /// <summary>
        ///  Возвращает координаты прямоугольника, который полностью охватывает элемент, относительно главного окна
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        internal static System.Windows.Rect BoundingRectangleUIElement(AutomationElement child, AutomationElement parent)
        {
            if (child == null || parent == null)
                return new System.Windows.Rect();
            System.Windows.Rect parentRect = parent.Current.BoundingRectangle;
            System.Windows.Rect childRect = child.Current.BoundingRectangle;
            System.Windows.Point relativePoint = new System.Windows.Point(childRect.X - parentRect.X, childRect.Y - parentRect.Y);
            return new System.Windows.Rect(relativePoint, childRect.Size);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        internal static Point CountRectangleCenter(System.Windows.Rect rect)
        {
            return new Point((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
        }

        /// <summary>
        /// Defines if process is a fullscreen
        /// </summary>
        /// <param name="handle">Window handle</param>
        /// <returns>True if process is a fullscreen</returns>
        internal static bool FullscreenProcess(IntPtr handle)
        {
            // get the placement
            WINDOWPLACEMENT forePlacement = new WINDOWPLACEMENT();
            forePlacement.length = Marshal.SizeOf(forePlacement);
            Interop.GetWindowPlacement(handle, ref forePlacement);
            RECT rect;
            var success = Interop.GetWindowRect(handle, out rect);
            Rectangle screenBounds = Screen.GetBounds(new Point(rect.Left, rect.Top));
            if (success && Math.Abs(rect.Left + rect.Width) >= screenBounds.Width)
                return true;
            return false;
        }
        /// <summary>
        /// Set window to minimize state
        /// </summary>
        /// <param name="hWnd">window handle</param>
        /// /// <param name="allState">False if window is already minimized</param>
        /// <returns></returns>
        internal static bool MinimizeWindow(IntPtr hWnd)
        {
            var placement = GetPlacement(hWnd);
            if (placement.showCmd != ShowWindowCommands.Minimized)
            {
                Interop.ShowWindow(hWnd, ShowWindowEnum.Minimize);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keystr"></param>
        /// <returns></returns>
        internal static Keys ConvertFromString(string keystr)
        {
            return (Keys)Enum.Parse(typeof(Keys), keystr);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gbc"></param>
        /// <returns></returns>
        internal static string ConvertFromGBC(GBC_KeyModifier gbc)
        {
            return gbc.ToString();
        }
        /// <summary>
        /// Check wich browser is a default in the system
        /// </summary>
        /// <returns>Default browser</returns>
        internal static InternetBrowser DefaultBrowser()
        {
            string keyLocation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";
            string keyName = "Progid";
            string keyValueReg = RegistryWorker.GetKeyValue<string>(Microsoft.Win32.RegistryHive.LocalMachine, keyLocation, keyName);
            if (keyValueReg == null)
                return InternetBrowser.InternetExplorer;
            return BrowserFromProgid(keyValueReg);

        }
        /// <summary>
        /// Retrieve InternetBrowser from registry value
        /// </summary>
        /// <param name="browser">String registry value HKCU\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice\Progid</param>
        /// <returns>InternetBrowser from registry value</returns>
        private static InternetBrowser BrowserFromProgid(string browser)
        {
            switch (browser)
            {
                case "ChromeHTML":
                    return InternetBrowser.GoogleChrome;
                case "FirefoxURL":
                    return InternetBrowser.Firefox;
                case "IE.HTTP":
                    return InternetBrowser.InternetExplorer;
                case "Opera.Protocol":
                    return InternetBrowser.Opera;
                default:
                    return InternetBrowser.InternetExplorer;
            }
        }
        internal static Keys WssBindCheck(string wssBindKey, string settingLocation, Keys defaultBind)
        {
            var bindProperty = defaultBind;
            CheckRegistrySettings(ref bindProperty, wssBindKey, settingLocation, defaultBind);
            return bindProperty;
        }
        internal static List<GBC_KeyModifier> WssModCheck(string wssModifierKey, string settingLocation, GBC_KeyModifier defaultModifier)
        {
            var modProperty = new List<GBC_KeyModifier>();
            var defaultModList = new List<GBC_KeyModifier> { defaultModifier };
            CheckRegistrySettings(ref modProperty, wssModifierKey, settingLocation, defaultModList);
            return modProperty;
        }

        internal static void SendKey(IntPtr hwnd, WindowsVirtualKey keyCode, bool extended)
        {
            uint scanCode = Interop.MapVirtualKey((uint)keyCode, 0);
            uint lParam;

            //KEY DOWN
            lParam = (0x00000001 | (scanCode << 16));
            if (extended)
            {
                lParam = lParam | 0x01000000;
            }
            Interop.PostMessage(hwnd, Const.WM_KEYDOWN, (IntPtr)keyCode, (IntPtr)lParam);
            //KEY UP
            Interop.PostMessage(hwnd, Const.WM_KEYUP, (IntPtr)keyCode, (IntPtr)lParam);
        }

        internal static void UncheckOtherToolStripMenuItems(ToolStripMenuItem selectedMenuItem)
        {
            //selectedMenuItem.Checked = true;

            // Select the other MenuItens from the ParentMenu(OwnerItens) and unchecked this,
            // The current Linq Expression verify if the item is a real ToolStripMenuItem
            // and if the item is a another ToolStripMenuItem to uncheck this.
            foreach (var ltoolStripMenuItem in (from object
                                                    item in selectedMenuItem.Owner.Items
                                                let ltoolStripMenuItem = item as ToolStripMenuItem
                                                where ltoolStripMenuItem != null
                                                where !item.Equals(selectedMenuItem)
                                                select ltoolStripMenuItem))
                (ltoolStripMenuItem).Checked = false;

            // This line is optional, for show the mainMenu after click
            selectedMenuItem.Owner.Show();
        }

        internal static bool AnyIsTrue(bool one, bool two, bool three, bool four, bool five)
        {
            return one || two || three || four || five ? true : false;
        }
        internal static bool AnyIsTrue(bool one, bool two, bool three, bool four, bool five, bool six)
        {
            return one || two || three || four || five || six ? true : false;
        }

        internal static string GetClassName(IntPtr hWnd)
        {
            StringBuilder title = new StringBuilder(Const.MAXTITLE);
            int titleLength = wowDisableWinKey.Interop.GetClassName(hWnd, title, title.Capacity + 1);
            title.Length = titleLength;

            return title.ToString();
        }
        /// <summary>
        /// Returns the caption of a window by given HWND identifier.
        /// </summary>
        internal static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder title = new StringBuilder(Const.MAXTITLE);
            int titleLength = wowDisableWinKey.Interop.GetWindowText(hWnd, title, title.Capacity + 1);
            title.Length = titleLength;

            return title.ToString();
        }
        internal static int GetProcessId(IntPtr hWnd)
        {
            uint processId;
            wowDisableWinKey.Interop.GetWindowThreadProcessId(hWnd, out processId);
            return (int)processId;
        }
        internal static string GetProcessName(IntPtr hWnd)
        {
            uint processId;
            wowDisableWinKey.Interop.GetWindowThreadProcessId(hWnd, out processId);
            return System.Diagnostics.Process.GetProcessById((int)processId).ProcessName;
        }
        internal static System.Diagnostics.Process GetProcess(IntPtr hWnd)
        {
            uint processId;
            wowDisableWinKey.Interop.GetWindowThreadProcessId(hWnd, out processId);
            return System.Diagnostics.Process.GetProcessById((int)processId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        internal static Process UIProcess(string processName)
        {
            Process uiProcess = null;
            Process[] procs = Process.GetProcessesByName(processName);
            if (procs.Length == 0)
                return null;
            else
                // the process must have a window
                uiProcess = procs.FirstOrDefault((proc) => proc.MainWindowHandle != IntPtr.Zero);
            return uiProcess;
        }
        internal static void SimulateBindActionOnce(WebSkypeSwitcher wss)
        {
            if (wss.IsActive)
                wss.BindAction();
        }
        internal static bool WebSkypeOrNotifyWindowHandle(IntPtr foregroundhWnd, IntPtr webSkypehWnd)
        {
            // Console.WriteLine(foregroundhWnd.ToString("X"));
            if (foregroundhWnd == webSkypehWnd)
                return true;
            try
            {
                var ae = AutomationElement.FromHandle(foregroundhWnd);
                return ae.Current.ClassName == Const.SHELL_TRAYWND ? true : false;
            }
            catch { return false; }
        }
        internal static bool NotWebSkypeButNotifyWindowHandle(IntPtr foregroundhWnd, IntPtr webSkypehWnd)
        {
            try
            {
                var ae = AutomationElement.FromHandle(foregroundhWnd);
                return foregroundhWnd != webSkypehWnd && ae.Current.ClassName == Const.SHELL_TRAYWND ? true : false;
            }
            catch { return false; }
        }
    }
    #region private structs and enums
    //--------------------------private structs----------------------------------------------// 
    public struct KBDLLHOOKSTRUCT
    {
        public Keys key;
        public int scanCode;
        public int flags;
        public int time;
        public IntPtr extra;
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public ShowWindowCommands showCmd;
        public System.Drawing.Point ptMinPosition;
        public System.Drawing.Point ptMaxPosition;
        public System.Drawing.Rectangle rcNormalPosition;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Width;
        public int Height;
    }
    public enum ShowWindowCommands : int
    {
        Hide = 0,
        Normal = 1,
        Minimized = 2,
        Maximized = 3,
    }
    public struct KeyboardIndicatorParameters
    {
        public ushort UnitId;
        public Locks LedFlags;
    }
    [Flags]
    public enum Locks : ushort
    {
        None = 0,
        KeyboardScrollLockOn = 1,
        KeyboardNumLockOn = 2,
        KeyboardCapsLockOn = 4
    }
    public enum GetWindow_Cmd : uint
    {
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6
    }
    public enum KeyModifier
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        WinKey = 8
    }
    public enum ShowWindowEnum
    {
        Hide = 0,
        ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
        Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
        Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
        Restore = 9, ShowDefault = 10, ForceMinimized = 11
    };
    public enum SwitchTo
    {
        Tab = 0,
        Window = 1
    }

    public struct WebSkypeStruct
    {
        public IntPtr windowHandle;
        public AutomationElement skypeTab;
        public AutomationElement toggleExtension;
    }

    public enum GBC_KeyModifier
    {
        None,
        Shift,
        Control,
        Alt,
        WinKey,
        ShiftControl,
        ShiftAlt,
        ControlAlt,
        ShiftControlAlt
    }
    public enum InternetBrowser
    {
        GoogleChrome,
        Firefox,
        Opera,
        TorBrowser,
        InternetExplorer
    }

    public enum WindowsVirtualKey
    {
        [Description("Left mouse button")]
        VK_LBUTTON = 0x01,

        [Description("Right mouse button")]
        VK_RBUTTON = 0x02,

        [Description("Control-break processing")]
        VK_CANCEL = 0x03,

        [Description("Middle mouse button (three-button mouse)")]
        VK_MBUTTON = 0x04,

        [Description("X1 mouse button")]
        VK_XBUTTON1 = 0x05,

        [Description("X2 mouse button")]
        VK_XBUTTON2 = 0x06,

        [Description("BACKSPACE key")]
        VK_BACK = 0x08,

        [Description("TAB key")]
        VK_TAB = 0x09,

        [Description("CLEAR key")]
        VK_CLEAR = 0x0C,

        [Description("ENTER key")]
        VK_RETURN = 0x0D,

        [Description("SHIFT key")]
        VK_SHIFT = 0x10,

        [Description("CTRL key")]
        VK_CONTROL = 0x11,

        [Description("ALT key")]
        VK_MENU = 0x12,

        [Description("PAUSE key")]
        VK_PAUSE = 0x13,

        [Description("CAPS LOCK key")]
        VK_CAPITAL = 0x14,

        [Description("IME Kana mode")]
        VK_KANA = 0x15,

        [Description("IME Hanguel mode (maintained for compatibility; use VK_HANGUL)")]
        VK_HANGUEL = 0x15,

        [Description("IME Hangul mode")]
        VK_HANGUL = 0x15,

        [Description("IME Junja mode")]
        VK_JUNJA = 0x17,

        [Description("IME final mode")]
        VK_FINAL = 0x18,

        [Description("IME Hanja mode")]
        VK_HANJA = 0x19,

        [Description("IME Kanji mode")]
        VK_KANJI = 0x19,

        [Description("ESC key")]
        VK_ESCAPE = 0x1B,

        [Description("IME convert")]
        VK_CONVERT = 0x1C,

        [Description("IME nonconvert")]
        VK_NONCONVERT = 0x1D,

        [Description("IME accept")]
        VK_ACCEPT = 0x1E,

        [Description("IME mode change request")]
        VK_MODECHANGE = 0x1F,

        [Description("SPACEBAR")]
        VK_SPACE = 0x20,

        [Description("PAGE UP key")]
        VK_PRIOR = 0x21,

        [Description("PAGE DOWN key")]
        VK_NEXT = 0x22,

        [Description("END key")]
        VK_END = 0x23,

        [Description("HOME key")]
        VK_HOME = 0x24,

        [Description("LEFT ARROW key")]
        VK_LEFT = 0x25,

        [Description("UP ARROW key")]
        VK_UP = 0x26,

        [Description("RIGHT ARROW key")]
        VK_RIGHT = 0x27,

        [Description("DOWN ARROW key")]
        VK_DOWN = 0x28,

        [Description("SELECT key")]
        VK_SELECT = 0x29,

        [Description("PRINT key")]
        VK_PRINT = 0x2A,

        [Description("EXECUTE key")]
        VK_EXECUTE = 0x2B,

        [Description("PRINT SCREEN key")]
        VK_SNAPSHOT = 0x2C,

        [Description("INS key")]
        VK_INSERT = 0x2D,

        [Description("DEL key")]
        VK_DELETE = 0x2E,

        [Description("HELP key")]
        VK_HELP = 0x2F,

        [Description("0 key")]
        K_0 = 0x30,

        [Description("1 key")]
        K_1 = 0x31,

        [Description("2 key")]
        K_2 = 0x32,

        [Description("3 key")]
        K_3 = 0x33,

        [Description("4 key")]
        K_4 = 0x34,

        [Description("5 key")]
        K_5 = 0x35,

        [Description("6 key")]
        K_6 = 0x36,

        [Description("7 key")]
        K_7 = 0x37,

        [Description("8 key")]
        K_8 = 0x38,

        [Description("9 key")]
        K_9 = 0x39,

        [Description("A key")]
        K_A = 0x41,

        [Description("B key")]
        K_B = 0x42,

        [Description("C key")]
        K_C = 0x43,

        [Description("D key")]
        K_D = 0x44,

        [Description("E key")]
        K_E = 0x45,

        [Description("F key")]
        K_F = 0x46,

        [Description("G key")]
        K_G = 0x47,

        [Description("H key")]
        K_H = 0x48,

        [Description("I key")]
        K_I = 0x49,

        [Description("J key")]
        K_J = 0x4A,

        [Description("K key")]
        K_K = 0x4B,

        [Description("L key")]
        K_L = 0x4C,

        [Description("M key")]
        K_M = 0x4D,

        [Description("N key")]
        K_N = 0x4E,

        [Description("O key")]
        K_O = 0x4F,

        [Description("P key")]
        K_P = 0x50,

        [Description("Q key")]
        K_Q = 0x51,

        [Description("R key")]
        K_R = 0x52,

        [Description("S key")]
        K_S = 0x53,

        [Description("T key")]
        K_T = 0x54,

        [Description("U key")]
        K_U = 0x55,

        [Description("V key")]
        K_V = 0x56,

        [Description("W key")]
        K_W = 0x57,

        [Description("X key")]
        K_X = 0x58,

        [Description("Y key")]
        K_Y = 0x59,

        [Description("Z key")]
        K_Z = 0x5A,

        [Description("Left Windows key (Natural keyboard)")]
        VK_LWIN = 0x5B,

        [Description("Right Windows key (Natural keyboard)")]
        VK_RWIN = 0x5C,

        [Description("Applications key (Natural keyboard)")]
        VK_APPS = 0x5D,

        [Description("Computer Sleep key")]
        VK_SLEEP = 0x5F,

        [Description("Numeric keypad 0 key")]
        VK_NUMPAD0 = 0x60,

        [Description("Numeric keypad 1 key")]
        VK_NUMPAD1 = 0x61,

        [Description("Numeric keypad 2 key")]
        VK_NUMPAD2 = 0x62,

        [Description("Numeric keypad 3 key")]
        VK_NUMPAD3 = 0x63,

        [Description("Numeric keypad 4 key")]
        VK_NUMPAD4 = 0x64,

        [Description("Numeric keypad 5 key")]
        VK_NUMPAD5 = 0x65,

        [Description("Numeric keypad 6 key")]
        VK_NUMPAD6 = 0x66,

        [Description("Numeric keypad 7 key")]
        VK_NUMPAD7 = 0x67,

        [Description("Numeric keypad 8 key")]
        VK_NUMPAD8 = 0x68,

        [Description("Numeric keypad 9 key")]
        VK_NUMPAD9 = 0x69,

        [Description("Multiply key")]
        VK_MULTIPLY = 0x6A,

        [Description("Add key")]
        VK_ADD = 0x6B,

        [Description("Separator key")]
        VK_SEPARATOR = 0x6C,

        [Description("Subtract key")]
        VK_SUBTRACT = 0x6D,

        [Description("Decimal key")]
        VK_DECIMAL = 0x6E,

        [Description("Divide key")]
        VK_DIVIDE = 0x6F,

        [Description("F1 key")]
        VK_F1 = 0x70,

        [Description("F2 key")]
        VK_F2 = 0x71,

        [Description("F3 key")]
        VK_F3 = 0x72,

        [Description("F4 key")]
        VK_F4 = 0x73,

        [Description("F5 key")]
        VK_F5 = 0x74,

        [Description("F6 key")]
        VK_F6 = 0x75,

        [Description("F7 key")]
        VK_F7 = 0x76,

        [Description("F8 key")]
        VK_F8 = 0x77,

        [Description("F9 key")]
        VK_F9 = 0x78,

        [Description("F10 key")]
        VK_F10 = 0x79,

        [Description("F11 key")]
        VK_F11 = 0x7A,

        [Description("F12 key")]
        VK_F12 = 0x7B,

        [Description("F13 key")]
        VK_F13 = 0x7C,

        [Description("F14 key")]
        VK_F14 = 0x7D,

        [Description("F15 key")]
        VK_F15 = 0x7E,

        [Description("F16 key")]
        VK_F16 = 0x7F,

        [Description("F17 key")]
        VK_F17 = 0x80,

        [Description("F18 key")]
        VK_F18 = 0x81,

        [Description("F19 key")]
        VK_F19 = 0x82,

        [Description("F20 key")]
        VK_F20 = 0x83,

        [Description("F21 key")]
        VK_F21 = 0x84,

        [Description("F22 key")]
        VK_F22 = 0x85,

        [Description("F23 key")]
        VK_F23 = 0x86,

        [Description("F24 key")]
        VK_F24 = 0x87,

        [Description("NUM LOCK key")]
        VK_NUMLOCK = 0x90,

        [Description("SCROLL LOCK key")]
        VK_SCROLL = 0x91,

        [Description("Left SHIFT key")]
        VK_LSHIFT = 0xA0,

        [Description("Right SHIFT key")]
        VK_RSHIFT = 0xA1,

        [Description("Left CONTROL key")]
        VK_LCONTROL = 0xA2,

        [Description("Right CONTROL key")]
        VK_RCONTROL = 0xA3,

        [Description("Left MENU key")]
        VK_LMENU = 0xA4,

        [Description("Right MENU key")]
        VK_RMENU = 0xA5,

        [Description("Browser Back key")]
        VK_BROWSER_BACK = 0xA6,

        [Description("Browser Forward key")]
        VK_BROWSER_FORWARD = 0xA7,

        [Description("Browser Refresh key")]
        VK_BROWSER_REFRESH = 0xA8,

        [Description("Browser Stop key")]
        VK_BROWSER_STOP = 0xA9,

        [Description("Browser Search key")]
        VK_BROWSER_SEARCH = 0xAA,

        [Description("Browser Favorites key")]
        VK_BROWSER_FAVORITES = 0xAB,

        [Description("Browser Start and Home key")]
        VK_BROWSER_HOME = 0xAC,

        [Description("Volume Mute key")]
        VK_VOLUME_MUTE = 0xAD,

        [Description("Volume Down key")]
        VK_VOLUME_DOWN = 0xAE,

        [Description("Volume Up key")]
        VK_VOLUME_UP = 0xAF,

        [Description("Next Track key")]
        VK_MEDIA_NEXT_TRACK = 0xB0,

        [Description("Previous Track key")]
        VK_MEDIA_PREV_TRACK = 0xB1,

        [Description("Stop Media key")]
        VK_MEDIA_STOP = 0xB2,

        [Description("Play/Pause Media key")]
        VK_MEDIA_PLAY_PAUSE = 0xB3,

        [Description("Start Mail key")]
        VK_LAUNCH_MAIL = 0xB4,

        [Description("Select Media key")]
        VK_LAUNCH_MEDIA_SELECT = 0xB5,

        [Description("Start Application 1 key")]
        VK_LAUNCH_APP1 = 0xB6,

        [Description("Start Application 2 key")]
        VK_LAUNCH_APP2 = 0xB7,

        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key")]
        VK_OEM_1 = 0xBA,

        [Description("For any country/region, the '+' key")]
        VK_OEM_PLUS = 0xBB,

        [Description("For any country/region, the ',' key")]
        VK_OEM_COMMA = 0xBC,

        [Description("For any country/region, the '-' key")]
        VK_OEM_MINUS = 0xBD,

        [Description("For any country/region, the '.' key")]
        VK_OEM_PERIOD = 0xBE,

        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key")]
        VK_OEM_2 = 0xBF,

        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key")]
        VK_OEM_3 = 0xC0,

        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '[{' key")]
        VK_OEM_4 = 0xDB,

        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\\|' key")]
        VK_OEM_5 = 0xDC,

        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ']}' key")]
        VK_OEM_6 = 0xDD,

        [Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the 'single-quote/double-quote' key")]
        VK_OEM_7 = 0xDE,

        [Description("Used for miscellaneous characters; it can vary by keyboard.")]
        VK_OEM_8 = 0xDF,


        [Description("Either the angle bracket key or the backslash key on the RT 102-key keyboard")]
        VK_OEM_102 = 0xE2,

        [Description("IME PROCESS key")]
        VK_PROCESSKEY = 0xE5,


        [Description("Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP")]
        VK_PACKET = 0xE7,

        [Description("Attn key")]
        VK_ATTN = 0xF6,

        [Description("CrSel key")]
        VK_CRSEL = 0xF7,

        [Description("ExSel key")]
        VK_EXSEL = 0xF8,

        [Description("Erase EOF key")]
        VK_EREOF = 0xF9,

        [Description("Play key")]
        VK_PLAY = 0xFA,

        [Description("Zoom key")]
        VK_ZOOM = 0xFB,

        [Description("PA1 key")]
        VK_PA1 = 0xFD,

        [Description("Clear key")]
        VK_OEM_CLEAR = 0xFE,

    }
    #endregion

}
