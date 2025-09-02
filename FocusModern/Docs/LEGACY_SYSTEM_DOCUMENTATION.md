# FOCUS Legacy System Documentation

## System Overview
**FOCUS** is a vehicle finance and loan recovery management system developed by Computer Management Centre, Bareilly, India. The system has been in operation since the late 1980s and handles vehicle loan disbursements, recoveries, and account management.

## Technical Architecture

### Platform & Technology Stack
- **Language**: Clipper (dBase compiler)
- **Operating System**: DOS/Windows (legacy)
- **Database**: dBase/Clipper format (.DBF, .FIL, .DAT, .NTX, .NDX)
- **Main Executable**: FOCUS.EXE (377KB)
- **Startup**: FOCUS.BAT with environment settings

### Database Schema Analysis

#### Core Data Files
1. **ACCOUNT Files** (.FIL/.DAT)
   - Customer account information
   - Account numbers, names, contact details
   - Primary customer data repository

2. **CASH Files** (.FIL/.DAT) 
   - Transaction records (debits/credits)
   - Voucher numbers, dates, amounts
   - Vehicle-wise payment tracking
   - Loan disbursement records

3. **CONTROL Files** (.FIL/.DAT)
   - System configuration and settings
   - Company information
   - System parameters

4. **ROUGH Files** (.FIL/.DAT)
   - Temporary/draft transaction data
   - Incomplete entries

#### Index Files (.NTX/.NDX)
- **ACCOUNT.NTX**: Customer account indexing
- **CASHDTAC.NTX**: Cash transactions by date/account
- **ACDTCASH.NTX**: Account-date-cash cross-reference
- **VNOCASH.NTX**: Vehicle number-cash mapping
- **ROUGH.NTX**: Rough data indexing

#### Configuration Files (.DBF)
- **CNTRL.DBF**: System control parameters
  - State codes (UP-02, UP-07, UP-12, DL-1G, HR-, PB-, UHQ, DIG)
  - Sequential numbering control
- **COMPANY.DBF**: Company master data
  - Multiple company profiles
  - Company names and configurations

### Data Structure Analysis

#### Transaction Records
```
Fields identified from OUTPUT.TXT analysis:
- Date: Transaction date
- V.No.: Voucher number (sequential)
- Vehicle Number: Format "STATE-CODE / VEHICLE-ID"
- Debit: Money outgoing (loan disbursement)
- Credit: Money incoming (recovery/payment)
```

#### Recovery Statement Format
```
From FRECOVERY statements:
- Customer/Vehicle identification
- Last Amount: Previous payment amount
- Last Date: Date of last payment
- Late Days: Days since last payment
- Balance Amount: Outstanding balance
- Vehicle Number: Complete vehicle registration
```

#### Vehicle Number Format
- **Pattern**: `STATE-CODE / VEHICLE-ID`
- **Examples**: UP-25E/T-8036, HR-38/F-2208, DL-1G/B-5149
- **States**: UP (Uttar Pradesh), HR (Haryana), DL (Delhi), etc.

### Multi-Branch Architecture
The system supports multiple branches through numbered subdirectories:
- **Branch 1**: `Old/1/` - Active branch with recent data (2024)
- **Branch 2**: `Old/2/` - Historical data (2006)
- **Branch 3**: `Old/3/` - Historical data (2006)

Each branch maintains identical file structure with separate data.

### Business Logic & Features

#### Core Functionalities
1. **Loan Management**
   - Vehicle loan disbursement tracking
   - Customer account management
   - Payment schedule management

2. **Recovery Operations**
   - Overdue account identification
   - Recovery statement generation
   - Late payment tracking with penalty calculation

3. **Reporting System**
   - Day Book (daily transaction summary)
   - Recovery Statements (overdue analysis)
   - Account-wise transaction history

4. **Financial Controls**
   - Double-entry bookkeeping (Debit/Credit)
   - Voucher-based transaction recording
   - Date-wise transaction tracking

### Data Integrity Features
- **Indexed Access**: Multiple index files for fast data retrieval
- **Sequential Voucher Numbers**: Ensures transaction sequence
- **Cross-Reference Tables**: Vehicle-to-cash mapping
- **Backup Structure**: Multiple branches preserve historical data

### Current System State (2024)
- **Active Operations**: Branch 1 shows transactions up to August 2024
- **Data Volume**: Large transaction files (7.9MB CASH.FIL in Branch 1)
- **Customer Base**: Hundreds of active vehicle loan accounts
- **Geographic Coverage**: Multi-state operations (UP, HR, DL, PB, etc.)

### System Limitations
1. **Technology Obsolescence**: DOS-based Clipper platform
2. **Single User**: No multi-user concurrent access
3. **Limited Scalability**: File-based database constraints
4. **No Web Interface**: Desktop-only application
5. **Manual Backup**: No automated backup systems
6. **Limited Integration**: No API or external system connectivity

### Critical Business Data
- **Historical Span**: 25+ years of financial data (1999-2024)
- **Transaction Volume**: Thousands of loan and recovery records
- **Customer Base**: Multi-state vehicle financing operations
- **Financial Value**: Millions in loan portfolios managed

## Migration Considerations
- All historical data must be preserved during migration
- Business continuity is critical (financial system)
- Regulatory compliance for financial record keeping
- Multi-branch data consolidation requirements
- Modern security and backup requirements