using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Gma.UserActivityMonitor;
using System.Windows.Automation;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using WindowHookNet;

namespace wowDisableWinKey
{
    using Tools = WowDisableWinKeyTools;

    public partial class Form1 : Form
    {
        #region public variables
        //--------------------------public variables---------------------------------------------// 
        public bool testingMarker = false;
        public int testingTime = 0;
        public TimeSpan testWatchTime;
        public Thread testingThread;
        public Stopwatch testWatch;
        #endregion
        #region private constants
        //--------------------------private constants--------------------------------------------//
        #endregion
        #region private variables
        //--------------------------private variables--------------------------------------------//        
        private bool disableAllGotHook = false;
        private bool capsSwitchEnable = false;
        private bool blockWinKey = false;
        private bool disableAll = false;
        private delegate void WssDelegate(SystemProcessHookForm winWatcher, bool state);
        private event WssDelegate wssEnabledChanged;
        private BABLanguageSwitcher chromeAddressBar;
        private BABLanguageSwitcher operaAddressBar;
        private BABLanguageSwitcher firefoxAddressBar;
        private BABLanguageSwitcher tbAddressBar;
        private BABLanguageSwitcher ieAddressBar;
        private bool wssEnable = false;
        private bool WssEnable
        {
            get { return wssEnable; }
            set
            {
                if (value != wssEnable)
                {
                    wssEnable = value;
                    wssEnabledChanged(windowWatcher, value);
                }
            }
        }
        private bool wssChromeEnable = false;
        private bool wssOperaEnable = false;
        private bool wssFirefoxEnable = false;
        private bool wssTorEnable = false;
        private bool wssIEEnable = false;
        private string wowProcessName = "";
        private uint IOCTL_KEYBOARD_SET_INDICATORS;
        private IntPtr ptrHookWinKey;
        private Interop.LowLevelKeyboardProc modifierKeyboardProcess;
        private Process globalWowProcess = null;
        private Process wowProcess;
        private Process[] allProcesses;
        private InternetBrowserData chromeUIProcess = new InternetBrowserData(InternetBrowser.GoogleChrome);
        private InternetBrowserData operaUIProcess = new InternetBrowserData(InternetBrowser.Opera);
        private InternetBrowserData firefoxUIProcess = new InternetBrowserData(InternetBrowser.Firefox);
        private InternetBrowserData torUIProcess = new InternetBrowserData(InternetBrowser.TorBrowser);
        private InternetBrowserData ieUIProcess = new InternetBrowserData(InternetBrowser.InternetExplorer);
        private System.Windows.Forms.Timer mainTimer;
        private LanguageLayout EnglishLayout;
        private LanguageLayout RussianLayout;
        private GlobalBindController CapsLanguageSwitchController;
        private System.ComponentModel.ComponentResourceManager resources;
        private WebSkypeSwitcher wssChrome;
        private WebSkypeSwitcher wssOpera;
        private WebSkypeSwitcher wssFirefox;
        private WebSkypeSwitcher wssTorBrowser;
        private WebSkypeSwitcher wssIE;
        private wowDisableWinKey.Controls.BindControl gcBindControl = new Controls.BindControl();
        private wowDisableWinKey.Controls.BindControl oBindControl = new Controls.BindControl();
        private wowDisableWinKey.Controls.BindControl ffBindControl = new Controls.BindControl();
        private wowDisableWinKey.Controls.BindControl tBindControl = new Controls.BindControl();
        private wowDisableWinKey.Controls.BindControl ieBindControl = new Controls.BindControl();
        private SystemProcessHookForm windowWatcher;

        #endregion

        #region private delegates
        //--------------------------private delegates-------------------------------------------//
        delegate void ChangeKeyboradLayoutDelegate(LanguageLayout lanLaym, Locks locks);
        #endregion

        public Form1()
        {
            InitializeComponent();
            //testingThread = new Thread(new ThreadStart(TestingThreadProc));
            //testWatch = new Stopwatch();
            #region some initialize shit
            gcBindControl.SelectedBindChanged += (sender, e) => bindControl_SelectedBindChanged(sender, e, ref wssChrome, Const.WEBSKYPE_KEY_CHROME, Const.SETTINGS_LOCATION);
            gcBindControl.SelectedModifierChanged += (sender, e) => bindControl_SelectedModifierChanged(sender, e, ref wssChrome, Const.WEBSKYPE_MODIFIER_CHROME, Const.SETTINGS_LOCATION);
            oBindControl.SelectedBindChanged += (sender, e) => bindControl_SelectedBindChanged(sender, e, ref wssOpera, Const.WEBSKYPE_KEY_OPERA, Const.SETTINGS_LOCATION);
            oBindControl.SelectedModifierChanged += (sender, e) => bindControl_SelectedModifierChanged(sender, e, ref wssOpera, Const.WEBSKYPE_MODIFIER_OPERA, Const.SETTINGS_LOCATION);
            ffBindControl.SelectedBindChanged += (sender, e) => bindControl_SelectedBindChanged(sender, e, ref wssFirefox, Const.WEBSKYPE_KEY_FIREFOX, Const.SETTINGS_LOCATION);
            ffBindControl.SelectedModifierChanged += (sender, e) => bindControl_SelectedModifierChanged(sender, e, ref wssFirefox, Const.WEBSKYPE_MODIFIER_FIREFOX, Const.SETTINGS_LOCATION);
            tBindControl.SelectedBindChanged += (sender, e) => bindControl_SelectedBindChanged(sender, e, ref wssTorBrowser, Const.WEBSKYPE_KEY_TOR, Const.SETTINGS_LOCATION);
            tBindControl.SelectedModifierChanged += (sender, e) => bindControl_SelectedModifierChanged(sender, e, ref wssTorBrowser, Const.WEBSKYPE_MODIFIER_TOR, Const.SETTINGS_LOCATION);
            ieBindControl.SelectedBindChanged += (sender, e) => bindControl_SelectedBindChanged(sender, e, ref wssIE, Const.WEBSKYPE_KEY_IE, Const.SETTINGS_LOCATION);
            ieBindControl.SelectedModifierChanged += (sender, e) => bindControl_SelectedModifierChanged(sender, e, ref wssIE, Const.WEBSKYPE_MODIFIER_IE, Const.SETTINGS_LOCATION);

            System.Windows.Forms.ToolStripControlHost tsHost;
            tsHost = new System.Windows.Forms.ToolStripControlHost(gcBindControl);
            this.googleChromeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsHost});
            tsHost = new System.Windows.Forms.ToolStripControlHost(oBindControl);
            this.operaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsHost});
            tsHost = new System.Windows.Forms.ToolStripControlHost(ffBindControl);
            this.firefoxToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsHost});
            tsHost = new System.Windows.Forms.ToolStripControlHost(tBindControl);
            this.torBrowserToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsHost});
            tsHost = new System.Windows.Forms.ToolStripControlHost(ieBindControl);
            this.internetExplorerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsHost});

            this.googleChromeToolStripMenuItem.CheckedChanged += (sender, e) => browserMenuItem_CheckedChanged(sender, e, ref wssChromeEnable, Const.WSS_CHROME, Const.SETTINGS_LOCATION, "Google Chrome");
            this.operaToolStripMenuItem.CheckedChanged += (sender, e) => browserMenuItem_CheckedChanged(sender, e, ref wssOperaEnable, Const.WSS_OPERA, Const.SETTINGS_LOCATION, "Opera");
            this.firefoxToolStripMenuItem.CheckedChanged += (sender, e) => browserMenuItem_CheckedChanged(sender, e, ref wssFirefoxEnable, Const.WSS_FIREFOX, Const.SETTINGS_LOCATION, "Firefox");
            this.torBrowserToolStripMenuItem.CheckedChanged += (sender, e) => browserMenuItem_CheckedChanged(sender, e, ref wssTorEnable, Const.WSS_TOR, Const.SETTINGS_LOCATION, "Tor Browser");
            this.internetExplorerToolStripMenuItem.CheckedChanged += (sender, e) => browserMenuItem_CheckedChanged(sender, e, ref wssIEEnable, Const.WSS_IE, Const.SETTINGS_LOCATION, "Internet Explorer");

            //Initialize project objects
            EnglishLayout = new LanguageLayout(LanguageLayout.LayoutEnum.En);
            RussianLayout = new LanguageLayout(LanguageLayout.LayoutEnum.Ru);

            wssEnabledChanged += Form1_wssEnabledChanged;
            windowWatcher = new SystemProcessHookForm();

            resources = new ComponentResourceManager(typeof(Form1));

            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            //------------------------------------------------------------------------------
            List<KeyValuePair<GBC_KeyModifier, Action>> clModifierPairList = new List<KeyValuePair<GBC_KeyModifier, Action>>
            {
                new KeyValuePair<GBC_KeyModifier, Action>(GBC_KeyModifier.None, delegate { ChangeKeyboardLayout(EnglishLayout, Locks.KeyboardCapsLockOn); }),
                new KeyValuePair<GBC_KeyModifier, Action>(GBC_KeyModifier.Shift, delegate { ChangeKeyboardLayout(RussianLayout, Locks.None); })
            };

            CapsLanguageSwitchController = new GlobalBindController(Keys.CapsLock,
                GlobalBindController.BindMethod.Hook,
                GlobalBindController.HookBehaviour.Replacement,
                clModifierPairList);
            //------------------------------------------------------------------------------
            chromeAddressBar = new BABLanguageSwitcher(InternetBrowser.GoogleChrome);
            #endregion
            //create WebSkypeSwitcher objects for different browsers
            CreateWSSobjects();

            //Check last settings
            ReadSettings();

            Interop.DefineDosDevice(Const.DddRawTargetPath, "keyboard", "\\Device\\KeyboardClass0");
            IOCTL_KEYBOARD_SET_INDICATORS =
               ControlCode(Const.FileDeviceKeyboard, 0x0002, Const.MethodBuffered, Const.FileAnyAccess);

            #region Init Timers
            //main timer
            mainTimer = new System.Windows.Forms.Timer();
            mainTimer.Tick += new System.EventHandler(this.MainTimerCallback);
            mainTimer.Interval = 2000;
            mainTimer.Start();
            #endregion

            DisableAllFunc();
            BlockWinKeyFunc();
            CapsLangFunc();
            WebSkypeFunc();

            notifyIcon1.MouseClick += new MouseEventHandler(notifyIcon1_MouseClick);
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            //change menu items
            InitMenuItems();
        }

        private void ReadSettings()
        {
            //1. Check WoW process name setting
            Tools.CheckRegistrySettings(ref wowProcessName, Const.WOW_PROCESS_KEY_NAME, Const.SETTINGS_LOCATION, Const.DEFAUL_WOW_PROCESS_NAME);
            //2. Check browser process name setting 
            //Tools.CheckRegistrySettings(ref browserProcessName, Const.BROWSER_PROCESS_KEYNAME, Const.SETTINGS_LOCATION, Const.DEFAULT_BROWSER_PROCESS_NAME);
            //3. Check block windows key setting
            Tools.CheckRegistrySettings(ref blockWinKey, Const.BLOCK_WINKEY, Const.SETTINGS_LOCATION, false);
            //4. Check disable all buttons key setting
            Tools.CheckRegistrySettings(ref disableAll, Const.DISABLE_ALL, Const.SETTINGS_LOCATION, false);
            //5. Check Chrome address bar language setting 
            chromeAddressBar.Enabled = Tools.CheckRegistrySettings(chromeAddressBar.Enabled, Const.ADR_BAR_LANG, Const.SETTINGS_LOCATION, false);
            //6.Capsswitch name Setting
            Tools.CheckRegistrySettings(ref capsSwitchEnable, Const.CAPS_SWITCH, Const.SETTINGS_LOCATION, false);
            //7. Web skype tab switcher
            WssEnable = Tools.CheckRegistrySettings(WssEnable, Const.WSS, Const.SETTINGS_LOCATION, false);
            Tools.CheckRegistrySettings(ref wssChromeEnable, Const.WSS_CHROME, Const.SETTINGS_LOCATION, true);
            Tools.CheckRegistrySettings(ref wssOperaEnable, Const.WSS_OPERA, Const.SETTINGS_LOCATION, false);
            Tools.CheckRegistrySettings(ref wssFirefoxEnable, Const.WSS_FIREFOX, Const.SETTINGS_LOCATION, false);
            Tools.CheckRegistrySettings(ref wssTorEnable, Const.WSS_TOR, Const.SETTINGS_LOCATION, false);
            Tools.CheckRegistrySettings(ref wssIEEnable, Const.WSS_IE, Const.SETTINGS_LOCATION, false);
            //8. Web skype bind and modifier for Google Chrome
            wssChrome.Bind = Tools.WssBindCheck(Const.WEBSKYPE_KEY_CHROME, Const.SETTINGS_LOCATION, Const.WEBSKYPE_DEFAULT_BIND);
            wssChrome.Modifiers = Tools.WssModCheck(Const.WEBSKYPE_MODIFIER_CHROME, Const.SETTINGS_LOCATION, Const.WEBSKYPE_DEFAULT_MODIFIER);
            //9. Web skype bind and modifier for Opera
            wssOpera.Bind = Tools.WssBindCheck(Const.WEBSKYPE_KEY_OPERA, Const.SETTINGS_LOCATION, Const.WEBSKYPE_DEFAULT_BIND);
            wssOpera.Modifiers = Tools.WssModCheck(Const.WEBSKYPE_MODIFIER_OPERA, Const.SETTINGS_LOCATION, Const.WEBSKYPE_DEFAULT_MODIFIER);
            //10. Web skype bind and modifier for Firefox
            wssFirefox.Bind = Tools.WssBindCheck(Const.WEBSKYPE_KEY_FIREFOX, Const.SETTINGS_LOCATION, Const.WEBSKYPE_DEFAULT_BIND);
            wssFirefox.Modifiers = Tools.WssModCheck(Const.WEBSKYPE_MODIFIER_FIREFOX, Const.SETTINGS_LOCATION, Const.WEBSKYPE_DEFAULT_MODIFIER);
            //11. Web skype bind and modifier for Tor Browser
            wssTorBrowser.Bind = Tools.WssBindCheck(Const.WEBSKYPE_KEY_TOR, Const.SETTINGS_LOCATION, Const.WEBSKYPE_DEFAULT_BIND);
            wssTorBrowser.Modifiers = Tools.WssModCheck(Const.WEBSKYPE_MODIFIER_TOR, Const.SETTINGS_LOCATION, Const.WEBSKYPE_DEFAULT_MODIFIER);
            //12. Web skype bind and modifier for Internet Explorer
            wssIE.Bind = Tools.WssBindCheck(Const.WEBSKYPE_KEY_IE, Const.SETTINGS_LOCATION, Const.WEBSKYPE_DEFAULT_BIND);
            wssIE.Modifiers = Tools.WssModCheck(Const.WEBSKYPE_MODIFIER_IE, Const.SETTINGS_LOCATION, Const.WEBSKYPE_DEFAULT_MODIFIER);
        }
        private void InitMenuItems()
        {
            //1. block windows key
            MenuItemsCondition(blockWinKey, ref conditionToolStripMenuItem, "Unblock win key", "Block win key");
            //2. disable all keys
            MenuItemsCondition(disableAll, ref disableAllToolStripMenuItem, "Unblock all keys", "Block all keys");
            //3. google chrome address bar focus
            MenuItemsCondition(chromeAddressBar.Enabled, ref chromeAdrbarToolStripMenuItem, "Unblock URL bar lang", "Block URL bar lang");
            //4. capslock language switcher
            MenuItemsCondition(CapsLanguageSwitchController.Execute, ref capsToolStripMenuItem, "CapsSwitch enabled", "CapsSwitch disabled");
            //5. web skype switcher
            MenuItemsCondition(wssEnable, ref skypeTabToolStripMenuItem, "SkypeTabSwitch enabled", "SkypeTabSwitch disabled");
            //6. wss google chrome 
            MenuItemsCondition(wssChromeEnable, ref googleChromeToolStripMenuItem, "Google Chrome", "Google Chrome");
            //7. wss opera
            MenuItemsCondition(wssOperaEnable, ref operaToolStripMenuItem, "Opera", "Opera");
            //8. wss firefox 
            MenuItemsCondition(wssFirefoxEnable, ref firefoxToolStripMenuItem, "Firefox", "Firefox");
            //9. wss tor browser 
            MenuItemsCondition(wssTorEnable, ref torBrowserToolStripMenuItem, "Tor Browser", "Tor Browser");
            //10. wss internet explorer
            MenuItemsCondition(wssIEEnable, ref internetExplorerToolStripMenuItem, "Internet Explorer", "Internet Explorer");
            //11. wss gooogle chrome bind and modifier
            MenuItemsBind(wssChrome.Bind, wssChrome.Modifiers, ref gcBindControl);
            //12. wss opera bind and modifier
            MenuItemsBind(wssOpera.Bind, wssOpera.Modifiers, ref oBindControl);
            //13. wss firefox bind and modifier
            MenuItemsBind(wssFirefox.Bind, wssFirefox.Modifiers, ref ffBindControl);
            //14. wss tor browser bind and modifier
            MenuItemsBind(wssTorBrowser.Bind, wssTorBrowser.Modifiers, ref tBindControl);
            //15. wss internet explorer bind and modifier
            MenuItemsBind(wssIE.Bind, wssIE.Modifiers, ref ieBindControl);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="menuItem"></param>
        /// <param name="enabledText"></param>
        /// <param name="disabledText"></param>
        private void MenuItemsCondition(string condition, ref ToolStripMenuItem menuItem, string enabledText, string disabledText)
        {
            if (condition == "true")
            {
                menuItem.Checked = true;
                menuItem.Text = enabledText;
            }
            else
            {
                menuItem.Checked = false;
                menuItem.Text = disabledText;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="menuItem"></param>
        /// <param name="enabledText"></param>
        /// <param name="disabledText"></param>
        private void MenuItemsCondition(bool condition, ref ToolStripMenuItem menuItem, string enabledText, string disabledText)
        {
            if (condition == true)
            {
                menuItem.Checked = true;
                menuItem.Text = enabledText;
            }
            else
            {
                menuItem.Checked = false;
                menuItem.Text = disabledText;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mod"></param>
        /// <param name="menuItem"></param>
        private void MenuItemsBind(Keys key, List<GBC_KeyModifier> mod, ref Controls.BindControl menuItem)
        {
            menuItem.Bind = key;
            if (mod != null && mod.Count > 0)
                menuItem.Modifier = mod.First();
        }

        private IntPtr captureWinKey(int nCode, IntPtr wp, IntPtr lp)
        {

            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.LWin || objKeyInfo.key == Keys.RWin) //вот тут и блокируется WinKey {      
                    return (IntPtr)1;
            }
            return Interop.CallNextHookEx(ptrHookWinKey, nCode, wp, lp);
        }

        private void DisableAllFunc()
        {
            if (disableAll == true && !disableAllGotHook)
            {
                HookManager.KeyBlock += HookManager_KeyBlock;
                disableAllGotHook = true;
            }
            else if (disableAll == false && disableAllGotHook)
            {
                HookManager.KeyBlock -= HookManager_KeyBlock;
                disableAllGotHook = false;
            }
        }
        private void BlockWinKeyFunc()
        {
            if (disableAll == false && blockWinKey == true && wowProcess != null && ptrHookWinKey == IntPtr.Zero && Tools.GetPlacement(wowProcess.MainWindowHandle).showCmd != ShowWindowCommands.Minimized)
            {
                ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
                modifierKeyboardProcess = new Interop.LowLevelKeyboardProc((int nCode, IntPtr wp, IntPtr lp) =>
                {
                    if (nCode >= 0)
                    {
                        KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));
                        if (objKeyInfo.key == Keys.LWin || objKeyInfo.key == Keys.RWin) //вот тут и блокируется WinKey {      
                            return (IntPtr)1;
                    }
                    return Interop.CallNextHookEx(ptrHookWinKey, nCode, wp, lp);
                });
                ptrHookWinKey = Interop.SetWindowsHookEx(13, modifierKeyboardProcess, Interop.GetModuleHandle(objCurrentModule.ModuleName), 0);
            }
            else if (disableAll == false && wowProcess == null && ptrHookWinKey != IntPtr.Zero)
            {
                Interop.UnhookWindowsHookEx(ptrHookWinKey);
                ptrHookWinKey = IntPtr.Zero;
                //Application.Exit();
            }
            else if (disableAll == false && wowProcess != null && Tools.GetPlacement(wowProcess.MainWindowHandle).showCmd == ShowWindowCommands.Minimized)
            {
                Interop.UnhookWindowsHookEx(ptrHookWinKey);
                ptrHookWinKey = IntPtr.Zero;
            }
            else if (disableAll == false && blockWinKey == false && ptrHookWinKey != IntPtr.Zero)
            {
                Interop.UnhookWindowsHookEx(ptrHookWinKey);
                ptrHookWinKey = IntPtr.Zero;
            }
        }

        private void CapsLangFunc()
        {
            CapsLanguageSwitchController.Execute = capsSwitchEnable ? true : false;
        }
        private void WebSkypeFunc()
        {
            wssChrome.IsActive = wssEnable && wssChromeEnable ? true : false;
            wssOpera.IsActive = wssEnable && wssOperaEnable ? true : false;
            wssFirefox.IsActive = wssEnable && wssFirefoxEnable ? true : false;
            wssTorBrowser.IsActive = wssEnable && wssTorEnable ? true : false;
            wssIE.IsActive = wssEnable && wssIEEnable ? true : false;

            if (wssEnable && Tools.AnyIsTrue(wssChromeEnable, wssOperaEnable, wssFirefoxEnable, wssTorEnable, wssIEEnable))
                AltTabSimulator.Instance.Start();
            if (!wssEnable || !Tools.AnyIsTrue(wssChromeEnable, wssOperaEnable, wssFirefoxEnable, wssTorEnable, wssIEEnable))
                AltTabSimulator.Instance.Suspend();
            WebSkypeInit();
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------

        private void MainTimerCallback(object sender, EventArgs e)
        {
            wowProcess = null;
            try
            {
                if (blockWinKey == true)
                {
                    allProcesses = Process.GetProcessesByName(wowProcessName);
                    if (allProcesses.Length != 0)
                    {
                        wowProcess = allProcesses.First();
                        globalWowProcess = wowProcess;
                    }
                }
                // testing current keyboard layout and toggle capslock LED on or off
                //if (CapsLanguageSwitchController.Execute == true)
                //{
                //    uint tpid = Interop.GetWindowThreadProcessId(Interop.GetForegroundWindow(), IntPtr.Zero);
                //    IntPtr hKL = Interop.GetKeyboardLayout(tpid);
                //    hKL = (IntPtr)(hKL.ToInt32() & 0x0000FFFF);
                //    if (hKL == (IntPtr)Const.ENG_LANG_KEYB_LAYOUT)
                //        ToggleLights(Locks.KeyboardCapsLockOn);
                //    else
                //        ToggleLights(Locks.None);
                //}
                ////
            }
            catch (Exception ex)
            { Console.WriteLine("Form1.1 :" + ex.Message); }
            // MessageBox.Show(GetPlacement(wowProcess.MainWindowHandle).showCmd.ToString());testWatch
        }
        #region old code
        //private void cromeURLcheck_Tick(object sender, EventArgs e)
        //{
        //    // testWatch.Start();
        //    #region automatic search, but too slow

        //    //Process[] procsChrome = Process.GetProcessesByName("chrome");
        //    //foreach (Process chrome in procsChrome)
        //    //{
        //    //        // the chrome process must have a window
        //    //        if (chrome.MainWindowHandle == IntPtr.Zero)
        //    //        {
        //    //            continue;
        //    //        };

        //    //        AutomationElement lync = AutomationElement.FromHandle(chrome.MainWindowHandle);
        //    //        AutomationElement urlBar = lync.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
        //    //        if (urlBar == null)
        //    //        {
        //    //          // it's not..
        //    //          continue;
        //    //        }
        //    //        if (urlBar.Current.HasKeyboardFocus)
        //    //        {
        //    //            Application.Exit();
        //    //        }
        //    #endregion
        //    #region faster one (mannually for Chrome Version 43.0.2357.65 m)
        //    // there are always multiple chrome processes, so we have to loop through all of them to find the
        //    // process with a Window Handle and an automation element of name "Address and search bar"
        //    Process[] procsChrome = Process.GetProcessesByName("chrome");
        //    foreach (Process chrome in procsChrome)
        //    {
        //        // the chrome process must have a window
        //        if (chrome.MainWindowHandle == IntPtr.Zero)
        //        {
        //            continue;
        //        }

        //        // find the automation element
        //        AutomationElement lync = AutomationElement.FromHandle(chrome.MainWindowHandle);
        //        // manually walk through the tree, searching using TreeScope.Descendants is too slow (even if it's more reliable)
        //        AutomationElement urlBar = null;
        //        try
        //        {
        //            // walking path found using inspect.exe (Windows SDK) for Chrome Version 43.0.2357.65 m (currently the latest stable)
        //            var lyncDaughter = lync.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));
        //            if (lyncDaughter == null) { continue; }  // not the right chrome.exe
        //            // here, you can optionally check if Incognito is enabled:
        //            //bool bIncognito = TreeWalker.RawViewWalker.GetFirstChild(TreeWalker.RawViewWalker.GetFirstChild(elm1)) != null;
        //            var lyncGranddoughter = TreeWalker.RawViewWalker.GetLastChild(lyncDaughter);
        //            var lyncGreatgranddoughter = lyncGranddoughter.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, ""))[1];
        //            var lyncGreatgreatgranddoughter = lyncGreatgranddoughter.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
        //            var lyncGreatgreatgreatgranddoughter = lyncGreatgreatgranddoughter.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, ""));
        //            urlBar = lyncGreatgreatgreatgranddoughter.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Form1.2");
        //            continue;
        //        }

        //        if (urlBar == null)
        //        {
        //            // it's not..
        //            continue;
        //        }
        //        if (urlBar.Current.HasKeyboardFocus)
        //        {
        //            IntPtr fore = Interop.GetForegroundWindow();
        //            uint tpid = Interop.GetWindowThreadProcessId(fore, IntPtr.Zero);
        //            IntPtr hKL = Interop.GetKeyboardLayout(tpid);
        //            hKL = (IntPtr)(hKL.ToInt32() & 0x0000FFFF);
        //            if (hKL != EngLangKeybLayout)
        //            {
        //                lastGoogleKeybLayout = hKL;
        //                Interop.PostMessage(chrome.MainWindowHandle, 0x0050, (IntPtr)2, IntPtr.Zero);
        //            }

        //        }
        //    #endregion
        //    }


        //    if (testWatch.IsRunning)
        //        testWatchTime = testWatch.Elapsed;
        //    testWatch.Stop();

        //}
        #endregion

        private void WebSkypeInit()
        {
            //google chrome
            try
            {
                if (wssEnable && wssChromeEnable)
                {
                    chromeUIProcess.FillProcessData(Tools.UIProcess(Const.CHROME_PROCESS_NAME));
                    HookManager.chromeProcess = chromeUIProcess.Process;
                    wssChrome.BrowserData = chromeUIProcess;
                }
                wssChrome.Update();
            }
            catch (Exception ex)
            { Console.WriteLine("Form1.WebSkypeInit " + ex.Message); }
            //opera
            try
            {
                if (wssEnable && wssOperaEnable)
                {
                    operaUIProcess.FillProcessData(Tools.UIProcess(Const.OPERA_PROCESS_NAME));
                    HookManager.operaProcess = operaUIProcess.Process;
                    wssOpera.BrowserData = operaUIProcess;
                }
                wssOpera.Update();
            }
            catch (Exception ex)
            { Console.WriteLine("Form1.WebSkypeInit2 " + ex.Message); }
        }

        private uint ControlCode(uint deviceType, uint function, uint method, uint access)
        {
            return ((deviceType) << 16) | ((access) << 14) | ((function) << 2) | (method);
        }

        /// <summary>
        /// http://www.aboutmycode.com/miscellaneous/faking-num-lock-caps-lock-and-scroll-lock-leds/
        /// </summary>
        /// <param name="locks"></param>
        private void ToggleLights(Locks locks)
        {
            Interop.DefineDosDevice(Const.DddRawTargetPath, "keyboard", "\\Device\\KeyboardClass0");

            var indicators = new KeyboardIndicatorParameters();

            try
            {
                // тут на вин 10 происхордит что-то не ладное, функция отказывается работать, все время возвращает -1
                var hKeybd = Interop.CreateFile("\\\\.\\keyboard", FileAccess.Write, 0, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

                indicators.UnitId = 0;
                indicators.LedFlags = locks;

                var size = Marshal.SizeOf(typeof(KeyboardIndicatorParameters));

                uint bytesReturned;
                Interop.DeviceIoControl(hKeybd, IOCTL_KEYBOARD_SET_INDICATORS, ref indicators, (uint)size,
                    IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);
            }
            catch (Exception ex) { Console.WriteLine("Form1.ToggleLights"); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keybLayout"></param>
        /// <param name="locks"></param>
        private void ChangeKeyboardLayout(LanguageLayout keybLayout, Locks locks)
        {
            IntPtr fore = Interop.GetForegroundWindow();
            uint tpid = Interop.GetWindowThreadProcessId(fore, IntPtr.Zero);
            IntPtr hKL = Interop.GetKeyboardLayout(tpid);
            hKL = (IntPtr)(hKL.ToInt32() & 0x0000FFFF);
            if (hKL != keybLayout.KeyboardLayout)
            {
                //https://msdn.microsoft.com/en-us/library/windows/desktop/ms632630(v=vs.85).aspx
                Interop.PostMessage(fore, Const.WM_INPUTLANGCHANGEREQUEST, (IntPtr)0x0001, (IntPtr)Interop.LoadKeyboardLayout(keybLayout.pwszKLID, Const.KLF_ACTIVATE));
                ToggleLights(locks);
            }
        }

        private void changeWindow(uint status, IntPtr hWnd)
        {
            IntPtr targetProcess = Interop.GetWindow(hWnd, status); // Get target process with status relative to the main window
            while (true) // Loop for each processes
            {
                IntPtr parentProces = Interop.GetParent(targetProcess); // Get parent process of the target process
                if (parentProces.Equals(IntPtr.Zero)) // No partent process of the target process
                    break; // Target process is the last one, it has the desired status
                targetProcess = parentProces; // Parent process of the current target process becomes the new target process
            }
            Interop.SetForegroundWindow(targetProcess); // Set target process to the foreground
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aElem"></param>
        /// <returns></returns>
        private string WebSkypeTabName(AutomationElement aElem)
        {
            string tabName;
            tabName = "Skype - Google Chrome";
            if (aElem.Current.Name == tabName)
                return tabName;

            for (int i = 1; i <= Const.MAX_ALLOWED_MESSAGES; i++)
            {
                tabName = String.Format("({0}) ", i) + "Skype - Google Chrome";
                if (aElem.Current.Name == tabName)
                    return tabName;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabsParent"></param>
        /// <param name="webSkypeTab"></param>
        /// <returns></returns>
        private int TabNumber(AutomationElement tabsParent, AutomationElement webSkypeTab)
        {
            var tabCollection = tabsParent.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem));
            int tabNumber = 1;
            foreach (AutomationElement child in tabCollection)
            {
                if (child == webSkypeTab || tabNumber > 8)
                    break;
                tabNumber++;
            }
            return tabNumber;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabsParent"></param>
        /// <returns></returns>
        private int CurrentTabKey(AutomationElement tabsParent)
        {
            var tabCollection = tabsParent.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem));
            int tabNumber = 1;
            foreach (AutomationElement child in tabCollection)
            {
                if (child.Current.HasKeyboardFocus)
                    break;
                tabNumber++;
            }
            return tabNumber;
        }

        //public void SimilateMouseClick(Point p)
        //{
        //    //Call the imported function with the cursor's current position
        //    Interop.SendMessage(Interop.WindowFromPoint(p), Const.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
        //}


        private IntPtr GetPreviousWindow()
        {
            IntPtr activeAppWindow = Interop.GetForegroundWindow();
            if (activeAppWindow == IntPtr.Zero)
                return IntPtr.Zero;

            IntPtr prevAppWindow = Interop.GetLastActivePopup(activeAppWindow);
            return Interop.IsWindowVisible(prevAppWindow) ? prevAppWindow : IntPtr.Zero;
        }

        /// <summary>
        /// 
        /// </summary>
        private void FocusToPreviousWindow()
        {
            IntPtr prevWindow = GetPreviousWindow();
            if (prevWindow != IntPtr.Zero)
                Interop.SetForegroundWindow(prevWindow);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetForegroundLastWindow()
        {
            IntPtr lastWindowHandle = Interop.GetWindow(Process.GetCurrentProcess().MainWindowHandle, (uint)GetWindow_Cmd.GW_HWNDNEXT);
            while (true)
            {
                IntPtr temp = Interop.GetParent(lastWindowHandle);
                if (temp.Equals(IntPtr.Zero)) break;
                lastWindowHandle = temp;
            }
            Interop.SetForegroundWindow(lastWindowHandle);
        }

        private void ChangeNotifyIcon(bool isNewMessage)
        {
            this.notifyIcon1.Icon = isNewMessage ? ((System.Drawing.Icon)(resources.GetObject("winlogo_with_message"))) : ((System.Drawing.Icon)(resources.GetObject("winlogo_blue2")));
        }

        private void CreateWSSobjects()
        {
            #region Google Chrome

            InternetBrowserData browserData = new InternetBrowserData(InternetBrowser.GoogleChrome);
            wssChrome = new WebSkypeSwitcher(browserData,
                Const.WEBSKYPE_DEFAULT_BIND,
                new List<GBC_KeyModifier> { GBC_KeyModifier.Alt },
                GlobalBindController.BindMethod.RegisterHotKey,
                GlobalBindController.HookBehaviour.Replacement);

            wssChrome.GotNewMessage += webSkypeSwitcher_GotNewMessage;
            #endregion
            #region Opera
            browserData = new InternetBrowserData(InternetBrowser.Opera);
            wssOpera = new WebSkypeSwitcher(browserData,
                Const.WEBSKYPE_DEFAULT_BIND,
                new List<GBC_KeyModifier> { GBC_KeyModifier.Alt },
                GlobalBindController.BindMethod.RegisterHotKey,
                GlobalBindController.HookBehaviour.Replacement);

            wssOpera.GotNewMessage += webSkypeSwitcher_GotNewMessage;
            #endregion
            #region Firefox
            browserData = new InternetBrowserData(InternetBrowser.Firefox);
            wssFirefox = new WebSkypeSwitcher(browserData,
                Const.WEBSKYPE_DEFAULT_BIND,
                new List<GBC_KeyModifier> { GBC_KeyModifier.Alt },
                GlobalBindController.BindMethod.RegisterHotKey,
                GlobalBindController.HookBehaviour.Replacement);

            wssFirefox.GotNewMessage += webSkypeSwitcher_GotNewMessage;
            #endregion
            #region Tor Browser
            browserData = new InternetBrowserData(InternetBrowser.TorBrowser);
            wssTorBrowser = new WebSkypeSwitcher(browserData,
                Const.WEBSKYPE_DEFAULT_BIND,
                new List<GBC_KeyModifier> { GBC_KeyModifier.Alt },
                GlobalBindController.BindMethod.RegisterHotKey,
                GlobalBindController.HookBehaviour.Replacement);
            wssTorBrowser.GotNewMessage += webSkypeSwitcher_GotNewMessage;
            #endregion
            #region Internet Explorer
            browserData = new InternetBrowserData(InternetBrowser.InternetExplorer);
            wssIE = new WebSkypeSwitcher(browserData,
                Const.WEBSKYPE_DEFAULT_BIND,
                new List<GBC_KeyModifier> { GBC_KeyModifier.Alt },
                GlobalBindController.BindMethod.RegisterHotKey,
                GlobalBindController.HookBehaviour.Replacement);

            wssIE.GotNewMessage += webSkypeSwitcher_GotNewMessage;
            #endregion
        }


        //-----------------------------------------------------------------------------------------------------
        private void Form1_Load(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                Hide();
            }));

        }
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip1.Show();
            if (e.Button == MouseButtons.Left && wssEnable)
            {
                Tools.SimulateBindActionOnce(wssChrome);
                Tools.SimulateBindActionOnce(wssOpera);
                Tools.SimulateBindActionOnce(wssFirefox);
                Tools.SimulateBindActionOnce(wssTorBrowser);
                Tools.SimulateBindActionOnce(wssIE);
            }
        }




        #region -----------------Menu items click---------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuItem"></param>
        /// <param name="itemText1"></param>
        /// <param name="itemText2"></param>
        /// <param name="func1"></param>
        /// <param name="func2"></param>
        private void ToolStripMenuItemsConditionChanger(ToolStripMenuItem menuItem, string itemText1, string itemText2, Action func1, Action func2)
        {
            if (!menuItem.Checked)
            {
                menuItem.Text = itemText1;
                func1();
            }
            else
            {
                menuItem.Text = itemText2;
                func2();
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //HookManager.KeyBlock -= HookManager_KeyBlock;
            Application.Exit();
        }

        private void conditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemsConditionChanger(conditionToolStripMenuItem, "Block win key", "Unblock win key",
                delegate { Tools.SaveRegistrySettings(ref blockWinKey, Const.BLOCK_WINKEY, Const.SETTINGS_LOCATION, false); },
                delegate { Tools.SaveRegistrySettings(ref blockWinKey, Const.BLOCK_WINKEY, Const.SETTINGS_LOCATION, true); });
            BlockWinKeyFunc();
        }

        private void disableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemsConditionChanger(disableAllToolStripMenuItem, "Block all keys", "Unblock all keys",
                delegate { Tools.SaveRegistrySettings(ref disableAll, Const.DISABLE_ALL, Const.SETTINGS_LOCATION, false); },
                delegate { Tools.SaveRegistrySettings(ref disableAll, Const.DISABLE_ALL, Const.SETTINGS_LOCATION, true); });
            DisableAllFunc();
        }

        private void chromeAdrbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemsConditionChanger(chromeAdrbarToolStripMenuItem, "Block URL bar", "Unblock URL bar lang",
                delegate { chromeAddressBar.Enabled = Tools.SaveRegistrySettings(chromeAddressBar.Enabled, Const.ADR_BAR_LANG, Const.SETTINGS_LOCATION, false); },
                delegate { chromeAddressBar.Enabled = Tools.SaveRegistrySettings(chromeAddressBar.Enabled, Const.ADR_BAR_LANG, Const.SETTINGS_LOCATION, true); });
        }

        private void capsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemsConditionChanger(capsToolStripMenuItem, "CapsSwitch Disabled", "CapsSwitch Enabled",
                delegate { Tools.SaveRegistrySettings(ref capsSwitchEnable, Const.CAPS_SWITCH, Const.SETTINGS_LOCATION, false); },
                delegate { Tools.SaveRegistrySettings(ref capsSwitchEnable, Const.CAPS_SWITCH, Const.SETTINGS_LOCATION, true); });
            CapsLangFunc();
        }

        private void skypeTabSwitchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItemsConditionChanger(skypeTabToolStripMenuItem, "SkypeTabSwitch disabled", "SkypeTabSwitch enabled",
                delegate { WssEnable = Tools.SaveRegistrySettings(WssEnable, Const.WSS, Const.SETTINGS_LOCATION, false); },
                delegate { WssEnable = Tools.SaveRegistrySettings(WssEnable, Const.WSS, Const.SETTINGS_LOCATION, true); });
            WebSkypeFunc();
        }

        void bindControl_SelectedBindChanged(object sender, EventArgs e, ref WebSkypeSwitcher wss, string wssKey, string wssKeyLocation)
        {
            if ((sender as Controls.BindControl).Bind == wss.Bind)
                return;
            Keys wssBindProperty = wss.Bind;
            Tools.SaveRegistrySettings(ref wssBindProperty, wssKey, wssKeyLocation, (sender as Controls.BindControl).Bind); // save bind
            wss.Bind = wssBindProperty;
        }

        void bindControl_SelectedModifierChanged(object sender, EventArgs e, ref WebSkypeSwitcher wss, string wssMod, string wssModLocation)
        {
            if ((sender as Controls.BindControl).Modifier == wss.Modifiers.First())
                return;
            var wssModProperty = wss.Modifiers;
            Tools.SaveRegistrySettings(ref wssModProperty, wssMod, wssModLocation, new List<GBC_KeyModifier> { (sender as Controls.BindControl).Modifier }); // save modifiers
            wss.Modifiers = wssModProperty;
        }

        private void browserMenuItem_Click(object sender, EventArgs e)
        {
            if (!(sender as ToolStripMenuItem).Checked)
            {
                Tools.UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
                (sender as ToolStripMenuItem).Checked = true;

            }
        }

        private void browserMenuItem_CheckedChanged(object sender, EventArgs e, ref bool wssActivity, string wssKeyName, string wssKeyLocation, string menuText)
        {
            bool wssKeyValue = wssActivity;

            ToolStripMenuItemsConditionChanger((ToolStripMenuItem)sender, menuText, menuText,
                            delegate { Tools.SaveRegistrySettings(ref wssKeyValue, wssKeyName, wssKeyLocation, false); },
                            delegate { Tools.SaveRegistrySettings(ref wssKeyValue, wssKeyName, wssKeyLocation, true); });
            wssActivity = wssKeyValue;
            WebSkypeFunc();
        }

        #endregion

        private void HookManager_KeyBlock(object sender, KeyPressEventArgs e)
        {
        }

        private void webSkypeSwitcher_GotNewMessage(WebSkypeSwitcher wss)
        {
            //change icon when recieve a message
            ChangeNotifyIcon(wss.NewMessage);
        }
        private void Form1_wssEnabledChanged(SystemProcessHookForm winWatcher, bool state)
        {
            if (state)
            {
                windowWatcher.WindowEvent += window_Created;
                //windowWatcher.WindowDestroyed += window_Destroyed;
            }
            else
            {
                windowWatcher.WindowEvent -= window_Created;
                //windowWatcher.WindowDestroyed -= window_Destroyed;
                //windowWatcher.Shutdown();
            }
        }

        //private void Form1_wssBrowserEnableChanged(WMI.Win32.ProcessWatcher pWatcher, bool state)
        //{
        //    if (state)
        //        pWatcher.Start();
        //    else
        //        pWatcher.Stop();
        //}
        //private void Form1_wssOperaEnableChanged()
        //{
        //}
        //private void Form1_wssFirefoxEnableChanged()
        //{
        //}
        //private void Form1_wssTorEnableChanged()
        //{
        //}
        //private void Form1_wssIEEnableChanged()
        //{
        //}
        //private void chromeProcess_Created(WMI.Win32.Win32_Process win32_proc)
        //{
        //    Console.WriteLine("Chrome Created");
        //    var procId = win32_proc.ProcessId;
        //    Process diag_Proc = Process.GetProcessById((int)procId);
        //    if (diag_Proc.MainWindowHandle != IntPtr.Zero)
        //        chromeUIProcess = diag_Proc;
        //    else
        //        return;
        //    if (chromeAdrbarFocusEnable == true || (wssEnable && wssChromeEnable))
        //    {
        //        HookManager.chromeProcess = chromeUIProcess;
        //        if (wssEnable && wssChromeEnable)
        //            wssChrome.BrowserData.process = chromeUIProcess;
        //    }
        //    ChromeUrlFunc(chromeUIProcess);
        //    wssChrome.Update();
        //}
        //private void chromeProcess_Deleted(WMI.Win32.Win32_Process win32_proc)
        //{
        //    Console.WriteLine("Chrome Deleted");

        //    if (win32_proc.ProcessId == chromeUIProcess.Id)
        //    {
        //        chromeUIProcess = null;
        //        HookManager.chromeProcess = null;
        //        ChromeUrlFunc(chromeUIProcess);
        //        wssChrome.Update();
        //    }
        //}
        //private void operaProcess_Created(WMI.Win32.Win32_Process win32_proc)
        //{
        //    Console.WriteLine("Opera Created");

        //    var procId = win32_proc.ProcessId;
        //    Process diag_Proc = Process.GetProcessById((int)procId);
        //    if (diag_Proc.MainWindowHandle != IntPtr.Zero)
        //        operaUIProcess = diag_Proc;
        //    else
        //        return;
        //    if (operaAdrbarFocusEnable == true || (wssEnable && wssOperaEnable))
        //    {
        //        HookManager.operaProcess = operaUIProcess;
        //        if (wssEnable && wssOperaEnable)
        //            wssOpera.BrowserData.process = operaUIProcess;
        //    }
        //    //OperaUrlFunc(operaUIProcess);
        //    wssOpera.Update();

        //}
        //private void operaProcess_Deleted(WMI.Win32.Win32_Process win32_proc)
        //{
        //    Console.WriteLine("Opera Deleted");

        //    if (win32_proc.ProcessId == operaUIProcess.Id)
        //    {
        //        operaUIProcess = null;
        //        HookManager.operaProcess = null;
        //        //OperaUrlFunc(operaUIProcess);
        //        wssOpera.Update();
        //    }
        //}
        private void window_Created(object sender, IntPtr hWnd, Interop.ShellEvents shell)
        {
            if (shell == Interop.ShellEvents.HSHELL_WINDOWDESTROYED)
            {
                if (hWnd == chromeUIProcess.MainWindowHandle || hWnd == operaUIProcess.MainWindowHandle || hWnd == firefoxUIProcess.MainWindowHandle || hWnd == torUIProcess.MainWindowHandle || hWnd == ieUIProcess.MainWindowHandle)
                    WebSkypeInit();
            }
            if (shell == Interop.ShellEvents.HSHELL_WINDOWCREATED)
            {
                //chrome and opera has a same class name
                if (Tools.GetClassName(hWnd) == Const.CHROME_CLASS_NAME)
                {
                    switch (Tools.GetProcessName(hWnd))
                    {
                        case Const.CHROME_PROCESS_NAME:
                            chromeUIProcess.FillProcessData(Tools.GetProcess(hWnd));
                            if (wssEnable && wssChromeEnable)
                            {
                                HookManager.chromeProcess = chromeUIProcess.Process;
                                wssChrome.BrowserData = chromeUIProcess;
                            }
                            wssChrome.Update();
                            break;
                        case Const.OPERA_PROCESS_NAME:
                            operaUIProcess.FillProcessData(Tools.GetProcess(hWnd));
                            if (wssEnable && wssOperaEnable)
                            {
                                HookManager.operaProcess = operaUIProcess.Process;
                                wssOpera.BrowserData = operaUIProcess;
                            }
                            wssOpera.Update();
                            break;
                    }
                }
            }

            #region with WindowHookNet
            ////chrome and opera has a same class name
            //if (aArgs.WindowClass == Const.CHROME_CLASS_NAME)
            //{
            //    switch (aArgs.ProcessName)
            //    {
            //        case Const.CHROME_PROCESS_NAME:
            //            chromeUIProcess = aArgs.Process;

            //            if (chromeAdrbarFocusEnable == true || (wssEnable && wssChromeEnable))
            //            {
            //                HookManager.chromeProcess = chromeUIProcess;
            //                if (wssEnable && wssChromeEnable)
            //                    wssChrome.BrowserData.process = chromeUIProcess;
            //            }
            //            ChromeUrlFunc(chromeUIProcess);
            //            wssChrome.Update();
            //            break;
            //        case Const.OPERA_PROCESS_NAME:
            //            operaUIProcess = aArgs.Process;

            //            if (operaAdrbarFocusEnable == true || (wssEnable && wssOperaEnable))
            //            {
            //                HookManager.operaProcess = operaUIProcess;
            //                if (wssEnable && wssOperaEnable)
            //                    wssOpera.BrowserData.process = operaUIProcess;
            //            }
            //            //OperaUrlFunc(operaUIProcess);
            //            wssOpera.Update();
            //            break;
            //    }
            //}
            #endregion
        }
        private void window_Destroyed(object sender, WindowHookEventArgs aArgs)
        {
            #region with WindowHookNet
            //chrome and opera has a same class name
            //if (aArgs.WindowClass == Const.CHROME_CLASS_NAME)
            //{
            //    switch (aArgs.ProcessName)
            //    {
            //        case Const.CHROME_PROCESS_NAME:
            //            //Check if chrome process still exists
            //            if (Interop.GetWindowThreadProcessId(aArgs.Handle, IntPtr.Zero) == 0)
            //            {
            //                chromeUIProcess = null;
            //                HookManager.chromeProcess = null;
            //                ChromeUrlFunc(chromeUIProcess);
            //                wssChrome.Update();
            //            }
            //            break;
            //        case Const.OPERA_PROCESS_NAME:
            //            //Check if opera process still exists
            //            if (Interop.GetWindowThreadProcessId(aArgs.Handle, IntPtr.Zero)==0)
            //            {
            //                operaUIProcess = null;
            //                HookManager.operaProcess = null;
            //                //OperaUrlFunc(operaUIProcess);
            //                wssOpera.Update();
            //            }
            //            break;
            //    }
            //}
            #endregion
        }

        #region use Win32 API to send window messages and thus get control properties or invoke events



        //Get Text property of UI element using Win32 API  
        //public static string GetText(AutomationElement element)
        //{
        //    //Get handle property of the control 
        //    int wndHandle = (int)element.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty);
        //    StringBuilder title = new StringBuilder();
        //    // Get the size of the string required to hold the window title. 
        //    Int32 size = SendMessage(wndHandle, Const.WM_GETTEXTLENGTH, 0, 0).ToInt32();
        //    // If the return is 0, there is no title. 
        //    if (size > 0)
        //    {
        //        title = new StringBuilder(size + 1);
        //        SendMessage(new IntPtr(wndHandle), Const.WM_GETTEXT, title.Capacity, title);
        //    }
        //    return title.ToString();
        //}

        #endregion
        private void OnApplicationExit(object sender, EventArgs e)
        {
        }

        public static void TestingThreadProc()
        {

        }

    }


    public class LanguageLayout
    {
        public IntPtr KeyboardLayout { get; set; }
        public string pwszKLID { get; set; }

        private readonly IntPtr EngLangKeybLayout = (IntPtr)1033;
        private readonly IntPtr RusLangKeybLayout = (IntPtr)1049;

        public enum LayoutEnum
        {
            En = 0,
            Ru = 1,
        }

        public LanguageLayout(LayoutEnum kl)
        {
            if (kl == LayoutEnum.En)
            {
                KeyboardLayout = EngLangKeybLayout;
                pwszKLID = "00000409";
            }
            if (kl == LayoutEnum.Ru)
            {
                KeyboardLayout = RusLangKeybLayout;
                pwszKLID = "00000419";
            }
        }
    }


    //public class ConditionMatcher : IMatchConditions
    //{
    //    public bool Matches(AutomationElement element, Condition condition)
    //    {
    //        return new TreeWalker(condition).Normalize(element) != null;
    //    }
    //}
}
