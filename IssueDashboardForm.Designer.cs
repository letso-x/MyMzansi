namespace MyMzansi
{
    partial class IssueDashboardForm
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
            this.listBoxIssues = new System.Windows.Forms.ListBox();
            this.btnViewDetails = new System.Windows.Forms.Button();
            this.btnCreateNewIssue = new System.Windows.Forms.Button();
            this.btnBackToMain = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxIssues
            // 
            this.listBoxIssues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxIssues.FormattingEnabled = true;
            this.listBoxIssues.Location = new System.Drawing.Point(24, 24);
            this.listBoxIssues.Name = "listBoxIssues";
            this.listBoxIssues.Size = new System.Drawing.Size(1067, 355);
            this.listBoxIssues.TabIndex = 0;
            // 
            // btnViewDetails
            // 
            this.btnViewDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewDetails.Location = new System.Drawing.Point(24, 410);
            this.btnViewDetails.Name = "btnViewDetails";
            this.btnViewDetails.Size = new System.Drawing.Size(120, 32);
            this.btnViewDetails.TabIndex = 1;
            this.btnViewDetails.Text = "View Details";
            this.btnViewDetails.UseVisualStyleBackColor = true;
            this.btnViewDetails.Click += new System.EventHandler(this.btnViewDetails_Click);
            // 
            // btnCreateNewIssue
            // 
            this.btnCreateNewIssue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCreateNewIssue.Location = new System.Drawing.Point(166, 410);
            this.btnCreateNewIssue.Name = "btnCreateNewIssue";
            this.btnCreateNewIssue.Size = new System.Drawing.Size(150, 32);
            this.btnCreateNewIssue.TabIndex = 2;
            this.btnCreateNewIssue.Text = "Report New Issue";
            this.btnCreateNewIssue.UseVisualStyleBackColor = true;
            this.btnCreateNewIssue.Click += new System.EventHandler(this.btnCreateNewIssue_Click);
            // 
            // btnBackToMain
            // 
            this.btnBackToMain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackToMain.Location = new System.Drawing.Point(941, 410);
            this.btnBackToMain.Name = "btnBackToMain";
            this.btnBackToMain.Size = new System.Drawing.Size(150, 32);
            this.btnBackToMain.TabIndex = 3;
            this.btnBackToMain.Text = "Back to Main Menu";
            this.btnBackToMain.UseVisualStyleBackColor = true;
            this.btnBackToMain.Click += new System.EventHandler(this.btnBackToMain_Click);
            // 
            // IssueDashboardForm
            // 
            this.ClientSize = new System.Drawing.Size(1115, 496);
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Controls.Add(this.btnBackToMain);
            this.Controls.Add(this.btnCreateNewIssue);
            this.Controls.Add(this.btnViewDetails);
            this.Controls.Add(this.listBoxIssues);
            this.Name = "IssueDashboardForm";
            this.Text = "Reported Issues";
            this.Load += new System.EventHandler(this.IssueDashboardForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxIssues;
        private System.Windows.Forms.Button btnViewDetails;
        private System.Windows.Forms.Button btnCreateNewIssue;
        private System.Windows.Forms.Button btnBackToMain;
    }
}