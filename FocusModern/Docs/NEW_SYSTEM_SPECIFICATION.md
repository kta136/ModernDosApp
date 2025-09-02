# FOCUS Modern - Single-User Windows Desktop System Specification

## System Overview
**FOCUS Modern** is a single-user Windows desktop application designed to replace the legacy DOS-based FOCUS system while maintaining all business functionality and adding modern capabilities in a familiar desktop environment.

## Technical Architecture

### Desktop Application Stack
```
┌─────────────────────────────────────────────────────────┐
│             C# Windows Forms Application                │
├─────────────────────────────────────────────────────────┤
│ .NET Framework 4.8 (Pre-installed on Windows 10/11)   │
│ Windows Forms UI (Native Windows Controls)             │
│ System.Data.SQLite (Embedded Database)                 │
│ Crystal Reports / ReportViewer (A4 Printing)           │
│ Single EXE Deployment (10-15MB)                        │
└─────────────────────────────────────────────────────────┘
```

### Branch-Separated Architecture
```
FOCUS Modern Desktop App
│
├── Branch 1 Instance
│   ├── focus_branch1.db (SQLite)
│   ├── Documents/Branch1/
│   └── Reports/Branch1/
│
├── Branch 2 Instance  
│   ├── focus_branch2.db (SQLite)
│   ├── Documents/Branch2/
│   └── Reports/Branch2/
│
├── Branch 3 Instance
│   ├── focus_branch3.db (SQLite)
│   ├── Documents/Branch3/
│   └── Reports/Branch3/
│
└── Shared Components
    ├── Application Logic
    ├── UI Components
    └── Print Templates
```

## Database Schema Design (Per Branch)

### Core Business Tables
```sql
-- Customer Management (focus_branch{1|2|3}.db)
CREATE TABLE customers (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    customer_code TEXT UNIQUE NOT NULL,
    name TEXT NOT NULL,
    father_name TEXT,
    address TEXT,
    city TEXT,
    state TEXT,
    pincode TEXT,
    phone TEXT,
    email TEXT,
    aadhar_number TEXT,
    pan_number TEXT,
    occupation TEXT,
    monthly_income DECIMAL(10,2),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    status TEXT DEFAULT 'active'
);

-- Vehicle Information
CREATE TABLE vehicles (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    registration_number TEXT UNIQUE NOT NULL,
    state_code TEXT NOT NULL,
    vehicle_code TEXT NOT NULL,
    chassis_number TEXT,
    engine_number TEXT,
    make TEXT,
    model TEXT,
    year INTEGER,
    color TEXT,
    customer_id INTEGER REFERENCES customers(id),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Loan Management
CREATE TABLE loans (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    loan_number TEXT UNIQUE NOT NULL,
    customer_id INTEGER REFERENCES customers(id) NOT NULL,
    vehicle_id INTEGER REFERENCES vehicles(id) NOT NULL,
    principal_amount DECIMAL(12,2) NOT NULL,
    interest_rate DECIMAL(5,2) NOT NULL,
    loan_term_months INTEGER NOT NULL,
    emi_amount DECIMAL(10,2),
    loan_date DATE NOT NULL,
    maturity_date DATE,
    status TEXT DEFAULT 'active',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Transaction Management
CREATE TABLE transactions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    voucher_number TEXT UNIQUE NOT NULL,
    transaction_date DATE NOT NULL,
    transaction_type TEXT NOT NULL, -- 'disbursement', 'payment', 'interest', 'penalty'
    customer_id INTEGER REFERENCES customers(id) NOT NULL,
    vehicle_id INTEGER REFERENCES vehicles(id),
    loan_id INTEGER REFERENCES loans(id),
    debit_amount DECIMAL(12,2) DEFAULT 0,
    credit_amount DECIMAL(12,2) DEFAULT 0,
    balance_amount DECIMAL(12,2),
    description TEXT,
    payment_method TEXT, -- 'cash', 'cheque', 'bank_transfer', 'upi'
    reference_number TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- System Configuration
CREATE TABLE system_config (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    config_key TEXT UNIQUE NOT NULL,
    config_value TEXT NOT NULL,
    description TEXT,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

## Feature Specifications

### 1. Single-User Operation
- **No Authentication**: Direct application launch
- **Branch Selection**: Choose branch on startup
- **Windows Integration**: Native Windows look and feel
- **Local Data**: All data stored locally

### 2. Branch Management System
```csharp
public class BranchManager
{
    private Dictionary<int, SQLiteConnection> branchConnections;
    
    public void SelectBranch(int branchNumber)
    {
        // Switch to selected branch database
        // Load branch-specific dashboard
        // Set current working branch
    }
    
    public void SwitchBranch(int newBranchNumber)
    {
        // Save current work
        // Switch database context
        // Reload interface for new branch
    }
}
```

### 3. Customer Management
```csharp
public class CustomerService
{
    public List<Customer> GetAllCustomers()
    public Customer GetCustomerById(int id)
    public bool CreateCustomer(Customer customer)
    public bool UpdateCustomer(Customer customer)
    public List<Customer> SearchCustomers(string searchTerm)
}
```

#### Customer Features
- **Customer Registration**: Complete customer profile management
- **Search & Filter**: By name, phone, customer code, vehicle number
- **Document Management**: Store customer documents locally
- **History Tracking**: Complete transaction history per customer

### 4. Vehicle Management
```csharp
public class VehicleService
{
    public List<Vehicle> GetAllVehicles()
    public Vehicle GetVehicleByRegistration(string regNumber)
    public bool RegisterVehicle(Vehicle vehicle)
    public List<Vehicle> GetVehiclesByCustomer(int customerId)
}
```

#### Vehicle Features
- **Vehicle Registration**: Complete vehicle details
- **State Code Management**: Support for all Indian states
- **Vehicle Search**: By registration number, customer, make/model
- **Vehicle History**: Complete loan and payment history

### 5. Loan Management
```csharp
public class LoanService
{
    public bool CreateLoan(Loan loan)
    public Loan GetLoanById(int loanId)
    public List<Loan> GetActiveLoans()
    public List<Loan> GetOverdueLoans()
    public decimal CalculateEMI(decimal principal, decimal rate, int months)
}
```

#### Loan Features
- **Loan Processing**: Step-by-step loan application
- **EMI Calculator**: Real-time EMI calculation
- **Interest Management**: Daily/monthly interest calculations
- **Overdue Tracking**: Automatic identification of overdue accounts

### 6. Payment Processing
```csharp
public class PaymentService
{
    public bool RecordPayment(Payment payment)
    public void GenerateReceipt(int paymentId)
    public List<Payment> GetPaymentHistory(int customerId)
    public decimal CalculateOutstanding(int loanId)
}
```

#### Payment Features
- **Payment Entry**: Quick payment recording
- **Receipt Generation**: Instant receipt printing
- **Payment Allocation**: Automatic principal/interest split
- **Payment History**: Complete payment tracking

### 7. Reporting System
```csharp
public class ReportService
{
    public Report GenerateRecoveryStatement(int customerId)
    public Report GenerateDayBook(DateTime date)
    public Report GenerateCustomerStatement(int customerId, DateTime from, DateTime to)
    public Report GenerateOverdueReport()
}
```

#### Report Features
- **Recovery Statements**: Legacy-compatible recovery reports
- **Day Book**: Daily transaction summaries
- **Customer Statements**: Period-wise customer reports
- **Overdue Reports**: Late payment tracking
- **A4 Printing**: Standard paper size printing only

## Windows Forms UI Design

### Main Application Flow
```
Application Launch
    ↓
Branch Selection Screen
[Branch 1] [Branch 2] [Branch 3]
    ↓
Selected Branch Dashboard
- Recent transactions
- Overdue accounts summary
- Quick actions menu
    ↓
Main Menu Options
- Customers
- Vehicles  
- Loans
- Payments
- Reports
- Settings
- Switch Branch
```

### Key Forms
```csharp
// Main Forms Structure
public partial class MainForm : Form           // Branch selection
public partial class DashboardForm : Form     // Branch dashboard
public partial class CustomerForm : Form      // Customer management
public partial class VehicleForm : Form       // Vehicle management
public partial class LoanForm : Form          // Loan processing
public partial class PaymentForm : Form       // Payment entry
public partial class ReportsForm : Form       // Report generation
public partial class SettingsForm : Form      // Application settings
```

## Single EXE Deployment

### Application Structure
```
FocusModern.exe (10-15MB)
├── Embedded SQLite Library
├── All UI Resources
├── Report Templates
├── Database Schemas
└── Configuration Files

Runtime Files (Auto-created):
├── focus_branch1.db
├── focus_branch2.db
├── focus_branch3.db
├── Documents/
├── Reports/
├── Backups/
└── Logs/
```

### System Requirements
- **OS**: Windows 10 or Windows 11
- **RAM**: 2GB minimum, 4GB recommended
- **Storage**: 500MB available space
- **Additional Software**: None required

## Data Migration Integration

### Legacy Data Import
```csharp
public class LegacyDataMigration
{
    public bool MigrateBranchData(string legacyPath, int branchNumber)
    {
        // Read legacy .FIL/.DAT files
        // Transform data to modern format
        // Create SQLite database for branch
        // Validate data integrity
        // Generate migration report
    }
}
```

### Migration Process
1. **Branch 1 Migration**: Migrate active data from Old/1/
2. **Branch 2 Migration**: Migrate historical data from Old/2/
3. **Branch 3 Migration**: Migrate historical data from Old/3/
4. **Validation**: Verify all data migrated correctly
5. **Reports**: Generate comparison reports

## Performance Specifications

### Application Performance
- **Startup Time**: < 5 seconds
- **Database Operations**: < 1 second for standard queries
- **Form Loading**: < 2 seconds for complex forms
- **Report Generation**: < 10 seconds for standard reports
- **Memory Usage**: < 100MB typical usage

### Database Performance
- **SQLite Optimization**: Proper indexing and query optimization
- **Concurrent Access**: Single-user, no locking issues
- **Backup Speed**: Full backup < 30 seconds
- **Data Integrity**: ACID compliance with SQLite

## Security Features

### Application Security
- **Local Data**: All data stored locally, no network exposure
- **File System**: Restricted access to application data folder
- **Database**: Optional password protection for SQLite files
- **Audit Trail**: Log all significant operations

### Data Protection
- **Backup Strategy**: Automated daily backups
- **Data Validation**: Input validation and business rule enforcement
- **Error Handling**: Graceful error handling with user feedback
- **Recovery**: Database repair and recovery utilities

## Integration Capabilities

### Future Extensions
- **Network Version**: Potential upgrade to multi-user
- **Cloud Backup**: Optional cloud storage integration
- **Mobile Companion**: Read-only mobile access
- **API Integration**: Future banking/SMS gateway integration

This specification provides a complete blueprint for building FOCUS Modern as a single-user Windows desktop application using C# Windows Forms and SQLite, maintaining all legacy functionality while providing modern reliability and performance.