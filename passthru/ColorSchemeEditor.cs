using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PassThru
{
    public partial class ColorSchemeEditor : Form
    {
        string themeName;
        SerializableDictionary<string, string> theme;

        public ColorSchemeEditor(string themeName)
        {
            this.themeName = themeName;
            theme = new SerializableDictionary<string,string>(ColorScheme.themes[themeName]);
            InitializeComponent();
        }

        private void ColorSchemeEditor_Load(object sender, EventArgs e)
        {
            this.Text = themeName;
            textBox1.Text = themeName;
            DataSet ds = new DataSet();
            foreach (KeyValuePair<string, string> i in theme)
            {
                if (i.Key != "BannerImage")
                {
                    string[] split = i.Value.Split(':');
                    dataGridView1.Rows.Add((string)i.Key, split[0], split[1], split[2], split[3]);
                }
            }
            ColorScheme.SetColorScheme(this);
            ColorScheme.ThemeChanged += new System.Threading.ThreadStart(ColorScheme_ThemeChanged);
        }

        void ColorScheme_ThemeChanged()
        {
            ColorScheme.SetColorScheme(this);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            themeName = textBox1.Text;
            this.Text = themeName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    theme[(string)row.Cells[0].Value] = ((string)row.Cells[1].Value) + ":" +((string)row.Cells[2].Value) + ":" + ((string)row.Cells[3].Value) + ":" + ((string)row.Cells[4].Value);
                }
            }
            catch { }
            ColorScheme.themes[themeName] = theme;
            ColorScheme.Save();
            ColorScheme.ChangeTheme(themeName);
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
                    theme["BannerImage"] = Convert.ToBase64String(File.ReadAllBytes(ofd.FileName));
                    button1_Click(null, null);
                }
            }
            catch { }
        }
    }
}
