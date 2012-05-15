using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FM;

namespace PassThru
{
    public partial class AdapterDisplay : UserControl
    {
        public AdapterInfo ai;

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

        public AdapterDisplay(AdapterInfo ai)
        {
            if (null != ai)
            {
                this.ai = ai;
                InitializeComponent();
                textBoxDetails.Text = ai.Summary;
                LogCenter.ti.adapters.MenuItems.Add(new MenuItem(ai.NIName, this.buttonConfig_Click));
            }
        }

        private void buttonConfig_Click(object sender, EventArgs e)
        {
            Form f = new Form();
            f.Text = ai.NIName;
            f.Width = 640;
            f.Height = 480;
            f.Controls.Add(new Modules.ModuleConfiguration(ai.NetAdapter) { Dock = DockStyle.Fill });
            System.Reflection.Assembly target = System.Reflection.Assembly.GetExecutingAssembly();
            f.Icon = new System.Drawing.Icon(target.GetManifestResourceStream("PassThru.Resources.newIcon.ico"));
            ColorScheme.SetColorScheme(f);
            f.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void AdapterDisplay_Load(object sender, EventArgs e)
        {
            switch (LanguageConfig.GetCurrentLanguage())
            {
                case LanguageConfig.Language.NONE:
                case LanguageConfig.Language.ENGLISH:
                    button1.Text = "Enabled";
                    buttonConfig.Text = "Configure Device";
                    break;
                case LanguageConfig.Language.PORTUGUESE:
                    button1.Text = "Permitir";
                    buttonConfig.Text = "Configurar Dispositivo";
                    break;
                case LanguageConfig.Language.CHINESE:
                    button1.Text = "启用";
                    buttonConfig.Text = "配置设备";
                    break;
                case LanguageConfig.Language.GERMAN:
                    button1.Text = "ermöglichen";
                    buttonConfig.Text = "Gerät konfigurieren";
                    break;
                case LanguageConfig.Language.RUSSIAN:
                    button1.Text = "Включить";
                    buttonConfig.Text = "Настройка устройства";
                    break;
                case LanguageConfig.Language.SPANISH:
                    button1.Text = "permitir";
                    buttonConfig.Text = "configurar dispositivo";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ai.NetAdapter.Enabled = !ai.NetAdapter.Enabled;
            if (ai.NetAdapter.Enabled)
            {
                switch (LanguageConfig.GetCurrentLanguage())
                {
                    case LanguageConfig.Language.NONE:
                    case LanguageConfig.Language.ENGLISH:
                        button1.Text = "Enabled";
                        break;
                    case LanguageConfig.Language.PORTUGUESE:
                        button1.Text = "Permitir";
                        break;
                    case LanguageConfig.Language.CHINESE:
                        button1.Text = "启用";
                        break;
                    case LanguageConfig.Language.GERMAN:
                        button1.Text = "ermöglichen";
                        break;
                    case LanguageConfig.Language.RUSSIAN:
                        button1.Text = "Включить";
                        break;
                    case LanguageConfig.Language.SPANISH:
                        button1.Text = "permitir";
                        break;
                }
            }
            else
            {
                switch (LanguageConfig.GetCurrentLanguage())
                {
                    case LanguageConfig.Language.NONE:
                    case LanguageConfig.Language.ENGLISH:
                        button1.Text = "Disabled";
                        break;
                    case LanguageConfig.Language.PORTUGUESE:
                        button1.Text = "desativado";
                        break;
                    case LanguageConfig.Language.CHINESE:
                        button1.Text = "禁用";
                        break;
                    case LanguageConfig.Language.GERMAN:
                        button1.Text = "deaktiviert";
                        break;
                    case LanguageConfig.Language.RUSSIAN:
                        button1.Text = "инвалид";
                        break;
                    case LanguageConfig.Language.SPANISH:
                        button1.Text = "discapacitado";
                        break;
                }
            }
        }
    }
}
