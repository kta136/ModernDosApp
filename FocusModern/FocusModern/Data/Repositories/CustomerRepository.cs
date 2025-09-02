using System;
using System.Collections.Generic;
using System.Data.SQLite;
using FocusModern.Data.Models;
using FocusModern.Utilities;

namespace FocusModern.Data.Repositories
{
    /// <summary>
    /// Repository for customer data operations
    /// </summary>
    public class CustomerRepository
    {
        private SQLiteConnection connection;

        public CustomerRepository(SQLiteConnection conn)
        {
            connection = conn ?? throw new ArgumentNullException(nameof(conn));
        }

        /// <summary>
        /// Get all customers
        /// </summary>
        public List<Customer> GetAll()
        {
            var customers = new List<Customer>();
            
            try
            {
                string query = @"
                    SELECT id, customer_code, name, father_name, address, city, state, 
                           pincode, phone, email, aadhar_number, pan_number, occupation, 
                           monthly_income, created_at, updated_at, status 
                    FROM customers 
                    ORDER BY name";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(MapReaderToCustomer(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting all customers: {0}", ex.Message), ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return customers;
        }

        /// <summary>
        /// Get customer by ID
        /// </summary>
        public Customer GetById(int customerId)
        {
            try
            {
                string query = @"
                    SELECT id, customer_code, name, father_name, address, city, state, 
                           pincode, phone, email, aadhar_number, pan_number, occupation, 
                           monthly_income, created_at, updated_at, status 
                    FROM customers 
                    WHERE id = @id";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", customerId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToCustomer(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting customer by ID {0}: {1}", customerId, ex.Message), ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return null;
        }

        /// <summary>
        /// Search customers by name or customer code
        /// </summary>
        public List<Customer> Search(string searchTerm)
        {
            var customers = new List<Customer>();
            
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            try
            {
                string query = @"
                    SELECT id, customer_code, name, father_name, address, city, state, 
                           pincode, phone, email, aadhar_number, pan_number, occupation, 
                           monthly_income, created_at, updated_at, status 
                    FROM customers 
                    WHERE name LIKE @search 
                       OR customer_code LIKE @search 
                       OR phone LIKE @search
                    ORDER BY name";

                string searchPattern = string.Format("%{0}%", searchTerm);

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@search", searchPattern);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(MapReaderToCustomer(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error searching customers: {0}", ex.Message), ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return customers;
        }

        /// <summary>
        /// Insert new customer
        /// </summary>
        public bool Insert(Customer customer)
        {
            if (customer == null || !customer.IsValid())
                return false;

            try
            {
                string query = @"
                    INSERT INTO customers (customer_code, name, father_name, address, 
                        city, state, pincode, phone, email, aadhar_number, pan_number, 
                        occupation, monthly_income, status)
                    VALUES (@customer_code, @name, @father_name, @address, 
                        @city, @state, @pincode, @phone, @email, @aadhar_number, 
                        @pan_number, @occupation, @monthly_income, @status)";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    AddCustomerParameters(command, customer);
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error inserting customer: {0}", ex.Message), ex);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Update existing customer
        /// </summary>
        public bool Update(Customer customer)
        {
            if (customer == null || !customer.IsValid())
                return false;

            try
            {
                customer.Touch(); // Update the UpdatedAt timestamp

                string query = @"
                    UPDATE customers SET 
                        customer_code = @customer_code, name = @name, father_name = @father_name, 
                        address = @address, city = @city, state = @state, pincode = @pincode, 
                        phone = @phone, email = @email, aadhar_number = @aadhar_number, 
                        pan_number = @pan_number, occupation = @occupation, 
                        monthly_income = @monthly_income, status = @status, 
                        updated_at = @updated_at
                    WHERE id = @id";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    AddCustomerParameters(command, customer);
                    command.Parameters.AddWithValue("@id", customer.Id);
                    command.Parameters.AddWithValue("@updated_at", customer.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error updating customer: {0}", ex.Message), ex);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get customer count
        /// </summary>
        public int GetCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM customers WHERE status = 'Active'";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result ?? 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting customer count: {0}", ex.Message), ex);
                return 0;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Add customer parameters to command
        /// </summary>
        private void AddCustomerParameters(SQLiteCommand command, Customer customer)
        {
            command.Parameters.AddWithValue("@customer_code", customer.CustomerCode ?? "");
            command.Parameters.AddWithValue("@name", customer.Name ?? "");
            command.Parameters.AddWithValue("@father_name", customer.FatherName ?? "");
            command.Parameters.AddWithValue("@address", customer.Address ?? "");
            command.Parameters.AddWithValue("@city", customer.City ?? "");
            command.Parameters.AddWithValue("@state", customer.State ?? "");
            command.Parameters.AddWithValue("@pincode", customer.Pincode ?? "");
            command.Parameters.AddWithValue("@phone", customer.Phone ?? "");
            command.Parameters.AddWithValue("@email", customer.Email ?? "");
            command.Parameters.AddWithValue("@aadhar_number", customer.AadharNumber ?? "");
            command.Parameters.AddWithValue("@pan_number", customer.PanNumber ?? "");
            command.Parameters.AddWithValue("@occupation", customer.Occupation ?? "");
            command.Parameters.AddWithValue("@monthly_income", customer.MonthlyIncome);
            command.Parameters.AddWithValue("@status", customer.Status ?? "Active");
        }

        /// <summary>
        /// Map SQLiteDataReader to Customer object
        /// </summary>
        private Customer MapReaderToCustomer(SQLiteDataReader reader)
        {
            return new Customer
            {
                Id = Convert.ToInt32(reader["id"]),
                CustomerCode = reader["customer_code"].ToString(),
                Name = reader["name"].ToString(),
                FatherName = reader["father_name"].ToString(),
                Address = reader["address"].ToString(),
                City = reader["city"].ToString(),
                State = reader["state"].ToString(),
                Pincode = reader["pincode"].ToString(),
                Phone = reader["phone"].ToString(),
                Email = reader["email"].ToString(),
                AadharNumber = reader["aadhar_number"].ToString(),
                PanNumber = reader["pan_number"].ToString(),
                Occupation = reader["occupation"].ToString(),
                MonthlyIncome = Convert.ToDecimal(reader["monthly_income"] ?? 0),
                CreatedAt = DateTime.Parse(reader["created_at"].ToString()),
                UpdatedAt = DateTime.Parse(reader["updated_at"].ToString()),
                Status = reader["status"].ToString()
            };
        }
    }
}