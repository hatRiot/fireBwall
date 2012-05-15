namespace PoisonIvy
{
    partial class PoisonIvyUI
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
            this.poisonBox = new System.Windows.Forms.ListBox();
            this.poisonPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // poisonBox
            // 
            this.poisonBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.poisonBox.FormattingEnabled = true;
            this.poisonBox.Items.AddRange(new object[] {
            "ARP ",
            "DNS",
            "DHCP",
            "FTP"});
            this.poisonBox.Location = new System.Drawing.Point(3, 3);
            this.poisonBox.Name = "poisonBox";
            this.poisonBox.Size = new System.Drawing.Size(59, 447);
            this.poisonBox.TabIndex = 6;
            this.poisonBox.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // poisonPanel
            // 
            this.poisonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.poisonPanel.Location = new System.Drawing.Point(68, 3);
            this.poisonPanel.Name = "poisonPanel";
            this.poisonPanel.Size = new System.Drawing.Size(490, 447);
            this.poisonPanel.TabIndex = 7;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.71171F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 88.28829F));
            this.tableLayoutPanel1.Controls.Add(this.poisonBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.poisonPanel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(561, 453);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // PoisonIvyUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PoisonIvyUI";
            this.Size = new System.Drawing.Size(561, 453);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox poisonBox;
        private System.Windows.Forms.Panel poisonPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
