namespace MyMzansi
{
    partial class ReportIssuesPage
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
            this.tellUsMoreLabel = new System.Windows.Forms.Label();
            this.locationTextBox = new System.Windows.Forms.TextBox();
            this.categoryDropDown = new System.Windows.Forms.ComboBox();
            this.locationLabel = new System.Windows.Forms.Label();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.DescriptionTextBox = new System.Windows.Forms.RichTextBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.imageUploadbtn = new System.Windows.Forms.Button();
            this.uploadedImagePreview = new System.Windows.Forms.PictureBox();
            this.btnBackToList = new System.Windows.Forms.Button();
            this.submitBtn = new System.Windows.Forms.Button();
            this.subjectLabel = new System.Windows.Forms.Label();
            this.subjectTxtBox = new System.Windows.Forms.TextBox();
            this.attachmentPathLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.uploadedImagePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // tellUsMoreLabel
            // 
            this.tellUsMoreLabel.AutoSize = true;
            this.tellUsMoreLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.tellUsMoreLabel.Location = new System.Drawing.Point(19, 19);
            this.tellUsMoreLabel.Name = "tellUsMoreLabel";
            this.tellUsMoreLabel.Size = new System.Drawing.Size(533, 24);
            this.tellUsMoreLabel.TabIndex = 0;
            this.tellUsMoreLabel.Text = "Please tell us more about your issue by filling in the form below";
            // 
            // locationTextBox
            // 
            this.locationTextBox.Location = new System.Drawing.Point(23, 363);
            this.locationTextBox.Name = "locationTextBox";
            this.locationTextBox.Size = new System.Drawing.Size(266, 20);
            this.locationTextBox.TabIndex = 1;
            this.locationTextBox.TextChanged += new System.EventHandler(this.InputChanged);
            // 
            // categoryDropDown
            // 
            this.categoryDropDown.FormattingEnabled = true;
            this.categoryDropDown.Location = new System.Drawing.Point(26, 147);
            this.categoryDropDown.Name = "categoryDropDown";
            this.categoryDropDown.Size = new System.Drawing.Size(266, 21);
            this.categoryDropDown.TabIndex = 2;
            this.categoryDropDown.SelectedIndexChanged += new System.EventHandler(this.InputChanged);
            // 
            // locationLabel
            // 
            this.locationLabel.AutoSize = true;
            this.locationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.locationLabel.Location = new System.Drawing.Point(23, 330);
            this.locationLabel.Name = "locationLabel";
            this.locationLabel.Size = new System.Drawing.Size(65, 18);
            this.locationLabel.TabIndex = 3;
            this.locationLabel.Text = "Location";
            // 
            // categoryLabel
            // 
            this.categoryLabel.AutoSize = true;
            this.categoryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.categoryLabel.Location = new System.Drawing.Point(20, 120);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(68, 18);
            this.categoryLabel.TabIndex = 4;
            this.categoryLabel.Text = "Category";
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Location = new System.Drawing.Point(26, 212);
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.Size = new System.Drawing.Size(266, 96);
            this.DescriptionTextBox.TabIndex = 5;
            this.DescriptionTextBox.Text = "";
            this.DescriptionTextBox.TextChanged += new System.EventHandler(this.InputChanged);
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.descriptionLabel.Location = new System.Drawing.Point(23, 182);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(83, 18);
            this.descriptionLabel.TabIndex = 6;
            this.descriptionLabel.Text = "Description";
            // 
            // imageUploadbtn
            // 
            this.imageUploadbtn.Location = new System.Drawing.Point(23, 421);
            this.imageUploadbtn.Name = "imageUploadbtn";
            this.imageUploadbtn.Size = new System.Drawing.Size(103, 25);
            this.imageUploadbtn.TabIndex = 7;
            this.imageUploadbtn.Text = "Attach Image / Document";
            this.imageUploadbtn.UseVisualStyleBackColor = true;
            this.imageUploadbtn.Click += new System.EventHandler(this.imageUploadbtn_Click);
            // 
            // uploadedImagePreview
            // 
            this.uploadedImagePreview.Location = new System.Drawing.Point(23, 469);
            this.uploadedImagePreview.Name = "uploadedImagePreview";
            this.uploadedImagePreview.Size = new System.Drawing.Size(170, 115);
            this.uploadedImagePreview.TabIndex = 8;
            this.uploadedImagePreview.TabStop = false;
            // 
            // btnBackToList
            // 
            this.btnBackToList.Location = new System.Drawing.Point(23, 630);
            this.btnBackToList.Name = "btnBackToList";
            this.btnBackToList.Size = new System.Drawing.Size(120, 23);
            this.btnBackToList.TabIndex = 9;
            this.btnBackToList.Text = "Back to Issues List";
            this.btnBackToList.UseVisualStyleBackColor = true;
            this.btnBackToList.Click += new System.EventHandler(this.btnBackToList_Click);
            // 
            // submitBtn
            // 
            this.submitBtn.Location = new System.Drawing.Point(702, 630);
            this.submitBtn.Name = "submitBtn";
            this.submitBtn.Size = new System.Drawing.Size(75, 23);
            this.submitBtn.TabIndex = 10;
            this.submitBtn.Text = "Submit";
            this.submitBtn.UseVisualStyleBackColor = true;
            this.submitBtn.Click += new System.EventHandler(this.submitBtn_Click);
            // 
            // subjectLabel
            // 
            this.subjectLabel.AutoSize = true;
            this.subjectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.subjectLabel.Location = new System.Drawing.Point(23, 61);
            this.subjectLabel.Name = "subjectLabel";
            this.subjectLabel.Size = new System.Drawing.Size(57, 18);
            this.subjectLabel.TabIndex = 11;
            this.subjectLabel.Text = "Subject";
            // 
            // subjectTxtBox
            // 
            this.subjectTxtBox.Location = new System.Drawing.Point(26, 97);
            this.subjectTxtBox.Name = "subjectTxtBox";
            this.subjectTxtBox.Size = new System.Drawing.Size(535, 20);
            this.subjectTxtBox.TabIndex = 12;
            // 
            // attachmentPathLabel
            // 
            this.attachmentPathLabel.AutoEllipsis = true;
            this.attachmentPathLabel.AutoSize = false;
            this.attachmentPathLabel.Location = new System.Drawing.Point(140, 425);
            this.attachmentPathLabel.Name = "attachmentPathLabel";
            this.attachmentPathLabel.Size = new System.Drawing.Size(420, 20);
            this.attachmentPathLabel.TabIndex = 13;
            this.attachmentPathLabel.Text = "No attachment selected";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = false;
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(8, 126, 139);
            this.statusLabel.Location = new System.Drawing.Point(315, 560);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(450, 30);
            this.statusLabel.TabIndex = 14;
            this.statusLabel.Text = "Start by entering the issue location.";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(315, 600);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(450, 23);
            this.progressBar.TabIndex = 15;
            // 
            // ReportIssuesPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.ClientSize = new System.Drawing.Size(800, 665);
            this.MinimumSize = new System.Drawing.Size(800, 665);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.attachmentPathLabel);
            this.Controls.Add(this.subjectTxtBox);
            this.Controls.Add(this.subjectLabel);
            this.Controls.Add(this.submitBtn);
            this.Controls.Add(this.btnBackToList);
            this.Controls.Add(this.uploadedImagePreview);
            this.Controls.Add(this.imageUploadbtn);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.DescriptionTextBox);
            this.Controls.Add(this.categoryLabel);
            this.Controls.Add(this.locationLabel);
            this.Controls.Add(this.categoryDropDown);
            this.Controls.Add(this.locationTextBox);
            this.Controls.Add(this.tellUsMoreLabel);
            this.Name = "ReportIssuesPage";
            this.Text = "ReportIssuesPage";
            ((System.ComponentModel.ISupportInitialize)(this.uploadedImagePreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label tellUsMoreLabel;
        private System.Windows.Forms.TextBox locationTextBox;
        private System.Windows.Forms.ComboBox categoryDropDown;
        private System.Windows.Forms.Label locationLabel;
        private System.Windows.Forms.Label categoryLabel;
        private System.Windows.Forms.RichTextBox DescriptionTextBox;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Button imageUploadbtn;
        private System.Windows.Forms.PictureBox uploadedImagePreview;
        private System.Windows.Forms.Button btnBackToList;
        private System.Windows.Forms.Button submitBtn;
        private System.Windows.Forms.Label subjectLabel;
        private System.Windows.Forms.TextBox subjectTxtBox;
        private System.Windows.Forms.Label attachmentPathLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}