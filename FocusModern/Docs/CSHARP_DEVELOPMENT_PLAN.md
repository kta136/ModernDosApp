# C# Development Plan for FOCUS Modern

## Visual Studio Project Structure

### Solution Layout
```
FocusModern.sln
├── FocusModern (Main Application Project)
│   ├── Properties/
│   │   ├── AssemblyInfo.cs
│   │   └── Resources.resx
│   ├── Forms/
│   │   ├── MainForm.cs (Branch selection)
│   │   ├── MainForm.Designer.cs
│   │   ├── DashboardForm.cs (Branch dashboard)
│   │   ├── CustomerForm.cs (Customer management)
│   │   ├── VehicleForm.cs (Vehicle management)
│   │   ├── LoanForm.cs (Loan processing)
│   │   ├── PaymentForm.cs (Payment entry)
│   │   ├── ReportsForm.cs (Report generation)
│   │   └── SettingsForm.cs (Settings)
│   ├── Services/
│   │   ├── CustomerService.cs
│   │   ├── VehicleService.cs
│   │   ├── LoanService.cs
│   │   ├── PaymentService.cs
│   │   └── ReportService.cs
│   ├── Data/
│   │   ├── Models/
│   │   │   ├── Customer.cs
│   │   │   ├── Vehicle.cs
│   │   │   ├── Loan.cs
│   │   │   └── Transaction.cs
│   │   ├── Repositories/
│   │   │   ├── CustomerRepository.cs
│   │   │   ├── VehicleRepository.cs
│   │   │   └── TransactionRepository.cs
│   │   └── DatabaseManager.cs
│   ├── Utilities/
│   │   ├── ValidationHelper.cs
│   │   ├── FormatHelper.cs
│   │   ├── PrintManager.cs
│   │   └── BackupManager.cs
│   ├── Resources/
│   │   ├── Icons/
│   │   ├── Templates/
│   │   └── DatabaseSchemas.sql
│   ├── App.config
│   └── Program.cs
│
├── FocusMigration (Migration Utility Project)
│   ├── Forms/
│   │   └── MigrationForm.cs
│   ├── Services/
│   │   ├── ClipperFileReader.cs
│   │   ├── DataConverter.cs
│   │   └── MigrationService.cs
│   ├── Models/
│   │   └── LegacyModels.cs
│   └── Program.cs
│
└── FocusModern.Tests (Unit Tests Project)
    ├── Services/
    ├── Data/
    └── Utilities/
```

## Development Environment Setup

### Prerequisites
```
Required Software:
✅ Visual Studio 2019/2022 Community (Free)
✅ .NET Framework 4.8 Developer Pack
✅ Git for version control

Optional Tools:
- SQLite Browser (for database inspection)
- Beyond Compare (for data validation)
- Crystal Reports runtime (for advanced reporting)
```

### NuGet Packages Required
```xml
<!-- Main Application Packages -->
<packages>
  <package id="System.Data.SQLite.Core" version="1.0.118" />
  <package id="Newtonsoft.Json" version="13.0.3" />
  <package id="CrystalReports.Engine" version="13.0.4000" />
  <package id="NLog" version="5.2.4" />
</packages>

<!-- Migration Utility Packages -->
<packages>
  <package id="System.Data.SQLite.Core" version="1.0.118" />
  <package id="Newtonsoft.Json" version="13.0.3" />
</packages>
```

## Development Phases

### Phase 1: Foundation Setup (Week 1-2)

#### Week 1: Project Setup
- **Day 1-2**: Create Visual Studio solution and projects
- **Day 3-4**: Setup database schema and connection management
- **Day 5-7**: Create basic Windows Forms templates

#### Week 2: Core Infrastructure
- **Day 8-10**: Implement repository pattern and data models
- **Day 11-12**: Create service layer architecture
- **Day 13-14**: Setup logging and error handling

#### Deliverables Phase 1:
```csharp
// Basic working application with:
- Branch selection form
- Database connection to SQLite
- Basic customer data model
- Repository pattern implementation
- Logging framework setup
```

### Phase 2: Migration System (Week 3-4)

#### Week 3: Legacy File Reading
- **Day 15-17**: Implement Clipper/dBase file reader
- **Day 18-19**: Create data extraction utilities
- **Day 20-21**: Test with sample legacy files

#### Week 4: Data Migration
- **Day 22-24**: Implement data transformation logic
- **Day 25-26**: Create validation and verification systems
- **Day 27-28**: Build migration UI and progress tracking

#### Deliverables Phase 2:
```csharp
// Migration utility with:
- Legacy file reader (.FIL/.DAT support)
- Data transformation engine
- SQLite database creation
- Migration progress tracking
- Data validation reports
```

### Phase 3: Core Business Logic (Week 5-8)

#### Week 5: Customer Management
- **Day 29-31**: Customer form design and implementation
- **Day 32-33**: Customer search and listing functionality
- **Day 34-35**: Customer validation and business rules

#### Week 6: Vehicle Management
- **Day 36-38**: Vehicle registration form
- **Day 39-40**: Vehicle-customer relationship management
- **Day 41-42**: Vehicle search and tracking

#### Week 7: Loan Management
- **Day 43-45**: Loan application form and workflow
- **Day 46-47**: EMI calculation and scheduling
- **Day 48-49**: Loan status management

#### Week 8: Payment Processing
- **Day 50-52**: Payment entry form
- **Day 53-54**: Payment allocation logic
- **Day 55-56**: Receipt generation system

#### Deliverables Phase 3:
```csharp
// Complete business functionality:
- Customer CRUD operations
- Vehicle registration and management
- Loan processing workflow
- Payment recording system
- Business rule validations
```

### Phase 4: Reporting & UI Polish (Week 9-10)

#### Week 9: Reports
- **Day 57-59**: Recovery statement generation
- **Day 60-61**: Day book and transaction reports
- **Day 62-63**: Customer statement reports

#### Week 10: UI/UX Enhancement
- **Day 64-66**: Form design improvements
- **Day 67-68**: Keyboard shortcuts and navigation
- **Day 69-70**: Print system integration

#### Deliverables Phase 4:
```csharp
// Professional application with:
- All legacy reports reproduced
- Modern Windows UI design
- Print preview and printing
- Keyboard navigation
- Professional look and feel
```

### Phase 5: Testing & Deployment (Week 11-12)

#### Week 11: Testing
- **Day 71-73**: Unit testing for business logic
- **Day 74-75**: Integration testing with real data
- **Day 76-77**: User acceptance testing

#### Week 12: Deployment
- **Day 78-80**: Single EXE packaging
- **Day 81-82**: Installation testing
- **Day 83-84**: Documentation and user training

## Key Development Guidelines

### Code Standards
```csharp
// Naming Conventions
public class CustomerService         // PascalCase for classes
{
    private DatabaseManager dbManager;   // camelCase for fields
    
    public List<Customer> GetAllCustomers()  // PascalCase for methods
    {
        var customers = new List<Customer>(); // var for obvious types
        return customers;
    }
}

// Error Handling
try
{
    // Business logic
}
catch (SqliteException sqlEx)
{
    Logger.Error($"Database error: {sqlEx.Message}");
    MessageBox.Show("Database error occurred. Please contact support.");
}
catch (Exception ex)
{
    Logger.Error($"Unexpected error: {ex.Message}");
    MessageBox.Show("An unexpected error occurred.");
}
```

### Windows Forms Best Practices
```csharp
public partial class CustomerForm : Form
{
    private CustomerService customerService;
    private bool isEditMode = false;
    
    public CustomerForm(CustomerService service)
    {
        InitializeComponent();
        customerService = service;
        SetupForm();
    }
    
    private void SetupForm()
    {
        // Set tab order
        this.TabStop = false;
        textBoxName.TabIndex = 1;
        textBoxPhone.TabIndex = 2;
        
        // Set keyboard shortcuts
        this.KeyPreview = true;
        
        // Bind events
        buttonSave.Click += ButtonSave_Click;
        this.KeyDown += CustomerForm_KeyDown;
    }
    
    private void CustomerForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.S)
        {
            ButtonSave_Click(sender, e);
        }
        else if (e.KeyCode == Keys.Escape)
        {
            this.Close();
        }
    }
}
```

### Database Connection Management
```csharp
public class DatabaseManager : IDisposable
{
    private Dictionary<int, SQLiteConnection> connections;
    
    public SQLiteConnection GetConnection(int branchId)
    {
        if (!connections.ContainsKey(branchId))
        {
            string dbPath = GetDatabasePath(branchId);
            connections[branchId] = new SQLiteConnection($"Data Source={dbPath}");
        }
        return connections[branchId];
    }
    
    public void Dispose()
    {
        foreach (var connection in connections.Values)
        {
            connection?.Dispose();
        }
        connections.Clear();
    }
}
```

## Testing Strategy

### Unit Testing
```csharp
[TestClass]
public class CustomerServiceTests
{
    private CustomerService customerService;
    private MockRepository mockRepo;
    
    [TestInitialize]
    public void Setup()
    {
        mockRepo = new MockRepository();
        customerService = new CustomerService(mockRepo);
    }
    
    [TestMethod]
    public void CreateCustomer_ValidData_ReturnsTrue()
    {
        // Arrange
        var customer = new Customer 
        { 
            Name = "Test Customer", 
            Phone = "9876543210" 
        };
        
        // Act
        var result = customerService.CreateCustomer(customer);
        
        // Assert
        Assert.IsTrue(result);
    }
}
```

### Integration Testing
```csharp
[TestClass]
public class DatabaseIntegrationTests
{
    private string testDatabasePath;
    private DatabaseManager dbManager;
    
    [TestInitialize]
    public void Setup()
    {
        testDatabasePath = Path.GetTempFileName();
        dbManager = new DatabaseManager();
        dbManager.CreateDatabase(testDatabasePath, 1);
    }
    
    [TestMethod]
    public void MigrationTest_LegacyData_CorrectlyMigrated()
    {
        // Test migration of sample legacy data
        // Verify data integrity after migration
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        dbManager.Dispose();
        File.Delete(testDatabasePath);
    }
}
```

## Build and Deployment Configuration

### Release Build Settings
```xml
<!-- FocusModern.csproj settings for single EXE -->
<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
  <PlatformTarget>AnyCPU</PlatformTarget>
  <DebugType>none</DebugType>
  <Optimize>true</Optimize>
  <OutputPath>bin\Release\</OutputPath>
  <DefineConstants>TRACE</DefineConstants>
  <ErrorReport>prompt</ErrorReport>
  <WarningLevel>4</WarningLevel>
  <Prefer32Bit>false</Prefer32Bit>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>false</SelfContained>
</PropertyGroup>
```

### Application Manifest
```xml
<!-- app.manifest for Windows compatibility -->
<?xml version="1.0" encoding="utf-8"?>
<assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
  <compatibility xmlns="urn:schemas-microsoft-com:compatibility.v1">
    <application>
      <!-- Windows 10 and Windows 11 -->
      <supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}"/>
      <supportedOS Id="{1f676c76-80e1-4239-95bb-83d0f6d0da78}"/>
    </application>
  </compatibility>
  <trustInfo xmlns="urn:schemas-microsoft-com:asm.v2">
    <security>
      <requestedPrivileges xmlns="urn:schemas-microsoft-com:asm.v3">
        <requestedExecutionLevel level="asInvoker" uiAccess="false" />
      </requestedPrivileges>
    </security>
  </trustInfo>
</assembly>
```

This development plan provides a structured approach to building the FOCUS Modern system with C# Windows Forms, ensuring a professional single-EXE deployment that meets all your requirements.