using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    /// MDI container หลัก — sidebar ซ้ายเปิดแต่ละ module เป็น MDI child ตามสิทธิ์
    /// </summary>
    public partial class MainForm : AppHubForm
    {
        /// <summary>true = ปิดเพราะ Logout (Program จะกลับไปหน้า Login)</summary>
        public bool IsLogout { get; private set; }

        private readonly Dictionary<Type, Button> _navByForm;
        private readonly HashSet<Form>            _moduleWindows = new HashSet<Form>();
        private Button    _activeNav;
        private MdiClient _mdiClient;

        public MainForm()
        {
            InitializeComponent();

            _navByForm = new Dictionary<Type, Button>
            {
                { typeof(CustomerCRUDForm),   navCRUD   },
                { typeof(ImportForm),         navImport },
                { typeof(ReportForm),         navReport },
                { typeof(ScanForm),           navScan   },
                { typeof(UserManagementForm), navUsers  },
            };
            foreach (var nav in _navByForm.Values)
                nav.Paint += DrawActiveMarker;

            Theme.SetIcon(navCRUD,           Theme.Icons.Customers);
            Theme.SetIcon(navImport,         Theme.Icons.Import);
            Theme.SetIcon(navReport,         Theme.Icons.Report);
            Theme.SetIcon(navScan,           Theme.Icons.Scan);
            Theme.SetIcon(navUsers,          Theme.Icons.Users);
            Theme.SetIcon(btnChangePassword, Theme.Icons.Password);
            Theme.SetIcon(btnLogout,         Theme.Icons.Power);

            SetMenuIcon(mnuCRUD,           Theme.Icons.Customers);
            SetMenuIcon(mnuImport,         Theme.Icons.Import);
            SetMenuIcon(mnuReport,         Theme.Icons.Report);
            SetMenuIcon(mnuScan,           Theme.Icons.Scan);
            SetMenuIcon(mnuUserManagement, Theme.Icons.Users);
            SetMenuIcon(mnuChangePassword, Theme.Icons.Password);
            SetMenuIcon(mnuLogout,         Theme.Icons.Power);

            ApplyPermissions();
        }

        private static void SetMenuIcon(ToolStripMenuItem item, char glyph)
            => item.Image = Theme.Icon(glyph, Theme.TextMuted, 16, 0);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _mdiClient = Controls.OfType<MdiClient>().FirstOrDefault();
            if (_mdiClient != null)
            {
                RemoveClientEdge(_mdiClient);
                _mdiClient.BackColor = Theme.Background;
                _mdiClient.BringToFront();              // เติมพื้นที่ที่เหลือหลัง sidebar + menu
                _mdiClient.Resize += (s, a) => FitModuleWindows();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!AppSession.HasAnyModule)
                ShowInfo("บัญชีของคุณยังไม่ได้รับสิทธิ์ใช้งาน module ใด\nกรุณาติดต่อผู้ดูแลระบบ", "ยังไม่มีสิทธิ์");
        }

        // ─── Permission ───────────────────────────────────────────────────────
        private void ApplyPermissions()
        {
            mnuCRUD.Enabled   = navCRUD.Enabled   = AppSession.HasPermission("CRUD");
            mnuImport.Enabled = navImport.Enabled = AppSession.HasPermission("IMPORT");
            mnuReport.Enabled = navReport.Enabled = AppSession.HasPermission("REPORT");
            mnuScan.Enabled   = navScan.Enabled   = AppSession.HasPermission("SCAN");

            // User Management เฉพาะ Admin — ซ่อนไม่ใช่แค่ disable
            mnuUserManagement.Visible = sepUserManagement.Visible = AppSession.IsAdmin;
            navUsers.Visible          = lblAdminSection.Visible   = AppSession.IsAdmin;

            lblUserName.Text = string.IsNullOrWhiteSpace(AppSession.FullName) ? AppSession.Username : AppSession.FullName;
            lblUserRole.Text = !AppSession.HasAnyModule ? "ยังไม่มีสิทธิ์ใช้งาน module"
                             : AppSession.IsAdmin        ? $"ผู้ดูแลระบบ · {AppSession.Username}"
                                                         : $"ผู้ใช้งาน · {AppSession.Username}";
            lblAvatar.Text   = Initial(lblUserName.Text);
        }

        private static string Initial(string name)
            => string.IsNullOrWhiteSpace(name) ? "?" : name.Trim().Substring(0, 1).ToUpperInvariant();

        // ─── Change password ──────────────────────────────────────────────────
        private void mnuChangePassword_Click(object sender, EventArgs e)
        {
            // เปิดอยู่แล้ว → focus หน้าต่างเดิม
            foreach (Form child in this.MdiChildren)
            {
                if (child is ChangePasswordDialog)
                {
                    child.Activate();
                    return;
                }
            }

            var dlg = new ChangePasswordDialog(AppSession.Username, requireCurrent: true);
            ShowDialogChild(dlg, () => ChangeOwnPassword(dlg.CurrentPassword, dlg.NewPassword));
        }

        private void ChangeOwnPassword(string currentPassword, string newPassword)
        {
            try
            {
                SQL.Connect();

                int updated = SQL.ExecuteCommand(@"
                    UPDATE dbo.t_Users
                    SET    Password_hash = @new
                    WHERE  User_id       = @id
                      AND  Password_hash = @current",
                    new Dictionary<string, object>
                    {
                        { "@new",     AccountRules.HashPassword(newPassword) },
                        { "@id",      AppSession.UserId },
                        { "@current", AccountRules.HashPassword(currentPassword) },
                    });

                if (updated == 0)
                {
                    ShowError("รหัสผ่านปัจจุบันไม่ถูกต้อง");
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowError("เปลี่ยนรหัสผ่านไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }

            ShowInfo("เปลี่ยนรหัสผ่านสำเร็จ");
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
                    if (child is AppHubForm hubForm)
                        hubForm.BringUp();
                    else
                        child.Activate();
                    return;
                }
            }

            var form = new T();
            MakeSeamless(form);
            form.MdiParent = this;
            _moduleWindows.Add(form);
            form.FormClosed += (s, e) => _moduleWindows.Remove(form);
            form.Show();
            FitToClient(form);
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

        // ─── Module window layout — เต็มพื้นที่ทำงาน ────────────────────────────
        /// <summary>
        /// ตัดกรอบหน้าต่าง Windows ออก (ดูเป็นหน้าเดียวกับแอป) + ปุ่มปิดที่หัวข้อของ module
        /// </summary>
        private static void MakeSeamless(Form form)
        {
            form.FormBorderStyle = FormBorderStyle.None;

            var title = form.Controls.Find("lblTitle", true).FirstOrDefault();
            if (title == null) return;

            var close = new Button
            {
                Name    = "btnCloseModule",
                Tag     = Theme.RoleIcon,
                Dock    = DockStyle.Right,
                Width   = 52,
                TabStop = false
            };
            Theme.SetIcon(close, Theme.Icons.Close);
            close.Click += (s, e) => form.Close();
            title.Controls.Add(close);
        }

        private const int GWL_EXSTYLE      = -20;
        private const int WS_EX_CLIENTEDGE = 0x200;
        private const uint SWP_FRAMECHANGED_NOMOVE_NOSIZE_NOZORDER = 0x0020 | 0x0002 | 0x0001 | 0x0004;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr after, int x, int y, int cx, int cy, uint flags);

        /// <summary>ลบขอบนูน 3D รอบพื้นที่ MDI</summary>
        private static void RemoveClientEdge(MdiClient client)
        {
            int style = GetWindowLong(client.Handle, GWL_EXSTYLE);
            SetWindowLong(client.Handle, GWL_EXSTYLE, style & ~WS_EX_CLIENTEDGE);
            SetWindowPos(client.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED_NOMOVE_NOSIZE_NOZORDER);
        }

        private Size _fittedSize;

        private void FitToClient(Form form)
        {
            if (_mdiClient == null || form.IsDisposed) return;
            form.WindowState = FormWindowState.Normal;
            form.Bounds      = new Rectangle(Point.Empty, _mdiClient.ClientSize);
            _fittedSize      = _mdiClient.ClientSize;
        }

        /// <summary>
        /// ย่อ/ขยาย MainForm → หน้าต่างที่เต็มพื้นที่อยู่ขยายตาม (หน้าต่างที่ผู้ใช้จัดเอง / tile ไม่ยุ่ง)
        /// </summary>
        private void FitModuleWindows()
        {
            var full = new Rectangle(Point.Empty, _fittedSize);
            foreach (var form in _moduleWindows.Where(f => f.WindowState == FormWindowState.Normal
                                                        && f.Bounds == full).ToList())
                FitToClient(form);
            _fittedSize = _mdiClient.ClientSize;
        }

        // ─── Sidebar highlight ────────────────────────────────────────────────
        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            var active = ActiveMdiChild;
            if (active == null) { SetActiveNav(null); return; }

            // dialog → ไฮไลต์เมนูของหน้าที่เปิด dialog นั้น
            var owner = MdiChildren.OfType<AppHubForm>().FirstOrDefault(f => f.OpenDialog == active);
            if (_navByForm.TryGetValue((owner ?? active).GetType(), out var nav))
                SetActiveNav(nav);
        }

        private void SetActiveNav(Button nav)
        {
            if (_activeNav == nav) return;
            if (_activeNav != null)
            {
                _activeNav.BackColor = Theme.Sidebar;
                _activeNav.ForeColor = Theme.SidebarText;
                Theme.RefreshIcon(_activeNav);
                _activeNav.Invalidate();
            }
            _activeNav = nav;
            if (_activeNav != null)
            {
                _activeNav.BackColor = Theme.SidebarHover;
                _activeNav.ForeColor = Color.White;
                Theme.RefreshIcon(_activeNav);
                _activeNav.Invalidate();
            }
        }

        /// <summary>แถบสีน้ำเงินด้านซ้ายของเมนูที่เลือกอยู่</summary>
        private void DrawActiveMarker(object sender, PaintEventArgs e)
        {
            if (sender != _activeNav) return;
            using (var brush = new SolidBrush(Theme.Primary))
                e.Graphics.FillRectangle(brush, 0, 6, 4, ((Control)sender).Height - 12);
        }

        private void lblAvatar_Paint(object sender, PaintEventArgs e)
        {
            var lbl = (Label)sender;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(Theme.Sidebar);
            using (var brush = new SolidBrush(Theme.Primary))
                e.Graphics.FillEllipse(brush, 0, 0, lbl.Width - 1, lbl.Height - 1);
            TextRenderer.DrawText(e.Graphics, lbl.Text, Theme.BodyBold, lbl.ClientRectangle, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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
