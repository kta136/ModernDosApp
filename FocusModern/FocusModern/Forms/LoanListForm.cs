using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FocusModern.Data;
using FocusModern.Data.Models;
using FocusModern.Data.Repositories;
using FocusModern.Services;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    public partial class LoanListForm : Form
    {
        private readonly int branchId;
        private DatabaseManager databaseManager;
        private LoanService loanService;
        private PaymentService paymentService;
        private CustomerRepository customerRepository;
        private VehicleRepository vehicleRepository;
        private LoanRepository loanRepository;
        private PaymentRepository paymentRepository;
        private TransactionRepository transactionRepository;
        private List<Loan> currentLoans;

        public LoanListForm(int branchNumber)
        {
            branchId = branchNumber;
            
            try
            {
                InitializeComponent();
                InitializeDatabaseAndServices();
                SetupForm();
                LoadLoans();

                Theme.Apply(this);

                Logger.Info($"Loan list form initialized for Branch {branchId}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing loan list form for Branch {branchId}: {ex.Message}", ex);
                MessageBox.Show($"Error initializing loan management: {ex.Message}", 
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDatabaseAndServices()
        {
            // Initialize database manager
            databaseManager = new DatabaseManager();
            databaseManager.InitializeBranchDatabase(branchId);
            
            var connection = databaseManager.GetConnection(branchId);
            
            // Initialize repositories
            customerRepository = new CustomerRepository(connection);
            vehicleRepository = new VehicleRepository(connection);
            loanRepository = new LoanRepository(connection);
            paymentRepository = new PaymentRepository(connection);
            transactionRepository = new TransactionRepository(databaseManager, branchId);
            
            // Initialize services
            loanService = new LoanService(
                loanRepository,
                paymentRepository,
                customerRepository,
                vehicleRepository,
                transactionRepository,
                databaseManager,
                branchId);
            paymentService = new PaymentService(paymentRepository, loanRepository, customerRepository, vehicleRepository, transactionRepository);
        }

        private void SetupForm()
        {
            this.Text = $"Loan Management - Branch {branchId}";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new System.Drawing.Size(1000, 600);

            // Setup status filter ComboBox
            cmbStatusFilter.Items.AddRange(new object[] { "All", "Active", "Paid", "Closed", "Overdue" });
            cmbStatusFilter.SelectedIndex = 0; // Select "All" by default

            // Setup data grid columns
            SetupDataGridColumns();
        }

        private void SetupDataGridColumns()
        {
            dgvLoans.AutoGenerateColumns = false;
            dgvLoans.AllowUserToAddRows = false;
            dgvLoans.AllowUserToDeleteRows = false;
            dgvLoans.ReadOnly = true;
            dgvLoans.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLoans.MultiSelect = false;

            // Clear existing columns
            dgvLoans.Columns.Clear();

            // Add columns
            dgvLoans.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LoanNumber",
                HeaderText = "Loan Number",
                DataPropertyName = "LoanNumber",
                Width = 120
            });

            dgvLoans.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerName",
                HeaderText = "Customer",
                DataPropertyName = "CustomerName", 
                Width = 150
            });

            dgvLoans.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VehicleNumber",
                HeaderText = "Vehicle",
                DataPropertyName = "VehicleNumber",
                Width = 120
            });

            dgvLoans.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrincipalAmount",
                HeaderText = "Loan Amount",
                DataPropertyName = "PrincipalAmount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dgvLoans.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "BalanceAmount",
                HeaderText = "Balance",
                DataPropertyName = "BalanceAmount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dgvLoans.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmiAmount",
                HeaderText = "EMI",
                DataPropertyName = "EmiAmount",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dgvLoans.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LoanDate",
                HeaderText = "Loan Date",
                DataPropertyName = "LoanDate",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            dgvLoans.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OverdueDays",
                HeaderText = "Overdue",
                DataPropertyName = "OverdueDays",
                Width = 70
            });

            dgvLoans.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                DataPropertyName = "Status",
                Width = 80
            });
        }

        private void LoadLoans()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                
                currentLoans = loanService.GetOverdueLoans().Union(loanRepository.GetActiveLoans()).ToList();
                
                // Update summary labels
                lblTotalLoans.Text = $"Total Loans: {currentLoans.Count}";
                lblActiveLoans.Text = $"Active: {currentLoans.Count(l => l.Status == "Active")}";
                lblOverdueLoans.Text = $"Overdue: {currentLoans.Count(l => l.OverdueDays > 0)}";
                
                var totalBalance = currentLoans.Where(l => l.Status == "Active").Sum(l => l.BalanceAmount);
                lblTotalBalance.Text = $"Total Balance: ₹{totalBalance:N2}";
                
                dgvLoans.DataSource = currentLoans;
                
                Logger.Info($"Loaded {currentLoans.Count} loans for Branch {branchId}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading loans: {ex.Message}", ex);
                MessageBox.Show($"Error loading loans: {ex.Message}", 
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnNewLoan_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new LoanEditForm(
                    loanService,
                    loanRepository,
                    customerRepository,
                    vehicleRepository,
                    branchId))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadLoans();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error opening new loan form: {ex.Message}", ex);
                MessageBox.Show($"Error opening new loan form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnViewLoan_Click(object sender, EventArgs e)
        {
            if (dgvLoans.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a loan to view.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var loan = dgvLoans.SelectedRows[0].DataBoundItem as Loan;
                if (loan == null)
                {
                    MessageBox.Show("Could not resolve selected loan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                using (var form = new LoanDetailsForm(loan.Id, loanService, paymentRepository))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error opening loan details: {ex.Message}", ex);
                MessageBox.Show($"Error opening loan details: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMakePayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvLoans.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a loan to make payment.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selected = dgvLoans.SelectedRows[0].DataBoundItem as Loan;
                int? loanId = selected?.Id;

                using (var entry = new PaymentEntryForm(
                    loanService,
                    loanRepository,
                    customerRepository,
                    vehicleRepository,
                    loanId))
                {
                    if (entry.ShowDialog() == DialogResult.OK)
                    {
                        LoadLoans();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error opening payment entry from loan list: {ex.Message}", ex);
                MessageBox.Show($"Error opening payment entry: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PerformSearch();
                e.Handled = true;
            }
        }

        private void PerformSearch()
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                
                if (string.IsNullOrEmpty(searchTerm))
                {
                    LoadLoans(); // Show all loans
                    return;
                }

                var filteredLoans = currentLoans.Where(l =>
                    l.LoanNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (l.Customer?.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Vehicle?.VehicleNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();

                dgvLoans.DataSource = filteredLoans;
                
                lblTotalLoans.Text = $"Found: {filteredLoans.Count}";
            }
            catch (Exception ex)
            {
                Logger.Error($"Error searching loans: {ex.Message}", ex);
                MessageBox.Show($"Error searching loans: {ex.Message}", 
                    "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadLoans();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvLoans_DoubleClick(object sender, EventArgs e)
        {
            btnViewLoan_Click(sender, e);
        }

        private void cmbStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterLoansByStatus();
        }

        private void FilterLoansByStatus()
        {
            try
            {
                string selectedStatus = cmbStatusFilter.SelectedItem?.ToString();
                
                if (string.IsNullOrEmpty(selectedStatus) || selectedStatus == "All")
                {
                    dgvLoans.DataSource = currentLoans;
                }
                else if (selectedStatus == "Overdue")
                {
                    var overdueLoans = currentLoans.Where(l => l.OverdueDays > 0).ToList();
                    dgvLoans.DataSource = overdueLoans;
                }
                else
                {
                    var filteredLoans = currentLoans.Where(l => l.Status == selectedStatus).ToList();
                    dgvLoans.DataSource = filteredLoans;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error filtering loans: {ex.Message}", ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    databaseManager?.Dispose();
                    Logger.Info($"Loan list form disposed for Branch {branchId}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error disposing loan list form: {ex.Message}", ex);
                }
            }
            base.Dispose(disposing);
        }
    }
}
