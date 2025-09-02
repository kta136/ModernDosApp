# Data Migration Strategy for FOCUS System

## Overview
This document outlines the comprehensive strategy for migrating legacy FOCUS data from Clipper/dBase format to modern SQLite databases (separate for each branch) while ensuring 100% data integrity and zero data loss.

## Legacy Data Analysis

### File Structure Inventory
```
Branch 1 (Active - 2024 data):
- ACCOUNT.FIL (94,487 bytes) - Customer accounts
- CASH.FIL (7,906,843 bytes) - Transaction records
- CONTROL.FIL (975 bytes) - System configuration
- ROUGH.FIL (220 bytes) - Temporary data

Branch 2 & 3 (Historical - 2006 data):
- Similar structure with historical transactions
- Preserved for audit and compliance
```

### Data Volume Estimation
- **Total Transactions**: ~50,000+ records (based on file sizes)
- **Customer Records**: ~1,000+ accounts
- **Time Span**: 25+ years (1999-2024)
- **Branches**: 3 active branches with separate data

## Migration Architecture

### Tools & Technologies
```csharp
// Primary Migration Stack
- C# .NET Framework 4.8 (Core migration application)
- System.Data.SQLite (SQLite database access)
- Custom dBase/Clipper file readers in C#
- Windows Forms for migration UI
- Embedded migration utility in main application
- Branch-separated SQLite databases
```

### Migration Pipeline
```
Legacy Files → Extract → Transform → Validate → Load → Verify
     ↓            ↓         ↓          ↓        ↓       ↓
   .FIL/.DAT → Raw Data → Clean Data → Checks → SQLite → Reports
```

## Phase-by-Phase Migration Plan

### Phase 1: Data Extraction (Week 1-2)

#### 1.1 Legacy File Reader Development
```csharp
// C# implementation for reading Clipper/dBase files
public class ClipperFileReader
{
    private string filePath;
    private DbfHeader header;
    
    public ClipperFileReader(string filePath)
    {
        this.filePath = filePath;
        this.header = ReadHeader();
    }
    
    private DbfHeader ReadHeader()
    {
        // Parse dBase header structure
        // Extract field definitions, record count
        using (var reader = new BinaryReader(File.OpenRead(filePath)))
        {
            // Read DBF header format
            return new DbfHeader(reader);
        }
    }
    
    public IEnumerable<Dictionary<string, object>> ReadRecords()
    {
        // Iterator to read records one by one
        // Handle deleted records and data types
        using (var reader = new BinaryReader(File.OpenRead(filePath)))
        {
            // Skip to data section
            reader.BaseStream.Seek(header.DataOffset, SeekOrigin.Begin);
            
            for (int i = 0; i < header.RecordCount; i++)
            {
                yield return ReadRecord(reader);
            }
        }
    }
}

#### 1.2 Data Extraction Process
1. **ACCOUNT.FIL Processing**
   - Extract customer information
   - Parse embedded address/contact data
   - Handle special characters and encoding issues

2. **CASH.FIL Processing**
   - Extract transaction records
   - Parse vehicle numbers and amounts
   - Handle date formats and currency

3. **Index File Processing**
   - Extract relationships from .NTX files
   - Rebuild foreign key relationships
   - Validate data consistency

### Phase 2: Data Transformation (Week 2-3)

#### 2.1 Data Cleaning Rules
```csharp
public static class DataCleaningRules
{
    public static string CleanCustomerName(string name)
    {
        if (string.IsNullOrEmpty(name)) return string.Empty;
        
        return name.Trim()
                   .ToTitleCase()
                   .Replace("/", " ")
                   .Replace("\\", " ")
                   .Replace("|", " ")
                   .RegexReplace(@"\s+", " "); // Multiple spaces to single
    }
    
    public static VehicleNumber ParseVehicleNumber(string vehicleStr)
    {
        // Format: "UP-25E / T-8036" -> {State: "UP", Code: "25E", Number: "T-8036"}
        var pattern = @"([A-Z]{2,3})-?(\w+)\s*/\s*([A-Z]-?\d+)";
        var match = Regex.Match(vehicleStr, pattern);
        
        if (match.Success)
        {
            return new VehicleNumber
            {
                State = match.Groups[1].Value,
                Code = match.Groups[2].Value,
                Number = match.Groups[3].Value
            };
        }
        throw new FormatException($"Invalid vehicle number format: {vehicleStr}");
    }
    
    public static decimal CleanAmount(string amountStr)
    {
        if (string.IsNullOrEmpty(amountStr)) return 0;
        
        // Remove currency symbols and extra spaces
        string cleaned = amountStr.Replace("₹", "")
                                  .Replace(",", "")
                                  .Trim();
        
        if (decimal.TryParse(cleaned, out decimal result))
            return Math.Round(result, 2);
            
        return 0;
    }
    
    public static DateTime ParseLegacyDate(string dateStr)
    {
        // Handle DD/MM/YYYY format from legacy system
        if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", 
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
        {
            if (result >= new DateTime(1990, 1, 1) && result <= DateTime.Now)
                return result;
        }
        throw new FormatException($"Invalid date format: {dateStr}");
    }
}
```

#### 2.2 Data Normalization
1. **Customer Data Normalization**
   ```sql
   -- Separate customer info
   customers (id, code, name, contact_info, branch)
   
   -- Extract address information
   customer_addresses (customer_id, type, address, city, state, pincode)
   ```

2. **Vehicle Data Extraction**
   ```python
   def parse_vehicle_number(vehicle_str):
       # "UP-25E / T-8036" -> state: "UP", code: "25E", number: "T-8036"
       pattern = r'([A-Z]{2,3})-?(\w+)\s*/\s*([A-Z]-?\d+)'
       return re.match(pattern, vehicle_str)
   ```

3. **Transaction Normalization**
   - Split combined debit/credit entries
   - Generate unique transaction IDs
   - Establish customer-vehicle-loan relationships

### Phase 3: Data Validation (Week 3-4)

#### 3.1 Integrity Checks
```python
validation_checks = [
    # Financial validations
    'sum_debits_equals_sum_credits_per_customer',
    'no_negative_balances_without_explanation',
    'transaction_dates_within_valid_range',
    
    # Referential integrity
    'all_vehicles_have_valid_customers',
    'all_transactions_have_valid_vehicles',
    'voucher_numbers_are_unique_per_branch',
    
    # Business logic validations
    'payment_amounts_are_reasonable',
    'late_days_calculation_correct',
    'customer_names_not_empty'
]
```

#### 3.2 Cross-Reference Validation
```sql
-- Validate transaction totals match legacy reports
SELECT 
    branch_id,
    SUM(debit_amount) as total_disbursed,
    SUM(credit_amount) as total_collected,
    SUM(debit_amount - credit_amount) as net_outstanding
FROM transactions 
GROUP BY branch_id;
```

### Phase 4: Database Loading (Week 4)

#### 4.1 SQLite Schema Preparation
```sql
-- Create tables with proper constraints for SQLite
CREATE TABLE customers (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    legacy_code TEXT UNIQUE NOT NULL,
    name TEXT NOT NULL,
    contact_info TEXT, -- JSON as TEXT in SQLite
    branch_id INTEGER,
    migrated_from TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Add indexes for performance
CREATE INDEX idx_customers_legacy_code ON customers(legacy_code);
CREATE INDEX idx_customers_branch ON customers(branch_id);
```

#### 4.2 Batch Loading Strategy
```python
# Process in batches to manage memory
BATCH_SIZE = 1000

def load_customers_batch(customer_records):
    with engine.begin() as conn:
        for batch in chunked(customer_records, BATCH_SIZE):
            conn.execute(
                customers_table.insert(),
                [transform_customer(record) for record in batch]
            )
            conn.commit()
```

## Data Mapping Specifications

### Customer Mapping
```
Legacy ACCOUNT.FIL → customers table
┌─────────────────┬──────────────────┬─────────────────┐
│ Legacy Field    │ Modern Field     │ Transformation  │
├─────────────────┼──────────────────┼─────────────────┤
│ Account Code    │ legacy_code      │ Trim, Validate  │
│ Customer Name   │ name             │ Title Case      │
│ Address (embed) │ contact_info     │ JSON Structure  │
│ Branch Dir      │ branch_id        │ Lookup Table    │
└─────────────────┴──────────────────┴─────────────────┘
```

### Transaction Mapping
```
Legacy CASH.FIL → transactions table
┌─────────────────┬──────────────────┬─────────────────┐
│ Legacy Field    │ Modern Field     │ Transformation  │
├─────────────────┼──────────────────┼─────────────────┤
│ Voucher No      │ voucher_number   │ Branch Prefix   │
│ Date            │ transaction_date │ Date Format     │
│ Vehicle No      │ vehicle_id       │ Lookup/Create   │
│ Debit Amount    │ debit_amount     │ Decimal(12,2)   │
│ Credit Amount   │ credit_amount    │ Decimal(12,2)   │
│ Description     │ description      │ Clean Text      │
└─────────────────┴──────────────────┴─────────────────┘
```

## Migration Quality Assurance

### Pre-Migration Validation
1. **Legacy System Backup**: Complete backup of all .FIL/.DAT files
2. **File Integrity Check**: Verify all files are readable and complete
3. **Sample Data Extraction**: Test with small data samples first

### Post-Migration Verification
```python
verification_tests = [
    # Data completeness
    'record_count_matches_legacy',
    'no_null_critical_fields',
    'all_branches_migrated',
    
    # Financial accuracy
    'total_debits_match_legacy',
    'total_credits_match_legacy',
    'customer_balances_accurate',
    
    # Relationship integrity
    'all_foreign_keys_valid',
    'no_orphaned_records',
    'unique_constraints_satisfied'
]
```

### Report Reconciliation
```sql
-- Generate comparison reports
-- Legacy: "FRECOVERY STATEMENT"
-- Modern: Equivalent query

SELECT 
    c.name as customer_name,
    v.registration_number,
    MAX(t.transaction_date) as last_payment_date,
    CURRENT_DATE - MAX(t.transaction_date) as days_since_payment,
    SUM(t.debit_amount - t.credit_amount) as balance_amount
FROM customers c
JOIN vehicles v ON v.customer_id = c.id
JOIN transactions t ON t.customer_id = c.id
WHERE SUM(t.debit_amount - t.credit_amount) > 0
GROUP BY c.id, c.name, v.registration_number
ORDER BY days_since_payment DESC;
```

## Risk Mitigation Strategies

### Data Loss Prevention
1. **Multiple Backups**: Original files + extracted data + checkpoint saves
2. **Incremental Migration**: Process one branch at a time
3. **Rollback Capability**: Ability to restore to any checkpoint

### Error Handling
```python
class MigrationError(Exception):
    def __init__(self, message, record_data=None, file_position=None):
        self.message = message
        self.record_data = record_data
        self.file_position = file_position
        super().__init__(self.message)

# Error logging and recovery
def safe_record_processing(record):
    try:
        return process_record(record)
    except Exception as e:
        log_migration_error(e, record)
        # Continue with next record or halt based on error severity
```

### Performance Optimization
1. **Parallel Processing**: Multi-threaded extraction where safe
2. **Database Optimization**: Temporary removal of indexes during bulk loading
3. **Memory Management**: Stream processing for large files

## Migration Timeline & Checkpoints

### Week 1: Setup & Extraction
- Day 1-2: Development environment setup
- Day 3-5: Legacy file reader development
- Day 6-7: Initial data extraction testing

### Week 2: Transformation Development
- Day 8-10: Data cleaning scripts
- Day 11-12: Transformation rules implementation
- Day 13-14: Initial transformation testing

### Week 3: Validation & Testing
- Day 15-17: Validation rule implementation
- Day 18-19: Sample migration testing
- Day 20-21: Error handling and edge cases

### Week 4: Full Migration Execution
- Day 22-23: Branch 1 migration (current data)
- Day 24-25: Branch 2 & 3 migration (historical)
- Day 26-28: Final validation and report generation

## Success Criteria
- ✅ 100% data extraction from legacy files
- ✅ Zero data loss during transformation
- ✅ All financial totals match legacy reports
- ✅ Foreign key relationships established correctly
- ✅ Migration completed within 4-week timeline
- ✅ Comprehensive audit trail of all changes