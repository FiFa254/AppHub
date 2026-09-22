namespace AppHub.Launcher
{
    partial class UserManagementForm
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
            this.pnlToolbar         = new System.Windows.Forms.Panel();
            this.btnAdd             = new System.Windows.Forms.Button();
            this.btnEditPermissions = new System.Windows.Forms.Button();
            this.btnToggleActive    = new System.Windows.Forms.Button();
            this.btnResetPassword   = new System.Windows.Forms.Button();
            this.btnRefresh         = new System.Windows.Forms.Button();
            this.pnlToolbar.SuspendLayout();
            this.SuspendLayout();

            // pnlToolbar
            this.pnlToolbar.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Height    = 44;
            this.pnlToolbar.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.btnAdd, this.btnEditPermissions, this.btnToggleActive, this.btnResetPassword, this.btnRefresh
            });

            this.btnAdd.Text      = "เพิ่มผู้ใช้";
            this.btnAdd.Tag       = "primary";
            this.btnAdd.Location  = new System.Drawing.Point(8, 8);
            this.btnAdd.Size      = new System.Drawing.Size(110, 28);
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Click    += new System.EventHandler(this.btnAdd_Click);

            this.btnEditPermissions.Text      = "กำหนดสิทธิ์";
            this.btnEditPermissions.Location  = new System.Drawing.Point(126, 8);
            this.btnEditPermissions.Size      = new System.Drawing.Size(120, 28);
            this.btnEditPermissions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditPermissions.Click    += new System.EventHandler(this.btnEditPermissions_Click);

            this.btnToggleActive.Text      = "เปิด/ปิดใช้งาน";
            this.btnToggleActive.Location  = new System.Drawing.Point(254, 8);
            this.btnToggleActive.Size      = new System.Drawing.Size(130, 28);
            this.btnToggleActive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleActive.Click    += new System.EventHandler(this.btnToggleActive_Click);

            this.btnResetPassword.Text      = "รีเซ็ตรหัสผ่าน";
            this.btnResetPassword.Location  = new System.Drawing.Point(392, 8);
            this.btnResetPassword.Size      = new System.Drawing.Size(130, 28);
            this.btnResetPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetPassword.Click    += new System.EventHandler(this.btnResetPassword_Click);

            this.btnRefresh.Text      = "รีเฟรช";
            this.btnRefresh.Location  = new System.Drawing.Point(530, 8);
            this.btnRefresh.Size      = new System.Drawing.Size(90, 28);
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Click    += new System.EventHandler(this.btnRefresh_Click);

            // DGV (จาก base)
            this.DGV.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_CellDoubleClick);

            // UserManagementForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(860, 540);
            this.Controls.Add(this.pnlToolbar);
            this.Name = "UserManagementForm";
            this.Text = "จัดการผู้ใช้งาน";

            this.pnlToolbar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel   pnlToolbar;
        private System.Windows.Forms.Button  btnAdd;
        private System.Windows.Forms.Button  btnEditPermissions;
        private System.Windows.Forms.Button  btnToggleActive;
        private System.Windows.Forms.Button  btnResetPassword;
        private System.Windows.Forms.Button  btnRefresh;
    }
}
