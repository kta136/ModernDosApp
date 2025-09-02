namespace FocusModern.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlBranchHeader;
        private System.Windows.Forms.Label lblCurrentBranch;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vehicleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loansToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paymentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnCustomers;
        private System.Windows.Forms.ToolStripButton btnVehicles;
        private System.Windows.Forms.ToolStripButton btnLoans;
        private System.Windows.Forms.ToolStripButton btnPayments;
        private System.Windows.Forms.ToolStripButton btnReports;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.Panel pnlDashboard;
        private System.Windows.Forms.GroupBox gbSummary;
        private System.Windows.Forms.Label lblTotalCustomersLabel;
        private System.Windows.Forms.Label lblTotalCustomers;
        private System.Windows.Forms.Label lblActiveLoansLabel;
        private System.Windows.Forms.Label lblActiveLoans;
        private System.Windows.Forms.Label lblTodaysPaymentsLabel;
        private System.Windows.Forms.Label lblTodaysPayments;
        private System.Windows.Forms.GroupBox gbRecentTransactions;
        private System.Windows.Forms.ListView lvRecentTransactions;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusSpacer;
        private System.Windows.Forms.Button btnSwitchBranch;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnSettings;

        private void InitializeComponent()
        {
            this.pnlBranchHeader = new System.Windows.Forms.Panel();
            this.lblCurrentBranch = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vehicleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loansToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paymentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnCustomers = new System.Windows.Forms.ToolStripButton();
            this.btnVehicles = new System.Windows.Forms.ToolStripButton();
            this.btnLoans = new System.Windows.Forms.ToolStripButton();
            this.btnPayments = new System.Windows.Forms.ToolStripButton();
            this.btnReports = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.pnlDashboard = new System.Windows.Forms.Panel();
            this.gbSummary = new System.Windows.Forms.GroupBox();
            this.lblTodaysPayments = new System.Windows.Forms.Label();
            this.lblTodaysPaymentsLabel = new System.Windows.Forms.Label();
            this.lblActiveLoans = new System.Windows.Forms.Label();
            this.lblActiveLoansLabel = new System.Windows.Forms.Label();
            this.lblTotalCustomers = new System.Windows.Forms.Label();
            this.lblTotalCustomersLabel = new System.Windows.Forms.Label();
            this.gbRecentTransactions = new System.Windows.Forms.GroupBox();
            this.lvRecentTransactions = new System.Windows.Forms.ListView();
            this.btnSwitchBranch = new System.Windows.Forms.Button();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatusSpacer = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlBranchHeader.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.pnlDashboard.SuspendLayout();
            this.gbSummary.SuspendLayout();
            this.gbRecentTransactions.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBranchHeader
            // 
            this.pnlBranchHeader.BackColor = System.Drawing.Color.LightBlue;
            this.pnlBranchHeader.Controls.Add(this.lblCurrentBranch);
            this.pnlBranchHeader.Controls.Add(this.lblTitle);
            this.pnlBranchHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBranchHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlBranchHeader.Name = "pnlBranchHeader";
            this.pnlBranchHeader.Size = new System.Drawing.Size(1000, 60);
            this.pnlBranchHeader.TabIndex = 0;
            // 
            // lblCurrentBranch
            // 
            this.lblCurrentBranch.AutoSize = true;
            this.lblCurrentBranch.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentBranch.Location = new System.Drawing.Point(800, 18);
            this.lblCurrentBranch.Name = "lblCurrentBranch";
            this.lblCurrentBranch.Size = new System.Drawing.Size(100, 26);
            this.lblCurrentBranch.TabIndex = 1;
            this.lblCurrentBranch.Text = "Branch 1";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(390, 29);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "FOCUS Modern - Dashboard";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.customerToolStripMenuItem,
            this.vehicleToolStripMenuItem,
            this.loansToolStripMenuItem,
            this.paymentsToolStripMenuItem,
            this.reportsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 60);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1000, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // customerToolStripMenuItem
            // 
            this.customerToolStripMenuItem.Name = "customerToolStripMenuItem";
            this.customerToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.customerToolStripMenuItem.Text = "&Customer";
            this.customerToolStripMenuItem.Click += new System.EventHandler(this.btnCustomers_Click);
            // 
            // vehicleToolStripMenuItem
            // 
            this.vehicleToolStripMenuItem.Name = "vehicleToolStripMenuItem";
            this.vehicleToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.vehicleToolStripMenuItem.Text = "&Vehicle";
            this.vehicleToolStripMenuItem.Click += new System.EventHandler(this.btnVehicles_Click);
            // 
            // loansToolStripMenuItem
            // 
            this.loansToolStripMenuItem.Name = "loansToolStripMenuItem";
            this.loansToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.loansToolStripMenuItem.Text = "&Loans";
            this.loansToolStripMenuItem.Click += new System.EventHandler(this.btnLoans_Click);
            // 
            // paymentsToolStripMenuItem
            // 
            this.paymentsToolStripMenuItem.Name = "paymentsToolStripMenuItem";
            this.paymentsToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.paymentsToolStripMenuItem.Text = "&Payments";
            this.paymentsToolStripMenuItem.Click += new System.EventHandler(this.btnPayments_Click);
            // 
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            this.reportsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.reportsToolStripMenuItem.Text = "&Reports";
            this.reportsToolStripMenuItem.Click += new System.EventHandler(this.btnReports_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCustomers,
            this.btnVehicles,
            this.btnLoans,
            this.btnPayments,
            this.btnReports,
            this.toolStripSeparator1,
            this.btnRefresh});
            this.toolStrip.Location = new System.Drawing.Point(0, 84);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1000, 25);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnCustomers
            // 
            this.btnCustomers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.Size = new System.Drawing.Size(78, 22);
            this.btnCustomers.Text = "Customers (F2)";
            this.btnCustomers.Click += new System.EventHandler(this.btnCustomers_Click);
            // 
            // btnVehicles
            // 
            this.btnVehicles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnVehicles.Name = "btnVehicles";
            this.btnVehicles.Size = new System.Drawing.Size(64, 22);
            this.btnVehicles.Text = "Vehicles (F3)";
            this.btnVehicles.Click += new System.EventHandler(this.btnVehicles_Click);
            // 
            // btnLoans
            // 
            this.btnLoans.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLoans.Name = "btnLoans";
            this.btnLoans.Size = new System.Drawing.Size(58, 22);
            this.btnLoans.Text = "Loans (F4)";
            this.btnLoans.Click += new System.EventHandler(this.btnLoans_Click);
            // 
            // btnPayments
            // 
            this.btnPayments.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPayments.Name = "btnPayments";
            this.btnPayments.Size = new System.Drawing.Size(79, 22);
            this.btnPayments.Text = "Payments (F5)";
            this.btnPayments.Click += new System.EventHandler(this.btnPayments_Click);
            // 
            // btnReports
            // 
            this.btnReports.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnReports.Name = "btnReports";
            this.btnReports.Size = new System.Drawing.Size(66, 22);
            this.btnReports.Text = "Reports (F6)";
            this.btnReports.Click += new System.EventHandler(this.btnReports_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // pnlDashboard
            // 
            this.pnlDashboard.Controls.Add(this.gbRecentTransactions);
            this.pnlDashboard.Controls.Add(this.gbSummary);
            this.pnlDashboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDashboard.Location = new System.Drawing.Point(0, 109);
            this.pnlDashboard.Name = "pnlDashboard";
            this.pnlDashboard.Size = new System.Drawing.Size(1000, 419);
            this.pnlDashboard.TabIndex = 3;
            // 
            // gbSummary
            // 
            this.gbSummary.Controls.Add(this.lblTodaysPayments);
            this.gbSummary.Controls.Add(this.lblTodaysPaymentsLabel);
            this.gbSummary.Controls.Add(this.lblActiveLoans);
            this.gbSummary.Controls.Add(this.lblActiveLoansLabel);
            this.gbSummary.Controls.Add(this.lblTotalCustomers);
            this.gbSummary.Controls.Add(this.lblTotalCustomersLabel);
            this.gbSummary.Location = new System.Drawing.Point(20, 20);
            this.gbSummary.Name = "gbSummary";
            this.gbSummary.Size = new System.Drawing.Size(960, 100);
            this.gbSummary.TabIndex = 0;
            this.gbSummary.TabStop = false;
            this.gbSummary.Text = "Summary";
            // 
            // lblTodaysPayments
            // 
            this.lblTodaysPayments.AutoSize = true;
            this.lblTodaysPayments.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTodaysPayments.ForeColor = System.Drawing.Color.Green;
            this.lblTodaysPayments.Location = new System.Drawing.Point(700, 50);
            this.lblTodaysPayments.Name = "lblTodaysPayments";
            this.lblTodaysPayments.Size = new System.Drawing.Size(60, 24);
            this.lblTodaysPayments.TabIndex = 5;
            this.lblTodaysPayments.Text = "0.00";
            // 
            // lblTodaysPaymentsLabel
            // 
            this.lblTodaysPaymentsLabel.AutoSize = true;
            this.lblTodaysPaymentsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTodaysPaymentsLabel.Location = new System.Drawing.Point(700, 30);
            this.lblTodaysPaymentsLabel.Name = "lblTodaysPaymentsLabel";
            this.lblTodaysPaymentsLabel.Size = new System.Drawing.Size(120, 17);
            this.lblTodaysPaymentsLabel.TabIndex = 4;
            this.lblTodaysPaymentsLabel.Text = "Today\'s Payments";
            // 
            // lblActiveLoans
            // 
            this.lblActiveLoans.AutoSize = true;
            this.lblActiveLoans.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveLoans.ForeColor = System.Drawing.Color.Blue;
            this.lblActiveLoans.Location = new System.Drawing.Point(400, 50);
            this.lblActiveLoans.Name = "lblActiveLoans";
            this.lblActiveLoans.Size = new System.Drawing.Size(21, 24);
            this.lblActiveLoans.TabIndex = 3;
            this.lblActiveLoans.Text = "0";
            // 
            // lblActiveLoansLabel
            // 
            this.lblActiveLoansLabel.AutoSize = true;
            this.lblActiveLoansLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveLoansLabel.Location = new System.Drawing.Point(400, 30);
            this.lblActiveLoansLabel.Name = "lblActiveLoansLabel";
            this.lblActiveLoansLabel.Size = new System.Drawing.Size(90, 17);
            this.lblActiveLoansLabel.TabIndex = 2;
            this.lblActiveLoansLabel.Text = "Active Loans";
            // 
            // lblTotalCustomers
            // 
            this.lblTotalCustomers.AutoSize = true;
            this.lblTotalCustomers.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCustomers.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTotalCustomers.Location = new System.Drawing.Point(100, 50);
            this.lblTotalCustomers.Name = "lblTotalCustomers";
            this.lblTotalCustomers.Size = new System.Drawing.Size(21, 24);
            this.lblTotalCustomers.TabIndex = 1;
            this.lblTotalCustomers.Text = "0";
            // 
            // lblTotalCustomersLabel
            // 
            this.lblTotalCustomersLabel.AutoSize = true;
            this.lblTotalCustomersLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCustomersLabel.Location = new System.Drawing.Point(100, 30);
            this.lblTotalCustomersLabel.Name = "lblTotalCustomersLabel";
            this.lblTotalCustomersLabel.Size = new System.Drawing.Size(108, 17);
            this.lblTotalCustomersLabel.TabIndex = 0;
            this.lblTotalCustomersLabel.Text = "Total Customers";
            // 
            // gbRecentTransactions
            // 
            this.gbRecentTransactions.Controls.Add(this.lvRecentTransactions);
            this.gbRecentTransactions.Location = new System.Drawing.Point(20, 140);
            this.gbRecentTransactions.Name = "gbRecentTransactions";
            this.gbRecentTransactions.Size = new System.Drawing.Size(960, 200);
            this.gbRecentTransactions.TabIndex = 1;
            this.gbRecentTransactions.TabStop = false;
            this.gbRecentTransactions.Text = "Recent Transactions";
            // 
            // lvRecentTransactions
            // 
            this.lvRecentTransactions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRecentTransactions.FullRowSelect = true;
            this.lvRecentTransactions.GridLines = true;
            this.lvRecentTransactions.Location = new System.Drawing.Point(3, 16);
            this.lvRecentTransactions.Name = "lvRecentTransactions";
            this.lvRecentTransactions.Size = new System.Drawing.Size(954, 181);
            this.lvRecentTransactions.TabIndex = 0;
            this.lvRecentTransactions.UseCompatibleStateImageBehavior = false;
            this.lvRecentTransactions.View = System.Windows.Forms.View.Details;
            // 
            // btnSwitchBranch
            // 
            this.btnSwitchBranch.BackColor = System.Drawing.Color.LightYellow;
            this.btnSwitchBranch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSwitchBranch.Location = new System.Drawing.Point(20, 360);
            this.btnSwitchBranch.Name = "btnSwitchBranch";
            this.btnSwitchBranch.Size = new System.Drawing.Size(100, 30);
            this.btnSwitchBranch.TabIndex = 2;
            this.btnSwitchBranch.Text = "Switch Branch";
            this.btnSwitchBranch.UseVisualStyleBackColor = false;
            this.btnSwitchBranch.Click += new System.EventHandler(this.btnSwitchBranch_Click);
            // 
            // btnBackup
            // 
            this.btnBackup.BackColor = System.Drawing.Color.LightCyan;
            this.btnBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackup.Location = new System.Drawing.Point(130, 360);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(100, 30);
            this.btnBackup.TabIndex = 3;
            this.btnBackup.Text = "Backup Data";
            this.btnBackup.UseVisualStyleBackColor = false;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.BackColor = System.Drawing.Color.LightGray;
            this.btnSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.Location = new System.Drawing.Point(880, 360);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(100, 30);
            this.btnSettings.TabIndex = 4;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblStatusSpacer});
            this.statusStrip.Location = new System.Drawing.Point(0, 528);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1000, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 17);
            this.lblStatus.Text = "Ready";
            // 
            // lblStatusSpacer
            // 
            this.lblStatusSpacer.Name = "lblStatusSpacer";
            this.lblStatusSpacer.Size = new System.Drawing.Size(946, 17);
            this.lblStatusSpacer.Spring = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 550);
            this.Controls.Add(this.pnlDashboard);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.btnSwitchBranch);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.pnlBranchHeader);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FOCUS Modern";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.pnlBranchHeader.ResumeLayout(false);
            this.pnlBranchHeader.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.pnlDashboard.ResumeLayout(false);
            this.gbSummary.ResumeLayout(false);
            this.gbSummary.PerformLayout();
            this.gbRecentTransactions.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}