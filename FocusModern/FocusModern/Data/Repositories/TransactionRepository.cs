using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using FocusModern.Data.Models;
using FocusModern.Utilities;

namespace FocusModern.Data.Repositories
{
    public class TransactionRepository
    {
        private readonly DatabaseManager dbManager;
        private readonly int branchId;

        public TransactionRepository(DatabaseManager databaseManager, int branchNumber)
        {
            dbManager = databaseManager;
            branchId = branchNumber;
        }

        /// <summary>
        /// Create a new transaction
        /// </summary>
        public bool Create(Transaction transaction)
        {
            var connection = dbManager.GetConnection(branchId);
            
            try
            {
                connection.Open();
                
                string sql = @"
                    INSERT INTO transactions (
                        voucher_number, transaction_date, vehicle_number, customer_name,
                        debit_amount, credit_amount, balance_amount, description,
                        payment_method, reference_number, customer_id, vehicle_id, created_at
                    ) VALUES (
                        @voucher_number, @transaction_date, @vehicle_number, @customer_name,
                        @debit_amount, @credit_amount, @balance_amount, @description,
                        @payment_method, @reference_number, @customer_id, @vehicle_id, @created_at
                    )";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@voucher_number", transaction.VoucherNumber);
                    command.Parameters.AddWithValue("@transaction_date", transaction.TransactionDate);
                    command.Parameters.AddWithValue("@vehicle_number", transaction.VehicleNumber ?? "");
                    command.Parameters.AddWithValue("@customer_name", transaction.CustomerName ?? "");
                    command.Parameters.AddWithValue("@debit_amount", transaction.DebitAmount);
                    command.Parameters.AddWithValue("@credit_amount", transaction.CreditAmount);
                    command.Parameters.AddWithValue("@balance_amount", transaction.BalanceAmount);
                    command.Parameters.AddWithValue("@description", transaction.Description ?? "");
                    command.Parameters.AddWithValue("@payment_method", transaction.PaymentMethod ?? "");
                    command.Parameters.AddWithValue("@reference_number", transaction.ReferenceNumber ?? "");
                    command.Parameters.AddWithValue("@customer_id", transaction.CustomerId);
                    command.Parameters.AddWithValue("@vehicle_id", transaction.VehicleId);
                    command.Parameters.AddWithValue("@created_at", transaction.CreatedAt);

                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error creating transaction: {ex.Message}");
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get transaction by voucher number
        /// </summary>
        public Transaction GetByVoucherNumber(string voucherNumber)
        {
            var connection = dbManager.GetConnection(branchId);
            
            try
            {
                connection.Open();
                
                string sql = @"
                    SELECT id, voucher_number, transaction_date, vehicle_number, customer_name,
                           debit_amount, credit_amount, balance_amount, description,
                           payment_method, reference_number, customer_id, vehicle_id, created_at
                    FROM transactions 
                    WHERE voucher_number = @voucher_number";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@voucher_number", voucherNumber);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapTransaction(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting transaction by voucher: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
            
            return null;
        }

        /// <summary>
        /// Get transactions by date range
        /// </summary>
        public List<Transaction> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            var transactions = new List<Transaction>();
            var connection = dbManager.GetConnection(branchId);
            
            try
            {
                connection.Open();
                
                string sql = @"
                    SELECT id, voucher_number, transaction_date, vehicle_number, customer_name,
                           debit_amount, credit_amount, balance_amount, description,
                           payment_method, reference_number, customer_id, vehicle_id, created_at
                    FROM transactions 
                    WHERE transaction_date BETWEEN @from_date AND @to_date
                    ORDER BY transaction_date DESC";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@from_date", fromDate.Date);
                    command.Parameters.AddWithValue("@to_date", toDate.Date.AddDays(1).AddTicks(-1));
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(MapTransaction(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting transactions by date range: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
            
            return transactions;
        }

        /// <summary>
        /// Get transactions for a specific customer
        /// </summary>
        public List<Transaction> GetByCustomerId(int customerId)
        {
            var transactions = new List<Transaction>();
            var connection = dbManager.GetConnection(branchId);
            
            try
            {
                connection.Open();
                
                string sql = @"
                    SELECT id, voucher_number, transaction_date, vehicle_number, customer_name,
                           debit_amount, credit_amount, balance_amount, description,
                           payment_method, reference_number, customer_id, vehicle_id, created_at
                    FROM transactions 
                    WHERE customer_id = @customer_id
                    ORDER BY transaction_date DESC";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@customer_id", customerId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(MapTransaction(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting transactions by customer: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
            
            return transactions;
        }

        /// <summary>
        /// Get transactions for a specific vehicle
        /// </summary>
        public List<Transaction> GetByVehicleNumber(string vehicleNumber)
        {
            var transactions = new List<Transaction>();
            var connection = dbManager.GetConnection(branchId);
            
            try
            {
                connection.Open();
                
                string sql = @"
                    SELECT id, voucher_number, transaction_date, vehicle_number, customer_name,
                           debit_amount, credit_amount, balance_amount, description,
                           payment_method, reference_number, customer_id, vehicle_id, created_at
                    FROM transactions 
                    WHERE vehicle_number = @vehicle_number
                    ORDER BY transaction_date DESC";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@vehicle_number", vehicleNumber);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(MapTransaction(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting transactions by vehicle: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
            
            return transactions;
        }

        /// <summary>
        /// Get all transactions with pagination
        /// </summary>
        public List<Transaction> GetAll(int limit = 100, int offset = 0)
        {
            var transactions = new List<Transaction>();
            var connection = dbManager.GetConnection(branchId);
            
            try
            {
                connection.Open();
                
                string sql = @"
                    SELECT id, voucher_number, transaction_date, vehicle_number, customer_name,
                           debit_amount, credit_amount, balance_amount, description,
                           payment_method, reference_number, customer_id, vehicle_id, created_at
                    FROM transactions 
                    ORDER BY transaction_date DESC
                    LIMIT @limit OFFSET @offset";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@limit", limit);
                    command.Parameters.AddWithValue("@offset", offset);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(MapTransaction(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting all transactions: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
            
            return transactions;
        }

        /// <summary>
        /// Get daily transaction summary
        /// </summary>
        public DayBookSummary GetDayBookSummary(DateTime date)
        {
            var connection = dbManager.GetConnection(branchId);
            
            try
            {
                connection.Open();
                
                string sql = @"
                    SELECT 
                        SUM(debit_amount) as total_debit,
                        SUM(credit_amount) as total_credit,
                        COUNT(*) as total_count
                    FROM transactions 
                    WHERE DATE(transaction_date) = DATE(@date)";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@date", date.Date);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new DayBookSummary
                            {
                                Date = date,
                                TotalDebit = reader["total_debit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["total_debit"]),
                                TotalCredit = reader["total_credit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["total_credit"]),
                                TransactionCount = Convert.ToInt32(reader["total_count"]),
                                Transactions = GetByDateRange(date, date)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting day book summary: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
            
            return new DayBookSummary { Date = date, Transactions = new List<Transaction>() };
        }

        /// <summary>
        /// Search transactions by description or reference
        /// </summary>
        public List<Transaction> SearchTransactions(string searchTerm)
        {
            var transactions = new List<Transaction>();
            var connection = dbManager.GetConnection(branchId);
            
            try
            {
                connection.Open();
                
                string sql = @"
                    SELECT id, voucher_number, transaction_date, vehicle_number, customer_name,
                           debit_amount, credit_amount, balance_amount, description,
                           payment_method, reference_number, customer_id, vehicle_id, created_at
                    FROM transactions 
                    WHERE description LIKE @search_term 
                       OR reference_number LIKE @search_term
                       OR vehicle_number LIKE @search_term
                       OR customer_name LIKE @search_term
                    ORDER BY transaction_date DESC";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@search_term", $"%{searchTerm}%");
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(MapTransaction(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error searching transactions: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
            
            return transactions;
        }

        /// <summary>
        /// Get running balance for a customer
        /// </summary>
        public decimal GetCustomerBalance(int customerId)
        {
            var connection = dbManager.GetConnection(branchId);
            
            try
            {
                connection.Open();
                
                string sql = @"
                    SELECT 
                        SUM(debit_amount) - SUM(credit_amount) as balance
                    FROM transactions 
                    WHERE customer_id = @customer_id";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@customer_id", customerId);
                    
                    var result = command.ExecuteScalar();
                    return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting customer balance: {ex.Message}");
                return 0;
            }
            finally
            {
                connection.Close();
            }
        }

        #region Private Methods

        private Transaction MapTransaction(SQLiteDataReader reader)
        {
            return new Transaction
            {
                Id = Convert.ToInt32(reader["id"]),
                VoucherNumber = reader["voucher_number"].ToString(),
                TransactionDate = Convert.ToDateTime(reader["transaction_date"]),
                VehicleNumber = reader["vehicle_number"].ToString(),
                CustomerName = reader["customer_name"].ToString(),
                DebitAmount = Convert.ToDecimal(reader["debit_amount"]),
                CreditAmount = Convert.ToDecimal(reader["credit_amount"]),
                BalanceAmount = Convert.ToDecimal(reader["balance_amount"]),
                Description = reader["description"].ToString(),
                PaymentMethod = reader["payment_method"].ToString(),
                ReferenceNumber = reader["reference_number"].ToString(),
                CustomerId = reader["customer_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["customer_id"]),
                VehicleId = reader["vehicle_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["vehicle_id"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };
        }

        #endregion
    }

    /// <summary>
    /// Day book summary model
    /// </summary>
    public class DayBookSummary
    {
        public DateTime Date { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public int TransactionCount { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public decimal NetAmount => TotalDebit - TotalCredit;
    }
}