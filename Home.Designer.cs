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
            this.reportIssuesBtn = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // welcomeLabel
            // 
            this.welcomeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.welcomeLabel.AutoSize = true;
            this.welcomeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.welcomeLabel.Location = new System.Drawing.Point(258, 102);
            this.welcomeLabel.Name = "welcomeLabel";
            this.welcomeLabel.Size = new System.Drawing.Size(277, 26);
            this.welcomeLabel.TabIndex = 0;
            this.welcomeLabel.Text = "Welcome to MyMzansi App";
            this.welcomeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // reportIssuesBtn
            // 
            this.reportIssuesBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.reportIssuesBtn.Location = new System.Drawing.Point(263, 154);
            this.reportIssuesBtn.Name = "reportIssuesBtn";
            this.reportIssuesBtn.Size = new System.Drawing.Size(260, 61);
            this.reportIssuesBtn.TabIndex = 1;
            this.reportIssuesBtn.Text = "Report Issues";
            this.reportIssuesBtn.UseVisualStyleBackColor = true;
            this.reportIssuesBtn.Click += new System.EventHandler(this.reportIssuesBtn_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.button2.Location = new System.Drawing.Point(263, 230);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(260, 61);
            this.button2.TabIndex = 2;
            this.button2.Text = "Local Events and Announcements";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.button3.Location = new System.Drawing.Point(263, 312);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(260, 61);
            this.button3.TabIndex = 3;
            this.button3.Text = "Service Request Status";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(818, 454);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.reportIssuesBtn);
            this.Controls.Add(this.welcomeLabel);
            this.Name = "Home";
            this.Text = "MyMzansi";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label welcomeLabel;
        private System.Windows.Forms.Button reportIssuesBtn;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}