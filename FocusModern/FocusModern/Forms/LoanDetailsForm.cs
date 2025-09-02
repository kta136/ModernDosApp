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
    /// Read-only loan details with payment history
    /// </summary>
    public class LoanDetailsForm : Form
    {
        private readonly int loanId;
        private readonly LoanService loanService;
        private readonly PaymentRepository paymentRepository;

        private Label lblHeader;
        private Label lblCustomer;
        private Label lblVehicle;
        private Label lblPrincipal;
        private Label lblRateTerm;
        private Label lblEmi;
        private Label lblLoanDates;
        private Label lblBalance;
        private Label lblPenalty;
        private DataGridView dgvPayments;
        private Button btnClose;

        public LoanDetailsForm(int loanId, LoanService loanService, PaymentRepository paymentRepository)
        {
            this.loanId = loanId;
            this.loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
            this.paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            InitializeComponent();
            LoadDetails();
            Theme.Apply(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Loan Details";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(900, 560);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) { this.DialogResult = DialogResult.OK; this.Close(); } };

            lblHeader = new Label { Text = "Loan", Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            lblCustomer = new Label { Location = new Point(20, 50), AutoSize = true };
            lblVehicle = new Label { Location = new Point(20, 75), AutoSize = true };
            lblPrincipal = new Label { Location = new Point(20, 100), AutoSize = true };
            lblRateTerm = new Label { Location = new Point(20, 125), AutoSize = true };
            lblEmi = new Label { Location = new Point(20, 150), AutoSize = true };
            lblLoanDates = new Label { Location = new Point(20, 175), AutoSize = true };
            lblBalance = new Label { Location = new Point(20, 200), AutoSize = true, ForeColor = Color.Maroon };
            lblPenalty = new Label { Location = new Point(20, 225), AutoSize = true };

            dgvPayments = new DataGridView { Location = new Point(20, 260), Size = new Size(860, 250), ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date", DataPropertyName = "PaymentDate", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Number", HeaderText = "Number", DataPropertyName = "PaymentNumber", Width = 140 });
            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "Amount", DataPropertyName = "TotalAmount", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Breakdown", HeaderText = "Breakdown", DataPropertyName = "PaymentBreakdown", Width = 280 });
            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Method", HeaderText = "Method", DataPropertyName = "PaymentMethod", Width = 90 });
            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Voucher", HeaderText = "Voucher", DataPropertyName = "VoucherNumber", Width = 90 });

            btnClose = new Button { Text = "Close", Location = new Point(800, 520), Width = 80, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };

            Controls.Add(lblHeader);
            Controls.Add(lblCustomer);
            Controls.Add(lblVehicle);
            Controls.Add(lblPrincipal);
            Controls.Add(lblRateTerm);
            Controls.Add(lblEmi);
            Controls.Add(lblLoanDates);
            Controls.Add(lblBalance);
            Controls.Add(lblPenalty);
            Controls.Add(dgvPayments);
            Controls.Add(btnClose);

            // Grid styling
            dgvPayments.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
            dgvPayments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void LoadDetails()
        {
            try
            {
                var loan = loanService.GetLoanDetails(loanId);
                if (loan == null)
                {
                    MessageBox.Show("Loan not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                lblHeader.Text = $"Loan {loan.LoanNumber}";
                lblCustomer.Text = $"Customer: {loan.Customer?.Name} ({loan.Customer?.CustomerCode})";
                lblVehicle.Text = $"Vehicle: {loan.Vehicle?.VehicleNumber} ({loan.Vehicle?.Make} {loan.Vehicle?.Model})";
                lblPrincipal.Text = $"Principal: ?{loan.PrincipalAmount:N2}";
                lblRateTerm.Text = $"Rate/Term: {loan.InterestRate}% for {loan.LoanTermMonths} months";
                lblEmi.Text = $"EMI: ?{loan.EmiAmount:N2} | Paid: ?{loan.TotalPaidAmount:N2}";
                lblLoanDates.Text = $"Loan Date: {loan.LoanDate:dd/MM/yyyy} | Maturity: {loan.MaturityDate:dd/MM/yyyy}";
                lblBalance.Text = $"Balance: ?{loan.BalanceAmount:N2} | Status: {loan.StatusDisplay}";
                lblPenalty.Text = $"Penalty: ?{loan.PenaltyAmount:N2} | Overdue: {loan.OverdueDays} days";

                var payments = paymentRepository.GetByLoanId(loanId);
                dgvPayments.DataSource = payments.OrderByDescending(p => p.PaymentDate).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading loan details: {ex.Message}", ex);
                MessageBox.Show($"Error loading loan details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
