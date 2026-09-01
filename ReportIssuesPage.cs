using MyMzansi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMzansi
{
    public partial class ReportIssuesPage : Form
    {
        private readonly List<Issue> issueList;
        private string attachmentPath;
        public Issue createdIssue { get; private set; }
        public ReportIssuesPage()
            : this(new List<Issue>())
        {
        }

        public ReportIssuesPage(List<Issue> issues)
        {
            InitializeComponent();
            issueList = issues ?? new List<Issue>();
            List<Category> categories = new List<Category>
            {
                new Category(0, " -- Select a Category -- "),
                new Category(1, "Potholes & Resurfacing"),
                new Category(2, "Traffic Signals"),
                new Category(3, "Water Leaks & Pipe Bursts"),
                new Category(4, "Sewer Overflows"),
                new Category(5, "Illegal Dumping"),
                new Category(6, "Missed Collection"),
                new Category(7, "Overgrown Vegetation"),
                new Category(8, "Streetlight Outages"),
                new Category(9, "Cable Theft & Vandalism"),
                new Category(10,"Damaged Facilities"),
            };

            categoryDropDown.DisplayMember = "Name";
            categoryDropDown.ValueMember = "Id";

            categoryDropDown.DataSource = categories;

            uploadedImagePreview.SizeMode = PictureBoxSizeMode.Zoom;
            UpdateEngagement();

        }
        private void ReportIssuesPage_Load(object sender, EventArgs e)
        {
            

        }

        private void imageUploadbtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Images and Documents|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.pdf;*.doc;*.docx|Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Documents|*.pdf;*.doc;*.docx|All Files|*.*";
                openFileDialog.Title = "Attach evidence for this issue";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        attachmentPath = openFileDialog.FileName;
                        attachmentPathLabel.Text = Path.GetFileName(attachmentPath);
                        uploadedImagePreview.Image = null;
                        string extension = Path.GetExtension(attachmentPath).ToLowerInvariant();
                        if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp")
                        {
                            using (Image sourceImage = Image.FromFile(attachmentPath))
                            {
                                uploadedImagePreview.Image = new Bitmap(sourceImage);
                            }
                        }
                        statusLabel.Text = "Supporting evidence attached.";
                        UpdateEngagement();
                        MessageBox.Show("Attachment selected successfully.", "Attachment");
                    }
                    catch(Exception)
                    {
                        attachmentPath = null;
                        attachmentPathLabel.Text = "No attachment selected";
                        MessageBox.Show("The selected attachment could not be loaded.", "Attachment Error");
                    }
                }

            }
        }

        private void submitBtn_Click(object sender, EventArgs e)
        {
                if(string.IsNullOrWhiteSpace(subjectTxtBox.Text))
                {
                    MessageBox.Show("Please enter a brief subject line.", "Validation Error");
                    return;
                }
                if(string.IsNullOrWhiteSpace(locationTextBox.Text))
                {
                    MessageBox.Show("Please enter a location.", "Validation Error");
                    return;
                }

                Category selectedCategory = categoryDropDown.SelectedItem as Category;
                if (selectedCategory == null || selectedCategory.Id == 0)
                {
                    MessageBox.Show("Please select an issue category.", "Validation Error");
                    return;
                }
                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                {
                    MessageBox.Show("Please provide a description of the issue.", "Validation Error");
                    return;
                }

                    Image uploadedImage = uploadedImagePreview.Image;
                    createdIssue = new Issue(subjectTxtBox.Text, locationTextBox.Text, selectedCategory, DescriptionTextBox.Text, uploadedImage);
                    createdIssue.AttachmentPath = attachmentPath;
                    issueList.Add(createdIssue);
                    DialogResult = DialogResult.OK;

                    MessageBox.Show("Your issue has been submitted successfully.", "Success");

                    ClearForm();

    
            }

        private void ClearForm()
        {
            locationTextBox.Clear();
            categoryDropDown.SelectedIndex = 0 ;
            DescriptionTextBox.Clear();
            uploadedImagePreview.Image = null;
            attachmentPath = null;
            attachmentPathLabel.Text = "No attachment selected";
            statusLabel.Text = "Start by entering the issue location.";
            UpdateEngagement();
        }

        private void btnBackToList_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void InputChanged(object sender, EventArgs e)
        {
            UpdateEngagement();
        }

        private void UpdateEngagement()
        {
            int progress = 0;
            if (!string.IsNullOrWhiteSpace(locationTextBox.Text)) progress += 25;
            if (categoryDropDown.SelectedItem is Category && ((Category)categoryDropDown.SelectedItem).Id != 0) progress += 25;
            if (!string.IsNullOrWhiteSpace(DescriptionTextBox.Text)) progress += 25;
            progressBar.Value = progress;

            if (progress == 100)
                statusLabel.Text = "You're ready to submit your report.";
            else if (!string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                statusLabel.Text = "You're almost there — your description helps us understand the problem.";
            else if (categoryDropDown.SelectedItem is Category && ((Category)categoryDropDown.SelectedItem).Id != 0)
                statusLabel.Text = "Excellent! Category selected.";
            else if (!string.IsNullOrWhiteSpace(locationTextBox.Text))
                statusLabel.Text = "Great start! Location captured.";
            else
                statusLabel.Text = "Start by entering the issue location.";
        }
    }
}
