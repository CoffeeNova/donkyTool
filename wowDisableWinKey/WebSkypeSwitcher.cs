using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using wowDisableWinKey.Browsers;
using System.Windows.Automation;
using Gma.UserActivityMonitor;

namespace wowDisableWinKey
{
    using GBC = GlobalBindController;
    using Tools = WowDisableWinKeyTools;

    public class WebSkypeSwitcher
    {
        //public int ID { get; set; }

        private bool wsTabGotFocusHook = false;
        //private bool ForegroundHook = false;
        private GBC controller;
        private IntPtr threadHwnd;
        private WebSkypeStruct webSkypeStruct;
        private AltTabSimulator atInstance;

        private InternetBrowserData browserData;
        public InternetBrowserData BrowserData
        {
            get { return browserData; }
            set
            {
                if (browserData != value)
                    browserData = value;
            }
        }
        private bool isActive = false;
        public bool IsActive
        {
            get { return controller.Execute; }
            set
            {
                controller.Execute = value;
                isActive = value;
            }
        }
        private bool newMessage = false;
        public bool NewMessage
        {
            get { return newMessage; }
            set
            {
                if (!value.Equals(newMessage))
                {
                    newMessage = value;
                    GotNewMessage(this);
                }
            }
        }
        public delegate void newMessageDelegate(WebSkypeSwitcher wss);
        public event newMessageDelegate GotNewMessage;

        public Keys Bind
        {
            get { return controller.Key; }
            set { controller.Key = value; }
        }
        //private 
        public List<GBC_KeyModifier> Modifiers
        {
            get
            {
                return controller.Tasks.Select<KeyValuePair<GBC_KeyModifier, Action>, GBC_KeyModifier>((pair, keymod) => { return pair.Key; }).ToList();
            }
            set
            {
                var kvp = new List<KeyValuePair<GBC_KeyModifier, Action>>();
                Action bindAction = BindAction;
                foreach (GBC_KeyModifier mod in value)
                    kvp.Add(new KeyValuePair<GBC_KeyModifier, Action>(mod, BindAction));
                controller.Tasks = kvp;

            }
        }

        public WebSkypeSwitcher(InternetBrowserData ibData, Keys switchBind, List<GBC_KeyModifier> modifiers, GBC.BindMethod bindMethod, GBC.HookBehaviour hBehaviour)
        {

            //initialize
            webSkypeStruct = new WebSkypeStruct();

            threadHwnd = IntPtr.Zero;

            browserData = ibData;
            //if(browserData.BrowserName == InternetBrowser.GoogleChrome)

            //Create
            var kvp = new List<KeyValuePair<GBC_KeyModifier, Action>>();
            Action bindAction = BindAction;
            foreach (GBC_KeyModifier mod in modifiers)
                kvp.Add(new KeyValuePair<GBC_KeyModifier, Action>(mod, BindAction));

            controller = new GBC(switchBind,
                GBC.BindMethod.RegisterHotKey,
                GBC.HookBehaviour.Replacement,
                kvp);

            atInstance = AltTabSimulator.Instance;

        }

        /// <summary>
        /// 
        /// </summary>
        public void BindAction()
        {
            if (threadHwnd == IntPtr.Zero)
            {
                Thread ws_thread = new Thread(() => WebSkypeCallback(browserData));
                threadHwnd = (IntPtr)ws_thread.ManagedThreadId;
                ws_thread.Start();
            }
        }
        private void WebSkypeCallback(InternetBrowserData data)
        {
            switch (data.Browser)
            {
                case InternetBrowser.GoogleChrome:
                    GoogleChrome(data);
                    break;
                case InternetBrowser.Firefox:
                    Firefox(data);
                    break;
                case InternetBrowser.Opera:
                    Opera(data);
                    break;
                case InternetBrowser.TorBrowser:
                    TorBrowser(data);
                    break;
            }
        }

        private void GoogleChrome(InternetBrowserData ibData)
        {
            IntPtr initialForeground;
            string windowName = "";
            List<IntPtr> rootWindowsChrome = new List<IntPtr>();

            try
            {
                if (ibData.Process == null) //we have google chrome started
                    return;
                initialForeground = Interop.GetForegroundWindow();

                // check if skype tab or toggle window exists
                if (webSkypeStruct.windowHandle == IntPtr.Zero || GoogleChromeSet.WebSkypeIsNullOrEmpty(webSkypeStruct))
                    if (!GoogleChromeSet.WebSkypeStructRefresh(ref webSkypeStruct, ibData))
                        return;

                AutomationElement chromeAE = AutomationElement.FromHandle(webSkypeStruct.windowHandle);
                windowName = chromeAE.Current.Name;

                if (windowName.Contains("Skype - Google Chrome") && Tools.WebSkypeOrNotifyWindowHandle(initialForeground, webSkypeStruct.windowHandle))
                {
                    if (atInstance.TabOrWindow == SwitchTo.Window)
                    {
                        Interop.SetForegroundWindow(atInstance.AltTabList[1]); //its a previus window in a list
                        Tools.RestoreMinimizedWindow(atInstance.AltTabList[1]);
                    }
                    //do nothing, we have an extension to switch between tabs
                    //ok just simulate mouse click 
                    if (atInstance.TabOrWindow == SwitchTo.Tab)
                        Tools.SimulateClickUIAutomation(webSkypeStruct.toggleExtension, chromeAE, webSkypeStruct.windowHandle);
                    return;
                }
                if (windowName.Contains("Skype - Google Chrome") && initialForeground != webSkypeStruct.windowHandle)
                {
                    Tools.RestoreMinimizedWindow(webSkypeStruct.windowHandle);
                    Interop.SetForegroundWindow(webSkypeStruct.windowHandle);
                    return;
                }
                //situation if chrome process is not foreground, and/or skype tab is not active
                //cant get SelectionItemPattern - http://stackoverflow.com/questions/22747613/control-pattern-availability-is-set-to-true-but-returns-unsupported-pattern-e
                //than simulate mouse click !
                Tools.RestoreMinimizedWindow(webSkypeStruct.windowHandle);
                Interop.SetForegroundWindow(webSkypeStruct.windowHandle);
                if (Tools.FullscreenProcess(webSkypeStruct.windowHandle))
                {
                    Interop.PostMessage(initialForeground, Const.WM_KEYDOWN, (IntPtr)WindowsVirtualKey.VK_ESCAPE, IntPtr.Zero);
                    Interop.PostMessage(initialForeground, Const.WM_KEYUP, (IntPtr)WindowsVirtualKey.VK_ESCAPE, IntPtr.Zero);
                    Thread.Sleep(50); // пауза ( на быстрых пк не успевает свернуться интерфейс, а условие необходимо для клика мыши по вкладке
                }
                Tools.SimulateClickUIAutomation(webSkypeStruct.skypeTab, chromeAE, webSkypeStruct.windowHandle);

                //if (Tools.NotWebSkypeButNotifyWindowHandle(currentForeground, webSkypeStruct.windowHandle) && atInstance.AltTabList[1] != webSkypeStruct.windowHandle)
                if (initialForeground != webSkypeStruct.windowHandle)
                {
                    if (!Tools.NotWebSkypeButNotifyWindowHandle(initialForeground, webSkypeStruct.windowHandle))
                    {
                        Thread.Sleep(100); //задержка, для того, чтобы успел сработать обработчик события, который среагирует на переключение вкладки
                        atInstance.TabOrWindow = SwitchTo.Window;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //testWatchTime = testWatch.Elapsed;
                //testWatch.Stop();
                //Console.WriteLine(testWatchTime.ToString());
                Thread.Sleep(50);
                threadHwnd = IntPtr.Zero;
            }
        }

        private void Firefox(InternetBrowserData ibData)
        {

        }

        private void Opera(InternetBrowserData ibData)
        {
            IntPtr initialForeground;
            string windowName = "";
            List<IntPtr> rootWindowsChrome = new List<IntPtr>();

            try
            {
                if (ibData.Process == null)
                    return;
                initialForeground = Interop.GetForegroundWindow();

                // check if skype tab or toggle window exists
                if (webSkypeStruct.windowHandle == IntPtr.Zero || OperaSet.WebSkypeIsNullOrEmpty(webSkypeStruct))
                    if (!OperaSet.WebSkypeStructRefresh(ref webSkypeStruct, ibData))
                        return;

                AutomationElement operaAE = AutomationElement.FromHandle(webSkypeStruct.windowHandle);
                windowName = operaAE.Current.Name;

                if (windowName.Contains("Skype - Opera") && Tools.WebSkypeOrNotifyWindowHandle(initialForeground, webSkypeStruct.windowHandle))
                {
                    Tools.RestoreMinimizedWindow(webSkypeStruct.windowHandle);
                    Interop.SetForegroundWindow(webSkypeStruct.windowHandle);
                    return;
                }
                if (windowName.Contains("Skype - Opera") && initialForeground != webSkypeStruct.windowHandle)
                {
                    if (atInstance.TabOrWindow == SwitchTo.Window)
                    {
                        Interop.SetForegroundWindow(atInstance.AltTabList[1]); //its a previus window in a list
                        Tools.RestoreMinimizedWindow(atInstance.AltTabList[1]);
                    }
                    //do nothing, we have an extension to switch between tabs
                    //ok just !(simulate mouse click) send ctrl+tab in to space 
                    if (atInstance.TabOrWindow == SwitchTo.Tab)
                    {
                        Interop.PostMessage(webSkypeStruct.windowHandle, Const.WM_KEYDOWN, (IntPtr)WindowsVirtualKey.VK_CONTROL, (IntPtr)0x001D0001);
                        Interop.PostMessage(webSkypeStruct.windowHandle, Const.WM_KEYDOWN, (IntPtr)WindowsVirtualKey.VK_TAB, (IntPtr)0x000F0001);

                        Interop.PostMessage(webSkypeStruct.windowHandle, Const.WM_KEYUP, (IntPtr)WindowsVirtualKey.VK_TAB, (IntPtr)0xC00F0001);
                        Interop.PostMessage(webSkypeStruct.windowHandle, Const.WM_KEYUP, (IntPtr)WindowsVirtualKey.VK_CONTROL, (IntPtr)0xC01D0001);
                    }
                    return;
                }
                //situation if process is not foreground, and/or skype tab is not active
                //cant get SelectionItemPattern - http://stackoverflow.com/questions/22747613/control-pattern-availability-is-set-to-true-but-returns-unsupported-pattern-e
                //than simulate mouse click !
                Tools.RestoreMinimizedWindow(webSkypeStruct.windowHandle);
                Interop.SetForegroundWindow(webSkypeStruct.windowHandle);
                if (Tools.FullscreenProcess(webSkypeStruct.windowHandle))
                {
                    Interop.PostMessage(initialForeground, Const.WM_KEYDOWN, (IntPtr)WindowsVirtualKey.VK_ESCAPE, IntPtr.Zero);
                    Interop.PostMessage(initialForeground, Const.WM_KEYUP, (IntPtr)WindowsVirtualKey.VK_ESCAPE, IntPtr.Zero);
                    Thread.Sleep(50); // пауза ( на быстрых пк не успевает свернуться интерфейс, а условие необходимо для клика мыши по вкладке
                }
                Tools.SimulateClickUIAutomation(webSkypeStruct.skypeTab, operaAE, webSkypeStruct.windowHandle);

                if (initialForeground != webSkypeStruct.windowHandle)
                {
                    if (!Tools.NotWebSkypeButNotifyWindowHandle(initialForeground, webSkypeStruct.windowHandle))
                    {
                        Thread.Sleep(100); //задержка, для того, чтобы успел сработать обработчик события, который среагирует на переключение вкладки
                        atInstance.TabOrWindow = SwitchTo.Window;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //testWatchTime = testWatch.Elapsed;
                //testWatch.Stop();
                //Console.WriteLine(testWatchTime.ToString());
                Thread.Sleep(50);
                threadHwnd = IntPtr.Zero;
            }

        }
        private void TestCallback()
        {

        }

        private void TorBrowser(InternetBrowserData ibData)
        {

        }
        private event EventHandler WebSkypeGotFocus
        {
            add
            {
                switch (BrowserData.Browser)
                {
                    case InternetBrowser.GoogleChrome:
                        HookManager.GwsGotFocus += value;
                        break;
                    case InternetBrowser.Opera:
                        HookManager.OwsGotFocus += value;
                        break;
                    case InternetBrowser.Firefox:
                        HookManager.FFwsGotFocus += value;
                        break;
                    case InternetBrowser.InternetExplorer:
                        HookManager.IEwsGotFocus += value;
                        break;
                    case InternetBrowser.TorBrowser:
                        HookManager.TBwsGotFocus += value;
                        break;
                }
            }
            remove
            {
                switch (BrowserData.Browser)
                {
                    case InternetBrowser.GoogleChrome:
                        HookManager.GwsGotFocus -= value;
                        break;
                    case InternetBrowser.Opera:
                        HookManager.OwsGotFocus -= value;
                        break;
                    case InternetBrowser.Firefox:
                        HookManager.FFwsGotFocus -= value;
                        break;
                    case InternetBrowser.InternetExplorer:
                        HookManager.IEwsGotFocus -= value;
                        break;
                    case InternetBrowser.TorBrowser:
                        HookManager.TBwsGotFocus -= value;
                        break;
                }
            }
        }

        private event EventHandler WebSkypeTabNameChange
        {
            add
            {
                switch (BrowserData.Browser)
                {
                    case InternetBrowser.GoogleChrome:
                        HookManager.GwsTabNameChange += value;
                        break;
                    case InternetBrowser.Opera:
                        HookManager.OwsTabNameChange += value;
                        break;
                    case InternetBrowser.Firefox:
                        HookManager.FFwsTabNameChange += value;
                        break;
                    case InternetBrowser.InternetExplorer:
                        HookManager.IEwsTabNameChange += value;
                        break;
                    case InternetBrowser.TorBrowser:
                        HookManager.TBwsTabNameChange += value;
                        break;
                }
            }
            remove
            {
                switch (BrowserData.Browser)
                {
                    case InternetBrowser.GoogleChrome:
                        HookManager.GwsTabNameChange -= value;
                        break;
                    case InternetBrowser.Opera:
                        HookManager.OwsTabNameChange -= value;
                        break;
                    case InternetBrowser.Firefox:
                        HookManager.FFwsTabNameChange -= value;
                        break;
                    case InternetBrowser.InternetExplorer:
                        HookManager.IEwsTabNameChange -= value;
                        break;
                    case InternetBrowser.TorBrowser:
                        HookManager.TBwsTabNameChange -= value;
                        break;
                }
            }
        }

        public void Update()
        {
            try
            {
                //web skype got focus hook
                if (isActive && browserData.Process != null && !wsTabGotFocusHook)
                {
                    WebSkypeGotFocus += HookManager_WebSkypeTabGotFocus;
                    WebSkypeTabNameChange += HookManager_WebSkypeTabNameChange;
                    wsTabGotFocusHook = true;
                    //обновим webSkypeStruct
                    WebSkypeStructRefresh(ref webSkypeStruct, browserData);
                }
                else if ((!isActive || browserData.Process == null) && wsTabGotFocusHook)
                {
                    WebSkypeGotFocus -= HookManager_WebSkypeTabGotFocus;
                    WebSkypeTabNameChange -= HookManager_WebSkypeTabNameChange;
                    //webSkypeHook.WindowEvent -= webSkypeHook_WindowEvent;
                    wsTabGotFocusHook = false;
                    //обнулим webSkypeStruct
                    WebSkypeStructToNull(ref webSkypeStruct);
                }
                ////ForegroundHook
                //if (isActive && browserData.process != null && !ForegroundHook)
                //{
                //    HookManager.ForegroundChanged += HookManager_ForegroundChanged;
                //    ForegroundHook = true;
                //}
                //else if ((!isActive || browserData.process == null) && ForegroundHook)
                //{
                //    HookManager.ForegroundChanged -= HookManager_ForegroundChanged;
                //    ForegroundHook = false;
                //}
                //обновим webSkypeStruct
                //if (isActive && browserData.process != null && GoogleChromeSet.WebSkypeIsEmpty(webSkypeStruct))
                //    WebSkypeStructRefresh(ref webSkypeStruct, browserData);
                //обнулим webSkypeStruct
                else if ((!isActive || browserData.Process == null) && webSkypeStruct.windowHandle != IntPtr.Zero)
                    WebSkypeStructToNull(ref webSkypeStruct);
                //change notification icon when recieve a message
                if (isActive && browserData.Process != null && webSkypeStruct.skypeTab != null)
                    NewMessage = HaveNewSkypeMessages(webSkypeStruct.skypeTab) ? true : false;
            }
            catch { }

        }

        /// <summary>
        /// Calls a similar method depending on browser type
        /// </summary>
        /// <param name="wSkype"></param>
        /// <param name="browserData"></param>
        private void WebSkypeStructRefresh(ref WebSkypeStruct wSkype, InternetBrowserData browserData)
        {
            switch (browserData.Browser)
            {
                case InternetBrowser.GoogleChrome:
                    GoogleChromeSet.WebSkypeStructRefresh(ref webSkypeStruct, browserData);
                    break;
                case InternetBrowser.Opera:
                    OperaSet.WebSkypeStructRefresh(ref webSkypeStruct, browserData);
                    break;
                case InternetBrowser.Firefox:
                    //FirefoxSet.WebSkypeStructRefresh(ref webSkypeStruct, browserData);
                    break;
                case InternetBrowser.TorBrowser:
                    //TorBrowserSet.WebSkypeStructRefresh(ref webSkypeStruct, browserData);
                    break;
            }

        }
        /// <summary>
        /// Calls a similar method depending on browser type
        /// </summary>
        /// <param name="wSkype"></param>
        private void WebSkypeStructToNull(ref WebSkypeStruct wSkype)
        {
            switch (browserData.Browser)
            {
                case InternetBrowser.GoogleChrome:
                    GoogleChromeSet.WebSkypeStructToNull(ref wSkype);
                    break;
                case InternetBrowser.Opera:
                    OperaSet.WebSkypeStructToNull(ref wSkype);
                    break;
                case InternetBrowser.Firefox:
                    //FirefoxSet.WebSkypeStructToNull(ref wSkype);
                    break;
                case InternetBrowser.TorBrowser:
                    //TorBrowserSet.WebSkypeStructToNull(ref wSkype);
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        private bool HaveNewSkypeMessages(AutomationElement tab)
        {
            return tab.Current.Name.StartsWith("(") ? true : false;
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/dd318066(v=vs.85).aspx
        /// EVENT_OBJECT_SELECTION
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HookManager_WebSkypeTabGotFocus(Object sender, EventArgs e)
        {
            try
            {
                //Console.WriteLine("GOT");
                AutomationElement lync = AutomationElement.FromHandle((IntPtr)sender);
                if (browserData.Process == null)
                    return;
                if (lync.Current.ProcessId == browserData.Process.Id)
                    atInstance.TabOrWindow = SwitchTo.Tab;
            }
            catch { }
        }
        private void HookManager_WebSkypeTabNameChange(Object sender, EventArgs e)
        {
            if (!isActive || browserData.Process == null || webSkypeStruct.skypeTab == null)
                return;
            try
            {
                //Console.WriteLine("GOT");
                AutomationElement lync = AutomationElement.FromHandle((IntPtr)sender);
                if (lync.Current.Name.Contains("Skype - "))
                    NewMessage = HaveNewSkypeMessages(webSkypeStruct.skypeTab) ? true : false;
            }
            catch { }
        }
        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/dd318066(v=vs.85).aspx
        /// EVENT_OBJECT_CREATE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HookManager_WebSkypeWindowCreate(Object sender, EventArgs e)
        {
        }

        public void Dispose()
        {
            //if (webSkypeHooks)
            //{
            //    HookManager.WebSkypeTabGotFocus -= HookManager_WebSkypeTabGotFocus;
            //    webSkypeHook.WindowEvent -= webSkypeHook_WindowEvent;
            //    webSkypeHooks = false;
            //}
            //if (ForegroundHook)
            //{
            //    HookManager.ForegroundChanged -= HookManager_ForegroundChanged;
            //    ForegroundHook = false;
            //}

        }
        ~WebSkypeSwitcher()
        {
            Dispose();
        }

    }
    public class InternetBrowserData
    {
        private InternetBrowser browser = InternetBrowser.GoogleChrome;
        public InternetBrowser Browser { get { return browser; } }
        private Process process = null;
        public Process Process { get { return process; } }
        private string processName = null;
        public string ProcessName { get { return processName; } }
        private int processId = 0;
        public int ProcessId { get { return processId; } }
        private IntPtr mainWindowHandle = IntPtr.Zero;
        public IntPtr MainWindowHandle { get { return mainWindowHandle; } }

        public InternetBrowserData(InternetBrowser iBrowser)
        {
            browser = iBrowser;
        }
        //public InternetBrowserData(){ }

        public void FillProcessData(Process prc)
        {
            process = prc;
            processId = prc == null ? 0 : prc.Id;
            processName = prc == null ? null : prc.ProcessName;
            mainWindowHandle = prc == null ? IntPtr.Zero : prc.MainWindowHandle;
            //browser = prc == null ? browser : DefineBrowser(prc);
        }
        private InternetBrowser DefineBrowser(Process process)
        {
            switch (process.ProcessName)
            {
                case Const.CHROME_PROCESS_NAME:
                    return InternetBrowser.GoogleChrome;
                case Const.OPERA_PROCESS_NAME:
                    return InternetBrowser.Opera;
                case Const.FIREFOX_PROCESS_NAME:
                    return InternetBrowser.Firefox;
                case Const.IE_PROCESS_NAME:
                    return InternetBrowser.InternetExplorer;
                default:
                    return InternetBrowser.GoogleChrome;
            }
        }
    }
}
