namespace FocusModern.Forms
{
    partial class PaymentListForm
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.btnDateRange = new System.Windows.Forms.Button();
            this.cmbPaymentMethodFilter = new System.Windows.Forms.ComboBox();
            this.lblMethodFilter = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnViewPayment = new System.Windows.Forms.Button();
            this.btnNewPayment = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.dgvPayments = new System.Windows.Forms.DataGridView();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblTodayAmount = new System.Windows.Forms.Label();
            this.lblTodayPayments = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblTotalPayments = new System.Windows.Forms.Label();
            this.pnlTop.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).BeginInit();
            this.pnlSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1200, 50);
            this.pnlTop.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(199, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Payment Management";
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlSearch.Controls.Add(this.btnDateRange);
            this.pnlSearch.Controls.Add(this.cmbPaymentMethodFilter);
            this.pnlSearch.Controls.Add(this.lblMethodFilter);
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Controls.Add(this.lblSearch);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(0, 50);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(1200, 50);
            this.pnlSearch.TabIndex = 1;
            // 
            // btnDateRange
            // 
            this.btnDateRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnDateRange.Location = new System.Drawing.Point(620, 13);
            this.btnDateRange.Name = "btnDateRange";
            this.btnDateRange.Size = new System.Drawing.Size(90, 25);
            this.btnDateRange.TabIndex = 5;
            this.btnDateRange.Text = "Date Range";
            this.btnDateRange.UseVisualStyleBackColor = true;
            this.btnDateRange.Click += new System.EventHandler(this.btnDateRange_Click);
            // 
            // cmbPaymentMethodFilter
            // 
            this.cmbPaymentMethodFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMethodFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cmbPaymentMethodFilter.Location = new System.Drawing.Point(480, 15);
            this.cmbPaymentMethodFilter.Name = "cmbPaymentMethodFilter";
            this.cmbPaymentMethodFilter.Size = new System.Drawing.Size(120, 23);
            this.cmbPaymentMethodFilter.TabIndex = 4;
            this.cmbPaymentMethodFilter.SelectedIndexChanged += new System.EventHandler(this.cmbPaymentMethodFilter_SelectedIndexChanged);
            // 
            // lblMethodFilter
            // 
            this.lblMethodFilter.AutoSize = true;
            this.lblMethodFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblMethodFilter.Location = new System.Drawing.Point(420, 18);
            this.lblMethodFilter.Name = "lblMethodFilter";
            this.lblMethodFilter.Size = new System.Drawing.Size(54, 15);
            this.lblMethodFilter.TabIndex = 3;
            this.lblMethodFilter.Text = "Method:";
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSearch.Location = new System.Drawing.Point(320, 13);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 25);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.txtSearch.Location = new System.Drawing.Point(80, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(230, 21);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblSearch.Location = new System.Drawing.Point(20, 18);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(54, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search:";
            // 
            // pnlButtons
            // 
            this.pnlButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlButtons.Controls.Add(this.btnClose);
            this.pnlButtons.Controls.Add(this.btnRefresh);
            this.pnlButtons.Controls.Add(this.btnViewPayment);
            this.pnlButtons.Controls.Add(this.btnNewPayment);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(0, 100);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(1200, 50);
            this.pnlButtons.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnClose.Location = new System.Drawing.Point(1100, 13);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 25);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRefresh.Location = new System.Drawing.Point(230, 13);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 25);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnViewPayment
            // 
            this.btnViewPayment.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnViewPayment.Location = new System.Drawing.Point(130, 13);
            this.btnViewPayment.Name = "btnViewPayment";
            this.btnViewPayment.Size = new System.Drawing.Size(90, 25);
            this.btnViewPayment.TabIndex = 1;
            this.btnViewPayment.Text = "View Details";
            this.btnViewPayment.UseVisualStyleBackColor = true;
            this.btnViewPayment.Click += new System.EventHandler(this.btnViewPayment_Click);
            // 
            // btnNewPayment
            // 
            this.btnNewPayment.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnNewPayment.Location = new System.Drawing.Point(20, 13);
            this.btnNewPayment.Name = "btnNewPayment";
            this.btnNewPayment.Size = new System.Drawing.Size(100, 25);
            this.btnNewPayment.TabIndex = 0;
            this.btnNewPayment.Text = "New Payment";
            this.btnNewPayment.UseVisualStyleBackColor = true;
            this.btnNewPayment.Click += new System.EventHandler(this.btnNewPayment_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.dgvPayments);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 200);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(10);
            this.pnlContent.Size = new System.Drawing.Size(1200, 450);
            this.pnlContent.TabIndex = 4;
            // 
            // dgvPayments
            // 
            this.dgvPayments.AllowUserToAddRows = false;
            this.dgvPayments.AllowUserToDeleteRows = false;
            this.dgvPayments.BackgroundColor = System.Drawing.Color.White;
            this.dgvPayments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPayments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPayments.Location = new System.Drawing.Point(10, 10);
            this.dgvPayments.MultiSelect = false;
            this.dgvPayments.Name = "dgvPayments";
            this.dgvPayments.ReadOnly = true;
            this.dgvPayments.RowHeadersWidth = 25;
            this.dgvPayments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPayments.Size = new System.Drawing.Size(1180, 430);
            this.dgvPayments.TabIndex = 0;
            this.dgvPayments.DoubleClick += new System.EventHandler(this.dgvPayments_DoubleClick);
            // 
            // pnlSummary
            // 
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlSummary.Controls.Add(this.lblTodayAmount);
            this.pnlSummary.Controls.Add(this.lblTodayPayments);
            this.pnlSummary.Controls.Add(this.lblTotalAmount);
            this.pnlSummary.Controls.Add(this.lblTotalPayments);
            this.pnlSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSummary.Location = new System.Drawing.Point(0, 150);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Size = new System.Drawing.Size(1200, 50);
            this.pnlSummary.TabIndex = 3;
            // 
            // lblTodayAmount
            // 
            this.lblTodayAmount.AutoSize = true;
            this.lblTodayAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblTodayAmount.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblTodayAmount.Location = new System.Drawing.Point(600, 18);
            this.lblTodayAmount.Name = "lblTodayAmount";
            this.lblTodayAmount.Size = new System.Drawing.Size(108, 15);
            this.lblTodayAmount.TabIndex = 3;
            this.lblTodayAmount.Text = "Today's Amount: ₹0";
            // 
            // lblTodayPayments
            // 
            this.lblTodayPayments.AutoSize = true;
            this.lblTodayPayments.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblTodayPayments.Location = new System.Drawing.Point(520, 18);
            this.lblTodayPayments.Name = "lblTodayPayments";
            this.lblTodayPayments.Size = new System.Drawing.Size(50, 15);
            this.lblTodayPayments.TabIndex = 2;
            this.lblTodayPayments.Text = "Today: 0";
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmount.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTotalAmount.Location = new System.Drawing.Point(200, 18);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(103, 15);
            this.lblTotalAmount.TabIndex = 1;
            this.lblTotalAmount.Text = "Total Amount: ₹0";
            // 
            // lblTotalPayments
            // 
            this.lblTotalPayments.AutoSize = true;
            this.lblTotalPayments.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalPayments.Location = new System.Drawing.Point(20, 18);
            this.lblTotalPayments.Name = "lblTotalPayments";
            this.lblTotalPayments.Size = new System.Drawing.Size(110, 15);
            this.lblTotalPayments.TabIndex = 0;
            this.lblTotalPayments.Text = "Total Payments: 0";
            // 
            // PaymentListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 650);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlSummary);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlSearch);
            this.Controls.Add(this.pnlTop);
            this.Name = "PaymentListForm";
            this.Text = "Payment Management";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).EndInit();
            this.pnlSummary.ResumeLayout(false);
            this.pnlSummary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Button btnDateRange;
        private System.Windows.Forms.ComboBox cmbPaymentMethodFilter;
        private System.Windows.Forms.Label lblMethodFilter;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnViewPayment;
        private System.Windows.Forms.Button btnNewPayment;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.DataGridView dgvPayments;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblTodayAmount;
        private System.Windows.Forms.Label lblTodayPayments;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Label lblTotalPayments;
    }
}