namespace EasyModifier
{
    partial class frmMain
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblHelp = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.lblDescribeSettings = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblHelp
            // 
            this.lblHelp.AutoSize = true;
            this.lblHelp.Location = new System.Drawing.Point(12, 9);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(143, 13);
            this.lblHelp.TabIndex = 1;
            this.lblHelp.Text = "Help/About(click to get help)";
            this.lblHelp.Click += new System.EventHandler(this.lblHelp_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(15, 35);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(568, 289);
            this.txtLog.TabIndex = 2;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(457, 331);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(126, 23);
            this.btnClearLog.TabIndex = 3;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // lblDescribeSettings
            // 
            this.lblDescribeSettings.AutoSize = true;
            this.lblDescribeSettings.Location = new System.Drawing.Point(457, 9);
            this.lblDescribeSettings.Name = "lblDescribeSettings";
            this.lblDescribeSettings.Size = new System.Drawing.Size(124, 13);
            this.lblDescribeSettings.TabIndex = 4;
            this.lblDescribeSettings.Text = "Click to describe settings";
            this.lblDescribeSettings.Click += new System.EventHandler(this.lblDescribeSettings_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 366);
            this.Controls.Add(this.lblDescribeSettings);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblHelp);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Easy Modify";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHelp;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Label lblDescribeSettings;
    }
}

