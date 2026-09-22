namespace AppHub.Scan
{
    partial class ScanForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle    = new System.Windows.Forms.Label();
            this.pnlInput    = new System.Windows.Forms.Panel();
            this.lblCode     = new System.Windows.Forms.Label();
            this.txtCode     = new System.Windows.Forms.TextBox();
            this.lblNote     = new System.Windows.Forms.Label();
            this.txtNote     = new System.Windows.Forms.TextBox();
            this.btnSave     = new System.Windows.Forms.Button();
            this.btnClear    = new System.Windows.Forms.Button();
            this.lblStatus   = new System.Windows.Forms.Label();
            this.pnlHistory  = new System.Windows.Forms.Panel();
            this.lblHistory  = new System.Windows.Forms.Label();
            this.btnRefresh  = new System.Windows.Forms.Button();
            this.dgvLog      = new System.Windows.Forms.DataGridView();
            this.pnlInput.SuspendLayout();
            this.pnlHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).BeginInit();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(30, 60, 120);
            this.lblTitle.Height    = 40;
            this.lblTitle.Padding   = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblTitle.Text      = "🔍 Scan / Input ข้อมูล";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // pnlInput
            this.pnlInput.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlInput.Height    = 110;
            this.pnlInput.BackColor = System.Drawing.Color.FromArgb(245, 247, 252);
            this.pnlInput.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.lblCode, this.txtCode, this.lblNote, this.txtNote,
                this.btnSave, this.btnClear, this.lblStatus
            });

            this.lblCode.Text     = "รหัสลูกค้า *";
            this.lblCode.Location = new System.Drawing.Point(8, 14);
            this.lblCode.AutoSize = true;

            this.txtCode.Location      = new System.Drawing.Point(110, 10);
            this.txtCode.Size          = new System.Drawing.Size(200, 24);
            this.txtCode.Font          = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.txtCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;

            this.lblNote.Text     = "หมายเหตุ";
            this.lblNote.Location = new System.Drawing.Point(8, 46);
            this.lblNote.AutoSize = true;

            this.txtNote.Location = new System.Drawing.Point(110, 42);
            this.txtNote.Size     = new System.Drawing.Size(400, 24);
            this.txtNote.Font     = new System.Drawing.Font("Segoe UI", 9F);

            this.btnSave.Text      = "✅ บันทึก";
            this.btnSave.Location  = new System.Drawing.Point(110, 74);
            this.btnSave.Size      = new System.Drawing.Size(90, 28);
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(200, 240, 200);
            this.btnSave.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.Click    += new System.EventHandler(this.btnSave_Click);

            this.btnClear.Text      = "ล้าง";
            this.btnClear.Location  = new System.Drawing.Point(208, 74);
            this.btnClear.Size      = new System.Drawing.Size(70, 28);
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Click    += new System.EventHandler(this.btnClear_Click);

            this.lblStatus.Location  = new System.Drawing.Point(290, 80);
            this.lblStatus.Size      = new System.Drawing.Size(560, 20);
            this.lblStatus.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Text      = "พร้อม scan...";

            // pnlHistory
            this.pnlHistory.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlHistory.Height    = 28;
            this.pnlHistory.Controls.Add(this.lblHistory);
            this.pnlHistory.Controls.Add(this.btnRefresh);

            this.lblHistory.Text      = "ประวัติ 50 รายการล่าสุด";
            this.lblHistory.Location  = new System.Drawing.Point(8, 6);
            this.lblHistory.AutoSize  = true;
            this.lblHistory.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblHistory.ForeColor = System.Drawing.Color.FromArgb(30, 60, 120);

            this.btnRefresh.Text      = "🔄 Refresh";
            this.btnRefresh.Location  = new System.Drawing.Point(750, 2);
            this.btnRefresh.Size      = new System.Drawing.Size(90, 24);
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Click    += new System.EventHandler(this.btnRefresh_Click);

            // dgvLog
            this.dgvLog.AllowUserToAddRows    = false;
            this.dgvLog.AllowUserToDeleteRows = false;
            this.dgvLog.AutoSizeColumnsMode   = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLog.Dock                  = System.Windows.Forms.DockStyle.Fill;
            this.dgvLog.ReadOnly              = true;
            this.dgvLog.RowHeadersVisible     = false;
            this.dgvLog.SelectionMode         = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(860, 540);
            this.Controls.Add(this.dgvLog);
            this.Controls.Add(this.pnlHistory);
            this.Controls.Add(this.pnlInput);
            this.Controls.Add(this.lblTitle);
            this.AcceptButton = this.btnSave;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "ScanForm";
            this.Text = "Scan / Input ข้อมูล";

            this.pnlInput.ResumeLayout(false);
            this.pnlInput.PerformLayout();
            this.pnlHistory.ResumeLayout(false);
            this.pnlHistory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label            lblTitle;
        private System.Windows.Forms.Panel            pnlInput;
        private System.Windows.Forms.Label            lblCode;
        private System.Windows.Forms.TextBox          txtCode;
        private System.Windows.Forms.Label            lblNote;
        private System.Windows.Forms.TextBox          txtNote;
        private System.Windows.Forms.Button           btnSave;
        private System.Windows.Forms.Button           btnClear;
        private System.Windows.Forms.Label            lblStatus;
        private System.Windows.Forms.Panel            pnlHistory;
        private System.Windows.Forms.Label            lblHistory;
        private System.Windows.Forms.Button           btnRefresh;
        private System.Windows.Forms.DataGridView     dgvLog;
    }
}
