using System;
using System.Linq;
using System.Windows.Forms;
using AppHub.Core.UI;
using AppHub.CRUD;
using AppHub.Launcher;
using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests
{
    /// <summary>
    /// Dialog เปิดเป็นหน้าต่างลูกใน MainForm (MDI จริง — ไม่ผ่าน hook ของ test)
    /// </summary>
    [TestClass]
    public class MdiDialogTests : UiTestBase
    {
        private MainForm _main;

        [TestInitialize]
        public void UseRealMdiDialogs()
        {
            // ใช้ ShowChild ของจริง แต่ยังดัก MessageBox ไว้
            var showMessage = UiServices.ShowMessage;
            UiServices.Reset();
            UiServices.ShowMessage = showMessage;
            UiServices.ShowDialog  = (d, o) => { Assert.Fail("ไม่ควรเปิด modal: " + d.Text); return DialogResult.None; };

            // แสดง MainForm จริงแต่อยู่นอกจอ — หน้าต่างลูกจะได้ handle เหมือนตอนใช้งานจริง
            SignInAs("admin");
            _main = Track(new MainForm());
            _main.WindowState   = FormWindowState.Normal;
            _main.StartPosition = FormStartPosition.Manual;
            _main.Location      = new System.Drawing.Point(-20000, -20000);
            _main.ShowInTaskbar = false;
            _main.Show();
        }

        private T OpenChild<T>() where T : Form, new()
        {
            var form = new T { MdiParent = _main };
            form.Show();
            return form;
        }

        private T OpenDialog<T>() where T : Form
            => _main.MdiChildren.OfType<T>().SingleOrDefault();

        private static void PressCancel(Form dialog)
            => Ui.Call(Ui.Get<Button>(dialog, "btnCancel"), "OnClick", EventArgs.Empty);

        [TestMethod]
        public void AddCustomer_OpensDialogInsideMainForm_AndLocksOwner()
        {
            var crud = OpenChild<CustomerCRUDForm>();

            Ui.Click(crud, "btnAdd_Click");

            var dlg = OpenDialog<CustomerEditDialog>();
            Assert.IsNotNull(dlg, "dialog ต้องเป็นหน้าต่างลูกของ MainForm");
            Assert.AreSame(_main, dlg.MdiParent);
            Assert.IsFalse(dlg.Modal);
            Assert.IsFalse(crud.Enabled, "หน้าเจ้าของต้องถูกล็อกระหว่างเปิด dialog");
            Assert.IsTrue(_main.Enabled, "MainForm และหน้าอื่นต้องยังใช้ได้");
        }

        [TestMethod]
        public void AddCustomer_Ok_SavesAndUnlocksOwner()
        {
            var crud = OpenChild<CustomerCRUDForm>();
            Ui.Click(crud, "btnAdd_Click");
            var dlg = OpenDialog<CustomerEditDialog>();

            Ui.Get<TextBox>(dlg, "txtCode").Text = "C777";
            Ui.Get<TextBox>(dlg, "txtName").Text = "ลูกค้า MDI";
            Ui.Click(dlg, "BtnOk_Click");

            AssertNoError();
            Assert.IsNull(OpenDialog<CustomerEditDialog>(), "กด OK แล้ว dialog ต้องปิด");
            Assert.IsTrue(crud.Enabled);
            Assert.AreEqual(1, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Customers WHERE Customer_code = 'C777'"));
            Assert.AreEqual(6, GridRowCount(crud));
        }

        [TestMethod]
        public void AddCustomer_ValidationError_KeepsDialogOpen()
        {
            var crud = OpenChild<CustomerCRUDForm>();
            Ui.Click(crud, "btnAdd_Click");
            var dlg = OpenDialog<CustomerEditDialog>();

            Ui.Click(dlg, "BtnOk_Click");   // ไม่กรอกอะไร

            AssertMessage(MessageBoxIcon.Warning, "กรุณากรอกรหัสลูกค้า");
            Assert.IsNotNull(OpenDialog<CustomerEditDialog>(), "กรอกไม่ครบต้องยังเปิดให้แก้ต่อ");
            Assert.IsFalse(crud.Enabled);
        }

        [TestMethod]
        public void CancelButton_ClosesDialogWithoutSaving()
        {
            var crud = OpenChild<CustomerCRUDForm>();
            Ui.Click(crud, "btnAdd_Click");
            var dlg = OpenDialog<CustomerEditDialog>();
            Ui.Get<TextBox>(dlg, "txtCode").Text = "C778";
            Ui.Get<TextBox>(dlg, "txtName").Text = "ไม่บันทึก";

            PressCancel(dlg);

            Assert.IsNull(OpenDialog<CustomerEditDialog>());
            Assert.IsTrue(crud.Enabled);
            Assert.AreEqual(0, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Customers WHERE Customer_code = 'C778'"));
        }

        [TestMethod]
        public void CloseByX_TreatedAsCancel()
        {
            var crud = OpenChild<CustomerCRUDForm>();
            Ui.Click(crud, "btnAdd_Click");
            var dlg = OpenDialog<CustomerEditDialog>();
            Ui.Get<TextBox>(dlg, "txtCode").Text = "C779";
            Ui.Get<TextBox>(dlg, "txtName").Text = "ปิดด้วย X";

            dlg.Close();

            Assert.IsTrue(crud.Enabled);
            Assert.AreEqual(0, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Customers WHERE Customer_code = 'C779'"));
        }

        [TestMethod]
        public void Dialog_IsCenteredInMainForm()
        {
            _main.ClientSize = new System.Drawing.Size(1000, 700);
            var crud = OpenChild<CustomerCRUDForm>();
            Ui.Click(crud, "btnAdd_Click");
            var dlg = OpenDialog<CustomerEditDialog>();

            var client = _main.Controls.OfType<MdiClient>().Single().ClientSize;
            Assert.AreEqual((client.Width  - dlg.Width)  / 2, dlg.Left);
            Assert.AreEqual((client.Height - dlg.Height) / 2, dlg.Top);
        }

        [TestMethod]
        public void PermissionDialog_OkButtonClosesAndSaves()
        {
            var users = OpenChild<UserManagementForm>();
            Ui.SelectRow(Grid(users), "Username", "user1");
            Ui.Click(users, "btnEditPermissions_Click");
            var dlg = OpenDialog<PermissionDialog>();
            Assert.IsNotNull(dlg);
            Assert.IsFalse(users.Enabled);

            foreach (var chk in Ui.Get<System.Collections.Generic.List<CheckBox>>(dlg, "_chkModules"))
                chk.Checked = (string)chk.Tag == "SCAN";
            Ui.Call(Ui.Get<Button>(dlg, "btnOk"), "OnClick", EventArgs.Empty);   // ปุ่มที่ตั้ง DialogResult = OK

            Assert.IsNull(OpenDialog<PermissionDialog>());
            Assert.IsTrue(users.Enabled);
            Assert.AreEqual("SCAN", TestDb.Scalar(
                "SELECT m.Module_code FROM dbo.t_UserModule m JOIN dbo.t_Users u ON u.User_id = m.User_id WHERE u.Username = 'user1'"));
        }

        [TestMethod]
        public void ResetPasswordDialog_OpensInsideMainForm()
        {
            var users = OpenChild<UserManagementForm>();
            Ui.SelectRow(Grid(users), "Username", "user1");

            Ui.Click(users, "btnResetPassword_Click");

            var dlg = OpenDialog<ChangePasswordDialog>();
            Assert.IsNotNull(dlg);
            PressCancel(dlg);
            Assert.IsTrue(users.Enabled);
        }

        [TestMethod]
        public void ChangePassword_FromMenu_OpensOnceInsideMainForm()
        {
            Ui.Click(_main, "mnuChangePassword_Click");
            Ui.Click(_main, "mnuChangePassword_Click");

            Assert.AreEqual(1, _main.MdiChildren.OfType<ChangePasswordDialog>().Count(), "กดซ้ำต้องไม่เปิดหน้าต่างที่ 2");
            Assert.IsTrue(_main.Enabled, "MainForm ต้องไม่ถูกล็อก");
        }

        [TestMethod]
        public void ChangePassword_FromMenu_SavesOnOk()
        {
            Ui.Click(_main, "mnuChangePassword_Click");
            var dlg = OpenDialog<ChangePasswordDialog>();

            Ui.Get<TextBox>(dlg, "txtCurrent").Text = "admin1234";
            Ui.Get<TextBox>(dlg, "txtNew").Text     = "N3w!Password";
            Ui.Get<TextBox>(dlg, "txtConfirm").Text = "N3w!Password";
            Ui.Click(dlg, "BtnOk_Click");

            AssertMessage(MessageBoxIcon.Information, "เปลี่ยนรหัสผ่านสำเร็จ");
            Assert.AreEqual(TestDb.Sha256("N3w!Password"),
                TestDb.Scalar("SELECT Password_hash FROM dbo.t_Users WHERE Username = 'admin'"));
        }

        [TestMethod]
        public void DialogDisposedWithoutClosing_StillUnlocksOwner()
        {
            var crud = OpenChild<CustomerCRUDForm>();
            Ui.Click(crud, "btnAdd_Click");

            OpenDialog<CustomerEditDialog>().Dispose();

            Assert.IsTrue(crud.Enabled);
        }

        [TestMethod]
        public void RegisterFromLogin_StaysModal()
        {
            bool modalUsed = false;
            UiServices.ShowDialog = (d, o) => { modalUsed = true; return DialogResult.Cancel; };
            var login = Track(new LoginForm());

            Ui.Call(login, "lnkRegister_LinkClicked", login, null);

            Assert.IsTrue(modalUsed, "หน้าสมัครสมาชิกเปิดก่อนมี MainForm จึงต้องเป็น modal");
        }
    }
}
