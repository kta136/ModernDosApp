using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Configuration;
using FocusModern.Utilities;

namespace FocusModern.Data
{
    /// <summary>
    /// Manages SQLite database connections for all three branches
    /// </summary>
    public class DatabaseManager : IDisposable
    {
        private Dictionary<int, SQLiteConnection> branchConnections;
        private string databaseBasePath;
        private bool disposed = false;

        public DatabaseManager()
        {
            branchConnections = new Dictionary<int, SQLiteConnection>();
            
            // Get database path from config, expand environment variables
            string configPath = ConfigurationManager.AppSettings["DatabasePath"] ?? "Data";
            string expanded = Environment.ExpandEnvironmentVariables(configPath);
            // If relative, resolve against current working directory
            if (!Path.IsPathRooted(expanded))
            {
                try
                {
                    expanded = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), expanded));
                }
                catch
                {
                    // Fallback to app base directory
                    expanded = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, expanded));
                }
            }
            databaseBasePath = expanded;
            
            // Ensure directory exists
            Directory.CreateDirectory(databaseBasePath);
            
            Logger.Info(string.Format("Database manager initialized with path: {0}", databaseBasePath));
        }

        /// <summary>
        /// Initialize all three branch databases
        /// </summary>
        public void InitializeAllBranches()
        {
            for (int branchNumber = 1; branchNumber <= 3; branchNumber++)
            {
                InitializeBranchDatabase(branchNumber);
            }
        }

        /// <summary>
        /// Initialize a specific branch database
        /// </summary>
        public void InitializeBranchDatabase(int branchNumber)
        {
            if (branchNumber < 1 || branchNumber > 3)
                throw new ArgumentException("Branch number must be 1, 2, or 3");

            string dbPath = GetDatabasePath(branchNumber);
            bool isNewDatabase = !File.Exists(dbPath);

            var connectionString = string.Format("Data Source={0};Version=3;", dbPath);
            branchConnections[branchNumber] = new SQLiteConnection(connectionString);

            if (isNewDatabase)
            {
                // Attempt to migrate from previous LocalAppData location if present
                TryMigrateFromLocalAppData(dbPath);
                CreateDatabaseSchema(branchNumber);
                Logger.Info(string.Format("Created new database for Branch {0}", branchNumber));
            }
            else
            {
                // Check and upgrade existing database if needed
                UpgradeDatabaseSchema(branchNumber);
                Logger.Info(string.Format("Connected to existing database for Branch {0}", branchNumber));
            }
        }

        /// <summary>
        /// Get connection for specific branch
        /// </summary>
        public SQLiteConnection GetConnection(int branchNumber)
        {
            if (!branchConnections.ContainsKey(branchNumber))
                throw new ArgumentException(string.Format("Branch {0} not initialized", branchNumber));

            var connection = branchConnections[branchNumber];
            if (connection == null)
                throw new InvalidOperationException(string.Format("Connection for Branch {0} is null", branchNumber));

            return connection;
        }

        /// <summary>
        /// Get database file path for branch
        /// </summary>
        private string GetDatabasePath(int branchNumber)
        {
            return Path.Combine(databaseBasePath, string.Format("focus_branch{0}.db", branchNumber));
        }

        /// <summary>
        /// If the new target db file does not exist, but an older db exists in %LocalAppData%\FocusModern, copy it over.
        /// </summary>
        private void TryMigrateFromLocalAppData(string targetDbPath)
        {
            try
            {
                if (File.Exists(targetDbPath)) return;
                string oldBase = Environment.ExpandEnvironmentVariables("%LocalAppData%\\FocusModern\\");
                string fileName = Path.GetFileName(targetDbPath);
                string oldPath = Path.Combine(oldBase, fileName);
                if (File.Exists(oldPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetDbPath) ?? databaseBasePath);
                    File.Copy(oldPath, targetDbPath, overwrite: false);
                    Logger.Info($"Migrated existing database from {oldPath} to {targetDbPath}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Database migration from LocalAppData failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Create complete database schema for a branch
        /// </summary>
        private void CreateDatabaseSchema(int branchNumber)
        {
            var connection = branchConnections[branchNumber];
            
            try
            {
                connection.Open();

                // Create all tables
                CreateCustomersTable(connection);
                CreateVehiclesTable(connection);
                CreateLoansTable(connection);
                CreatePaymentsTable(connection);
                CreateTransactionsTable(connection);
                CreateSystemConfigTable(connection);
                
                // Create indexes for performance
                CreateIndexes(connection);
                
                // Insert default configuration
                InsertDefaultConfiguration(connection, branchNumber);

                Logger.Info(string.Format("Database schema created for Branch {0}", branchNumber));
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Upgrade existing database schema if needed
        /// </summary>
        private void UpgradeDatabaseSchema(int branchNumber)
        {
            var connection = branchConnections[branchNumber];
            
            try
            {
                connection.Open();

                // Check if vehicles table needs upgrading
                if (TableExists(connection, "vehicles"))
                {
                    // Check if new columns exist, add them if missing
                    if (!ColumnExists(connection, "vehicles", "loan_amount"))
                    {
                        Logger.Info(string.Format("Upgrading vehicles table for Branch {0} - adding financial columns", branchNumber));
                        
                        // Add missing columns
                        ExecuteNonQuery(connection, "ALTER TABLE vehicles ADD COLUMN loan_amount DECIMAL(15,2) DEFAULT 0");
                        ExecuteNonQuery(connection, "ALTER TABLE vehicles ADD COLUMN paid_amount DECIMAL(15,2) DEFAULT 0");
                        ExecuteNonQuery(connection, "ALTER TABLE vehicles ADD COLUMN balance_amount DECIMAL(15,2) DEFAULT 0");
                        ExecuteNonQuery(connection, "ALTER TABLE vehicles ADD COLUMN status TEXT DEFAULT 'Active'");
                        ExecuteNonQuery(connection, "ALTER TABLE vehicles ADD COLUMN updated_at DATETIME DEFAULT CURRENT_TIMESTAMP");
                        
                        Logger.Info(string.Format("Successfully upgraded vehicles table for Branch {0}", branchNumber));
                    }
                }
                else
                {
                    // Create vehicles table if it doesn't exist
                    CreateVehiclesTable(connection);
                    Logger.Info(string.Format("Created missing vehicles table for Branch {0}", branchNumber));
                }

                // Ensure other tables exist
                if (!TableExists(connection, "customers"))
                {
                    CreateCustomersTable(connection);
                }

                if (!TableExists(connection, "transactions"))
                {
                    CreateTransactionsTable(connection);
                }

                if (!TableExists(connection, "system_config"))
                {
                    CreateSystemConfigTable(connection);
                    InsertDefaultConfiguration(connection, branchNumber);
                }

                // Ensure new loans and payments tables exist
                if (!TableExists(connection, "loans"))
                {
                    CreateLoansTable(connection);
                    Logger.Info(string.Format("Created loans table for Branch {0}", branchNumber));
                }

                if (!TableExists(connection, "payments"))
                {
                    CreatePaymentsTable(connection);
                    Logger.Info(string.Format("Created payments table for Branch {0}", branchNumber));
                }

                // Recreate indexes
                CreateIndexes(connection);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Check if table exists
        /// </summary>
        private bool TableExists(SQLiteConnection connection, string tableName)
        {
            string sql = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@tableName";
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@tableName", tableName);
                var result = command.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        /// <summary>
        /// Check if column exists in table
        /// </summary>
        private bool ColumnExists(SQLiteConnection connection, string tableName, string columnName)
        {
            string sql = string.Format("PRAGMA table_info({0})", tableName);
            using (var command = new SQLiteCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["name"].ToString().Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Create customers table (based on ACCOUNT.FIL structure)
        /// </summary>
        private void CreateCustomersTable(SQLiteConnection connection)
        {
            string sql = @"
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
                    monthly_income DECIMAL(10,2) DEFAULT 0,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    status TEXT DEFAULT 'Active'
                );";
            
            ExecuteNonQuery(connection, sql);
        }

        /// <summary>
        /// Create vehicles table
        /// </summary>
        private void CreateVehiclesTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE vehicles (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    vehicle_number TEXT UNIQUE NOT NULL,
                    state_code TEXT NOT NULL,
                    series_code TEXT NOT NULL,
                    registration_number TEXT NOT NULL,
                    chassis_number TEXT,
                    engine_number TEXT,
                    make TEXT,
                    model TEXT,
                    year INTEGER,
                    color TEXT,
                    loan_amount DECIMAL(15,2) DEFAULT 0,
                    paid_amount DECIMAL(15,2) DEFAULT 0,
                    balance_amount DECIMAL(15,2) DEFAULT 0,
                    status TEXT DEFAULT 'Active',
                    customer_id INTEGER REFERENCES customers(id),
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
            
            ExecuteNonQuery(connection, sql);
        }

        /// <summary>
        /// Create transactions table (based on CASH.FIL structure)
        /// </summary>
        private void CreateTransactionsTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE transactions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    voucher_number TEXT UNIQUE NOT NULL,
                    transaction_date DATE NOT NULL,
                    vehicle_number TEXT NOT NULL,
                    customer_name TEXT,
                    debit_amount DECIMAL(12,2) DEFAULT 0.00,
                    credit_amount DECIMAL(12,2) DEFAULT 0.00,
                    balance_amount DECIMAL(12,2) DEFAULT 0.00,
                    description TEXT,
                    payment_method TEXT DEFAULT 'cash',
                    reference_number TEXT,
                    customer_id INTEGER REFERENCES customers(id),
                    vehicle_id INTEGER REFERENCES vehicles(id),
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
            
            ExecuteNonQuery(connection, sql);
        }

        /// <summary>
        /// Create system configuration table
        /// </summary>
        private void CreateSystemConfigTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE system_config (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    config_key TEXT UNIQUE NOT NULL,
                    config_value TEXT NOT NULL,
                    description TEXT,
                    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
            
            ExecuteNonQuery(connection, sql);
        }

        /// <summary>
        /// Create loans table for loan management
        /// </summary>
        private void CreateLoansTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS loans (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    loan_number TEXT UNIQUE NOT NULL,
                    customer_id INTEGER NOT NULL REFERENCES customers(id),
                    vehicle_id INTEGER NOT NULL REFERENCES vehicles(id),
                    principal_amount DECIMAL(15,2) NOT NULL,
                    interest_rate DECIMAL(5,2) DEFAULT 0.00,
                    loan_term_months INTEGER NOT NULL,
                    emi_amount DECIMAL(12,2) DEFAULT 0.00,
                    loan_date DATE NOT NULL,
                    maturity_date DATE,
                    total_paid_amount DECIMAL(15,2) DEFAULT 0.00,
                    interest_paid_amount DECIMAL(15,2) DEFAULT 0.00,
                    principal_paid_amount DECIMAL(15,2) DEFAULT 0.00,
                    balance_amount DECIMAL(15,2) DEFAULT 0.00,
                    overdue_days INTEGER DEFAULT 0,
                    penalty_amount DECIMAL(12,2) DEFAULT 0.00,
                    status TEXT DEFAULT 'Active',
                    remarks TEXT,
                    branch_id INTEGER NOT NULL,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
            
            ExecuteNonQuery(connection, sql);
        }

        /// <summary>
        /// Create payments table for payment tracking
        /// </summary>
        private void CreatePaymentsTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS payments (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    payment_number TEXT UNIQUE NOT NULL,
                    voucher_number TEXT,
                    payment_date DATE NOT NULL,
                    loan_id INTEGER NOT NULL REFERENCES loans(id),
                    customer_id INTEGER NOT NULL REFERENCES customers(id),
                    vehicle_id INTEGER NOT NULL REFERENCES vehicles(id),
                    total_amount DECIMAL(12,2) NOT NULL,
                    principal_amount DECIMAL(12,2) DEFAULT 0.00,
                    interest_amount DECIMAL(12,2) DEFAULT 0.00,
                    penalty_amount DECIMAL(12,2) DEFAULT 0.00,
                    payment_method TEXT DEFAULT 'Cash',
                    reference_number TEXT,
                    description TEXT,
                    received_by TEXT,
                    branch_id INTEGER NOT NULL,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
            
            ExecuteNonQuery(connection, sql);
        }

        /// <summary>
        /// Create database indexes for performance
        /// </summary>
        private void CreateIndexes(SQLiteConnection connection)
        {
            string[] indexes = {
                "CREATE INDEX IF NOT EXISTS idx_customers_code ON customers(customer_code);",
                "CREATE INDEX IF NOT EXISTS idx_customers_name ON customers(name);",
                "CREATE INDEX IF NOT EXISTS idx_vehicles_number ON vehicles(vehicle_number);",
                "CREATE INDEX IF NOT EXISTS idx_vehicles_customer ON vehicles(customer_id);",
                "CREATE INDEX IF NOT EXISTS idx_loans_number ON loans(loan_number);",
                "CREATE INDEX IF NOT EXISTS idx_loans_customer ON loans(customer_id);",
                "CREATE INDEX IF NOT EXISTS idx_loans_vehicle ON loans(vehicle_id);",
                "CREATE INDEX IF NOT EXISTS idx_loans_status ON loans(status);",
                "CREATE INDEX IF NOT EXISTS idx_loans_overdue ON loans(overdue_days);",
                "CREATE INDEX IF NOT EXISTS idx_payments_number ON payments(payment_number);",
                "CREATE INDEX IF NOT EXISTS idx_payments_loan ON payments(loan_id);",
                "CREATE INDEX IF NOT EXISTS idx_payments_customer ON payments(customer_id);",
                "CREATE INDEX IF NOT EXISTS idx_payments_date ON payments(payment_date);",
                "CREATE INDEX IF NOT EXISTS idx_transactions_voucher ON transactions(voucher_number);",
                "CREATE INDEX IF NOT EXISTS idx_transactions_date ON transactions(transaction_date);",
                "CREATE INDEX IF NOT EXISTS idx_transactions_vehicle ON transactions(vehicle_number);",
                "CREATE INDEX IF NOT EXISTS idx_transactions_customer ON transactions(customer_id);"
            };

            foreach (string indexSql in indexes)
            {
                ExecuteNonQuery(connection, indexSql);
            }
        }

        /// <summary>
        /// Insert default configuration values
        /// </summary>
        private void InsertDefaultConfiguration(SQLiteConnection connection, int branchNumber)
        {
            var configs = new Dictionary<string, string>
            {
                { "db_version", "1.0" },
                { "branch_number", branchNumber.ToString() },
                { "company_name", ConfigurationManager.AppSettings["CompanyName"] ?? "XYZ 01-01-2000" },
                { "last_voucher_number", "0" },
                { "created_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
            };

            foreach (var config in configs)
            {
                string sql = @"
                    INSERT INTO system_config (config_key, config_value, description) 
                    VALUES (@key, @value, @description);";

                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@key", config.Key);
                    cmd.Parameters.AddWithValue("@value", config.Value);
                    cmd.Parameters.AddWithValue("@description", $"Default configuration for {config.Key}");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Execute a non-query SQL command
        /// </summary>
        private void ExecuteNonQuery(SQLiteConnection connection, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Test connection to a branch database
        /// </summary>
        public bool TestConnection(int branchNumber)
        {
            try
            {
                var connection = GetConnection(branchNumber);
                connection.Open();
                
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM system_config;", connection))
                {
                    var result = cmd.ExecuteScalar();
                    return result != null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Connection test failed for Branch {0}: {1}", branchNumber, ex.Message));
                return false;
            }
            finally
            {
                try
                {
                    GetConnection(branchNumber).Close();
                }
                catch { }
            }
        }

        /// <summary>
        /// Get next voucher number for a branch
        /// </summary>
        public int GetNextVoucherNumber(int branchNumber)
        {
            var connection = GetConnection(branchNumber);
            
            try
            {
                connection.Open();
                
                using (var cmd = new SQLiteCommand(
                    "SELECT config_value FROM system_config WHERE config_key = 'last_voucher_number';", 
                    connection))
                {
                    var result = cmd.ExecuteScalar();
                    int lastVoucher = Convert.ToInt32(result ?? "0");
                    int nextVoucher = lastVoucher + 1;
                    
                    // Update the last voucher number
                    using (var updateCmd = new SQLiteCommand(
                        "UPDATE system_config SET config_value = @voucher WHERE config_key = 'last_voucher_number';", 
                        connection))
                    {
                        updateCmd.Parameters.AddWithValue("@voucher", nextVoucher.ToString());
                        updateCmd.ExecuteNonQuery();
                    }
                    
                    return nextVoucher;
                }
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Dispose of all database connections
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                foreach (var connection in branchConnections.Values)
                {
                    try
                    {
                        connection?.Close();
                        connection?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(string.Format("Error disposing database connection: {0}", ex.Message));
                    }
                }
                
                branchConnections.Clear();
                disposed = true;
                
                Logger.Info("Database manager disposed");
            }
        }
    }
}
