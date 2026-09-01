using MyMzansi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMzansi
{
    public partial class ReportIssuesPage : Form
    {
        private List<Issue> issueList = new List<Issue>();
        public ReportIssuesPage()
        {
            
            InitializeComponent();
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

        }
        private void ReportIssuesPage_Load(object sender, EventArgs e)
        {
            

        }

        private void imageUploadbtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png)";
                openFileDialog.Title = "Upload an image of the issue";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (uploadedImagePreview.Image != null)
                        {
                            uploadedImagePreview.Dispose();
                        }

                        uploadedImagePreview.Image = Image.FromFile(openFileDialog.FileName);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Could not load the image file. Error: " + ex.Message, "Upload Error");
                    }
                }

            }
        }

        private void submitBtn_Click(object sender, EventArgs e)
        {
                if(string.IsNullOrWhiteSpace(locationTextBox.Text))
                {
                    MessageBox.Show("Please enter a location.", "Validation Error");
                    return;
                }

                if (categoryDropDown.SelectedValue != null && (int)categoryDropDown.SelectedValue ==0)
                {
                    MessageBox.Show("Please choose a valid category.", "Validation Error");
                    return;
                }
                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                {
                    MessageBox.Show("Please enter a location.", "Validation Error");
                    return;
                }

                    Category selectedCategory = categoryDropDown.SelectedItem as Category;

                    Image uploadedImage = uploadedImagePreview.Image;

                    Issue issue = new Issue(locationTextBox.Text, selectedCategory, DescriptionTextBox.Text, uploadedImage);
                    issueList.Add(issue);

                    MessageBox.Show($"Successfully saved new issue to the memory list!", "Success");

                    ClearForm();

    
            }

        private void ClearForm()
        {
            locationTextBox.Clear();
            categoryDropDown.SelectedIndex = 0 ;
            DescriptionTextBox.Clear();
            uploadedImagePreview.Image = null;
        }
    }
}
