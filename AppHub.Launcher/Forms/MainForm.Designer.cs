namespace AppHub.Launcher
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.menuMain          = new System.Windows.Forms.MenuStrip();
            this.mnuSystem         = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUserManagement = new System.Windows.Forms.ToolStripMenuItem();
            this.sepUserManagement = new System.Windows.Forms.ToolStripSeparator();
            this.mnuChangePassword = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLogout         = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit           = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModules        = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCRUD           = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuImport         = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReport         = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuScan           = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWindow         = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCascade        = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTileHorizontal = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTileVertical   = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlSidebar        = new System.Windows.Forms.Panel();
            this.lblBrand          = new System.Windows.Forms.Label();
            this.lblNavSection     = new System.Windows.Forms.Label();
            this.navCRUD           = new System.Windows.Forms.Button();
            this.navImport         = new System.Windows.Forms.Button();
            this.navReport         = new System.Windows.Forms.Button();
            this.navScan           = new System.Windows.Forms.Button();
            this.lblAdminSection   = new System.Windows.Forms.Label();
            this.navUsers          = new System.Windows.Forms.Button();
            this.pnlUserCard       = new System.Windows.Forms.Panel();
            this.lblAvatar         = new System.Windows.Forms.Label();
            this.lblUserName       = new System.Windows.Forms.Label();
            this.lblUserRole       = new System.Windows.Forms.Label();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.btnLogout         = new System.Windows.Forms.Button();
            this.menuMain.SuspendLayout();
            this.pnlSidebar.SuspendLayout();
            this.pnlUserCard.SuspendLayout();
            this.SuspendLayout();

            // ─── menuMain ─────────────────────────────────────────────────────
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.mnuSystem, this.mnuModules, this.mnuWindow
            });
            this.menuMain.MdiWindowListItem = this.mnuWindow;
            this.menuMain.Padding           = new System.Windows.Forms.Padding(8, 4, 0, 4);

            this.mnuSystem.Text = "ระบบ";
            this.mnuSystem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.mnuUserManagement, this.sepUserManagement, this.mnuChangePassword, this.mnuLogout, this.mnuExit
            });
            this.mnuUserManagement.Text   = "จัดการผู้ใช้";
            this.mnuUserManagement.Click += new System.EventHandler(this.mnuUserManagement_Click);
            this.mnuChangePassword.Text   = "เปลี่ยนรหัสผ่าน";
            this.mnuChangePassword.Click += new System.EventHandler(this.mnuChangePassword_Click);
            this.mnuLogout.Text           = "ออกจากระบบ (Logout)";
            this.mnuLogout.Click         += new System.EventHandler(this.mnuLogout_Click);
            this.mnuExit.Text             = "ปิดโปรแกรม";
            this.mnuExit.Click           += new System.EventHandler(this.mnuExit_Click);

            this.mnuModules.Text = "โมดูล";
            this.mnuModules.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.mnuCRUD, this.mnuImport, this.mnuReport, this.mnuScan
            });
            this.mnuCRUD.Text     = "จัดการข้อมูลลูกค้า";
            this.mnuCRUD.Click   += new System.EventHandler(this.mnuCRUD_Click);
            this.mnuImport.Text   = "นำเข้า Excel";
            this.mnuImport.Click += new System.EventHandler(this.mnuImport_Click);
            this.mnuReport.Text   = "รายงาน";
            this.mnuReport.Click += new System.EventHandler(this.mnuReport_Click);
            this.mnuScan.Text     = "Scan / Input";
            this.mnuScan.Click   += new System.EventHandler(this.mnuScan_Click);

            this.mnuWindow.Text = "หน้าต่าง";
            this.mnuWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.mnuCascade, this.mnuTileHorizontal, this.mnuTileVertical
            });
            this.mnuCascade.Text          = "เรียงซ้อน";
            this.mnuCascade.Click        += new System.EventHandler(this.mnuCascade_Click);
            this.mnuTileHorizontal.Text   = "เรียงแนวนอน";
            this.mnuTileHorizontal.Click += new System.EventHandler(this.mnuTileHorizontal_Click);
            this.mnuTileVertical.Text     = "เรียงแนวตั้ง";
            this.mnuTileVertical.Click   += new System.EventHandler(this.mnuTileVertical_Click);

            // ─── pnlSidebar (Tag = ไม่ให้ Theme ทำเป็น toolbar) ──────────────
            this.pnlSidebar.Dock      = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Width     = 236;
            this.pnlSidebar.Tag       = "sidebar";
            this.pnlSidebar.BackColor = AppHub.Core.UI.Theme.Sidebar;
            // Dock = Top: ตัวที่ Add ทีหลังอยู่บนสุด
            this.pnlSidebar.Controls.Add(this.navUsers);
            this.pnlSidebar.Controls.Add(this.lblAdminSection);
            this.pnlSidebar.Controls.Add(this.navScan);
            this.pnlSidebar.Controls.Add(this.navReport);
            this.pnlSidebar.Controls.Add(this.navImport);
            this.pnlSidebar.Controls.Add(this.navCRUD);
            this.pnlSidebar.Controls.Add(this.lblNavSection);
            this.pnlSidebar.Controls.Add(this.lblBrand);
            this.pnlSidebar.Controls.Add(this.pnlUserCard);

            this.lblBrand.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblBrand.Height    = 68;
            this.lblBrand.Text      = "◆  AppHub";
            this.lblBrand.Font      = new System.Drawing.Font("Segoe UI Semibold", 15F);
            this.lblBrand.ForeColor = System.Drawing.Color.White;
            this.lblBrand.Padding   = new System.Windows.Forms.Padding(18, 0, 0, 0);
            this.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblNavSection.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblNavSection.Height    = 30;
            this.lblNavSection.Text      = "เมนูหลัก";
            this.lblNavSection.Font      = AppHub.Core.UI.Theme.Small;
            this.lblNavSection.ForeColor = AppHub.Core.UI.Theme.SidebarMuted;
            this.lblNavSection.Padding   = new System.Windows.Forms.Padding(20, 0, 0, 4);
            this.lblNavSection.TextAlign = System.Drawing.ContentAlignment.BottomLeft;

            this.navCRUD.Dock    = System.Windows.Forms.DockStyle.Top;
            this.navCRUD.Height  = 44;
            this.navCRUD.Tag     = "nav";
            this.navCRUD.Text    = "จัดการข้อมูลลูกค้า";
            this.navCRUD.Click  += new System.EventHandler(this.mnuCRUD_Click);

            this.navImport.Dock   = System.Windows.Forms.DockStyle.Top;
            this.navImport.Height = 44;
            this.navImport.Tag    = "nav";
            this.navImport.Text   = "นำเข้า Excel";
            this.navImport.Click += new System.EventHandler(this.mnuImport_Click);

            this.navReport.Dock   = System.Windows.Forms.DockStyle.Top;
            this.navReport.Height = 44;
            this.navReport.Tag    = "nav";
            this.navReport.Text   = "รายงาน";
            this.navReport.Click += new System.EventHandler(this.mnuReport_Click);

            this.navScan.Dock   = System.Windows.Forms.DockStyle.Top;
            this.navScan.Height = 44;
            this.navScan.Tag    = "nav";
            this.navScan.Text   = "Scan / Input";
            this.navScan.Click += new System.EventHandler(this.mnuScan_Click);

            this.lblAdminSection.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblAdminSection.Height    = 42;
            this.lblAdminSection.Text      = "ผู้ดูแลระบบ";
            this.lblAdminSection.Font      = AppHub.Core.UI.Theme.Small;
            this.lblAdminSection.ForeColor = AppHub.Core.UI.Theme.SidebarMuted;
            this.lblAdminSection.Padding   = new System.Windows.Forms.Padding(20, 0, 0, 4);
            this.lblAdminSection.TextAlign = System.Drawing.ContentAlignment.BottomLeft;

            this.navUsers.Dock   = System.Windows.Forms.DockStyle.Top;
            this.navUsers.Height = 44;
            this.navUsers.Tag    = "nav";
            this.navUsers.Text   = "จัดการผู้ใช้";
            this.navUsers.Click += new System.EventHandler(this.mnuUserManagement_Click);

            // ─── pnlUserCard (ล่างสุดของ sidebar) ─────────────────────────────
            this.pnlUserCard.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlUserCard.Height    = 150;
            this.pnlUserCard.Tag       = "usercard";
            this.pnlUserCard.BackColor = AppHub.Core.UI.Theme.Sidebar;
            this.pnlUserCard.Controls.Add(this.btnLogout);
            this.pnlUserCard.Controls.Add(this.btnChangePassword);
            this.pnlUserCard.Controls.Add(this.lblUserRole);
            this.pnlUserCard.Controls.Add(this.lblUserName);
            this.pnlUserCard.Controls.Add(this.lblAvatar);

            this.lblAvatar.Location = new System.Drawing.Point(18, 18);
            this.lblAvatar.Size     = new System.Drawing.Size(38, 38);
            this.lblAvatar.Paint   += new System.Windows.Forms.PaintEventHandler(this.lblAvatar_Paint);

            this.lblUserName.Location     = new System.Drawing.Point(66, 18);
            this.lblUserName.Size         = new System.Drawing.Size(160, 20);
            this.lblUserName.AutoEllipsis = true;
            this.lblUserName.Font         = AppHub.Core.UI.Theme.BodyBold;
            this.lblUserName.ForeColor    = System.Drawing.Color.White;

            this.lblUserRole.Location     = new System.Drawing.Point(66, 38);
            this.lblUserRole.Size         = new System.Drawing.Size(160, 18);
            this.lblUserRole.AutoEllipsis = true;
            this.lblUserRole.Font         = AppHub.Core.UI.Theme.Small;
            this.lblUserRole.ForeColor    = AppHub.Core.UI.Theme.SidebarMuted;

            this.btnChangePassword.Location = new System.Drawing.Point(0, 70);
            this.btnChangePassword.Size     = new System.Drawing.Size(236, 36);
            this.btnChangePassword.Tag      = "nav";
            this.btnChangePassword.Text     = "เปลี่ยนรหัสผ่าน";
            this.btnChangePassword.Click   += new System.EventHandler(this.mnuChangePassword_Click);

            this.btnLogout.Location = new System.Drawing.Point(0, 106);
            this.btnLogout.Size     = new System.Drawing.Size(236, 36);
            this.btnLogout.Tag      = "nav";
            this.btnLogout.Text     = "ออกจากระบบ";
            this.btnLogout.Click   += new System.EventHandler(this.mnuLogout_Click);

            // ─── MainForm ─────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(1200, 760);
            // sidebar Add ทีหลัง → dock ก่อน → สูงเต็มจอ, menu อยู่ขวาของ sidebar
            this.Controls.Add(this.menuMain);
            this.Controls.Add(this.pnlSidebar);
            this.Font           = new System.Drawing.Font("Segoe UI", 9F);
            this.IsMdiContainer = true;
            this.MainMenuStrip  = this.menuMain;
            this.MinimumSize    = new System.Drawing.Size(900, 600);
            this.Name           = "MainForm";
            this.StartPosition  = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text           = "AppHub";
            this.WindowState    = System.Windows.Forms.FormWindowState.Maximized;
            this.MdiChildActivate += new System.EventHandler(this.MainForm_MdiChildActivate);

            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.pnlUserCard.ResumeLayout(false);
            this.pnlSidebar.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.MenuStrip            menuMain;
        private System.Windows.Forms.ToolStripMenuItem    mnuSystem;
        private System.Windows.Forms.ToolStripMenuItem    mnuUserManagement;
        private System.Windows.Forms.ToolStripSeparator   sepUserManagement;
        private System.Windows.Forms.ToolStripMenuItem    mnuChangePassword;
        private System.Windows.Forms.ToolStripMenuItem    mnuLogout;
        private System.Windows.Forms.ToolStripMenuItem    mnuExit;
        private System.Windows.Forms.ToolStripMenuItem    mnuModules;
        private System.Windows.Forms.ToolStripMenuItem    mnuCRUD;
        private System.Windows.Forms.ToolStripMenuItem    mnuImport;
        private System.Windows.Forms.ToolStripMenuItem    mnuReport;
        private System.Windows.Forms.ToolStripMenuItem    mnuScan;
        private System.Windows.Forms.ToolStripMenuItem    mnuWindow;
        private System.Windows.Forms.ToolStripMenuItem    mnuCascade;
        private System.Windows.Forms.ToolStripMenuItem    mnuTileHorizontal;
        private System.Windows.Forms.ToolStripMenuItem    mnuTileVertical;
        private System.Windows.Forms.Panel                pnlSidebar;
        private System.Windows.Forms.Label                lblBrand;
        private System.Windows.Forms.Label                lblNavSection;
        private System.Windows.Forms.Button               navCRUD;
        private System.Windows.Forms.Button               navImport;
        private System.Windows.Forms.Button               navReport;
        private System.Windows.Forms.Button               navScan;
        private System.Windows.Forms.Label                lblAdminSection;
        private System.Windows.Forms.Button               navUsers;
        private System.Windows.Forms.Panel                pnlUserCard;
        private System.Windows.Forms.Label                lblAvatar;
        private System.Windows.Forms.Label                lblUserName;
        private System.Windows.Forms.Label                lblUserRole;
        private System.Windows.Forms.Button               btnChangePassword;
        private System.Windows.Forms.Button               btnLogout;
    }
}
