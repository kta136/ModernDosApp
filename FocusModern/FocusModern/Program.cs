using System;
using System.Windows.Forms;
using System.Text;
using FocusModern.Forms;
using FocusModern.Utilities;

namespace FocusModern
{
    /// <summary>
    /// Main entry point for FOCUS Modern application
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Enable code page encodings (e.g., Windows-1252) used by legacy files
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                // Enable visual styles for Windows Forms
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Initialize logging
                Logger.Initialize();
                Logger.Info("FOCUS Modern application starting...");

                // Show branch selection form first
                using (var branchSelectionForm = new BranchSelectionForm())
                {
                    if (branchSelectionForm.ShowDialog() == DialogResult.OK)
                    {
                        int selectedBranch = branchSelectionForm.SelectedBranch;
                        Logger.Info($"Branch {selectedBranch} selected");

                        // Launch main application for selected branch
                        Application.Run(new MainForm(selectedBranch));
                    }
                }

                Logger.Info("FOCUS Modern application closing...");
            }
            catch (Exception ex)
            {
                Logger.Error($"Fatal error in main application: {ex.Message}", ex);
                MessageBox.Show($"A fatal error occurred: {ex.Message}", "FOCUS Modern Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
