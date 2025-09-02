using System;
using System.Data.SQLite;
using System.IO;
using FocusModern.Data;

class Program
{
    static int Main(string[] args)
    {
        try
        {
            // Enable legacy encodings used by DBF files (e.g., Windows-1252)
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            // Defaults
            string legacyDir = args.Length > 0 ? args[0] : Path.Combine("Old", "1");
            int branch = 1;
            for (int i = 0; i < args.Length; i++)
            {
                if ((args[i] == "-b" || args[i] == "--branch") && i + 1 < args.Length && int.TryParse(args[i + 1], out var b))
                {
                    branch = b; i++;
                }
            }

            if (!Directory.Exists(legacyDir))
            {
                Console.Error.WriteLine($"Legacy directory not found: {legacyDir}");
                return 2;
            }

            Console.WriteLine($"Using legacy dir: {Path.GetFullPath(legacyDir)}");
            Console.WriteLine($"Target branch: {branch}");

            using var dbm = new DatabaseManager();
            dbm.InitializeBranchDatabase(branch);

            var importer = new LegacyImporter(dbm, branch);
            importer.EnsureLegacyTables();

            // Stage files
            var candidates = new[] { "ACCOUNT.FIL", "CASH.FIL", "CONTROL.FIL" };
            var files = new System.Collections.Generic.List<string>();
            foreach (var name in candidates)
            {
                var full = Path.Combine(legacyDir, name);
                if (File.Exists(full)) files.Add(full);
            }
            if (files.Count == 0)
            {
                Console.Error.WriteLine("No legacy files found (ACCOUNT.FIL/CASH.FIL/CONTROL.FIL).");
            }
            else
            {
                var staged = importer.ImportFiles(files);
                Console.WriteLine($"Staged {staged} files into legacy_* tables.");
            }

            // Debug: show a few parsed account records
            var accountPath = Path.Combine(legacyDir, "ACCOUNT.FIL");
            if (File.Exists(accountPath))
            {
                Console.WriteLine("Previewing first 5 ACCOUNT records (normalized):");
                var rr = new RobustLegacyReader(accountPath);
                int shown = 0;
                foreach (var rec in rr.ExtractAccountRecords())
                {
                    Console.WriteLine(string.Join(", ", rec.Select(kv => kv.Key+":"+kv.Value)));
                    if (++shown >= 5) break;
                }
            }

            // Import account -> customers
            int cust = importer.ImportAccountsToCustomers();
            Console.WriteLine($"Imported/updated customers: {cust}");

            // Debug: show a few parsed cash records
            var cashPath = Path.Combine(legacyDir, "CASH.FIL");
            if (File.Exists(cashPath))
            {
                Console.WriteLine("Previewing first 5 CASH records (normalized):");
                var rr2 = new RobustLegacyReader(cashPath);
                int shown2 = 0;
                foreach (var rec in rr2.ExtractCashRecords())
                {
                    Console.WriteLine(string.Join(", ", rec.Select(kv => kv.Key+":"+kv.Value)));
                    if (++shown2 >= 5) break;
                }
            }

            // Import cash -> transactions
            int txns = importer.ImportCashToTransactions();
            Console.WriteLine($"Imported transactions: {txns}");

            // Import vehicles from day book text (OUTPUT.TXT)
            int veh = importer.ImportVehiclesFromDayBook();
            Console.WriteLine($"Imported vehicles: {veh}");

            // Show counts
            var conn = dbm.GetConnection(branch);
            conn.Open();
            try
            {
                long c1 = ScalarLong(conn, "SELECT COUNT(*) FROM customers;");
                long c2 = ScalarLong(conn, "SELECT COUNT(*) FROM transactions;");
                long c3 = ScalarLong(conn, "SELECT COUNT(*) FROM vehicles;");
                Console.WriteLine($"Totals -> customers: {c1}, transactions: {c2}, vehicles: {c3}");
            }
            finally { conn.Close(); }

            Console.WriteLine("Done.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Import failed: {ex.Message}\n{ex}");
            return 1;
        }
    }

    static long ScalarLong(SQLiteConnection conn, string sql)
    {
        using var cmd = new SQLiteCommand(sql, conn);
        var obj = cmd.ExecuteScalar();
        return Convert.ToInt64(obj ?? 0);
    }
}
