using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoClick
{
    public class SettingsForm : Form
    {
        private TrackBar dragScrollRatioTrackBar;
        private Label dragScrollRatioValueLabel;
        private CheckBox runAtStartupCheckBox;
        private CheckBox minimizeToTrayCheckBox;
        private TabControl settingsTabControl;
        
        public float DragScrollTimeRatio { get; private set; }
        public bool RunAtStartup { get; private set; }
        public bool MinimizeToTray { get; private set; }
        
        public SettingsForm(float currentDragScrollTimeRatio)
        {
            DragScrollTimeRatio = currentDragScrollTimeRatio;
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            // Form properties
            this.Text = "Settings";
            this.Size = new Size(450, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            
            // Create tab control
            settingsTabControl = new TabControl
            {
                Dock = DockStyle.Top,
                Height = 260
            };
            
            // Create General tab
            TabPage generalTab = new TabPage("General");
            runAtStartupCheckBox = new CheckBox
            {
                Text = "Run at Windows startup",
                Location = new Point(20, 20),
                AutoSize = true
            };
            
            minimizeToTrayCheckBox = new CheckBox
            {
                Text = "Minimize to system tray",
                Location = new Point(20, 50),
                AutoSize = true
            };
            
            Label runtimeLabel = new Label
            {
                Text = ".NET Runtime Version: " + Environment.Version.ToString(),
                Location = new Point(20, 120),
                AutoSize = true,
                ForeColor = Color.Gray
            };
            
            Label authorLabel = new Label
            {
                Text = "Auto Click Tool for Windows",
                Location = new Point(20, 150),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };
            
            generalTab.Controls.Add(runAtStartupCheckBox);
            generalTab.Controls.Add(minimizeToTrayCheckBox);
            generalTab.Controls.Add(runtimeLabel);
            generalTab.Controls.Add(authorLabel);
            
            // Create Action Settings tab
            TabPage actionTab = new TabPage("Action Settings");
            
            // Drag/scroll time ratio
            Label dragScrollRatioLabel = new Label
            {
                Text = "Drag/Scroll Time Ratio:",
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            
            // Add description
            Label dragScrollRatioDescLabel = new Label
            {
                Text = "Percentage of time spent on the action versus waiting:",
                AutoSize = true,
                Location = new Point(20, 45)
            };
            
            // TrackBar for ratio
            dragScrollRatioTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)(DragScrollTimeRatio * 100),
                TickFrequency = 10,
                LargeChange = 10,
                SmallChange = 5,
                Location = new Point(20, 70),
                Width = 380
            };
            
            // Labels for trackbar
            Label minLabel = new Label
            {
                Text = "0% (All Waiting)",
                AutoSize = true,
                Location = new Point(20, 110)
            };
            
            Label maxLabel = new Label
            {
                Text = "100% (All Action)",
                AutoSize = true,
                Location = new Point(300, 110)
            };
            
            // Value display
            dragScrollRatioValueLabel = new Label
            {
                Text = $"{(int)(DragScrollTimeRatio * 100)}% Action, {(int)((1 - DragScrollTimeRatio) * 100)}% Waiting",
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold),
                Location = new Point(150, 110)
            };
            
            // Add to action tab
            actionTab.Controls.Add(dragScrollRatioLabel);
            actionTab.Controls.Add(dragScrollRatioDescLabel);
            actionTab.Controls.Add(dragScrollRatioTrackBar);
            actionTab.Controls.Add(minLabel);
            actionTab.Controls.Add(maxLabel);
            actionTab.Controls.Add(dragScrollRatioValueLabel);
            
            // Add tabs to tab control
            settingsTabControl.TabPages.Add(generalTab);
            settingsTabControl.TabPages.Add(actionTab);
            
            // Buttons
            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(250, 280),
                Width = 80
            };
            
            Button cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(340, 280),
                Width = 80
            };
            
            // Add controls
            this.Controls.Add(settingsTabControl);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
            
            // Event handling
            dragScrollRatioTrackBar.ValueChanged += DragScrollRatioTrackBar_ValueChanged;
            okButton.Click += OkButton_Click;
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }
        
        private void DragScrollRatioTrackBar_ValueChanged(object sender, EventArgs e)
        {
            int actionPercentage = dragScrollRatioTrackBar.Value;
            int waitingPercentage = 100 - actionPercentage;
            dragScrollRatioValueLabel.Text = $"{actionPercentage}% Action, {waitingPercentage}% Waiting";
            
            // Update the ratio
            DragScrollTimeRatio = actionPercentage / 100.0f;
        }
        
        private void OkButton_Click(object sender, EventArgs e)
        {
            // DragScrollTimeRatio property is already updated in the ValueChanged event
            RunAtStartup = runAtStartupCheckBox.Checked;
            MinimizeToTray = minimizeToTrayCheckBox.Checked;
        }
    }
} 