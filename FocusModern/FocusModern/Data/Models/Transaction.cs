using System;

namespace FocusModern.Data.Models
{
    /// <summary>
    /// Transaction model based on legacy CASH.FIL and day book structure
    /// </summary>
    public class Transaction
    {
        public int Id { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string VehicleNumber { get; set; }
        public string CustomerName { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public string Description { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public int? CustomerId { get; set; }
        public int? VehicleId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public Vehicle Vehicle { get; set; }

        public Transaction()
        {
            TransactionDate = DateTime.Now;
            CreatedAt = DateTime.Now;
            PaymentMethod = "cash";
            DebitAmount = 0;
            CreditAmount = 0;
            BalanceAmount = 0;
        }

        /// <summary>
        /// Transaction type based on debit/credit amounts
        /// </summary>
        public string TransactionType
        {
            get
            {
                if (DebitAmount > 0 && CreditAmount == 0)
                    return "Payment";
                else if (CreditAmount > 0 && DebitAmount == 0)
                    return "Loan/Disbursement";
                else if (DebitAmount > 0 && CreditAmount > 0)
                    return "Adjustment";
                else
                    return "Unknown";
            }
        }

        /// <summary>
        /// Net amount (Credit - Debit)
        /// </summary>
        public decimal NetAmount
        {
            get { return CreditAmount - DebitAmount; }
        }

        /// <summary>
        /// Amount for display (absolute value)
        /// </summary>
        public decimal DisplayAmount
        {
            get { return Math.Max(DebitAmount, CreditAmount); }
        }

        /// <summary>
        /// Format amount for legacy report display
        /// </summary>
        public string FormattedDebitAmount
        {
            get
            {
                return DebitAmount > 0 ? DebitAmount.ToString("N2") : "";
            }
        }

        /// <summary>
        /// Format amount for legacy report display
        /// </summary>
        public string FormattedCreditAmount
        {
            get
            {
                return CreditAmount > 0 ? CreditAmount.ToString("N2") : "";
            }
        }

        /// <summary>
        /// Generate voucher number in legacy format
        /// </summary>
        public void GenerateVoucherNumber(int sequenceNumber)
        {
            if (string.IsNullOrEmpty(VoucherNumber))
            {
                VoucherNumber = sequenceNumber.ToString();
            }
        }

        /// <summary>
        /// Validate transaction data
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(VoucherNumber) &&
                   !string.IsNullOrWhiteSpace(VehicleNumber) &&
                   (DebitAmount > 0 || CreditAmount > 0) &&
                   TransactionDate > DateTime.MinValue;
        }

        /// <summary>
        /// Format date for legacy reports (dd/MM/yyyy)
        /// </summary>
        public string FormattedDate
        {
            get { return TransactionDate.ToString("dd/MM/yyyy"); }
        }

        /// <summary>
        /// Format date for day book (dd/MM/yyyy)
        /// </summary>
        public string DayBookDate
        {
            get { return TransactionDate.ToString("dd/MM/yyyy"); }
        }

        /// <summary>
        /// Get customer/vehicle display for day book
        /// </summary>
        public string DayBookDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(CustomerName) && !string.IsNullOrEmpty(VehicleNumber))
                    return string.Format("{0} / {1}", CustomerName, VehicleNumber);
                else if (!string.IsNullOrEmpty(VehicleNumber))
                    return VehicleNumber;
                else if (!string.IsNullOrEmpty(CustomerName))
                    return CustomerName;
                else
                    return Description ?? "";
            }
        }

        public override string ToString()
        {
            return string.Format("V#{0} - {1} - {2} - {3}: {4:C}", VoucherNumber, FormattedDate, VehicleNumber, TransactionType, DisplayAmount);
        }
    }
}