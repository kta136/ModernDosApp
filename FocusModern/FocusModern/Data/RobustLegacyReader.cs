using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using FocusModern.Utilities;

namespace FocusModern.Data
{
    /// <summary>
    /// Robust legacy file reader that can handle both proper dBase files and
    /// corrupted/non-standard formats by falling back to heuristic parsing
    /// </summary>
    public class RobustLegacyReader
    {
        private readonly string filePath;

        public RobustLegacyReader(string filePath)
        {
            this.filePath = filePath;
        }

        public IEnumerable<Dictionary<string, object>> ExtractAccountRecords()
        {
            // Try dBase format first
            var dbfRecords = new List<Dictionary<string, object>>();
            bool dbfSuccess = false;
            
            try
            {
                using (var dbfReader = new DBaseReader(filePath))
                {
                    Logger.Info($"Successfully opened as dBase: {dbfReader.Header.RecordCount} records");
                    foreach (var record in dbfReader.ReadRecords())
                    {
                        dbfRecords.Add(ConvertDbfRecord(record));
                    }
                    dbfSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"dBase parsing failed: {ex.Message}, trying heuristic parsing");
            }

            if (dbfSuccess)
            {
                foreach (var record in dbfRecords)
                    yield return record;
            }
            else
            {
                // Fall back to heuristic parsing for corrupted files
                foreach (var record in ParseAccountsHeuristically())
                {
                    yield return record;
                }
            }
        }

        public IEnumerable<Dictionary<string, object>> ExtractCashRecords()
        {
            // Try dBase format first
            var dbfRecords = new List<Dictionary<string, object>>();
            bool dbfSuccess = false;
            
            try
            {
                using (var dbfReader = new DBaseReader(filePath))
                {
                    Logger.Info($"Successfully opened as dBase: {dbfReader.Header.RecordCount} records");
                    foreach (var record in dbfReader.ReadRecords())
                    {
                        dbfRecords.Add(ConvertDbfRecord(record));
                    }
                    dbfSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"dBase parsing failed: {ex.Message}, trying heuristic parsing");
            }

            if (dbfSuccess)
            {
                foreach (var record in dbfRecords)
                    yield return record;
            }
            else
            {
                // Fall back to heuristic parsing for corrupted files
                foreach (var record in ParseCashHeuristically())
                {
                    yield return record;
                }
            }
        }

        private Dictionary<string, object> ConvertDbfRecord(DBaseRecord dbfRecord)
        {
            var result = new Dictionary<string, object>();
            foreach (var kvp in dbfRecord.Values)
            {
                result[kvp.Key] = kvp.Value;
            }
            return result;
        }

        private IEnumerable<Dictionary<string, object>> ParseAccountsHeuristically()
        {
            Logger.Info("Using heuristic parsing for ACCOUNT file");
            
            var bytes = File.ReadAllBytes(filePath);
            var text = TryDecodeBytes(bytes);
            
            if (string.IsNullOrEmpty(text))
            {
                // Try parsing as fixed-width records directly from bytes
                foreach (var record in ParseAccountsFromBytes(bytes))
                {
                    yield return record;
                }
                yield break;
            }

            // Parse from text using patterns
            var lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var record = ParseAccountLine(line);
                if (record != null)
                    yield return record;
            }
        }

        private IEnumerable<Dictionary<string, object>> ParseCashHeuristically()
        {
            Logger.Info("Using heuristic parsing for CASH file");
            
            var bytes = File.ReadAllBytes(filePath);
            
            // CASH files are often very large and might be corrupted
            // Try to extract patterns from the raw bytes
            foreach (var record in ParseCashFromBytes(bytes))
            {
                yield return record;
            }
        }

        private IEnumerable<Dictionary<string, object>> ParseAccountsFromBytes(byte[] bytes)
        {
            var results = new List<Dictionary<string, object>>();
            
            // Based on ACCOUNT.FIL analysis: 49 bytes per record, starting at offset 161
            const int recordLength = 49;
            const int dataStart = 161;
            
            for (int offset = dataStart; offset + recordLength <= bytes.Length; offset += recordLength)
            {
                try
                {
                    // Check if record is deleted (first byte is '*')
                    if (bytes[offset] == 0x2A) continue;

                    // Extract ANAME field (30 bytes starting at offset+1)
                    var nameBytes = new byte[30];
                    Array.Copy(bytes, offset + 1, nameBytes, 0, 30);
                    var nameStr = Encoding.GetEncoding(1252).GetString(nameBytes).Trim('\0', ' ');

                    // Extract OPBAL field (11 bytes starting at offset+31)
                    var balanceBytes = new byte[11];
                    Array.Copy(bytes, offset + 31, balanceBytes, 0, 11);
                    var balanceStr = Encoding.ASCII.GetString(balanceBytes).Trim('\0', ' ');

                    // Extract CD field (1 byte at offset+42)
                    char cd = (char)bytes[offset + 42];

                    // Extract ECODE field (6 bytes starting at offset+43)
                    var ecodeBytes = new byte[6];
                    Array.Copy(bytes, offset + 43, ecodeBytes, 0, 6);
                    var ecodeStr = Encoding.ASCII.GetString(ecodeBytes).Trim('\0', ' ');

                    // Parse customer code and name from ANAME field
                    var (customerCode, customerName) = ParseCustomerFromNameField(nameStr);

                    if (!string.IsNullOrEmpty(customerCode) && !string.IsNullOrEmpty(customerName))
                    {
                        results.Add(new Dictionary<string, object>
                        {
                            ["CustomerCode"] = customerCode,
                            ["CustomerName"] = customerName,
                            ["OpeningBalance"] = ParseDecimal(balanceStr),
                            ["CD"] = cd.ToString(),
                            ["ECODE"] = ParseLong(ecodeStr)
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error parsing account record at offset {offset}: {ex.Message}");
                    continue;
                }
            }
            
            return results;
        }

        private IEnumerable<Dictionary<string, object>> ParseCashFromBytes(byte[] bytes)
        {
            var results = new List<Dictionary<string, object>>();
            
            // Try to find transaction patterns in the raw bytes
            // Look for date patterns (YYYYMMDD), amounts, and vehicle patterns
            
            var text = TryDecodeBytes(bytes);
            if (string.IsNullOrEmpty(text)) return results;

            // Look for transaction patterns using regex
            var patterns = new[]
            {
                @"(?<name>[A-Z]{2,4})\s*\/\s*(?<code>\d{4,6})\s+(?<voucher>\d+)(?<vehicle>CV-\d+|\w{2,3}-\d+)\s+(?<date>\d{8})",
                @"(?<amount>\d+\.\d{2})(?<type>[CD])\s+(?<name>[A-Z]{2,4})\s*\/\s*(?<code>\d{4,6})",
                @"(?<vehicle>\w{2,3}-\d+|\bCV-\d+)\s*\/\s*(?<code>\d{4,6})"
            };

            foreach (var pattern in patterns)
            {
                var regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (Match match in regex.Matches(text))
                {
                    try
                    {
                        var record = new Dictionary<string, object>
                        {
                            ["CustomerName"] = match.Groups["name"].Success ? match.Groups["name"].Value : "",
                            ["CustomerCode"] = match.Groups["code"].Success ? match.Groups["code"].Value : "",
                            ["VehicleNumber"] = match.Groups["vehicle"].Success ? match.Groups["vehicle"].Value : "",
                            ["VoucherNumber"] = match.Groups["voucher"].Success ? match.Groups["voucher"].Value : "",
                            ["Amount"] = match.Groups["amount"].Success ? ParseDecimal(match.Groups["amount"].Value) : 0m,
                            ["TransactionType"] = match.Groups["type"].Success ? match.Groups["type"].Value : "C",
                            ["Date"] = match.Groups["date"].Success ? ParseDate(match.Groups["date"].Value) : DateTime.Now
                        };

                        // Only add records that have some meaningful data
                        if (!string.IsNullOrEmpty(record["CustomerCode"].ToString()) || 
                            !string.IsNullOrEmpty(record["VehicleNumber"].ToString()) ||
                            (decimal)record["Amount"] > 0)
                        {
                            results.Add(record);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error parsing cash record: {ex.Message}");
                        continue;
                    }
                }
            }
            
            return results;
        }

        private string TryDecodeBytes(byte[] bytes)
        {
            var encodings = new[]
            {
                Encoding.GetEncoding(1252), // Windows-1252
                Encoding.ASCII,
                Encoding.UTF8,
                Encoding.GetEncoding(437)   // DOS code page
            };

            foreach (var encoding in encodings)
            {
                try
                {
                    var text = encoding.GetString(bytes);
                    // Check if decoded text looks reasonable
                    if (HasReasonableContent(text))
                        return text;
                }
                catch { }
            }

            return string.Empty;
        }

        private bool HasReasonableContent(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            
            // Check for reasonable ratio of printable characters
            int printableCount = 0;
            int totalCount = Math.Min(1000, text.Length); // Check first 1000 chars
            
            for (int i = 0; i < totalCount; i++)
            {
                char c = text[i];
                if (c >= 32 && c <= 126) printableCount++;
            }
            
            return (double)printableCount / totalCount > 0.7; // At least 70% printable
        }

        private (string code, string name) ParseCustomerFromNameField(string nameField)
        {
            if (string.IsNullOrEmpty(nameField)) return (null, null);

            // Pattern: "UHA    / 5494" -> code=5494, name=UHA
            var match = Regex.Match(nameField, @"^([A-Z0-9-]+)\s*\/\s*(\d{2,8})");
            if (match.Success)
            {
                return (match.Groups[2].Value, match.Groups[1].Value.Trim());
            }

            // Pattern: "5494 UHA" -> code=5494, name=UHA
            match = Regex.Match(nameField, @"^(\d{2,8})\s+([A-Z0-9-]+)");
            if (match.Success)
            {
                return (match.Groups[1].Value, match.Groups[2].Value.Trim());
            }

            return (null, null);
        }

        private Dictionary<string, object> ParseAccountLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;

            var (code, name) = ParseCustomerFromNameField(line);
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(name))
            {
                return new Dictionary<string, object>
                {
                    ["CustomerCode"] = code,
                    ["CustomerName"] = name
                };
            }

            return null;
        }

        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0m;
            return decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result) ? result : 0m;
        }

        private long ParseLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            return long.TryParse(value.Trim(), out long result) ? result : 0;
        }

        private DateTime ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return DateTime.MinValue;
            
            // Try YYYYMMDD format first
            if (value.Length == 8 && DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                return result;
                
            // Try other common formats
            if (DateTime.TryParse(value, out result))
                return result;
                
            return DateTime.MinValue;
        }
    }
}