using System;
using System.Collections.Generic;
using System.Linq;
using FocusModern.Data.Models;
using FocusModern.Data.Repositories;
using FocusModern.Utilities;

namespace FocusModern.Services
{
    public class PaymentService
    {
        private readonly PaymentRepository paymentRepository;
        private readonly LoanRepository loanRepository;
        private readonly CustomerRepository customerRepository;
        private readonly VehicleRepository vehicleRepository;
        private readonly TransactionRepository transactionRepository;

        public PaymentService(
            PaymentRepository paymentRepo,
            LoanRepository loanRepo,
            CustomerRepository customerRepo,
            VehicleRepository vehicleRepo,
            TransactionRepository transactionRepo)
        {
            paymentRepository = paymentRepo;
            loanRepository = loanRepo;
            customerRepository = customerRepo;
            vehicleRepository = vehicleRepo;
            transactionRepository = transactionRepo;
        }

        /// <summary>
        /// Get all payments with pagination
        /// </summary>
        public List<Payment> GetAllPayments(int page = 1, int pageSize = 100)
        {
            try
            {
                return paymentRepository.GetAll()
                    .OrderByDescending(p => p.PaymentDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting payments: {ex.Message}");
                return new List<Payment>();
            }
        }

        /// <summary>
        /// Get payments by date range
        /// </summary>
        public List<Payment> GetPaymentsByDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return paymentRepository.GetByDateRange(fromDate, toDate);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting payments by date range: {ex.Message}");
                return new List<Payment>();
            }
        }

        /// <summary>
        /// Get payments for a specific customer
        /// </summary>
        public List<Payment> GetCustomerPayments(int customerId)
        {
            try
            {
                var customerLoans = loanRepository.GetAll()
                    .Where(l => l.CustomerId == customerId)
                    .Select(l => l.Id)
                    .ToList();

                var payments = new List<Payment>();
                foreach (var loanId in customerLoans)
                {
                    payments.AddRange(paymentRepository.GetByLoanId(loanId));
                }

                return payments.OrderByDescending(p => p.PaymentDate).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting customer payments: {ex.Message}");
                return new List<Payment>();
            }
        }

        /// <summary>
        /// Get payment details with related entities
        /// </summary>
        public Payment GetPaymentDetails(int paymentId)
        {
            try
            {
                var payment = paymentRepository.GetById(paymentId);
                if (payment != null)
                {
                    // Load related data
                    payment.Loan = loanRepository.GetById(payment.LoanId);
                    payment.Customer = customerRepository.GetById(payment.CustomerId);
                    payment.Vehicle = vehicleRepository.GetById(payment.VehicleId);
                }
                return payment;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting payment details: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Search payments by various criteria
        /// </summary>
        public List<Payment> SearchPayments(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return GetAllPayments();

                var allPayments = paymentRepository.GetAll();

                return allPayments.Where(p =>
                    p.PaymentNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.VoucherNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.ReferenceNumber?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    p.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true
                ).OrderByDescending(p => p.PaymentDate).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error searching payments: {ex.Message}");
                return new List<Payment>();
            }
        }

        /// <summary>
        /// Validate payment data
        /// </summary>
        public bool ValidatePayment(Payment payment, out string errorMessage)
        {
            errorMessage = "";

            try
            {
                if (payment.TotalAmount <= 0)
                {
                    errorMessage = "Payment amount must be positive";
                    return false;
                }

                if (payment.LoanId <= 0)
                {
                    errorMessage = "Loan must be selected";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(payment.PaymentMethod))
                {
                    errorMessage = "Payment method is required";
                    return false;
                }

                if (payment.PaymentDate > DateTime.Now.AddDays(1))
                {
                    errorMessage = "Payment date cannot be in the future";
                    return false;
                }

                if (payment.PaymentDate < DateTime.Now.AddYears(-2))
                {
                    errorMessage = "Payment date is too old";
                    return false;
                }

                // Check if loan exists and is active
                var loan = loanRepository.GetById(payment.LoanId);
                if (loan == null)
                {
                    errorMessage = "Loan not found";
                    return false;
                }

                if (loan.Status == "Closed" || loan.Status == "Paid")
                {
                    errorMessage = "Cannot make payment to a closed/paid loan";
                    return false;
                }

                // Check if payment amount is reasonable
                if (payment.TotalAmount > loan.BalanceAmount * 1.5m) // Allow some flexibility for penalties
                {
                    errorMessage = "Payment amount seems too high for this loan";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error validating payment: {ex.Message}");
                errorMessage = "Error validating payment data";
                return false;
            }
        }

        /// <summary>
        /// Update payment record
        /// </summary>
        public bool UpdatePayment(Payment payment)
        {
            try
            {
                if (!ValidatePayment(payment, out string error))
                {
                    Logger.Error($"Invalid payment data: {error}");
                    return false;
                }

                payment.Touch();
                return paymentRepository.Update(payment);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error updating payment: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete payment (soft delete by marking as cancelled)
        /// </summary>
        public bool CancelPayment(int paymentId, string reason)
        {
            try
            {
                var payment = paymentRepository.GetById(paymentId);
                if (payment == null)
                {
                    Logger.Error($"Payment not found: {paymentId}");
                    return false;
                }

                // Create reversal entry
                var reversal = new Payment
                {
                    PaymentNumber = $"REV-{payment.PaymentNumber}",
                    VoucherNumber = payment.VoucherNumber + "R",
                    PaymentDate = DateTime.Now,
                    LoanId = payment.LoanId,
                    CustomerId = payment.CustomerId,
                    VehicleId = payment.VehicleId,
                    TotalAmount = -payment.TotalAmount,
                    PrincipalAmount = -payment.PrincipalAmount,
                    InterestAmount = -payment.InterestAmount,
                    PenaltyAmount = -payment.PenaltyAmount,
                    PaymentMethod = payment.PaymentMethod,
                    Description = $"Reversal: {reason}",
                    ReceivedBy = Environment.UserName,
                    BranchId = payment.BranchId
                };

                var result = paymentRepository.Insert(reversal);
                if (result)
                {
                    Logger.Info($"Payment cancelled: {payment.PaymentNumber} - Reason: {reason}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error cancelling payment: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Generate daily payment report
        /// </summary>
        public DailyPaymentReport GenerateDailyReport(DateTime reportDate)
        {
            try
            {
                var payments = paymentRepository.GetByDateRange(reportDate.Date, reportDate.Date.AddDays(1).AddTicks(-1));

                return new DailyPaymentReport
                {
                    ReportDate = reportDate,
                    Payments = payments,
                    TotalAmount = payments.Sum(p => p.TotalAmount),
                    TotalCount = payments.Count,
                    CashAmount = payments.Where(p => p.PaymentMethod == "Cash").Sum(p => p.TotalAmount),
                    ChequeAmount = payments.Where(p => p.PaymentMethod == "Cheque").Sum(p => p.TotalAmount),
                    OnlineAmount = payments.Where(p => p.PaymentMethod == "Online").Sum(p => p.TotalAmount),
                    GeneratedOn = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Error generating daily payment report: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Generate monthly collection summary
        /// </summary>
        public MonthlyCollectionSummary GenerateMonthlyCollection(int year, int month)
        {
            try
            {
                var fromDate = new DateTime(year, month, 1);
                var toDate = fromDate.AddMonths(1).AddDays(-1);
                
                var payments = paymentRepository.GetByDateRange(fromDate, toDate);
                var groupedByDay = payments.GroupBy(p => p.PaymentDate.Date)
                                          .OrderBy(g => g.Key)
                                          .ToDictionary(g => g.Key, g => g.Sum(p => p.TotalAmount));

                return new MonthlyCollectionSummary
                {
                    Year = year,
                    Month = month,
                    MonthName = fromDate.ToString("MMMM yyyy"),
                    TotalCollection = payments.Sum(p => p.TotalAmount),
                    TotalCount = payments.Count,
                    DailyCollections = groupedByDay,
                    AverageDaily = groupedByDay.Count > 0 ? groupedByDay.Values.Average() : 0,
                    HighestDay = groupedByDay.Count > 0 ? groupedByDay.OrderByDescending(kv => kv.Value).First() : new KeyValuePair<DateTime, decimal>(),
                    GeneratedOn = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Error generating monthly collection summary: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create transaction record for payment
        /// </summary>
        public bool CreatePaymentTransaction(Payment payment)
        {
            try
            {
                var transaction = payment.ToTransaction();
                return transactionRepository.Create(transaction);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error creating payment transaction: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get payment statistics for dashboard
        /// </summary>
        public PaymentStatistics GetPaymentStatistics()
        {
            try
            {
                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);
                
                var todayPayments = GetPaymentsByDateRange(today, today.AddDays(1));
                var monthPayments = GetPaymentsByDateRange(thisMonth, thisMonth.AddMonths(1));
                
                return new PaymentStatistics
                {
                    TodayCount = todayPayments.Count,
                    TodayAmount = todayPayments.Sum(p => p.TotalAmount),
                    MonthCount = monthPayments.Count,
                    MonthAmount = monthPayments.Sum(p => p.TotalAmount),
                    LastPaymentDate = paymentRepository.GetAll()
                        .OrderByDescending(p => p.PaymentDate)
                        .FirstOrDefault()?.PaymentDate ?? DateTime.MinValue,
                    TotalPaymentsCount = paymentRepository.GetAll().Count
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting payment statistics: {ex.Message}");
                return new PaymentStatistics();
            }
        }
    }

    #region Report Models

    public class DailyPaymentReport
    {
        public DateTime ReportDate { get; set; }
        public List<Payment> Payments { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalCount { get; set; }
        public decimal CashAmount { get; set; }
        public decimal ChequeAmount { get; set; }
        public decimal OnlineAmount { get; set; }
        public DateTime GeneratedOn { get; set; }
    }

    public class MonthlyCollectionSummary
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal TotalCollection { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<DateTime, decimal> DailyCollections { get; set; }
        public decimal AverageDaily { get; set; }
        public KeyValuePair<DateTime, decimal> HighestDay { get; set; }
        public DateTime GeneratedOn { get; set; }
    }

    public class PaymentStatistics
    {
        public int TodayCount { get; set; }
        public decimal TodayAmount { get; set; }
        public int MonthCount { get; set; }
        public decimal MonthAmount { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public int TotalPaymentsCount { get; set; }
    }

    #endregion
}