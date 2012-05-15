using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Net;
using System.Windows.Forms;
using FM;

namespace PoisonIvy
{
    public partial class DNSUI : UserControl
    {
        private PoisonIvy ivy;
        public DNSUI(PoisonIvy ivy)
        {
            InitializeComponent();
            this.ivy = ivy;
        }
    }
}
