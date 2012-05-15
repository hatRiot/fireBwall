using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FM;

namespace PassThru.Modules
{
    public partial class ModuleConfiguration : UserControl
    {
        //List<FirewallModule> modules = new List<FirewallModule>();
        List<KeyValuePair<bool, string>> moduleOrder = new List<KeyValuePair<bool, string>>();
        NetworkAdapter na;
        string dragged = null;
        bool loading = false;
        
        public ModuleConfiguration(NetworkAdapter na)
        {
            this.na = na;
            moduleOrder = na.modules.GetModuleOrder();
            InitializeComponent();
        }

        void UpdateView()
        {
            if (this.checkedListBoxModules.InvokeRequired)
            {
                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(UpdateView);
                this.checkedListBoxModules.Invoke(ts);
            }
            else
            {
                lock (this)
                {
                    loading = true;
                    checkedListBoxModules.Items.Clear();
                    for (int x = 0; x < moduleOrder.Count; x++)
                    {
                        checkedListBoxModules.Items.Add(moduleOrder[x].Value, moduleOrder[x].Key);
                    }
                    loading = false;
                }
            }
        }

        private void ModuleConfiguration_Load(object sender, EventArgs e)
        {
            loading = true;
            UpdateView();
            switch (FM.LanguageConfig.GetCurrentLanguage())
            {
                case FM.LanguageConfig.Language.NONE:
                case FM.LanguageConfig.Language.ENGLISH:
                    buttonEnable.Text = "Enable/Disable";
                    buttonOpenConfiguration.Text = "Open Configuration";
                    buttonHelp.Text = "Help";
                    buttonMoveDown.Text = "Move Down";
                    buttonMoveUp.Text = "Move Up";
                    break;
                case FM.LanguageConfig.Language.PORTUGUESE:
                    buttonEnable.Text = "Activar / Desactivar";
                    buttonOpenConfiguration.Text = "Abrir Configuração";
                    buttonHelp.Text = "ajudar";
                    buttonMoveDown.Text = "mover para Baixo";
                    buttonMoveUp.Text = "mover para cima";
                    break;
                case FM.LanguageConfig.Language.RUSSIAN:
                    buttonEnable.Text = "Включение / выключение";
                    buttonOpenConfiguration.Text = "Открытая конфигурация";
                    buttonHelp.Text = "Помогите";
                    buttonMoveDown.Text = "спускать";
                    buttonMoveUp.Text = "вверх";
                    break;
                case FM.LanguageConfig.Language.SPANISH:
                    buttonEnable.Text = "Activar / Desactivar";
                    buttonOpenConfiguration.Text = "Abrir Configuración";
                    buttonHelp.Text = "ayuda";
                    buttonMoveDown.Text = "Bajar";
                    buttonMoveUp.Text = "Subir";
                    break;
                case FM.LanguageConfig.Language.CHINESE:
                    buttonEnable.Text = "启用/禁用";
                    buttonOpenConfiguration.Text = "打开配置";
                    buttonHelp.Text = "帮助";
                    buttonMoveDown.Text = "下移";
                    buttonMoveUp.Text = "动起来";
                    break;
                case FM.LanguageConfig.Language.GERMAN:
                    buttonEnable.Text = "Aktivieren / Deaktivieren";
                    buttonOpenConfiguration.Text = "Konfiguration öffnen";
                    buttonHelp.Text = "Hilfe";
                    buttonMoveDown.Text = "Nach unten";
                    buttonMoveUp.Text = "Nach oben";
                    break;
            }
            loading = false;
            if (checkedListBoxModules.Items.Count != 0)
            {
                checkedListBoxModules.SelectedIndex = 0;
            }
            ColorScheme.SetColorScheme(this);
            ColorScheme.ThemeChanged += new System.Threading.ThreadStart(ColorScheme_ThemeChanged);
        }

        void ColorScheme_ThemeChanged()
        {
            ColorScheme.SetColorScheme(this);
        }

        private void buttonEnable_Click(object sender, EventArgs e)
        {
            try
            {
                int temp = checkedListBoxModules.SelectedIndex;
                moduleOrder[checkedListBoxModules.SelectedIndex] = new KeyValuePair<bool, string>(!moduleOrder[checkedListBoxModules.SelectedIndex].Key, moduleOrder[checkedListBoxModules.SelectedIndex].Value);
                na.modules.UpdateModuleOrder(moduleOrder);
                moduleOrder = na.modules.GetModuleOrder();
                UpdateView();
                checkedListBoxModules.SelectedIndex = temp;
            }
            catch (Exception ne)
            {
                LogCenter.WriteErrorLog(ne);
            }       
        }

        private void checkedListBoxModules_ItemCheck_1(object sender, ItemCheckEventArgs e)
        {
            lock (this)
            {
                if (!loading && moduleOrder.Count > e.Index && e.Index != -1)
                {
                    moduleOrder[e.Index] = new KeyValuePair<bool, string>(e.NewValue == CheckState.Checked, moduleOrder[e.Index].Value);
                    na.modules.UpdateModuleOrder(moduleOrder);
                    moduleOrder = na.modules.GetModuleOrder();
                }
            }
        }        

        private void buttonOpenConfiguration_Click(object sender, EventArgs e)
        {
            try
            {
                UserControl uc = na.modules.GetModule(checkedListBoxModules.SelectedIndex).GetControl();
                if (uc != null)
                {
                    ThemedForm f = new ThemedForm();
                    f.Size = new System.Drawing.Size(640, 480);
                    System.Reflection.Assembly target = System.Reflection.Assembly.GetExecutingAssembly();
                    f.Icon = new System.Drawing.Icon(target.GetManifestResourceStream("PassThru.Resources.newIcon.ico"));
                    f.Text = na.InterfaceInformation.Name + ": " + na.modules.GetModule(checkedListBoxModules.SelectedIndex).MetaData.Name + " - " + na.modules.GetModule(checkedListBoxModules.SelectedIndex).MetaData.Version;
                    f.Controls.Add(uc);
                    ColorScheme.SetColorScheme(f);
                    f.Show();
                }
            }
            catch (Exception ne)
            {
                LogCenter.WriteErrorLog(ne);
            }
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkedListBoxModules.SelectedIndex != 0)
                {
                    KeyValuePair<bool, string> temp = moduleOrder[checkedListBoxModules.SelectedIndex];
                    moduleOrder.RemoveAt(checkedListBoxModules.SelectedIndex);
                    moduleOrder.Insert(checkedListBoxModules.SelectedIndex - 1, temp);
                    na.modules.UpdateModuleOrder(moduleOrder);
                    moduleOrder = na.modules.GetModuleOrder();
                    int newIndex = checkedListBoxModules.SelectedIndex - 1;
                    UpdateView();
                    checkedListBoxModules.SelectedIndex = newIndex;
                }
            }
            catch(Exception ne)
            {
                LogCenter.WriteErrorLog(ne);
            }
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkedListBoxModules.SelectedIndex != moduleOrder.Count - 1)
                {
                    KeyValuePair<bool, string> temp = moduleOrder[checkedListBoxModules.SelectedIndex];
                    moduleOrder.RemoveAt(checkedListBoxModules.SelectedIndex);
                    moduleOrder.Insert(checkedListBoxModules.SelectedIndex + 1, temp);
                    na.modules.UpdateModuleOrder(moduleOrder);
                    moduleOrder = na.modules.GetModuleOrder();
                    int newIndex = checkedListBoxModules.SelectedIndex + 1;
                    UpdateView();
                    checkedListBoxModules.SelectedIndex = newIndex;
                }
            }
            catch (Exception ne)
            {
                LogCenter.WriteErrorLog(ne);
            }
        }

        /// <summary>
        /// Builds the module Help window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            try
            {
                ThemedForm f = new ThemedForm();
                f.Size = new System.Drawing.Size(640, 480);
                System.Reflection.Assembly target = System.Reflection.Assembly.GetExecutingAssembly();
                f.Icon = new System.Drawing.Icon(target.GetManifestResourceStream("PassThru.Resources.newIcon.ico"));
                f.Text = "Help";
                Help uc = new Help(checkedListBoxModules.SelectedItem);
                uc.Dock = DockStyle.Fill;
                f.Controls.Add(uc);
                ColorScheme.SetColorScheme(f);
                f.Show();
            }
            catch(Exception ne)
            { 
                LogCenter.WriteErrorLog(ne); 
            }
        }
    }
}
