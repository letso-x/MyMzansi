using System.Drawing;
using System.Windows.Forms;

namespace MyMzansi
{
    public sealed class ServiceRequestStatusForm : Form
    {
        public ServiceRequestStatusForm()
        {
            Text = "Service Request Status - MyMzansi";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(600, 300);
            MinimumSize = new Size(520, 280);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10F);

            Label heading = new Label
            {
                Text = "Service Request Status",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 53, 91),
                AutoSize = true,
                Location = new Point(28, 28)
            };
            Label message = new Label
            {
                Text = "Service request tracking has not been implemented yet.\r\n\r\nYou can still report a municipal issue from the main menu. Submitted issue information is currently held in memory for this session.",
                AutoSize = false,
                Location = new Point(30, 82),
                Size = new Size(530, 120),
                ForeColor = Color.FromArgb(55, 65, 81)
            };
            Button backButton = new Button
            {
                Text = "Back to main menu",
                BackColor = Color.FromArgb(18, 53, 91),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(170, 38),
                Location = new Point(30, 220),
                UseVisualStyleBackColor = false
            };
            backButton.Click += delegate { Close(); };
            Controls.Add(heading);
            Controls.Add(message);
            Controls.Add(backButton);
        }
    }
}
