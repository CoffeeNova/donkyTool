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

    public static class GoogleChromeSet
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool WebSkypeIsNullOrEmpty(WebSkypeStruct wSkype)
        {
            if (wSkype.skypeTab == null || wSkype.toggleExtension == null || String.IsNullOrEmpty(wSkype.toggleExtension.Current.Name) || String.IsNullOrEmpty(wSkype.skypeTab.Current.Name))
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


        public static bool WebSkypeIsNull(WebSkypeStruct wSkype)
        {
            if (wSkype.skypeTab == null || wSkype.toggleExtension == null)
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

            //найдем элементы: вкладку скайпа и расширение toggle extension
            foreach (IntPtr widgetHandle in widgetsHandles)
            {
                bool isRestored = Tools.RestoreMinimizedWindow(widgetHandle);

                wSkype.skypeTab = SkypeTab(widgetHandle);
                wSkype.toggleExtension = ToggleExtension(data.MainWindowHandle);
                wSkype.windowHandle = widgetHandle;
                if (isRestored)
                    Tools.MinimizeWindow(widgetHandle);

                if (wSkype.skypeTab != null && wSkype.toggleExtension != null)
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
        /// Manual search google chrome tab control element
        /// walking path found using inspect.exe (Windows SDK) for Chrome Version 43.0.2357.65 m (currently the latest stable)
        /// </summary>
        /// <param name="chrome"></param>
        /// <returns></returns>
        private static AutomationElement TabControl(AutomationElement chrome)
        {
            if (chrome == null)
                return null;
            // manually walk through the tree, searching using TreeScope.Descendants is too slow (even if it's more reliable)
            var chromeDaughter = chrome.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));

            if (chromeDaughter == null) { return null; } // not the right chrome.exe

            // here, you can optionally check if Incognito is enabled:
            var chromeGranddaughter = TreeWalker.RawViewWalker.GetLastChild(chromeDaughter);
            var chromeGreatgranddaughter = chromeGranddaughter.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, ""))[1];
            return chromeGreatgranddaughter.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Tab));
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
            return ExtensionDependLanguage(toolbar);
        }
        /// <summary>
        /// Extension name property in dependence of chrome language 
        /// </summary>
        /// <param name="toolbar"></param>
        /// <returns></returns>
        private static AutomationElement ExtensionDependLanguage(AutomationElement toolbar)
        {
            string[] extensionNames = new string[] { "Extensions", "Расширения" };
            AutomationElement extension = null;
            for(int i =0; i < extensionNames.Count(); i++)
            {
                extension = toolbar.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, extensionNames[i]));
                if (extension == null)
                    continue;
                else
                    break;
            }
            return extension;
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
        
    }
}
