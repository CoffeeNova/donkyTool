using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace wowDisableWinKey
{
    /// <summary>
    /// http://stackoverflow.com/questions/21912686/most-efficient-way-for-getting-notified-on-window-open/21914783#21914783
    /// </summary>
    public class SystemProcessHookForm : Form
    {
        private readonly int msgNotify;
        public delegate void EventHandler(object sender, IntPtr hWnd, Interop.ShellEvents shell);
        public event EventHandler WindowEvent;
        protected virtual void OnWindowEvent(IntPtr hWnd, Interop.ShellEvents shell)
        {
            var handler = WindowEvent;
            if (handler != null)
            {
                handler(this, hWnd, shell);
            }
        }

        public SystemProcessHookForm()
        {
            // Hook on to the shell
            msgNotify = Interop.RegisterWindowMessage("SHELLHOOK");
            Interop.RegisterShellHookWindow(this.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == msgNotify)
            {
                // Receive shell messages
                switch ((Interop.ShellEvents)m.WParam.ToInt32())
                {
                    case Interop.ShellEvents.HSHELL_WINDOWCREATED:
                    case Interop.ShellEvents.HSHELL_WINDOWDESTROYED:
                    case Interop.ShellEvents.HSHELL_WINDOWACTIVATED:
                        //string wName = GetWindowName(m.LParam);
                        //var action = (Interop.ShellEvents)m.WParam.ToInt32();
                        //string.Format("{0} - {1}: {2}", action, m.LParam, wName)
                        OnWindowEvent(m.LParam, (Interop.ShellEvents)m.WParam.ToInt32());
                        break;
                }
            }
            base.WndProc(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            try { Interop.DeregisterShellHookWindow(this.Handle); }
            catch { }
            base.Dispose(disposing);
        }

    }
}
