using System;
using System.Collections.Generic;
using System.Data.SQLite;
using FocusModern.Data.Models;
using FocusModern.Utilities;

namespace FocusModern.Data.Repositories
{
    /// <summary>
    /// Repository for loan data operations
    /// </summary>
    public class LoanRepository
    {
        private SQLiteConnection connection;

        public LoanRepository(SQLiteConnection conn)
        {
            connection = conn ?? throw new ArgumentNullException(nameof(conn));
        }

        /// <summary>
        /// Get all loans with customer and vehicle information
        /// </summary>
        public List<Loan> GetAll()
        {
            var loans = new List<Loan>();
            
            try
            {
                string query = @"
                    SELECT l.id, l.loan_number, l.customer_id, l.vehicle_id, l.principal_amount,
                           l.interest_rate, l.loan_term_months, l.emi_amount, l.loan_date, 
                           l.maturity_date, l.total_paid_amount, l.interest_paid_amount,
                           l.principal_paid_amount, l.balance_amount, l.overdue_days,
                           l.penalty_amount, l.status, l.remarks, l.branch_id, 
                           l.created_at, l.updated_at,
                           c.name as customer_name, c.customer_code, c.phone as customer_phone,
                           v.vehicle_number, v.make, v.model
                    FROM loans l
                    LEFT JOIN customers c ON l.customer_id = c.id
                    LEFT JOIN vehicles v ON l.vehicle_id = v.id
                    ORDER BY l.loan_date DESC, l.loan_number";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        loans.Add(MapReaderToLoan(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting all loans: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return loans;
        }

        /// <summary>
        /// Get loan by ID with related data
        /// </summary>
        public Loan GetById(int loanId)
        {
            try
            {
                string query = @"
                    SELECT l.id, l.loan_number, l.customer_id, l.vehicle_id, l.principal_amount,
                           l.interest_rate, l.loan_term_months, l.emi_amount, l.loan_date, 
                           l.maturity_date, l.total_paid_amount, l.interest_paid_amount,
                           l.principal_paid_amount, l.balance_amount, l.overdue_days,
                           l.penalty_amount, l.status, l.remarks, l.branch_id, 
                           l.created_at, l.updated_at,
                           c.name as customer_name, c.customer_code, c.phone as customer_phone,
                           v.vehicle_number, v.make, v.model
                    FROM loans l
                    LEFT JOIN customers c ON l.customer_id = c.id
                    LEFT JOIN vehicles v ON l.vehicle_id = v.id
                    WHERE l.id = @id";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", loanId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToLoan(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting loan by ID {loanId}: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return null;
        }

        /// <summary>
        /// Get loans by customer ID
        /// </summary>
        public List<Loan> GetByCustomerId(int customerId)
        {
            var loans = new List<Loan>();
            
            try
            {
                string query = @"
                    SELECT l.id, l.loan_number, l.customer_id, l.vehicle_id, l.principal_amount,
                           l.interest_rate, l.loan_term_months, l.emi_amount, l.loan_date, 
                           l.maturity_date, l.total_paid_amount, l.interest_paid_amount,
                           l.principal_paid_amount, l.balance_amount, l.overdue_days,
                           l.penalty_amount, l.status, l.remarks, l.branch_id, 
                           l.created_at, l.updated_at,
                           c.name as customer_name, c.customer_code, c.phone as customer_phone,
                           v.vehicle_number, v.make, v.model
                    FROM loans l
                    LEFT JOIN customers c ON l.customer_id = c.id
                    LEFT JOIN vehicles v ON l.vehicle_id = v.id
                    WHERE l.customer_id = @customer_id
                    ORDER BY l.loan_date DESC";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@customer_id", customerId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            loans.Add(MapReaderToLoan(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting loans for customer {customerId}: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return loans;
        }

        /// <summary>
        /// Get active loans (not closed)
        /// </summary>
        public List<Loan> GetActiveLoans()
        {
            var loans = new List<Loan>();
            
            try
            {
                string query = @"
                    SELECT l.id, l.loan_number, l.customer_id, l.vehicle_id, l.principal_amount,
                           l.interest_rate, l.loan_term_months, l.emi_amount, l.loan_date, 
                           l.maturity_date, l.total_paid_amount, l.interest_paid_amount,
                           l.principal_paid_amount, l.balance_amount, l.overdue_days,
                           l.penalty_amount, l.status, l.remarks, l.branch_id, 
                           l.created_at, l.updated_at,
                           c.name as customer_name, c.customer_code, c.phone as customer_phone,
                           v.vehicle_number, v.make, v.model
                    FROM loans l
                    LEFT JOIN customers c ON l.customer_id = c.id
                    LEFT JOIN vehicles v ON l.vehicle_id = v.id
                    WHERE l.status = 'Active' AND l.balance_amount > 0
                    ORDER BY l.overdue_days DESC, l.loan_date DESC";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        loans.Add(MapReaderToLoan(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting active loans: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return loans;
        }

        /// <summary>
        /// Get overdue loans
        /// </summary>
        public List<Loan> GetOverdueLoans()
        {
            var loans = new List<Loan>();
            
            try
            {
                string query = @"
                    SELECT l.id, l.loan_number, l.customer_id, l.vehicle_id, l.principal_amount,
                           l.interest_rate, l.loan_term_months, l.emi_amount, l.loan_date, 
                           l.maturity_date, l.total_paid_amount, l.interest_paid_amount,
                           l.principal_paid_amount, l.balance_amount, l.overdue_days,
                           l.penalty_amount, l.status, l.remarks, l.branch_id, 
                           l.created_at, l.updated_at,
                           c.name as customer_name, c.customer_code, c.phone as customer_phone,
                           v.vehicle_number, v.make, v.model
                    FROM loans l
                    LEFT JOIN customers c ON l.customer_id = c.id
                    LEFT JOIN vehicles v ON l.vehicle_id = v.id
                    WHERE l.status = 'Active' AND l.overdue_days > 0 AND l.balance_amount > 0
                    ORDER BY l.overdue_days DESC, l.balance_amount DESC";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        loans.Add(MapReaderToLoan(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting overdue loans: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return loans;
        }

        /// <summary>
        /// Insert new loan
        /// </summary>
        public bool Insert(Loan loan)
        {
            if (loan == null || !loan.IsValid())
                return false;

            try
            {
                string query = @"
                    INSERT INTO loans (loan_number, customer_id, vehicle_id, principal_amount,
                        interest_rate, loan_term_months, emi_amount, loan_date, maturity_date,
                        total_paid_amount, interest_paid_amount, principal_paid_amount, 
                        balance_amount, overdue_days, penalty_amount, status, remarks, branch_id)
                    VALUES (@loan_number, @customer_id, @vehicle_id, @principal_amount,
                        @interest_rate, @loan_term_months, @emi_amount, @loan_date, @maturity_date,
                        @total_paid_amount, @interest_paid_amount, @principal_paid_amount,
                        @balance_amount, @overdue_days, @penalty_amount, @status, @remarks, @branch_id)";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    AddLoanParameters(command, loan);
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error inserting loan: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Update existing loan
        /// </summary>
        public bool Update(Loan loan)
        {
            if (loan == null || !loan.IsValid())
                return false;

            try
            {
                loan.Touch(); // Update timestamp

                string query = @"
                    UPDATE loans SET 
                        loan_number = @loan_number, customer_id = @customer_id, vehicle_id = @vehicle_id,
                        principal_amount = @principal_amount, interest_rate = @interest_rate,
                        loan_term_months = @loan_term_months, emi_amount = @emi_amount,
                        loan_date = @loan_date, maturity_date = @maturity_date,
                        total_paid_amount = @total_paid_amount, interest_paid_amount = @interest_paid_amount,
                        principal_paid_amount = @principal_paid_amount, balance_amount = @balance_amount,
                        overdue_days = @overdue_days, penalty_amount = @penalty_amount,
                        status = @status, remarks = @remarks, branch_id = @branch_id,
                        updated_at = @updated_at
                    WHERE id = @id";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    AddLoanParameters(command, loan);
                    command.Parameters.AddWithValue("@id", loan.Id);
                    command.Parameters.AddWithValue("@updated_at", loan.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error updating loan: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get loan count by status
        /// </summary>
        public int GetCount(string status = null)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM loans";
                if (!string.IsNullOrEmpty(status))
                {
                    query += " WHERE status = @status";
                }

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        command.Parameters.AddWithValue("@status", status);
                    }
                    
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result ?? 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting loan count: {ex.Message}", ex);
                return 0;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Check if loan number exists
        /// </summary>
        public bool LoanNumberExists(string loanNumber, int? excludeId = null)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM loans WHERE loan_number = @loan_number";
                if (excludeId.HasValue)
                {
                    query += " AND id != @exclude_id";
                }

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@loan_number", loanNumber);
                    if (excludeId.HasValue)
                    {
                        command.Parameters.AddWithValue("@exclude_id", excludeId.Value);
                    }
                    
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result ?? 0) > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error checking loan number existence: {ex.Message}", ex);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Add loan parameters to command
        /// </summary>
        private void AddLoanParameters(SQLiteCommand command, Loan loan)
        {
            command.Parameters.AddWithValue("@loan_number", loan.LoanNumber ?? "");
            command.Parameters.AddWithValue("@customer_id", loan.CustomerId);
            command.Parameters.AddWithValue("@vehicle_id", loan.VehicleId);
            command.Parameters.AddWithValue("@principal_amount", loan.PrincipalAmount);
            command.Parameters.AddWithValue("@interest_rate", loan.InterestRate);
            command.Parameters.AddWithValue("@loan_term_months", loan.LoanTermMonths);
            command.Parameters.AddWithValue("@emi_amount", loan.EmiAmount);
            command.Parameters.AddWithValue("@loan_date", loan.LoanDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@maturity_date", loan.MaturityDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@total_paid_amount", loan.TotalPaidAmount);
            command.Parameters.AddWithValue("@interest_paid_amount", loan.InterestPaidAmount);
            command.Parameters.AddWithValue("@principal_paid_amount", loan.PrincipalPaidAmount);
            command.Parameters.AddWithValue("@balance_amount", loan.BalanceAmount);
            command.Parameters.AddWithValue("@overdue_days", loan.OverdueDays);
            command.Parameters.AddWithValue("@penalty_amount", loan.PenaltyAmount);
            command.Parameters.AddWithValue("@status", loan.Status ?? "Active");
            command.Parameters.AddWithValue("@remarks", loan.Remarks ?? "");
            command.Parameters.AddWithValue("@branch_id", loan.BranchId);
        }

        /// <summary>
        /// Map SQLiteDataReader to Loan object
        /// </summary>
        private Loan MapReaderToLoan(SQLiteDataReader reader)
        {
            var loan = new Loan
            {
                Id = Convert.ToInt32(reader["id"]),
                LoanNumber = reader["loan_number"].ToString(),
                CustomerId = Convert.ToInt32(reader["customer_id"]),
                VehicleId = Convert.ToInt32(reader["vehicle_id"]),
                PrincipalAmount = Convert.ToDecimal(reader["principal_amount"]),
                InterestRate = Convert.ToDecimal(reader["interest_rate"]),
                LoanTermMonths = Convert.ToInt32(reader["loan_term_months"]),
                EmiAmount = Convert.ToDecimal(reader["emi_amount"]),
                LoanDate = DateTime.Parse(reader["loan_date"].ToString()),
                MaturityDate = DateTime.Parse(reader["maturity_date"].ToString()),
                TotalPaidAmount = Convert.ToDecimal(reader["total_paid_amount"] ?? 0),
                InterestPaidAmount = Convert.ToDecimal(reader["interest_paid_amount"] ?? 0),
                PrincipalPaidAmount = Convert.ToDecimal(reader["principal_paid_amount"] ?? 0),
                BalanceAmount = Convert.ToDecimal(reader["balance_amount"] ?? 0),
                OverdueDays = Convert.ToInt32(reader["overdue_days"] ?? 0),
                PenaltyAmount = Convert.ToDecimal(reader["penalty_amount"] ?? 0),
                Status = reader["status"].ToString(),
                Remarks = reader["remarks"].ToString(),
                BranchId = Convert.ToInt32(reader["branch_id"]),
                CreatedAt = DateTime.Parse(reader["created_at"].ToString()),
                UpdatedAt = DateTime.Parse(reader["updated_at"].ToString())
            };

            // Add customer and vehicle info if available
            if (reader["customer_name"] != DBNull.Value)
            {
                loan.Customer = new Customer
                {
                    Id = loan.CustomerId,
                    Name = reader["customer_name"].ToString(),
                    CustomerCode = reader["customer_code"].ToString(),
                    Phone = reader["customer_phone"].ToString()
                };
            }

            if (reader["vehicle_number"] != DBNull.Value)
            {
                loan.Vehicle = new Vehicle
                {
                    Id = loan.VehicleId,
                    VehicleNumber = reader["vehicle_number"].ToString(),
                    Make = reader["make"].ToString(),
                    Model = reader["model"].ToString()
                };
            }

            return loan;
        }
    }
}