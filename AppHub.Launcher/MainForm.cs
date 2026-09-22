using System;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;
using AppHub.CRUD;
using AppHub.Import;
using AppHub.Report;
using AppHub.Scan;

namespace AppHub.Launcher
{
    /// <summary>
    /// MDI container หลัก — เปิดแต่ละ module เป็น MDI child ตามสิทธิ์
    /// </summary>
    public partial class MainForm : AppHubForm
    {
        /// <summary>true = ปิดเพราะ Logout (Program จะกลับไปหน้า Login)</summary>
        public bool IsLogout { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            ApplyPermissions();
        }

        // ─── Permission ───────────────────────────────────────────────────────
        private void ApplyPermissions()
        {
            mnuCRUD.Enabled   = tsbCRUD.Enabled   = AppSession.HasPermission("CRUD");
            mnuImport.Enabled = tsbImport.Enabled = AppSession.HasPermission("IMPORT");
            mnuReport.Enabled = tsbReport.Enabled = AppSession.HasPermission("REPORT");
            mnuScan.Enabled   = tsbScan.Enabled   = AppSession.HasPermission("SCAN");

            // User Management เฉพาะ Admin — ซ่อนไม่ใช่แค่ disable
            mnuUserManagement.Visible = tsbUserManagement.Visible = AppSession.IsAdmin;
            sepUserManagement.Visible = tsSepUserManagement.Visible = AppSession.IsAdmin;

            lblStatusUser.Text = $"ผู้ใช้: {AppSession.FullName} ({AppSession.Username})"
                               + (AppSession.IsAdmin ? " — Admin" : "");
        }

        // ─── Open MDI child ───────────────────────────────────────────────────
        private void OpenChild<T>(string moduleCode) where T : Form, new()
        {
            if (moduleCode != null && !AppSession.HasPermission(moduleCode))
            {
                ShowWarning("คุณไม่มีสิทธิ์ใช้งาน module นี้");
                return;
            }

            // เปิดซ้ำ → focus หน้าต่างเดิม
            foreach (Form child in this.MdiChildren)
            {
                if (child is T)
                {
                    if (child.WindowState == FormWindowState.Minimized)
                        child.WindowState = FormWindowState.Normal;
                    child.Activate();
                    return;
                }
            }

            var form = new T();
            form.MdiParent = this;
            form.Show();
        }

        private void mnuCRUD_Click(object sender, EventArgs e)   => OpenChild<CustomerCRUDForm>("CRUD");
        private void mnuImport_Click(object sender, EventArgs e) => OpenChild<ImportForm>("IMPORT");
        private void mnuReport_Click(object sender, EventArgs e) => OpenChild<ReportForm>("REPORT");
        private void mnuScan_Click(object sender, EventArgs e)   => OpenChild<ScanForm>("SCAN");

        private void mnuUserManagement_Click(object sender, EventArgs e)
        {
            if (!AppSession.IsAdmin) return;
            OpenChild<UserManagementForm>(null);
        }

        // ─── Window ───────────────────────────────────────────────────────────
        private void mnuCascade_Click(object sender, EventArgs e)       => LayoutMdi(MdiLayout.Cascade);
        private void mnuTileHorizontal_Click(object sender, EventArgs e) => LayoutMdi(MdiLayout.TileHorizontal);
        private void mnuTileVertical_Click(object sender, EventArgs e)   => LayoutMdi(MdiLayout.TileVertical);

        // ─── Logout / Exit ────────────────────────────────────────────────────
        private void mnuLogout_Click(object sender, EventArgs e)
        {
            if (!Confirm("ต้องการออกจากระบบ?")) return;
            IsLogout = true;
            this.Close();
        }

        private void mnuExit_Click(object sender, EventArgs e) => this.Close();
    }
}
