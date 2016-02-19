using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Diagnostics;

namespace wowDisableWinKey.Browsers
{
    using Tools = WowDisableWinKeyTools;

    public static class OperaSet
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool WebSkypeIsNullOrEmpty(WebSkypeStruct wSkype)
        {
            // if (wSkype.skypeTab == null || wSkype.toggleExtension == null || String.IsNullOrEmpty(wSkype.toggleExtension.Current.Name) || String.IsNullOrEmpty(wSkype.skypeTab.Current.Name))
            if (wSkype.skypeTab == null || String.IsNullOrEmpty(wSkype.skypeTab.Current.Name))
                return true;
            return false;
        }
        /// <summary>
        /// Обнуляет структу webSkypeStruct
        /// </summary>
        public static void WebSkypeStructToNull(ref WebSkypeStruct wSkype)
        {
            wSkype.skypeTab = null;
            wSkype.toggleExtension = null;
            wSkype.windowHandle = IntPtr.Zero;
        }
        public static bool WebSkypeIsEmpty(WebSkypeStruct wSkype)
        {
            if (wSkype.skypeTab != null && String.IsNullOrEmpty(wSkype.toggleExtension.Current.Name))
                return true;
            return false;
        }

        /// <summary>
        /// Обновляет структу webSkypeStruct
        /// </summary>
        public static bool WebSkypeStructRefresh(ref WebSkypeStruct wSkype, InternetBrowserData data)
        {
            //получим список нужных окон
            List<IntPtr> widgetsHandles = WowDisableWinKeyTools.GetWidgetWindowHandles(data.ProcessId, Const.CHROME_CLASS_NAME);

            //найдем элементы: вкладку скайпа
            foreach (IntPtr widgetHandle in widgetsHandles)
            {
                bool isRestored = Tools.RestoreMinimizedWindow(widgetHandle);

                wSkype.skypeTab = SkypeTab(widgetHandle);
                //wSkype.toggleExtension = ToggleExtension(data.process.MainWindowHandle);
                wSkype.windowHandle = widgetHandle;
                if (isRestored)
                    Tools.MinimizeWindow(widgetHandle);

                // if (wSkype.skypeTab != null && wSkype.toggleExtension != null)
                if (wSkype.skypeTab != null)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private static AutomationElement SkypeTab(IntPtr handle)
        {
            string windowName = "";

            try
            {
                // find the automation element
                AutomationElement windowAE = AutomationElement.FromHandle(handle);
                windowName = windowAE.Current.Name;
                //situation if process is not foreground, and/or skype tab is not active
                AutomationElement tabControl = TabControl(windowAE);
                if (tabControl == null)
                    return null;
                AutomationElementCollection tabItems = TabItems(tabControl);
                AutomationElement skype = SkypeTabItem(tabItems);
                return skype == null ? null : skype;
            }
            catch { return null; }
        }
        /// <summary>
        /// Manual search Opera tab control element
        /// walking path found using inspect.exe (Windows SDK) for Opera Version 33.0.1990.115 (currently the latest stable)
        /// </summary>
        /// <param name="opera"></param>
        /// <returns></returns>
        private static AutomationElement TabControl(AutomationElement opera)
        {
            if (opera == null)
                return null;
            // manually walk through the tree, searching using TreeScope.Descendants is too slow (even if it's more reliable)
           // var operaDaughter = opera.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.HelpTextProperty, ""));
            var operaDaughter = opera.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Browser container"));

            if (operaDaughter == null) { return null; } // not the right opera.exe

            var operaGranddaughter = operaDaughter.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Browser client"));
            var operaGreatgranddaughter = operaGranddaughter.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Browser contents"));
            var operasBelovedChild = operaGreatgranddaughter.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Top bar container"));
            var operasUnlovedChild = operasBelovedChild.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Tab bar"));
            //OPERA STRONG AND YOUNG!
            return operasUnlovedChild;
        }
        /// <summary>
        /// Collection of google chrome tab items
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        private static AutomationElementCollection TabItems(AutomationElement tab)
        {
            return tab.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem));
        }
        /// <summary>
        /// Retrieve web skype tab from google chrome tab collecion
        /// </summary>
        /// <param name="tabItems"></param>
        /// <returns></returns>
        private static AutomationElement SkypeTabItem(AutomationElementCollection tabItems)
        {
            foreach (AutomationElement tab in tabItems)
            {
                if (tab.Current.Name.Contains("Skype"))
                    return tab;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private static AutomationElement ToggleExtension(IntPtr handle)
        {
            string chromeWindowName = "";

            try
            {
                // find the automation element
                AutomationElement chromeAE = AutomationElement.FromHandle(handle);
                chromeWindowName = chromeAE.Current.Name;
                AutomationElement chromeToolBar = ExtensionsGroup(chromeAE);
                if (chromeToolBar == null)
                    return null;
                AutomationElementCollection chromeExtentions = Extensions(chromeToolBar);
                AutomationElement extension = TooglePluginButton(chromeExtentions);
                return extension == null ? null : extension;

            }
            catch { return null; }
        }
        /// <summary>
        /// Manual search google chrome plugins toolbar automation element
        /// walking path found using inspect.exe (Windows SDK) for Chrome Version 43.0.2357.65 m (currently the latest stable)
        /// </summary>
        /// <param name="chrome"></param>
        /// <returns></returns>
        private static AutomationElement ExtensionsGroup(AutomationElement chrome)
        {
            if (chrome == null)
                return null;
            // manually walk through the tree, searching using TreeScope.Descendants is too slow (even if it's more reliable)
            var chromeDaughter = chrome.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));

            if (chromeDaughter == null) { return null; } // not the right chrome.exe

            // here, you can optionally check if Incognito is enabled:
            var chromeGranddaughter = TreeWalker.RawViewWalker.GetLastChild(chromeDaughter);
            var chromeGreatgranddaughter = chromeGranddaughter.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, ""))[1];
            var toolbar = chromeGreatgranddaughter.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
            return toolbar.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Extensions"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="extentionsGr"></param>
        /// <returns></returns>
        private static AutomationElementCollection Extensions(AutomationElement extentionsGr)
        {
            return extentionsGr.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugins"></param>
        /// <returns></returns>
        private static AutomationElement TooglePluginButton(AutomationElementCollection plugins)
        {
            foreach (AutomationElement plugin in plugins)
            {
                if (plugin.Current.Name.Contains("Toggle between tabs"))
                    return plugin;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operaProcessName"></param>
        /// <returns></returns>
        public static Process UIProcess(string operaProcessName)
        {
            Process operaUIProcess = null;
            Process[] procsOpera = Process.GetProcessesByName(operaProcessName);
            if (procsOpera.Length == 0)
                return null;
            else
            {
                foreach (Process opera in procsOpera)
                {
                    // the chrome process must have a window

                    if (opera.MainWindowHandle != IntPtr.Zero)
                    {
                        operaUIProcess = opera;
                        break;
                    }
                }
            }
            return operaUIProcess;
        }
    }
}
