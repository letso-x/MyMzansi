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
    public partial class Home : Form
    {
        private List<Issue> masterIssuesList = new List<Issue>();
        public Home()
        {
            InitializeComponent();
        }

        private void reportIssuesBtn_Click(object sender, EventArgs e)
        {
            using (IssueDashboardForm dashboard = new IssueDashboardForm(masterIssuesList))
            {
                dashboard.ShowDialog();
            }
        }

        private void localEventsBtn_Click(object sender, EventArgs e)
        {
            using (LocalEventsForm eventsForm = new LocalEventsForm())
            {
                eventsForm.ShowDialog(this);
            }
        }

        private void serviceRequestStatusBtn_Click(object sender, EventArgs e)
        {
            using (ServiceRequestStatusForm statusForm = new ServiceRequestStatusForm())
            {
                statusForm.ShowDialog(this);
            }
        }
    }
}
