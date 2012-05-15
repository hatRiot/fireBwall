using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FM;

namespace PoisonIvy
{
    public partial class DHCPUI : UserControl
    {
        private PoisonIvy ivy;
        public DHCPUI(PoisonIvy ivy)
        {
            InitializeComponent();
            this.ivy = ivy;
        }
    }
}
