using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using fireBwall.Configuration;

namespace fireBwall.UI.Tabs
{
    public partial class OptionsDisplay : DynamicUserControl
    {
        public OptionsDisplay()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event for when displayTrayLogs changes state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void displayTrayLogs_CheckedChanged(object sender, EventArgs e)
        {
            GeneralConfiguration.Instance.ShowPopups = displayTrayLogs.Checked;
        }

        /// <summary>
        /// Language load for option settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptionsDisplay_Load(object sender, EventArgs e)
        {
            foreach (string theme in ThemeConfiguration.Instance.Schemes.Keys)
                themeBox.Items.Add(theme);
            themeBox.SelectedItem = GeneralConfiguration.Instance.CurrentTheme;

            multistring.SetString(Language.ENGLISH, "Display Icon Popups", "Display Icon Popups");
            multistring.SetString(Language.ENGLISH, "Language: ", "Language: ");

            multistring.SetString(Language.PORTUGUESE, "Display Icon Popups", "Mostrar Popups Icon");
            multistring.SetString(Language.PORTUGUESE, "Language: ", "Linguagem: ");

            multistring.SetString(Language.CHINESE, "Display Icon Popups", "显示图标弹出窗口");
            multistring.SetString(Language.CHINESE, "Language: ", "語言標籤: ");

            multistring.SetString(Language.GERMAN, "Display Icon Popups", "Anzeige Icon Popups");
            multistring.SetString(Language.GERMAN, "Language: ", "Sprachensiegel: ");

            multistring.SetString(Language.RUSSIAN, "Display Icon Popups", "Показать Иконка всплывающие окна");
            multistring.SetString(Language.RUSSIAN, "Language: ", "Язык этикетки: ");

            multistring.SetString(Language.SPANISH, "Display Icon Popups", "Mostrar ventanas emergentes Icono");
            multistring.SetString(Language.SPANISH, "Language: ", "Lenguaje de etiquetas: ");

            switch (GeneralConfiguration.Instance.PreferredLanguage)
            {
                case null:
                case "en":
                    languageBox.SelectedIndex = 1;
                    break;
                case "pt":
                    languageBox.SelectedIndex = 6;
                    break;
                case "zh":
                    languageBox.SelectedIndex = 4;
                    break;
                case "de":
                    languageBox.SelectedIndex = 3;
                    break;
                case "ru":
                    languageBox.SelectedIndex = 5;
                    break;
                case "es":
                    languageBox.SelectedIndex = 2;
                    break;
            }
            checkBox1.Checked = GeneralConfiguration.Instance.CheckUpdateOnStartup;
            checkBox2.Checked = GeneralConfiguration.Instance.IntervaledUpdateChecks;
            checkBoxStartMinimized.Checked = GeneralConfiguration.Instance.StartMinimized;
            textBox1.Text = GeneralConfiguration.Instance.IntervaledUpdateMinutes.ToString();
            displayTrayLogs.Checked = GeneralConfiguration.Instance.ShowPopups;

            maxLogsBox.Text = Convert.ToString(GeneralConfiguration.Instance.MaxLogs);
            maxPcapBox.Text = Convert.ToString(GeneralConfiguration.Instance.MaxPcapLogs);
        }

        public override void LanguageChanged()
        {
            displayTrayLogs.Text = multistring.GetString("Display Icon Popups");
            languageLabel.Text = multistring.GetString("Language: ");
        }

        void ColorScheme_ThemeChanged()
        {
            themeBox.Items.Clear();
            foreach (string theme in ThemeConfiguration.Instance.Schemes.Keys)
                themeBox.Items.Add(theme);
            themeBox.SelectedItem = GeneralConfiguration.Instance.CurrentTheme;
            ThemeConfiguration.Instance.SetColorScheme(this);
        }

        /// <summary>
        /// Let the user select their language!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (languageBox.SelectedIndex)
            {
                case 0:
                    GeneralConfiguration.Instance.PreferredLanguage = null;
                    break;
                case 1:
                    GeneralConfiguration.Instance.PreferredLanguage = "en";
                    break;
                case 2:
                    GeneralConfiguration.Instance.PreferredLanguage = "es";
                    break;
                case 3:
                    GeneralConfiguration.Instance.PreferredLanguage = "de";
                    break;
                case 4:
                    GeneralConfiguration.Instance.PreferredLanguage = "zh";
                    break;
                case 5:
                    GeneralConfiguration.Instance.PreferredLanguage = "ru";
                    break;
                case 6:
                    GeneralConfiguration.Instance.PreferredLanguage = "pt";
                    break;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            GeneralConfiguration.Instance.IntervaledUpdateChecks = checkBox2.Checked;
            GeneralConfiguration.Instance.Save();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            GeneralConfiguration.Instance.CheckUpdateOnStartup = checkBox1.Checked;
            GeneralConfiguration.Instance.Save();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            uint v = 0;
            if (uint.TryParse(textBox1.Text, out v) && v >= 10)
            {
                GeneralConfiguration.Instance.IntervaledUpdateMinutes = v;
                GeneralConfiguration.Instance.Save();
            }
        }

        private void checkBoxStartMinimized_CheckedChanged(object sender, EventArgs e)
        {
            GeneralConfiguration.Instance.StartMinimized = checkBoxStartMinimized.Checked;
            GeneralConfiguration.Instance.Save();
        }

        private void themeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ThemeConfiguration.Instance.ChangeTheme(themeBox.SelectedText);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorSchemeEditor cse = new ColorSchemeEditor(GeneralConfiguration.Instance.CurrentTheme);
            cse.Width = 640;
            cse.Height = 480;
            cse.Show();
        }

        /// <summary>
        /// Stores the new max log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maxLogsBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                uint tmp = Convert.ToUInt32(maxLogsBox.Text);
                if (tmp < UInt32.MaxValue)
                {
                    GeneralConfiguration.Instance.MaxLogs = tmp;
                    GeneralConfiguration.Instance.Save();
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Stores the new max pcap log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maxPcapBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                uint tmp = Convert.ToUInt32(maxPcapBox.Text);
                if (tmp < UInt32.MaxValue)
                {
                    GeneralConfiguration.Instance.MaxPcapLogs = tmp;
                    GeneralConfiguration.Instance.Save();
                }
            }
            catch { }
        }
    }
}
