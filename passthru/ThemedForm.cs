using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PassThru
{
    public partial class ThemedForm : Form
    {
        public ThemedForm()
        {
            InitializeComponent();
        }

        private void ThemedForm_Load(object sender, EventArgs e)
        {
            ColorScheme.ThemeChanged += new System.Threading.ThreadStart(ColorScheme_ThemeChanged);
        }

        void ColorScheme_ThemeChanged()
        {
            ColorScheme.SetColorScheme(this);
        }
    }
}
