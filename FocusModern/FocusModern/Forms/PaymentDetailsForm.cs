using System;
using System.Drawing;
using System.Windows.Forms;
using FocusModern.Data.Models;
using FocusModern.Services;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    /// <summary>
    /// Shows payment details and allows cancellation (reversal)
    /// </summary>
    public class PaymentDetailsForm : Form
    {
        private readonly int paymentId;
        private readonly PaymentService paymentService;

        private Label lblHeader;
        private Label lblNumber;
        private Label lblDate;
        private Label lblCustomer;
        private Label lblVehicle;
        private Label lblAmount;
        private Label lblBreakdown;
        private Label lblMethod;
        private Label lblVoucher;
        private TextBox txtDescription;
        private Button btnCancelPayment;
        private Button btnClose;

        private Payment payment;

        public PaymentDetailsForm(int paymentId, PaymentService service)
        {
            this.paymentId = paymentId;
            this.paymentService = service ?? throw new ArgumentNullException(nameof(service));
            InitializeComponent();
            LoadPayment();
        }

        private void InitializeComponent()
        {
            this.Text = "Payment Details";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ClientSize = new Size(560, 360);

            lblHeader = new Label { Text = "Payment", Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            lblNumber = new Label { Text = "Number:", Location = new Point(20, 50), AutoSize = true };
            lblDate = new Label { Text = "Date:", Location = new Point(20, 75), AutoSize = true };
            lblCustomer = new Label { Text = "Customer:", Location = new Point(20, 100), AutoSize = true };
            lblVehicle = new Label { Text = "Vehicle:", Location = new Point(20, 125), AutoSize = true };
            lblAmount = new Label { Text = "Amount:", Location = new Point(20, 150), AutoSize = true, ForeColor = Color.Maroon };
            lblBreakdown = new Label { Text = "Breakdown:", Location = new Point(20, 175), AutoSize = true };
            lblMethod = new Label { Text = "Method:", Location = new Point(20, 200), AutoSize = true };
            lblVoucher = new Label { Text = "Voucher:", Location = new Point(20, 225), AutoSize = true };

            var lblDesc = new Label { Text = "Description:", Location = new Point(20, 255), AutoSize = true };
            txtDescription = new TextBox { Location = new Point(110, 252), Width = 420, ReadOnly = true };

            btnCancelPayment = new Button { Text = "Cancel Payment", Location = new Point(300, 300), Width = 120 };
            btnClose = new Button { Text = "Close", Location = new Point(430, 300), Width = 100 };
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            btnCancelPayment.Click += (s, e) => CancelPayment();

            this.Controls.AddRange(new Control[]
            {
                lblHeader, lblNumber, lblDate, lblCustomer, lblVehicle, lblAmount, lblBreakdown,
                lblMethod, lblVoucher, lblDesc, txtDescription, btnCancelPayment, btnClose
            });
        }

        private void LoadPayment()
        {
            try
            {
                payment = paymentService.GetPaymentDetails(paymentId);
                if (payment == null)
                {
                    MessageBox.Show("Payment not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                lblHeader.Text = $"Payment {payment.PaymentNumber}";
                lblNumber.Text = $"Number: {payment.PaymentNumber}";
                lblDate.Text = $"Date: {payment.PaymentDate:dd/MM/yyyy}";
                lblCustomer.Text = $"Customer: {payment.Customer?.Name ?? ""}";
                lblVehicle.Text = $"Vehicle: {payment.Vehicle?.VehicleNumber ?? ""}";
                lblAmount.Text = $"Amount: ?{payment.TotalAmount:N2}";
                lblBreakdown.Text = $"Breakdown: {payment.PaymentBreakdown}";
                lblMethod.Text = $"Method: {payment.PaymentMethod} ({payment.PaymentType})";
                lblVoucher.Text = $"Voucher: {payment.VoucherNumber}";
                txtDescription.Text = payment.Description ?? "";

                // Disable cancel if already a reversal (negative) or zero amount
                btnCancelPayment.Enabled = payment.TotalAmount > 0;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading payment details: {ex.Message}", ex);
                MessageBox.Show($"Error loading payment details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelPayment()
        {
            try
            {
                if (payment == null || payment.TotalAmount <= 0)
                    return;

                var confirm = MessageBox.Show(
                    "Cancel (reverse) this payment? A reversal entry will be created.",
                    "Confirm Cancellation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                string reason = Prompt("Enter reason for cancellation:", "Payment Cancellation");
                if (reason == null) return; // user cancelled

                if (paymentService.CancelPayment(payment.Id, reason))
                {
                    MessageBox.Show("Payment cancelled successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to cancel payment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error cancelling payment: {ex.Message}", ex);
                MessageBox.Show($"Error cancelling payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string Prompt(string text, string caption)
        {
            var form = new Form()
            {
                Width = 420,
                Height = 160,
                Text = caption,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var lbl = new Label() { Left = 10, Top = 10, Text = text, AutoSize = true };
            var txt = new TextBox() { Left = 10, Top = 40, Width = 380 };
            var btnOk = new Button() { Text = "OK", Left = 230, Width = 75, Top = 80, DialogResult = DialogResult.OK };
            var btnCancel = new Button() { Text = "Cancel", Left = 315, Width = 75, Top = 80, DialogResult = DialogResult.Cancel };

            form.Controls.Add(lbl);
            form.Controls.Add(txt);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : null;
        }
    }
}

