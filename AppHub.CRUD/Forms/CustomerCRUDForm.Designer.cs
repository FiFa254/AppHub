namespace AppHub.CRUD
{
    partial class CustomerCRUDForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        // Title / Grid / Count มาจาก AppHubCRUDForm — ที่นี่มีแค่ toolbar
        private void InitializeComponent()
        {
            this.pnlToolbar = new System.Windows.Forms.Panel();
            this.txtSearch  = new System.Windows.Forms.TextBox();
            this.btnSearch  = new System.Windows.Forms.Button();
            this.btnClear   = new System.Windows.Forms.Button();
            this.btnAdd     = new System.Windows.Forms.Button();
            this.btnEdit    = new System.Windows.Forms.Button();
            this.btnDelete  = new System.Windows.Forms.Button();
            this.pnlToolbar.SuspendLayout();
            this.SuspendLayout();

            // pnlToolbar
            this.pnlToolbar.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Height    = 44;
            this.pnlToolbar.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.txtSearch, this.btnSearch, this.btnClear,
                this.btnAdd, this.btnEdit, this.btnDelete
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

            this.btnAdd.Text      = "เพิ่ม";
            this.btnAdd.Tag       = "primary";
            this.btnAdd.Location  = new System.Drawing.Point(420, 8);
            this.btnAdd.Size      = new System.Drawing.Size(80, 28);
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Click    += new System.EventHandler(this.btnAdd_Click);

            this.btnEdit.Text      = "แก้ไข";
            this.btnEdit.Location  = new System.Drawing.Point(508, 8);
            this.btnEdit.Size      = new System.Drawing.Size(80, 28);
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Click    += new System.EventHandler(this.btnEdit_Click);

            this.btnDelete.Text      = "ลบ";
            this.btnDelete.Tag       = "danger";
            this.btnDelete.Location  = new System.Drawing.Point(596, 8);
            this.btnDelete.Size      = new System.Drawing.Size(80, 28);
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Click    += new System.EventHandler(this.btnDelete_Click);

            // DGV (จาก base)
            this.DGV.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellDoubleClick);

            // CustomerCRUDForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(860, 540);
            this.Controls.Add(this.pnlToolbar);
            this.Name = "CustomerCRUDForm";
            this.Text = "จัดการข้อมูลลูกค้า";

            this.pnlToolbar.ResumeLayout(false);
            this.pnlToolbar.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel    pnlToolbar;
        private System.Windows.Forms.TextBox  txtSearch;
        private System.Windows.Forms.Button   btnSearch;
        private System.Windows.Forms.Button   btnClear;
        private System.Windows.Forms.Button   btnAdd;
        private System.Windows.Forms.Button   btnEdit;
        private System.Windows.Forms.Button   btnDelete;
    }
}
