using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using FocusModern.Utilities;
using System.Text.RegularExpressions;
using FocusModern.Data.Repositories;

namespace FocusModern.Data
{
    public class LegacyImporter
    {
        private readonly DatabaseManager dbManager;
        private readonly int branchId;

        public LegacyImporter(DatabaseManager manager, int branchNumber)
        {
            dbManager = manager;
            branchId = branchNumber;
        }

        public void EnsureLegacyTables()
        {
            dbManager.InitializeBranchDatabase(branchId);
            var conn = dbManager.GetConnection(branchId);
            conn.Open();
            try
            {
                Exec(conn, "CREATE TABLE IF NOT EXISTS legacy_files (id INTEGER PRIMARY KEY AUTOINCREMENT, branch_id INTEGER, file_name TEXT, file_path TEXT, file_size INTEGER, file_type TEXT, imported_at DATETIME DEFAULT CURRENT_TIMESTAMP);");
                Exec(conn, "CREATE TABLE IF NOT EXISTS legacy_raw (id INTEGER PRIMARY KEY AUTOINCREMENT, legacy_file_id INTEGER, line_no INTEGER, content TEXT);");
                Exec(conn, "CREATE TABLE IF NOT EXISTS legacy_payloads (legacy_file_id INTEGER PRIMARY KEY, content BLOB);");
            }
            finally
            {
                conn.Close();
            }
        }

        private static void Exec(SQLiteConnection conn, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn)) { cmd.ExecuteNonQuery(); }
        }

        public int ImportFiles(IEnumerable<string> filePaths)
        {
            EnsureLegacyTables();
            dbManager.InitializeBranchDatabase(branchId);
            int importedFiles = 0;
            var conn = dbManager.GetConnection(branchId);
            conn.Open();
            try
            {
                foreach (var path in filePaths)
                {
                    var fi = new FileInfo(path);
                    long legacyFileId;
                    using (var insert = new SQLiteCommand(@"
                        INSERT INTO legacy_files (branch_id, file_name, file_path, file_size, file_type)
                        VALUES (@b, @n, @p, @s, @t);
                        SELECT last_insert_rowid();
                    ", conn))
                    {
                        insert.Parameters.AddWithValue("@b", branchId);
                        insert.Parameters.AddWithValue("@n", fi.Name);
                        insert.Parameters.AddWithValue("@p", fi.FullName);
                        insert.Parameters.AddWithValue("@s", fi.Length);
                        insert.Parameters.AddWithValue("@t", fi.Extension.Trim('.').ToUpperInvariant());
                        legacyFileId = (long)(insert.ExecuteScalar() ?? 0L);
                    }

                    // Always capture full payload bytes (for .FIL/.DAT etc.)
                    try
                    {
                        var bytes = File.ReadAllBytes(fi.FullName);
                        using (var pb = new SQLiteCommand("INSERT OR REPLACE INTO legacy_payloads (legacy_file_id, content) VALUES (@id, @blob);", conn))
                        {
                            pb.Parameters.AddWithValue("@id", legacyFileId);
                            var p = pb.Parameters.Add("@blob", System.Data.DbType.Binary);
                            p.Value = bytes;
                            pb.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to capture payload for {fi.Name}: {ex.Message}");
                    }

                    // Try to read as text with best-effort encoding detection
                    if (IsLikelyText(fi.FullName))
                    {
                        int lineNo = 0;
                        foreach (var line in ReadAllLinesBestEffort(fi.FullName))
                        {
                            lineNo++;
                            using (var ins = new SQLiteCommand("INSERT INTO legacy_raw (legacy_file_id, line_no, content) VALUES (@f, @l, @c);", conn))
                            {
                                ins.Parameters.AddWithValue("@f", legacyFileId);
                                ins.Parameters.AddWithValue("@l", lineNo);
                                ins.Parameters.AddWithValue("@c", line);
                                ins.ExecuteNonQuery();
                            }
                        }
                    }

                    importedFiles++;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Legacy import failed: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                conn.Close();
            }
            return importedFiles;
        }

        private static bool IsLikelyText(string path)
        {
            try
            {
                using (var fs = File.OpenRead(path))
                {
                    int toRead = (int)Math.Min(2048, fs.Length);
                    var buffer = new byte[toRead];
                    fs.Read(buffer, 0, toRead);
                    // Heuristic: if there are null bytes or too many control characters, treat as binary
                    int nulls = buffer.Count(b => b == 0);
                    int controls = buffer.Count(b => b < 9 || (b > 13 && b < 32));
                    return nulls == 0 && controls < buffer.Length / 10; // <10% controls
                }
            }
            catch { return false; }
        }

        private static IEnumerable<string> ReadAllLinesBestEffort(string path)
        {
            Encoding[] candidates = new[]
            {
                new UTF8Encoding(false, true),
                Encoding.GetEncoding(1252),
                Encoding.ASCII
            };
            foreach (var enc in candidates)
            {
                try
                {
                    return File.ReadAllLines(path, enc);
                }
                catch { }
            }
            // Fallback: read bytes and filter
            try
            {
                var bytes = File.ReadAllBytes(path);
                var text = Encoding.UTF8.GetString(bytes);
                return text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            }
            catch { return Array.Empty<string>(); }
        }

        public int ImportAccountsToCustomers()
        {
            int imported = 0;
            dbManager.InitializeBranchDatabase(branchId);
            var conn = dbManager.GetConnection(branchId);
            conn.Open();
            try
            {
                // Get file path for ACCOUNT.FIL (latest imported)
                string filePath = null;
                using (var cmd = new SQLiteCommand(@"
                    SELECT lf.file_path
                    FROM legacy_files lf
                    WHERE UPPER(lf.file_name) = 'ACCOUNT.FIL'
                      AND lf.branch_id = @b
                    ORDER BY lf.id DESC
                    LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("@b", branchId);
                    filePath = cmd.ExecuteScalar() as string;
                }

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    Logger.Info("ACCOUNT.FIL not found or not accessible");
                    return 0;
                }

                // Use robust reader that can handle both proper dBase and corrupted files
                var robustReader = new RobustLegacyReader(filePath);
                
                foreach (var record in robustReader.ExtractAccountRecords())
                {
                    try
                    {
                        string customerCode = record.ContainsKey("CustomerCode") ? record["CustomerCode"]?.ToString() : null;
                        string customerName = record.ContainsKey("CustomerName") ? record["CustomerName"]?.ToString() : null;

                        // Clean up the data
                        customerCode = customerCode?.Trim();
                        customerName = CollapseSpaces(customerName?.Trim());

                        if (!string.IsNullOrWhiteSpace(customerCode) && !string.IsNullOrWhiteSpace(customerName) && customerCode != "0" && customerCode != "0000")
                        {
                            using (var up = new SQLiteCommand(@"
                                INSERT INTO customers (customer_code, name, created_at, updated_at, status)
                                VALUES (@code, @name, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'Active')
                                ON CONFLICT(customer_code) DO UPDATE SET name = excluded.name, updated_at = CURRENT_TIMESTAMP;", conn))
                            {
                                up.Parameters.AddWithValue("@code", customerCode);
                                up.Parameters.AddWithValue("@name", customerName);
                                up.ExecuteNonQuery();
                                imported++;
                            }
                        }
                    }
                    catch (Exception recordEx)
                    {
                        Logger.Error($"Error processing customer record: {recordEx.Message}");
                        // Continue with next record
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error importing accounts to customers: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                conn.Close();
            }
            return imported;
        }

        private static (string code, string name)? ParseAccountLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            // Look for pattern "<name> / <code>" where code is digits
            int slash = line.IndexOf('/');
            if (slash <= 0 || slash >= line.Length - 1) return null;
            var left = line.Substring(0, slash).Trim();
            var right = line.Substring(slash + 1).Trim();
            // Extract leading digits from right part
            string digits = new string(right.TakeWhile(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(digits)) return null;
            string name = CollapseSpaces(left);
            string code = digits; // keep numeric customer code
            return (code, name);
        }

        private static string CollapseSpaces(string s)
        {
            var sb = new StringBuilder();
            bool lastSpace = false;
            foreach (var ch in s)
            {
                if (char.IsWhiteSpace(ch))
                {
                    if (!lastSpace) { sb.Append(' '); lastSpace = true; }
                }
                else { sb.Append(ch); lastSpace = false; }
            }
            return sb.ToString().Trim();
        }

        private static string TryDecode(byte[] bytes, IEnumerable<Encoding> encodings)
        {
            try { Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); } catch { }
            foreach (var enc in encodings)
            {
                try { return enc.GetString(bytes); } catch { }
            }
            try { return Encoding.UTF8.GetString(bytes); } catch { return string.Empty; }
        }

        public int ImportCashToTransactions()
        {
            int imported = 0;
            dbManager.InitializeBranchDatabase(branchId);
            var conn = dbManager.GetConnection(branchId);
            conn.Open();
            try
            {
                // Get file path for CASH.FIL (latest imported)
                string filePath = null;
                using (var cmd = new SQLiteCommand(@"
                    SELECT lf.file_path
                    FROM legacy_files lf
                    WHERE UPPER(lf.file_name) = 'CASH.FIL'
                      AND lf.branch_id = @b
                    ORDER BY lf.id DESC
                    LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("@b", branchId);
                    filePath = cmd.ExecuteScalar() as string;
                }

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    Logger.Info("CASH.FIL not found or not accessible");
                    return 0;
                }

                var txnRepo = new TransactionRepository(dbManager, branchId);
                var robustReader = new RobustLegacyReader(filePath);

                foreach (var record in robustReader.ExtractCashRecords())
                {
                    try
                    {
                        // Extract transaction data from robust reader
                        DateTime transactionDate = record.ContainsKey("Date") && record["Date"] is DateTime dt && dt > DateTime.MinValue 
                            ? dt : DateTime.Now;
                        decimal amount = record.ContainsKey("Amount") ? Convert.ToDecimal(record["Amount"]) : 0m;
                        string transactionType = record.ContainsKey("TransactionType") ? record["TransactionType"].ToString() : "C";
                        string vehicleNumber = record.ContainsKey("VehicleNumber") ? record["VehicleNumber"].ToString() : "";
                        string customerCode = record.ContainsKey("CustomerCode") ? record["CustomerCode"].ToString() : "";
                        string customerName = record.ContainsKey("CustomerName") ? record["CustomerName"].ToString() : "";
                        string voucherNumber = record.ContainsKey("VoucherNumber") ? record["VoucherNumber"].ToString() : "";

                        // Generate voucher number if not present
                        if (string.IsNullOrEmpty(voucherNumber))
                            voucherNumber = dbManager.GetNextVoucherNumber(branchId).ToString();

                        // Build description
                        var descParts = new List<string>();
                        if (!string.IsNullOrEmpty(customerName)) descParts.Add(customerName);
                        if (!string.IsNullOrEmpty(vehicleNumber)) descParts.Add(vehicleNumber);
                        if (!string.IsNullOrEmpty(customerCode)) descParts.Add($"Code: {customerCode}");
                        string description = string.Join(" - ", descParts);
                        if (string.IsNullOrEmpty(description)) description = "Legacy Cash Transaction";

                        // Only create transaction if we have meaningful data
                        if (amount > 0 || !string.IsNullOrEmpty(vehicleNumber) || !string.IsNullOrEmpty(customerCode))
                        {
                            var txn = new Models.Transaction
                            {
                                VoucherNumber = voucherNumber,
                                TransactionDate = transactionDate,
                                VehicleNumber = vehicleNumber,
                                CustomerName = customerName,
                                DebitAmount = transactionType.ToUpper() == "D" ? amount : 0m,
                                CreditAmount = transactionType.ToUpper() == "C" ? amount : amount, // Default to credit
                                BalanceAmount = 0,
                                Description = description,
                                PaymentMethod = "cash",
                                ReferenceNumber = "CASH.FIL",
                                CustomerId = 0,
                                VehicleId = 0,
                                CreatedAt = DateTime.Now
                            };

                            if (txnRepo.Create(txn)) imported++;
                        }
                    }
                    catch (Exception recordEx)
                    {
                        Logger.Error($"Error processing cash record: {recordEx.Message}");
                        // Continue with next record
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error importing cash file: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                conn.Close();
            }
            return imported;
        }

        private static string ExtractNameLikeToken(string line)
        {
            // Heuristic: first token before '/' is a 2-4 letter code that may represent area/customer group
            int slash = line.IndexOf('/');
            if (slash > 0)
            {
                var left = line.Substring(0, slash).Trim();
                var parts = left.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0) return parts[parts.Length - 1];
            }
            return "";
        }

        /// <summary>
        /// Import vehicles by parsing text reports (OUTPUT.TXT, F.TXT, FOCU.TXT, CMC.TXT)
        /// and extracting vehicle numbers like "UP-25E / T-8036".
        /// Populates vehicles table with unique vehicle_number, and parses state_code, series_code, registration_number.
        /// </summary>
        public int ImportVehiclesFromDayBook()
        {
            int imported = 0;
            dbManager.InitializeBranchDatabase(branchId);
            var conn = dbManager.GetConnection(branchId);
            conn.Open();
            try
            {
                // Collect candidate text files
                var candidates = new List<string>();
                foreach (var name in new[] { "OUTPUT.TXT", "F.TXT", "FOCU.TXT", "CMC.TXT" })
                {
                    string path = null;
                    using (var cmd = new SQLiteCommand(@"
                        SELECT lf.file_path
                        FROM legacy_files lf
                        WHERE UPPER(lf.file_name) = @n
                          AND lf.branch_id = @b
                        ORDER BY lf.id DESC
                        LIMIT 1;", conn))
                    {
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@b", branchId);
                        path = cmd.ExecuteScalar() as string;
                    }
                    if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    {
                        var fb = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Old"), branchId.ToString(), name);
                        if (File.Exists(fb)) path = fb;
                    }
                    if (!string.IsNullOrEmpty(path) && File.Exists(path)) candidates.Add(path);
                }

                if (candidates.Count == 0) { Logger.Info("No text reports found for vehicle import"); return 0; }

                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var path in candidates)
                {
                    foreach (var line in ReadAllLinesBestEffort(path))
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var veh = TryParseVehicleFromDayBookLine(line);
                        if (veh == null) continue;

                        var vehicleNumber = veh.Value.vehicleNumber;
                        if (!seen.Add(vehicleNumber)) continue; // dedupe within this run

                        using (var up = new SQLiteCommand(@"
                            INSERT INTO vehicles (vehicle_number, state_code, series_code, registration_number, status, created_at, updated_at)
                            VALUES (@vn, @st, @sr, @reg, 'Active', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
                            ON CONFLICT(vehicle_number) DO NOTHING;", conn))
                        {
                            up.Parameters.AddWithValue("@vn", vehicleNumber);
                            up.Parameters.AddWithValue("@st", veh.Value.stateCode);
                            up.Parameters.AddWithValue("@sr", veh.Value.seriesCode);
                            up.Parameters.AddWithValue("@reg", veh.Value.registration);
                            var res = up.ExecuteNonQuery();
                            if (res > 0) imported++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error importing vehicles from day book: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                conn.Close();
            }
            return imported;
        }

        private (string vehicleNumber, string stateCode, string seriesCode, string registration)? TryParseVehicleFromDayBookLine(string line)
        {
            // Look for pattern like "UP-25E / T-8036" anywhere in the line
            int slash = line.IndexOf('/');
            if (slash <= 0 || slash >= line.Length - 1) return null;

            // Extract left and right tokens around '/'
            // Allow extra spaces
            var left = line.Substring(0, slash).TrimEnd();
            var right = line.Substring(slash + 1).TrimStart();

            // Pull last whitespace-separated token on the left (vehicle prefix often at line end before slash)
            var leftParts = left.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (leftParts.Length == 0) return null;
            var leftToken = leftParts[leftParts.Length - 1]; // e.g., "UP-25E"

            // Pull first token on right
            var rightParts = right.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (rightParts.Length == 0) return null;
            var rightToken = rightParts[0]; // e.g., "T-8036"

            // Validate token shapes
            if (!leftToken.Contains("-")) return null;
            if (!rightToken.Contains("-")) return null;

            // Derive state code and series code from left token
            var lt = leftToken.Split('-');
            if (lt.Length < 2) return null;
            string state = lt[0].Trim();         // e.g., "UP"
            string series = string.Join("-", lt.Skip(1)).Trim(); // e.g., "25E"

            string reg = rightToken.Trim();      // e.g., "T-8036"
            string full = state + "-" + series + " / " + reg;

            // Basic sanity checks
            if (state.Length < 2 || series.Length == 0 || reg.Length < 2) return null;
            return (full, state, series, reg);
        }
    }
}
