using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FocusModern.Data;
using FocusModern.Data.Models;
using FocusModern.Services;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    /// <summary>
    /// Vehicle list and search form
    /// </summary>
    public partial class VehicleListForm : Form
    {
        private VehicleService vehicleService;
        private List<Vehicle> allVehicles;
        private int currentBranch;

        public VehicleListForm(VehicleService service, int branchNumber)
        {
            InitializeComponent();
            vehicleService = service ?? throw new ArgumentNullException(nameof(service));
            currentBranch = branchNumber;
            InitializeForm();
        }

        private void InitializeForm()
        {
            try
            {
                // Set form properties
                this.Text = $"Vehicle Management - Branch {currentBranch}";
                this.Size = new Size(1100, 600);
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimumSize = new Size(900, 500);

                // Load all vehicles
                LoadVehicles();

                Logger.Info($"Vehicle list form initialized for Branch {currentBranch}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize vehicle list form for Branch {currentBranch}", ex);
                MessageBox.Show($"Error loading vehicles: {ex.Message}", 
                    "FOCUS Modern Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadVehicles()
        {
            try
            {
                // Show loading cursor
                Cursor = Cursors.WaitCursor;

                // Get all vehicles
                allVehicles = vehicleService.GetAllVehicles();
                
                // Update vehicle count and statistics
                var stats = vehicleService.GetVehicleStatistics();
                lblVehicleCount.Text = $"Total Vehicles: {stats.TotalVehicles} | Active: {stats.ActiveVehicles}";
                lblFinancialSummary.Text = $"Total Loan: ₹{stats.TotalLoanAmount:N2} | Balance: ₹{stats.TotalBalanceAmount:N2}";

                // Populate the grid
                PopulateVehicleGrid(allVehicles);

                Logger.Debug($"Loaded {allVehicles.Count} vehicles for Branch {currentBranch}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading vehicles for Branch {currentBranch}", ex);
                MessageBox.Show($"Error loading vehicles: {ex.Message}", 
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void PopulateVehicleGrid(List<Vehicle> vehicles)
        {
            try
            {
                // Clear existing data
                dgvVehicles.DataSource = null;
                dgvVehicles.Rows.Clear();

                if (vehicles == null || !vehicles.Any())
                {
                    return;
                }

                // Set up columns if not already done
                if (dgvVehicles.Columns.Count == 0)
                {
                    SetupDataGridColumns();
                }

                // Add vehicle data
                foreach (var vehicle in vehicles.OrderBy(v => v.VehicleNumber))
                {
                    int rowIndex = dgvVehicles.Rows.Add();
                    DataGridViewRow row = dgvVehicles.Rows[rowIndex];

                    row.Cells["colVehicleNumber"].Value = vehicle.FormattedVehicleNumber;
                    row.Cells["colMake"].Value = vehicle.Make ?? "";
                    row.Cells["colModel"].Value = vehicle.Model ?? "";
                    row.Cells["colYear"].Value = vehicle.Year?.ToString() ?? "";
                    row.Cells["colColor"].Value = vehicle.Color ?? "";
                    row.Cells["colCustomer"].Value = vehicle.Customer?.Name ?? "No Owner";
                    row.Cells["colLoanAmount"].Value = vehicle.LoanAmount.ToString("₹#,##0.00");
                    row.Cells["colBalanceAmount"].Value = vehicle.BalanceAmount.ToString("₹#,##0.00");
                    row.Cells["colStatus"].Value = vehicle.Status ?? "Active";
                    row.Cells["colCreatedAt"].Value = vehicle.CreatedAt.ToString("dd/MM/yyyy");
                    
                    // Store the vehicle ID in the Tag property
                    row.Tag = vehicle.Id;

                    // Color code by status and balance
                    if (vehicle.Status == "Inactive")
                    {
                        row.DefaultCellStyle.ForeColor = Color.Gray;
                    }
                    else if (vehicle.BalanceAmount > 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                    }
                    else if (vehicle.BalanceAmount == 0 && vehicle.LoanAmount > 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }

                // Auto-resize columns
                dgvVehicles.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                Logger.Error("Error populating vehicle grid", ex);
                throw;
            }
        }

        private void SetupDataGridColumns()
        {
            dgvVehicles.Columns.Clear();
            dgvVehicles.AutoGenerateColumns = false;

            // Vehicle Number
            dgvVehicles.Columns.Add("colVehicleNumber", "Vehicle Number");
            dgvVehicles.Columns["colVehicleNumber"].Width = 130;
            dgvVehicles.Columns["colVehicleNumber"].ReadOnly = true;

            // Make
            dgvVehicles.Columns.Add("colMake", "Make");
            dgvVehicles.Columns["colMake"].Width = 100;
            dgvVehicles.Columns["colMake"].ReadOnly = true;

            // Model
            dgvVehicles.Columns.Add("colModel", "Model");
            dgvVehicles.Columns["colModel"].Width = 120;
            dgvVehicles.Columns["colModel"].ReadOnly = true;

            // Year
            dgvVehicles.Columns.Add("colYear", "Year");
            dgvVehicles.Columns["colYear"].Width = 60;
            dgvVehicles.Columns["colYear"].ReadOnly = true;

            // Color
            dgvVehicles.Columns.Add("colColor", "Color");
            dgvVehicles.Columns["colColor"].Width = 80;
            dgvVehicles.Columns["colColor"].ReadOnly = true;

            // Customer
            dgvVehicles.Columns.Add("colCustomer", "Owner");
            dgvVehicles.Columns["colCustomer"].Width = 150;
            dgvVehicles.Columns["colCustomer"].ReadOnly = true;

            // Loan Amount
            dgvVehicles.Columns.Add("colLoanAmount", "Loan Amount");
            dgvVehicles.Columns["colLoanAmount"].Width = 100;
            dgvVehicles.Columns["colLoanAmount"].ReadOnly = true;
            dgvVehicles.Columns["colLoanAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Balance Amount
            dgvVehicles.Columns.Add("colBalanceAmount", "Balance");
            dgvVehicles.Columns["colBalanceAmount"].Width = 100;
            dgvVehicles.Columns["colBalanceAmount"].ReadOnly = true;
            dgvVehicles.Columns["colBalanceAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Status
            dgvVehicles.Columns.Add("colStatus", "Status");
            dgvVehicles.Columns["colStatus"].Width = 80;
            dgvVehicles.Columns["colStatus"].ReadOnly = true;

            // Created Date
            dgvVehicles.Columns.Add("colCreatedAt", "Created");
            dgvVehicles.Columns["colCreatedAt"].Width = 90;
            dgvVehicles.Columns["colCreatedAt"].ReadOnly = true;

            // Set grid properties
            dgvVehicles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVehicles.MultiSelect = false;
            dgvVehicles.ReadOnly = true;
            dgvVehicles.AllowUserToAddRows = false;
            dgvVehicles.AllowUserToDeleteRows = false;
            dgvVehicles.RowHeadersVisible = false;
        }

        private void SearchVehicles()
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    // Show all vehicles
                    PopulateVehicleGrid(allVehicles);
                }
                else
                {
                    // Filter vehicles based on search term
                    var filteredVehicles = allVehicles.Where(v =>
                        (v.VehicleNumber?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (v.Make?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (v.Model?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (v.ChassisNumber?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (v.EngineNumber?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (v.Customer?.Name?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (v.Customer?.CustomerCode?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    ).ToList();

                    PopulateVehicleGrid(filteredVehicles);
                    
                    lblVehicleCount.Text = $"Showing {filteredVehicles.Count} of {allVehicles.Count} vehicles";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error searching vehicles", ex);
                MessageBox.Show($"Error searching vehicles: {ex.Message}", 
                    "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OpenVehicleEditForm(int? vehicleId = null)
        {
            try
            {
                using (var editForm = new VehicleEditForm(vehicleService, currentBranch, vehicleId))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        // Refresh the vehicle list
                        LoadVehicles();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error opening vehicle edit form", ex);
                MessageBox.Show($"Error opening vehicle form: {ex.Message}", 
                    "Form Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event Handlers
        private void btnNew_Click(object sender, EventArgs e)
        {
            OpenVehicleEditForm();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvVehicles.SelectedRows.Count > 0)
            {
                int vehicleId = (int)dgvVehicles.SelectedRows[0].Tag;
                OpenVehicleEditForm(vehicleId);
            }
            else
            {
                MessageBox.Show("Please select a vehicle to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadVehicles();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchVehicles();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchVehicles();
                e.Handled = true;
            }
        }

        private void dgvVehicles_DoubleClick(object sender, EventArgs e)
        {
            if (dgvVehicles.SelectedRows.Count > 0)
            {
                int vehicleId = (int)dgvVehicles.SelectedRows[0].Tag;
                OpenVehicleEditForm(vehicleId);
            }
        }

        private void VehicleListForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:
                    OpenVehicleEditForm();
                    break;
                case Keys.F5:
                    LoadVehicles();
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                Logger.Info($"Vehicle list form closing for Branch {currentBranch}");
            }
            catch (Exception ex)
            {
                Logger.Error("Error during vehicle list form close", ex);
            }
            
            base.OnFormClosing(e);
        }
    }
}