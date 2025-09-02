using System;

namespace FocusModern.Data.Models
{
    /// <summary>
    /// Customer model based on legacy ACCOUNT.FIL structure
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AadharNumber { get; set; }
        public string PanNumber { get; set; }
        public string Occupation { get; set; }
        public decimal MonthlyIncome { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; }

        public Customer()
        {
            Status = "Active";
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            MonthlyIncome = 0;
        }

        /// <summary>
        /// Generate customer code based on name and date
        /// </summary>
        public void GenerateCustomerCode()
        {
            if (string.IsNullOrEmpty(CustomerCode))
            {
                string namePrefix = GetNamePrefix();
                string dateCode = DateTime.Now.ToString("yyMMdd");
                CustomerCode = $"{namePrefix}{dateCode}";
            }
        }

        /// <summary>
        /// Get prefix from customer name for customer code
        /// </summary>
        private string GetNamePrefix()
        {
            if (string.IsNullOrEmpty(Name) || Name.Length < 2)
                return "CU";

            // Take first two characters and make uppercase
            return Name.Substring(0, Math.Min(2, Name.Length)).ToUpper();
        }

        /// <summary>
        /// Get display name for UI
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(FatherName))
                    return string.Format("{0} S/O {1}", Name, FatherName);
                return Name;
            }
        }

        /// <summary>
        /// Validate customer data
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && 
                   !string.IsNullOrWhiteSpace(CustomerCode);
        }

        /// <summary>
        /// Update the UpdatedAt timestamp
        /// </summary>
        public void Touch()
        {
            UpdatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", CustomerCode, DisplayName);
        }
    }
}