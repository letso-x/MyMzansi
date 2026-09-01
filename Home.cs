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
        public Home()
        {
            InitializeComponent();
        }

        private void reportIssuesBtn_Click(object sender, EventArgs e)
        {
            using (ReportIssuesPage reportIssuesPage = new ReportIssuesPage())
            {
                reportIssuesPage.ShowDialog();
            }
        }

        
    }
}
