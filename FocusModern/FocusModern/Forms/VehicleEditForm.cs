using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FocusModern.Data.Models;
using FocusModern.Services;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    /// <summary>
    /// Vehicle add/edit form
    /// </summary>
    public partial class VehicleEditForm : Form
    {
        private VehicleService vehicleService;
        private Vehicle currentVehicle;
        private int currentBranch;
        private bool isEditMode;

        public VehicleEditForm(VehicleService service, int branchNumber, int? vehicleId = null)
        {
            InitializeComponent();
            vehicleService = service ?? throw new ArgumentNullException(nameof(service));
            currentBranch = branchNumber;
            isEditMode = vehicleId.HasValue;
            
            if (isEditMode)
            {
                LoadVehicle(vehicleId.Value);
            }
            else
            {
                currentVehicle = new Vehicle();
            }

            InitializeForm();
        }

        private void InitializeForm()
        {
            try
            {
                // Set form properties
                this.Text = isEditMode ? $"Edit Vehicle - Branch {currentBranch}" : $"New Vehicle - Branch {currentBranch}";
                this.Size = new Size(600, 550);
                this.StartPosition = FormStartPosition.CenterParent;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;

                // Set button text
                btnSave.Text = isEditMode ? "Update Vehicle" : "Create Vehicle";

                // Load customer dropdown
                LoadCustomerDropdown();

                // Load vehicle data if editing
                if (isEditMode && currentVehicle != null)
                {
                    PopulateForm();
                }

                // Set focus to vehicle number field
                txtVehicleNumber.Focus();

                Logger.Info($"Vehicle edit form initialized for Branch {currentBranch} (Edit: {isEditMode})");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize vehicle edit form for Branch {currentBranch}", ex);
                MessageBox.Show($"Error initializing form: {ex.Message}", 
                    "FOCUS Modern Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadVehicle(int vehicleId)
        {
            try
            {
                currentVehicle = vehicleService.GetVehicleById(vehicleId);
                if (currentVehicle == null)
                {
                    throw new Exception($"Vehicle with ID {vehicleId} not found");
                }

                Logger.Debug($"Loaded vehicle {currentVehicle.VehicleNumber} for editing");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading vehicle {vehicleId}", ex);
                MessageBox.Show($"Error loading vehicle: {ex.Message}", 
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                // Create new vehicle if load failed
                currentVehicle = new Vehicle();
                isEditMode = false;
            }
        }

        private void LoadCustomerDropdown()
        {
            try
            {
                var customers = vehicleService.GetAllCustomers();
                
                cmbCustomer.Items.Clear();
                cmbCustomer.Items.Add("-- No Owner --");
                
                foreach (var customer in customers.OrderBy(c => c.Name))
                {
                    cmbCustomer.Items.Add(customer);
                }
                
                cmbCustomer.DisplayMember = "DisplayName";
                cmbCustomer.ValueMember = "Id";
                cmbCustomer.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Logger.Error("Error loading customers for dropdown", ex);
                throw;
            }
        }

        private void PopulateForm()
        {
            if (currentVehicle == null) return;

            try
            {
                txtVehicleNumber.Text = currentVehicle.VehicleNumber ?? "";
                txtMake.Text = currentVehicle.Make ?? "";
                txtModel.Text = currentVehicle.Model ?? "";
                numYear.Value = currentVehicle.Year ?? DateTime.Now.Year;
                txtColor.Text = currentVehicle.Color ?? "";
                txtChassisNumber.Text = currentVehicle.ChassisNumber ?? "";
                txtEngineNumber.Text = currentVehicle.EngineNumber ?? "";
                numLoanAmount.Value = currentVehicle.LoanAmount;
                numPaidAmount.Value = currentVehicle.PaidAmount;
                cmbStatus.Text = currentVehicle.Status ?? "Active";

                // Set customer selection
                if (currentVehicle.CustomerId.HasValue)
                {
                    for (int i = 1; i < cmbCustomer.Items.Count; i++)
                    {
                        if (cmbCustomer.Items[i] is Customer customer && customer.Id == currentVehicle.CustomerId.Value)
                        {
                            cmbCustomer.SelectedIndex = i;
                            break;
                        }
                    }
                }

                // Show creation info
                lblCreatedInfo.Text = $"Created: {currentVehicle.CreatedAt:dd/MM/yyyy HH:mm}";
                lblUpdatedInfo.Text = $"Updated: {currentVehicle.UpdatedAt:dd/MM/yyyy HH:mm}";

                // Calculate and show balance
                CalculateBalance();
            }
            catch (Exception ex)
            {
                Logger.Error("Error populating vehicle form", ex);
                throw;
            }
        }

        private void CollectFormData()
        {
            if (currentVehicle == null) return;

            try
            {
                currentVehicle.VehicleNumber = txtVehicleNumber.Text.Trim().ToUpper();
                currentVehicle.Make = txtMake.Text.Trim();
                currentVehicle.Model = txtModel.Text.Trim();
                currentVehicle.Year = (int)numYear.Value;
                currentVehicle.Color = txtColor.Text.Trim();
                currentVehicle.ChassisNumber = txtChassisNumber.Text.Trim().ToUpper();
                currentVehicle.EngineNumber = txtEngineNumber.Text.Trim().ToUpper();
                currentVehicle.LoanAmount = numLoanAmount.Value;
                currentVehicle.PaidAmount = numPaidAmount.Value;
                currentVehicle.Status = cmbStatus.Text.Trim();

                // Set customer
                if (cmbCustomer.SelectedIndex > 0 && cmbCustomer.SelectedItem is Customer selectedCustomer)
                {
                    currentVehicle.CustomerId = selectedCustomer.Id;
                }
                else
                {
                    currentVehicle.CustomerId = null;
                }

                // Update timestamp
                currentVehicle.Touch();

                // Calculate balance
                currentVehicle.CalculateBalance();
            }
            catch (Exception ex)
            {
                Logger.Error("Error collecting form data", ex);
                throw;
            }
        }

        private void CalculateBalance()
        {
            try
            {
                decimal loanAmount = numLoanAmount.Value;
                decimal paidAmount = numPaidAmount.Value;
                decimal balance = loanAmount - paidAmount;
                
                lblBalanceAmount.Text = $"Balance: ₹{balance:N2}";
                
                // Color code the balance
                if (balance > 0)
                {
                    lblBalanceAmount.ForeColor = Color.Red;
                }
                else if (balance == 0 && loanAmount > 0)
                {
                    lblBalanceAmount.ForeColor = Color.Green;
                }
                else
                {
                    lblBalanceAmount.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error calculating balance", ex);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtVehicleNumber.Text))
            {
                MessageBox.Show("Vehicle number is required", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtVehicleNumber.Focus();
                return false;
            }

            if (numPaidAmount.Value > numLoanAmount.Value)
            {
                MessageBox.Show("Paid amount cannot exceed loan amount", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numPaidAmount.Focus();
                return false;
            }

            return true;
        }

        private void SaveVehicle()
        {
            try
            {
                if (!ValidateForm()) return;

                // Collect form data
                CollectFormData();

                bool success;
                if (isEditMode)
                {
                    success = vehicleService.UpdateVehicle(currentVehicle);
                }
                else
                {
                    success = vehicleService.CreateVehicle(currentVehicle);
                }

                if (success)
                {
                    MessageBox.Show($"Vehicle {currentVehicle.VehicleNumber} {(isEditMode ? "updated" : "created")} successfully!", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Failed to {(isEditMode ? "update" : "create")} vehicle. Please check your input and try again.", 
                        "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error saving vehicle for Branch {currentBranch}", ex);
                MessageBox.Show($"Error saving vehicle: {ex.Message}", 
                    "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event Handlers
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveVehicle();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void numLoanAmount_ValueChanged(object sender, EventArgs e)
        {
            CalculateBalance();
        }

        private void numPaidAmount_ValueChanged(object sender, EventArgs e)
        {
            CalculateBalance();
        }

        private void VehicleEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10:
                    SaveVehicle();
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
                Logger.Info($"Vehicle edit form closing for Branch {currentBranch}");
            }
            catch (Exception ex)
            {
                Logger.Error("Error during vehicle edit form close", ex);
            }
            
            base.OnFormClosing(e);
        }
    }
}