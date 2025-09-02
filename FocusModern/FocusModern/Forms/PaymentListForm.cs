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
    public partial class PaymentListForm : Form
    {
        private readonly int branchId;
        private DatabaseManager databaseManager;
        private PaymentService paymentService;
        private LoanService loanService;
        private CustomerRepository customerRepository;
        private VehicleRepository vehicleRepository;
        private LoanRepository loanRepository;
        private PaymentRepository paymentRepository;
        private TransactionRepository transactionRepository;
        private List<Payment> currentPayments;

        public PaymentListForm(int branchNumber)
        {
            branchId = branchNumber;
            
            try
            {
                InitializeComponent();
                InitializeDatabaseAndServices();
                SetupForm();
                LoadPayments();
                
                Logger.Info($"Payment list form initialized for Branch {branchId}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing payment list form for Branch {branchId}: {ex.Message}", ex);
                MessageBox.Show($"Error initializing payment management: {ex.Message}", 
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
            loanService = new LoanService(loanRepository, paymentRepository, customerRepository, vehicleRepository);
            paymentService = new PaymentService(paymentRepository, loanRepository, customerRepository, vehicleRepository, transactionRepository);
        }

        private void SetupForm()
        {
            this.Text = $"Payment Management - Branch {branchId}";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new System.Drawing.Size(1000, 600);

            // Setup payment method filter ComboBox
            cmbPaymentMethodFilter.Items.AddRange(new object[] { "All", "Cash", "Cheque", "Online", "UPI" });
            cmbPaymentMethodFilter.SelectedIndex = 0; // Select "All" by default

            // Setup data grid columns
            SetupDataGridColumns();
        }

        private void SetupDataGridColumns()
        {
            dgvPayments.AutoGenerateColumns = false;
            dgvPayments.AllowUserToAddRows = false;
            dgvPayments.AllowUserToDeleteRows = false;
            dgvPayments.ReadOnly = true;
            dgvPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPayments.MultiSelect = false;

            // Clear existing columns
            dgvPayments.Columns.Clear();

            // Add columns
            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaymentNumber",
                HeaderText = "Payment No.",
                DataPropertyName = "PaymentNumber",
                Width = 120
            });

            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaymentDate",
                HeaderText = "Date",
                DataPropertyName = "PaymentDate",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerName",
                HeaderText = "Customer",
                DataPropertyName = "CustomerName", 
                Width = 150
            });

            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VehicleNumber",
                HeaderText = "Vehicle",
                DataPropertyName = "VehicleNumber",
                Width = 120
            });

            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAmount",
                HeaderText = "Amount",
                DataPropertyName = "TotalAmount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaymentMethod",
                HeaderText = "Method",
                DataPropertyName = "PaymentMethod",
                Width = 80
            });

            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaymentType",
                HeaderText = "Type",
                DataPropertyName = "PaymentType",
                Width = 120
            });

            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ReceivedBy",
                HeaderText = "Received By",
                DataPropertyName = "ReceivedBy",
                Width = 100
            });

            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "Description",
                DataPropertyName = "Description",
                Width = 200
            });
        }

        private void LoadPayments()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                
                // Load payments for the current month by default
                var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var toDate = DateTime.Now;
                
                currentPayments = paymentService.GetPaymentsByDateRange(fromDate, toDate);
                
                // Update summary labels
                lblTotalPayments.Text = $"Total Payments: {currentPayments.Count}";
                var totalAmount = currentPayments.Sum(p => p.TotalAmount);
                lblTotalAmount.Text = $"Total Amount: ₹{totalAmount:N2}";
                
                var todayPayments = currentPayments.Where(p => p.PaymentDate.Date == DateTime.Today).ToList();
                lblTodayPayments.Text = $"Today: {todayPayments.Count}";
                lblTodayAmount.Text = $"Today's Amount: ₹{todayPayments.Sum(p => p.TotalAmount):N2}";
                
                dgvPayments.DataSource = currentPayments;
                
                Logger.Info($"Loaded {currentPayments.Count} payments for Branch {branchId}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading payments: {ex.Message}", ex);
                MessageBox.Show($"Error loading payments: {ex.Message}", 
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnNewPayment_Click(object sender, EventArgs e)
        {
            MessageBox.Show("New Payment form will be implemented next", "Coming Soon", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnViewPayment_Click(object sender, EventArgs e)
        {
            if (dgvPayments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a payment to view.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Payment Details form will be implemented next", "Coming Soon", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    LoadPayments(); // Show all payments
                    return;
                }

                var filteredPayments = currentPayments.Where(p =>
                    p.PaymentNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (p.Customer?.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Vehicle?.VehicleNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();

                dgvPayments.DataSource = filteredPayments;
                
                lblTotalPayments.Text = $"Found: {filteredPayments.Count}";
                var totalAmount = filteredPayments.Sum(p => p.TotalAmount);
                lblTotalAmount.Text = $"Amount: ₹{totalAmount:N2}";
            }
            catch (Exception ex)
            {
                Logger.Error($"Error searching payments: {ex.Message}", ex);
                MessageBox.Show($"Error searching payments: {ex.Message}", 
                    "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPayments();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvPayments_DoubleClick(object sender, EventArgs e)
        {
            btnViewPayment_Click(sender, e);
        }

        private void cmbPaymentMethodFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterPaymentsByMethod();
        }

        private void FilterPaymentsByMethod()
        {
            try
            {
                string selectedMethod = cmbPaymentMethodFilter.SelectedItem?.ToString();
                
                if (string.IsNullOrEmpty(selectedMethod) || selectedMethod == "All")
                {
                    dgvPayments.DataSource = currentPayments;
                }
                else
                {
                    var filteredPayments = currentPayments.Where(p => p.PaymentMethod == selectedMethod).ToList();
                    dgvPayments.DataSource = filteredPayments;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error filtering payments: {ex.Message}", ex);
            }
        }

        private void btnDateRange_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dateRangeForm = new DateRangeSelectionForm())
                {
                    if (dateRangeForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadPaymentsByDateRange(dateRangeForm.FromDate, dateRangeForm.ToDate);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Date range selection will be available soon. Currently showing this month's payments.", 
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadPaymentsByDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                
                currentPayments = paymentService.GetPaymentsByDateRange(fromDate, toDate);
                dgvPayments.DataSource = currentPayments;
                
                // Update summary
                lblTotalPayments.Text = $"Total Payments: {currentPayments.Count} ({fromDate:dd/MM} - {toDate:dd/MM})";
                var totalAmount = currentPayments.Sum(p => p.TotalAmount);
                lblTotalAmount.Text = $"Total Amount: ₹{totalAmount:N2}";
                
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading payments by date range: {ex.Message}", ex);
                MessageBox.Show($"Error loading payments: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    databaseManager?.Dispose();
                    Logger.Info($"Payment list form disposed for Branch {branchId}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error disposing payment list form: {ex.Message}", ex);
                }
            }
            base.Dispose(disposing);
        }
    }

    // Simple placeholder for date range selection form
    public class DateRangeSelectionForm : Form
    {
        public DateTime FromDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime ToDate { get; set; } = DateTime.Today;
        
        public DateRangeSelectionForm()
        {
            // This would be a proper date selection form
            this.DialogResult = DialogResult.Cancel;
        }
    }
}