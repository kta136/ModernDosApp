using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FocusModern.Data.Models;
using FocusModern.Services;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    /// <summary>
    /// Customer add/edit form
    /// </summary>
    public partial class CustomerEditForm : Form
    {
        private CustomerService customerService;
        private Customer currentCustomer;
        private int currentBranch;
        private bool isEditMode;

        public CustomerEditForm(CustomerService service, int branchNumber, int? customerId = null)
        {
            InitializeComponent();
            customerService = service ?? throw new ArgumentNullException(nameof(service));
            currentBranch = branchNumber;
            isEditMode = customerId.HasValue;
            
            if (isEditMode)
            {
                LoadCustomer(customerId.Value);
            }
            else
            {
                currentCustomer = new Customer();
            }

            InitializeForm();
        }

        private void InitializeForm()
        {
            try
            {
                // Set form properties
                this.Text = isEditMode ? $"Edit Customer - Branch {currentBranch}" : $"New Customer - Branch {currentBranch}";
                this.Size = new Size(600, 650);
                this.StartPosition = FormStartPosition.CenterParent;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;

                // Set button text
                btnSave.Text = isEditMode ? "Update Customer" : "Create Customer";

                // Load customer data if editing
                if (isEditMode && currentCustomer != null)
                {
                    PopulateForm();
                }

                // Set focus to name field
                txtName.Focus();

                Logger.Info($"Customer edit form initialized for Branch {currentBranch} (Edit: {isEditMode})");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize customer edit form for Branch {currentBranch}", ex);
                MessageBox.Show($"Error initializing form: {ex.Message}", 
                    "FOCUS Modern Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCustomer(int customerId)
        {
            try
            {
                currentCustomer = customerService.GetCustomerById(customerId);
                if (currentCustomer == null)
                {
                    throw new Exception($"Customer with ID {customerId} not found");
                }

                Logger.Debug($"Loaded customer {currentCustomer.CustomerCode} for editing");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading customer {customerId}", ex);
                MessageBox.Show($"Error loading customer: {ex.Message}", 
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                // Create new customer if load failed
                currentCustomer = new Customer();
                isEditMode = false;
            }
        }

        private void PopulateForm()
        {
            if (currentCustomer == null) return;

            try
            {
                txtCustomerCode.Text = currentCustomer.CustomerCode ?? "";
                txtName.Text = currentCustomer.Name ?? "";
                txtFatherName.Text = currentCustomer.FatherName ?? "";
                txtAddress.Text = currentCustomer.Address ?? "";
                txtCity.Text = currentCustomer.City ?? "";
                txtState.Text = currentCustomer.State ?? "";
                txtPincode.Text = currentCustomer.Pincode ?? "";
                txtPhone.Text = currentCustomer.Phone ?? "";
                txtEmail.Text = currentCustomer.Email ?? "";
                txtAadhar.Text = currentCustomer.AadharNumber ?? "";
                txtPan.Text = currentCustomer.PanNumber ?? "";
                txtOccupation.Text = currentCustomer.Occupation ?? "";
                numMonthlyIncome.Value = currentCustomer.MonthlyIncome;
                cmbStatus.Text = currentCustomer.Status ?? "Active";

                // Show creation info
                lblCreatedInfo.Text = $"Created: {currentCustomer.CreatedAt:dd/MM/yyyy HH:mm}";
                lblUpdatedInfo.Text = $"Updated: {currentCustomer.UpdatedAt:dd/MM/yyyy HH:mm}";
            }
            catch (Exception ex)
            {
                Logger.Error("Error populating customer form", ex);
                throw;
            }
        }

        private void CollectFormData()
        {
            if (currentCustomer == null) return;

            try
            {
                currentCustomer.CustomerCode = txtCustomerCode.Text.Trim().ToUpper();
                currentCustomer.Name = txtName.Text.Trim();
                currentCustomer.FatherName = txtFatherName.Text.Trim();
                currentCustomer.Address = txtAddress.Text.Trim();
                currentCustomer.City = txtCity.Text.Trim();
                currentCustomer.State = txtState.Text.Trim();
                currentCustomer.Pincode = txtPincode.Text.Trim();
                currentCustomer.Phone = txtPhone.Text.Trim();
                currentCustomer.Email = txtEmail.Text.Trim();
                currentCustomer.AadharNumber = txtAadhar.Text.Trim();
                currentCustomer.PanNumber = txtPan.Text.Trim().ToUpper();
                currentCustomer.Occupation = txtOccupation.Text.Trim();
                currentCustomer.MonthlyIncome = numMonthlyIncome.Value;
                currentCustomer.Status = cmbStatus.Text.Trim();

                // Update timestamp
                currentCustomer.Touch();
            }
            catch (Exception ex)
            {
                Logger.Error("Error collecting form data", ex);
                throw;
            }
        }

        private bool ValidateForm()
        {
            var errors = new System.Collections.Generic.List<string>();

            // Name validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errors.Add("Customer name is required");
                txtName.BackColor = Color.LightPink;
            }
            else
            {
                txtName.BackColor = SystemColors.Window;
            }

            // Customer code validation
            if (string.IsNullOrWhiteSpace(txtCustomerCode.Text))
            {
                errors.Add("Customer code is required");
                txtCustomerCode.BackColor = Color.LightPink;
            }
            else
            {
                txtCustomerCode.BackColor = SystemColors.Window;
            }

            // Phone validation
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                if (txtPhone.Text.Length < 10)
                {
                    errors.Add("Phone number must be at least 10 digits");
                    txtPhone.BackColor = Color.LightPink;
                }
                else if (!Regex.IsMatch(txtPhone.Text, @"^\d{10,15}$"))
                {
                    errors.Add("Phone number should contain only digits");
                    txtPhone.BackColor = Color.LightPink;
                }
                else
                {
                    txtPhone.BackColor = SystemColors.Window;
                }
            }
            else
            {
                txtPhone.BackColor = SystemColors.Window;
            }

            // Email validation
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    errors.Add("Invalid email format");
                    txtEmail.BackColor = Color.LightPink;
                }
                else
                {
                    txtEmail.BackColor = SystemColors.Window;
                }
            }
            else
            {
                txtEmail.BackColor = SystemColors.Window;
            }

            // Aadhar validation
            if (!string.IsNullOrWhiteSpace(txtAadhar.Text))
            {
                if (!Regex.IsMatch(txtAadhar.Text, @"^\d{12}$"))
                {
                    errors.Add("Aadhar number must be exactly 12 digits");
                    txtAadhar.BackColor = Color.LightPink;
                }
                else
                {
                    txtAadhar.BackColor = SystemColors.Window;
                }
            }
            else
            {
                txtAadhar.BackColor = SystemColors.Window;
            }

            // PAN validation
            if (!string.IsNullOrWhiteSpace(txtPan.Text))
            {
                if (!Regex.IsMatch(txtPan.Text, @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$"))
                {
                    errors.Add("Invalid PAN format (should be ABCDE1234F)");
                    txtPan.BackColor = Color.LightPink;
                }
                else
                {
                    txtPan.BackColor = SystemColors.Window;
                }
            }
            else
            {
                txtPan.BackColor = SystemColors.Window;
            }

            // Pincode validation
            if (!string.IsNullOrWhiteSpace(txtPincode.Text))
            {
                if (!Regex.IsMatch(txtPincode.Text, @"^\d{6}$"))
                {
                    errors.Add("Pincode must be exactly 6 digits");
                    txtPincode.BackColor = Color.LightPink;
                }
                else
                {
                    txtPincode.BackColor = SystemColors.Window;
                }
            }
            else
            {
                txtPincode.BackColor = SystemColors.Window;
            }

            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errors), "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SaveCustomer()
        {
            try
            {
                if (!ValidateForm()) return;

                // Collect form data
                CollectFormData();

                // Generate customer code if needed
                if (string.IsNullOrEmpty(currentCustomer.CustomerCode))
                {
                    currentCustomer.GenerateCustomerCode();
                }

                bool success;
                if (isEditMode)
                {
                    success = customerService.UpdateCustomer(currentCustomer);
                }
                else
                {
                    success = customerService.CreateCustomer(currentCustomer);
                }

                if (success)
                {
                    MessageBox.Show($"Customer {currentCustomer.CustomerCode} {(isEditMode ? "updated" : "created")} successfully!", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Failed to {(isEditMode ? "update" : "create")} customer. Please check your input and try again.", 
                        "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error saving customer for Branch {currentBranch}", ex);
                MessageBox.Show($"Error saving customer: {ex.Message}", 
                    "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateCustomerCode()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtName.Text))
                {
                    var tempCustomer = new Customer { Name = txtName.Text.Trim() };
                    tempCustomer.GenerateCustomerCode();
                    txtCustomerCode.Text = tempCustomer.CustomerCode;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error generating customer code", ex);
                MessageBox.Show($"Error generating customer code: {ex.Message}", 
                    "Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Event Handlers
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCustomer();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            GenerateCustomerCode();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            // Auto-generate customer code if in new mode and code is empty
            if (!isEditMode && string.IsNullOrWhiteSpace(txtCustomerCode.Text))
            {
                GenerateCustomerCode();
            }
        }

        private void CustomerEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10:
                    SaveCustomer();
                    break;
                case Keys.Escape:
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                Logger.Info($"Customer edit form closing for Branch {currentBranch}");
            }
            catch (Exception ex)
            {
                Logger.Error("Error during customer edit form close", ex);
            }
            
            base.OnFormClosing(e);
        }
    }
}