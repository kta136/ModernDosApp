using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FocusModern.Data;
using FocusModern.Data.Models;
using FocusModern.Data.Repositories;
using FocusModern.Services;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    /// <summary>
    /// Reports UI: Daily payments, Monthly collection, Loan statement (CSV export supported)
    /// </summary>
    public class ReportsForm : Form
    {
        private readonly int branchId;

        private DatabaseManager dbManager;
        private PaymentRepository paymentRepository;
        private LoanRepository loanRepository;
        private CustomerRepository customerRepository;
        private VehicleRepository vehicleRepository;
        private PaymentService paymentService;
        private LoanService loanService;

        private TabControl tabs;

        // Daily tab controls
        private DateTimePicker dtDaily;
        private Button btnDailyGenerate;
        private Button btnDailyExport;
        private Button btnDailyPrint;
        private Button btnDailyPdf;
        private Label lblDailySummary;
        private DataGridView dgvDaily;

        // Monthly tab controls
        private NumericUpDown numYear;
        private ComboBox cmbMonth;
        private Button btnMonthlyGenerate;
        private Button btnMonthlyExport;
        private Button btnMonthlyPrint;
        private Button btnMonthlyPdf;
        private Label lblMonthlySummary;
        private DataGridView dgvMonthly;

        // Loan Statement tab controls
        private ComboBox cmbLoan;
        private Button btnLoanGenerate;
        private Button btnLoanExport;
        private Button btnLoanPrint;
        private Button btnLoanPdf;
        private Label lblLoanSummary;
        private DataGridView dgvLoanPayments;

        public ReportsForm(int branchNumber)
        {
            branchId = branchNumber;
            InitializeComponent();
            InitializeServices();
            LoadLoanDropdown();
            Theme.Apply(this);
        }

        private void InitializeComponent()
        {
            this.Text = $"Reports - Branch {branchId}";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(1100, 700);
            this.MinimumSize = new Size(1000, 600);
            this.AutoScaleMode = AutoScaleMode.Font;

            tabs = new TabControl { Dock = DockStyle.Fill };
            var tpDaily = new TabPage("Daily Payments");
            var tpMonthly = new TabPage("Monthly Collection");
            var tpLoan = new TabPage("Loan Statement");

            // Daily tab UI
            dtDaily = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(20, 20), Width = 140 };
            btnDailyGenerate = new Button { Text = "Generate", Location = new Point(180, 18), Width = 90 };
            btnDailyExport = new Button { Text = "CSV", Location = new Point(280, 18), Width = 60 };
            btnDailyPrint = new Button { Text = "Print", Location = new Point(345, 18), Width = 60 };
            btnDailyPdf = new Button { Text = "PDF", Location = new Point(410, 18), Width = 60 };
            lblDailySummary = new Label { Location = new Point(400, 22), AutoSize = true };
            dgvDaily = new DataGridView { Location = new Point(20, 60), Size = new Size(1030, 560), ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            dgvDaily.Columns.Add(new DataGridViewTextBoxColumn { Name = "PaymentNumber", HeaderText = "Payment No.", DataPropertyName = "PaymentNumber", Width = 140 });
            dgvDaily.Columns.Add(new DataGridViewTextBoxColumn { Name = "PaymentDate", HeaderText = "Date", DataPropertyName = "PaymentDate", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvDaily.Columns.Add(new DataGridViewTextBoxColumn { Name = "Customer", HeaderText = "Customer", DataPropertyName = "CustomerName", Width = 200 });
            dgvDaily.Columns.Add(new DataGridViewTextBoxColumn { Name = "Vehicle", HeaderText = "Vehicle", DataPropertyName = "VehicleNumber", Width = 150 });
            dgvDaily.Columns.Add(new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "Amount", DataPropertyName = "TotalAmount", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvDaily.Columns.Add(new DataGridViewTextBoxColumn { Name = "Breakdown", HeaderText = "Breakdown", DataPropertyName = "PaymentBreakdown", Width = 280 });
            btnDailyGenerate.Click += (s, e) => GenerateDaily();
            btnDailyExport.Click += (s, e) => ExportGridToCsv(dgvDaily, $"daily_{dtDaily.Value:yyyyMMdd}.csv");
            btnDailyPrint.Click += (s, e) => new Utilities.ReportPrinter(dgvDaily, "Daily Payments", $"Date: {dtDaily.Value:dd/MM/yyyy}").ShowPreview(this);
            btnDailyPdf.Click += (s, e) => new Utilities.ReportPrinter(dgvDaily, "Daily Payments", $"Date: {dtDaily.Value:dd/MM/yyyy}").ExportPdf(this);
            tpDaily.Controls.AddRange(new Control[] { dtDaily, btnDailyGenerate, btnDailyExport, btnDailyPrint, btnDailyPdf, lblDailySummary, dgvDaily });

            // Monthly tab UI
            numYear = new NumericUpDown { Location = new Point(20, 20), Minimum = 2000, Maximum = 2100, Value = DateTime.Now.Year, Width = 100 };
            cmbMonth = new ComboBox { Location = new Point(130, 18), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMonth.Items.AddRange(System.Globalization.DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToArray());
            cmbMonth.SelectedIndex = DateTime.Now.Month - 1;
            btnMonthlyGenerate = new Button { Text = "Generate", Location = new Point(290, 18), Width = 90 };
            btnMonthlyExport = new Button { Text = "CSV", Location = new Point(390, 18), Width = 60 };
            btnMonthlyPrint = new Button { Text = "Print", Location = new Point(455, 18), Width = 60 };
            btnMonthlyPdf = new Button { Text = "PDF", Location = new Point(520, 18), Width = 60 };
            lblMonthlySummary = new Label { Location = new Point(600, 22), AutoSize = true };
            dgvMonthly = new DataGridView { Location = new Point(20, 60), Size = new Size(1030, 560), ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            dgvMonthly.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date", DataPropertyName = "Date", Width = 140 });
            dgvMonthly.Columns.Add(new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "Amount", DataPropertyName = "Amount", Width = 160 });
            btnMonthlyGenerate.Click += (s, e) => GenerateMonthly();
            btnMonthlyExport.Click += (s, e) => ExportGridToCsv(dgvMonthly, $"monthly_{numYear.Value}_{cmbMonth.SelectedIndex + 1:D2}.csv");
            btnMonthlyPrint.Click += (s, e) => new Utilities.ReportPrinter(dgvMonthly, "Monthly Collection", $"{cmbMonth.SelectedItem} {numYear.Value}").ShowPreview(this);
            btnMonthlyPdf.Click += (s, e) => new Utilities.ReportPrinter(dgvMonthly, "Monthly Collection", $"{cmbMonth.SelectedItem} {numYear.Value}").ExportPdf(this);
            tpMonthly.Controls.AddRange(new Control[] { numYear, cmbMonth, btnMonthlyGenerate, btnMonthlyExport, btnMonthlyPrint, btnMonthlyPdf, lblMonthlySummary, dgvMonthly });

            // Loan Statement tab UI
            cmbLoan = new ComboBox { Location = new Point(20, 18), Width = 350, DropDownStyle = ComboBoxStyle.DropDownList };
            btnLoanGenerate = new Button { Text = "Generate", Location = new Point(380, 18), Width = 90 };
            btnLoanExport = new Button { Text = "CSV", Location = new Point(480, 18), Width = 60 };
            btnLoanPrint = new Button { Text = "Print", Location = new Point(545, 18), Width = 60 };
            btnLoanPdf = new Button { Text = "PDF", Location = new Point(610, 18), Width = 60 };
            lblLoanSummary = new Label { Location = new Point(690, 22), AutoSize = true };
            dgvLoanPayments = new DataGridView { Location = new Point(20, 60), Size = new Size(1030, 560), ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            dgvLoanPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date", DataPropertyName = "PaymentDate", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvLoanPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Number", HeaderText = "Number", DataPropertyName = "PaymentNumber", Width = 140 });
            dgvLoanPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "Amount", DataPropertyName = "TotalAmount", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvLoanPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Breakdown", HeaderText = "Breakdown", DataPropertyName = "PaymentBreakdown", Width = 300 });
            dgvLoanPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Method", HeaderText = "Method", DataPropertyName = "PaymentMethod", Width = 120 });
            btnLoanGenerate.Click += (s, e) => GenerateLoanStatement();
            btnLoanExport.Click += (s, e) => ExportGridToCsv(dgvLoanPayments, $"loan_statement_{(cmbLoan.SelectedItem as LoanItem)?.Loan?.LoanNumber}.csv");
            btnLoanPrint.Click += (s, e) => new Utilities.ReportPrinter(dgvLoanPayments, "Loan Statement", (cmbLoan.SelectedItem as LoanItem)?.Loan?.LoanNumber ?? string.Empty).ShowPreview(this);
            btnLoanPdf.Click += (s, e) => new Utilities.ReportPrinter(dgvLoanPayments, "Loan Statement", (cmbLoan.SelectedItem as LoanItem)?.Loan?.LoanNumber ?? string.Empty).ExportPdf(this);
            tpLoan.Controls.AddRange(new Control[] { cmbLoan, btnLoanGenerate, btnLoanExport, btnLoanPrint, btnLoanPdf, lblLoanSummary, dgvLoanPayments });

            tabs.TabPages.Add(tpDaily);
            tabs.TabPages.Add(tpMonthly);
            tabs.TabPages.Add(tpLoan);

            this.Controls.Add(tabs);

            // Grids styling
            foreach (var grid in new[] { dgvDaily, dgvMonthly, dgvLoanPayments })
            {
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        private void InitializeServices()
        {
            try
            {
                dbManager = new DatabaseManager();
                dbManager.InitializeBranchDatabase(branchId);
                var conn = dbManager.GetConnection(branchId);

                paymentRepository = new PaymentRepository(conn);
                loanRepository = new LoanRepository(conn);
                customerRepository = new CustomerRepository(conn);
                vehicleRepository = new VehicleRepository(conn);

                var txnRepo = new TransactionRepository(dbManager, branchId);
                paymentService = new PaymentService(paymentRepository, loanRepository, customerRepository, vehicleRepository, txnRepo);
                loanService = new LoanService(loanRepository, paymentRepository, customerRepository, vehicleRepository, txnRepo, dbManager, branchId);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing reports services: {ex.Message}", ex);
                MessageBox.Show($"Error initializing reports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLoanDropdown()
        {
            try
            {
                var loans = loanRepository.GetAll().OrderByDescending(l => l.LoanDate).ToList();
                cmbLoan.Items.Clear();
                foreach (var l in loans) cmbLoan.Items.Add(new LoanItem(l));
                if (cmbLoan.Items.Count > 0) cmbLoan.SelectedIndex = 0;

                // Prefill default reports
                dtDaily.Value = DateTime.Today;
                GenerateDaily();
                GenerateMonthly();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading loans for report: {ex.Message}", ex);
            }
        }

        private void GenerateDaily()
        {
            try
            {
                var report = paymentService.GenerateDailyReport(dtDaily.Value.Date);
                if (report == null)
                {
                    dgvDaily.DataSource = new List<Payment>();
                    lblDailySummary.Text = "No data";
                    return;
                }

                dgvDaily.DataSource = report.Payments.Select(p => new
                {
                    p.PaymentNumber,
                    p.PaymentDate,
                    CustomerName = p.Customer?.Name ?? string.Empty,
                    VehicleNumber = p.Vehicle?.VehicleNumber ?? string.Empty,
                    p.TotalAmount,
                    PaymentBreakdown = p.PaymentBreakdown
                }).ToList();

                lblDailySummary.Text = $"Date: {report.ReportDate:dd/MM/yyyy} | Count: {report.TotalCount} | Total: {report.TotalAmount:C2} | Cash: {report.CashAmount:C2} | Cheque: {report.ChequeAmount:C2} | Online: {report.OnlineAmount:C2}";
            }
            catch (Exception ex)
            {
                Logger.Error($"Error generating daily report: {ex.Message}", ex);
                MessageBox.Show($"Error generating daily report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateMonthly()
        {
            try
            {
                var year = (int)numYear.Value;
                var month = cmbMonth.SelectedIndex + 1;
                var report = paymentService.GenerateMonthlyCollection(year, month);
                if (report == null)
                {
                    dgvMonthly.DataSource = new List<object>();
                    lblMonthlySummary.Text = "No data";
                    return;
                }

                var rows = report.DailyCollections
                    .OrderBy(kv => kv.Key)
                    .Select(kv => new { Date = kv.Key.ToString("dd/MM/yyyy"), Amount = kv.Value.ToString("C2") })
                    .ToList();

                dgvMonthly.DataSource = rows;
                lblMonthlySummary.Text = $"{report.MonthName}: Count {report.TotalCount} | Total {report.TotalCollection:C2} | Avg/Day {report.AverageDaily:C2} | Highest { (report.HighestDay.Key == default ? "-" : report.HighestDay.Key.ToString("dd/MM")) } { (report.HighestDay.Value) }";
            }
            catch (Exception ex)
            {
                Logger.Error($"Error generating monthly collection: {ex.Message}", ex);
                MessageBox.Show($"Error generating monthly collection: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateLoanStatement()
        {
            try
            {
                var loan = (cmbLoan.SelectedItem as LoanItem)?.Loan;
                if (loan == null)
                {
                    MessageBox.Show("Select a loan.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var statement = loanService.GenerateLoanStatement(loan.Id);
                if (statement == null)
                {
                    dgvLoanPayments.DataSource = new List<object>();
                    lblLoanSummary.Text = "No data";
                    return;
                }

                dgvLoanPayments.DataSource = statement.Payments.Select(p => new
                {
                    p.PaymentDate,
                    p.PaymentNumber,
                    p.TotalAmount,
                    PaymentBreakdown = p.PaymentBreakdown,
                    p.PaymentMethod
                }).ToList();

                lblLoanSummary.Text = $"Loan {statement.Loan.LoanNumber} | Outstanding {statement.OutstandingBalance:C2} | Total Paid {statement.TotalPaid:C2} | Next Due {statement.NextDueDate:dd/MM/yyyy}";
            }
            catch (Exception ex)
            {
                Logger.Error($"Error generating loan statement: {ex.Message}", ex);
                MessageBox.Show($"Error generating loan statement: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportGridToCsv(DataGridView grid, string defaultFileName)
        {
            try
            {
                if (grid.DataSource == null)
                {
                    MessageBox.Show("No data to export.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var sfd = new SaveFileDialog { Filter = "CSV Files (*.csv)|*.csv", FileName = defaultFileName })
                {
                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    var sb = new StringBuilder();
                    // Headers
                    var headers = grid.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText.Replace(",", " "));
                    sb.AppendLine(string.Join(",", headers));
                    // Rows
                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        if (row.IsNewRow) continue;
                        var cells = row.Cells.Cast<DataGridViewCell>().Select(c => (c.Value?.ToString() ?? string.Empty).Replace(",", " "));
                        sb.AppendLine(string.Join(",", cells));
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Exported successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error exporting CSV: {ex.Message}", ex);
                MessageBox.Show($"Error exporting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbManager?.Dispose();
            }
            base.Dispose(disposing);
        }

        private sealed class LoanItem
        {
            public Loan Loan { get; }
            public LoanItem(Loan loan) { Loan = loan; }
            public override string ToString()
            {
                try { return $"{Loan.LoanNumber} - {Loan.Customer?.Name} - {Loan.Vehicle?.VehicleNumber}"; } catch { return "Loan"; }
            }
        }
    }
}
