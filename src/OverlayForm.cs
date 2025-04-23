using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AutoClick
{
    public class OverlayForm : Form
    {
        private MainForm mainForm;
        private AutoClickActionType actionType;
        private int actionId;
        private Point startPoint = Point.Empty;
        private Point endPoint = Point.Empty;
        private bool isDragging = false;
        private int defaultTimeToNextStep = 1000; // Default 1 second

        private Label infoLabel;
        private Label idLabel;
        private TextBox timeToNextStepTextBox;
        private Button confirmButton;
        private Button cancelButton;
        private Panel controlPanel;

        // Visualization properties
        private Color pointColor = Color.FromArgb(240, 80, 80); // Bright red
        private Color endPointColor = Color.FromArgb(80, 240, 80); // Bright green
        private Color lineColor = Color.FromArgb(240, 240, 80); // Bright yellow
        private const int pointRadius = 10; // Larger point for better visibility
        private const int pointBorderWidth = 2;

        public OverlayForm(MainForm mainForm, AutoClickActionType actionType, int actionId)
        {
            this.mainForm = mainForm;
            this.actionType = actionType;
            this.actionId = actionId;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Set form properties
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.Opacity = 0.6; // Slightly higher opacity for better visibility
            this.BackColor = Color.Black;
            this.Cursor = Cursors.Cross;
            this.ShowInTaskbar = false;
            
            // Create controls
            infoLabel = new Label
            {
                AutoSize = true,
                BackColor = Color.Yellow,
                ForeColor = Color.Black,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                Text = GetInstructionText(),
                Padding = new Padding(5)
            };

            idLabel = new Label
            {
                AutoSize = true,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Visible = false,
                Padding = new Padding(3),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create a panel for the controls to ensure they appear on top and are more visible
            controlPanel = new Panel
            {
                BackColor = Color.FromArgb(220, 220, 220),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                AutoSize = true,
                Padding = new Padding(10)
            };

            Label timeLabel = new Label
            {
                Text = "Time to Next (ms):",
                AutoSize = true,
                Location = new Point(10, 10),
                Font = new Font("Arial", 10)
            };

            timeToNextStepTextBox = new TextBox
            {
                Width = 100,
                Height = 25,
                Location = new Point(10, 35),
                Text = defaultTimeToNextStep.ToString(),
                Font = new Font("Arial", 10)
            };

            confirmButton = new Button
            {
                Text = "Confirm",
                Width = 100,
                Height = 30,
                Location = new Point(10, 70),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };

            cancelButton = new Button
            {
                Text = "Cancel",
                Width = 100,
                Height = 30,
                Location = new Point(120, 70),
                BackColor = Color.LightPink,
                FlatStyle = FlatStyle.Flat
            };

            // Add controls to panel
            controlPanel.Controls.Add(timeLabel);
            controlPanel.Controls.Add(timeToNextStepTextBox);
            controlPanel.Controls.Add(confirmButton);
            controlPanel.Controls.Add(cancelButton);

            // Add controls to form
            this.Controls.Add(infoLabel);
            this.Controls.Add(idLabel);
            this.Controls.Add(controlPanel);

            // Add event handlers
            this.MouseDown += OverlayForm_MouseDown;
            this.MouseMove += OverlayForm_MouseMove;
            this.MouseUp += OverlayForm_MouseUp;
            confirmButton.Click += ConfirmButton_Click;
            cancelButton.Click += CancelButton_Click;

            // Handle keyboard escape
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.DialogResult = DialogResult.Cancel; };
        }

        private string GetInstructionText()
        {
            switch (actionType)
            {
                case AutoClickActionType.LeftClick:
                    return "Select a point for left click";
                case AutoClickActionType.RightClick:
                    return "Select a point for right click";
                case AutoClickActionType.LeftDrag:
                    return "Select start point for left drag, then drag to end point";
                case AutoClickActionType.RightDrag:
                    return "Select start point for right drag, then drag to end point";
                case AutoClickActionType.Scroll:
                    return "Select start point for scroll, then drag to define scroll direction and distance";
                default:
                    return "Select a point";
            }
        }

        private void OverlayForm_MouseDown(object sender, MouseEventArgs e)
        {
            // Single point actions
            if (actionType == AutoClickActionType.LeftClick || actionType == AutoClickActionType.RightClick)
            {
                startPoint = e.Location;
                ShowActionDetails();
            }
            // Two point actions
            else
            {
                if (startPoint == Point.Empty)
                {
                    startPoint = e.Location;
                    idLabel.Text = $"ID_{actionId}_1";
                    idLabel.Location = new Point(startPoint.X + 15, startPoint.Y - 15);
                    idLabel.Visible = true;
                    isDragging = true;
                }
            }
        }

        private void OverlayForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                endPoint = e.Location;
                this.Refresh();
            }
        }

        private void OverlayForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                endPoint = e.Location;
                ShowActionDetails();
            }
        }

        private void ShowActionDetails()
        {
            // Position controls near the action point
            int x, y;
            if (actionType == AutoClickActionType.LeftClick || actionType == AutoClickActionType.RightClick)
            {
                x = startPoint.X + 30;
                y = startPoint.Y + 30;
                idLabel.Text = $"ID_{actionId}";
                idLabel.Location = new Point(startPoint.X + 15, startPoint.Y - 15);
            }
            else
            {
                x = endPoint.X + 30;
                y = endPoint.Y + 30;
                idLabel.Text = $"ID_{actionId}_2";
                idLabel.Location = new Point(endPoint.X + 15, endPoint.Y - 15);
            }

            // Position the control panel
            controlPanel.Location = new Point(x, y);
            controlPanel.Visible = true;
            idLabel.Visible = true;
            
            // Focus on the textbox
            timeToNextStepTextBox.Focus();
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(timeToNextStepTextBox.Text, out int timeToNextStep) || timeToNextStep < 0)
            {
                MessageBox.Show("Please enter a valid time (milliseconds).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AutoClickAction action = new AutoClickAction
            {
                ActionType = actionType,
                StartPoint = startPoint,
                EndPoint = endPoint,
                TimeToNextStep = timeToNextStep
            };

            mainForm.AddActionComplete(action);
            this.DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias; // For smoother circles
            
            // Draw start point
            if (startPoint != Point.Empty)
            {
                // Draw a filled circle with border
                using (SolidBrush brush = new SolidBrush(pointColor))
                {
                    g.FillEllipse(brush, 
                        startPoint.X - pointRadius, 
                        startPoint.Y - pointRadius, 
                        pointRadius * 2, 
                        pointRadius * 2);
                }
                
                // Draw border
                using (Pen pen = new Pen(Color.White, pointBorderWidth))
                {
                    g.DrawEllipse(pen, 
                        startPoint.X - pointRadius, 
                        startPoint.Y - pointRadius, 
                        pointRadius * 2, 
                        pointRadius * 2);
                }
            }

            // Draw end point and line for drag/scroll
            if (endPoint != Point.Empty && 
                (actionType == AutoClickActionType.LeftDrag || 
                 actionType == AutoClickActionType.RightDrag || 
                 actionType == AutoClickActionType.Scroll))
            {
                // Draw line connecting points
                using (Pen pen = new Pen(lineColor, 3))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawLine(pen, startPoint, endPoint);
                }
                
                // Draw end point
                using (SolidBrush brush = new SolidBrush(endPointColor))
                {
                    g.FillEllipse(brush, 
                        endPoint.X - pointRadius, 
                        endPoint.Y - pointRadius, 
                        pointRadius * 2, 
                        pointRadius * 2);
                }
                
                // Draw border
                using (Pen pen = new Pen(Color.White, pointBorderWidth))
                {
                    g.DrawEllipse(pen, 
                        endPoint.X - pointRadius, 
                        endPoint.Y - pointRadius, 
                        pointRadius * 2, 
                        pointRadius * 2);
                }
            }
        }
    }
} 