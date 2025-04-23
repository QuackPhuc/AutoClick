using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoClick
{
    public class EditActionForm : Form
    {
        private AutoClickAction action;
        private TextBox timeToNextStepTextBox;
        private NumericUpDown startXNumeric;
        private NumericUpDown startYNumeric;
        private NumericUpDown endXNumeric;
        private NumericUpDown endYNumeric;
        private Label endPositionLabel;
        private Label endXLabel;
        private Label endYLabel;
        
        public EditActionForm(AutoClickAction action)
        {
            this.action = action;
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            // Form properties
            this.Text = "Edit Action";
            this.Size = new Size(350, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            
            // Action type label
            Label actionTypeLabel = new Label
            {
                Text = "Action Type:",
                AutoSize = true,
                Location = new Point(20, 20)
            };
            
            Label actionTypeValueLabel = new Label
            {
                Text = action.ActionType.ToString(),
                AutoSize = true,
                Location = new Point(120, 20),
                Font = new Font(this.Font, FontStyle.Bold)
            };
            
            // Start position
            Label startPositionLabel = new Label
            {
                Text = "Start Position:",
                AutoSize = true,
                Location = new Point(20, 50)
            };
            
            Label startXLabel = new Label
            {
                Text = "X:",
                AutoSize = true,
                Location = new Point(120, 50)
            };
            
            startXNumeric = new NumericUpDown
            {
                Location = new Point(140, 48),
                Width = 70,
                Minimum = 0,
                Maximum = Screen.PrimaryScreen.Bounds.Width,
                Value = action.StartPoint.X
            };
            
            Label startYLabel = new Label
            {
                Text = "Y:",
                AutoSize = true,
                Location = new Point(220, 50)
            };
            
            startYNumeric = new NumericUpDown
            {
                Location = new Point(240, 48),
                Width = 70,
                Minimum = 0,
                Maximum = Screen.PrimaryScreen.Bounds.Height,
                Value = action.StartPoint.Y
            };
            
            // End position (only for drag and scroll actions)
            endPositionLabel = new Label
            {
                Text = "End Position:",
                AutoSize = true,
                Location = new Point(20, 80),
                Visible = IsTwoPointAction()
            };
            
            endXLabel = new Label
            {
                Text = "X:",
                AutoSize = true,
                Location = new Point(120, 80),
                Visible = IsTwoPointAction()
            };
            
            endXNumeric = new NumericUpDown
            {
                Location = new Point(140, 78),
                Width = 70,
                Minimum = 0,
                Maximum = Screen.PrimaryScreen.Bounds.Width,
                Value = action.EndPoint.X,
                Visible = IsTwoPointAction()
            };
            
            endYLabel = new Label
            {
                Text = "Y:",
                AutoSize = true,
                Location = new Point(220, 80),
                Visible = IsTwoPointAction()
            };
            
            endYNumeric = new NumericUpDown
            {
                Location = new Point(240, 78),
                Width = 70,
                Minimum = 0,
                Maximum = Screen.PrimaryScreen.Bounds.Height,
                Value = action.EndPoint.Y,
                Visible = IsTwoPointAction()
            };
            
            // Time to next step
            Label timeToNextStepLabel = new Label
            {
                Text = "Time to Next Step (ms):",
                AutoSize = true,
                Location = new Point(20, IsTwoPointAction() ? 110 : 80)
            };
            
            timeToNextStepTextBox = new TextBox
            {
                Location = new Point(160, IsTwoPointAction() ? 108 : 78),
                Width = 100,
                Text = action.TimeToNextStep.ToString()
            };
            
            // Buttons
            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(150, IsTwoPointAction() ? 150 : 120),
                Width = 80
            };
            
            Button cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(240, IsTwoPointAction() ? 150 : 120),
                Width = 80
            };
            
            // Add controls
            this.Controls.Add(actionTypeLabel);
            this.Controls.Add(actionTypeValueLabel);
            this.Controls.Add(startPositionLabel);
            this.Controls.Add(startXLabel);
            this.Controls.Add(startXNumeric);
            this.Controls.Add(startYLabel);
            this.Controls.Add(startYNumeric);
            this.Controls.Add(endPositionLabel);
            this.Controls.Add(endXLabel);
            this.Controls.Add(endXNumeric);
            this.Controls.Add(endYLabel);
            this.Controls.Add(endYNumeric);
            this.Controls.Add(timeToNextStepLabel);
            this.Controls.Add(timeToNextStepTextBox);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
            
            // Event handling
            okButton.Click += OkButton_Click;
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }
        
        private bool IsTwoPointAction()
        {
            return action.ActionType == AutoClickActionType.LeftDrag || 
                   action.ActionType == AutoClickActionType.RightDrag || 
                   action.ActionType == AutoClickActionType.Scroll;
        }
        
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(timeToNextStepTextBox.Text, out int timeToNextStep) || timeToNextStep < 0)
            {
                MessageBox.Show("Please enter a valid time (milliseconds).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }
            
            // Update action properties
            action.StartPoint = new Point((int)startXNumeric.Value, (int)startYNumeric.Value);
            
            if (IsTwoPointAction())
            {
                action.EndPoint = new Point((int)endXNumeric.Value, (int)endYNumeric.Value);
            }
            
            action.TimeToNextStep = timeToNextStep;
        }
    }
} 