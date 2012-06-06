using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using fireBwall.Utils;
using fireBwall.Configuration;
using fireBwall.Logging;

namespace fireBwall.UI.Tabs
{
    public partial class ColorSchemeEditor : DynamicForm
    {
        string themeName;
        ColorScheme theme;

        public ColorSchemeEditor(string themeName) : base()
        {
            this.themeName = themeName;
            theme = new ColorScheme();
            ColorScheme temp = ThemeConfiguration.Instance.Schemes[themeName];
            theme.Base64Image = temp.Base64Image;
            theme.colors = new SerializableDictionary<string, ColorScheme.Color>(temp.colors);
            theme.Name = temp.Name;
            InitializeComponent();
        }

        private void ColorSchemeEditor_Load(object sender, EventArgs e)
        {
            this.Text = themeName;
            textBox1.Text = themeName;
            DataSet ds = new DataSet();
            foreach (KeyValuePair<string, ColorScheme.Color> i in theme.colors)
            {
                dataGridView1.Rows.Add((string)i.Key, i.Value.Alpha, i.Value.Red, i.Value.Green, i.Value.Blue);
            }
            ThemeChanged();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            themeName = textBox1.Text;
            this.Text = themeName;
            theme.Name = themeName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    theme.colors[(string)row.Cells[0].Value] = new ColorScheme.Color(Convert.ToByte(row.Cells[1].Value), Convert.ToByte(row.Cells[2].Value), Convert.ToByte(row.Cells[3].Value), Convert.ToByte(row.Cells[4].Value));
                }
            }
            catch { }
            ThemeConfiguration.Instance.AddScheme(theme);
            ThemeConfiguration.Instance.Save();
            ThemeConfiguration.Instance.ChangeTheme(theme.Name, true);
        }

        [STAThread]
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = "c:\\";
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    theme.Base64Image = Convert.ToBase64String(File.ReadAllBytes(ofd.FileName));
                    button1_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                LogCenter.Instance.LogException(ex);
            }
        }
    }
}
