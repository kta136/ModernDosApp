using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

namespace FocusModern.Data
{
    /// <summary>
    /// dBase/Clipper .FIL/.DBF file reader that properly parses binary database structure
    /// instead of treating files as text
    /// </summary>
    public class DBaseReader : IDisposable
    {
        private readonly BinaryReader reader;
        private readonly DBaseHeader header;
        private readonly List<DBaseField> fields;
        private bool disposed = false;

        public DBaseReader(string filePath)
        {
            reader = new BinaryReader(File.OpenRead(filePath), Encoding.GetEncoding(1252));
            header = ReadHeader();
            fields = ReadFieldDescriptors();
        }

        public DBaseHeader Header => header;
        public IReadOnlyList<DBaseField> Fields => fields;

        private DBaseHeader ReadHeader()
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            
            var header = new DBaseHeader
            {
                FileType = reader.ReadByte(),
                LastUpdate = new DateTime(
                    reader.ReadByte() + 1900,  // Year stored as offset from 1900
                    reader.ReadByte(),         // Month
                    reader.ReadByte()          // Day
                ),
                RecordCount = reader.ReadInt32(),
                HeaderLength = reader.ReadInt16(),
                RecordLength = reader.ReadInt16()
            };
            
            // Skip reserved bytes
            reader.BaseStream.Seek(20, SeekOrigin.Current);
            
            return header;
        }

        private List<DBaseField> ReadFieldDescriptors()
        {
            var fieldList = new List<DBaseField>();
            reader.BaseStream.Seek(32, SeekOrigin.Begin); // Start of field descriptors
            
            while (reader.BaseStream.Position < header.HeaderLength - 1)
            {
                var field = new DBaseField
                {
                    Name = Encoding.ASCII.GetString(reader.ReadBytes(11)).TrimEnd('\0'),
                    Type = (char)reader.ReadByte(),
                    DataAddress = reader.ReadInt32(),
                    Length = reader.ReadByte(),
                    DecimalCount = reader.ReadByte()
                };
                
                // Skip reserved bytes
                reader.BaseStream.Seek(14, SeekOrigin.Current);
                
                fieldList.Add(field);
            }
            
            return fieldList;
        }

        public IEnumerable<DBaseRecord> ReadRecords()
        {
            // Position at start of data records
            reader.BaseStream.Seek(header.HeaderLength, SeekOrigin.Begin);
            
            for (int i = 0; i < header.RecordCount; i++)
            {
                var record = ReadRecord();
                if (record != null)
                    yield return record;
            }
        }

        private DBaseRecord ReadRecord()
        {
            if (reader.BaseStream.Position >= reader.BaseStream.Length)
                return null;
                
            var deletedFlag = reader.ReadByte();
            if (deletedFlag == 0x2A) // '*' indicates deleted record
            {
                // Skip the rest of this record
                reader.BaseStream.Seek(header.RecordLength - 1, SeekOrigin.Current);
                return null; // Don't return deleted records
            }

            var record = new DBaseRecord();
            
            foreach (var field in fields)
            {
                var rawBytes = reader.ReadBytes(field.Length);
                var value = ParseFieldValue(field, rawBytes);
                record.Values[field.Name] = value;
            }
            
            return record;
        }

        private object ParseFieldValue(DBaseField field, byte[] rawBytes)
        {
            // Convert bytes to string using appropriate encoding
            var stringValue = Encoding.GetEncoding(1252).GetString(rawBytes).TrimEnd('\0', ' ');
            
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
            if (!disposed)
            {
                reader?.Dispose();
                disposed = true;
            }
        }
    }

    public class DBaseHeader
    {
        public byte FileType { get; set; }
        public DateTime LastUpdate { get; set; }
        public int RecordCount { get; set; }
        public short HeaderLength { get; set; }
        public short RecordLength { get; set; }
    }

    public class DBaseField
    {
        public string Name { get; set; }
        public char Type { get; set; }
        public int DataAddress { get; set; }
        public byte Length { get; set; }
        public byte DecimalCount { get; set; }
    }

    public class DBaseRecord
    {
        public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();
        
        public T GetValue<T>(string fieldName, T defaultValue = default(T))
        {
            if (Values.TryGetValue(fieldName, out var value) && value is T typedValue)
                return typedValue;
            return defaultValue;
        }
        
        public string GetString(string fieldName) => GetValue<string>(fieldName)?.Trim() ?? string.Empty;
        public decimal GetDecimal(string fieldName) => GetValue<decimal>(fieldName);
        public long GetLong(string fieldName) => GetValue<long>(fieldName);
        public DateTime GetDateTime(string fieldName) => GetValue<DateTime>(fieldName);
        public bool GetBoolean(string fieldName) => GetValue<bool>(fieldName);
    }
}