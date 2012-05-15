namespace PassThru
{
		partial class RuleEditor: System.Windows.Forms.UserControl {
			private System.Windows.Forms.Button buttonApply;

			/// <summary>
			/// Required designer variable.
			/// </summary>
			private System.ComponentModel.IContainer components = null;
			private System.Windows.Forms.GroupBox groupBox1;
			private System.Windows.Forms.GroupBox groupBox2;
			private System.Windows.Forms.GroupBox groupBox3;
			private System.Windows.Forms.GroupBox groupBox4;
			private System.Windows.Forms.GroupBox groupBox5;
			private System.Windows.Forms.GroupBox groupBox6;
			private System.Windows.Forms.GroupBox groupBox7;
			private System.Windows.Forms.GroupBox groupBox8;
			private System.Windows.Forms.TabControl tabControl1;
			private System.Windows.Forms.TabPage tabPage1;
			private System.Windows.Forms.TabPage tabPage2;
			private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
			private System.Windows.Forms.TableLayoutPanel tableLayoutPanel11;
			private System.Windows.Forms.TableLayoutPanel tableLayoutPanel12;
			private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
			private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
			private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
			private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
			private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
			private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
			private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
			private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
			private System.Windows.Forms.ListBox tcpDoNotNotifyIn;
			private System.Windows.Forms.TextBox tcpDoNotNotifyInputIn;
			private System.Windows.Forms.TextBox tcpDoNotNotifyInputOut;
			private System.Windows.Forms.ListBox tcpDoNotNotifyOut;
			private System.Windows.Forms.ListBox tcpWhiteListIn;
			private System.Windows.Forms.TextBox tcpWhiteListInputIn;
			private System.Windows.Forms.TextBox tcpWhiteListInputOut;
			private System.Windows.Forms.ListBox tcpWhiteListOut;
			private System.Windows.Forms.ListBox udpDoNotNotifyIn;
			private System.Windows.Forms.TextBox udpDoNotNotifyInputIn;
			private System.Windows.Forms.TextBox udpDoNotNotifyInputOut;
			private System.Windows.Forms.ListBox udpDoNotNotifyOut;
			private System.Windows.Forms.TextBox udpWhiteListInputIn;
			private System.Windows.Forms.TextBox udpWhiteListInputOut;
			private System.Windows.Forms.ListBox udpWhiteListPortIn;
			private System.Windows.Forms.ListBox udpWhiteListPortOut;

			/// <summary>
			/// Clean up any resources being used.
			/// </summary>
			/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
			protected override void Dispose(bool disposing) {
				if (disposing && (components != null))
				{
						components.Dispose();
				}
				base.Dispose(disposing);
			}

			/// <summary>
			/// Required method for Designer support - do not modify
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent() {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tcpDoNotNotifyOut = new System.Windows.Forms.ListBox();
            this.tcpDoNotNotifyInputOut = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tcpDoNotNotifyIn = new System.Windows.Forms.ListBox();
            this.tcpDoNotNotifyInputIn = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tcpWhiteListOut = new System.Windows.Forms.ListBox();
            this.tcpWhiteListInputOut = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tcpWhiteListIn = new System.Windows.Forms.ListBox();
            this.tcpWhiteListInputIn = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.udpDoNotNotifyOut = new System.Windows.Forms.ListBox();
            this.udpDoNotNotifyInputOut = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.udpDoNotNotifyIn = new System.Windows.Forms.ListBox();
            this.udpDoNotNotifyInputIn = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.udpWhiteListPortOut = new System.Windows.Forms.ListBox();
            this.udpWhiteListInputOut = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.udpWhiteListPortIn = new System.Windows.Forms.ListBox();
            this.udpWhiteListInputIn = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonApply = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(548, 344);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(540, 318);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "TCP";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel6, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.29586F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.70414F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(534, 312);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.groupBox4, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tcpDoNotNotifyInputOut, 0, 1);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(270, 159);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(261, 150);
            this.tableLayoutPanel6.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tcpDoNotNotifyOut);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(255, 119);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Do Not Notify Ports(Outbound)";
            // 
            // tcpDoNotNotifyOut
            // 
            this.tcpDoNotNotifyOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcpDoNotNotifyOut.FormattingEnabled = true;
            this.tcpDoNotNotifyOut.Location = new System.Drawing.Point(3, 16);
            this.tcpDoNotNotifyOut.Name = "tcpDoNotNotifyOut";
            this.tcpDoNotNotifyOut.Size = new System.Drawing.Size(249, 100);
            this.tcpDoNotNotifyOut.TabIndex = 0;
            // 
            // tcpDoNotNotifyInputOut
            // 
            this.tcpDoNotNotifyInputOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcpDoNotNotifyInputOut.Location = new System.Drawing.Point(3, 128);
            this.tcpDoNotNotifyInputOut.Name = "tcpDoNotNotifyInputOut";
            this.tcpDoNotNotifyInputOut.Size = new System.Drawing.Size(255, 20);
            this.tcpDoNotNotifyInputOut.TabIndex = 1;
            this.tcpDoNotNotifyInputOut.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tcpDoNotNotifyInputOut_KeyUp);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.tcpDoNotNotifyInputIn, 0, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 159);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(261, 150);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tcpDoNotNotifyIn);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(255, 119);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Do Not Notify Ports(Inbound)";
            // 
            // tcpDoNotNotifyIn
            // 
            this.tcpDoNotNotifyIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcpDoNotNotifyIn.FormattingEnabled = true;
            this.tcpDoNotNotifyIn.Location = new System.Drawing.Point(3, 16);
            this.tcpDoNotNotifyIn.Name = "tcpDoNotNotifyIn";
            this.tcpDoNotNotifyIn.Size = new System.Drawing.Size(249, 100);
            this.tcpDoNotNotifyIn.TabIndex = 0;
            // 
            // tcpDoNotNotifyInputIn
            // 
            this.tcpDoNotNotifyInputIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcpDoNotNotifyInputIn.Location = new System.Drawing.Point(3, 128);
            this.tcpDoNotNotifyInputIn.Name = "tcpDoNotNotifyInputIn";
            this.tcpDoNotNotifyInputIn.Size = new System.Drawing.Size(255, 20);
            this.tcpDoNotNotifyInputIn.TabIndex = 1;
            this.tcpDoNotNotifyInputIn.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tcpDoNotNotifyInputIn_KeyUp);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tcpWhiteListInputOut, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(270, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(261, 150);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tcpWhiteListOut);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(255, 119);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Whitelisted Ports(Outbound)";
            // 
            // tcpWhiteListOut
            // 
            this.tcpWhiteListOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcpWhiteListOut.FormattingEnabled = true;
            this.tcpWhiteListOut.Location = new System.Drawing.Point(3, 16);
            this.tcpWhiteListOut.Name = "tcpWhiteListOut";
            this.tcpWhiteListOut.Size = new System.Drawing.Size(249, 100);
            this.tcpWhiteListOut.TabIndex = 0;
            // 
            // tcpWhiteListInputOut
            // 
            this.tcpWhiteListInputOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcpWhiteListInputOut.Location = new System.Drawing.Point(3, 128);
            this.tcpWhiteListInputOut.Name = "tcpWhiteListInputOut";
            this.tcpWhiteListInputOut.Size = new System.Drawing.Size(255, 20);
            this.tcpWhiteListInputOut.TabIndex = 1;
            this.tcpWhiteListInputOut.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tcpWhiteListInputOut_KeyUp);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tcpWhiteListInputIn, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(261, 150);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tcpWhiteListIn);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(255, 119);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Whitelisted Ports(Inbound)";
            // 
            // tcpWhiteListIn
            // 
            this.tcpWhiteListIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcpWhiteListIn.FormattingEnabled = true;
            this.tcpWhiteListIn.Location = new System.Drawing.Point(3, 16);
            this.tcpWhiteListIn.Name = "tcpWhiteListIn";
            this.tcpWhiteListIn.Size = new System.Drawing.Size(249, 100);
            this.tcpWhiteListIn.TabIndex = 0;
            // 
            // tcpWhiteListInputIn
            // 
            this.tcpWhiteListInputIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcpWhiteListInputIn.Location = new System.Drawing.Point(3, 128);
            this.tcpWhiteListInputIn.Name = "tcpWhiteListInputIn";
            this.tcpWhiteListInputIn.Size = new System.Drawing.Size(255, 20);
            this.tcpWhiteListInputIn.TabIndex = 1;
            this.tcpWhiteListInputIn.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tcpWhiteListInputIn_KeyUp);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel7);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(540, 318);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "UDP";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel8, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel9, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel10, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel11, 0, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 2;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.29586F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.70414F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(534, 312);
            this.tableLayoutPanel7.TabIndex = 2;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 1;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Controls.Add(this.groupBox5, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.udpDoNotNotifyInputOut, 0, 1);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(270, 159);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(261, 150);
            this.tableLayoutPanel8.TabIndex = 3;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.udpDoNotNotifyOut);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(255, 119);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Do Not Notify Ports(Outbound)";
            // 
            // udpDoNotNotifyOut
            // 
            this.udpDoNotNotifyOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udpDoNotNotifyOut.FormattingEnabled = true;
            this.udpDoNotNotifyOut.Location = new System.Drawing.Point(3, 16);
            this.udpDoNotNotifyOut.Name = "udpDoNotNotifyOut";
            this.udpDoNotNotifyOut.Size = new System.Drawing.Size(249, 100);
            this.udpDoNotNotifyOut.TabIndex = 0;
            // 
            // udpDoNotNotifyInputOut
            // 
            this.udpDoNotNotifyInputOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udpDoNotNotifyInputOut.Location = new System.Drawing.Point(3, 128);
            this.udpDoNotNotifyInputOut.Name = "udpDoNotNotifyInputOut";
            this.udpDoNotNotifyInputOut.Size = new System.Drawing.Size(255, 20);
            this.udpDoNotNotifyInputOut.TabIndex = 1;
            this.udpDoNotNotifyInputOut.KeyUp += new System.Windows.Forms.KeyEventHandler(this.udpDoNotNotifyInputOut_KeyUp);
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 1;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Controls.Add(this.groupBox6, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.udpDoNotNotifyInputIn, 0, 1);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(3, 159);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 2;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(261, 150);
            this.tableLayoutPanel9.TabIndex = 2;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.udpDoNotNotifyIn);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Location = new System.Drawing.Point(3, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(255, 119);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Do Not Notify Ports(Inbound)";
            // 
            // udpDoNotNotifyIn
            // 
            this.udpDoNotNotifyIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udpDoNotNotifyIn.FormattingEnabled = true;
            this.udpDoNotNotifyIn.Location = new System.Drawing.Point(3, 16);
            this.udpDoNotNotifyIn.Name = "udpDoNotNotifyIn";
            this.udpDoNotNotifyIn.Size = new System.Drawing.Size(249, 100);
            this.udpDoNotNotifyIn.TabIndex = 0;
            // 
            // udpDoNotNotifyInputIn
            // 
            this.udpDoNotNotifyInputIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udpDoNotNotifyInputIn.Location = new System.Drawing.Point(3, 128);
            this.udpDoNotNotifyInputIn.Name = "udpDoNotNotifyInputIn";
            this.udpDoNotNotifyInputIn.Size = new System.Drawing.Size(255, 20);
            this.udpDoNotNotifyInputIn.TabIndex = 1;
            this.udpDoNotNotifyInputIn.KeyUp += new System.Windows.Forms.KeyEventHandler(this.udpDoNotNotifyInputIn_KeyUp);
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 1;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.Controls.Add(this.groupBox7, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.udpWhiteListInputOut, 0, 1);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(270, 3);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 2;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(261, 150);
            this.tableLayoutPanel10.TabIndex = 1;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.udpWhiteListPortOut);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(3, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(255, 119);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Whitelisted Ports(Outbound)";
            // 
            // udpWhiteListPortOut
            // 
            this.udpWhiteListPortOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udpWhiteListPortOut.FormattingEnabled = true;
            this.udpWhiteListPortOut.Location = new System.Drawing.Point(3, 16);
            this.udpWhiteListPortOut.Name = "udpWhiteListPortOut";
            this.udpWhiteListPortOut.Size = new System.Drawing.Size(249, 100);
            this.udpWhiteListPortOut.TabIndex = 0;
            // 
            // udpWhiteListInputOut
            // 
            this.udpWhiteListInputOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udpWhiteListInputOut.Location = new System.Drawing.Point(3, 128);
            this.udpWhiteListInputOut.Name = "udpWhiteListInputOut";
            this.udpWhiteListInputOut.Size = new System.Drawing.Size(255, 20);
            this.udpWhiteListInputOut.TabIndex = 1;
            this.udpWhiteListInputOut.KeyUp += new System.Windows.Forms.KeyEventHandler(this.udpWhiteListInputOut_KeyUp);
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.ColumnCount = 1;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.Controls.Add(this.groupBox8, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.udpWhiteListInputIn, 0, 1);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 2;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(261, 150);
            this.tableLayoutPanel11.TabIndex = 0;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.udpWhiteListPortIn);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Location = new System.Drawing.Point(3, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(255, 119);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Whitelisted Ports(Inbound)";
            // 
            // udpWhiteListPortIn
            // 
            this.udpWhiteListPortIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udpWhiteListPortIn.FormattingEnabled = true;
            this.udpWhiteListPortIn.Location = new System.Drawing.Point(3, 16);
            this.udpWhiteListPortIn.Name = "udpWhiteListPortIn";
            this.udpWhiteListPortIn.Size = new System.Drawing.Size(249, 100);
            this.udpWhiteListPortIn.TabIndex = 0;
            // 
            // udpWhiteListInputIn
            // 
            this.udpWhiteListInputIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udpWhiteListInputIn.Location = new System.Drawing.Point(3, 128);
            this.udpWhiteListInputIn.Name = "udpWhiteListInputIn";
            this.udpWhiteListInputIn.Size = new System.Drawing.Size(255, 20);
            this.udpWhiteListInputIn.TabIndex = 1;
            this.udpWhiteListInputIn.KeyUp += new System.Windows.Forms.KeyEventHandler(this.udpWhiteListInputIn_KeyUp);
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 1;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.buttonApply, 0, 1);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 2;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(554, 381);
            this.tableLayoutPanel12.TabIndex = 1;
            // 
            // buttonApply
            // 
            this.buttonApply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonApply.Location = new System.Drawing.Point(3, 353);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(548, 25);
            this.buttonApply.TabIndex = 1;
            this.buttonApply.Text = "Apply Rules";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // RuleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel12);
            this.Name = "RuleEditor";
            this.Size = new System.Drawing.Size(554, 381);
            this.Load += new System.EventHandler(this.RuleEditor_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.tableLayoutPanel12.ResumeLayout(false);
            this.ResumeLayout(false);

			}
		}
}
