# C# Windows Forms Architecture Specification

## Application Architecture Overview

### Single EXE Application Structure
```
FocusModern.exe
├── Presentation Layer (Windows Forms)
│   ├── MainForm.cs - Branch selection
│   ├── DashboardForm.cs - Branch dashboard
│   ├── CustomerForm.cs - Customer management
│   ├── VehicleForm.cs - Vehicle management
│   ├── LoanForm.cs - Loan processing
│   ├── PaymentForm.cs - Payment entry
│   ├── ReportsForm.cs - Report generation
│   └── SettingsForm.cs - Application settings
│
├── Business Logic Layer
│   ├── Services/
│   │   ├── CustomerService.cs
│   │   ├── VehicleService.cs
│   │   ├── LoanService.cs
│   │   ├── PaymentService.cs
│   │   └── ReportService.cs
│   ├── Managers/
│   │   ├── DatabaseManager.cs
│   │   ├── BranchManager.cs
│   │   └── PrintManager.cs
│   └── Utilities/
│       ├── ValidationHelper.cs
│       ├── FormatHelper.cs
│       └── BackupManager.cs
│
├── Data Access Layer
│   ├── Repositories/
│   │   ├── CustomerRepository.cs
│   │   ├── VehicleRepository.cs
│   │   ├── LoanRepository.cs
│   │   └── TransactionRepository.cs
│   ├── Models/
│   │   ├── Customer.cs
│   │   ├── Vehicle.cs
│   │   ├── Loan.cs
│   │   ├── Transaction.cs
│   │   └── Branch.cs
│   └── DatabaseContext.cs
│
└── Resources (Embedded)
    ├── Images/ - Icons and logos
    ├── Templates/ - Report templates
    ├── Schemas/ - Database creation scripts
    └── Configuration/ - Default settings
```

## Core Components Specification

### 1. Main Application Form (MainForm.cs)
```csharp
public partial class MainForm : Form
{
    private BranchManager branchManager;
    private DatabaseManager databaseManager;
    
    public MainForm()
    {
        InitializeComponent();
        InitializeApplication();
    }
    
    private void InitializeApplication()
    {
        // Initialize database connections for all branches
        // Create UI for branch selection
        // Set up application-wide settings
    }
    
    private void SelectBranch(int branchNumber)
    {
        // Switch to selected branch
        // Open dashboard for selected branch
        // Load branch-specific data
    }
}
```

### 2. Database Manager (DatabaseManager.cs)
```csharp
public class DatabaseManager
{
    private Dictionary<int, SQLiteConnection> branchConnections;
    private int currentBranch;
    
    public DatabaseManager()
    {
        branchConnections = new Dictionary<int, SQLiteConnection>();
        InitializeDatabases();
    }
    
    private void InitializeDatabases()
    {
        for (int i = 1; i <= 3; i++)
        {
            string dbPath = GetDatabasePath(i);
            EnsureDatabaseExists(dbPath, i);
            branchConnections[i] = new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }
    }
    
    private string GetDatabasePath(int branchNumber)
    {
        string appDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);
        string focusPath = Path.Combine(appDataPath, "FocusModern");
        Directory.CreateDirectory(focusPath);
        return Path.Combine(focusPath, $"focus_branch{branchNumber}.db");
    }
    
    public SQLiteConnection GetConnection(int branchNumber)
    {
        if (branchConnections.ContainsKey(branchNumber))
            return branchConnections[branchNumber];
        throw new ArgumentException($"Invalid branch number: {branchNumber}");
    }
}
```

### 3. Customer Service Layer (CustomerService.cs)
```csharp
public class CustomerService
{
    private CustomerRepository customerRepository;
    private DatabaseManager databaseManager;
    
    public CustomerService(DatabaseManager dbManager, int branchNumber)
    {
        databaseManager = dbManager;
        customerRepository = new CustomerRepository(dbManager.GetConnection(branchNumber));
    }
    
    public List<Customer> GetAllCustomers()
    {
        return customerRepository.GetAll();
    }
    
    public Customer GetCustomerById(int customerId)
    {
        return customerRepository.GetById(customerId);
    }
    
    public bool CreateCustomer(Customer customer)
    {
        // Validate customer data
        if (!ValidateCustomer(customer))
            return false;
            
        // Generate customer code
        customer.CustomerCode = GenerateCustomerCode();
        
        // Save to database
        return customerRepository.Insert(customer);
    }
    
    private bool ValidateCustomer(Customer customer)
    {
        // Business rule validations
        return !string.IsNullOrEmpty(customer.Name) && 
               !string.IsNullOrEmpty(customer.Phone);
    }
    
    private string GenerateCustomerCode()
    {
        // Generate unique customer code
        return $"C{DateTime.Now:yyMMdd}{GetNextSequence():000}";
    }
}
```

### 4. Data Models (Models/Customer.cs)
```csharp
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
    public string AlternatePhone { get; set; }
    public string Email { get; set; }
    public string AadharNumber { get; set; }
    public string PanNumber { get; set; }
    public string Occupation { get; set; }
    public decimal MonthlyIncome { get; set; }
    public int BranchId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status { get; set; } = "Active";
}

public class Vehicle
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; }
    public string StateCode { get; set; }
    public string VehicleCode { get; set; }
    public string ChassisNumber { get; set; }
    public string EngineNumber { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public int CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Loan
{
    public int Id { get; set; }
    public string LoanNumber { get; set; }
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int LoanTermMonths { get; set; }
    public decimal EmiAmount { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime MaturityDate { get; set; }
    public string Status { get; set; } = "Active";
    public int BranchId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Transaction
{
    public int Id { get; set; }
    public string VoucherNumber { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; }
    public int CustomerId { get; set; }
    public int? VehicleId { get; set; }
    public int? LoanId { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public string Description { get; set; }
    public string PaymentMethod { get; set; }
    public string ReferenceNumber { get; set; }
    public int BranchId { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 5. Repository Pattern (CustomerRepository.cs)
```csharp
public class CustomerRepository
{
    private SQLiteConnection connection;
    
    public CustomerRepository(SQLiteConnection conn)
    {
        connection = conn;
        EnsureTableExists();
    }
    
    private void EnsureTableExists()
    {
        string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS customers (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                customer_code TEXT UNIQUE NOT NULL,
                name TEXT NOT NULL,
                father_name TEXT,
                address TEXT,
                city TEXT,
                state TEXT,
                pincode TEXT,
                phone TEXT,
                alternate_phone TEXT,
                email TEXT,
                aadhar_number TEXT,
                pan_number TEXT,
                occupation TEXT,
                monthly_income DECIMAL(10,2),
                branch_id INTEGER,
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                status TEXT DEFAULT 'Active'
            );";
        
        using (var command = new SQLiteCommand(createTableQuery, connection))
        {
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    
    public List<Customer> GetAll()
    {
        var customers = new List<Customer>();
        string query = "SELECT * FROM customers ORDER BY name";
        
        using (var command = new SQLiteCommand(query, connection))
        {
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    customers.Add(MapReaderToCustomer(reader));
                }
            }
            connection.Close();
        }
        
        return customers;
    }
    
    public bool Insert(Customer customer)
    {
        string query = @"
            INSERT INTO customers (customer_code, name, father_name, address, 
                city, state, pincode, phone, alternate_phone, email, 
                aadhar_number, pan_number, occupation, monthly_income, branch_id)
            VALUES (@customerCode, @name, @fatherName, @address, 
                @city, @state, @pincode, @phone, @alternatePhone, @email,
                @aadharNumber, @panNumber, @occupation, @monthlyIncome, @branchId)";
        
        try
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                AddCustomerParameters(command, customer);
                connection.Open();
                int result = command.ExecuteNonQuery();
                connection.Close();
                return result > 0;
            }
        }
        catch (Exception ex)
        {
            // Log error
            return false;
        }
    }
}
```

### 6. Windows Forms UI Design Specifications

#### Main Form Layout
```
┌─────────────────────────────────────────────────────────────┐
│ FOCUS Modern - Vehicle Finance Management System            │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│           Select Branch to Continue                         │
│                                                             │
│     ┌─────────────┐  ┌─────────────┐  ┌─────────────┐      │
│     │   Branch 1  │  │   Branch 2  │  │   Branch 3  │      │
│     │   (Active)  │  │ (Historical)│  │ (Historical)│      │
│     │             │  │             │  │             │      │
│     │    Click    │  │    Click    │  │    Click    │      │
│     │  to Enter   │  │  to Enter   │  │  to Enter   │      │
│     └─────────────┘  └─────────────┘  └─────────────┘      │
│                                                             │
│  Status: Ready          Version: 1.0      [Settings] [Help]│
└─────────────────────────────────────────────────────────────┘
```

#### Dashboard Form Layout (After Branch Selection)
```
┌─────────────────────────────────────────────────────────────┐
│ FOCUS Modern - Branch 1 Dashboard                          │
├─────────────────────────────────────────────────────────────┤
│ File  Edit  View  Reports  Tools  Help                     │
├─────────────────────────────────────────────────────────────┤
│ ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐ │
│ │ Total Customers │ │ Active Loans    │ │ Overdue Amount  │ │
│ │      1,234      │ │      567        │ │   ₹12,45,678   │ │
│ └─────────────────┘ └─────────────────┘ └─────────────────┘ │
│                                                             │
│ Quick Actions:                                              │
│ [New Customer] [Record Payment] [Generate Report]          │
│                                                             │
│ Recent Transactions:                                        │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ Date       Customer      Vehicle     Amount     Type    │ │
│ │ 31/08/24   RAM KUMAR     UP-25/T123  25,000   Payment  │ │
│ │ 31/08/24   VIJAY SINGH   UP-38/B456  30,000   Loan     │ │
│ │ 30/08/24   MOHAN LAL     HR-42/C789  15,000   Payment  │ │
│ └─────────────────────────────────────────────────────────┘ │
│                                                             │
│ [Switch Branch]  [Backup Data]      Status: Connected     │
└─────────────────────────────────────────────────────────────┘
```

### 7. Print Manager (PrintManager.cs)
```csharp
public class PrintManager
{
    private PrintDocument printDocument;
    private string currentReportContent;
    
    public PrintManager()
    {
        printDocument = new PrintDocument();
        printDocument.PrintPage += PrintDocument_PrintPage;
    }
    
    public void PrintReport(string reportContent, string title)
    {
        currentReportContent = reportContent;
        printDocument.DocumentName = title;
        
        // Show print preview dialog
        PrintPreviewDialog previewDialog = new PrintPreviewDialog();
        previewDialog.Document = printDocument;
        previewDialog.ShowDialog();
    }
    
    private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
    {
        // A4 paper dimensions: 8.27" x 11.69"
        Font printFont = new Font("Courier New", 10);
        float yPos = 0;
        int count = 0;
        float leftMargin = e.MarginBounds.Left;
        float topMargin = e.MarginBounds.Top;
        string line = null;
        
        StringReader reader = new StringReader(currentReportContent);
        
        while (count < e.MarginBounds.Height / printFont.GetHeight(e.Graphics) && 
               ((line = reader.ReadLine()) != null))
        {
            yPos = topMargin + (count * printFont.GetHeight(e.Graphics));
            e.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos);
            count++;
        }
        
        if (line != null)
            e.HasMorePages = true;
        else
            e.HasMorePages = false;
    }
}
```

## Deployment and Distribution

### Single EXE Configuration
```xml
<!-- App.config for deployment -->
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <startup>
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8" />
    </startup>
    <appSettings>
        <add key="DatabasePath" value="%LocalAppData%\FocusModern\" />
        <add key="BackupInterval" value="24" />
        <add key="LogLevel" value="Info" />
    </appSettings>
</configuration>
```

### Build Configuration
```
Project Settings:
- Target Framework: .NET Framework 4.8
- Platform Target: Any CPU
- Output Type: Windows Application
- Embed all resources: True
- Single File: True
- Optimize Code: True
```

This C# architecture provides:
- **Single EXE deployment** with all dependencies embedded
- **Native Windows performance** and look-and-feel
- **Branch-separated databases** with independent operations
- **Modern development practices** with clean architecture
- **Extensible design** for future enhancements
- **Professional UI** suitable for business use