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
    /// Customer list and search form
    /// </summary>
    public partial class CustomerListForm : Form
    {
        private CustomerService customerService;
        private List<Customer> allCustomers;
        private int currentBranch;

        public CustomerListForm(CustomerService service, int branchNumber)
        {
            InitializeComponent();
            customerService = service ?? throw new ArgumentNullException(nameof(service));
            currentBranch = branchNumber;
            InitializeForm();
        }

        private void InitializeForm()
        {
            try
            {
                // Set form properties
                this.Text = $"Customer Management - Branch {currentBranch}";
                this.Size = new Size(1000, 600);
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimumSize = new Size(800, 500);

                // Load all customers
                LoadCustomers();

                Logger.Info($"Customer list form initialized for Branch {currentBranch}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize customer list form for Branch {currentBranch}", ex);
                MessageBox.Show($"Error loading customers: {ex.Message}", 
                    "FOCUS Modern Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCustomers()
        {
            try
            {
                // Show loading cursor
                Cursor = Cursors.WaitCursor;

                // Get all customers
                allCustomers = customerService.GetAllCustomers();
                
                // Update customer count
                lblCustomerCount.Text = $"Total Customers: {allCustomers.Count}";

                // Populate the grid
                PopulateCustomerGrid(allCustomers);

                Logger.Debug($"Loaded {allCustomers.Count} customers for Branch {currentBranch}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading customers for Branch {currentBranch}", ex);
                MessageBox.Show($"Error loading customers: {ex.Message}", 
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void PopulateCustomerGrid(List<Customer> customers)
        {
            try
            {
                // Clear existing data
                dgvCustomers.DataSource = null;
                dgvCustomers.Rows.Clear();

                if (customers == null || !customers.Any())
                {
                    return;
                }

                // Set up columns if not already done
                if (dgvCustomers.Columns.Count == 0)
                {
                    SetupDataGridColumns();
                }

                // Add customer data
                foreach (var customer in customers.OrderBy(c => c.Name))
                {
                    int rowIndex = dgvCustomers.Rows.Add();
                    DataGridViewRow row = dgvCustomers.Rows[rowIndex];

                    row.Cells["colCustomerCode"].Value = customer.CustomerCode;
                    row.Cells["colName"].Value = customer.Name;
                    row.Cells["colFatherName"].Value = customer.FatherName ?? "";
                    row.Cells["colPhone"].Value = customer.Phone ?? "";
                    row.Cells["colCity"].Value = customer.City ?? "";
                    row.Cells["colStatus"].Value = customer.Status ?? "Active";
                    row.Cells["colCreatedAt"].Value = customer.CreatedAt.ToString("dd/MM/yyyy");
                    
                    // Store the customer ID in the Tag property
                    row.Tag = customer.Id;

                    // Color code by status
                    if (customer.Status == "Inactive")
                    {
                        row.DefaultCellStyle.ForeColor = Color.Gray;
                    }
                }

                // Auto-resize columns
                dgvCustomers.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                Logger.Error("Error populating customer grid", ex);
                throw;
            }
        }

        private void SetupDataGridColumns()
        {
            dgvCustomers.Columns.Clear();
            dgvCustomers.AutoGenerateColumns = false;

            // Customer Code
            dgvCustomers.Columns.Add("colCustomerCode", "Customer Code");
            dgvCustomers.Columns["colCustomerCode"].Width = 120;
            dgvCustomers.Columns["colCustomerCode"].ReadOnly = true;

            // Name
            dgvCustomers.Columns.Add("colName", "Customer Name");
            dgvCustomers.Columns["colName"].Width = 200;
            dgvCustomers.Columns["colName"].ReadOnly = true;

            // Father Name
            dgvCustomers.Columns.Add("colFatherName", "Father Name");
            dgvCustomers.Columns["colFatherName"].Width = 150;
            dgvCustomers.Columns["colFatherName"].ReadOnly = true;

            // Phone
            dgvCustomers.Columns.Add("colPhone", "Phone");
            dgvCustomers.Columns["colPhone"].Width = 120;
            dgvCustomers.Columns["colPhone"].ReadOnly = true;

            // City
            dgvCustomers.Columns.Add("colCity", "City");
            dgvCustomers.Columns["colCity"].Width = 100;
            dgvCustomers.Columns["colCity"].ReadOnly = true;

            // Status
            dgvCustomers.Columns.Add("colStatus", "Status");
            dgvCustomers.Columns["colStatus"].Width = 80;
            dgvCustomers.Columns["colStatus"].ReadOnly = true;

            // Created Date
            dgvCustomers.Columns.Add("colCreatedAt", "Created");
            dgvCustomers.Columns["colCreatedAt"].Width = 100;
            dgvCustomers.Columns["colCreatedAt"].ReadOnly = true;

            // Set grid properties
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AllowUserToDeleteRows = false;
            dgvCustomers.RowHeadersVisible = false;
        }

        private void SearchCustomers()
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    // Show all customers
                    PopulateCustomerGrid(allCustomers);
                }
                else
                {
                    // Filter customers based on search term
                    var filteredCustomers = allCustomers.Where(c =>
                        (c.CustomerCode?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (c.Name?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (c.FatherName?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (c.Phone?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (c.City?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    ).ToList();

                    PopulateCustomerGrid(filteredCustomers);
                    
                    lblCustomerCount.Text = $"Showing {filteredCustomers.Count} of {allCustomers.Count} customers";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error searching customers", ex);
                MessageBox.Show($"Error searching customers: {ex.Message}", 
                    "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OpenCustomerEditForm(int? customerId = null)
        {
            try
            {
                using (var editForm = new CustomerEditForm(customerService, currentBranch, customerId))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        // Refresh the customer list
                        LoadCustomers();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error opening customer edit form", ex);
                MessageBox.Show($"Error opening customer form: {ex.Message}", 
                    "Form Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event Handlers
        private void btnNew_Click(object sender, EventArgs e)
        {
            OpenCustomerEditForm();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                int customerId = (int)dgvCustomers.SelectedRows[0].Tag;
                OpenCustomerEditForm(customerId);
            }
            else
            {
                MessageBox.Show("Please select a customer to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchCustomers();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchCustomers();
                e.Handled = true;
            }
        }

        private void dgvCustomers_DoubleClick(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                int customerId = (int)dgvCustomers.SelectedRows[0].Tag;
                OpenCustomerEditForm(customerId);
            }
        }

        private void CustomerListForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F2:
                    OpenCustomerEditForm();
                    break;
                case Keys.F5:
                    LoadCustomers();
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
                Logger.Info($"Customer list form closing for Branch {currentBranch}");
            }
            catch (Exception ex)
            {
                Logger.Error("Error during customer list form close", ex);
            }
            
            base.OnFormClosing(e);
        }
    }
}