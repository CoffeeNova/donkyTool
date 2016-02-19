using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Gma.UserActivityMonitor;
using System.Windows.Automation;

namespace wowDisableWinKey
{
    using Tools = WowDisableWinKeyTools;

    //Browser Address Bar Language Switcher
    public class BABLanguageSwitcher
    {
        private bool adrbarGotHook = false;
        private InternetBrowser browser;
        private IntPtr lastKeybLayout;
        //private List<AutomationElement> addressBarAE;
        private InternetBrowserData uiProcess;
        //private SystemProcessHookForm windowWatcher;

        //private delegate void WssDelegate(SystemProcessHookForm winWatcher, bool state);
        //private event WssDelegate enabledChanged;

        private bool enabled = false;
        public bool Enabled //(chromeAdrbarFocusEnable)
        {
            get { return enabled; }
            set
            {
                if (value != enabled)
                {
                    enabled = value;
                    EnabledInit();
                   // enabledChanged(windowWatcher, value);
                }
            }
        }
        public BABLanguageSwitcher(InternetBrowser ib)
        {
            browser = ib;
            uiProcess = new InternetBrowserData(ib);
            //windowWatcher = new SystemProcessHookForm();
            //enabledChanged += BABLanguageSwitcher_enabledChanged;
        }

        ~BABLanguageSwitcher()
        {
            Enabled = false;
        }
        private void UrlFunc(Process prc)
        {
            if (enabled && prc != null && !adrbarGotHook)
            {
                switch (browser)
                {
                    case InternetBrowser.GoogleChrome:
                        HookManager.GoogleGotFocus += (sender, e) => HookManager_BrowserGotFocus(sender, e, prc);
                        //init chrome address bars list (in case with many windows)
                        //addressBarAE = SearchChromeAdressBarAE(prc.Id);
                        break;
                    case InternetBrowser.Opera:
                    case InternetBrowser.Firefox:
                    case InternetBrowser.TorBrowser:
                    case InternetBrowser.InternetExplorer:
                        break;
                }
                adrbarGotHook = true;
            }
            else if ((!enabled || prc == null) && adrbarGotHook)
            {

                switch (browser)
                {
                    case InternetBrowser.GoogleChrome:
                        HookManager.GoogleGotFocus -= (sender, e) => HookManager_BrowserGotFocus(sender, e, prc);
                        //addressBarAE.Clear();
                        break;
                    case InternetBrowser.Opera:
                    case InternetBrowser.Firefox:
                    case InternetBrowser.TorBrowser:
                    case InternetBrowser.InternetExplorer:
                        break;
                }
                adrbarGotHook = false;
            }
        }
        private void HookManager_BrowserGotFocus(Object sender, EventArgs e, Process prc)
        {
            //if (!addressBarAE.Any((ae) => ae.Current.HasKeyboardFocus))
            //    return;
            //тут меняем язык на инглишь, какой бы он не был там
            IntPtr fore = Interop.GetForegroundWindow();
            uint tpid = Interop.GetWindowThreadProcessId(fore, IntPtr.Zero);
            IntPtr hKL = Interop.GetKeyboardLayout(tpid);
            hKL = (IntPtr)(hKL.ToInt32() & 0x0000FFFF);
            if (hKL != (IntPtr)Const.ENG_LANG_KEYB_LAYOUT)
            {
                lastKeybLayout = hKL;
                Interop.PostMessage(prc.MainWindowHandle, 0x0050, (IntPtr)2, IntPtr.Zero);
            }
        }
        private List<AutomationElement> SearchChromeAdressBarAE(int processId)
        {
            var result = new List<AutomationElement>();
            List<IntPtr> widgetsHandles = WowDisableWinKeyTools.GetWidgetWindowHandles(processId, Const.CHROME_CLASS_NAME);

            foreach (IntPtr widgetHandle in widgetsHandles)
            {
                bool isRestored = Tools.RestoreMinimizedWindow(widgetHandle);
                if (isRestored)
                    Tools.MinimizeWindow(widgetHandle);
                var addrbar = GetAddressBarFromHandle(widgetHandle);
                if(addrbar!=null)
                    result.Add(addrbar);
            }
            return result;
        }
        private List<AutomationElement> SearchOperaAdressBarAE(int processId)
        {
            return null;
        }
        private void EnabledInit()
        {
            switch (browser)
            {
                case InternetBrowser.GoogleChrome:
                    uiProcess.FillProcessData(Tools.UIProcess(Const.CHROME_PROCESS_NAME));
                    HookManager.chromeProcess = uiProcess.Process;
                    break;
                case InternetBrowser.Opera:
                    uiProcess.FillProcessData(Tools.UIProcess(Const.OPERA_PROCESS_NAME));
                    HookManager.operaProcess = uiProcess.Process;
                    break;
                case InternetBrowser.Firefox:
                case InternetBrowser.TorBrowser:
                case InternetBrowser.InternetExplorer:
                    break;
            }
            UrlFunc(uiProcess.Process);
        }

        //void BABLanguageSwitcher_enabledChanged(SystemProcessHookForm winWatcher, bool state)
        //{
        //    if (state)
        //        windowWatcher.WindowEvent += windowWatcher_WindowEvent;
        //    else
        //        windowWatcher.WindowEvent -= windowWatcher_WindowEvent;
        //}

        //void windowWatcher_WindowEvent(object sender, IntPtr hWnd, Interop.ShellEvents shell)
        //{
        //    if (shell == Interop.ShellEvents.HSHELL_WINDOWDESTROYED)
        //    {
        //        //Remove address bar automation element from list of this elements
        //        if ((Tools.GetClassName(hWnd) == Const.CHROME_CLASS_NAME &&
        //            Tools.GetProcessName(hWnd) == Const.CHROME_PROCESS_NAME &&
        //            browser == InternetBrowser.GoogleChrome) ||
        //            (Tools.GetClassName(hWnd) == Const.CHROME_CLASS_NAME &&
        //            Tools.GetProcessName(hWnd) == Const.OPERA_PROCESS_NAME &&
        //            browser == InternetBrowser.Opera) ||
        //            (Tools.GetClassName(hWnd) == Const.FIREFOX_CLASS_NAME &&
        //            Tools.GetProcessName(hWnd) == Const.FIREFOX_PROCESS_NAME &&
        //            browser == InternetBrowser.Firefox) ||
        //            (Tools.GetClassName(hWnd) == Const.IE_CLASS_NAME &&
        //            Tools.GetProcessName(hWnd) == Const.IE_PROCESS_NAME &&
        //            browser == InternetBrowser.InternetExplorer))
        //            EnabledInit();
        //    }
        //    if (shell == Interop.ShellEvents.HSHELL_WINDOWCREATED)
        //    {
        //        if ((Tools.GetClassName(hWnd) == Const.CHROME_CLASS_NAME &&
        //            Tools.GetProcessName(hWnd) == Const.CHROME_PROCESS_NAME &&
        //            browser == InternetBrowser.GoogleChrome) ||
        //            (Tools.GetClassName(hWnd) == Const.CHROME_CLASS_NAME &&
        //            Tools.GetProcessName(hWnd) == Const.OPERA_PROCESS_NAME &&
        //            browser == InternetBrowser.Opera) ||
        //            (Tools.GetClassName(hWnd) == Const.FIREFOX_CLASS_NAME &&
        //            Tools.GetProcessName(hWnd) == Const.FIREFOX_PROCESS_NAME &&
        //            browser == InternetBrowser.Firefox) ||
        //            (Tools.GetClassName(hWnd) == Const.IE_CLASS_NAME &&
        //            Tools.GetProcessName(hWnd) == Const.IE_PROCESS_NAME &&
        //            browser == InternetBrowser.InternetExplorer))
        //            EnabledInit();
        //    }
        //}
        private AutomationElement GetAddressBarFromHandle(IntPtr hWnd)
        {
            try
            {
                // find the automation element
                AutomationElement lync = AutomationElement.FromHandle(hWnd);
                if (lync == null)
                    return null;
                // manually walk through the tree, searching using TreeScope.Descendants is too slow (even if it's more reliable)
                AutomationElement urlBar = null;
                // walking path found using inspect.exe (Windows SDK) for Chrome Version 43.0.2357.65 m (currently the latest stable)
                var lyncDaughter = lync.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));
                if (lyncDaughter == null) { return null; }  // not the right chrome.exe
                // here, you can optionally check if Incognito is enabled:
                //bool bIncognito = TreeWalker.RawViewWalker.GetFirstChild(TreeWalker.RawViewWalker.GetFirstChild(elm1)) != null;
                var lyncGranddoughter = TreeWalker.RawViewWalker.GetLastChild(lyncDaughter);
                var lyncGreatgranddoughter = lyncGranddoughter.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, ""))[1];
                var lyncGreatgreatgranddoughter = lyncGreatgranddoughter.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
                var lyncGreatgreatgreatgranddoughter = lyncGreatgreatgranddoughter.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, ""));
                urlBar = lyncGreatgreatgreatgranddoughter.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                return urlBar;
            }
            catch
            {
                Console.WriteLine("Exeption: BABLanguageSwitcher.GetAddressBarFromHandle");
                return null;
            }
        }
    }
}
