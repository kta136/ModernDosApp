using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    public class SettingsForm : Form
    {
        private TextBox txtDatabasePath;
        private NumericUpDown numBackupInterval;
        private ComboBox cmbLogLevel;
        private TextBox txtCompanyName;
        private Button btnSave;
        private Button btnCancel;
        private Button btnBrowse;

        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
            Theme.Apply(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Settings";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ClientSize = new System.Drawing.Size(640, 240);

            int x1 = 20, x2 = 150, w = 380, y = 20, dy = 36;

            Controls.Add(new Label { Text = "Database Path:", AutoSize = true, Left = x1, Top = y + 4 });
            txtDatabasePath = new TextBox { Left = x2, Top = y, Width = w };
            btnBrowse = new Button { Text = "Browse", Left = x2 + w + 10, Top = y - 1, Width = 70 };
            btnBrowse.Click += (s, e) => BrowsePath();
            y += dy;

            Controls.Add(new Label { Text = "Backup Interval (hrs):", AutoSize = true, Left = x1, Top = y + 4 });
            numBackupInterval = new NumericUpDown { Left = x2, Top = y, Width = 80, Minimum = 1, Maximum = 168 };
            y += dy;

            Controls.Add(new Label { Text = "Log Level:", AutoSize = true, Left = x1, Top = y + 4 });
            cmbLogLevel = new ComboBox { Left = x2, Top = y, Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbLogLevel.Items.AddRange(new object[] { "Debug", "Info", "Warn", "Error" });
            y += dy;

            Controls.Add(new Label { Text = "Company Name:", AutoSize = true, Left = x1, Top = y + 4 });
            txtCompanyName = new TextBox { Left = x2, Top = y, Width = w };
            y += dy;

            btnSave = new Button { Text = "Save", Left = x2 + w - 160, Top = y, Width = 80 };
            btnCancel = new Button { Text = "Cancel", Left = x2 + w - 70, Top = y, Width = 70 };
            btnSave.Click += (s, e) => SaveSettings();
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            Controls.Add(txtDatabasePath);
            Controls.Add(btnBrowse);
            Controls.Add(numBackupInterval);
            Controls.Add(cmbLogLevel);
            Controls.Add(txtCompanyName);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private void LoadSettings()
        {
            txtDatabasePath.Text = ConfigurationManager.AppSettings["DatabasePath"] ?? "%LocalAppData%\\FocusModern\\";
            txtCompanyName.Text = ConfigurationManager.AppSettings["CompanyName"] ?? "";
            cmbLogLevel.SelectedItem = ConfigurationManager.AppSettings["LogLevel"] ?? "Info";
            if (!int.TryParse(ConfigurationManager.AppSettings["BackupInterval"], out var hours)) hours = 24;
            numBackupInterval.Value = Math.Max(1, Math.Min(168, hours));
        }

        private void SaveSettings()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                SetAppSetting(config, "DatabasePath", txtDatabasePath.Text.Trim());
                SetAppSetting(config, "BackupInterval", ((int)numBackupInterval.Value).ToString());
                SetAppSetting(config, "LogLevel", cmbLogLevel.SelectedItem?.ToString() ?? "Info");
                SetAppSetting(config, "CompanyName", txtCompanyName.Text.Trim());
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                MessageBox.Show("Settings saved.", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BrowsePath()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                try
                {
                    var current = Environment.ExpandEnvironmentVariables(txtDatabasePath.Text.Trim());
                    if (Directory.Exists(current)) fbd.SelectedPath = current;
                }
                catch { }
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtDatabasePath.Text = fbd.SelectedPath;
                }
            }
        }

        private static void SetAppSetting(Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;
        }
    }
}

