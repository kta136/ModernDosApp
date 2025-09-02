using System;
using System.Collections.Generic;

namespace FocusModern.Data.Models
{
    /// <summary>
    /// Loan model representing a vehicle finance loan
    /// </summary>
    public class Loan
    {
        public int Id { get; set; }
        public string LoanNumber { get; set; }
        public int CustomerId { get; set; }
        public int VehicleId { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int LoanTermMonths { get; set; }
        public decimal EmiAmount { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal InterestPaidAmount { get; set; }
        public decimal PrincipalPaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public int OverdueDays { get; set; }
        public decimal PenaltyAmount { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int BranchId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public Vehicle Vehicle { get; set; }
        public List<Payment> Payments { get; set; }

        public Loan()
        {
            Status = "Active";
            LoanDate = DateTime.Now;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            InterestRate = 0;
            PenaltyAmount = 0;
            TotalPaidAmount = 0;
            InterestPaidAmount = 0;
            PrincipalPaidAmount = 0;
            OverdueDays = 0;
            Payments = new List<Payment>();
        }

        /// <summary>
        /// Calculate EMI amount based on principal, rate, and term
        /// </summary>
        public void CalculateEMI()
        {
            if (PrincipalAmount <= 0 || InterestRate <= 0 || LoanTermMonths <= 0)
            {
                EmiAmount = 0;
                return;
            }

            decimal monthlyRate = InterestRate / 100 / 12;
            double temp = Math.Pow((double)(1 + monthlyRate), LoanTermMonths);
            EmiAmount = PrincipalAmount * monthlyRate * (decimal)temp / (decimal)(temp - 1);
            EmiAmount = Math.Round(EmiAmount, 2);
        }

        /// <summary>
        /// Calculate maturity date based on loan date and term
        /// </summary>
        public void CalculateMaturityDate()
        {
            MaturityDate = LoanDate.AddMonths(LoanTermMonths);
        }

        /// <summary>
        /// Calculate current balance amount
        /// </summary>
        public void CalculateBalance()
        {
            BalanceAmount = PrincipalAmount - PrincipalPaidAmount;
            
            // Recalculate overdue days
            CalculateOverdueDays();
        }

        /// <summary>
        /// Calculate overdue days based on EMI schedule
        /// </summary>
        public void CalculateOverdueDays()
        {
            if (Status != "Active" || BalanceAmount <= 0)
            {
                OverdueDays = 0;
                return;
            }

            // Simple calculation: check if payment is overdue based on months elapsed
            int monthsElapsed = ((DateTime.Now.Year - LoanDate.Year) * 12) + DateTime.Now.Month - LoanDate.Month;
            decimal expectedPaidAmount = EmiAmount * monthsElapsed;
            
            if (TotalPaidAmount < expectedPaidAmount)
            {
                // Calculate days overdue
                DateTime nextDueDate = LoanDate.AddMonths(monthsElapsed + 1);
                if (DateTime.Now > nextDueDate)
                {
                    OverdueDays = (DateTime.Now - nextDueDate).Days;
                }
            }
            else
            {
                OverdueDays = 0;
            }
        }

        /// <summary>
        /// Calculate penalty based on overdue days
        /// </summary>
        public void CalculatePenalty(decimal penaltyRate = 2.0m)
        {
            if (OverdueDays <= 0 || BalanceAmount <= 0)
            {
                PenaltyAmount = 0;
                return;
            }

            // Simple penalty calculation: 2% per month on overdue amount
            decimal overdueMonths = OverdueDays / 30.0m;
            PenaltyAmount = Math.Round(BalanceAmount * (penaltyRate / 100) * overdueMonths, 2);
        }

        /// <summary>
        /// Get total outstanding amount (principal + penalty)
        /// </summary>
        public decimal TotalOutstanding
        {
            get { return BalanceAmount + PenaltyAmount; }
        }

        /// <summary>
        /// Get loan progress percentage
        /// </summary>
        public decimal ProgressPercentage
        {
            get
            {
                if (PrincipalAmount <= 0) return 0;
                return Math.Round((PrincipalPaidAmount / PrincipalAmount) * 100, 1);
            }
        }

        /// <summary>
        /// Get expected EMI amount for current month
        /// </summary>
        public decimal CurrentEMI
        {
            get
            {
                if (Status != "Active" || BalanceAmount <= 0)
                    return 0;
                
                return Math.Min(EmiAmount, BalanceAmount);
            }
        }

        /// <summary>
        /// Check if loan is overdue
        /// </summary>
        public bool IsOverdue
        {
            get { return OverdueDays > 0 && Status == "Active"; }
        }

        /// <summary>
        /// Check if loan is completed
        /// </summary>
        public bool IsCompleted
        {
            get { return BalanceAmount <= 0 || Status == "Closed"; }
        }

        /// <summary>
        /// Generate loan number in format: L-YYYYMM-XXXX
        /// </summary>
        public void GenerateLoanNumber(int sequenceNumber)
        {
            if (string.IsNullOrEmpty(LoanNumber))
            {
                LoanNumber = $"L-{LoanDate:yyyyMM}-{sequenceNumber:D4}";
            }
        }

        /// <summary>
        /// Validate loan data
        /// </summary>
        public bool IsValid()
        {
            return PrincipalAmount > 0 &&
                   InterestRate >= 0 &&
                   LoanTermMonths > 0 &&
                   CustomerId > 0 &&
                   VehicleId > 0 &&
                   !string.IsNullOrWhiteSpace(LoanNumber);
        }

        /// <summary>
        /// Update the UpdatedAt timestamp
        /// </summary>
        public void Touch()
        {
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// Get loan display summary
        /// </summary>
        public string DisplaySummary
        {
            get
            {
                return $"{LoanNumber} - ₹{PrincipalAmount:N2} @ {InterestRate}% for {LoanTermMonths} months";
            }
        }

        /// <summary>
        /// Get loan status with overdue indication
        /// </summary>
        public string StatusDisplay
        {
            get
            {
                if (IsOverdue)
                    return $"Overdue ({OverdueDays} days)";
                else if (IsCompleted)
                    return "Closed";
                else
                    return Status;
            }
        }

        public override string ToString()
        {
            return $"{LoanNumber} - {Customer?.Name} - {Vehicle?.VehicleNumber} - ₹{BalanceAmount:N2}";
        }
    }
}