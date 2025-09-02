using System;
using System.Collections.Generic;

namespace FocusModern.Data.Models
{
    /// <summary>
    /// Payment model for loan payments and transactions
    /// </summary>
    public class Payment
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public int LoanId { get; set; }
        public int CustomerId { get; set; }
        public int VehicleId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal PenaltyAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
        public string ReceivedBy { get; set; }
        public int BranchId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Loan Loan { get; set; }
        public Customer Customer { get; set; }
        public Vehicle Vehicle { get; set; }

        public Payment()
        {
            PaymentDate = DateTime.Now;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            PaymentMethod = "Cash";
            TotalAmount = 0;
            PrincipalAmount = 0;
            InterestAmount = 0;
            PenaltyAmount = 0;
            ReceivedBy = Environment.UserName;
        }

        /// <summary>
        /// Calculate payment allocation based on loan details
        /// </summary>
        public void AllocatePayment(Loan loan, decimal paymentAmount)
        {
            TotalAmount = paymentAmount;
            
            // Simple allocation: penalty first, then interest, then principal
            decimal remainingAmount = paymentAmount;

            // Allocate penalty first
            if (loan.PenaltyAmount > 0 && remainingAmount > 0)
            {
                PenaltyAmount = Math.Min(remainingAmount, loan.PenaltyAmount);
                remainingAmount -= PenaltyAmount;
            }

            // Calculate interest for current EMI
            decimal currentInterest = CalculateCurrentInterest(loan);
            if (currentInterest > 0 && remainingAmount > 0)
            {
                InterestAmount = Math.Min(remainingAmount, currentInterest);
                remainingAmount -= InterestAmount;
            }

            // Rest goes to principal
            if (remainingAmount > 0)
            {
                PrincipalAmount = Math.Min(remainingAmount, loan.BalanceAmount);
            }
        }

        /// <summary>
        /// Calculate interest component for current payment
        /// </summary>
        private decimal CalculateCurrentInterest(Loan loan)
        {
            if (loan.InterestRate <= 0 || loan.BalanceAmount <= 0)
                return 0;

            // Simple interest calculation for monthly payment
            decimal monthlyRate = loan.InterestRate / 100 / 12;
            return Math.Round(loan.BalanceAmount * monthlyRate, 2);
        }

        /// <summary>
        /// Generate payment number in format: P-YYYYMM-XXXX
        /// </summary>
        public void GeneratePaymentNumber(int sequenceNumber)
        {
            if (string.IsNullOrEmpty(PaymentNumber))
            {
                PaymentNumber = $"P-{PaymentDate:yyyyMM}-{sequenceNumber:D4}";
            }
        }

        /// <summary>
        /// Generate voucher number for transaction records
        /// </summary>
        public void GenerateVoucherNumber(int voucherSequence)
        {
            if (string.IsNullOrEmpty(VoucherNumber))
            {
                VoucherNumber = voucherSequence.ToString();
            }
        }

        /// <summary>
        /// Validate payment data
        /// </summary>
        public bool IsValid()
        {
            return TotalAmount > 0 &&
                   LoanId > 0 &&
                   CustomerId > 0 &&
                   VehicleId > 0 &&
                   PaymentDate >= DateTime.Today.AddDays(-30) && // Not more than 30 days back
                   !string.IsNullOrWhiteSpace(PaymentNumber) &&
                   !string.IsNullOrWhiteSpace(PaymentMethod);
        }

        /// <summary>
        /// Check if payment allocation is balanced
        /// </summary>
        public bool IsAllocated
        {
            get
            {
                decimal allocatedTotal = PrincipalAmount + InterestAmount + PenaltyAmount;
                return Math.Abs(TotalAmount - allocatedTotal) < 0.01m;
            }
        }

        /// <summary>
        /// Get payment type based on amounts
        /// </summary>
        public string PaymentType
        {
            get
            {
                if (PenaltyAmount > 0)
                    return "Penalty Payment";
                else if (InterestAmount > 0 && PrincipalAmount > 0)
                    return "EMI Payment";
                else if (PrincipalAmount > 0)
                    return "Principal Payment";
                else if (InterestAmount > 0)
                    return "Interest Payment";
                else
                    return "Payment";
            }
        }

        /// <summary>
        /// Get formatted payment breakdown
        /// </summary>
        public string PaymentBreakdown
        {
            get
            {
                var parts = new List<string>();
                if (PrincipalAmount > 0) parts.Add($"Principal: ₹{PrincipalAmount:N2}");
                if (InterestAmount > 0) parts.Add($"Interest: ₹{InterestAmount:N2}");
                if (PenaltyAmount > 0) parts.Add($"Penalty: ₹{PenaltyAmount:N2}");
                return string.Join(", ", parts);
            }
        }

        /// <summary>
        /// Update the UpdatedAt timestamp
        /// </summary>
        public void Touch()
        {
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// Get payment display summary
        /// </summary>
        public string DisplaySummary
        {
            get
            {
                return $"{PaymentNumber} - {PaymentDate:dd/MM/yyyy} - ₹{TotalAmount:N2} ({PaymentMethod})";
            }
        }

        /// <summary>
        /// Convert to transaction record for day book
        /// </summary>
        public Transaction ToTransaction()
        {
            return new Transaction
            {
                VoucherNumber = VoucherNumber,
                TransactionDate = PaymentDate,
                VehicleNumber = Vehicle?.VehicleNumber ?? "",
                CustomerName = Customer?.Name ?? "",
                DebitAmount = TotalAmount, // Payment is debit (money received)
                CreditAmount = 0,
                Description = $"Payment received - {PaymentType}",
                PaymentMethod = PaymentMethod,
                ReferenceNumber = PaymentNumber,
                CustomerId = CustomerId,
                VehicleId = VehicleId,
                CreatedAt = CreatedAt
            };
        }

        public override string ToString()
        {
            return $"{PaymentNumber} - {PaymentDate:dd/MM/yyyy} - ₹{TotalAmount:N2} - {PaymentMethod}";
        }
    }
}