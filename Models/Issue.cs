using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMzansi.Models
{
    public class Issue
    {
        public string Location { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public Image IssueImage { get; set; }

        public Issue(string location, Category category, string description, Image issueImage)
        {
            Location = location;
            Category = category;
            Description = description;
            IssueImage = issueImage;

        }
    }
}
