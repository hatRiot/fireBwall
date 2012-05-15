namespace PoisonIvy
{
    partial class ARPUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.poisonIP = new System.Windows.Forms.TextBox();
            this.poisonButton = new System.Windows.Forms.Button();
            this.toIP = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.IP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MAC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stopSpoofButton = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.localGateway = new System.Windows.Forms.TextBox();
            this.localSubnet = new System.Windows.Forms.TextBox();
            this.localMAC = new System.Windows.Forms.TextBox();
            this.localIP = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.spoofUpdateBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // poisonIP
            // 
            this.poisonIP.AcceptsReturn = true;
            this.poisonIP.Location = new System.Drawing.Point(160, 19);
            this.poisonIP.Name = "poisonIP";
            this.poisonIP.Size = new System.Drawing.Size(197, 20);
            this.poisonIP.TabIndex = 10;
            // 
            // poisonButton
            // 
            this.poisonButton.Location = new System.Drawing.Point(466, 19);
            this.poisonButton.Name = "poisonButton";
            this.poisonButton.Size = new System.Drawing.Size(75, 23);
            this.poisonButton.TabIndex = 13;
            this.poisonButton.Text = "Spoof";
            this.poisonButton.UseVisualStyleBackColor = true;
            this.poisonButton.Click += new System.EventHandler(this.poisonButton_Click_1);
            // 
            // toIP
            // 
            this.toIP.Location = new System.Drawing.Point(160, 45);
            this.toIP.Name = "toIP";
            this.toIP.Size = new System.Drawing.Size(197, 20);
            this.toIP.TabIndex = 12;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IP,
            this.MAC});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 325);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(555, 122);
            this.dataGridView1.TabIndex = 14;
            // 
            // IP
            // 
            this.IP.HeaderText = "IP Address";
            this.IP.Name = "IP";
            // 
            // MAC
            // 
            this.MAC.HeaderText = "MAC Address";
            this.MAC.Name = "MAC";
            // 
            // stopSpoofButton
            // 
            this.stopSpoofButton.Location = new System.Drawing.Point(466, 49);
            this.stopSpoofButton.Name = "stopSpoofButton";
            this.stopSpoofButton.Size = new System.Drawing.Size(75, 23);
            this.stopSpoofButton.TabIndex = 15;
            this.stopSpoofButton.Text = "Stop Spoof";
            this.stopSpoofButton.UseVisualStyleBackColor = true;
            this.stopSpoofButton.Click += new System.EventHandler(this.stopSpoofButton_Click);
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Location = new System.Drawing.Point(3, 300);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(35, 13);
            this.status.TabIndex = 16;
            this.status.Text = "status";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71.74888F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.25112F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(561, 450);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.spoofUpdateBox);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.status);
            this.panel1.Controls.Add(this.stopSpoofButton);
            this.panel1.Controls.Add(this.poisonButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(555, 316);
            this.panel1.TabIndex = 15;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.localGateway);
            this.groupBox2.Controls.Add(this.localSubnet);
            this.groupBox2.Controls.Add(this.localMAC);
            this.groupBox2.Controls.Add(this.localIP);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(48, 116);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(393, 118);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Local";
            // 
            // localGateway
            // 
            this.localGateway.Location = new System.Drawing.Point(160, 90);
            this.localGateway.Name = "localGateway";
            this.localGateway.ReadOnly = true;
            this.localGateway.Size = new System.Drawing.Size(197, 20);
            this.localGateway.TabIndex = 7;
            // 
            // localSubnet
            // 
            this.localSubnet.Location = new System.Drawing.Point(160, 63);
            this.localSubnet.Name = "localSubnet";
            this.localSubnet.ReadOnly = true;
            this.localSubnet.Size = new System.Drawing.Size(197, 20);
            this.localSubnet.TabIndex = 6;
            // 
            // localMAC
            // 
            this.localMAC.Location = new System.Drawing.Point(160, 41);
            this.localMAC.Name = "localMAC";
            this.localMAC.ReadOnly = true;
            this.localMAC.Size = new System.Drawing.Size(197, 20);
            this.localMAC.TabIndex = 5;
            // 
            // localIP
            // 
            this.localIP.Location = new System.Drawing.Point(160, 13);
            this.localIP.Name = "localIP";
            this.localIP.ReadOnly = true;
            this.localIP.Size = new System.Drawing.Size(197, 20);
            this.localIP.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "MAC Address:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Gateway:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Subnet Mask:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "IP Address:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(500, 300);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "seconds";
            // 
            // spoofUpdateBox
            // 
            this.spoofUpdateBox.Location = new System.Drawing.Point(394, 297);
            this.spoofUpdateBox.Name = "spoofUpdateBox";
            this.spoofUpdateBox.Size = new System.Drawing.Size(100, 20);
            this.spoofUpdateBox.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.poisonIP);
            this.groupBox1.Controls.Add(this.toIP);
            this.groupBox1.Location = new System.Drawing.Point(48, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(393, 78);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ARP Spoof";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "To Address:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "From Address:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(312, 300);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Spoof Update:";
            // 
            // ARPUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ARPUI";
            this.Size = new System.Drawing.Size(561, 450);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox poisonIP;
        private System.Windows.Forms.Button poisonButton;
        private System.Windows.Forms.TextBox toIP;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn IP;
        private System.Windows.Forms.DataGridViewTextBoxColumn MAC;
        private System.Windows.Forms.Button stopSpoofButton;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox spoofUpdateBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox localGateway;
        private System.Windows.Forms.TextBox localSubnet;
        private System.Windows.Forms.TextBox localMAC;
        private System.Windows.Forms.TextBox localIP;


    }
}
