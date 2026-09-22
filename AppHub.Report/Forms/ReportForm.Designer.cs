namespace AppHub.Report
{
    partial class ReportForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        // Title / Grid / Count / Footer มาจาก AppHubCRUDForm — ที่นี่มีแค่ filter + ปุ่ม Export
        private void InitializeComponent()
        {
            this.pnlFilter     = new System.Windows.Forms.Panel();
            this.txtSearch     = new System.Windows.Forms.TextBox();
            this.btnSearch     = new System.Windows.Forms.Button();
            this.btnClear      = new System.Windows.Forms.Button();
            this.chkDateFilter = new System.Windows.Forms.CheckBox();
            this.dtpFrom       = new System.Windows.Forms.DateTimePicker();
            this.lblTo         = new System.Windows.Forms.Label();
            this.dtpTo         = new System.Windows.Forms.DateTimePicker();
            this.btnExport     = new System.Windows.Forms.Button();
            this.pnlFilter.SuspendLayout();
            this.SuspendLayout();

            // pnlFilter
            this.pnlFilter.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Height    = 78;
            this.pnlFilter.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.txtSearch, this.btnSearch, this.btnClear,
                this.chkDateFilter, this.dtpFrom, this.lblTo, this.dtpTo
            });

            this.txtSearch.Location = new System.Drawing.Point(8, 10);
            this.txtSearch.Size     = new System.Drawing.Size(220, 24);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);

            this.btnSearch.Text      = "ค้นหา";
            this.btnSearch.Location  = new System.Drawing.Point(236, 8);
            this.btnSearch.Size      = new System.Drawing.Size(80, 28);
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Click    += new System.EventHandler(this.btnSearch_Click);

            this.btnClear.Text      = "ล้าง";
            this.btnClear.Location  = new System.Drawing.Point(324, 8);
            this.btnClear.Size      = new System.Drawing.Size(60, 28);
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Click    += new System.EventHandler(this.btnClear_Click);

            this.chkDateFilter.Text     = "กรองตามวันที่สร้าง";
            this.chkDateFilter.Location = new System.Drawing.Point(8, 44);
            this.chkDateFilter.AutoSize = true;
            this.chkDateFilter.CheckedChanged += new System.EventHandler(this.chkDateFilter_CheckedChanged);

            this.dtpFrom.Location = new System.Drawing.Point(150, 42);
            this.dtpFrom.Size     = new System.Drawing.Size(130, 22);
            this.dtpFrom.Format   = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Enabled  = false;

            this.lblTo.Text      = "ถึง";
            this.lblTo.Location  = new System.Drawing.Point(286, 46);
            this.lblTo.AutoSize  = true;

            this.dtpTo.Location  = new System.Drawing.Point(306, 42);
            this.dtpTo.Size      = new System.Drawing.Size(130, 22);
            this.dtpTo.Format    = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Enabled   = false;

            // btnExport (อยู่ใน PnlFooter ของ base ชิดขวา)
            this.btnExport.Text      = "Export CSV";
            this.btnExport.Tag       = "primary";
            this.btnExport.Dock      = System.Windows.Forms.DockStyle.Right;
            this.btnExport.Width     = 110;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.Click    += new System.EventHandler(this.btnExport_Click);
            this.PnlFooter.Controls.Add(this.btnExport);

            // ReportForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(860, 540);
            this.Controls.Add(this.pnlFilter);
            this.Name = "ReportForm";
            this.Text = "รายงานข้อมูลลูกค้า";

            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel            pnlFilter;
        private System.Windows.Forms.TextBox          txtSearch;
        private System.Windows.Forms.Button           btnSearch;
        private System.Windows.Forms.Button           btnClear;
        private System.Windows.Forms.CheckBox         chkDateFilter;
        private System.Windows.Forms.DateTimePicker   dtpFrom;
        private System.Windows.Forms.Label            lblTo;
        private System.Windows.Forms.DateTimePicker   dtpTo;
        private System.Windows.Forms.Button           btnExport;
    }
}
