using System;
using System.Collections.Generic;
using FocusModern.Data;
using FocusModern.Data.Models;
using FocusModern.Data.Repositories;
using FocusModern.Utilities;

namespace FocusModern.Services
{
    /// <summary>
    /// Business logic service for customer operations
    /// </summary>
    public class CustomerService
    {
        private CustomerRepository customerRepository;
        private DatabaseManager databaseManager;
        private int branchNumber;

        public CustomerService(DatabaseManager dbManager, int branchNumber)
        {
            this.databaseManager = dbManager ?? throw new ArgumentNullException(nameof(dbManager));
            this.branchNumber = branchNumber;
            
            var connection = dbManager.GetConnection(branchNumber);
            this.customerRepository = new CustomerRepository(connection);
        }

        /// <summary>
        /// Get all customers for current branch
        /// </summary>
        public List<Customer> GetAllCustomers()
        {
            try
            {
                Logger.Debug(string.Format("Getting all customers for branch {0}", branchNumber));
                return customerRepository.GetAll();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting all customers for branch {0}: {1}", branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Get customer by ID
        /// </summary>
        public Customer GetCustomerById(int customerId)
        {
            try
            {
                Logger.Debug(string.Format("Getting customer {0} for branch {1}", customerId, branchNumber));
                return customerRepository.GetById(customerId);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting customer {0} for branch {1}: {2}", customerId, branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Search customers
        /// </summary>
        public List<Customer> SearchCustomers(string searchTerm)
        {
            try
            {
                Logger.Debug(string.Format("Searching customers with term '{0}' for branch {1}", searchTerm, branchNumber));
                return customerRepository.Search(searchTerm);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error searching customers for branch {0}: {1}", branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Create new customer
        /// </summary>
        public bool CreateCustomer(Customer customer)
        {
            if (customer == null)
                return false;

            try
            {
                // Generate customer code if not provided
                if (string.IsNullOrEmpty(customer.CustomerCode))
                {
                    customer.GenerateCustomerCode();
                }

                // Validate customer data
                if (!ValidateCustomer(customer))
                {
                    Logger.Warning(string.Format("Customer validation failed for branch {0}", branchNumber));
                    return false;
                }

                Logger.Info(string.Format("Creating customer {0} for branch {1}", customer.CustomerCode, branchNumber));
                
                bool result = customerRepository.Insert(customer);
                
                if (result)
                {
                    Logger.Info(string.Format("Customer {0} created successfully for branch {1}", customer.CustomerCode, branchNumber));
                }
                else
                {
                    Logger.Warning(string.Format("Failed to create customer {0} for branch {1}", customer.CustomerCode, branchNumber));
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error creating customer for branch {0}: {1}", branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Update existing customer
        /// </summary>
        public bool UpdateCustomer(Customer customer)
        {
            if (customer == null)
                return false;

            try
            {
                // Validate customer data
                if (!ValidateCustomer(customer))
                {
                    Logger.Warning(string.Format("Customer validation failed for update in branch {0}", branchNumber));
                    return false;
                }

                Logger.Info(string.Format("Updating customer {0} for branch {1}", customer.CustomerCode, branchNumber));
                
                bool result = customerRepository.Update(customer);
                
                if (result)
                {
                    Logger.Info(string.Format("Customer {0} updated successfully for branch {1}", customer.CustomerCode, branchNumber));
                }
                else
                {
                    Logger.Warning(string.Format("Failed to update customer {0} for branch {1}", customer.CustomerCode, branchNumber));
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error updating customer for branch {0}: {1}", branchNumber, ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Get customer count
        /// </summary>
        public int GetCustomerCount()
        {
            try
            {
                return customerRepository.GetCount();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error getting customer count for branch {0}: {1}", branchNumber, ex.Message), ex);
                return 0;
            }
        }

        /// <summary>
        /// Validate customer business rules
        /// </summary>
        private bool ValidateCustomer(Customer customer)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                Logger.Warning("Customer name is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(customer.CustomerCode))
            {
                Logger.Warning("Customer code is required");
                return false;
            }

            // Phone number validation (basic)
            if (!string.IsNullOrWhiteSpace(customer.Phone))
            {
                if (customer.Phone.Length < 10)
                {
                    Logger.Warning("Phone number must be at least 10 digits");
                    return false;
                }
            }

            // Email validation (basic)
            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                if (!customer.Email.Contains("@"))
                {
                    Logger.Warning("Invalid email format");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if customer code already exists
        /// </summary>
        public bool CustomerCodeExists(string customerCode)
        {
            try
            {
                var customers = customerRepository.Search(customerCode);
                return customers.Exists(c => c.CustomerCode.Equals(customerCode, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error checking customer code existence: {0}", ex.Message), ex);
                return false;
            }
        }
    }
}