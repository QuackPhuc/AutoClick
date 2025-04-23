using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace AutoClick
{
    public class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form properties
            this.Text = "About Auto Click Tool";
            this.Size = new Size(400, 320);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // App icon
            PictureBox iconPictureBox = new PictureBox
            {
                Size = new Size(64, 64),
                Location = new Point(20, 20),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            
            // Try to get the application icon
            try 
            {
                // First try to load logo.jpeg from application directory
                string logoPath = Path.Combine(Application.StartupPath, "logo.jpeg");
                if (File.Exists(logoPath))
                {
                    using (var img = Image.FromFile(logoPath))
                    {
                        // Create a copy to avoid file locking issues
                        iconPictureBox.Image = new Bitmap(img);
                    }
                }
                else
                {
                    // Fall back to application icon if logo not found
                    iconPictureBox.Image = Icon.ExtractAssociatedIcon(Application.ExecutablePath)?.ToBitmap();
                }
            }
            catch 
            {
                // Failed to load icon, use a default color instead
                iconPictureBox.BackColor = Color.LightBlue;
            }

            // App title
            Label titleLabel = new Label
            {
                Text = "Auto Click Tool",
                Location = new Point(100, 20),
                AutoSize = true,
                Font = new Font(this.Font.FontFamily, 16, FontStyle.Bold)
            };

            // Version
            string version = "1.0.0";
            try
            {
                version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch { }

            Label versionLabel = new Label
            {
                Text = $"Version {version}",
                Location = new Point(100, 50),
                AutoSize = true
            };

            // Description
            Label descriptionLabel = new Label
            {
                Text = "A tool for automating mouse clicks and movements in Windows.",
                Location = new Point(20, 100),
                Size = new Size(350, 40),
                Font = new Font(this.Font, FontStyle.Regular)
            };

            // Copyright
            Label copyrightLabel = new Label
            {
                Text = "Copyright © 2023",
                Location = new Point(20, 150),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            // System info
            Label systemInfoLabel = new Label
            {
                Text = $".NET Runtime: {Environment.Version}\nOS: {Environment.OSVersion}",
                Location = new Point(20, 180),
                Size = new Size(350, 40),
                ForeColor = Color.DimGray
            };

            // OK button
            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(150, 240),
                Size = new Size(100, 30)
            };

            // Add controls
            this.Controls.Add(iconPictureBox);
            this.Controls.Add(titleLabel);
            this.Controls.Add(versionLabel);
            this.Controls.Add(descriptionLabel);
            this.Controls.Add(copyrightLabel);
            this.Controls.Add(systemInfoLabel);
            this.Controls.Add(okButton);

            // Event handling
            this.AcceptButton = okButton;
        }
    }
} 