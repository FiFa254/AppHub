using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AppHub.Core.UI;
using AppHub.CRUD;
using AppHub.Launcher;
using AppHub.Scan;
using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests
{
    /// <summary>
    /// Design system — สีตามบทบาทปุ่ม, หัวตารางภาษาไทย, ไม่มี emoji, sidebar, หน้าต่างแบบไร้กรอบ
    /// </summary>
    [TestClass]
    public class ThemeTests : UiTestBase
    {
        private MainForm _main;

        [TestInitialize]
        public void SignIn() => SignInAs("admin");

        private void ShowMainOffscreen()
        {
            // ใช้ ShowChild ของจริง (dialog เป็น MDI) แต่ยังดัก MessageBox
            var showMessage = UiServices.ShowMessage;
            UiServices.Reset();
            UiServices.ShowMessage = showMessage;

            _main = Track(new MainForm());
            _main.WindowState   = FormWindowState.Normal;
            _main.StartPosition = FormStartPosition.Manual;
            _main.Location      = new System.Drawing.Point(-20000, -20000);
            _main.ShowInTaskbar = false;
            _main.Show();
        }

        private static void Pump()
        {
            for (int i = 0; i < 5; i++) Application.DoEvents();
        }

        private static IEnumerable<Control> All(Control root)
            => root.Controls.Cast<Control>().SelectMany(c => new[] { c }.Concat(All(c)));

        // ─── Buttons / grid ──────────────────────────────────────────────────
        [TestMethod]
        public void Buttons_GetColorsFromRole()
        {
            var form = Open<CustomerCRUDForm>();

            Assert.AreEqual(Theme.Primary, Ui.Get<Button>(form, "btnAdd").BackColor);
            Assert.AreEqual(Theme.Surface, Ui.Get<Button>(form, "btnDelete").BackColor);
            Assert.AreEqual(Theme.Danger,  Ui.Get<Button>(form, "btnDelete").ForeColor);
            Assert.AreEqual(Theme.Surface, Ui.Get<Button>(form, "btnEdit").BackColor);
            Assert.IsTrue(All(form).OfType<Button>().All(b => b.FlatStyle == FlatStyle.Flat));
        }

        [TestMethod]
        public void DisabledPrimaryButton_LooksDisabled_UntilEnabled()
        {
            var form = Open<AppHub.Import.ImportForm>();
            var import = Ui.Get<Button>(form, "btnImport");

            Assert.IsFalse(import.Enabled);
            Assert.AreEqual(Theme.SurfaceAlt, import.BackColor);

            import.Enabled = true;
            Assert.AreEqual(Theme.Primary, import.BackColor);
        }

        [TestMethod]
        public void AcceptButton_IsPrimaryAutomatically()
        {
            var form = Open<ScanForm>();

            Assert.AreEqual(Theme.Primary, Ui.Get<Button>(form, "btnSave").BackColor);
        }

        [TestMethod]
        public void ToolbarButtons_HaveIcons()
        {
            var form = Open<CustomerCRUDForm>();

            foreach (var name in new[] { "btnSearch", "btnAdd", "btnEdit", "btnDelete" })
                Assert.IsNotNull(Ui.Get<Button>(form, name).Image, name);
        }

        [TestMethod]
        public void Grid_ShowsThaiHeadersAndDateFormat()
        {
            var form = Open<CustomerCRUDForm>();
            var grid = Grid(form);

            Assert.AreEqual("รหัสลูกค้า",   grid.Columns["Customer_code"].HeaderText);
            Assert.AreEqual("ชื่อ-นามสกุล", grid.Columns["Full_name"].HeaderText);
            Assert.AreEqual(Theme.DateTimeFormat, grid.Columns["Created_date"].DefaultCellStyle.Format);
            Assert.IsFalse(grid.Columns["Customer_id"].Visible);
        }

        [TestMethod]
        public void NoEmojiInAnyScreen()
        {
            // GDI วาด emoji เป็นกล่องสี่เหลี่ยม — ต้องใช้ไอคอน MDL2 แทน
            var forms = new Form[]
            {
                Open<CustomerCRUDForm>(), Open<UserManagementForm>(), Open<ScanForm>(),
                Open<AppHub.Report.ReportForm>(), Open<AppHub.Import.ImportForm>(),
                Track(new MainForm()), Track(new LoginForm()), Track(new RegisterForm())
            };
            foreach (var form in forms)
                foreach (var c in All(form).Concat(new[] { (Control)form }))
                    Assert.IsFalse(c.Text.Any(char.IsSurrogate) || c.Text.Contains('\uFE0F'),
                        $"{form.Name}.{c.Name}: \"{c.Text}\"");
        }

        // ─── MainForm ────────────────────────────────────────────────────────
        [TestMethod]
        public void OpenModule_IsBorderlessWithCloseButton_AndHighlightsNav()
        {
            ShowMainOffscreen();

            Ui.Click(_main, "mnuCRUD_Click");
            Pump();

            var crud = _main.MdiChildren.OfType<CustomerCRUDForm>().Single();
            Assert.AreEqual(FormBorderStyle.None, crud.FormBorderStyle);
            Assert.AreEqual(Theme.SidebarActive, Ui.Get<Button>(_main, "navCRUD").BackColor, "เมนูที่เปิดอยู่ต้องถูกไฮไลต์");

            var client = _main.Controls.OfType<MdiClient>().Single();
            Assert.AreEqual(client.ClientSize, crud.Size, "หน้า module ต้องเต็มพื้นที่ทำงาน");

            var close = crud.Controls.Find("btnCloseModule", true).Single();
            ((Button)close).PerformClick();
            Pump();
            Assert.AreEqual(0, _main.MdiChildren.OfType<CustomerCRUDForm>().Count());
        }

        [TestMethod]
        public void SwitchingModules_MovesHighlight()
        {
            ShowMainOffscreen();

            Ui.Click(_main, "mnuCRUD_Click");
            Ui.Click(_main, "mnuReport_Click");
            Pump();

            Assert.AreEqual(Theme.SidebarActive, Ui.Get<Button>(_main, "navReport").BackColor);
            Assert.AreEqual(Theme.Sidebar,      Ui.Get<Button>(_main, "navCRUD").BackColor);
        }

        [TestMethod]
        public void MdiDialog_HasModernChrome()
        {
            ShowMainOffscreen();
            Ui.Click(_main, "mnuCRUD_Click");
            var crud = _main.MdiChildren.OfType<CustomerCRUDForm>().Single();

            Ui.Click(crud, "btnAdd_Click");
            Pump();

            var dlg = _main.MdiChildren.OfType<CustomerEditDialog>().Single();
            Assert.AreEqual(FormBorderStyle.None, dlg.FormBorderStyle);
            Assert.AreEqual(Theme.Surface, dlg.BackColor, "dialog ต้องพื้นขาว");
            Assert.AreEqual(dlg.Text, dlg.Controls.Find("DialogTitle", true).Single().Text);

            ((Button)dlg.Controls.Find("DialogClose", true).Single()).PerformClick();
            Pump();
            Assert.AreEqual(0, _main.MdiChildren.OfType<CustomerEditDialog>().Count());
            Assert.IsTrue(crud.Enabled);
        }

        [TestMethod]
        public void ReturningToLockedOwner_BringsItsDialogToFront()
        {
            ShowMainOffscreen();
            Ui.Click(_main, "mnuCRUD_Click");
            var crud = _main.MdiChildren.OfType<CustomerCRUDForm>().Single();
            Ui.Click(crud, "btnAdd_Click");
            Pump();

            Ui.Click(_main, "mnuReport_Click");   // ไปหน้าอื่น → dialog จมหลัง
            Pump();
            Ui.Click(_main, "mnuCRUD_Click");     // กลับมาที่หน้าเดิม
            Pump();

            Assert.IsInstanceOfType(_main.ActiveMdiChild, typeof(CustomerEditDialog),
                "dialog ที่ค้างอยู่ต้องขึ้นมาข้างหน้า ไม่งั้นผู้ใช้เจอหน้าที่ถูกล็อกแต่มองไม่เห็น dialog");
            Assert.AreEqual(Theme.SidebarActive, Ui.Get<Button>(_main, "navCRUD").BackColor,
                "เมนูต้องไฮไลต์หน้าที่เป็นเจ้าของ dialog");
        }
    }
}
