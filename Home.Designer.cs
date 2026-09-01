namespace MyMzansi
{
    partial class Home
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
            this.welcomeLabel = new System.Windows.Forms.Label();
            this.btnReportIssues = new System.Windows.Forms.Button();
            this.btnLocalEvents = new System.Windows.Forms.Button();
            this.btnServiceRequestStatus = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // welcomeLabel
            // 
            this.welcomeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.welcomeLabel.AutoSize = true;
            this.welcomeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.welcomeLabel.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
            this.welcomeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.welcomeLabel.Location = new System.Drawing.Point(258, 102);
            this.welcomeLabel.Name = "welcomeLabel";
            this.welcomeLabel.Size = new System.Drawing.Size(277, 26);
            this.welcomeLabel.TabIndex = 0;
            this.welcomeLabel.Text = "Welcome to MyMzansi App";
            this.welcomeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnReportIssues
            // 
            this.btnReportIssues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReportIssues.BackColor = System.Drawing.Color.FromArgb(18, 53, 91);
            this.btnReportIssues.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReportIssues.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnReportIssues.ForeColor = System.Drawing.Color.White;
            this.btnReportIssues.Location = new System.Drawing.Point(263, 154);
            this.btnReportIssues.Name = "btnReportIssues";
            this.btnReportIssues.Size = new System.Drawing.Size(260, 61);
            this.btnReportIssues.TabIndex = 1;
            this.btnReportIssues.Text = "Report Issues";
            this.btnReportIssues.UseVisualStyleBackColor = false;
            this.btnReportIssues.Click += new System.EventHandler(this.reportIssuesBtn_Click);
            // 
            // btnLocalEvents
            // 
            this.btnLocalEvents.Enabled = false;
            this.btnLocalEvents.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnLocalEvents.Location = new System.Drawing.Point(263, 230);
            this.btnLocalEvents.Name = "btnLocalEvents";
            this.btnLocalEvents.Size = new System.Drawing.Size(260, 61);
            this.btnLocalEvents.TabIndex = 2;
            this.btnLocalEvents.Text = "Local Events and Announcements (Coming Soon)";
            this.btnLocalEvents.UseVisualStyleBackColor = true;
            // 
            // btnServiceRequestStatus
            // 
            this.btnServiceRequestStatus.Enabled = false;
            this.btnServiceRequestStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnServiceRequestStatus.Location = new System.Drawing.Point(263, 312);
            this.btnServiceRequestStatus.Name = "btnServiceRequestStatus";
            this.btnServiceRequestStatus.Size = new System.Drawing.Size(260, 61);
            this.btnServiceRequestStatus.TabIndex = 3;
            this.btnServiceRequestStatus.Text = "Service Request Status (Coming Soon)";
            this.btnServiceRequestStatus.UseVisualStyleBackColor = true;
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.ClientSize = new System.Drawing.Size(818, 454);
            this.MinimumSize = new System.Drawing.Size(650, 420);
            this.Controls.Add(this.btnServiceRequestStatus);
            this.Controls.Add(this.btnLocalEvents);
            this.Controls.Add(this.btnReportIssues);
            this.Controls.Add(this.welcomeLabel);
            this.Name = "Home";
            this.Text = "MyMzansi";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label welcomeLabel;
        private System.Windows.Forms.Button btnReportIssues;
        private System.Windows.Forms.Button btnLocalEvents;
        private System.Windows.Forms.Button btnServiceRequestStatus;
    }
}