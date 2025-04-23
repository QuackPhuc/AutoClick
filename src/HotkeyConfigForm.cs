using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AutoClick
{
    /// <summary>
    /// Form cấu hình phím tắt cho ứng dụng
    /// </summary>
    public class HotkeyConfigForm : Form
    {
        #region Fields
        private CheckBox _ctrlCheckBox;
        private CheckBox _altCheckBox;
        private CheckBox _shiftCheckBox;
        private ComboBox _keyComboBox;
        
        // Các hằng số cho phím sửa đổi
        private const int MOD_ALT = 0x0001;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;
        
        // Từ điển ánh xạ tên thân thiện với người dùng đến giá trị Keys
        private readonly Dictionary<string, Keys> _keyMap = new Dictionary<string, Keys>();
        #endregion

        #region Properties
        /// <summary>
        /// Phím sửa đổi được chọn (Ctrl, Alt, Shift)
        /// </summary>
        public int SelectedModifiers { get; private set; }
        
        /// <summary>
        /// Phím chính được chọn
        /// </summary>
        public Keys SelectedKey { get; private set; }
        #endregion
        
        /// <summary>
        /// Khởi tạo form cấu hình phím tắt
        /// </summary>
        /// <param name="currentModifiers">Phím sửa đổi hiện tại</param>
        /// <param name="currentKey">Phím chính hiện tại</param>
        public HotkeyConfigForm(int currentModifiers, Keys currentKey)
        {
            SelectedModifiers = currentModifiers;
            SelectedKey = currentKey;
            
            InitializeComponent();
            PopulateKeyMap();
            PopulateKeyComboBox();
            InitializeValues(currentModifiers, currentKey);
        }
        
        /// <summary>
        /// Khởi tạo giao diện
        /// </summary>
        private void InitializeComponent()
        {
            // Thuộc tính form
            Text = "Cấu hình phím tắt";
            Size = new Size(380, 220);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            
            // Tiêu đề
            Label titleLabel = new Label
            {
                Text = "Cấu hình phím tắt Bắt đầu/Dừng",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            
            // Hướng dẫn
            Label instructionLabel = new Label
            {
                Text = "Chọn phím ít phổ biến để tránh xung đột với các ứng dụng khác.",
                AutoSize = true,
                Location = new Point(20, 40)
            };
            
            // Checkbox phím sửa đổi
            Label modifiersLabel = new Label
            {
                Text = "Phím sửa đổi:",
                AutoSize = true,
                Location = new Point(20, 70)
            };
            
            _ctrlCheckBox = new CheckBox
            {
                Text = "Ctrl",
                AutoSize = true,
                Location = new Point(100, 70)
            };
            
            _altCheckBox = new CheckBox
            {
                Text = "Alt",
                AutoSize = true,
                Location = new Point(150, 70)
            };
            
            _shiftCheckBox = new CheckBox
            {
                Text = "Shift",
                AutoSize = true,
                Location = new Point(200, 70)
            };
            
            // Lựa chọn phím
            Label keyLabel = new Label
            {
                Text = "Phím:",
                AutoSize = true,
                Location = new Point(20, 100)
            };
            
            _keyComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200,
                Location = new Point(100, 100)
            };
            
            // Cảnh báo
            Label warningLabel = new Label
            {
                Text = "Lưu ý: Một số phím hệ thống không thể dùng làm phím tắt.",
                AutoSize = true,
                ForeColor = Color.DarkRed,
                Location = new Point(20, 130)
            };
            
            // Nút bấm
            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(180, 150),
                Width = 80
            };
            
            Button cancelButton = new Button
            {
                Text = "Hủy",
                DialogResult = DialogResult.Cancel,
                Location = new Point(270, 150),
                Width = 80
            };
            
            // Thêm sự kiện
            okButton.Click += OkButton_Click;
            
            // Thêm các điều khiển vào form
            Controls.Add(titleLabel);
            Controls.Add(instructionLabel);
            Controls.Add(modifiersLabel);
            Controls.Add(_ctrlCheckBox);
            Controls.Add(_altCheckBox);
            Controls.Add(_shiftCheckBox);
            Controls.Add(keyLabel);
            Controls.Add(_keyComboBox);
            Controls.Add(warningLabel);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            
            AcceptButton = okButton;
            CancelButton = cancelButton;
        }
        
        /// <summary>
        /// Điền từ điển ánh xạ phím
        /// </summary>
        private void PopulateKeyMap()
        {
            // Phím chức năng
            for (int i = 1; i <= 12; i++)
            {
                _keyMap.Add($"F{i}", (Keys)Enum.Parse(typeof(Keys), $"F{i}"));
            }
            
            // Phím chữ
            for (char c = 'A'; c <= 'Z'; c++)
            {
                _keyMap.Add(c.ToString(), (Keys)Enum.Parse(typeof(Keys), c.ToString()));
            }
            
            // Phím số
            for (int i = 0; i <= 9; i++)
            {
                _keyMap.Add(i.ToString(), (Keys)Enum.Parse(typeof(Keys), $"D{i}"));
            }
            
            // Phím đặc biệt
            _keyMap.Add("Insert", Keys.Insert);
            _keyMap.Add("Delete", Keys.Delete);
            _keyMap.Add("Home", Keys.Home);
            _keyMap.Add("End", Keys.End);
            _keyMap.Add("PageUp", Keys.PageUp);
            _keyMap.Add("PageDown", Keys.PageDown);
            _keyMap.Add("Pause", Keys.Pause);
            _keyMap.Add("ScrollLock", Keys.Scroll);
        }
        
        /// <summary>
        /// Điền danh sách phím vào combobox
        /// </summary>
        private void PopulateKeyComboBox()
        {
            // Thêm phím chức năng
            for (int i = 1; i <= 12; i++)
            {
                _keyComboBox.Items.Add($"F{i}");
            }
            
            // Thêm phím thông dụng
            _keyComboBox.Items.Add("Insert");
            _keyComboBox.Items.Add("Delete");
            _keyComboBox.Items.Add("Home");
            _keyComboBox.Items.Add("End");
            _keyComboBox.Items.Add("PageUp");
            _keyComboBox.Items.Add("PageDown");
            _keyComboBox.Items.Add("Pause");
            _keyComboBox.Items.Add("ScrollLock");

            // Thêm phím số
            for (int i = 0; i <= 9; i++)
            {
                _keyComboBox.Items.Add(i.ToString());
            }

            // Thêm phím chữ
            for (char c = 'A'; c <= 'Z'; c++)
            {
                _keyComboBox.Items.Add(c.ToString());
            }
        }
        
        /// <summary>
        /// Khởi tạo giá trị cho các điều khiển
        /// </summary>
        /// <param name="modifiers">Phím sửa đổi</param>
        /// <param name="key">Phím chính</param>
        private void InitializeValues(int modifiers, Keys key)
        {
            // Thiết lập phím sửa đổi
            _ctrlCheckBox.Checked = (modifiers & MOD_CONTROL) != 0;
            _altCheckBox.Checked = (modifiers & MOD_ALT) != 0;
            _shiftCheckBox.Checked = (modifiers & MOD_SHIFT) != 0;
            
            // Tìm tên phím trong map
            string keyName = null;
            foreach (var pair in _keyMap)
            {
                if (pair.Value == key)
                {
                    keyName = pair.Key;
                    break;
                }
            }
            
            // Chọn phím trong combobox
            if (keyName != null && _keyComboBox.Items.Contains(keyName))
            {
                _keyComboBox.SelectedItem = keyName;
            }
            else
            {
                // Mặc định là F9 nếu không tìm thấy
                if (_keyComboBox.Items.Contains("F9"))
                {
                    _keyComboBox.SelectedItem = "F9";
                }
                else if (_keyComboBox.Items.Count > 0)
                {
                    _keyComboBox.SelectedIndex = 0;
                }
            }
        }
        
        /// <summary>
        /// Xử lý sự kiện khi nhấn nút OK
        /// </summary>
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (_keyComboBox.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một phím.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }
            
            // Tính toán phím sửa đổi
            SelectedModifiers = 0;
            if (_ctrlCheckBox.Checked) SelectedModifiers |= MOD_CONTROL;
            if (_altCheckBox.Checked) SelectedModifiers |= MOD_ALT;
            if (_shiftCheckBox.Checked) SelectedModifiers |= MOD_SHIFT;
            
            // Lấy phím được chọn
            string keyName = _keyComboBox.SelectedItem.ToString();
            if (_keyMap.ContainsKey(keyName))
            {
                SelectedKey = _keyMap[keyName];
            }
            else
            {
                SelectedKey = Keys.F9; // Mặc định là F9 nếu không tìm thấy
            }
        }
    }
} 