using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AppHub.Launcher;
using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests
{
    /// <summary>
    /// TC-U — User Management (Admin)
    /// </summary>
    [TestClass]
    public class UserManagementTests : UiTestBase
    {
        [TestInitialize]
        public void SignIn() => SignInAs("admin");

        // ─── Dialog helpers ──────────────────────────────────────────────────
        private static DialogResult FillUserDialog(Form dialog, string username, string password,
            string fullName, params string[] modules)
        {
            Ui.Get<TextBox>(dialog, "txtUsername").Text = username;
            Ui.Get<TextBox>(dialog, "txtPassword").Text = password;
            Ui.Get<TextBox>(dialog, "txtConfirm").Text  = password;
            Ui.Get<TextBox>(dialog, "txtFullName").Text = fullName;
            CheckModules(dialog, modules);
            Ui.Click(dialog, "BtnOk_Click");
            return dialog.DialogResult;
        }

        private static void CheckModules(Form dialog, params string[] modules)
        {
            foreach (var chk in Ui.Get<List<CheckBox>>(dialog, "_chkModules"))
                chk.Checked = modules.Contains((string)chk.Tag);
        }

        private static List<string> ModulesOf(string username)
            => TestDb.Query("SELECT m.Module_code FROM dbo.t_UserModule m JOIN dbo.t_Users u " +
                            "ON u.User_id = m.User_id WHERE u.Username = @u ORDER BY m.Module_code",
                            TestDb.P("@u", username))
                     .AsEnumerable().Select(r => (string)r[0]).ToList();

        // ─── Tests ───────────────────────────────────────────────────────────
        [TestMethod, TestCategory("TC-U-01")]
        public void Open_LoadsAllUsers()
        {
            var form = Open<UserManagementForm>();

            Assert.AreEqual(2, GridRowCount(form));
            Assert.IsFalse(Grid(form).Columns["User_id"].Visible);
            StringAssert.Contains(Ui.Get<Label>(form, "LblCount").Text, "2");
            AssertNoError();
        }

        [TestMethod, TestCategory("TC-U-02")]
        public void AddUser_WithModules_AppearsInGridAndDb()
        {
            var form = Open<UserManagementForm>();
            DialogHandlers.Enqueue(d => FillUserDialog(d, "user2", "test1234", "User Two", "CRUD", "IMPORT"));

            Ui.Click(form, "btnAdd_Click");

            AssertNoError();
            Assert.AreEqual(3, GridRowCount(form));
            Assert.AreEqual(TestDb.Sha256("test1234"),
                TestDb.Scalar("SELECT Password_hash FROM dbo.t_Users WHERE Username = 'user2'"));
            CollectionAssert.AreEqual(new[] { "CRUD", "IMPORT" }, ModulesOf("user2"));
        }

        [TestMethod, TestCategory("TC-U-03")]
        public void AddUser_DuplicateUsername_ShowsWarning()
        {
            var form = Open<UserManagementForm>();
            DialogHandlers.Enqueue(d => FillUserDialog(d, "user1", "x1234", "Dup"));

            Ui.Click(form, "btnAdd_Click");

            AssertMessage(MessageBoxIcon.Warning, "มีอยู่ในระบบแล้ว");
            Assert.AreEqual(2, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Users"));
        }

        [TestMethod, TestCategory("TC-U-04")]
        public void AddUser_EmptyUsername_ValidatesInDialog()
        {
            var form = Open<UserManagementForm>();
            DialogHandlers.Enqueue(d => FillUserDialog(d, "", "x1234", "No name"));

            Ui.Click(form, "btnAdd_Click");

            AssertMessage(MessageBoxIcon.Warning, "กรุณากรอก Username และ Password");
            Assert.AreEqual(2, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Users"));
        }

        [TestMethod]
        public void AddUser_PasswordMismatch_ValidatesInDialog()
        {
            var form = Open<UserManagementForm>();
            DialogHandlers.Enqueue(d =>
            {
                Ui.Get<TextBox>(d, "txtUsername").Text = "user3";
                Ui.Get<TextBox>(d, "txtPassword").Text = "aaaa";
                Ui.Get<TextBox>(d, "txtConfirm").Text  = "bbbb";
                Ui.Click(d, "BtnOk_Click");
                return d.DialogResult;
            });

            Ui.Click(form, "btnAdd_Click");

            AssertMessage(MessageBoxIcon.Warning, "ไม่ตรงกัน");
            Assert.AreEqual(2, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Users"));
        }

        [TestMethod]
        public void AddUserDialog_ListsModulesFromDb()
        {
            TestDb.Execute("INSERT INTO dbo.t_Modules (Module_code, Module_name, Sort_order) VALUES ('AUDIT', N'ตรวจสอบ', 9)");
            var form = Open<UserManagementForm>();
            List<string> shown = null;
            DialogHandlers.Enqueue(d =>
            {
                shown = Ui.Get<List<CheckBox>>(d, "_chkModules").Select(c => (string)c.Tag).ToList();
                return DialogResult.Cancel;
            });

            Ui.Click(form, "btnAdd_Click");

            CollectionAssert.AreEqual(new[] { "CRUD", "IMPORT", "REPORT", "SCAN", "AUDIT" }, shown);
        }

        [TestMethod, TestCategory("TC-U-05")]
        public void GrantScan_ToUser1()
        {
            var form = Open<UserManagementForm>();
            Ui.SelectRow(Grid(form), "Username", "user1");
            List<string> preChecked = null;
            DialogHandlers.Enqueue(d =>
            {
                preChecked = Ui.Get<List<CheckBox>>(d, "_chkModules")
                               .Where(c => c.Checked).Select(c => (string)c.Tag).ToList();
                CheckModules(d, "CRUD", "REPORT", "SCAN");
                return DialogResult.OK;
            });

            Ui.Click(form, "btnEditPermissions_Click");

            CollectionAssert.AreEqual(new[] { "CRUD", "REPORT" }, preChecked);
            CollectionAssert.AreEqual(new[] { "CRUD", "REPORT", "SCAN" }, ModulesOf("user1"));
            AssertMessage(MessageBoxIcon.Information, "บันทึกสิทธิ์สำเร็จ");

            SignInAs("user1");
            var main = Track(new MainForm());
            Assert.IsTrue(Ui.Get<ToolStripItem>(main, "mnuScan").Enabled);
        }

        [TestMethod, TestCategory("TC-U-06")]
        public void RevokeCrud_FromUser1()
        {
            var form = Open<UserManagementForm>();
            Ui.SelectRow(Grid(form), "Username", "user1");
            DialogHandlers.Enqueue(d => { CheckModules(d, "REPORT"); return DialogResult.OK; });

            Ui.Click(form, "btnEditPermissions_Click");

            CollectionAssert.AreEqual(new[] { "REPORT" }, ModulesOf("user1"));
            SignInAs("user1");
            var main = Track(new MainForm());
            Assert.IsFalse(Ui.Get<ToolStripItem>(main, "mnuCRUD").Enabled);
        }

        [TestMethod, TestCategory("TC-U-07")]
        public void DeactivateUser_ConfirmYes_BlocksLogin()
        {
            var form = Open<UserManagementForm>();
            Ui.SelectRow(Grid(form), "Username", "user1");
            ConfirmAnswers.Enqueue(true);

            Ui.Click(form, "btnToggleActive_Click");

            Assert.AreEqual(false, TestDb.Scalar("SELECT Is_active FROM dbo.t_Users WHERE Username = 'user1'"));

            var login = Track(new LoginForm());
            Ui.Get<TextBox>(login, "txtUsername").Text = "user1";
            Ui.Get<TextBox>(login, "txtPassword").Text = "user1234";
            Ui.Click(login, "btnLogin_Click");
            Assert.AreNotEqual(DialogResult.OK, login.DialogResult);
        }

        [TestMethod, TestCategory("TC-U-08")]
        public void DeactivateUser_ConfirmNo_NoChange()
        {
            var form = Open<UserManagementForm>();
            Ui.SelectRow(Grid(form), "Username", "user1");
            ConfirmAnswers.Enqueue(false);

            Ui.Click(form, "btnToggleActive_Click");

            Assert.AreEqual(true, TestDb.Scalar("SELECT Is_active FROM dbo.t_Users WHERE Username = 'user1'"));
        }

        [TestMethod]
        public void Admin_CannotDeactivateSelf()
        {
            var form = Open<UserManagementForm>();
            Ui.SelectRow(Grid(form), "Username", "admin");

            Ui.Click(form, "btnToggleActive_Click");

            AssertMessage(MessageBoxIcon.Warning, "ไม่สามารถปิดใช้งานบัญชีที่กำลัง login");
            Assert.AreEqual(true, TestDb.Scalar("SELECT Is_active FROM dbo.t_Users WHERE Username = 'admin'"));
        }
    }
}
