namespace FocusModern.Forms
{
    partial class BranchSelectionForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Button btnBranch1;
        private System.Windows.Forms.Button btnBranch2;
        private System.Windows.Forms.Button btnBranch3;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblVersion;

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.btnBranch1 = new System.Windows.Forms.Button();
            this.btnBranch2 = new System.Windows.Forms.Button();
            this.btnBranch3 = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTitle.Location = new System.Drawing.Point(150, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 29);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "FOCUS Modern";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle.Location = new System.Drawing.Point(180, 70);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(240, 20);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Vehicle Finance Management System";
            // 
            // btnBranch1
            // 
            this.btnBranch1.BackColor = System.Drawing.Color.LightGreen;
            this.btnBranch1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBranch1.Location = new System.Drawing.Point(100, 140);
            this.btnBranch1.Name = "btnBranch1";
            this.btnBranch1.Size = new System.Drawing.Size(120, 100);
            this.btnBranch1.TabIndex = 2;
            this.btnBranch1.Text = "Branch 1\n(Active)";
            this.btnBranch1.UseVisualStyleBackColor = false;
            this.btnBranch1.Click += new System.EventHandler(this.BranchButton_Click);
            // 
            // btnBranch2
            // 
            this.btnBranch2.BackColor = System.Drawing.Color.LightBlue;
            this.btnBranch2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBranch2.Location = new System.Drawing.Point(240, 140);
            this.btnBranch2.Name = "btnBranch2";
            this.btnBranch2.Size = new System.Drawing.Size(120, 100);
            this.btnBranch2.TabIndex = 3;
            this.btnBranch2.Text = "Branch 2\n(Historical)";
            this.btnBranch2.UseVisualStyleBackColor = false;
            this.btnBranch2.Click += new System.EventHandler(this.BranchButton_Click);
            // 
            // btnBranch3
            // 
            this.btnBranch3.BackColor = System.Drawing.Color.LightBlue;
            this.btnBranch3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBranch3.Location = new System.Drawing.Point(380, 140);
            this.btnBranch3.Name = "btnBranch3";
            this.btnBranch3.Size = new System.Drawing.Size(120, 100);
            this.btnBranch3.TabIndex = 4;
            this.btnBranch3.Text = "Branch 3\n(Historical)";
            this.btnBranch3.UseVisualStyleBackColor = false;
            this.btnBranch3.Click += new System.EventHandler(this.BranchButton_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.LightCoral;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(520, 320);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 30);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblStatus.Location = new System.Drawing.Point(30, 330);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 17);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Status: Ready";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.Gray;
            this.lblVersion.Location = new System.Drawing.Point(250, 280);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(100, 15);
            this.lblVersion.TabIndex = 7;
            this.lblVersion.Text = "Version 1.0.0";
            // 
            // BranchSelectionForm
            // 
            this.AcceptButton = this.btnBranch1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(600, 360);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnBranch3);
            this.Controls.Add(this.btnBranch2);
            this.Controls.Add(this.btnBranch1);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BranchSelectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FOCUS Modern - Branch Selection";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BranchSelectionForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}