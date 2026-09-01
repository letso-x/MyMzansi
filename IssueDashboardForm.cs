using MyMzansi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMzansi
{
    public partial class IssueDashboardForm : Form
    {
        private List<Issue> _issues;
        public IssueDashboardForm(List<Issue> issuesList)
        {
            InitializeComponent();
            _issues = issuesList;

            PopulateList();
        }

        private void PopulateList()
        {

            listBoxIssues.Items.Clear();

            foreach(var issue in _issues)
            {
                listBoxIssues.Items.Add(issue.Subject);
            }

        }

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            if(listBoxIssues.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an issue summary from the tracker list first. ");
                return;

            }

            Issue selectedIssue = _issues[listBoxIssues.SelectedIndex];


            string details = $" SUBJECT: {selectedIssue.Subject}\n\n" +
                             $" CATEGORY: {(selectedIssue.Category != null ? selectedIssue.Category.Name : "None")}\n\n" +
                             $" DESCRIPTION: \n{selectedIssue.Description}";

            MessageBox.Show(details, "");
        }

        private void btnCreateNewIssue_Click(object sender, EventArgs e)
        {
            using (ReportIssuesPage reportIssuesPage = new ReportIssuesPage(_issues))
            {
               
                if (reportIssuesPage.ShowDialog() == DialogResult.OK)
                {
                    
                    Issue newIssue = reportIssuesPage.createdIssue;

                    if (newIssue != null)
                    {
                        _issues.Add(newIssue);
                        PopulateList(); 
                    }
                }
            }

        }

        private void btnBackToMain_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void IssueDashboardForm_Load(object sender, EventArgs e)
        {

        }
    }
}
