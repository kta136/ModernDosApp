using System;
using System.Collections.Generic;
using System.Linq;
using FocusModern.Data.Models;
using FocusModern.Data.Repositories;
using FocusModern.Utilities;

namespace FocusModern.Services
{
    public class LoanService
    {
        private readonly LoanRepository loanRepository;
        private readonly PaymentRepository paymentRepository;
        private readonly CustomerRepository customerRepository;
        private readonly VehicleRepository vehicleRepository;
        private readonly TransactionRepository transactionRepository;
        private readonly Data.DatabaseManager dbManager;
        private readonly int branchId;

        public LoanService(
            LoanRepository loanRepo, 
            PaymentRepository paymentRepo,
            CustomerRepository customerRepo,
            VehicleRepository vehicleRepo)
        {
            loanRepository = loanRepo;
            paymentRepository = paymentRepo;
            customerRepository = customerRepo;
            vehicleRepository = vehicleRepo;
            transactionRepository = null;
            dbManager = null;
            branchId = 0;
        }

        // Overload that enables voucher sequencing and transactions
        public LoanService(
            LoanRepository loanRepo,
            PaymentRepository paymentRepo,
            CustomerRepository customerRepo,
            VehicleRepository vehicleRepo,
            TransactionRepository txnRepo,
            Data.DatabaseManager databaseManager,
            int branchNumber)
        {
            loanRepository = loanRepo;
            paymentRepository = paymentRepo;
            customerRepository = customerRepo;
            vehicleRepository = vehicleRepo;
            transactionRepository = txnRepo;
            dbManager = databaseManager;
            branchId = branchNumber;
        }

        /// <summary>
        /// Create a new loan with validation
        /// </summary>
        public bool CreateLoan(Loan loan)
        {
            try
            {
                // Validate loan data
                if (!ValidateLoanData(loan))
                    return false;

                // Generate loan number if not provided
                if (string.IsNullOrEmpty(loan.LoanNumber))
                {
                    loan.GenerateLoanNumber(GetNextLoanSequence());
                }

                // Calculate EMI and other financial details
                loan.CalculateEMI();
                loan.CalculateMaturityDate();
                loan.BalanceAmount = loan.PrincipalAmount;

                // Save to database
                var result = loanRepository.Insert(loan);
                if (result)
                {
                    // Update vehicle with loan information
                    UpdateVehicleLoanStatus(loan);
                    Logger.Info($"Loan created successfully: {loan.LoanNumber}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error creating loan: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Process a payment against a loan
        /// </summary>
        public bool ProcessPayment(int loanId, decimal paymentAmount, string paymentMethod, string description = "")
        {
            try
            {
                var loan = loanRepository.GetById(loanId);
                if (loan == null)
                {
                    Logger.Error($"Loan not found: {loanId}");
                    return false;
                }

                if (paymentAmount <= 0)
                {
                    Logger.Error("Payment amount must be positive");
                    return false;
                }

                // Create payment record
                var payment = new Payment
                {
                    LoanId = loanId,
                    CustomerId = loan.CustomerId,
                    VehicleId = loan.VehicleId,
                    PaymentMethod = paymentMethod,
                    Description = description,
                    BranchId = loan.BranchId
                };

                // Allocate payment amounts
                payment.AllocatePayment(loan, paymentAmount);

                // Generate payment and voucher numbers
                var paymentSequence = GetNextPaymentSequence();
                payment.GeneratePaymentNumber(paymentSequence);
                payment.GenerateVoucherNumber(GetNextVoucherNumber());

                // Save payment
                var paymentResult = paymentRepository.Insert(payment);
                if (paymentResult)
                {
                    // Update loan balances
                    UpdateLoanAfterPayment(loan, payment);
                    
                    // Populate related entities for transaction display
                    try
                    {
                        payment.Customer = customerRepository.GetById(loan.CustomerId);
                        payment.Vehicle = vehicleRepository.GetById(loan.VehicleId);
                    }
                    catch {}

                    // Create day-book transaction if repository available
                    try
                    {
                        transactionRepository?.Create(payment.ToTransaction());
                    }
                    catch (Exception tex)
                    {
                        Logger.Error($"Error creating transaction for payment: {tex.Message}");
                    }
                    
                    Logger.Info($"Payment processed: {payment.PaymentNumber} for loan {loan.LoanNumber}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error processing payment: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all active loans for a customer
        /// </summary>
        public List<Loan> GetCustomerActiveLoans(int customerId)
        {
            try
            {
                return loanRepository.GetActiveLoans()
                    .Where(l => l.CustomerId == customerId)
                    .ToList();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting customer loans: {ex.Message}");
                return new List<Loan>();
            }
        }

        /// <summary>
        /// Get loan with full details (customer, vehicle, payments)
        /// </summary>
        public Loan GetLoanDetails(int loanId)
        {
            try
            {
                var loan = loanRepository.GetById(loanId);
                if (loan != null)
                {
                    // Load related data
                    loan.Customer = customerRepository.GetById(loan.CustomerId);
                    loan.Vehicle = vehicleRepository.GetById(loan.VehicleId);
                    loan.Payments = paymentRepository.GetByLoanId(loan.Id);
                }
                return loan;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting loan details: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get overdue loans
        /// </summary>
        public List<Loan> GetOverdueLoans()
        {
            try
            {
                return loanRepository.GetOverdueLoans();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting overdue loans: {ex.Message}");
                return new List<Loan>();
            }
        }

        /// <summary>
        /// Calculate and update penalty for overdue loans
        /// </summary>
        public void UpdateOverduePenalties()
        {
            try
            {
                var overdueLoans = GetOverdueLoans();
                foreach (var loan in overdueLoans)
                {
                    var oldPenalty = loan.PenaltyAmount;
                    loan.CalculateOverdueDays();
                    loan.CalculatePenalty(2.0m); // 2% monthly penalty rate

                    if (loan.PenaltyAmount != oldPenalty)
                    {
                        loanRepository.Update(loan);
                        Logger.Info($"Updated penalty for loan {loan.LoanNumber}: ₹{loan.PenaltyAmount:N2}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error updating overdue penalties: {ex.Message}");
            }
        }

        /// <summary>
        /// Close/settle a loan
        /// </summary>
        public bool CloseLoan(int loanId, decimal settlementAmount)
        {
            try
            {
                var loan = loanRepository.GetById(loanId);
                if (loan == null || loan.Status != "Active")
                    return false;

                // Process final payment
                if (settlementAmount > 0)
                {
                    ProcessPayment(loanId, settlementAmount, "Settlement", "Loan closure settlement");
                }

                // Update loan status
                loan.Status = "Closed";
                loan.Touch();

                var result = loanRepository.Update(loan);
                if (result)
                {
                    // Update vehicle status
                    var vehicle = vehicleRepository.GetById(loan.VehicleId);
                    if (vehicle != null)
                    {
                        vehicle.Status = "Closed";
                        vehicle.BalanceAmount = 0;
                        vehicleRepository.Update(vehicle);
                    }
                    
                    Logger.Info($"Loan closed: {loan.LoanNumber}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error closing loan: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get payment history for a loan
        /// </summary>
        public List<Payment> GetLoanPaymentHistory(int loanId)
        {
            try
            {
                return paymentRepository.GetByLoanId(loanId);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting payment history: {ex.Message}");
                return new List<Payment>();
            }
        }

        /// <summary>
        /// Generate loan statement
        /// </summary>
        public LoanStatement GenerateLoanStatement(int loanId)
        {
            try
            {
                var loan = GetLoanDetails(loanId);
                if (loan == null)
                    return null;

                var payments = GetLoanPaymentHistory(loanId);

                return new LoanStatement
                {
                    Loan = loan,
                    Payments = payments,
                    GeneratedOn = DateTime.Now,
                    TotalPaid = payments.Sum(p => p.TotalAmount),
                    PrincipalPaid = payments.Sum(p => p.PrincipalAmount),
                    InterestPaid = payments.Sum(p => p.InterestAmount),
                    PenaltyPaid = payments.Sum(p => p.PenaltyAmount),
                    OutstandingBalance = loan.BalanceAmount,
                    NextDueDate = CalculateNextDueDate(loan, payments)
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Error generating loan statement: {ex.Message}");
                return null;
            }
        }

        #region Private Helper Methods

        private bool ValidateLoanData(Loan loan)
        {
            if (loan.PrincipalAmount <= 0)
            {
                Logger.Error("Principal amount must be positive");
                return false;
            }

            if (loan.LoanTermMonths <= 0)
            {
                Logger.Error("Loan term must be positive");
                return false;
            }

            if (loan.CustomerId <= 0 || loan.VehicleId <= 0)
            {
                Logger.Error("Customer and Vehicle must be selected");
                return false;
            }

            // Check if customer exists
            var customer = customerRepository.GetById(loan.CustomerId);
            if (customer == null)
            {
                Logger.Error($"Customer not found: {loan.CustomerId}");
                return false;
            }

            // Check if vehicle exists and is available
            var vehicle = vehicleRepository.GetById(loan.VehicleId);
            if (vehicle == null)
            {
                Logger.Error($"Vehicle not found: {loan.VehicleId}");
                return false;
            }

            if (vehicle.Status == "Financed")
            {
                Logger.Error("Vehicle is already financed");
                return false;
            }

            return true;
        }

        private void UpdateVehicleLoanStatus(Loan loan)
        {
            try
            {
                var vehicle = vehicleRepository.GetById(loan.VehicleId);
                if (vehicle != null)
                {
                    vehicle.LoanAmount = loan.PrincipalAmount;
                    vehicle.BalanceAmount = loan.PrincipalAmount;
                    vehicle.Status = "Financed";
                    vehicleRepository.Update(vehicle);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error updating vehicle loan status: {ex.Message}");
            }
        }

        private void UpdateLoanAfterPayment(Loan loan, Payment payment)
        {
            try
            {
                // Update loan amounts
                loan.TotalPaidAmount += payment.TotalAmount;
                loan.PrincipalPaidAmount += payment.PrincipalAmount;
                loan.InterestPaidAmount += payment.InterestAmount;
                loan.BalanceAmount -= payment.PrincipalAmount;
                loan.PenaltyAmount -= payment.PenaltyAmount;

                // Ensure amounts don't go negative
                if (loan.BalanceAmount < 0) loan.BalanceAmount = 0;
                if (loan.PenaltyAmount < 0) loan.PenaltyAmount = 0;

                // Update status if fully paid
                if (loan.BalanceAmount <= 0.01m) // Allow for rounding errors
                {
                    loan.Status = "Paid";
                }

                loan.Touch();
                loanRepository.Update(loan);

                // Update vehicle balance
                var vehicle = vehicleRepository.GetById(loan.VehicleId);
                if (vehicle != null)
                {
                    vehicle.PaidAmount += payment.TotalAmount;
                    vehicle.BalanceAmount = loan.BalanceAmount;
                    if (loan.Status == "Paid")
                    {
                        vehicle.Status = "Paid";
                    }
                    vehicleRepository.Update(vehicle);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error updating loan after payment: {ex.Message}");
            }
        }

        private DateTime CalculateNextDueDate(Loan loan, List<Payment> payments)
        {
            // Simple calculation: loan date + months based on payments made
            var paymentsCount = payments.Count;
            return loan.LoanDate.AddMonths(paymentsCount + 1);
        }

        private int GetNextPaymentSequence()
        {
            // This should ideally come from a database sequence
            return (int)(DateTime.Now.Ticks % 10000);
        }

        private int GetNextVoucherNumber()
        {
            try
            {
                if (dbManager != null && branchId > 0)
                {
                    return dbManager.GetNextVoucherNumber(branchId);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting next voucher number from DB: {ex.Message}");
            }
            // Fallback if dbManager not provided
            return (int)(DateTime.Now.Ticks % 100000);
        }

        /// <summary>
        /// Update an existing loan with validation
        /// </summary>
        public bool UpdateLoan(Loan loan)
        {
            try
            {
                if (loan == null) return false;

                if (!ValidateLoanData(loan))
                    return false;

                // Recompute derived fields
                loan.CalculateEMI();
                loan.CalculateMaturityDate();
                loan.CalculateBalance();

                var result = loanRepository.Update(loan);
                if (result)
                {
                    // Keep vehicle amounts in sync
                    UpdateVehicleLoanStatus(loan);
                    Logger.Info($"Loan updated successfully: {loan.LoanNumber}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error updating loan: {ex.Message}");
                return false;
            }
        }

        private int GetNextLoanSequence()
        {
            // This should ideally come from a database sequence
            return (int)(DateTime.Now.Ticks % 10000);
        }

        #endregion
    }

    /// <summary>
    /// Loan statement model for reporting
    /// </summary>
    public class LoanStatement
    {
        public Loan Loan { get; set; }
        public List<Payment> Payments { get; set; }
        public DateTime GeneratedOn { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal PrincipalPaid { get; set; }
        public decimal InterestPaid { get; set; }
        public decimal PenaltyPaid { get; set; }
        public decimal OutstandingBalance { get; set; }
        public DateTime NextDueDate { get; set; }
    }
}
