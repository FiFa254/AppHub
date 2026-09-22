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
            this.menuMain            = new System.Windows.Forms.MenuStrip();
            this.mnuSystem           = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUserManagement   = new System.Windows.Forms.ToolStripMenuItem();
            this.sepUserManagement   = new System.Windows.Forms.ToolStripSeparator();
            this.mnuLogout           = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit             = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModules          = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCRUD             = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuImport           = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReport           = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuScan             = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWindow           = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCascade          = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTileHorizontal   = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTileVertical     = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMain            = new System.Windows.Forms.ToolStrip();
            this.tsbCRUD             = new System.Windows.Forms.ToolStripButton();
            this.tsbImport           = new System.Windows.Forms.ToolStripButton();
            this.tsbReport           = new System.Windows.Forms.ToolStripButton();
            this.tsbScan             = new System.Windows.Forms.ToolStripButton();
            this.tsSepUserManagement = new System.Windows.Forms.ToolStripSeparator();
            this.tsbUserManagement   = new System.Windows.Forms.ToolStripButton();
            this.statusMain          = new System.Windows.Forms.StatusStrip();
            this.lblStatusUser       = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuMain.SuspendLayout();
            this.toolMain.SuspendLayout();
            this.statusMain.SuspendLayout();
            this.SuspendLayout();

            // menuMain
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.mnuSystem, this.mnuModules, this.mnuWindow
            });
            this.menuMain.MdiWindowListItem = this.mnuWindow;

            // mnuSystem
            this.mnuSystem.Text = "ระบบ";
            this.mnuSystem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.mnuUserManagement, this.sepUserManagement, this.mnuLogout, this.mnuExit
            });
            this.mnuUserManagement.Text   = "👤 จัดการผู้ใช้";
            this.mnuUserManagement.Click += new System.EventHandler(this.mnuUserManagement_Click);
            this.mnuLogout.Text           = "ออกจากระบบ (Logout)";
            this.mnuLogout.Click         += new System.EventHandler(this.mnuLogout_Click);
            this.mnuExit.Text             = "ปิดโปรแกรม";
            this.mnuExit.Click           += new System.EventHandler(this.mnuExit_Click);

            // mnuModules
            this.mnuModules.Text = "โมดูล";
            this.mnuModules.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.mnuCRUD, this.mnuImport, this.mnuReport, this.mnuScan
            });
            this.mnuCRUD.Text     = "📦 จัดการข้อมูลลูกค้า";
            this.mnuCRUD.Click   += new System.EventHandler(this.mnuCRUD_Click);
            this.mnuImport.Text   = "📥 นำเข้า Excel";
            this.mnuImport.Click += new System.EventHandler(this.mnuImport_Click);
            this.mnuReport.Text   = "📊 รายงาน";
            this.mnuReport.Click += new System.EventHandler(this.mnuReport_Click);
            this.mnuScan.Text     = "🔍 Scan / Input";
            this.mnuScan.Click   += new System.EventHandler(this.mnuScan_Click);

            // mnuWindow
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

            // toolMain
            this.toolMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.tsbCRUD, this.tsbImport, this.tsbReport, this.tsbScan,
                this.tsSepUserManagement, this.tsbUserManagement
            });
            this.tsbCRUD.Text               = "📦 จัดการข้อมูล";
            this.tsbCRUD.Click             += new System.EventHandler(this.mnuCRUD_Click);
            this.tsbImport.Text             = "📥 Import Excel";
            this.tsbImport.Click           += new System.EventHandler(this.mnuImport_Click);
            this.tsbReport.Text             = "📊 รายงาน";
            this.tsbReport.Click           += new System.EventHandler(this.mnuReport_Click);
            this.tsbScan.Text               = "🔍 Scan";
            this.tsbScan.Click             += new System.EventHandler(this.mnuScan_Click);
            this.tsbUserManagement.Text     = "👤 User Management";
            this.tsbUserManagement.Click   += new System.EventHandler(this.mnuUserManagement_Click);

            // statusMain
            this.statusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.lblStatusUser });

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.statusMain);
            this.Controls.Add(this.toolMain);
            this.Controls.Add(this.menuMain);
            this.Font           = new System.Drawing.Font("Segoe UI", 9F);
            this.IsMdiContainer = true;
            this.MainMenuStrip  = this.menuMain;
            this.Name           = "MainForm";
            this.StartPosition  = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text           = "AppHub";
            this.WindowState    = System.Windows.Forms.FormWindowState.Maximized;

            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.toolMain.ResumeLayout(false);
            this.toolMain.PerformLayout();
            this.statusMain.ResumeLayout(false);
            this.statusMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.MenuStrip            menuMain;
        private System.Windows.Forms.ToolStripMenuItem    mnuSystem;
        private System.Windows.Forms.ToolStripMenuItem    mnuUserManagement;
        private System.Windows.Forms.ToolStripSeparator   sepUserManagement;
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
        private System.Windows.Forms.ToolStrip            toolMain;
        private System.Windows.Forms.ToolStripButton      tsbCRUD;
        private System.Windows.Forms.ToolStripButton      tsbImport;
        private System.Windows.Forms.ToolStripButton      tsbReport;
        private System.Windows.Forms.ToolStripButton      tsbScan;
        private System.Windows.Forms.ToolStripSeparator   tsSepUserManagement;
        private System.Windows.Forms.ToolStripButton      tsbUserManagement;
        private System.Windows.Forms.StatusStrip          statusMain;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusUser;
    }
}
