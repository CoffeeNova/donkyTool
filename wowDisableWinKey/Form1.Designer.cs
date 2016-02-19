using System;
using Gma.UserActivityMonitor;
namespace wowDisableWinKey
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {

     
            if (ptrHookWinKey != IntPtr.Zero)
            {
                Interop.UnhookWindowsHookEx(ptrHookWinKey);
                ptrHookWinKey = IntPtr.Zero;
            }
            if (disableAllGotHook)
                HookManager.KeyBlock -= HookManager_KeyBlock;
            
            base.Dispose(disposing);
            if (disposing && (components != null))
            {
                components.Dispose();
            }
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.skypeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.googleChromeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.operaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.firefoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.torBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.internetExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.capsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chromeAdrbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.conditionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.globalEventProvider1 = new Gma.UserActivityMonitor.GlobalEventProvider();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.skypeTabToolStripMenuItem,
            this.capsToolStripMenuItem,
            this.chromeAdrbarToolStripMenuItem,
            this.conditionToolStripMenuItem,
            this.disableAllToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(160, 158);
            // 
            // skypeTabToolStripMenuItem
            // 
            this.skypeTabToolStripMenuItem.CheckOnClick = true;
            this.skypeTabToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.googleChromeToolStripMenuItem,
            this.operaToolStripMenuItem,
            this.firefoxToolStripMenuItem,
            this.torBrowserToolStripMenuItem,
            this.internetExplorerToolStripMenuItem});
            this.skypeTabToolStripMenuItem.Name = "skypeTabToolStripMenuItem";
            this.skypeTabToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.skypeTabToolStripMenuItem.Text = "SkypeTabSwitch";
            this.skypeTabToolStripMenuItem.Click += new System.EventHandler(this.skypeTabSwitchToolStripMenuItem_Click);
            // 
            // googleChromeToolStripMenuItem
            // 
            this.googleChromeToolStripMenuItem.Name = "googleChromeToolStripMenuItem";
            this.googleChromeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.googleChromeToolStripMenuItem.Text = "Google Chrome";
            this.googleChromeToolStripMenuItem.Click += new System.EventHandler(this.browserMenuItem_Click);
            // 
            // operaToolStripMenuItem
            // 
            this.operaToolStripMenuItem.Name = "operaToolStripMenuItem";
            this.operaToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.operaToolStripMenuItem.Text = "Opera";
            this.operaToolStripMenuItem.Click += new System.EventHandler(this.browserMenuItem_Click);
            // 
            // firefoxToolStripMenuItem
            // 
            this.firefoxToolStripMenuItem.Name = "firefoxToolStripMenuItem";
            this.firefoxToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.firefoxToolStripMenuItem.Text = "Firefox";
            this.firefoxToolStripMenuItem.Click += new System.EventHandler(this.browserMenuItem_Click);
            // 
            // torBrowserToolStripMenuItem
            // 
            this.torBrowserToolStripMenuItem.Name = "torBrowserToolStripMenuItem";
            this.torBrowserToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.torBrowserToolStripMenuItem.Text = "Tor Browser";
            this.torBrowserToolStripMenuItem.Click += new System.EventHandler(this.browserMenuItem_Click);
            // 
            // internetExplorerToolStripMenuItem
            // 
            this.internetExplorerToolStripMenuItem.Name = "internetExplorerToolStripMenuItem";
            this.internetExplorerToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.internetExplorerToolStripMenuItem.Text = "Internet Explorer";
            this.internetExplorerToolStripMenuItem.Click += new System.EventHandler(this.browserMenuItem_Click);
            // 
            // capsToolStripMenuItem
            // 
            this.capsToolStripMenuItem.CheckOnClick = true;
            this.capsToolStripMenuItem.Name = "capsToolStripMenuItem";
            this.capsToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.capsToolStripMenuItem.Text = "CapsSwitch";
            this.capsToolStripMenuItem.Click += new System.EventHandler(this.capsToolStripMenuItem_Click);
            // 
            // chromeAdrbarToolStripMenuItem
            // 
            this.chromeAdrbarToolStripMenuItem.CheckOnClick = true;
            this.chromeAdrbarToolStripMenuItem.Name = "chromeAdrbarToolStripMenuItem";
            this.chromeAdrbarToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.chromeAdrbarToolStripMenuItem.Text = "ChromeAdrbar";
            this.chromeAdrbarToolStripMenuItem.Click += new System.EventHandler(this.chromeAdrbarToolStripMenuItem_Click);
            // 
            // conditionToolStripMenuItem
            // 
            this.conditionToolStripMenuItem.CheckOnClick = true;
            this.conditionToolStripMenuItem.Name = "conditionToolStripMenuItem";
            this.conditionToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.conditionToolStripMenuItem.Text = "Condition";
            this.conditionToolStripMenuItem.Click += new System.EventHandler(this.conditionToolStripMenuItem_Click);
            // 
            // disableAllToolStripMenuItem
            // 
            this.disableAllToolStripMenuItem.CheckOnClick = true;
            this.disableAllToolStripMenuItem.Name = "disableAllToolStripMenuItem";
            this.disableAllToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.disableAllToolStripMenuItem.Text = "DisableAll";
            this.disableAllToolStripMenuItem.Click += new System.EventHandler(this.disableAllToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = Const.NOTIFY_ICON_TEXT;
            this.notifyIcon1.Visible = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(193, 113);
            this.ControlBox = false;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem conditionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableAllToolStripMenuItem;
        private Gma.UserActivityMonitor.GlobalEventProvider globalEventProvider1;
        private System.Windows.Forms.ToolStripMenuItem chromeAdrbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem capsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skypeTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem googleChromeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem operaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem firefoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem torBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem internetExplorerToolStripMenuItem;
        //private System.ComponentModel.ComponentResourceManager resources;
    }
}

