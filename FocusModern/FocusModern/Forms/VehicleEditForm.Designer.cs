namespace FocusModern.Forms
{
    partial class VehicleEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.GroupBox grpVehicleInfo;
        private System.Windows.Forms.Label lblVehicleNumber;
        private System.Windows.Forms.TextBox txtVehicleNumber;
        private System.Windows.Forms.Label lblMake;
        private System.Windows.Forms.TextBox txtMake;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.NumericUpDown numYear;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.TextBox txtColor;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.GroupBox grpTechnicalInfo;
        private System.Windows.Forms.Label lblChassisNumber;
        private System.Windows.Forms.TextBox txtChassisNumber;
        private System.Windows.Forms.Label lblEngineNumber;
        private System.Windows.Forms.TextBox txtEngineNumber;
        private System.Windows.Forms.GroupBox grpOwnershipInfo;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.ComboBox cmbCustomer;
        private System.Windows.Forms.GroupBox grpFinancialInfo;
        private System.Windows.Forms.Label lblLoanAmount;
        private System.Windows.Forms.NumericUpDown numLoanAmount;
        private System.Windows.Forms.Label lblPaidAmount;
        private System.Windows.Forms.NumericUpDown numPaidAmount;
        private System.Windows.Forms.Label lblBalanceAmount;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.Label lblCreatedInfo;
        private System.Windows.Forms.Label lblUpdatedInfo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.grpVehicleInfo = new System.Windows.Forms.GroupBox();
            this.lblVehicleNumber = new System.Windows.Forms.Label();
            this.txtVehicleNumber = new System.Windows.Forms.TextBox();
            this.lblMake = new System.Windows.Forms.Label();
            this.txtMake = new System.Windows.Forms.TextBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.lblYear = new System.Windows.Forms.Label();
            this.numYear = new System.Windows.Forms.NumericUpDown();
            this.lblColor = new System.Windows.Forms.Label();
            this.txtColor = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.grpTechnicalInfo = new System.Windows.Forms.GroupBox();
            this.lblChassisNumber = new System.Windows.Forms.Label();
            this.txtChassisNumber = new System.Windows.Forms.TextBox();
            this.lblEngineNumber = new System.Windows.Forms.Label();
            this.txtEngineNumber = new System.Windows.Forms.TextBox();
            this.grpOwnershipInfo = new System.Windows.Forms.GroupBox();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.grpFinancialInfo = new System.Windows.Forms.GroupBox();
            this.lblLoanAmount = new System.Windows.Forms.Label();
            this.numLoanAmount = new System.Windows.Forms.NumericUpDown();
            this.lblPaidAmount = new System.Windows.Forms.Label();
            this.numPaidAmount = new System.Windows.Forms.NumericUpDown();
            this.lblBalanceAmount = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.lblCreatedInfo = new System.Windows.Forms.Label();
            this.lblUpdatedInfo = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.grpVehicleInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYear)).BeginInit();
            this.grpTechnicalInfo.SuspendLayout();
            this.grpOwnershipInfo.SuspendLayout();
            this.grpFinancialInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLoanAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPaidAmount)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.pnlInfo.SuspendLayout();
            this.SuspendLayout();

            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(584, 50);
            this.pnlHeader.TabIndex = 0;

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(15, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(126, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Vehicle Entry";

            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.grpVehicleInfo);
            this.pnlMain.Controls.Add(this.grpTechnicalInfo);
            this.pnlMain.Controls.Add(this.grpOwnershipInfo);
            this.pnlMain.Controls.Add(this.grpFinancialInfo);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 50);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(15, 15, 15, 0);
            this.pnlMain.Size = new System.Drawing.Size(584, 422);
            this.pnlMain.TabIndex = 1;

            // 
            // grpVehicleInfo
            // 
            this.grpVehicleInfo.Controls.Add(this.lblVehicleNumber);
            this.grpVehicleInfo.Controls.Add(this.txtVehicleNumber);
            this.grpVehicleInfo.Controls.Add(this.lblMake);
            this.grpVehicleInfo.Controls.Add(this.txtMake);
            this.grpVehicleInfo.Controls.Add(this.lblModel);
            this.grpVehicleInfo.Controls.Add(this.txtModel);
            this.grpVehicleInfo.Controls.Add(this.lblYear);
            this.grpVehicleInfo.Controls.Add(this.numYear);
            this.grpVehicleInfo.Controls.Add(this.lblColor);
            this.grpVehicleInfo.Controls.Add(this.txtColor);
            this.grpVehicleInfo.Controls.Add(this.lblStatus);
            this.grpVehicleInfo.Controls.Add(this.cmbStatus);
            this.grpVehicleInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpVehicleInfo.Location = new System.Drawing.Point(15, 15);
            this.grpVehicleInfo.Name = "grpVehicleInfo";
            this.grpVehicleInfo.Size = new System.Drawing.Size(554, 120);
            this.grpVehicleInfo.TabIndex = 0;
            this.grpVehicleInfo.TabStop = false;
            this.grpVehicleInfo.Text = "Basic Vehicle Information";

            // 
            // lblVehicleNumber
            // 
            this.lblVehicleNumber.AutoSize = true;
            this.lblVehicleNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblVehicleNumber.Location = new System.Drawing.Point(15, 25);
            this.lblVehicleNumber.Name = "lblVehicleNumber";
            this.lblVehicleNumber.Size = new System.Drawing.Size(95, 15);
            this.lblVehicleNumber.TabIndex = 0;
            this.lblVehicleNumber.Text = "Vehicle Number:";

            // 
            // txtVehicleNumber
            // 
            this.txtVehicleNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtVehicleNumber.Location = new System.Drawing.Point(120, 22);
            this.txtVehicleNumber.Name = "txtVehicleNumber";
            this.txtVehicleNumber.PlaceholderText = "e.g., UP-25E/T-8036";
            this.txtVehicleNumber.Size = new System.Drawing.Size(150, 23);
            this.txtVehicleNumber.TabIndex = 1;

            // 
            // lblMake
            // 
            this.lblMake.AutoSize = true;
            this.lblMake.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMake.Location = new System.Drawing.Point(290, 25);
            this.lblMake.Name = "lblMake";
            this.lblMake.Size = new System.Drawing.Size(39, 15);
            this.lblMake.TabIndex = 2;
            this.lblMake.Text = "Make:";

            // 
            // txtMake
            // 
            this.txtMake.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMake.Location = new System.Drawing.Point(340, 22);
            this.txtMake.Name = "txtMake";
            this.txtMake.PlaceholderText = "e.g., Maruti";
            this.txtMake.Size = new System.Drawing.Size(120, 23);
            this.txtMake.TabIndex = 3;

            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblModel.Location = new System.Drawing.Point(15, 55);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(44, 15);
            this.lblModel.TabIndex = 4;
            this.lblModel.Text = "Model:";

            // 
            // txtModel
            // 
            this.txtModel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtModel.Location = new System.Drawing.Point(120, 52);
            this.txtModel.Name = "txtModel";
            this.txtModel.PlaceholderText = "e.g., Swift";
            this.txtModel.Size = new System.Drawing.Size(150, 23);
            this.txtModel.TabIndex = 5;

            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblYear.Location = new System.Drawing.Point(290, 55);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(32, 15);
            this.lblYear.TabIndex = 6;
            this.lblYear.Text = "Year:";

            // 
            // numYear
            // 
            this.numYear.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numYear.Location = new System.Drawing.Point(340, 52);
            this.numYear.Maximum = new decimal(new int[] { 2030, 0, 0, 0 });
            this.numYear.Minimum = new decimal(new int[] { 1950, 0, 0, 0 });
            this.numYear.Name = "numYear";
            this.numYear.Size = new System.Drawing.Size(80, 23);
            this.numYear.TabIndex = 7;
            this.numYear.Value = new decimal(new int[] { 2024, 0, 0, 0 });

            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblColor.Location = new System.Drawing.Point(15, 85);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(39, 15);
            this.lblColor.TabIndex = 8;
            this.lblColor.Text = "Color:";

            // 
            // txtColor
            // 
            this.txtColor.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtColor.Location = new System.Drawing.Point(120, 82);
            this.txtColor.Name = "txtColor";
            this.txtColor.PlaceholderText = "e.g., White";
            this.txtColor.Size = new System.Drawing.Size(150, 23);
            this.txtColor.TabIndex = 9;

            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(290, 85);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(42, 15);
            this.lblStatus.TabIndex = 10;
            this.lblStatus.Text = "Status:";

            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Items.AddRange(new object[] { "Active", "Inactive", "Sold", "Repossessed" });
            this.cmbStatus.Location = new System.Drawing.Point(340, 82);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(120, 23);
            this.cmbStatus.TabIndex = 11;

            // 
            // grpTechnicalInfo
            // 
            this.grpTechnicalInfo.Controls.Add(this.lblChassisNumber);
            this.grpTechnicalInfo.Controls.Add(this.txtChassisNumber);
            this.grpTechnicalInfo.Controls.Add(this.lblEngineNumber);
            this.grpTechnicalInfo.Controls.Add(this.txtEngineNumber);
            this.grpTechnicalInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpTechnicalInfo.Location = new System.Drawing.Point(15, 145);
            this.grpTechnicalInfo.Name = "grpTechnicalInfo";
            this.grpTechnicalInfo.Size = new System.Drawing.Size(554, 80);
            this.grpTechnicalInfo.TabIndex = 1;
            this.grpTechnicalInfo.TabStop = false;
            this.grpTechnicalInfo.Text = "Technical Information";

            // 
            // lblChassisNumber
            // 
            this.lblChassisNumber.AutoSize = true;
            this.lblChassisNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblChassisNumber.Location = new System.Drawing.Point(15, 25);
            this.lblChassisNumber.Name = "lblChassisNumber";
            this.lblChassisNumber.Size = new System.Drawing.Size(95, 15);
            this.lblChassisNumber.TabIndex = 0;
            this.lblChassisNumber.Text = "Chassis Number:";

            // 
            // txtChassisNumber
            // 
            this.txtChassisNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtChassisNumber.Location = new System.Drawing.Point(120, 22);
            this.txtChassisNumber.Name = "txtChassisNumber";
            this.txtChassisNumber.Size = new System.Drawing.Size(200, 23);
            this.txtChassisNumber.TabIndex = 1;

            // 
            // lblEngineNumber
            // 
            this.lblEngineNumber.AutoSize = true;
            this.lblEngineNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEngineNumber.Location = new System.Drawing.Point(15, 50);
            this.lblEngineNumber.Name = "lblEngineNumber";
            this.lblEngineNumber.Size = new System.Drawing.Size(93, 15);
            this.lblEngineNumber.TabIndex = 2;
            this.lblEngineNumber.Text = "Engine Number:";

            // 
            // txtEngineNumber
            // 
            this.txtEngineNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtEngineNumber.Location = new System.Drawing.Point(120, 47);
            this.txtEngineNumber.Name = "txtEngineNumber";
            this.txtEngineNumber.Size = new System.Drawing.Size(200, 23);
            this.txtEngineNumber.TabIndex = 3;

            // 
            // grpOwnershipInfo
            // 
            this.grpOwnershipInfo.Controls.Add(this.lblCustomer);
            this.grpOwnershipInfo.Controls.Add(this.cmbCustomer);
            this.grpOwnershipInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpOwnershipInfo.Location = new System.Drawing.Point(15, 235);
            this.grpOwnershipInfo.Name = "grpOwnershipInfo";
            this.grpOwnershipInfo.Size = new System.Drawing.Size(554, 55);
            this.grpOwnershipInfo.TabIndex = 2;
            this.grpOwnershipInfo.TabStop = false;
            this.grpOwnershipInfo.Text = "Ownership Information";

            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCustomer.Location = new System.Drawing.Point(15, 25);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(48, 15);
            this.lblCustomer.TabIndex = 0;
            this.lblCustomer.Text = "Owner:";

            // 
            // cmbCustomer
            // 
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(120, 22);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(300, 23);
            this.cmbCustomer.TabIndex = 1;

            // 
            // grpFinancialInfo
            // 
            this.grpFinancialInfo.Controls.Add(this.lblLoanAmount);
            this.grpFinancialInfo.Controls.Add(this.numLoanAmount);
            this.grpFinancialInfo.Controls.Add(this.lblPaidAmount);
            this.grpFinancialInfo.Controls.Add(this.numPaidAmount);
            this.grpFinancialInfo.Controls.Add(this.lblBalanceAmount);
            this.grpFinancialInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpFinancialInfo.Location = new System.Drawing.Point(15, 300);
            this.grpFinancialInfo.Name = "grpFinancialInfo";
            this.grpFinancialInfo.Size = new System.Drawing.Size(554, 80);
            this.grpFinancialInfo.TabIndex = 3;
            this.grpFinancialInfo.TabStop = false;
            this.grpFinancialInfo.Text = "Financial Information";

            // 
            // lblLoanAmount
            // 
            this.lblLoanAmount.AutoSize = true;
            this.lblLoanAmount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLoanAmount.Location = new System.Drawing.Point(15, 25);
            this.lblLoanAmount.Name = "lblLoanAmount";
            this.lblLoanAmount.Size = new System.Drawing.Size(83, 15);
            this.lblLoanAmount.TabIndex = 0;
            this.lblLoanAmount.Text = "Loan Amount:";

            // 
            // numLoanAmount
            // 
            this.numLoanAmount.DecimalPlaces = 2;
            this.numLoanAmount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numLoanAmount.Location = new System.Drawing.Point(120, 22);
            this.numLoanAmount.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            this.numLoanAmount.Name = "numLoanAmount";
            this.numLoanAmount.Size = new System.Drawing.Size(120, 23);
            this.numLoanAmount.TabIndex = 1;
            this.numLoanAmount.ThousandsSeparator = true;
            this.numLoanAmount.ValueChanged += new System.EventHandler(this.numLoanAmount_ValueChanged);

            // 
            // lblPaidAmount
            // 
            this.lblPaidAmount.AutoSize = true;
            this.lblPaidAmount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPaidAmount.Location = new System.Drawing.Point(260, 25);
            this.lblPaidAmount.Name = "lblPaidAmount";
            this.lblPaidAmount.Size = new System.Drawing.Size(80, 15);
            this.lblPaidAmount.TabIndex = 2;
            this.lblPaidAmount.Text = "Paid Amount:";

            // 
            // numPaidAmount
            // 
            this.numPaidAmount.DecimalPlaces = 2;
            this.numPaidAmount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numPaidAmount.Location = new System.Drawing.Point(350, 22);
            this.numPaidAmount.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            this.numPaidAmount.Name = "numPaidAmount";
            this.numPaidAmount.Size = new System.Drawing.Size(120, 23);
            this.numPaidAmount.TabIndex = 3;
            this.numPaidAmount.ThousandsSeparator = true;
            this.numPaidAmount.ValueChanged += new System.EventHandler(this.numPaidAmount_ValueChanged);

            // 
            // lblBalanceAmount
            // 
            this.lblBalanceAmount.AutoSize = true;
            this.lblBalanceAmount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblBalanceAmount.ForeColor = System.Drawing.Color.Red;
            this.lblBalanceAmount.Location = new System.Drawing.Point(15, 52);
            this.lblBalanceAmount.Name = "lblBalanceAmount";
            this.lblBalanceAmount.Size = new System.Drawing.Size(86, 15);
            this.lblBalanceAmount.TabIndex = 4;
            this.lblBalanceAmount.Text = "Balance: ₹0.00";

            // 
            // pnlButtons
            // 
            this.pnlButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 472);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(584, 45);
            this.pnlButtons.TabIndex = 2;

            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(125)))), ((int)(((byte)(50)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(15, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 25);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save Vehicle (F10)";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(449, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 25);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel (Esc)";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // 
            // pnlInfo
            // 
            this.pnlInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlInfo.Controls.Add(this.lblCreatedInfo);
            this.pnlInfo.Controls.Add(this.lblUpdatedInfo);
            this.pnlInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlInfo.Location = new System.Drawing.Point(0, 517);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(584, 30);
            this.pnlInfo.TabIndex = 3;

            // 
            // lblCreatedInfo
            // 
            this.lblCreatedInfo.AutoSize = true;
            this.lblCreatedInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblCreatedInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblCreatedInfo.Location = new System.Drawing.Point(15, 8);
            this.lblCreatedInfo.Name = "lblCreatedInfo";
            this.lblCreatedInfo.Size = new System.Drawing.Size(46, 13);
            this.lblCreatedInfo.TabIndex = 0;
            this.lblCreatedInfo.Text = "Created:";

            // 
            // lblUpdatedInfo
            // 
            this.lblUpdatedInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUpdatedInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblUpdatedInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblUpdatedInfo.Location = new System.Drawing.Point(400, 8);
            this.lblUpdatedInfo.Name = "lblUpdatedInfo";
            this.lblUpdatedInfo.Size = new System.Drawing.Size(169, 13);
            this.lblUpdatedInfo.TabIndex = 1;
            this.lblUpdatedInfo.Text = "Updated:";
            this.lblUpdatedInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // VehicleEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 547);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlInfo);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VehicleEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Vehicle Entry";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.VehicleEditForm_KeyDown);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.grpVehicleInfo.ResumeLayout(false);
            this.grpVehicleInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYear)).EndInit();
            this.grpTechnicalInfo.ResumeLayout(false);
            this.grpTechnicalInfo.PerformLayout();
            this.grpOwnershipInfo.ResumeLayout(false);
            this.grpOwnershipInfo.PerformLayout();
            this.grpFinancialInfo.ResumeLayout(false);
            this.grpFinancialInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLoanAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPaidAmount)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}