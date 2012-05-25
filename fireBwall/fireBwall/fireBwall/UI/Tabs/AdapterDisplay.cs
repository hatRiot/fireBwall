using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using fireBwall.Filters.NDIS;
using fireBwall.Configuration;
using fireBwall.Logging;

namespace fireBwall.UI.Tabs
{
    public partial class AdapterDisplay : DynamicUserControl
    {
        public AdapterInformation ai;

        public void Update()
        {
            if (textBoxDetails.InvokeRequired)
            {
                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(Update);
                textBoxDetails.Invoke(ts);
            }
            else
            {
                textBoxDetails.Text = ai.Summary;
            }
        }

        public AdapterDisplay(AdapterInformation ai)
        {
            multistring.SetString(Language.ENGLISH, "Enabled", "Enabled");
            multistring.SetString(Language.ENGLISH, "Disabled", "Disabled");
            multistring.SetString(Language.ENGLISH, "Configure Device", "Configure Device");

            multistring.SetString(Language.PORTUGUESE, "Enabled", "Permitir");
            multistring.SetString(Language.PORTUGUESE, "Disabled", "Desativado");
            multistring.SetString(Language.PORTUGUESE, "Configure Device", "Configurar Dispositivo");

            multistring.SetString(Language.CHINESE, "Enabled", "启用");
            multistring.SetString(Language.CHINESE, "Disabled", "禁用");
            multistring.SetString(Language.CHINESE, "Configure Device", "配置设备");

            multistring.SetString(Language.GERMAN, "Enabled", "Ermöglichen");
            multistring.SetString(Language.GERMAN, "Disabled", "Deaktiviert");
            multistring.SetString(Language.GERMAN, "Configure Device", "Gerät konfigurieren");

            multistring.SetString(Language.RUSSIAN, "Enabled", "Включить");
            multistring.SetString(Language.RUSSIAN, "Disabled", "инвалид");
            multistring.SetString(Language.RUSSIAN, "Configure Device", "Настройка устройства");

            multistring.SetString(Language.SPANISH, "Enabled", "Permitir");
            multistring.SetString(Language.SPANISH, "Disabled", "Discapacitado");
            multistring.SetString(Language.SPANISH, "Configure Device", "Configurar Dispositivo");
            if (null != ai)
            {
                this.ai = ai;
                InitializeComponent();
                textBoxDetails.Text = ai.Summary;
                Program.trayIcon.adapters.MenuItems.Add(new MenuItem(ai.Name, this.buttonConfig_Click));
            }
        }

        private void buttonConfig_Click(object sender, EventArgs e)
        {
            DynamicForm f = new DynamicForm();
            f.Text = ai.Name;
            f.Width = 640;
            f.Height = 480;
            f.Controls.Add(new ModuleConfiguration(ai.Adapter) { Dock = DockStyle.Fill });
            f.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void AdapterDisplay_Load(object sender, EventArgs e)
        {
            LanguageChanged();
        }

        public override void LanguageChanged()
        {
            if (ai.Adapter.Filtering)
                button1.Text = multistring.GetString("Enabled");
            else
                button1.Text = multistring.GetString("Disabled");
            buttonConfig.Text = multistring.GetString("Configure Device");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ai.Adapter.Filtering = !ai.Adapter.Filtering;
            LanguageChanged();
        }
    }
}
