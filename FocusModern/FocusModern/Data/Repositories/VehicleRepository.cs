using System;
using System.Collections.Generic;
using System.Data.SQLite;
using FocusModern.Data.Models;
using FocusModern.Utilities;

namespace FocusModern.Data.Repositories
{
    /// <summary>
    /// Repository for vehicle data operations
    /// </summary>
    public class VehicleRepository
    {
        private SQLiteConnection connection;

        public VehicleRepository(SQLiteConnection conn)
        {
            connection = conn ?? throw new ArgumentNullException(nameof(conn));
        }

        /// <summary>
        /// Get all vehicles
        /// </summary>
        public List<Vehicle> GetAll()
        {
            var vehicles = new List<Vehicle>();
            
            try
            {
                string query = @"
                    SELECT v.id, v.vehicle_number, v.state_code, v.series_code, 
                           v.registration_number, v.chassis_number, v.engine_number,
                           v.make, v.model, v.year, v.color, v.loan_amount, v.paid_amount,
                           v.balance_amount, v.status, v.customer_id, v.created_at, v.updated_at,
                           c.name as customer_name, c.customer_code
                    FROM vehicles v 
                    LEFT JOIN customers c ON v.customer_id = c.id
                    ORDER BY v.vehicle_number";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vehicles.Add(MapReaderToVehicle(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting all vehicles: {0}", ex.Message), ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return vehicles;
        }

        /// <summary>
        /// Get vehicle by ID
        /// </summary>
        public Vehicle GetById(int vehicleId)
        {
            try
            {
                string query = @"
                    SELECT v.id, v.vehicle_number, v.state_code, v.series_code, 
                           v.registration_number, v.chassis_number, v.engine_number,
                           v.make, v.model, v.year, v.color, v.loan_amount, v.paid_amount,
                           v.balance_amount, v.status, v.customer_id, v.created_at, v.updated_at,
                           c.name as customer_name, c.customer_code
                    FROM vehicles v 
                    LEFT JOIN customers c ON v.customer_id = c.id
                    WHERE v.id = @id";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", vehicleId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToVehicle(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting vehicle by ID {0}: {1}", vehicleId, ex.Message), ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return null;
        }

        /// <summary>
        /// Search vehicles by vehicle number, make, model, or customer
        /// </summary>
        public List<Vehicle> Search(string searchTerm)
        {
            var vehicles = new List<Vehicle>();
            
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            try
            {
                string query = @"
                    SELECT v.id, v.vehicle_number, v.state_code, v.series_code, 
                           v.registration_number, v.chassis_number, v.engine_number,
                           v.make, v.model, v.year, v.color, v.loan_amount, v.paid_amount,
                           v.balance_amount, v.status, v.customer_id, v.created_at, v.updated_at,
                           c.name as customer_name, c.customer_code
                    FROM vehicles v 
                    LEFT JOIN customers c ON v.customer_id = c.id
                    WHERE v.vehicle_number LIKE @search 
                       OR v.make LIKE @search 
                       OR v.model LIKE @search
                       OR v.chassis_number LIKE @search
                       OR v.engine_number LIKE @search
                       OR c.name LIKE @search
                       OR c.customer_code LIKE @search
                    ORDER BY v.vehicle_number";

                string searchPattern = string.Format("%{0}%", searchTerm);

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@search", searchPattern);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            vehicles.Add(MapReaderToVehicle(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error searching vehicles: {0}", ex.Message), ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return vehicles;
        }

        /// <summary>
        /// Get vehicles for a specific customer
        /// </summary>
        public List<Vehicle> GetByCustomerId(int customerId)
        {
            var vehicles = new List<Vehicle>();
            
            try
            {
                string query = @"
                    SELECT v.id, v.vehicle_number, v.state_code, v.series_code, 
                           v.registration_number, v.chassis_number, v.engine_number,
                           v.make, v.model, v.year, v.color, v.loan_amount, v.paid_amount,
                           v.balance_amount, v.status, v.customer_id, v.created_at, v.updated_at,
                           c.name as customer_name, c.customer_code
                    FROM vehicles v 
                    LEFT JOIN customers c ON v.customer_id = c.id
                    WHERE v.customer_id = @customer_id
                    ORDER BY v.vehicle_number";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@customer_id", customerId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            vehicles.Add(MapReaderToVehicle(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting vehicles for customer {0}: {1}", customerId, ex.Message), ex);
                throw;
            }
            finally
            {
                connection.Close();
            }

            return vehicles;
        }

        /// <summary>
        /// Insert new vehicle
        /// </summary>
        public bool Insert(Vehicle vehicle)
        {
            if (vehicle == null || !vehicle.IsValid())
                return false;

            try
            {
                string query = @"
                    INSERT INTO vehicles (vehicle_number, state_code, series_code, 
                        registration_number, chassis_number, engine_number, make, model, 
                        year, color, loan_amount, paid_amount, balance_amount, status, customer_id)
                    VALUES (@vehicle_number, @state_code, @series_code, 
                        @registration_number, @chassis_number, @engine_number, @make, @model, 
                        @year, @color, @loan_amount, @paid_amount, @balance_amount, @status, @customer_id)";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    AddVehicleParameters(command, vehicle);
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error inserting vehicle: {0}", ex.Message), ex);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Update existing vehicle
        /// </summary>
        public bool Update(Vehicle vehicle)
        {
            if (vehicle == null || !vehicle.IsValid())
                return false;

            try
            {
                vehicle.Touch(); // Update the UpdatedAt timestamp

                string query = @"
                    UPDATE vehicles SET 
                        vehicle_number = @vehicle_number, state_code = @state_code, 
                        series_code = @series_code, registration_number = @registration_number,
                        chassis_number = @chassis_number, engine_number = @engine_number,
                        make = @make, model = @model, year = @year, color = @color,
                        loan_amount = @loan_amount, paid_amount = @paid_amount, 
                        balance_amount = @balance_amount, status = @status, 
                        customer_id = @customer_id, updated_at = @updated_at
                    WHERE id = @id";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    AddVehicleParameters(command, vehicle);
                    command.Parameters.AddWithValue("@id", vehicle.Id);
                    command.Parameters.AddWithValue("@updated_at", vehicle.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error updating vehicle: {0}", ex.Message), ex);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Get vehicle count
        /// </summary>
        public int GetCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM vehicles WHERE status = 'Active'";

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result ?? 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting vehicle count: {0}", ex.Message), ex);
                return 0;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Check if vehicle number already exists
        /// </summary>
        public bool VehicleNumberExists(string vehicleNumber, int? excludeId = null)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM vehicles WHERE vehicle_number = @vehicle_number";
                if (excludeId.HasValue)
                {
                    query += " AND id != @exclude_id";
                }

                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@vehicle_number", vehicleNumber);
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
                Logger.Error(string.Format("Error checking vehicle number existence: {0}", ex.Message), ex);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Add vehicle parameters to command
        /// </summary>
        private void AddVehicleParameters(SQLiteCommand command, Vehicle vehicle)
        {
            command.Parameters.AddWithValue("@vehicle_number", vehicle.VehicleNumber ?? "");
            command.Parameters.AddWithValue("@state_code", vehicle.StateCode ?? "");
            command.Parameters.AddWithValue("@series_code", vehicle.SeriesCode ?? "");
            command.Parameters.AddWithValue("@registration_number", vehicle.RegistrationNumber ?? "");
            command.Parameters.AddWithValue("@chassis_number", vehicle.ChassisNumber ?? "");
            command.Parameters.AddWithValue("@engine_number", vehicle.EngineNumber ?? "");
            command.Parameters.AddWithValue("@make", vehicle.Make ?? "");
            command.Parameters.AddWithValue("@model", vehicle.Model ?? "");
            command.Parameters.AddWithValue("@year", vehicle.Year ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@color", vehicle.Color ?? "");
            command.Parameters.AddWithValue("@loan_amount", vehicle.LoanAmount);
            command.Parameters.AddWithValue("@paid_amount", vehicle.PaidAmount);
            command.Parameters.AddWithValue("@balance_amount", vehicle.BalanceAmount);
            command.Parameters.AddWithValue("@status", vehicle.Status ?? "Active");
            command.Parameters.AddWithValue("@customer_id", vehicle.CustomerId ?? (object)DBNull.Value);
        }

        /// <summary>
        /// Map SQLiteDataReader to Vehicle object
        /// </summary>
        private Vehicle MapReaderToVehicle(SQLiteDataReader reader)
        {
            var vehicle = new Vehicle
            {
                Id = Convert.ToInt32(reader["id"]),
                VehicleNumber = reader["vehicle_number"].ToString(),
                StateCode = reader["state_code"].ToString(),
                SeriesCode = reader["series_code"].ToString(),
                RegistrationNumber = reader["registration_number"].ToString(),
                ChassisNumber = reader["chassis_number"].ToString(),
                EngineNumber = reader["engine_number"].ToString(),
                Make = reader["make"].ToString(),
                Model = reader["model"].ToString(),
                Year = reader["year"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["year"]),
                Color = reader["color"].ToString(),
                LoanAmount = Convert.ToDecimal(reader["loan_amount"] ?? 0),
                PaidAmount = Convert.ToDecimal(reader["paid_amount"] ?? 0),
                BalanceAmount = Convert.ToDecimal(reader["balance_amount"] ?? 0),
                Status = reader["status"].ToString(),
                CustomerId = reader["customer_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["customer_id"]),
                CreatedAt = DateTime.Parse(reader["created_at"].ToString()),
                UpdatedAt = DateTime.Parse(reader["updated_at"].ToString())
            };

            // Add customer info if available
            if (reader["customer_name"] != DBNull.Value)
            {
                vehicle.Customer = new Customer
                {
                    Id = vehicle.CustomerId ?? 0,
                    Name = reader["customer_name"].ToString(),
                    CustomerCode = reader["customer_code"].ToString()
                };
            }

            return vehicle;
        }
    }
}