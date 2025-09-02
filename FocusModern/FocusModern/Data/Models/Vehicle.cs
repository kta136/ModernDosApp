using System;

namespace FocusModern.Data.Models
{
    /// <summary>
    /// Vehicle model based on legacy vehicle number format like "UP-25E/T-8036"
    /// </summary>
    public class Vehicle
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; }
        public string StateCode { get; set; }
        public string SeriesCode { get; set; }
        public string RegistrationNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string EngineNumber { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public string Color { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public string Status { get; set; }
        public int? CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public Customer Customer { get; set; }

        public Vehicle()
        {
            Status = "Active";
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            LoanAmount = 0;
            PaidAmount = 0;
            BalanceAmount = 0;
        }

        /// <summary>
        /// Parse legacy vehicle number format like "UP-25E/T-8036"
        /// </summary>
        public void ParseVehicleNumber(string vehicleNumber)
        {
            if (string.IsNullOrEmpty(vehicleNumber))
                return;

            VehicleNumber = vehicleNumber.Trim();

            try
            {
                // Split by "/" to separate state-series from registration
                var parts = VehicleNumber.Split('/');
                if (parts.Length >= 2)
                {
                    // First part: "UP-25E"
                    var stateSeries = parts[0].Trim();
                    var stateSeriesParts = stateSeries.Split('-');
                    if (stateSeriesParts.Length >= 2)
                    {
                        StateCode = stateSeriesParts[0].Trim(); // "UP"
                        SeriesCode = stateSeriesParts[1].Trim(); // "25E"
                    }

                    // Second part: "T-8036" 
                    RegistrationNumber = parts[1].Trim(); // "T-8036"
                }
                else
                {
                    // Handle single part vehicle numbers
                    StateCode = "UK"; // Unknown
                    SeriesCode = "00";
                    RegistrationNumber = VehicleNumber;
                }
            }
            catch (Exception)
            {
                // If parsing fails, store as-is
                StateCode = "UK";
                SeriesCode = "00";
                RegistrationNumber = VehicleNumber;
            }
        }

        /// <summary>
        /// Generate vehicle number from components
        /// </summary>
        public void GenerateVehicleNumber()
        {
            if (!string.IsNullOrEmpty(StateCode) && !string.IsNullOrEmpty(SeriesCode) && !string.IsNullOrEmpty(RegistrationNumber))
            {
                VehicleNumber = string.Format("{0}-{1}/{2}", StateCode, SeriesCode, RegistrationNumber);
            }
        }

        /// <summary>
        /// Get formatted vehicle number for display
        /// </summary>
        public string FormattedVehicleNumber
        {
            get
            {
                if (!string.IsNullOrEmpty(VehicleNumber))
                    return VehicleNumber.ToUpper();
                
                GenerateVehicleNumber();
                return VehicleNumber?.ToUpper() ?? "";
            }
        }

        /// <summary>
        /// Validate vehicle data
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(VehicleNumber) &&
                   !string.IsNullOrWhiteSpace(StateCode) &&
                   !string.IsNullOrWhiteSpace(RegistrationNumber);
        }

        /// <summary>
        /// Update the UpdatedAt timestamp
        /// </summary>
        public void Touch()
        {
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// Calculate balance amount based on loan and paid amounts
        /// </summary>
        public void CalculateBalance()
        {
            BalanceAmount = LoanAmount - PaidAmount;
        }

        /// <summary>
        /// Get vehicle display info
        /// </summary>
        public string DisplayInfo
        {
            get
            {
                var info = FormattedVehicleNumber;
                if (!string.IsNullOrEmpty(Make) && !string.IsNullOrEmpty(Model))
                {
                    info += string.Format(" ({0} {1})", Make, Model);
                }
                return info;
            }
        }

        public override string ToString()
        {
            return FormattedVehicleNumber;
        }
    }
}