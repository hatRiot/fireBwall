using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PassThru
{
		public partial class RuleEditor: UserControl 
        {
            /// <summary>
            /// Loads the BasicFirewall module it is controlling
            /// </summary>
            /// <param name="basicFirewall"></param>
            public RuleEditor(BasicFirewall basicFirewall)
            {
                this.basicFirewall = basicFirewall;
                rules = new List<BasicFirewall.Rule>(basicFirewall.rules);
                InitializeComponent();
            }

            List<BasicFirewall.Rule> rules = new List<BasicFirewall.Rule>();
            private BasicFirewall basicFirewall;

            /// <summary>
            /// Loads the current rules into the gui
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
			private void RuleEditor_Load(object sender, EventArgs e) 
            {
				tcpWhiteListIn.Items.Clear();
				tcpWhiteListOut.Items.Clear();
				tcpDoNotNotifyIn.Items.Clear();
				tcpDoNotNotifyOut.Items.Clear();

				udpWhiteListPortIn.Items.Clear();
				udpWhiteListPortOut.Items.Clear();
				udpDoNotNotifyIn.Items.Clear();
				udpDoNotNotifyOut.Items.Clear();

                rules = new List<BasicFirewall.Rule>(basicFirewall.rules);
				foreach (BasicFirewall.Rule rule in rules)
				{
						if (rule.transport == Protocol.TCP)
						{
								if((rule.Direction & 0x1) == 0x1 && rule.Set == BasicFirewall.PacketStatus.ALLOWED)
										tcpWhiteListIn.Items.Add(rule.Port);

								if ((rule.Direction & 0x2) == 0x2 && rule.Set == BasicFirewall.PacketStatus.ALLOWED)
										tcpWhiteListOut.Items.Add(rule.Port);

								foreach (int i in rule.doNotNotifyIncoming)
								{
										tcpDoNotNotifyIn.Items.Add(i);
								}

								foreach (int i in rule.doNotNotifyOutgoing)
								{
										tcpDoNotNotifyOut.Items.Add(i);
								}
						}
						else if (rule.transport == Protocol.UDP)
						{
								if ((rule.Direction & 0x1) == 0x1 && rule.Set == BasicFirewall.PacketStatus.ALLOWED)
										udpWhiteListPortIn.Items.Add(rule.Port);

								if ((rule.Direction & 0x2) == 0x2 && rule.Set == BasicFirewall.PacketStatus.ALLOWED)
										udpWhiteListPortOut.Items.Add(rule.Port);

								foreach (int i in rule.doNotNotifyIncoming)
								{
										udpDoNotNotifyIn.Items.Add(i);
								}

								foreach (int i in rule.doNotNotifyOutgoing)
								{
										udpDoNotNotifyOut.Items.Add(i);
								}
						}
				}
			}

            /// <summary>
            /// Updates the rules in the module
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
			private void buttonApply_Click(object sender, EventArgs e) 
            {
                this.basicFirewall.InstanceGetRuleUpdates(rules);
			}

            /// <summary>
            /// Adds or removes port from this rule area
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
			private void tcpDoNotNotifyInputIn_KeyUp(object sender, KeyEventArgs e) 
            {
				if (e.KeyCode == Keys.Enter)
				{
					string input = tcpDoNotNotifyInputIn.Text;
					tcpDoNotNotifyInputIn.Text = "";
					int port = 0;
					if (int.TryParse(input, out port))
					{
						BasicFirewall.Rule found = null;
						BasicFirewall.Rule lastTCP = null;
						foreach (BasicFirewall.Rule r in rules)
						{
							if (r.transport == Protocol.TCP && (r.Direction & 0x1) == 0x1)
							{
								if (r.Port == port)
								{
									found = r;
									r.Notify = false;
									tcpDoNotNotifyIn.Items.Remove(port);
								}
								else if (r.doNotNotifyIncoming.Contains(port))
								{
									r.doNotNotifyIncoming.Remove(port);
									tcpDoNotNotifyIn.Items.Remove(port);
									found = r;
								}
								lastTCP = r;
							}
						}
						if (found == null)
						{
							lastTCP.doNotNotifyIncoming.Add(port);
							tcpDoNotNotifyIn.Items.Add(port);
						}
					}
				}
			}

            /// <summary>
            /// Adds or removes port from this rule area
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
			private void tcpDoNotNotifyInputOut_KeyUp(object sender, KeyEventArgs e) 
            {
				if (e.KeyCode == Keys.Enter)
				{
						string input = tcpDoNotNotifyInputOut.Text;
						tcpDoNotNotifyInputOut.Text = "";
						int port = 0;
						if (int.TryParse(input, out port))
						{
								BasicFirewall.Rule found = null;
								BasicFirewall.Rule lastTCP = null;
								foreach (BasicFirewall.Rule r in rules)
								{
										if (r.transport == Protocol.TCP && (r.Direction & 0x2) == 0x2)
										{
												if (r.Port == port)
												{
														found = r;
														r.Notify = false;
														tcpDoNotNotifyOut.Items.Remove(port);
												}
												else if (r.doNotNotifyOutgoing.Contains(port))
												{
														r.doNotNotifyOutgoing.Remove(port);
														tcpDoNotNotifyOut.Items.Remove(port);
														found = r;
												}
												lastTCP = r;
										}
								}
								if (found == null)
								{
										lastTCP.doNotNotifyOutgoing.Add(port);
										tcpDoNotNotifyOut.Items.Add(port);
								}
						}
				}
			}

            /// <summary>
            /// Adds or removes port from this rule area
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
			private void tcpWhiteListInputIn_KeyUp(object sender, KeyEventArgs e) 
            {
				if (e.KeyCode == Keys.Enter)
				{
						string input = tcpWhiteListInputIn.Text;
						tcpWhiteListInputIn.Text = "";
						int port = 0;
						if (int.TryParse(input, out port))
						{
								BasicFirewall.Rule found = null;
								foreach (BasicFirewall.Rule r in rules)
								{
										if (r.transport == Protocol.TCP)
										{
												if (r.Set == BasicFirewall.PacketStatus.ALLOWED && (r.Port == port || r.Port == -1) && (r.Direction & 0x1) == 0x1)
												{
														found = r;
														tcpWhiteListIn.Items.Remove(port);
														break;
												}
										}
								}
								if (found != null)
								{
										rules.Remove(found);
								}
								else
								{
										rules.Insert(0, new BasicFirewall.Rule(BasicFirewall.PacketStatus.ALLOWED, port, Protocol.TCP, false, false, null, 0x1));
										tcpWhiteListIn.Items.Add(port);
								}
						}
				}
			}

            /// <summary>
            /// Adds or removes port from this rule area
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
			private void tcpWhiteListInputOut_KeyUp(object sender, KeyEventArgs e) 
            {
				if (e.KeyCode == Keys.Enter)
				{
						string input = tcpWhiteListInputOut.Text;
						tcpWhiteListInputOut.Text = "";
						int port = 0;
						if (int.TryParse(input, out port))
						{
								BasicFirewall.Rule found = null;
								foreach (BasicFirewall.Rule r in rules)
								{
										if (r.transport == Protocol.TCP)
										{
												if (r.Set == BasicFirewall.PacketStatus.ALLOWED && (r.Port == port || r.Port == -1) && (r.Direction & 0x2) == 0x2)
												{
														found = r;
														tcpWhiteListOut.Items.Remove(port);
														break;
												}
										}
								}
								if (found != null)
								{
										rules.Remove(found);
								}
								else
								{
										rules.Insert(0, new BasicFirewall.Rule(BasicFirewall.PacketStatus.ALLOWED, port, Protocol.TCP, false, false, null, 0x2));
										tcpWhiteListOut.Items.Add(port);
								}
						}
				}
			}

            /// <summary>
            /// Adds or removes port from this rule area
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
			private void udpDoNotNotifyInputIn_KeyUp(object sender, KeyEventArgs e) 
            {
				if (e.KeyCode == Keys.Enter)
				{
						string input = udpDoNotNotifyInputIn.Text;
						udpDoNotNotifyInputIn.Text = "";
						int port = 0;
						if (int.TryParse(input, out port))
						{
								BasicFirewall.Rule found = null;
								BasicFirewall.Rule lastTCP = null;
								foreach (BasicFirewall.Rule r in rules)
								{
										if (r.transport == Protocol.UDP && (r.Direction & 0x1) == 0x1)
										{
												if (r.Port == port)
												{
														found = r;
														r.Notify = false;
														udpDoNotNotifyIn.Items.Remove(port);
												}
												else if (r.doNotNotifyIncoming.Contains(port))
												{
														r.doNotNotifyIncoming.Remove(port);
														udpDoNotNotifyIn.Items.Remove(port);
														found = r;
												}
												lastTCP = r;
										}
								}
								if (found == null)
								{
										lastTCP.doNotNotifyIncoming.Add(port);
										udpDoNotNotifyIn.Items.Add(port);
								}
						}
				}
			}

            /// <summary>
            /// Adds or removes port from this rule area
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
			private void udpDoNotNotifyInputOut_KeyUp(object sender, KeyEventArgs e) 
            {
				if (e.KeyCode == Keys.Enter)
				{
						string input = udpDoNotNotifyInputOut.Text;
						udpDoNotNotifyInputOut.Text = "";
						int port = 0;
						if (int.TryParse(input, out port))
						{
								BasicFirewall.Rule found = null;
								BasicFirewall.Rule lastTCP = null;
								foreach (BasicFirewall.Rule r in rules)
								{
										if (r.transport == Protocol.UDP && (r.Direction & 0x2) == 0x2)
										{
												if (r.Port == port)
												{
														found = r;
														r.Notify = false;
														udpDoNotNotifyOut.Items.Remove(port);
												}
												else if (r.doNotNotifyOutgoing.Contains(port))
												{
														r.doNotNotifyOutgoing.Remove(port);
														udpDoNotNotifyOut.Items.Remove(port);
														found = r;
												}
												lastTCP = r;
										}
								}
								if (found == null)
								{
										lastTCP.doNotNotifyOutgoing.Add(port);
										udpDoNotNotifyOut.Items.Add(port);
								}
						}
				}
			}

            /// <summary>
            /// Adds or removes port from this rule area
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
			private void udpWhiteListInputIn_KeyUp(object sender, KeyEventArgs e) 
            {
				if (e.KeyCode == Keys.Enter)
				{
						string input = udpWhiteListInputIn.Text;
						udpWhiteListInputIn.Text = "";
						int port = 0;
						if (int.TryParse(input, out port))
						{
								BasicFirewall.Rule found = null;
								foreach (BasicFirewall.Rule r in rules)
								{
										if (r.transport == Protocol.UDP)
										{
												if (r.Set == BasicFirewall.PacketStatus.ALLOWED && (r.Port == port || r.Port == -1) && (r.Direction & 0x1) == 0x1)
												{
														found = r;
														udpWhiteListPortIn.Items.Remove(port);
														break;
												}
										}
								}
								if (found != null)
								{
										rules.Remove(found);
								}
								else
								{
										rules.Insert(0, new BasicFirewall.Rule(BasicFirewall.PacketStatus.ALLOWED, port, Protocol.UDP, false, false, null, 0x1));
										udpWhiteListPortIn.Items.Add(port);
								}
						}
				}
			}

            /// <summary>
            /// Adds or removes port from this rule area
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
			private void udpWhiteListInputOut_KeyUp(object sender, KeyEventArgs e) 
            {
				if (e.KeyCode == Keys.Enter)
				{
						string input = udpWhiteListInputOut.Text;
						udpWhiteListInputOut.Text = "";
						int port = 0;
						if (int.TryParse(input, out port))
						{
								BasicFirewall.Rule found = null;
								foreach (BasicFirewall.Rule r in rules)
								{
										if (r.transport == Protocol.UDP)
										{
												if (r.Set == BasicFirewall.PacketStatus.ALLOWED && (r.Port == port || r.Port == -1) && (r.Direction & 0x2) == 0x2)
												{
														found = r;
														udpWhiteListPortOut.Items.Remove(port);
														break;
												}
										}
								}
								if (found != null)
								{
										rules.Remove(found);
								}
								else
								{
										rules.Insert(0, new BasicFirewall.Rule(BasicFirewall.PacketStatus.ALLOWED, port, Protocol.UDP, false, false, null, 0x2));
										udpWhiteListPortOut.Items.Add(port);
								}
						}
				}
			}
		}
}
