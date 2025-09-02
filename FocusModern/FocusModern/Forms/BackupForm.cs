using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    public class BackupForm : Form
    {
        private readonly int branchId;
        private TextBox txtPath;
        private Button btnBrowseFolder;
        private Button btnBackup;
        private Button btnRestore;
        private Button btnClose;
        private Label lblInfo;

        private string BasePath => Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["DatabasePath"] ?? "%LocalAppData%\\FocusModern\\");

        public BackupForm(int branchNumber)
        {
            branchId = branchNumber;
            InitializeComponent();
            Theme.Apply(this);
        }

        private void InitializeComponent()
        {
            this.Text = $"Backup & Restore - Branch {branchId}";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ClientSize = new System.Drawing.Size(600, 200);

            var lblBase = new Label { Text = "Data Folder:", AutoSize = true, Left = 20, Top = 20 };
            txtPath = new TextBox { Left = 110, Top = 16, Width = 380, ReadOnly = true };
            btnBrowseFolder = new Button { Text = "Open Folder", Left = 500, Top = 15, Width = 80 };

            var lblActions = new Label { Text = "Actions:", AutoSize = true, Left = 20, Top = 60 };
            btnBackup = new Button { Text = "Create Backup (ZIP)", Left = 110, Top = 55, Width = 170 };
            btnRestore = new Button { Text = "Restore From ZIP", Left = 290, Top = 55, Width = 170 };

            lblInfo = new Label { Text = "Backups include focus_branch*.db in the selected data folder.", AutoSize = true, Left = 20, Top = 100 };
            btnClose = new Button { Text = "Close", Left = 500, Top = 140, Width = 80 };

            txtPath.Text = BasePath;

            btnBrowseFolder.Click += (s, e) =>
            {
                try { System.Diagnostics.Process.Start("explorer.exe", BasePath); } catch { }
            };

            btnBackup.Click += (s, e) => DoBackup();
            btnRestore.Click += (s, e) => DoRestore();
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };

            this.Controls.Add(lblBase);
            this.Controls.Add(txtPath);
            this.Controls.Add(btnBrowseFolder);
            this.Controls.Add(lblActions);
            this.Controls.Add(btnBackup);
            this.Controls.Add(btnRestore);
            this.Controls.Add(lblInfo);
            this.Controls.Add(btnClose);
        }

        private void DoBackup()
        {
            try
            {
                var dir = BasePath;
                if (!Directory.Exists(dir))
                {
                    MessageBox.Show($"Data folder not found: {dir}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var dbFiles = Directory.GetFiles(dir, "focus_branch*.db");
                if (dbFiles.Length == 0)
                {
                    MessageBox.Show("No database files found to backup.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var sfd = new SaveFileDialog { Filter = "ZIP Files (*.zip)|*.zip", FileName = $"FocusModern_BranchAll_{DateTime.Now:yyyyMMdd_HHmm}.zip" })
                {
                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    var tempDir = Path.Combine(Path.GetTempPath(), "FocusModernBackup_" + Guid.NewGuid());
                    Directory.CreateDirectory(tempDir);

                    foreach (var f in dbFiles)
                    {
                        var dest = Path.Combine(tempDir, Path.GetFileName(f));
                        File.Copy(f, dest, true);
                    }

                    var logsDir = Path.Combine(dir, "Logs");
                    if (Directory.Exists(logsDir))
                    {
                        var tmpLogs = Path.Combine(tempDir, "Logs");
                        Directory.CreateDirectory(tmpLogs);
                        foreach (var lf in Directory.GetFiles(logsDir, "*.log"))
                        {
                            File.Copy(lf, Path.Combine(tmpLogs, Path.GetFileName(lf)), true);
                        }
                    }

                    if (File.Exists(sfd.FileName)) File.Delete(sfd.FileName);
                    ZipFile.CreateFromDirectory(tempDir, sfd.FileName, CompressionLevel.Optimal, false);
                    Directory.Delete(tempDir, true);

                    MessageBox.Show("Backup created successfully.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DoRestore()
        {
            try
            {
                using (var ofd = new OpenFileDialog { Filter = "ZIP Files (*.zip)|*.zip" })
                {
                    if (ofd.ShowDialog() != DialogResult.OK) return;

                    if (MessageBox.Show("Restoring will overwrite existing database files. Continue?", "Confirm Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        return;

                    var dir = BasePath;
                    var tempExtract = Path.Combine(Path.GetTempPath(), "FocusModernRestore_" + Guid.NewGuid());
                    Directory.CreateDirectory(tempExtract);
                    ZipFile.ExtractToDirectory(ofd.FileName, tempExtract);

                    foreach (var f in Directory.GetFiles(tempExtract, "focus_branch*.db"))
                    {
                        var dest = Path.Combine(dir, Path.GetFileName(f));
                        File.Copy(f, dest, true);
                    }
                    Directory.Delete(tempExtract, true);

                    MessageBox.Show("Restore completed. Please re-open the branch to reload data.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Restore failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

