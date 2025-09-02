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
    /// Minimal payment entry form to record a payment against a loan
    /// </summary>
    public class PaymentEntryForm : Form
    {
        private readonly LoanService loanService;
        private readonly LoanRepository loanRepository;
        private readonly CustomerRepository customerRepository;
        private readonly VehicleRepository vehicleRepository;

        private ComboBox cmbLoan;
        private Label lblCustomer;
        private Label lblVehicle;
        private Label lblBalance;
        private NumericUpDown numAmount;
        private ComboBox cmbMethod;
        private TextBox txtDescription;
        private Button btnSave;
        private Button btnCancel;

        private Loan selectedLoan;

        public PaymentEntryForm(
            LoanService loanService,
            LoanRepository loanRepo,
            CustomerRepository customerRepo,
            VehicleRepository vehicleRepo,
            int? preselectedLoanId = null)
        {
            this.loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
            this.loanRepository = loanRepo ?? throw new ArgumentNullException(nameof(loanRepo));
            this.customerRepository = customerRepo ?? throw new ArgumentNullException(nameof(customerRepo));
            this.vehicleRepository = vehicleRepo ?? throw new ArgumentNullException(nameof(vehicleRepo));

            InitializeComponent();
            LoadLoans(preselectedLoanId);
        }

        private void InitializeComponent()
        {
            this.Text = "New Payment";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ClientSize = new Size(520, 320);

            var lblLoan = new Label { Text = "Loan:", Location = new Point(20, 20), AutoSize = true };
            cmbLoan = new ComboBox { Location = new Point(120, 16), Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbLoan.SelectedIndexChanged += (s, e) => UpdateLoanInfo();

            var lblCustomerCaption = new Label { Text = "Customer:", Location = new Point(20, 60), AutoSize = true };
            lblCustomer = new Label { Text = "-", Location = new Point(120, 60), AutoSize = true };

            var lblVehicleCaption = new Label { Text = "Vehicle:", Location = new Point(20, 90), AutoSize = true };
            lblVehicle = new Label { Text = "-", Location = new Point(120, 90), AutoSize = true };

            var lblBalanceCaption = new Label { Text = "Balance:", Location = new Point(20, 120), AutoSize = true };
            lblBalance = new Label { Text = "?0.00", Location = new Point(120, 120), AutoSize = true, ForeColor = Color.Maroon };

            var lblAmount = new Label { Text = "Amount:", Location = new Point(20, 160), AutoSize = true };
            numAmount = new NumericUpDown { Location = new Point(120, 156), Width = 150, DecimalPlaces = 2, Maximum = 100000000, Minimum = 0.01m, Increment = 100 };

            var lblMethod = new Label { Text = "Method:", Location = new Point(20, 190), AutoSize = true };
            cmbMethod = new ComboBox { Location = new Point(120, 186), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMethod.Items.AddRange(new object[] { "Cash", "Cheque", "Online", "UPI" });
            cmbMethod.SelectedIndex = 0;

            var lblDesc = new Label { Text = "Description:", Location = new Point(20, 220), AutoSize = true };
            txtDescription = new TextBox { Location = new Point(120, 216), Width = 360 };

            btnSave = new Button { Text = "Save", Location = new Point(320, 260), Width = 75 };
            btnCancel = new Button { Text = "Cancel", Location = new Point(405, 260), Width = 75 };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            btnSave.Click += (s, e) => SavePayment();

            this.Controls.Add(lblLoan);
            this.Controls.Add(cmbLoan);
            this.Controls.Add(lblCustomerCaption);
            this.Controls.Add(lblCustomer);
            this.Controls.Add(lblVehicleCaption);
            this.Controls.Add(lblVehicle);
            this.Controls.Add(lblBalanceCaption);
            this.Controls.Add(lblBalance);
            this.Controls.Add(lblAmount);
            this.Controls.Add(numAmount);
            this.Controls.Add(lblMethod);
            this.Controls.Add(cmbMethod);
            this.Controls.Add(lblDesc);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void LoadLoans(int? preselectedLoanId)
        {
            try
            {
                var loans = loanRepository.GetActiveLoans();
                var items = loans
                    .OrderBy(l => l.LoanNumber)
                    .Select(l => new LoanListItem(l))
                    .ToList();

                cmbLoan.Items.Clear();
                foreach (var item in items) cmbLoan.Items.Add(item);
                if (cmbLoan.Items.Count > 0)
                {
                    if (preselectedLoanId.HasValue)
                    {
                        var idx = items.FindIndex(i => i.Loan.Id == preselectedLoanId.Value);
                        cmbLoan.SelectedIndex = idx >= 0 ? idx : 0;
                    }
                    else
                    {
                        cmbLoan.SelectedIndex = 0;
                    }
                }
                UpdateLoanInfo();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading loans for payment entry: {ex.Message}", ex);
                MessageBox.Show($"Error loading loans: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateLoanInfo()
        {
            try
            {
                selectedLoan = (cmbLoan.SelectedItem as LoanListItem)?.Loan;
                if (selectedLoan == null)
                {
                    lblCustomer.Text = "-";
                    lblVehicle.Text = "-";
                    lblBalance.Text = "?0.00";
                    return;
                }

                var customer = customerRepository.GetById(selectedLoan.CustomerId);
                var vehicle = vehicleRepository.GetById(selectedLoan.VehicleId);

                lblCustomer.Text = customer?.Name ?? "-";
                lblVehicle.Text = vehicle?.VehicleNumber ?? "-";
                lblBalance.Text = $"?{selectedLoan.BalanceAmount:N2}";

                // Suggest default amount as EMI or remaining balance
                decimal defaultAmount = selectedLoan.EmiAmount > 0 ? selectedLoan.EmiAmount : selectedLoan.BalanceAmount;
                if (defaultAmount <= 0) defaultAmount = 0.01m;
                numAmount.Value = Math.Min(numAmount.Maximum, defaultAmount);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error updating loan info: {ex.Message}");
            }
        }

        private void SavePayment()
        {
            try
            {
                if (selectedLoan == null)
                {
                    MessageBox.Show("Please select a loan.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var amount = numAmount.Value;
                if (amount <= 0)
                {
                    MessageBox.Show("Amount must be positive.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var method = cmbMethod.SelectedItem?.ToString() ?? "Cash";
                var description = txtDescription.Text.Trim();

                if (loanService.ProcessPayment(selectedLoan.Id, amount, method, description))
                {
                    MessageBox.Show("Payment recorded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to record payment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error saving payment: {ex.Message}", ex);
                MessageBox.Show($"Error saving payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private sealed class LoanListItem
        {
            public Loan Loan { get; }
            public LoanListItem(Loan loan) { Loan = loan; }
            public override string ToString()
            {
                try
                {
                    return $"{Loan.LoanNumber} | Bal:?{Loan.BalanceAmount:N2}";
                }
                catch { return "Loan"; }
            }
        }
    }
}

