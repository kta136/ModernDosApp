using System;
using System.Drawing;
using System.Windows.Forms;
using FocusModern.Data;
using FocusModern.Utilities;

namespace FocusModern.Forms
{
    /// <summary>
    /// Branch selection form - first screen shown to user
    /// </summary>
    public partial class BranchSelectionForm : Form
    {
        private DatabaseManager databaseManager;
        public int SelectedBranch { get; private set; }

        public BranchSelectionForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            try
            {
                // Initialize database manager
                databaseManager = new DatabaseManager();
                databaseManager.InitializeAllBranches();

                // Update branch button states based on database availability
                UpdateBranchButtonStates();
                
                Logger.Info("Branch selection form initialized");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to initialize branch selection form", ex);
                MessageBox.Show($"Error initializing application: {ex.Message}", "FOCUS Modern Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateBranchButtonStates()
        {
            // Test connections and update button states
            for (int i = 1; i <= 3; i++)
            {
                Button branchButton = GetBranchButton(i);
                if (branchButton != null)
                {
                    bool isConnected = databaseManager.TestConnection(i);
                    branchButton.Enabled = isConnected;
                    
                    if (!isConnected)
                    {
                        branchButton.BackColor = Color.LightGray;
                        branchButton.Text = $"Branch {i}\n(Database Error)";
                    }
                }
            }
        }

        private Button GetBranchButton(int branchNumber)
        {
            return branchNumber switch
            {
                1 => btnBranch1,
                2 => btnBranch2,
                3 => btnBranch3,
                _ => null
            };
        }

        private void BranchButton_Click(object sender, EventArgs e)
        {
            try
            {
                Button clickedButton = sender as Button;
                
                if (clickedButton == btnBranch1)
                    SelectedBranch = 1;
                else if (clickedButton == btnBranch2)
                    SelectedBranch = 2;
                else if (clickedButton == btnBranch3)
                    SelectedBranch = 3;

                Logger.Info($"User selected Branch {SelectedBranch}");
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("Error in branch selection", ex);
                MessageBox.Show($"Error selecting branch: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Logger.Info("User chose to exit application");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BranchSelectionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                databaseManager?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error("Error disposing database manager", ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                databaseManager?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}