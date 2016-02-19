using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wowDisableWinKey.Controls
{
    public partial class BindControl : UserControl
    {
        private List<GBC_ModItem> modifierList = new List<GBC_ModItem>
        {
            new GBC_ModItem{Mod = GBC_KeyModifier.Shift, ModTitle="Shift"},
            new GBC_ModItem{Mod = GBC_KeyModifier.Control, ModTitle="Control"},
            new GBC_ModItem{Mod = GBC_KeyModifier.Alt, ModTitle="Alt"},
            new GBC_ModItem{Mod = GBC_KeyModifier.WinKey, ModTitle="Windows"},
            new GBC_ModItem{Mod = GBC_KeyModifier.ShiftControl, ModTitle="Control+Shift"},
            new GBC_ModItem{Mod = GBC_KeyModifier.ShiftAlt, ModTitle="Shift+Alt"},
            new GBC_ModItem{Mod = GBC_KeyModifier.ControlAlt, ModTitle="Control+Alt"},
            new GBC_ModItem{Mod = GBC_KeyModifier.ShiftControlAlt, ModTitle="Shift+Control+Alt"}
        };
        private List<BindItem> bindList = new List<BindItem>();
        private List<Keys> exeptionBind = new List<Keys>
        {
            Keys.Shift, Keys.ShiftKey, Keys.LShiftKey, Keys.RShiftKey, Keys.Control, Keys.ControlKey, Keys.LControlKey, Keys.RControlKey, Keys.Alt, Keys.LMenu, Keys.RMenu, Keys.Menu, Keys.Modifiers, Keys.NoName
        };

        public Keys Bind {
            get { return (Keys)bindComboBox.SelectedValue; }
            set { bindComboBox.SelectedValue = value; }
        }

        public GBC_KeyModifier Modifier
        {
            get { return (GBC_KeyModifier)modComboBox.SelectedValue;}
            set{modComboBox.SelectedValue = value;}
        }
        public String BindString
        {
            get { return (bindComboBox.SelectedItem as string);}
        }
        public String ModifierString
        {
            get { return (modComboBox.SelectedItem as string); }
        }


        private event EventHandler m_SelectedBindChanged;
        public event EventHandler SelectedBindChanged
        {
            add
            {
                if (m_SelectedBindChanged == null)
                {
                    this.bindComboBox.SelectedIndexChanged += bindComboBox_SelectedIndexChanged;
                }
                m_SelectedBindChanged += value;
            }
            remove
            {
                m_SelectedBindChanged -= value;
                if (m_SelectedBindChanged == null)
                {
                    this.bindComboBox.SelectedIndexChanged -= bindComboBox_SelectedIndexChanged;
                }
            }
        }
        private void bindComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_SelectedBindChanged != null)
            {
                m_SelectedBindChanged.Invoke(this, e);
            }
        }

        private event EventHandler m_SelectedModifierChanged;
        public event EventHandler SelectedModifierChanged
        {
            add
            {
                if (m_SelectedModifierChanged == null)
                {
                    this.modComboBox.SelectedIndexChanged += modComboBox_SelectedIndexChanged;
                }
                m_SelectedModifierChanged += value;
            }
            remove
            {
                m_SelectedModifierChanged -= value;
                if (m_SelectedBindChanged == null)
                {
                    this.modComboBox.SelectedIndexChanged -= modComboBox_SelectedIndexChanged;
                }
            }
        }
        private void modComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_SelectedModifierChanged != null)
            {
                m_SelectedModifierChanged.Invoke(this, e);
            }
        }


        public BindControl()
        {
            
            InitializeComponent();
            GenerateBindList(ref bindList, exeptionBind);

            bindComboBox.DataSource = bindList;
            modComboBox.DataSource = modifierList;

        }
        public BindControl(List<BindItem> binds, List<GBC_ModItem> modifiers)
        {
            InitializeComponent();
            GenerateBindList(ref bindList, exeptionBind);

            bindComboBox.DataSource = binds;
            modComboBox.DataSource = modifiers;
        }
        private void GenerateBindList(ref List<BindItem> blblbl, List<Keys> exception)
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if(!exception.Contains(key))
                    blblbl.Add(new BindItem { Bind = key, BindTitle = key.ToString() });
            }
        }
       
    }

    public class GBC_ModItem
    {
        public GBC_KeyModifier Mod { get; set; }
        public string ModTitle { get; set; }
    }
    public class BindItem
    {
        public Keys Bind { get; set; }
        public string BindTitle { get; set; }
    }
}
