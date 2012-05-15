using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FM;

namespace BasicFirewall
{
    public partial class AddEditRule : Form
    {
        private Rule newRule;
        public Rule NewRule
        {
            get { return newRule; }
            set { newRule = value; }
        }
        
        public AddEditRule()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor for initializing the GUi when editing a rule
        /// </summary>
        /// <param name="tmp"></param>
        public AddEditRule(Rule tmp)
        {
            InitializeComponent();

            // force the combo box to load the rules
            AddEditRule_Load(this, null);

            // disallow changing the base type of the rule
            comboBox1.Enabled = false;

            // set the rule type and GUI options
            if (tmp is AllRule)
            {
                AllRule t = (AllRule)tmp;
                checkBoxLog.Checked = t.log;

                // set the rule type
                comboBox1.SelectedIndex = 5;

                // set the direction
                checkBoxIn.Checked = ((t.direction & Direction.IN) != 0) ? true : false;
                checkBoxOut.Checked = ((t.direction & Direction.OUT) != 0) ? true : false;

                // set the action box
                comboBoxAction.SelectedIndex = ((t.ps & PacketStatus.ALLOWED) != 0) ? 1 : 0;

                //notify
                notifyBox.Checked = (t.notify);
            }
            else if (tmp is IPRule)
            {
                IPRule t = (IPRule)tmp;
                checkBoxLog.Checked = t.log;

                //idx
                comboBox1.SelectedIndex = 6;

                //direction
                checkBoxIn.Checked = ((t.direction & Direction.IN) != 0) ? true : false;
                checkBoxOut.Checked = ((t.direction & Direction.OUT) != 0) ? true : false;

                //action
                comboBoxAction.SelectedIndex = ((t.ps & PacketStatus.ALLOWED) != 0) ? 1 : 0;

                //args
                textBoxArguments.Text = t.GetIPString();

                //notify
                notifyBox.Checked = (t.notify);
            }
            else if (tmp is TCPAllRule)
            {
                TCPAllRule t = (TCPAllRule)tmp;
                checkBoxLog.Checked = t.log;

                //idx
                comboBox1.SelectedIndex = 0;

                //dir
                checkBoxIn.Checked = ((t.direction & Direction.IN) != 0) ? true : false;
                checkBoxOut.Checked = ((t.direction & Direction.OUT) != 0) ? true : false;

                //action
                comboBoxAction.SelectedIndex = ((t.ps & PacketStatus.ALLOWED) != 0) ? 1 : 0;

                //notify
                notifyBox.Checked = (t.notify);
            }
            else if (tmp is TCPIPPortRule)
            {
                TCPIPPortRule t = (TCPIPPortRule)tmp;
                checkBoxLog.Checked = t.log;

                //idx
                comboBox1.SelectedIndex = 1;

                //dir
                checkBoxIn.Checked = ((t.direction & Direction.IN) != 0) ? true : false;
                checkBoxOut.Checked = ((t.direction & Direction.OUT) != 0) ? true : false;

                //action
                comboBoxAction.SelectedIndex = ((t.ps & PacketStatus.ALLOWED) != 0) ? 1 : 0;

                //args
                textBoxArguments.Text = String.Format("{0} {1}", t.ip, t.port);

                //notify
                notifyBox.Checked = (t.notify);
            }
            else if (tmp is TCPPortRule)
            {
                TCPPortRule t = (TCPPortRule)tmp;
                checkBoxLog.Checked = t.log;

                //idx
                comboBox1.SelectedIndex = 2;

                //dir
                checkBoxIn.Checked = ((t.direction & Direction.IN) != 0) ? true : false;
                checkBoxOut.Checked = ((t.direction & Direction.OUT) != 0) ? true : false;

                //action
                comboBoxAction.SelectedIndex = ((t.ps & PacketStatus.ALLOWED) != 0) ? 1 : 0;

                //args
                textBoxArguments.Text = t.GetPortString();

                //notify
                notifyBox.Checked = (t.notify);
            }
            else if (tmp is UDPAllRule)
            {
                UDPAllRule t = (UDPAllRule)tmp;
                checkBoxLog.Checked = t.log;

                //idx
                comboBox1.SelectedIndex = 3;

                //dir
                checkBoxIn.Checked = ((t.direction & Direction.IN) != 0) ? true : false;
                checkBoxOut.Checked = ((t.direction & Direction.OUT) != 0) ? true : false;

                //action
                comboBoxAction.SelectedIndex = ((t.ps & PacketStatus.ALLOWED) != 0) ? 1 : 0;

                //notify
                notifyBox.Checked = (t.notify);
            }
            else if (tmp is UDPPortRule)
            {
                UDPPortRule t = (UDPPortRule)tmp;
                checkBoxLog.Checked = t.log;

                //idx
                comboBox1.SelectedIndex = 4;

                //dir
                checkBoxIn.Checked = ((t.direction & Direction.IN) != 0) ? true : false;
                checkBoxOut.Checked = ((t.direction & Direction.OUT) != 0) ? true : false;

                //action
                comboBoxAction.SelectedIndex = ((t.ps & PacketStatus.ALLOWED) != 0) ? 1 : 0;

                //args
                textBoxArguments.Text = t.GetPortString();

                //notify
                notifyBox.Checked = (t.notify);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tmp = "";
            bool enableArgs = true;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                case 3:
                case 5:
                    // most of these aren't going to translate very differently 
                    // into their foreign tongue, so i'm leaving it as is
                    tmp = "No args";
                    enableArgs = false;
                    break;
                case 2:
                case 4:
                    tmp = "(Space Separated) Port";
                    break;
                case 1:
                    tmp = "(Space Separated) IP Port";
                    break;
                case 6:
                    tmp = "(Space Separated) IP";
                    break;
            }

            labelArgs.Text = tmp;
            textBoxArguments.Enabled = enableArgs;
        }

        /// <summary>
        /// Event handles the OK button to generate a new rule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBoxIn.Checked || checkBoxOut.Checked)
            {
                try
                {
                    RuleType rt = RuleType.ALL;
                    switch (comboBox1.SelectedIndex)
                    {
                        case 3:
                            rt = RuleType.UDPALL;
                            break;
                        case 4:
                            rt = RuleType.UDPPORT;
                            break;
                        case 1:
                            rt = RuleType.TCPIPPORT;
                            break;
                        case 2:
                            rt = RuleType.TCPPORT;
                            break;
                        case 0:
                            rt = RuleType.TCPALL;
                            break;
                        case 5:
                            rt = RuleType.ALL;
                            break;
                        case 6:
                            rt = RuleType.IP;
                            break;
                    }
                    Direction dir;
                    if (checkBoxIn.Checked && checkBoxOut.Checked)
                    {
                        dir = Direction.IN | Direction.OUT;
                    }
                    else if (checkBoxOut.Checked)
                    {
                        dir = Direction.OUT;
                    }
                    else
                    {
                        dir = Direction.IN;
                    }
                    PacketStatus ps;
                    if (comboBoxAction.Text == "Block")
                        ps = PacketStatus.BLOCKED;
                    else
                        ps = PacketStatus.ALLOWED;

                    // multiple ports and the ip:port rule are parsed later on, so send all args as a string

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    newRule = BasicFirewallModule.RuleFactory.MakeRule(rt, ps, dir, textBoxArguments.Text, 
                                                            checkBoxLog.Checked, notifyBox.Checked);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in creating rule.");
                    //LogCenter.WriteErrorLog(exception);
                }
            }
            else
            {
                MessageBox.Show("You need to select in or out first");
            }
        }

        /// <summary>
        /// Event handles the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddEditRule_Load(object sender, EventArgs e)
        {
            switch (LanguageConfig.GetCurrentLanguage())
            {
                case LanguageConfig.Language.ENGLISH:
                    comboBox1.Items.Add("All TCP Rule");//0
                    comboBox1.Items.Add("TCP IP and Port Rule");//1
                    comboBox1.Items.Add("TCP Port Rule");//2
                    comboBox1.Items.Add("All UDP Rule");//3
                    comboBox1.Items.Add("UDP Port Rule");//4
                    comboBox1.Items.Add("All Rule");//5
                    comboBox1.Items.Add("IP Rule");//6
                    break;
                case LanguageConfig.Language.PORTUGUESE:
                    comboBox1.Items.Add("Todos Regra TCP");
                    comboBox1.Items.Add("TCP IP e regra de porta");
                    comboBox1.Items.Add("Porta TCP Regra");
                    comboBox1.Items.Add("Todos Regra UDP");
                    comboBox1.Items.Add("Porta UDP Regra");
                    comboBox1.Items.Add("Todos Regra");
                    comboBox1.Items.Add("Regra IP");
                    break;
                case LanguageConfig.Language.CHINESE:
                    comboBox1.Items.Add("所有的TCP規則");
                    comboBox1.Items.Add("TCP IP和端口規則");
                    comboBox1.Items.Add("TCP端口規則");
                    comboBox1.Items.Add("所有的UDP規則");
                    comboBox1.Items.Add("UDP端口規則");
                    comboBox1.Items.Add("所有規則");
                    comboBox1.Items.Add("IP规则");
                    break;
                case LanguageConfig.Language.GERMAN:
                    comboBox1.Items.Add("Alle TCP Rule");
                    comboBox1.Items.Add("TCP IP und Port Regel");
                    comboBox1.Items.Add("TCP Port Rule");
                    comboBox1.Items.Add("Alle UDP Regel");
                    comboBox1.Items.Add("UDP Port Rule");
                    comboBox1.Items.Add("Alle Rule");
                    comboBox1.Items.Add("IP Rule");
                    break;
                case LanguageConfig.Language.RUSSIAN:
                    comboBox1.Items.Add("Все правила TCP");
                    comboBox1.Items.Add("TCP-IP и порт Правило");
                    comboBox1.Items.Add("TCP-порт Правило");
                    comboBox1.Items.Add("Все правила UDP");
                    comboBox1.Items.Add("UDP-порта Правило");
                    comboBox1.Items.Add("Все правила");
                    comboBox1.Items.Add("IP правила");
                    break;
                case LanguageConfig.Language.SPANISH:
                    comboBox1.Items.Add("Todos los Regla TCP");
                    comboBox1.Items.Add("TCP IP y puerto de Regla");
                    comboBox1.Items.Add("Puerto TCP Regla");
                    comboBox1.Items.Add("Todos los Regla UDP");
                    comboBox1.Items.Add("El puerto UDP Regla");
                    comboBox1.Items.Add("todos los Regla");
                    comboBox1.Items.Add("Regla IP");
                    break;
            }
        }

        /// <summary>
        /// if checked, lock Log into a check state as well since we can't notify without
        /// logging enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyBox_CheckedChanged(object sender, EventArgs e)
        {
            if (notifyBox.Checked)
            {
                checkBoxLog.Checked = true;
                checkBoxLog.Enabled = false;
            }
            else
            {
                checkBoxLog.Checked = false;
                checkBoxLog.Enabled = true;
            }
        }
    }
}
