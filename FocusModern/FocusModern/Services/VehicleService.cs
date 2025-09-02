using System;
using System.Collections.Generic;
using System.Linq;
using FocusModern.Data;
using FocusModern.Data.Models;
using FocusModern.Data.Repositories;
using FocusModern.Utilities;

namespace FocusModern.Services
{
    /// <summary>
    /// Business logic service for vehicle operations
    /// </summary>
    public class VehicleService
    {
        private VehicleRepository vehicleRepository;
        private CustomerRepository customerRepository;
        private DatabaseManager databaseManager;
        private int branchNumber;

        public VehicleService(DatabaseManager dbManager, int branchNumber)
        {
            this.databaseManager = dbManager ?? throw new ArgumentNullException(nameof(dbManager));
            this.branchNumber = branchNumber;
            
            var connection = dbManager.GetConnection(branchNumber);
            this.vehicleRepository = new VehicleRepository(connection);
            this.customerRepository = new CustomerRepository(connection);
        }

        /// <summary>
        /// Get all vehicles for current branch
        /// </summary>
        public List<Vehicle> GetAllVehicles()
        {
            try
            {
                Logger.Debug(string.Format("Getting all vehicles for branch {0}", branchNumber));
                return vehicleRepository.GetAll();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting all vehicles for branch {0}: {1}", branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Get vehicle by ID
        /// </summary>
        public Vehicle GetVehicleById(int vehicleId)
        {
            try
            {
                Logger.Debug(string.Format("Getting vehicle {0} for branch {1}", vehicleId, branchNumber));
                return vehicleRepository.GetById(vehicleId);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting vehicle {0} for branch {1}: {2}", vehicleId, branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Search vehicles
        /// </summary>
        public List<Vehicle> SearchVehicles(string searchTerm)
        {
            try
            {
                Logger.Debug(string.Format("Searching vehicles with term '{0}' for branch {1}", searchTerm, branchNumber));
                return vehicleRepository.Search(searchTerm);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error searching vehicles for branch {0}: {1}", branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Get vehicles for a specific customer
        /// </summary>
        public List<Vehicle> GetVehiclesByCustomer(int customerId)
        {
            try
            {
                Logger.Debug(string.Format("Getting vehicles for customer {0} in branch {1}", customerId, branchNumber));
                return vehicleRepository.GetByCustomerId(customerId);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting vehicles for customer {0} in branch {1}: {2}", customerId, branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Get all customers for dropdown lists
        /// </summary>
        public List<Customer> GetAllCustomers()
        {
            try
            {
                return customerRepository.GetAll();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting customers for vehicle service in branch {0}: {1}", branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Create new vehicle
        /// </summary>
        public bool CreateVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
                return false;

            try
            {
                // Parse and generate vehicle number if needed
                if (string.IsNullOrEmpty(vehicle.VehicleNumber))
                {
                    vehicle.GenerateVehicleNumber();
                }
                else
                {
                    vehicle.ParseVehicleNumber(vehicle.VehicleNumber);
                }

                // Calculate balance amount
                vehicle.CalculateBalance();

                // Validate vehicle data
                if (!ValidateVehicle(vehicle))
                {
                    Logger.Warning(string.Format("Vehicle validation failed for branch {0}", branchNumber));
                    return false;
                }

                // Check for duplicate vehicle number
                if (vehicleRepository.VehicleNumberExists(vehicle.VehicleNumber))
                {
                    Logger.Warning(string.Format("Vehicle number {0} already exists in branch {1}", vehicle.VehicleNumber, branchNumber));
                    return false;
                }

                Logger.Info(string.Format("Creating vehicle {0} for branch {1}", vehicle.VehicleNumber, branchNumber));
                
                bool result = vehicleRepository.Insert(vehicle);
                
                if (result)
                {
                    Logger.Info(string.Format("Vehicle {0} created successfully for branch {1}", vehicle.VehicleNumber, branchNumber));
                }
                else
                {
                    Logger.Warning(string.Format("Failed to create vehicle {0} for branch {1}", vehicle.VehicleNumber, branchNumber));
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error creating vehicle for branch {0}: {1}", branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Update existing vehicle
        /// </summary>
        public bool UpdateVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
                return false;

            try
            {
                // Parse vehicle number components
                vehicle.ParseVehicleNumber(vehicle.VehicleNumber);

                // Calculate balance amount
                vehicle.CalculateBalance();

                // Validate vehicle data
                if (!ValidateVehicle(vehicle))
                {
                    Logger.Warning(string.Format("Vehicle validation failed for update in branch {0}", branchNumber));
                    return false;
                }

                // Check for duplicate vehicle number (excluding current vehicle)
                if (vehicleRepository.VehicleNumberExists(vehicle.VehicleNumber, vehicle.Id))
                {
                    Logger.Warning(string.Format("Vehicle number {0} already exists in branch {1}", vehicle.VehicleNumber, branchNumber));
                    return false;
                }

                Logger.Info(string.Format("Updating vehicle {0} for branch {1}", vehicle.VehicleNumber, branchNumber));
                
                bool result = vehicleRepository.Update(vehicle);
                
                if (result)
                {
                    Logger.Info(string.Format("Vehicle {0} updated successfully for branch {1}", vehicle.VehicleNumber, branchNumber));
                }
                else
                {
                    Logger.Warning(string.Format("Failed to update vehicle {0} for branch {1}", vehicle.VehicleNumber, branchNumber));
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error updating vehicle for branch {0}: {1}", branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Get vehicle count
        /// </summary>
        public int GetVehicleCount()
        {
            try
            {
                return vehicleRepository.GetCount();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting vehicle count for branch {0}: {1}", branchNumber, ex.Message), ex);
                return 0;
            }
        }

        /// <summary>
        /// Get summary statistics for vehicles
        /// </summary>
        public VehicleStatistics GetVehicleStatistics()
        {
            try
            {
                var allVehicles = GetAllVehicles();
                
                return new VehicleStatistics
                {
                    TotalVehicles = allVehicles.Count,
                    ActiveVehicles = allVehicles.Count(v => v.Status == "Active"),
                    TotalLoanAmount = allVehicles.Sum(v => v.LoanAmount),
                    TotalPaidAmount = allVehicles.Sum(v => v.PaidAmount),
                    TotalBalanceAmount = allVehicles.Sum(v => v.BalanceAmount),
                    VehiclesWithBalance = allVehicles.Count(v => v.BalanceAmount > 0)
                };
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting vehicle statistics for branch {0}: {1}", branchNumber, ex.Message), ex);
                return new VehicleStatistics();
            }
        }

        /// <summary>
        /// Validate vehicle business rules
        /// </summary>
        private bool ValidateVehicle(Vehicle vehicle)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(vehicle.VehicleNumber))
            {
                Logger.Warning("Vehicle number is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(vehicle.StateCode))
            {
                Logger.Warning("State code is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(vehicle.RegistrationNumber))
            {
                Logger.Warning("Registration number is required");
                return false;
            }

            // Year validation
            if (vehicle.Year.HasValue)
            {
                int currentYear = DateTime.Now.Year;
                if (vehicle.Year.Value < 1950 || vehicle.Year.Value > currentYear + 1)
                {
                    Logger.Warning(string.Format("Vehicle year must be between 1950 and {0}", currentYear + 1));
                    return false;
                }
            }

            // Amount validation
            if (vehicle.LoanAmount < 0)
            {
                Logger.Warning("Loan amount cannot be negative");
                return false;
            }

            if (vehicle.PaidAmount < 0)
            {
                Logger.Warning("Paid amount cannot be negative");
                return false;
            }

            if (vehicle.PaidAmount > vehicle.LoanAmount)
            {
                Logger.Warning("Paid amount cannot exceed loan amount");
                return false;
            }

            // Customer validation
            if (vehicle.CustomerId.HasValue)
            {
                var customer = customerRepository.GetById(vehicle.CustomerId.Value);
                if (customer == null)
                {
                    Logger.Warning(string.Format("Customer with ID {0} not found", vehicle.CustomerId.Value));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if vehicle number already exists
        /// </summary>
        public bool VehicleNumberExists(string vehicleNumber)
        {
            try
            {
                return vehicleRepository.VehicleNumberExists(vehicleNumber);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error checking vehicle number existence: {0}", ex.Message), ex);
                return false;
            }
        }

        /// <summary>
        /// Generate suggested vehicle number based on state and series
        /// </summary>
        public string GenerateSuggestedVehicleNumber(string stateCode, string seriesCode)
        {
            try
            {
                if (string.IsNullOrEmpty(stateCode) || string.IsNullOrEmpty(seriesCode))
                    return "";

                // Find existing vehicles with same state and series
                var existingVehicles = GetAllVehicles()
                    .Where(v => v.StateCode.Equals(stateCode, StringComparison.OrdinalIgnoreCase) && 
                               v.SeriesCode.Equals(seriesCode, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Generate next sequential number
                int nextNumber = 1;
                if (existingVehicles.Any())
                {
                    var maxNumber = existingVehicles
                        .Select(v => ExtractNumberFromRegistration(v.RegistrationNumber))
                        .Where(n => n.HasValue)
                        .DefaultIfEmpty(0)
                        .Max();
                    
                    nextNumber = (maxNumber ?? 0) + 1;
                }

                return string.Format("{0}-{1}/A-{2:D4}", stateCode.ToUpper(), seriesCode.ToUpper(), nextNumber);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error generating suggested vehicle number: {0}", ex.Message), ex);
                return "";
            }
        }

        /// <summary>
        /// Extract numeric part from registration number like "A-1234" returns 1234
        /// </summary>
        private int? ExtractNumberFromRegistration(string registration)
        {
            if (string.IsNullOrEmpty(registration))
                return null;

            try
            {
                // Handle formats like "A-1234", "T-5678", etc.
                var parts = registration.Split('-');
                if (parts.Length >= 2 && int.TryParse(parts[parts.Length - 1], out int number))
                {
                    return number;
                }

                // Handle pure numbers
                if (int.TryParse(registration, out int directNumber))
                {
                    return directNumber;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(string.Format("Could not extract number from registration '{0}': {1}", registration, ex.Message));
            }

            return null;
        }
    }

    /// <summary>
    /// Vehicle statistics data structure
    /// </summary>
    public class VehicleStatistics
    {
        public int TotalVehicles { get; set; }
        public int ActiveVehicles { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalBalanceAmount { get; set; }
        public int VehiclesWithBalance { get; set; }
    }
}