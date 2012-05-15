using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;

namespace SimpleMacEncryption
{
    public partial class MacEntryControl : UserControl
    {
        SimpleMacEncryptionSend sme;

        public MacEntryControl(SimpleMacEncryptionSend sme)
        {
            this.sme = sme;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sme.SetOtherMac(PhysicalAddress.Parse(textBox1.Text));
        }

        private void MacEntryControl_Load(object sender, EventArgs e)
        {

        }
    }
}
