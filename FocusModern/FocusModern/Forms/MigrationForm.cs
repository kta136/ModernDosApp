using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using FocusModern.Data;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    /// <summary>
    /// Legacy data migration scaffold. Lets you pick Old/{1,2,3} folders,
    /// shows file counts, and performs a dry-run scan. Import button is
    /// stubbed for now to be implemented per file format rules.
    /// </summary>
    public class MigrationForm : Form
    {
        private readonly DatabaseManager dbManager;

        private TextBox txtRoot;
        private Button btnBrowseRoot;
        private ComboBox cmbBranch;
        private Label lblTargetDb;

        private SplitContainer splitMain;
        private ListView lvFiles;
        private TabControl previewTabs;
        private TabPage tabText;
        private TabPage tabHex;
        private TabPage tabParsed;
        private RichTextBox rtbText;
        private RichTextBox rtbHex;
        private DataGridView dgvParsed;
        private Button btnScan;
        private Button btnImport;
        private Button btnClose;
        private Label lblSummary;
        private Button btnAccountsToCustomers;
        private Button btnCashToTransactions;

        public MigrationForm(DatabaseManager manager)
        {
            dbManager = manager;
            InitializeComponent();
            Theme.Apply(this);
            UpdateTargetInfo();
            TryAutoSetRoot();
        }

        private void InitializeComponent()
        {
            this.Text = "Legacy Data Migration";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new System.Drawing.Size(900, 560);
            this.MinimumSize = new System.Drawing.Size(840, 520);

            var lblRoot = new Label { Text = "Legacy Root (Old):", Left = 20, Top = 20, AutoSize = true };
            txtRoot = new TextBox { Left = 150, Top = 16, Width = 560 };
            btnBrowseRoot = new Button { Text = "Browse", Left = 720, Top = 15, Width = 80 };
            btnBrowseRoot.Click += (s, e) => BrowseRoot();

            var lblBranch = new Label { Text = "Target Branch:", Left = 20, Top = 56, AutoSize = true };
            cmbBranch = new ComboBox { Left = 150, Top = 52, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBranch.Items.AddRange(new object[] { 1, 2, 3 });
            cmbBranch.SelectedIndex = 0;
            cmbBranch.SelectedIndexChanged += (s, e) => UpdateTargetInfo();

            lblTargetDb = new Label { Left = 280, Top = 56, AutoSize = true };

            splitMain = new SplitContainer { Left = 20, Top = 90, Width = 840, Height = 380, Orientation = Orientation.Horizontal };

            lvFiles = new ListView { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true };
            lvFiles.Columns.Add("Branch", 70);
            lvFiles.Columns.Add("File", 220);
            lvFiles.Columns.Add("Type", 80);
            lvFiles.Columns.Add("Size (KB)", 90);
            lvFiles.Columns.Add("Path", 360);
            lvFiles.SelectedIndexChanged += (s, e) => LoadPreviewForSelected();
            splitMain.Panel1.Controls.Add(lvFiles);

            previewTabs = new TabControl { Dock = DockStyle.Fill };
            tabText = new TabPage("Text");
            tabHex = new TabPage("Hex");
            tabParsed = new TabPage("Parsed");
            rtbText = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, Font = new System.Drawing.Font("Consolas", 10) };
            rtbHex = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, Font = new System.Drawing.Font("Consolas", 10) };
            dgvParsed = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells };
            tabText.Controls.Add(rtbText);
            tabHex.Controls.Add(rtbHex);
            tabParsed.Controls.Add(dgvParsed);
            previewTabs.TabPages.Add(tabText);
            previewTabs.TabPages.Add(tabHex);
            previewTabs.TabPages.Add(tabParsed);
            splitMain.Panel2.Controls.Add(previewTabs);

            btnScan = new Button { Text = "Dry Run Scan", Left = 20, Top = 480, Width = 120 };
            btnImport = new Button { Text = "Stage Files", Left = 150, Top = 480, Width = 120 };
            btnAccountsToCustomers = new Button { Text = "ACCOUNT.FIL → Customers", Left = 280, Top = 480, Width = 200 };
            btnCashToTransactions = new Button { Text = "CASH.FIL → Transactions", Left = 490, Top = 480, Width = 200 };
            btnClose = new Button { Text = "Close", Left = 780, Top = 480, Width = 80 };
            lblSummary = new Label { Left = 290, Top = 485, AutoSize = true };

            btnScan.Click += (s, e) => DoScan();
            btnImport.Click += (s, e) => DoImport();
            btnAccountsToCustomers.Click += (s, e) => DoAccountsToCustomers();
            btnCashToTransactions.Click += (s, e) => DoCashToTransactions();
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };

            this.Controls.AddRange(new Control[]
            {
                lblRoot, txtRoot, btnBrowseRoot,
                lblBranch, cmbBranch, lblTargetDb,
                splitMain, btnScan, btnImport, btnAccountsToCustomers, btnCashToTransactions, btnClose, lblSummary
            });
        }

        private void UpdateTargetInfo()
        {
            try
            {
                var branch = (int)cmbBranch.SelectedItem;
                // Show database file path hint
                var dbPath = System.Environment.ExpandEnvironmentVariables(System.Configuration.ConfigurationManager.AppSettings["DatabasePath"] ?? "%LocalAppData%\\FocusModern\\");
                lblTargetDb.Text = $"DB Folder: {dbPath} (focus_branch{branch}.db)";

                // Auto-detect branch from selected folder name when possible
                var folder = (txtRoot.Text ?? string.Empty).Trim();
                if (Directory.Exists(folder))
                {
                    var name = new DirectoryInfo(folder).Name;
                    if (int.TryParse(name, out var detected) && detected >= 1 && detected <= 3)
                    {
                        cmbBranch.SelectedItem = detected; // triggers UpdateTargetInfo again
                    }
                }
            }
            catch { }
        }

        private void BrowseRoot()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (Directory.Exists(txtRoot.Text)) fbd.SelectedPath = txtRoot.Text;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtRoot.Text = fbd.SelectedPath;
                    UpdateTargetInfo();
                }
            }
        }

        private void TryAutoSetRoot()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtRoot.Text) && Directory.Exists(txtRoot.Text)) return;
                string baseDir = AppContext.BaseDirectory;
                var candidates = new List<string>();
                // Look for a nearby 'Old' folder
                var dir = new DirectoryInfo(baseDir);
                for (int i = 0; i < 6 && dir != null; i++)
                {
                    var probe = Path.Combine(dir.FullName, "Old");
                    if (Directory.Exists(probe)) candidates.Add(probe);
                    dir = dir.Parent;
                }
                var pick = candidates.FirstOrDefault();
                if (pick != null)
                {
                    // If Old/1 exists, prefer it
                    var b1 = Path.Combine(pick, "1");
                    txtRoot.Text = Directory.Exists(b1) ? b1 : pick;
                    UpdateTargetInfo();
                }
            }
            catch { }
        }

        private void DoScan()
        {
            try
            {
                lvFiles.Items.Clear();
                lblSummary.Text = "";

                var root = txtRoot.Text.Trim();
                if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
                {
                    MessageBox.Show("Select a valid legacy root folder (containing Old\\1, Old\\2, Old\\3).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int total = 0;
                bool scannedAnyBranchSubfolders = false;

                // Case A: user selected a root containing 1/2/3 subfolders
                for (int b = 1; b <= 3; b++)
                {
                    var sub = Path.Combine(root, b.ToString());
                    if (!Directory.Exists(sub)) continue;
                    scannedAnyBranchSubfolders = true;
                    total += ScanBranchFolder(b, sub);
                }

                // Case B: user selected a specific branch folder Old/1 (or just a folder with files)
                if (!scannedAnyBranchSubfolders)
                {
                    int assumedBranch = (int)cmbBranch.SelectedItem;
                    var name = new DirectoryInfo(root).Name;
                    if (int.TryParse(name, out var detected) && detected >= 1 && detected <= 3)
                        assumedBranch = detected;

                    total += ScanBranchFolder(assumedBranch, root);
                }

                lblSummary.Text = total > 0
                    ? $"Found {total} legacy files (.FIL/.NTX/.DAT)."
                    : "No legacy files found in the selected location.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Scan failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ScanBranchFolder(int branch, string folder)
        {
            int count = 0;
            var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".FIL", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".NTX", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".DAT", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var f in files)
            {
                var fi = new FileInfo(f);
                var type = fi.Extension.Trim('.').ToUpperInvariant();
                var item = new ListViewItem(new[]
                {
                    branch.ToString(), fi.Name, type, (fi.Length/1024).ToString("N0"), fi.FullName
                });
                lvFiles.Items.Add(item);
                count++;
            }
            return count;
        }

        private void DoImport()
        {
            try
            {
                if (lvFiles.Items.Count == 0)
                {
                    // Auto-scan if user skipped it
                    DoScan();
                    if (lvFiles.Items.Count == 0)
                    {
                        MessageBox.Show("No legacy files found to stage.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                var branch = (int)cmbBranch.SelectedItem;
                var importer = new LegacyImporter(dbManager, branch);
                var files = lvFiles.Items.Cast<ListViewItem>().Select(i => i.SubItems[4].Text).Distinct().ToList();
                var count = importer.ImportFiles(files);
                MessageBox.Show($"Imported {count} files into staging (legacy_files, legacy_raw, legacy_payloads).", "Migration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Import failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DoAccountsToCustomers()
        {
            try
            {
                var branch = (int)cmbBranch.SelectedItem;
                var importer = new LegacyImporter(dbManager, branch);
                var count = importer.ImportAccountsToCustomers();
                MessageBox.Show($"Imported/updated {count} customers from ACCOUNT.FIL.", "Migration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ACCOUNT.FIL parse failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DoCashToTransactions()
        {
            try
            {
                var branch = (int)cmbBranch.SelectedItem;
                var importer = new LegacyImporter(dbManager, branch);
                var count = importer.ImportCashToTransactions();
                MessageBox.Show($"Imported {count} transactions from CASH.FIL into day book.", "Migration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CASH.FIL import failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Preview helpers
        private void LoadPreviewForSelected()
        {
            try
            {
                if (lvFiles.SelectedItems.Count == 0) return;
                var item = lvFiles.SelectedItems[0];
                int branch = int.Parse(item.SubItems[0].Text);
                string filePath = item.SubItems[4].Text;
                var bytes = GetPayloadBytesFromDb(branch, filePath) ?? (File.Exists(filePath) ? File.ReadAllBytes(filePath) : Array.Empty<byte>());
                if (bytes.Length == 0)
                {
                    rtbText.Text = "(no content)";
                    rtbHex.Text = "(no content)";
                    dgvParsed.DataSource = null;
                    return;
                }

                // Text preview
                string text = TryDecode(bytes);
                rtbText.Text = text.Length > 8000 ? text.Substring(0, 8000) + "..." : text;

                // Hex preview (first 2048 bytes)
                int count = Math.Min(2048, bytes.Length);
                rtbHex.Text = BuildHexDump(bytes, count);

                // Parsed preview using proper dBase reader
                try
                {
                    using (var dbfReader = new DBaseReader(filePath))
                    {
                        if (Path.GetFileName(filePath).Equals("ACCOUNT.FIL", StringComparison.OrdinalIgnoreCase))
                        {
                            var rows = ParseAccountsForPreviewDbf(dbfReader).Take(200).ToList();
                            dgvParsed.DataSource = rows;
                        }
                        else if (Path.GetFileName(filePath).Equals("CASH.FIL", StringComparison.OrdinalIgnoreCase))
                        {
                            var rows = ParseCashForPreviewDbf(dbfReader).Take(200).ToList();
                            dgvParsed.DataSource = rows;
                        }
                        else
                        {
                            // Show field structure for any dBase file
                            var fieldInfo = dbfReader.Fields.Select(f => new 
                            { 
                                Field = f.Name, 
                                Type = f.Type, 
                                Length = f.Length,
                                Sample = dbfReader.ReadRecords().FirstOrDefault()?.Values.ContainsKey(f.Name) == true 
                                    ? dbfReader.ReadRecords().First().Values[f.Name]?.ToString() ?? "" 
                                    : ""
                            }).ToList();
                            dgvParsed.DataSource = fieldInfo;
                        }
                    }
                }
                catch (Exception dbfEx)
                {
                    // Fallback to text parsing if dBase reading fails
                    if (Path.GetFileName(filePath).Equals("ACCOUNT.FIL", StringComparison.OrdinalIgnoreCase))
                    {
                        var rows = ParseAccountsForPreview(text).Take(200).Select(t => new { Code = t.code, Name = t.name }).ToList();
                        dgvParsed.DataSource = rows;
                    }
                    else if (Path.GetFileName(filePath).Equals("CASH.FIL", StringComparison.OrdinalIgnoreCase))
                    {
                        var rows = ParseCashForPreview(text).Take(200).ToList();
                        dgvParsed.DataSource = rows;
                    }
                    else
                    {
                        dgvParsed.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                rtbText.Text = $"Preview error: {ex.Message}";
                rtbHex.Text = string.Empty;
                dgvParsed.DataSource = null;
            }
        }

        private byte[] GetPayloadBytesFromDb(int branch, string filePath)
        {
            try
            {
                dbManager.InitializeBranchDatabase(branch);
                using (var conn = dbManager.GetConnection(branch))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SQLite.SQLiteCommand(@"
                        SELECT lp.content FROM legacy_files lf
                        JOIN legacy_payloads lp ON lp.legacy_file_id = lf.id
                        WHERE lf.file_path = @p AND lf.branch_id = @b
                        ORDER BY lf.id DESC LIMIT 1;", conn))
                    {
                        cmd.Parameters.AddWithValue("@p", filePath);
                        cmd.Parameters.AddWithValue("@b", branch);
                        var obj = cmd.ExecuteScalar();
                        return obj as byte[];
                    }
                }
            }
            catch { return null; }
        }

        private static string TryDecode(byte[] bytes)
        {
            try { Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); } catch { }
            var encs = new[] { Encoding.GetEncoding(1252), Encoding.ASCII, Encoding.UTF8 };
            foreach (var e in encs)
            {
                try { return e.GetString(bytes); } catch { }
            }
            try { return Encoding.UTF8.GetString(bytes); } catch { return string.Empty; }
        }

        private static string BuildHexDump(byte[] bytes, int count)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < count; i += 16)
            {
                var chunk = bytes.Skip(i).Take(Math.Min(16, count - i)).ToArray();
                var hex = string.Join(" ", chunk.Select(b => b.ToString("X2")));
                var ascii = new string(chunk.Select(b => b >= 32 && b <= 126 ? (char)b : '.').ToArray());
                sb.AppendLine($"{i:X8}  {hex,-47}  {ascii}");
            }
            return sb.ToString();
        }

        private static IEnumerable<(string code, string name)> ParseAccountsForPreview(string text)
        {
            var rxNameSlashCode = new System.Text.RegularExpressions.Regex(@"(?<name>[A-Z0-9][A-Z0-9\s]{1,60})\s*/\s*(?<code>\d{2,8})", System.Text.RegularExpressions.RegexOptions.Multiline);
            foreach (System.Text.RegularExpressions.Match m in rxNameSlashCode.Matches(text))
            {
                var name = CollapseSpaces(m.Groups["name"].Value.Trim());
                var code = m.Groups["code"].Value.Trim();
                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(code))
                    yield return (code, name);
            }

            // Also support code-first lines like "5494 UHA" or "5275 00 UHA"
            var rxCodeFirst = new System.Text.RegularExpressions.Regex(@"(?m)^(?<code>\d{2,8})\s+(?<tail>[A-Z0-9 ]{2,40})$", System.Text.RegularExpressions.RegexOptions.Multiline);
            foreach (System.Text.RegularExpressions.Match m in rxCodeFirst.Matches(text))
            {
                var code = m.Groups["code"].Value.Trim();
                if (string.IsNullOrWhiteSpace(code) || code == "0000") continue;
                var tail = CollapseSpaces(m.Groups["tail"].Value.Trim());
                var name = tail.StartsWith("00 ") ? tail.Substring(3).Trim() : tail;
                if (!string.IsNullOrWhiteSpace(name)) yield return (code, name);
            }
        }

        private static IEnumerable<object> ParseCashForPreview(string text)
        {
            var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var rxAmt = new System.Text.RegularExpressions.Regex(@"(?<amt>\d+\.\d{2})(?<type>[CD])\b");
            var rxDate = new System.Text.RegularExpressions.Regex(@"\b\d{8}\b");
            var rxVeh = new System.Text.RegularExpressions.Regex(@"\b([A-Z]{1,3}-\d{1,4}|CV-\d{1,5})\b");
            var rxCode = new System.Text.RegularExpressions.Regex(@"/\s*(?<code>\d{2,8})\b");
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                var mAmt = rxAmt.Match(line);
                var mDate = rxDate.Match(line);
                var mVeh = rxVeh.Match(line);
                var mCode = rxCode.Match(line);
                yield return new
                {
                    Date = mDate.Success ? mDate.Value : "",
                    Amount = mAmt.Success ? mAmt.Groups["amt"].Value : "",
                    Type = mAmt.Success ? mAmt.Groups["type"].Value : "",
                    Vehicle = mVeh.Success ? mVeh.Value : "",
                    Code = mCode.Success ? mCode.Groups["code"].Value : "",
                    Text = line.Length > 60 ? line.Substring(0, 60) + "…" : line
                };
            }
        }

        private static IEnumerable<object> ParseAccountsForPreviewDbf(DBaseReader reader)
        {
            foreach (var record in reader.ReadRecords())
            {
                string code = "";
                string name = "";

                // Try to find customer code field
                foreach (var codeField in new[] { "CODE", "ACCT", "ACCOUNT", "ACCT_NO", "CUST_CODE", "ID" })
                {
                    if (record.Values.ContainsKey(codeField))
                    {
                        var value = record.GetString(codeField);
                        if (!string.IsNullOrWhiteSpace(value) && value != "0" && value != "0000")
                        {
                            code = value;
                            break;
                        }
                    }
                }

                // Try to find customer name field
                foreach (var nameField in new[] { "NAME", "CUSTOMER", "CUST_NAME", "ACCT_NAME", "PARTY" })
                {
                    if (record.Values.ContainsKey(nameField))
                    {
                        var value = record.GetString(nameField);
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            name = value;
                            break;
                        }
                    }
                }

                yield return new { Code = code, Name = name };
            }
        }

        private static IEnumerable<object> ParseCashForPreviewDbf(DBaseReader reader)
        {
            foreach (var record in reader.ReadRecords())
            {
                DateTime date = DateTime.MinValue;
                decimal amount = 0;
                string vehicle = "";
                string customer = "";
                string description = "";

                // Try to find date
                foreach (var dateField in new[] { "DATE", "TXN_DATE", "TRANS_DATE", "DT" })
                {
                    if (record.Values.ContainsKey(dateField))
                    {
                        date = record.GetDateTime(dateField);
                        if (date > DateTime.MinValue) break;
                    }
                }

                // Try to find amount
                foreach (var amountField in new[] { "AMOUNT", "AMT", "DEBIT", "CREDIT", "DR", "CR" })
                {
                    if (record.Values.ContainsKey(amountField))
                    {
                        amount = record.GetDecimal(amountField);
                        if (amount > 0) break;
                    }
                }

                // Try to find vehicle
                foreach (var vehicleField in new[] { "VEHICLE", "VEH_NO", "REG_NO", "REGISTRATION" })
                {
                    if (record.Values.ContainsKey(vehicleField))
                    {
                        vehicle = record.GetString(vehicleField);
                        if (!string.IsNullOrWhiteSpace(vehicle)) break;
                    }
                }

                // Try to find customer
                foreach (var customerField in new[] { "NAME", "CUST_NAME", "CUSTOMER" })
                {
                    if (record.Values.ContainsKey(customerField))
                    {
                        customer = record.GetString(customerField);
                        if (!string.IsNullOrWhiteSpace(customer)) break;
                    }
                }

                // Try to find description
                foreach (var descField in new[] { "DESCRIPTION", "DESC", "PARTICULARS", "REMARKS" })
                {
                    if (record.Values.ContainsKey(descField))
                    {
                        description = record.GetString(descField);
                        if (!string.IsNullOrWhiteSpace(description)) break;
                    }
                }

                yield return new
                {
                    Date = date == DateTime.MinValue ? "" : date.ToString("dd/MM/yyyy"),
                    Amount = amount.ToString("F2"),
                    Vehicle = vehicle,
                    Customer = customer,
                    Description = description.Length > 50 ? description.Substring(0, 50) + "…" : description
                };
            }
        }

        private static string CollapseSpaces(string s)
        {
            var sb = new StringBuilder();
            bool last = false;
            foreach (var ch in s)
            {
                if (char.IsWhiteSpace(ch)) { if (!last) { sb.Append(' '); last = true; } }
                else { sb.Append(ch); last = false; }
            }
            return sb.ToString().Trim();
        }
    }
}
