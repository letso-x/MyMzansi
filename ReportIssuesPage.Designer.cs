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
            this.cancelBtn = new System.Windows.Forms.Button();
            this.submitBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.uploadedImagePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // tellUsMoreLabel
            // 
            this.tellUsMoreLabel.AutoSize = true;
            this.tellUsMoreLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.tellUsMoreLabel.Location = new System.Drawing.Point(22, 28);
            this.tellUsMoreLabel.Name = "tellUsMoreLabel";
            this.tellUsMoreLabel.Size = new System.Drawing.Size(533, 24);
            this.tellUsMoreLabel.TabIndex = 0;
            this.tellUsMoreLabel.Text = "Please tell us more about your issue by filling in the form below";
            // 
            // locationTextBox
            // 
            this.locationTextBox.Location = new System.Drawing.Point(35, 89);
            this.locationTextBox.Name = "locationTextBox";
            this.locationTextBox.Size = new System.Drawing.Size(266, 20);
            this.locationTextBox.TabIndex = 1;
            // 
            // categoryDropDown
            // 
            this.categoryDropDown.FormattingEnabled = true;
            this.categoryDropDown.Location = new System.Drawing.Point(35, 146);
            this.categoryDropDown.Name = "categoryDropDown";
            this.categoryDropDown.Size = new System.Drawing.Size(266, 21);
            this.categoryDropDown.TabIndex = 2;
            // 
            // locationLabel
            // 
            this.locationLabel.AutoSize = true;
            this.locationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.locationLabel.Location = new System.Drawing.Point(35, 68);
            this.locationLabel.Name = "locationLabel";
            this.locationLabel.Size = new System.Drawing.Size(65, 18);
            this.locationLabel.TabIndex = 3;
            this.locationLabel.Text = "Location";
            // 
            // categoryLabel
            // 
            this.categoryLabel.AutoSize = true;
            this.categoryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.categoryLabel.Location = new System.Drawing.Point(32, 125);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(68, 18);
            this.categoryLabel.TabIndex = 4;
            this.categoryLabel.Text = "Category";
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Location = new System.Drawing.Point(35, 230);
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.Size = new System.Drawing.Size(266, 96);
            this.DescriptionTextBox.TabIndex = 5;
            this.DescriptionTextBox.Text = "";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.descriptionLabel.Location = new System.Drawing.Point(35, 199);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(83, 18);
            this.descriptionLabel.TabIndex = 6;
            this.descriptionLabel.Text = "Description";
            // 
            // imageUploadbtn
            // 
            this.imageUploadbtn.Location = new System.Drawing.Point(35, 357);
            this.imageUploadbtn.Name = "imageUploadbtn";
            this.imageUploadbtn.Size = new System.Drawing.Size(103, 25);
            this.imageUploadbtn.TabIndex = 7;
            this.imageUploadbtn.Text = "Upload Image";
            this.imageUploadbtn.UseVisualStyleBackColor = true;
            this.imageUploadbtn.Click += new System.EventHandler(this.imageUploadbtn_Click);
            // 
            // uploadedImagePreview
            // 
            this.uploadedImagePreview.Location = new System.Drawing.Point(35, 424);
            this.uploadedImagePreview.Name = "uploadedImagePreview";
            this.uploadedImagePreview.Size = new System.Drawing.Size(170, 115);
            this.uploadedImagePreview.TabIndex = 8;
            this.uploadedImagePreview.TabStop = false;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(35, 630);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(120, 23);
            this.cancelBtn.TabIndex = 9;
            this.cancelBtn.Text = "Back to Main Menu";
            this.cancelBtn.UseVisualStyleBackColor = true;
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
            // ReportIssuesPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 665);
            this.Controls.Add(this.submitBtn);
            this.Controls.Add(this.cancelBtn);
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
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button submitBtn;
    }
}