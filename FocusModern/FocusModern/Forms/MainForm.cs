using System;
using System.Drawing;
using System.Windows.Forms;
using FocusModern.Data;
using FocusModern.Data.Repositories;
using FocusModern.Services;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    /// <summary>
    /// Main application form for selected branch
    /// </summary>
    public partial class MainForm : Form
    {
        private DatabaseManager databaseManager;
        private CustomerService customerService;
        private VehicleService vehicleService;
        private int currentBranch;

        public MainForm(int branchNumber)
        {
            InitializeComponent();
            currentBranch = branchNumber;
            InitializeForm();
        }

        private void InitializeForm()
        {
            try
            {
                // Initialize database for current branch
                databaseManager = new DatabaseManager();
                databaseManager.InitializeBranchDatabase(currentBranch);

                // Initialize services
                customerService = new CustomerService(databaseManager, currentBranch);
                Logger.Debug(string.Format("CustomerService initialized for Branch {0}", currentBranch));
                
                vehicleService = new VehicleService(databaseManager, currentBranch);
                Logger.Debug(string.Format("VehicleService initialized for Branch {0}", currentBranch));

                // Update form title
                this.Text = string.Format("FOCUS Modern - Branch {0}", currentBranch);
                lblCurrentBranch.Text = string.Format("Branch {0}", currentBranch);

                // Set branch-specific UI colors
                SetBranchColors();

                // Load dashboard data
                LoadDashboardData();

                // Apply modern theme
                Theme.Apply(this);

                Logger.Info(string.Format("Main form initialized for Branch {0}", currentBranch));
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Failed to initialize main form for Branch {0}", currentBranch), ex);
                MessageBox.Show(string.Format("Error initializing Branch {0}: {1}", currentBranch, ex.Message), 
                    "FOCUS Modern Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetBranchColors()
        {
            Color branchColor = currentBranch switch
            {
                1 => Color.LightGreen,    // Active branch
                2 => Color.LightBlue,     // Historical
                3 => Color.LightBlue,     // Historical
                _ => Color.LightGray
            };

            pnlBranchHeader.BackColor = branchColor;
        }

        private void LoadDashboardData()
        {
            try
            {
                // Load actual customer count
                int customerCount = customerService.GetCustomerCount();
                lblTotalCustomers.Text = string.Format("Total Customers: {0}", customerCount);
                
                // Load actual vehicle count and statistics
                var vehicleStats = vehicleService.GetVehicleStatistics();
                lblActiveLoans.Text = string.Format("Vehicles: {0} | Active: {1}", vehicleStats.TotalVehicles, vehicleStats.ActiveVehicles);
                
                // Show financial summary
                lblTodaysPayments.Text = string.Format("Total Loans: ₹{0:N0} | Balance: ₹{1:N0}", vehicleStats.TotalLoanAmount, vehicleStats.TotalBalanceAmount);
                
                // Update status
                lblStatus.Text = string.Format("Branch {0} | Connected", currentBranch);
                lblStatus.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                Logger.Error("Error loading dashboard data", ex);
                lblStatus.Text = string.Format("Branch {0} | Error", currentBranch);
                lblStatus.ForeColor = Color.Red;
            }
        }

        // Menu and button event handlers
        private void btnCustomers_Click(object sender, EventArgs e)
        {
            OpenCustomerManagement();
        }

        private void OpenCustomerManagement()
        {
            try
            {
                using (var customerListForm = new CustomerListForm(customerService, currentBranch))
                {
                    customerListForm.ShowDialog();
                    
                    // Refresh dashboard data after customer form closes
                    LoadDashboardData();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error opening customer management for Branch {currentBranch}", ex);
                MessageBox.Show($"Error opening customer management: {ex.Message}", 
                    "FOCUS Modern Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVehicles_Click(object sender, EventArgs e)
        {
            OpenVehicleManagement();
        }

        private void OpenVehicleManagement()
        {
            try
            {
                // Check if services are initialized
                if (vehicleService == null)
                {
                    MessageBox.Show("Vehicle service not initialized. Reinitializing services...", 
                        "Service Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    
                    // Try to reinitialize services
                    try
                    {
                        vehicleService = new VehicleService(databaseManager, currentBranch);
                    }
                    catch (Exception initEx)
                    {
                        Logger.Error($"Failed to initialize vehicle service for Branch {currentBranch}", initEx);
                        MessageBox.Show($"Failed to initialize vehicle service: {initEx.Message}", 
                            "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                using (var vehicleListForm = new VehicleListForm(vehicleService, currentBranch))
                {
                    vehicleListForm.ShowDialog();
                    
                    // Refresh dashboard data after vehicle form closes
                    LoadDashboardData();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error opening vehicle management for Branch {currentBranch}", ex);
                MessageBox.Show($"Error opening vehicle management: {ex.Message}", 
                    "FOCUS Modern Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoans_Click(object sender, EventArgs e)
        {
            OpenLoanManagement();
        }

        private void OpenLoanManagement()
        {
            try
            {
                using (var loanListForm = new LoanListForm(currentBranch))
                {
                    loanListForm.ShowDialog();
                    
                    // Refresh dashboard data after loan form closes
                    LoadDashboardData();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error opening loan management for Branch {currentBranch}", ex);
                MessageBox.Show($"Error opening loan management: {ex.Message}", 
                    "FOCUS Modern Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPayments_Click(object sender, EventArgs e)
        {
            try
            {
                using (var paymentsForm = new PaymentListForm(currentBranch))
                {
                    paymentsForm.ShowDialog();
                    // Refresh dashboard data after payments form closes
                    LoadDashboardData();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error opening payment management for Branch {currentBranch}", ex);
                MessageBox.Show($"Error opening payment management: {ex.Message}",
                    "FOCUS Modern Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            try
            {
                using (var reports = new ReportsForm(currentBranch))
                {
                    reports.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error opening reports: {ex.Message}", ex);
                MessageBox.Show($"Error opening reports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSwitchBranch_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Info("User requested branch switch");
                
                // Close current form and show branch selection again
                using (var branchSelectionForm = new BranchSelectionForm())
                {
                    this.Hide();
                    
                    if (branchSelectionForm.ShowDialog() == DialogResult.OK)
                    {
                        int newBranch = branchSelectionForm.SelectedBranch;
                        
                        if (newBranch != currentBranch)
                        {
                            // Open new main form for selected branch
                            var newMainForm = new MainForm(newBranch);
                            newMainForm.Show();
                            this.Close();
                        }
                        else
                        {
                            this.Show();
                        }
                    }
                    else
                    {
                        this.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error switching branches", ex);
                MessageBox.Show($"Error switching branches: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                using (var backup = new BackupForm(currentBranch))
                {
                    backup.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error opening backup: {ex.Message}", ex);
                MessageBox.Show($"Error opening backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                using (var settings = new SettingsForm())
                {
                    if (settings.ShowDialog() == DialogResult.OK)
                    {
                        // After settings change, we may need to refresh database paths/colors etc.
                        LoadDashboardData();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error opening settings: {ex.Message}", ex);
                MessageBox.Show($"Error opening settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        // Keyboard shortcuts
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F2:
                    btnCustomers_Click(null, null);
                    return true;
                case Keys.F3:
                    btnVehicles_Click(null, null);
                    return true;
                case Keys.F4:
                    btnLoans_Click(null, null);
                    return true;
                case Keys.F5:
                    btnPayments_Click(null, null);
                    return true;
                case Keys.F6:
                    // Temporarily use F6 to open Day Book until Reports UI exists
                    try
                    {
                        var txnRepo = new TransactionRepository(databaseManager, currentBranch);
                        using (var dayBook = new DayBookForm(currentBranch, txnRepo))
                        {
                            dayBook.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error opening day book: {ex.Message}", ex);
                        MessageBox.Show($"Error opening day book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return true;
                case Keys.Control | Keys.B:
                    btnSwitchBranch_Click(null, null);
                    return true;
                case Keys.F1:
                    MessageBox.Show("F2: Customers\nF3: Vehicles\nF4: Loans\nF5: Payments\nF6: Reports\nCtrl+B: Switch Branch", 
                        "Keyboard Shortcuts", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                case Keys.Escape:
                    if (MessageBox.Show("Exit FOCUS Modern?", "Confirm Exit", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                    return true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Logger.Info($"Main form closing for Branch {currentBranch}");
                databaseManager?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error("Error closing main form", ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                databaseManager?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
