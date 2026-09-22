namespace AppHub.Import
{
    partial class ImportForm
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
            this.pnlFile     = new System.Windows.Forms.Panel();
            this.txtFile     = new System.Windows.Forms.TextBox();
            this.btnBrowse   = new System.Windows.Forms.Button();
            this.btnImport   = new System.Windows.Forms.Button();
            this.lblInfo     = new System.Windows.Forms.Label();
            this.dgvPreview  = new System.Windows.Forms.DataGridView();
            this.pnlFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(30, 60, 120);
            this.lblTitle.Height    = 40;
            this.lblTitle.Padding   = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblTitle.Text      = "📥 นำเข้าข้อมูล Excel";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // pnlFile
            this.pnlFile.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlFile.Height    = 44;
            this.pnlFile.BackColor = System.Drawing.Color.FromArgb(245, 247, 252);
            this.pnlFile.Controls.Add(this.txtFile);
            this.pnlFile.Controls.Add(this.btnBrowse);
            this.pnlFile.Controls.Add(this.btnImport);

            this.txtFile.Location  = new System.Drawing.Point(8, 10);
            this.txtFile.Size      = new System.Drawing.Size(400, 24);
            this.txtFile.ReadOnly  = true;
            this.txtFile.Font      = new System.Drawing.Font("Segoe UI", 9F);

            this.btnBrowse.Text      = "📂 เลือกไฟล์";
            this.btnBrowse.Location  = new System.Drawing.Point(416, 8);
            this.btnBrowse.Size      = new System.Drawing.Size(100, 28);
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Click    += new System.EventHandler(this.btnBrowse_Click);

            this.btnImport.Text      = "✅ นำเข้า";
            this.btnImport.Location  = new System.Drawing.Point(524, 8);
            this.btnImport.Size      = new System.Drawing.Size(100, 28);
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImport.BackColor = System.Drawing.Color.FromArgb(200, 240, 200);
            this.btnImport.Enabled   = false;
            this.btnImport.Click    += new System.EventHandler(this.btnImport_Click);

            // lblInfo
            this.lblInfo.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblInfo.Height    = 24;
            this.lblInfo.Padding   = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this.lblInfo.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblInfo.Text      = "เลือกไฟล์ Excel เพื่อดูตัวอย่างก่อนนำเข้า";

            // dgvPreview
            this.dgvPreview.AllowUserToAddRows    = false;
            this.dgvPreview.AllowUserToDeleteRows = false;
            this.dgvPreview.AutoSizeColumnsMode   = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPreview.Dock                  = System.Windows.Forms.DockStyle.Fill;
            this.dgvPreview.ReadOnly              = true;
            this.dgvPreview.RowHeadersVisible     = false;
            this.dgvPreview.SelectionMode         = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(860, 540);
            this.Controls.Add(this.dgvPreview);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.pnlFile);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "ImportForm";
            this.Text = "นำเข้าข้อมูล Excel";

            this.pnlFile.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label         lblTitle;
        private System.Windows.Forms.Panel         pnlFile;
        private System.Windows.Forms.TextBox       txtFile;
        private System.Windows.Forms.Button        btnBrowse;
        private System.Windows.Forms.Button        btnImport;
        private System.Windows.Forms.Label         lblInfo;
        private System.Windows.Forms.DataGridView  dgvPreview;
    }
}
