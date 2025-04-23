using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace AutoClick
{
    /// <summary>
    /// Main form of the AutoClick application
    /// </summary>
    public partial class MainForm : Form
    {
        #region Native Methods & Constants
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_WHEEL = 0x0800;

        // Hotkey constants
        private const int MOD_ALT = 0x0001;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID_START_STOP = 1;
        #endregion

        #region Fields
        private readonly List<AutoClickAction> _clickActions = new List<AutoClickAction>();
        private bool _isRunning;
        private Thread _clickThread;
        private CancellationTokenSource _cancellationTokenSource;
        private AutoClickActionType _currentActionType;
        private int _currentActionId = -1;
        private string _currentScriptPath = string.Empty;
        private float _dragScrollTimeRatio = 0.7f; // Default: 70% time for action, 30% for waiting
        private int _repeatCount; // 0 = infinite, otherwise specific repeat count
        private int _currentLoopCount;

        // Hotkey configuration
        private Keys _startStopHotkey = Keys.F9;
        private int _startStopModifiers; // Default: no modifiers
        #endregion

        #region UI Controls
        private ListView _actionsList;
        private ToolStripStatusLabel _statusLabel;
        private Button _startStopBtn;
        private Label _currentHotkeyLabel;
        private NumericUpDown _repeatCountNumeric;
        #endregion

        /// <summary>
        /// Initializes the main form
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the user interface
        /// </summary>
        private void InitializeComponent()
        {
            // Form properties
            Text = "Auto Click Tool";
            Size = new Size(800, 600);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            // Set application icon if the file exists
            try
            {
                string logoPath = Path.Combine(Application.StartupPath, "logo.jpeg");
                if (File.Exists(logoPath))
                {
                    this.Icon = Icon.ExtractAssociatedIcon(logoPath);
                }
            }
            catch (Exception ex)
            {
                // Log error but continue without icon
                Console.WriteLine($"Error loading icon: {ex.Message}");
            }

            // Create ListView to display action list
            _actionsList = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HideSelection = false
            };
            _actionsList.Columns.Add("ID", 40);
            _actionsList.Columns.Add("Action Type", 100);
            _actionsList.Columns.Add("Position", 180);
            _actionsList.Columns.Add("Wait Time (ms)", 120);

            // Create button panel
            Panel buttonsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 140
            };

            // Create action buttons
            Button addLeftClickBtn = CreateButton("Left Click", new Point(10, 10));
            Button addRightClickBtn = CreateButton("Right Click", new Point(140, 10));
            Button addLeftDragBtn = CreateButton("Left Drag", new Point(270, 10));
            Button addRightDragBtn = CreateButton("Right Drag", new Point(400, 10));
            Button addScrollBtn = CreateButton("Mouse Scroll", new Point(530, 10));
            
            Button deleteActionBtn = CreateButton("Delete Action", new Point(10, 50));
            _startStopBtn = CreateButton("Start", new Point(140, 50));
            Button saveScriptBtn = CreateButton("Save Script", new Point(270, 50));
            Button loadScriptBtn = CreateButton("Load Script", new Point(400, 50));
            Button settingsBtn = CreateButton("Settings", new Point(530, 50));
            Button aboutBtn = CreateButton("About", new Point(660, 50));

            // Repeat count controls
            Label repeatCountLabel = new Label 
            { 
                Text = "Repeat Count (0 = infinite):", 
                AutoSize = true, 
                Location = new Point(10, 90) 
            };
            _repeatCountNumeric = new NumericUpDown 
            { 
                Location = new Point(170, 88), 
                Width = 80, 
                Minimum = 0, 
                Maximum = 999999, 
                Value = 0 
            };

            // Hotkey configuration button
            Button hotkeyConfigBtn = CreateButton("Hotkey Config", new Point(270, 90));
            
            // Display current hotkey
            _currentHotkeyLabel = new Label 
            { 
                Text = $"Start/Stop Hotkey: {GetHotkeyDisplayText(_startStopModifiers, _startStopHotkey)}", 
                AutoSize = true, 
                Location = new Point(400, 90) 
            };

            // Add event handlers
            addLeftClickBtn.Click += (s, e) => AddAction(AutoClickActionType.LeftClick);
            addRightClickBtn.Click += (s, e) => AddAction(AutoClickActionType.RightClick);
            addLeftDragBtn.Click += (s, e) => AddAction(AutoClickActionType.LeftDrag);
            addRightDragBtn.Click += (s, e) => AddAction(AutoClickActionType.RightDrag);
            addScrollBtn.Click += (s, e) => AddAction(AutoClickActionType.Scroll);
            deleteActionBtn.Click += DeleteAction;
            _startStopBtn.Click += ToggleStartStop;
            saveScriptBtn.Click += SaveScript;
            loadScriptBtn.Click += LoadScript;
            settingsBtn.Click += ShowSettings;
            aboutBtn.Click += ShowAboutDialog;
            hotkeyConfigBtn.Click += (s, e) => ShowHotkeyConfig(_currentHotkeyLabel);

            // Add controls to panel
            buttonsPanel.Controls.Add(addLeftClickBtn);
            buttonsPanel.Controls.Add(addRightClickBtn);
            buttonsPanel.Controls.Add(addLeftDragBtn);
            buttonsPanel.Controls.Add(addRightDragBtn);
            buttonsPanel.Controls.Add(addScrollBtn);
            buttonsPanel.Controls.Add(deleteActionBtn);
            buttonsPanel.Controls.Add(_startStopBtn);
            buttonsPanel.Controls.Add(saveScriptBtn);
            buttonsPanel.Controls.Add(loadScriptBtn);
            buttonsPanel.Controls.Add(settingsBtn);
            buttonsPanel.Controls.Add(aboutBtn);
            buttonsPanel.Controls.Add(repeatCountLabel);
            buttonsPanel.Controls.Add(_repeatCountNumeric);
            buttonsPanel.Controls.Add(hotkeyConfigBtn);
            buttonsPanel.Controls.Add(_currentHotkeyLabel);

            // Create status strip
            StatusStrip statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel("Ready");
            statusStrip.Items.Add(_statusLabel);

            // Add controls to form
            Controls.Add(_actionsList);
            Controls.Add(buttonsPanel);
            Controls.Add(statusStrip);

            // Register events
            _actionsList.DoubleClick += EditAction;
            _actionsList.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete) DeleteAction(s, e); };
            
            // Set up hotkeys
            Load += (s, e) => RegisterHotkeys();
            FormClosing += (s, e) => UnregisterHotkeys();
        }

        /// <summary>
        /// Creates a button with standard size and position
        /// </summary>
        private Button CreateButton(string text, Point location)
        {
            return new Button
            {
                Text = text,
                Width = 120,
                Height = 30,
                Location = location
            };
        }

        /// <summary>
        /// Converts modifiers and key to display text
        /// </summary>
        private string GetHotkeyDisplayText(int modifiers, Keys key)
        {
            string text = "";
            if ((modifiers & MOD_CONTROL) != 0) text += "Ctrl+";
            if ((modifiers & MOD_ALT) != 0) text += "Alt+";
            if ((modifiers & MOD_SHIFT) != 0) text += "Shift+";
            if ((modifiers & MOD_WIN) != 0) text += "Win+";
            text += key.ToString();
            return text;
        }

        /// <summary>
        /// Registers global hotkeys
        /// </summary>
        private void RegisterHotkeys()
        {
            UnregisterHotkeys(); // Unregister previous hotkeys

            // Convert Keys to virtual key code
            int vkCode = (int)_startStopHotkey;
            
            // Register hotkey
            if (!RegisterHotKey(Handle, HOTKEY_ID_START_STOP, _startStopModifiers, vkCode))
            {
                MessageBox.Show("Unable to register Start/Stop hotkey. The hotkey may be in use by another application.",
                    "Hotkey Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Unregisters hotkeys
        /// </summary>
        private void UnregisterHotkeys()
        {
            UnregisterHotKey(Handle, HOTKEY_ID_START_STOP);
        }

        /// <summary>
        /// Processes Windows messages to capture global hotkeys
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == HOTKEY_ID_START_STOP)
                {
                    ToggleStartStop(this, EventArgs.Empty);
                }
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Shows hotkey configuration form
        /// </summary>
        private void ShowHotkeyConfig(Label currentHotkeyLabel)
        {
            HotkeyConfigForm hotkeyForm = new HotkeyConfigForm(_startStopModifiers, _startStopHotkey);
            if (hotkeyForm.ShowDialog() == DialogResult.OK)
            {
                _startStopModifiers = hotkeyForm.SelectedModifiers;
                _startStopHotkey = hotkeyForm.SelectedKey;
                
                // Update label and re-register hotkeys
                currentHotkeyLabel.Text = $"Start/Stop Hotkey: {GetHotkeyDisplayText(_startStopModifiers, _startStopHotkey)}";
                RegisterHotkeys();
            }
        }

        /// <summary>
        /// Adds a new action
        /// </summary>
        private void AddAction(AutoClickActionType actionType)
        {
            if (_isRunning) return;

            _currentActionType = actionType;
            _currentActionId = _clickActions.Count;

            // Create overlay form to select click/drag position
            OverlayForm overlay = new OverlayForm(this, actionType, _currentActionId);
            if (overlay.ShowDialog() == DialogResult.OK)
            {
                UpdateActionsListView();
            }
        }

        /// <summary>
        /// Adds a new action to the list (called from OverlayForm)
        /// </summary>
        public void AddActionComplete(AutoClickAction action)
        {
            _clickActions.Add(action);
            UpdateActionsListView();
        }

        /// <summary>
        /// Opens action edit form
        /// </summary>
        private void EditAction(object sender, EventArgs e)
        {
            if (_isRunning || _actionsList.SelectedItems.Count == 0) return;

            int index = _actionsList.SelectedIndices[0];
            AutoClickAction action = _clickActions[index];

            // Show edit form
            EditActionForm editForm = new EditActionForm(action);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                UpdateActionsListView();
            }
        }

        /// <summary>
        /// Deletes selected action from the list
        /// </summary>
        private void DeleteAction(object sender, EventArgs e)
        {
            if (_isRunning || _actionsList.SelectedItems.Count == 0) return;

            int index = _actionsList.SelectedIndices[0];
            _clickActions.RemoveAt(index);
            UpdateActionsListView();
        }

        /// <summary>
        /// Updates the ListView displaying the action list
        /// </summary>
        private void UpdateActionsListView()
        {
            _actionsList.Items.Clear();
            
            for (int i = 0; i < _clickActions.Count; i++)
            {
                AutoClickAction action = _clickActions[i];
                ListViewItem item = new ListViewItem(i.ToString());
                
                item.SubItems.Add(GetActionTypeDisplayName(action.ActionType));
                
                if (action.ActionType == AutoClickActionType.LeftClick || action.ActionType == AutoClickActionType.RightClick)
                {
                    item.SubItems.Add($"({action.StartPoint.X}, {action.StartPoint.Y})");
                }
                else
                {
                    item.SubItems.Add($"({action.StartPoint.X}, {action.StartPoint.Y}) → ({action.EndPoint.X}, {action.EndPoint.Y})");
                }
                
                item.SubItems.Add(action.TimeToNextStep.ToString());
                _actionsList.Items.Add(item);
            }
        }

        /// <summary>
        /// Gets display name for action type
        /// </summary>
        private string GetActionTypeDisplayName(AutoClickActionType actionType)
        {
            switch (actionType)
            {
                case AutoClickActionType.LeftClick: return "Left Click";
                case AutoClickActionType.RightClick: return "Right Click";
                case AutoClickActionType.LeftDrag: return "Left Drag";
                case AutoClickActionType.RightDrag: return "Right Drag";
                case AutoClickActionType.Scroll: return "Mouse Scroll";
                default: return actionType.ToString();
            }
        }

        /// <summary>
        /// Starts or stops script execution
        /// </summary>
        private void ToggleStartStop(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                StopScript();
            }
            else
            {
                StartScript();
            }
        }

        /// <summary>
        /// Starts script execution
        /// </summary>
        private void StartScript()
        {
            if (_clickActions.Count == 0)
            {
                MessageBox.Show("No actions to execute.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Reset loop counter
            _currentLoopCount = 0;
            _repeatCount = (int)_repeatCountNumeric.Value;

            _isRunning = true;
            _startStopBtn.Text = "Stop";
            _statusLabel.Text = "Running...";

            _cancellationTokenSource = new CancellationTokenSource();
            _clickThread = new Thread(() => ExecuteScript(_cancellationTokenSource.Token));
            _clickThread.IsBackground = true;
            _clickThread.Start();
        }

        /// <summary>
        /// Stops script execution
        /// </summary>
        private void StopScript()
        {
            _isRunning = false;
            _startStopBtn.Text = "Start";
            _statusLabel.Text = "Stopped";

            if (_clickThread != null && _clickThread.IsAlive)
            {
                // Send cancellation signal instead of using Abort
                _cancellationTokenSource.Cancel();
                
                // Wait a short time for thread to exit
                if (!_clickThread.Join(1000))
                {
                    // If cannot clean shutdown, log but don't abort
                    // Thread will terminate itself as it's a background thread
                    _statusLabel.Text = "Stopping thread...";
                }
            }
        }

        /// <summary>
        /// Executes the script
        /// </summary>
        private void ExecuteScript(CancellationToken cancellationToken)
        {
            try
            {
                while (_isRunning && !cancellationToken.IsCancellationRequested)
                {
                    // Check if repeat limit has been reached
                    if (_repeatCount > 0 && _currentLoopCount >= _repeatCount)
                    {
                        break;
                    }

                    foreach (var action in _clickActions)
                    {
                        if (!_isRunning || cancellationToken.IsCancellationRequested) break;

                        // Perform action
                        switch (action.ActionType)
                        {
                            case AutoClickActionType.LeftClick:
                                PerformLeftClick(action.StartPoint.X, action.StartPoint.Y);
                                break;
                            case AutoClickActionType.RightClick:
                                PerformRightClick(action.StartPoint.X, action.StartPoint.Y);
                                break;
                            case AutoClickActionType.LeftDrag:
                                PerformLeftDrag(action.StartPoint, action.EndPoint, action.TimeToNextStep, cancellationToken);
                                break;
                            case AutoClickActionType.RightDrag:
                                PerformRightDrag(action.StartPoint, action.EndPoint, action.TimeToNextStep, cancellationToken);
                                break;
                            case AutoClickActionType.Scroll:
                                PerformScroll(action.StartPoint, action.EndPoint, action.TimeToNextStep, cancellationToken);
                                break;
                        }

                        // Wait for next action if it's a click
                        if (action.ActionType == AutoClickActionType.LeftClick || 
                            action.ActionType == AutoClickActionType.RightClick)
                        {
                            // Use cancellable sleep
                            CancellableWait(action.TimeToNextStep, cancellationToken);
                        }
                        else
                        {
                            // For drag/scroll, part of the time was already used for the action
                            // So only wait the remaining time
                            int waitTime = (int)(action.TimeToNextStep * (1 - _dragScrollTimeRatio));
                            if (waitTime > 0)
                            {
                                CancellableWait(waitTime, cancellationToken);
                            }
                        }
                    }
                    
                    // Increment loop counter if a full set was completed
                    if (_repeatCount > 0 && _isRunning && !cancellationToken.IsCancellationRequested)
                    {
                        _currentLoopCount++;
                        this.Invoke((MethodInvoker)delegate {
                            _statusLabel.Text = $"Running... (Loop {_currentLoopCount}/{_repeatCount})";
                        });
                        
                        // Check if complete
                        if (_currentLoopCount >= _repeatCount)
                        {
                            this.Invoke((MethodInvoker)delegate {
                                StopScript();
                                _statusLabel.Text = "All loops completed";
                            });
                            break;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Cancellation requested, just exit thread
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _isRunning = false;
                this.Invoke((MethodInvoker)delegate
                {
                    _startStopBtn.Text = "Start";
                    _statusLabel.Text = "Error";
                });
            }
        }

        /// <summary>
        /// Wait with cancellation support
        /// </summary>
        private void CancellableWait(int waitTime, CancellationToken cancellationToken)
        {
            try
            {
                // Break wait time into smaller chunks for faster cancellation
                for (int i = 0; i < waitTime; i += 100)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    int sleepTime = Math.Min(100, waitTime - i);
                    if (sleepTime > 0)
                        Thread.Sleep(sleepTime);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
        }

        /// <summary>
        /// Performs left mouse click
        /// </summary>
        private void PerformLeftClick(int x, int y)
        {
            Cursor.Position = new Point(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// Performs right mouse click
        /// </summary>
        private void PerformRightClick(int x, int y)
        {
            Cursor.Position = new Point(x, y);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// Performs left mouse drag from start to end point
        /// </summary>
        private void PerformLeftDrag(Point start, Point end, int totalTime, CancellationToken cancellationToken)
        {
            int actionTime = (int)(totalTime * _dragScrollTimeRatio);
            Cursor.Position = start;
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            
            PerformSmoothMove(start, end, actionTime, cancellationToken);
            
            Cursor.Position = end;
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// Performs right mouse drag from start to end point
        /// </summary>
        private void PerformRightDrag(Point start, Point end, int totalTime, CancellationToken cancellationToken)
        {
            int actionTime = (int)(totalTime * _dragScrollTimeRatio);
            Cursor.Position = start;
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            
            PerformSmoothMove(start, end, actionTime, cancellationToken);
            
            Cursor.Position = end;
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// Smoothly moves cursor from start to end point
        /// </summary>
        private void PerformSmoothMove(Point start, Point end, int actionTime, CancellationToken cancellationToken)
        {
            int steps = Math.Max(actionTime / 10, 1); // Move every 10ms, minimum 1 step
            for (int i = 1; i <= steps; i++)
            {
                if (!_isRunning || cancellationToken.IsCancellationRequested) break;
                
                float ratio = (float)i / steps;
                int x = (int)(start.X + (end.X - start.X) * ratio);
                int y = (int)(start.Y + (end.Y - start.Y) * ratio);
                Cursor.Position = new Point(x, y);
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Performs mouse scroll
        /// </summary>
        private void PerformScroll(Point start, Point end, int totalTime, CancellationToken cancellationToken)
        {
            int actionTime = (int)(totalTime * _dragScrollTimeRatio);
            Cursor.Position = start;
            
            // Calculate scroll direction and distance
            int scrollAmount = (start.Y - end.Y) * 5; // Positive = up, negative = down
            
            int steps = Math.Max(actionTime / 50, 1); // Scroll every 50ms, minimum 1 step
            for (int i = 1; i <= steps; i++)
            {
                if (!_isRunning || cancellationToken.IsCancellationRequested) break;
                
                int stepScroll = scrollAmount / steps;
                mouse_event(MOUSEEVENTF_WHEEL, 0, 0, stepScroll, 0);
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Saves script to file
        /// </summary>
        private void SaveScript(object sender, EventArgs e)
        {
            if (_clickActions.Count == 0)
            {
                MessageBox.Show("No actions to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Auto Click Script (*.acs)|*.acs",
                Title = "Save Script"
            };
            
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(_clickActions, options);
                    File.WriteAllText(saveDialog.FileName, json);
                    _currentScriptPath = saveDialog.FileName;
                    _statusLabel.Text = "Script saved.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Loads script from file
        /// </summary>
        private void LoadScript(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "Auto Click Script (*.acs)|*.acs",
                Title = "Load Script"
            };
            
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string json = File.ReadAllText(openDialog.FileName);
                    var loadedActions = JsonSerializer.Deserialize<List<AutoClickAction>>(json);
                    
                    // Clear existing list and add new items instead of reassigning
                    _clickActions.Clear();
                    _clickActions.AddRange(loadedActions);
                    
                    _currentScriptPath = openDialog.FileName;
                    UpdateActionsListView();
                    _statusLabel.Text = "Script loaded.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Shows settings form
        /// </summary>
        private void ShowSettings(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(_dragScrollTimeRatio);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                _dragScrollTimeRatio = settingsForm.DragScrollTimeRatio;
            }
        }

        /// <summary>
        /// Shows about dialog
        /// </summary>
        private void ShowAboutDialog(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }
    }

    /// <summary>
    /// Auto click action type enumeration
    /// </summary>
    public enum AutoClickActionType
    {
        LeftClick,
        RightClick,
        LeftDrag,
        RightDrag,
        Scroll
    }

    /// <summary>
    /// Stores information about a click action
    /// </summary>
    public class AutoClickAction
    {
        public AutoClickActionType ActionType { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public int TimeToNextStep { get; set; }

        public AutoClickAction()
        {
            TimeToNextStep = 1000; // Default 1 second
        }
    }
} 