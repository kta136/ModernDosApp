using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FocusModern.Data.Repositories;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    /// <summary>
    /// Simple Day Book viewer for transactions with date range and search
    /// </summary>
    public class DayBookForm : Form
    {
        private readonly int branchId;
        private readonly TransactionRepository transactionRepository;

        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private TextBox txtSearch;
        private Button btnLoad;
        private DataGridView dgv;
        private Label lblSummary;

        public DayBookForm(int branchNumber, TransactionRepository transactionRepo)
        {
            branchId = branchNumber;
            transactionRepository = transactionRepo;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = $"Day Book - Branch {branchId}";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(1000, 600);
            this.MinimumSize = new Size(900, 500);

            dtFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(20, 16), Width = 120, Anchor = AnchorStyles.Top | AnchorStyles.Left };
            dtTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(150, 16), Width = 120, Anchor = AnchorStyles.Top | AnchorStyles.Left };
            txtSearch = new TextBox { Location = new Point(290, 16), Width = 300, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, PlaceholderText = "Search description, reference, vehicle, customer" };
            btnLoad = new Button { Text = "Load", Location = new Point(600, 14), Width = 80, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnLoad.Click += (s, e) => LoadData();

            lblSummary = new Label { Location = new Point(700, 18), AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Right };

            dgv = new DataGridView { Location = new Point(20, 50), Size = new Size(950, 520), ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date", DataPropertyName = "TransactionDate", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Voucher", HeaderText = "Voucher", DataPropertyName = "VoucherNumber", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Vehicle", HeaderText = "Vehicle", DataPropertyName = "VehicleNumber", Width = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Customer", HeaderText = "Customer", DataPropertyName = "CustomerName", Width = 180 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Debit", HeaderText = "Debit", DataPropertyName = "DebitAmount", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Credit", HeaderText = "Credit", DataPropertyName = "CreditAmount", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Desc", HeaderText = "Description", DataPropertyName = "Description", Width = 230 });

            dtFrom.Value = DateTime.Today.AddDays(-7);
            dtTo.Value = DateTime.Today;

            this.Controls.Add(dtFrom);
            this.Controls.Add(dtTo);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnLoad);
            this.Controls.Add(lblSummary);
            this.Controls.Add(dgv);
        }

        private void LoadData()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                var from = dtFrom.Value.Date;
                var to = dtTo.Value.Date.AddDays(1).AddTicks(-1);

                var list = transactionRepository.GetByDateRange(from, to);
                var search = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(search))
                {
                    var lower = search.ToLowerInvariant();
                    list = list.Where(t =>
                        (t.Description?.ToLowerInvariant().Contains(lower) ?? false) ||
                        (t.ReferenceNumber?.ToLowerInvariant().Contains(lower) ?? false) ||
                        (t.VehicleNumber?.ToLowerInvariant().Contains(lower) ?? false) ||
                        (t.CustomerName?.ToLowerInvariant().Contains(lower) ?? false)
                    ).ToList();
                }

                dgv.DataSource = list;

                var totalDebit = list.Sum(t => t.DebitAmount);
                var totalCredit = list.Sum(t => t.CreditAmount);
                lblSummary.Text = $"Count: {list.Count} | Debit: {totalDebit:C2} | Credit: {totalCredit:C2} | Net: {(totalDebit - totalCredit):C2}";
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading day book: {ex.Message}", ex);
                MessageBox.Show($"Error loading transactions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
