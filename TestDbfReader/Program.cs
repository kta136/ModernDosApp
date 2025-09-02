using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace TestDbfReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DBF File Reader Test Tool");
            Console.WriteLine("========================");
            
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: TestDbfReader <path-to-dbf-file> [--hex]");
                Console.WriteLine("Example: TestDbfReader C:\\Old\\1\\ACCOUNT.FIL");
                Console.WriteLine("         TestDbfReader C:\\Old\\1\\ACCOUNT.FIL --hex");
                return;
            }
            
            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }
            
            bool showHex = args.Length > 1 && args[1] == "--hex";
            
            try
            {
                Console.WriteLine($"Reading file: {filePath}");
                Console.WriteLine($"File size: {new FileInfo(filePath).Length} bytes");
                Console.WriteLine();
                
                if (showHex)
                {
                    ShowHexDump(filePath, 512); // Show first 512 bytes
                    return;
                }
                
                using (var reader = new SimpleDbfReader(filePath))
                {
                    Console.WriteLine("=== FILE HEADER ===");
                    Console.WriteLine($"File Type: 0x{reader.FileType:X2}");
                    Console.WriteLine($"Last Update: {reader.LastUpdate:yyyy-MM-dd}");
                    Console.WriteLine($"Record Count: {reader.RecordCount}");
                    Console.WriteLine($"Header Length: {reader.HeaderLength}");
                    Console.WriteLine($"Record Length: {reader.RecordLength}");
                    Console.WriteLine();
                    
                    Console.WriteLine("=== FIELDS ===");
                    for (int i = 0; i < reader.Fields.Count; i++)
                    {
                        var field = reader.Fields[i];
                        Console.WriteLine($"{i+1,2}. {field.Name,-12} {field.Type} {field.Length,3} {field.DecimalCount,2}");
                    }
                    Console.WriteLine();
                    
                    Console.WriteLine("=== SAMPLE RECORDS (first 10) ===");
                    int recordNum = 0;
                    foreach (var record in reader.ReadRecords())
                    {
                        recordNum++;
                        Console.WriteLine($"Record #{recordNum}:");
                        foreach (var field in reader.Fields)
                        {
                            var value = record.ContainsKey(field.Name) ? record[field.Name] : null;
                            var displayValue = value?.ToString() ?? "(null)";
                            if (displayValue.Length > 50) displayValue = displayValue.Substring(0, 50) + "...";
                            Console.WriteLine($"  {field.Name,-12}: {displayValue}");
                        }
                        Console.WriteLine();
                        
                        if (recordNum >= 10) break; // Only show first 10 records
                    }
                    
                    Console.WriteLine($"Showed {Math.Min(recordNum, 10)} of {reader.RecordCount} total records.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading DBF file: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            // Don't wait for keypress in automated environments
            if (Environment.UserInteractive)
            {
                Console.WriteLine("\nPress any key to exit...");
                try { Console.ReadKey(); } catch { }
            }
        }
        
        static void ShowHexDump(string filePath, int maxBytes)
        {
            Console.WriteLine("=== HEX DUMP ===");
            var bytes = File.ReadAllBytes(filePath);
            int count = Math.Min(maxBytes, bytes.Length);
            
            for (int i = 0; i < count; i += 16)
            {
                // Address
                Console.Write($"{i:X8}  ");
                
                // Hex bytes
                for (int j = 0; j < 16; j++)
                {
                    if (i + j < count)
                        Console.Write($"{bytes[i + j]:X2} ");
                    else
                        Console.Write("   ");
                        
                    if (j == 7) Console.Write(" ");
                }
                
                Console.Write(" |");
                
                // ASCII
                for (int j = 0; j < 16 && i + j < count; j++)
                {
                    byte b = bytes[i + j];
                    Console.Write(b >= 32 && b <= 126 ? (char)b : '.');
                }
                
                Console.WriteLine("|");
            }
        }
    }
    
    public class SimpleDbfReader : IDisposable
    {
        private readonly BinaryReader reader;
        private readonly List<DbfField> fields;
        
        public byte FileType { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public int RecordCount { get; private set; }
        public short HeaderLength { get; private set; }
        public short RecordLength { get; private set; }
        public List<DbfField> Fields => fields;
        
        public SimpleDbfReader(string filePath)
        {
            reader = new BinaryReader(File.OpenRead(filePath));
            fields = new List<DbfField>();
            ReadHeader();
            ReadFieldDescriptors();
        }
        
        private void ReadHeader()
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            
            FileType = reader.ReadByte();
            
            // Last update date (YY MM DD format, year is offset from 1900)
            byte year = reader.ReadByte();
            byte month = reader.ReadByte();
            byte day = reader.ReadByte();
            
            try
            {
                LastUpdate = new DateTime(1900 + year, month, day);
            }
            catch
            {
                LastUpdate = DateTime.MinValue;
            }
            
            RecordCount = reader.ReadInt32();
            HeaderLength = reader.ReadInt16();
            RecordLength = reader.ReadInt16();
            
            // Skip reserved bytes (20 bytes)
            reader.BaseStream.Seek(20, SeekOrigin.Current);
        }
        
        private void ReadFieldDescriptors()
        {
            reader.BaseStream.Seek(32, SeekOrigin.Begin); // Start of field descriptors
            
            while (reader.BaseStream.Position < HeaderLength - 1)
            {
                // Read field name (11 bytes, null-terminated)
                var nameBytes = reader.ReadBytes(11);
                string name = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');
                
                char type = (char)reader.ReadByte();
                int dataAddress = reader.ReadInt32(); // Field data address (not used in DBF files)
                byte length = reader.ReadByte();
                byte decimalCount = reader.ReadByte();
                
                // Skip reserved bytes (14 bytes)
                reader.BaseStream.Seek(14, SeekOrigin.Current);
                
                fields.Add(new DbfField
                {
                    Name = name,
                    Type = type,
                    DataAddress = dataAddress,
                    Length = length,
                    DecimalCount = decimalCount
                });
            }
        }
        
        public IEnumerable<Dictionary<string, object>> ReadRecords()
        {
            // Position at start of data records
            reader.BaseStream.Seek(HeaderLength, SeekOrigin.Begin);
            
            for (int i = 0; i < RecordCount; i++)
            {
                var record = ReadRecord();
                if (record != null)
                    yield return record;
            }
        }
        
        private Dictionary<string, object> ReadRecord()
        {
            if (reader.BaseStream.Position >= reader.BaseStream.Length)
                return null;
                
            var deletedFlag = reader.ReadByte();
            if (deletedFlag == 0x2A) // '*' indicates deleted record
            {
                // Skip the rest of this record
                reader.BaseStream.Seek(RecordLength - 1, SeekOrigin.Current);
                return null; // Don't return deleted records
            }
            
            var record = new Dictionary<string, object>();
            
            foreach (var field in fields)
            {
                var rawBytes = reader.ReadBytes(field.Length);
                var value = ParseFieldValue(field, rawBytes);
                record[field.Name] = value;
            }
            
            return record;
        }
        
        private object ParseFieldValue(DbfField field, byte[] rawBytes)
        {
            // Try different encodings
            string stringValue = null;
            try
            {
                // Try Windows-1252 first (common for older databases)
                stringValue = Encoding.GetEncoding(1252).GetString(rawBytes).TrimEnd('\0', ' ');
            }
            catch
            {
                try
                {
                    // Fall back to ASCII
                    stringValue = Encoding.ASCII.GetString(rawBytes).TrimEnd('\0', ' ');
                }
                catch
                {
                    // Last resort - just show raw bytes as hex
                    return BitConverter.ToString(rawBytes);
                }
            }
            
            switch (field.Type)
            {
                case 'C': // Character
                    return stringValue;
                    
                case 'N': // Numeric
                    if (string.IsNullOrWhiteSpace(stringValue))
                        return field.DecimalCount > 0 ? (object)0.0m : (object)0;
                    
                    if (field.DecimalCount > 0)
                    {
                        if (decimal.TryParse(stringValue.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal decValue))
                            return decValue;
                        return 0.0m;
                    }
                    else
                    {
                        if (long.TryParse(stringValue.Trim(), out long longValue))
                            return longValue;
                        return 0;
                    }
                    
                case 'D': // Date (YYYYMMDD format)
                    if (string.IsNullOrWhiteSpace(stringValue) || stringValue.Length != 8)
                        return DateTime.MinValue;
                        
                    if (DateTime.TryParseExact(stringValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                        return dateValue;
                    return DateTime.MinValue;
                    
                case 'L': // Logical (Boolean)
                    return stringValue.ToUpperInvariant() == "T" || stringValue == "Y" || stringValue == "1";
                    
                case 'M': // Memo (just return the memo number for now)
                    if (long.TryParse(stringValue.Trim(), out long memoNumber))
                        return memoNumber;
                    return 0L;
                    
                default:
                    return stringValue; // Unknown type, return as string
            }
        }
        
        public void Dispose()
        {
            reader?.Dispose();
        }
    }
    
    public class DbfField
    {
        public string Name { get; set; }
        public char Type { get; set; }
        public int DataAddress { get; set; }
        public byte Length { get; set; }
        public byte DecimalCount { get; set; }
    }
}