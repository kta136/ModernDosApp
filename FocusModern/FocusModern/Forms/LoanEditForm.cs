using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FocusModern.Data.Models;
using FocusModern.Data.Repositories;
using FocusModern.Services;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    /// <summary>
    /// Create/Edit Loan form
    /// </summary>
    public class LoanEditForm : Form
    {
        private readonly LoanService loanService;
        private readonly LoanRepository loanRepository;
        private readonly CustomerRepository customerRepository;
        private readonly VehicleRepository vehicleRepository;
        private readonly int branchId;

        private readonly bool isEditMode;
        private Loan currentLoan;

        private ComboBox cmbCustomer;
        private ComboBox cmbVehicle;
        private NumericUpDown numPrincipal;
        private NumericUpDown numInterest;
        private NumericUpDown numTerm;
        private DateTimePicker dtLoanDate;
        private TextBox txtRemarks;
        private Label lblEMI;
        private Label lblMaturity;
        private Button btnSave;
        private Button btnCancel;

        public LoanEditForm(
            LoanService loanService,
            LoanRepository loanRepository,
            CustomerRepository customerRepository,
            VehicleRepository vehicleRepository,
            int branchId,
            int? loanId = null)
        {
            this.loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
            this.loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
            this.customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            this.vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            this.branchId = branchId;

            isEditMode = loanId.HasValue;
            if (isEditMode)
            {
                currentLoan = loanRepository.GetById(loanId.Value) ?? new Loan();
            }
            else
            {
                currentLoan = new Loan { BranchId = branchId };
            }

            InitializeComponent();
            LoadDropdowns();
            PopulateFormIfEditing();
            RecalculateDerived();
        }

        private void InitializeComponent()
        {
            this.Text = isEditMode ? "Edit Loan" : "New Loan";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ClientSize = new Size(640, 360);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) { this.DialogResult = DialogResult.Cancel; this.Close(); } };

            int x1 = 20, x2 = 140, w = 460, y = 20, dy = 30;

            Controls.Add(new Label { Text = "Customer:", Location = new Point(x1, y), AutoSize = true });
            cmbCustomer = new ComboBox { Location = new Point(x2, y - 4), Width = w, DropDownStyle = ComboBoxStyle.DropDownList };
            y += dy;

            Controls.Add(new Label { Text = "Vehicle:", Location = new Point(x1, y), AutoSize = true });
            cmbVehicle = new ComboBox { Location = new Point(x2, y - 4), Width = w, DropDownStyle = ComboBoxStyle.DropDownList };
            y += dy;

            Controls.Add(new Label { Text = "Principal:", Location = new Point(x1, y), AutoSize = true });
            numPrincipal = new NumericUpDown { Location = new Point(x2, y - 4), Width = 120, DecimalPlaces = 2, Maximum = 100000000, Minimum = 0.01m, Increment = 1000 };
            y += dy;

            Controls.Add(new Label { Text = "Interest %:", Location = new Point(x1, y), AutoSize = true });
            numInterest = new NumericUpDown { Location = new Point(x2, y - 4), Width = 80, DecimalPlaces = 2, Maximum = 100, Minimum = 0, Increment = 0.25m };
            y += dy;

            Controls.Add(new Label { Text = "Term (months):", Location = new Point(x1, y), AutoSize = true });
            numTerm = new NumericUpDown { Location = new Point(x2, y - 4), Width = 80, Maximum = 1200, Minimum = 1 };
            y += dy;

            Controls.Add(new Label { Text = "Loan Date:", Location = new Point(x1, y), AutoSize = true });
            dtLoanDate = new DateTimePicker { Location = new Point(x2, y - 4), Width = 140, Format = DateTimePickerFormat.Short };
            y += dy;

            Controls.Add(new Label { Text = "EMI:", Location = new Point(x1, y), AutoSize = true });
            lblEMI = new Label { Location = new Point(x2, y), AutoSize = true, ForeColor = Color.Maroon };
            y += dy;

            Controls.Add(new Label { Text = "Maturity:", Location = new Point(x1, y), AutoSize = true });
            lblMaturity = new Label { Location = new Point(x2, y), AutoSize = true };
            y += dy;

            Controls.Add(new Label { Text = "Remarks:", Location = new Point(x1, y), AutoSize = true });
            txtRemarks = new TextBox { Location = new Point(x2, y - 4), Width = w, Height = 50, Multiline = true };
            y += 60;

            btnSave = new Button { Text = isEditMode ? "Update" : "Create", Location = new Point(420, y), Width = 100 };
            btnCancel = new Button { Text = "Cancel", Location = new Point(530, y), Width = 80 };

            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            btnSave.Click += (s, e) => SaveLoan();
            numPrincipal.ValueChanged += (_, __) => RecalculateDerived();
            numInterest.ValueChanged += (_, __) => RecalculateDerived();
            numTerm.ValueChanged += (_, __) => RecalculateDerived();
            dtLoanDate.ValueChanged += (_, __) => RecalculateDerived();

            Controls.Add(cmbCustomer);
            Controls.Add(cmbVehicle);
            Controls.Add(numPrincipal);
            Controls.Add(numInterest);
            Controls.Add(numTerm);
            Controls.Add(dtLoanDate);
            Controls.Add(lblEMI);
            Controls.Add(lblMaturity);
            Controls.Add(txtRemarks);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadDropdowns()
        {
            try
            {
                var customers = customerRepository.GetAll().OrderBy(c => c.Name).ToList();
                cmbCustomer.Items.Clear();
                foreach (var c in customers) cmbCustomer.Items.Add(c);
                cmbCustomer.DisplayMember = "DisplayName";
                cmbCustomer.ValueMember = "Id";

                var vehicles = vehicleRepository.GetAll().OrderBy(v => v.VehicleNumber).ToList();
                cmbVehicle.Items.Clear();
                foreach (var v in vehicles) cmbVehicle.Items.Add(v);
                cmbVehicle.DisplayMember = "DisplayInfo";
                cmbVehicle.ValueMember = "Id";

                if (!isEditMode)
                {
                    if (cmbCustomer.Items.Count > 0) cmbCustomer.SelectedIndex = 0;
                    if (cmbVehicle.Items.Count > 0) cmbVehicle.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading dropdowns for loan edit: {ex.Message}", ex);
            }
        }

        private void PopulateFormIfEditing()
        {
            if (!isEditMode || currentLoan == null) return;
            try
            {
                // Select customer/vehicle
                for (int i = 0; i < cmbCustomer.Items.Count; i++)
                    if ((cmbCustomer.Items[i] as Customer)?.Id == currentLoan.CustomerId) { cmbCustomer.SelectedIndex = i; break; }
                for (int i = 0; i < cmbVehicle.Items.Count; i++)
                    if ((cmbVehicle.Items[i] as Vehicle)?.Id == currentLoan.VehicleId) { cmbVehicle.SelectedIndex = i; break; }

                numPrincipal.Value = currentLoan.PrincipalAmount;
                numInterest.Value = currentLoan.InterestRate;
                numTerm.Value = currentLoan.LoanTermMonths;
                dtLoanDate.Value = currentLoan.LoanDate;
                txtRemarks.Text = currentLoan.Remarks ?? "";
            }
            catch (Exception ex)
            {
                Logger.Error($"Error populating loan form: {ex.Message}", ex);
            }
        }

        private void RecalculateDerived()
        {
            try
            {
                var tmp = new Loan
                {
                    PrincipalAmount = numPrincipal.Value,
                    InterestRate = numInterest.Value,
                    LoanTermMonths = (int)numTerm.Value,
                    LoanDate = dtLoanDate.Value
                };
                tmp.CalculateEMI();
                tmp.CalculateMaturityDate();
                lblEMI.Text = tmp.EmiAmount.ToString("C2");
                lblMaturity.Text = tmp.MaturityDate.ToString("dd/MM/yyyy");
            }
            catch { }
        }

        private void SaveLoan()
        {
            try
            {
                if (cmbCustomer.SelectedItem is not Customer selCustomer)
                {
                    MessageBox.Show("Select a customer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cmbVehicle.SelectedItem is not Vehicle selVehicle)
                {
                    MessageBox.Show("Select a vehicle.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (numPrincipal.Value <= 0 || numTerm.Value <= 0)
                {
                    MessageBox.Show("Enter valid principal and term.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!isEditMode)
                {
                    currentLoan = new Loan();
                }

                currentLoan.CustomerId = selCustomer.Id;
                currentLoan.VehicleId = selVehicle.Id;
                currentLoan.PrincipalAmount = numPrincipal.Value;
                currentLoan.InterestRate = numInterest.Value;
                currentLoan.LoanTermMonths = (int)numTerm.Value;
                currentLoan.LoanDate = dtLoanDate.Value.Date;
                currentLoan.Remarks = txtRemarks.Text.Trim();
                currentLoan.BranchId = branchId;

                bool ok;
                if (isEditMode)
                {
                    // Regenerate EMI/maturity, keep loan number
                    currentLoan.CalculateEMI();
                    currentLoan.CalculateMaturityDate();
                    currentLoan.BalanceAmount = Math.Max(0, currentLoan.PrincipalAmount - currentLoan.PrincipalPaidAmount);
                    ok = loanService.UpdateLoan(currentLoan);
                }
                else
                {
                    ok = loanService.CreateLoan(currentLoan);
                }

                if (ok)
                {
                    MessageBox.Show(isEditMode ? "Loan updated." : "Loan created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to save loan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error saving loan: {ex.Message}", ex);
                MessageBox.Show($"Error saving loan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
