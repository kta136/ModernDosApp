using System;
using System.Collections.Generic;
using System.Data.SQLite;
using FocusModern.Data.Models;
using FocusModern.Utilities;

namespace FocusModern.Data.Repositories
{
    /// <summary>
    /// Repository for payment data operations
    /// </summary>
    public class PaymentRepository
    {
        private SQLiteConnection connection;

        public PaymentRepository(SQLiteConnection conn)
        {
            connection = conn ?? throw new ArgumentNullException(nameof(conn));
        }

        /// <summary>
        /// Get all payments with related loan, customer, and vehicle information
        /// </summary>
        public List<Payment> GetAll()
        {
            var payments = new List<Payment>();
            
            try
            {
                string query = @"
                    SELECT p.id, p.payment_number, p.voucher_number, p.payment_date, p.loan_id,
                           p.customer_id, p.vehicle_id, p.total_amount, p.principal_amount,
                           p.interest_amount, p.penalty_amount, p.payment_method, p.reference_number,
                           p.description, p.received_by, p.branch_id, p.created_at, p.updated_at,
                           l.loan_number, l.principal_amount as loan_principal,
                           c.name as customer_name, c.customer_code, c.phone as customer_phone,
                           v.vehicle_number, v.make, v.model
                    FROM payments p
                    LEFT JOIN loans l ON p.loan_id = l.id
                    LEFT JOIN customers c ON p.customer_id = c.id
                    LEFT JOIN vehicles v ON p.vehicle_id = v.id
                    ORDER BY p.payment_date DESC, p.payment_number";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add(MapReaderToPayment(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting all payments: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return payments;
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        public Payment GetById(int paymentId)
        {
            try
            {
                string query = @"
                    SELECT p.id, p.payment_number, p.voucher_number, p.payment_date, p.loan_id,
                           p.customer_id, p.vehicle_id, p.total_amount, p.principal_amount,
                           p.interest_amount, p.penalty_amount, p.payment_method, p.reference_number,
                           p.description, p.received_by, p.branch_id, p.created_at, p.updated_at,
                           l.loan_number, l.principal_amount as loan_principal,
                           c.name as customer_name, c.customer_code, c.phone as customer_phone,
                           v.vehicle_number, v.make, v.model
                    FROM payments p
                    LEFT JOIN loans l ON p.loan_id = l.id
                    LEFT JOIN customers c ON p.customer_id = c.id
                    LEFT JOIN vehicles v ON p.vehicle_id = v.id
                    WHERE p.id = @id";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", paymentId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToPayment(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting payment by ID {paymentId}: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return null;
        }

        /// <summary>
        /// Get payments by loan ID
        /// </summary>
        public List<Payment> GetByLoanId(int loanId)
        {
            var payments = new List<Payment>();
            
            try
            {
                string query = @"
                    SELECT p.id, p.payment_number, p.voucher_number, p.payment_date, p.loan_id,
                           p.customer_id, p.vehicle_id, p.total_amount, p.principal_amount,
                           p.interest_amount, p.penalty_amount, p.payment_method, p.reference_number,
                           p.description, p.received_by, p.branch_id, p.created_at, p.updated_at,
                           l.loan_number, l.principal_amount as loan_principal,
                           c.name as customer_name, c.customer_code, c.phone as customer_phone,
                           v.vehicle_number, v.make, v.model
                    FROM payments p
                    LEFT JOIN loans l ON p.loan_id = l.id
                    LEFT JOIN customers c ON p.customer_id = c.id
                    LEFT JOIN vehicles v ON p.vehicle_id = v.id
                    WHERE p.loan_id = @loan_id
                    ORDER BY p.payment_date DESC";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@loan_id", loanId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(MapReaderToPayment(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting payments for loan {loanId}: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return payments;
        }

        /// <summary>
        /// Get payments by customer ID
        /// </summary>
        public List<Payment> GetByCustomerId(int customerId)
        {
            var payments = new List<Payment>();
            
            try
            {
                string query = @"
                    SELECT p.id, p.payment_number, p.voucher_number, p.payment_date, p.loan_id,
                           p.customer_id, p.vehicle_id, p.total_amount, p.principal_amount,
                           p.interest_amount, p.penalty_amount, p.payment_method, p.reference_number,
                           p.description, p.received_by, p.branch_id, p.created_at, p.updated_at,
                           l.loan_number, l.principal_amount as loan_principal,
                           c.name as customer_name, c.customer_code, c.phone as customer_phone,
                           v.vehicle_number, v.make, v.model
                    FROM payments p
                    LEFT JOIN loans l ON p.loan_id = l.id
                    LEFT JOIN customers c ON p.customer_id = c.id
                    LEFT JOIN vehicles v ON p.vehicle_id = v.id
                    WHERE p.customer_id = @customer_id
                    ORDER BY p.payment_date DESC";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@customer_id", customerId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(MapReaderToPayment(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting payments for customer {customerId}: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return payments;
        }

        /// <summary>
        /// Get payments by date range
        /// </summary>
        public List<Payment> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            var payments = new List<Payment>();
            
            try
            {
                string query = @"
                    SELECT p.id, p.payment_number, p.voucher_number, p.payment_date, p.loan_id,
                           p.customer_id, p.vehicle_id, p.total_amount, p.principal_amount,
                           p.interest_amount, p.penalty_amount, p.payment_method, p.reference_number,
                           p.description, p.received_by, p.branch_id, p.created_at, p.updated_at,
                           l.loan_number, l.principal_amount as loan_principal,
                           c.name as customer_name, c.customer_code, c.phone as customer_phone,
                           v.vehicle_number, v.make, v.model
                    FROM payments p
                    LEFT JOIN loans l ON p.loan_id = l.id
                    LEFT JOIN customers c ON p.customer_id = c.id
                    LEFT JOIN vehicles v ON p.vehicle_id = v.id
                    WHERE DATE(p.payment_date) >= DATE(@from_date) 
                      AND DATE(p.payment_date) <= DATE(@to_date)
                    ORDER BY p.payment_date DESC";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@from_date", fromDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@to_date", toDate.ToString("yyyy-MM-dd"));
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(MapReaderToPayment(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting payments by date range: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return payments;
        }

        /// <summary>
        /// Insert new payment
        /// </summary>
        public bool Insert(Payment payment)
        {
            if (payment == null || !payment.IsValid())
                return false;

            try
            {
                string query = @"
                    INSERT INTO payments (payment_number, voucher_number, payment_date, loan_id,
                        customer_id, vehicle_id, total_amount, principal_amount, interest_amount,
                        penalty_amount, payment_method, reference_number, description, received_by, branch_id)
                    VALUES (@payment_number, @voucher_number, @payment_date, @loan_id,
                        @customer_id, @vehicle_id, @total_amount, @principal_amount, @interest_amount,
                        @penalty_amount, @payment_method, @reference_number, @description, @received_by, @branch_id)";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    AddPaymentParameters(command, payment);
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error inserting payment: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Update existing payment
        /// </summary>
        public bool Update(Payment payment)
        {
            if (payment == null || !payment.IsValid())
                return false;

            try
            {
                payment.Touch(); // Update timestamp

                string query = @"
                    UPDATE payments SET 
                        payment_number = @payment_number, voucher_number = @voucher_number,
                        payment_date = @payment_date, loan_id = @loan_id, customer_id = @customer_id,
                        vehicle_id = @vehicle_id, total_amount = @total_amount, 
                        principal_amount = @principal_amount, interest_amount = @interest_amount,
                        penalty_amount = @penalty_amount, payment_method = @payment_method,
                        reference_number = @reference_number, description = @description,
                        received_by = @received_by, branch_id = @branch_id, updated_at = @updated_at
                    WHERE id = @id";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    AddPaymentParameters(command, payment);
                    command.Parameters.AddWithValue("@id", payment.Id);
                    command.Parameters.AddWithValue("@updated_at", payment.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error updating payment: {ex.Message}", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get total payments by loan ID
        /// </summary>
        public decimal GetTotalPaymentsByLoanId(int loanId)
        {
            try
            {
                string query = "SELECT COALESCE(SUM(total_amount), 0) FROM payments WHERE loan_id = @loan_id";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@loan_id", loanId);
                    var result = command.ExecuteScalar();
                    return Convert.ToDecimal(result ?? 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting total payments for loan {loanId}: {ex.Message}", ex);
                return 0;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get payment count
        /// </summary>
        public int GetCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM payments";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result ?? 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting payment count: {ex.Message}", ex);
                return 0;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Check if payment number exists
        /// </summary>
        public bool PaymentNumberExists(string paymentNumber, int? excludeId = null)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM payments WHERE payment_number = @payment_number";
                if (excludeId.HasValue)
                {
                    query += " AND id != @exclude_id";
                }

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@payment_number", paymentNumber);
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
                Logger.Error($"Error checking payment number existence: {ex.Message}", ex);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Add payment parameters to command
        /// </summary>
        private void AddPaymentParameters(SQLiteCommand command, Payment payment)
        {
            command.Parameters.AddWithValue("@payment_number", payment.PaymentNumber ?? "");
            command.Parameters.AddWithValue("@voucher_number", payment.VoucherNumber ?? "");
            command.Parameters.AddWithValue("@payment_date", payment.PaymentDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@loan_id", payment.LoanId);
            command.Parameters.AddWithValue("@customer_id", payment.CustomerId);
            command.Parameters.AddWithValue("@vehicle_id", payment.VehicleId);
            command.Parameters.AddWithValue("@total_amount", payment.TotalAmount);
            command.Parameters.AddWithValue("@principal_amount", payment.PrincipalAmount);
            command.Parameters.AddWithValue("@interest_amount", payment.InterestAmount);
            command.Parameters.AddWithValue("@penalty_amount", payment.PenaltyAmount);
            command.Parameters.AddWithValue("@payment_method", payment.PaymentMethod ?? "Cash");
            command.Parameters.AddWithValue("@reference_number", payment.ReferenceNumber ?? "");
            command.Parameters.AddWithValue("@description", payment.Description ?? "");
            command.Parameters.AddWithValue("@received_by", payment.ReceivedBy ?? "");
            command.Parameters.AddWithValue("@branch_id", payment.BranchId);
        }

        /// <summary>
        /// Map SQLiteDataReader to Payment object
        /// </summary>
        private Payment MapReaderToPayment(SQLiteDataReader reader)
        {
            var payment = new Payment
            {
                Id = Convert.ToInt32(reader["id"]),
                PaymentNumber = reader["payment_number"].ToString(),
                VoucherNumber = reader["voucher_number"].ToString(),
                PaymentDate = DateTime.Parse(reader["payment_date"].ToString()),
                LoanId = Convert.ToInt32(reader["loan_id"]),
                CustomerId = Convert.ToInt32(reader["customer_id"]),
                VehicleId = Convert.ToInt32(reader["vehicle_id"]),
                TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                PrincipalAmount = Convert.ToDecimal(reader["principal_amount"] ?? 0),
                InterestAmount = Convert.ToDecimal(reader["interest_amount"] ?? 0),
                PenaltyAmount = Convert.ToDecimal(reader["penalty_amount"] ?? 0),
                PaymentMethod = reader["payment_method"].ToString(),
                ReferenceNumber = reader["reference_number"].ToString(),
                Description = reader["description"].ToString(),
                ReceivedBy = reader["received_by"].ToString(),
                BranchId = Convert.ToInt32(reader["branch_id"]),
                CreatedAt = DateTime.Parse(reader["created_at"].ToString()),
                UpdatedAt = DateTime.Parse(reader["updated_at"].ToString())
            };

            // Add loan info if available
            if (reader["loan_number"] != DBNull.Value)
            {
                payment.Loan = new Loan
                {
                    Id = payment.LoanId,
                    LoanNumber = reader["loan_number"].ToString(),
                    PrincipalAmount = Convert.ToDecimal(reader["loan_principal"] ?? 0)
                };
            }

            // Add customer info if available
            if (reader["customer_name"] != DBNull.Value)
            {
                payment.Customer = new Customer
                {
                    Id = payment.CustomerId,
                    Name = reader["customer_name"].ToString(),
                    CustomerCode = reader["customer_code"].ToString(),
                    Phone = reader["customer_phone"].ToString()
                };
            }

            // Add vehicle info if available
            if (reader["vehicle_number"] != DBNull.Value)
            {
                payment.Vehicle = new Vehicle
                {
                    Id = payment.VehicleId,
                    VehicleNumber = reader["vehicle_number"].ToString(),
                    Make = reader["make"].ToString(),
                    Model = reader["model"].ToString()
                };
            }

            return payment;
        }
    }
}