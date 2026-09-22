using System.Windows.Forms;
using AppHub.Core;
using AppHub.Launcher;
using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests
{
    /// <summary>
    /// TC-REG — สมัครสมาชิก
    /// </summary>
    [TestClass]
    public class RegisterTests : UiTestBase
    {
        private const string StrongPassword = "Str0ng!Pass";

        private RegisterForm Register(string fullName, string username, string password, string confirm = null)
        {
            var form = Track(new RegisterForm());
            Fill(form, fullName, username, password, confirm);
            Ui.Click(form, "btnRegister_Click");
            return form;
        }

        private static void Fill(RegisterForm form, string fullName, string username, string password, string confirm = null)
        {
            Ui.Get<TextBox>(form, "txtFullName").Text = fullName;
            Ui.Get<TextBox>(form, "txtUsername").Text = username;
            Ui.Get<TextBox>(form, "txtPassword").Text = password;
            Ui.Get<TextBox>(form, "txtConfirm").Text  = confirm ?? password;
        }

        private static int UserCount() => TestDb.Count("SELECT COUNT(*) FROM dbo.t_Users");

        [TestMethod, TestCategory("TC-REG-01")]
        public void Register_Success_CreatesActiveUserWithoutModules()
        {
            var form = Register("สมศรี ทดสอบ", "somsri", StrongPassword);

            Assert.AreEqual(DialogResult.OK, form.DialogResult);
            Assert.AreEqual("somsri", form.RegisteredUsername);
            AssertMessage(MessageBoxIcon.Information, "สมัครสมาชิกสำเร็จ");

            var row = TestDb.Query("SELECT Full_name, Password_hash, Is_admin, Is_active, User_id " +
                                   "FROM dbo.t_Users WHERE Username = 'somsri'").Rows[0];
            Assert.AreEqual("สมศรี ทดสอบ", row["Full_name"]);
            Assert.AreEqual(TestDb.Sha256(StrongPassword), row["Password_hash"]);
            Assert.AreEqual(false, row["Is_admin"]);
            Assert.AreEqual(true,  row["Is_active"]);
            Assert.AreEqual(0, TestDb.Count("SELECT COUNT(*) FROM dbo.t_UserModule WHERE User_id = @id",
                TestDb.P("@id", row["User_id"])));
        }

        [TestMethod, TestCategory("TC-REG-01")]
        public void Register_ThenLogin_SeesNoModulesAndNotice()
        {
            Register("สมศรี ทดสอบ", "somsri", StrongPassword);

            var login = Track(new LoginForm());
            Ui.Get<TextBox>(login, "txtUsername").Text = "somsri";
            Ui.Get<TextBox>(login, "txtPassword").Text = StrongPassword;
            Ui.Click(login, "btnLogin_Click");
            Assert.AreEqual(DialogResult.OK, login.DialogResult);
            Assert.IsFalse(AppSession.HasAnyModule);

            var main = Track(new MainForm());
            foreach (var item in new[] { "mnuCRUD", "mnuImport", "mnuReport", "mnuScan" })
                Assert.IsFalse(Ui.Get<ToolStripItem>(main, item).Enabled, item);
            Assert.IsFalse(Ui.Get<ToolStripItem>(main, "mnuUserManagement").Available);

            Ui.Call(main, "OnShown", System.EventArgs.Empty);
            AssertMessage(MessageBoxIcon.Information, "ยังไม่ได้รับสิทธิ์");
        }

        [TestMethod, TestCategory("TC-REG-01")]
        public void LoginLink_OpensRegister_AndPrefillsUsername()
        {
            var login = Track(new LoginForm());
            DialogHandlers.Enqueue(d =>
            {
                var reg = (RegisterForm)d;
                Fill(reg, "สมศรี ทดสอบ", "somsri", StrongPassword);
                Ui.Click(reg, "btnRegister_Click");
                return reg.DialogResult;
            });

            Ui.Call(login, "lnkRegister_LinkClicked", login, null);

            Assert.AreEqual("somsri", Ui.Get<TextBox>(login, "txtUsername").Text);
            Assert.AreEqual(3, UserCount());
        }

        [TestMethod, TestCategory("TC-REG-02")]
        public void Register_DuplicateUsername_ShowsWarning()
        {
            var form = Register("ซ้ำ", "user1", StrongPassword);

            Assert.AreNotEqual(DialogResult.OK, form.DialogResult);
            AssertMessage(MessageBoxIcon.Warning, "มีผู้ใช้แล้ว");
            Assert.AreEqual(2, UserCount());
        }

        [DataTestMethod, TestCategory("TC-REG-03")]
        [DataRow("Abc!efg",   "อย่างน้อย 8")]
        [DataRow("abcdefg!",  "พิมพ์ใหญ่")]
        [DataRow("ABCDEFG!",  "พิมพ์เล็ก")]
        [DataRow("Abcdefgh1", "อักขระพิเศษ")]
        public void Register_WeakPassword_Rejected(string password, string expected)
        {
            var form = Register("ทดสอบ", "weakuser", password);

            Assert.AreNotEqual(DialogResult.OK, form.DialogResult);
            AssertMessage(MessageBoxIcon.Warning, expected);
            Assert.AreEqual(2, UserCount());
        }

        [TestMethod, TestCategory("TC-REG-04")]
        public void Register_ConfirmMismatch_Rejected()
        {
            Register("ทดสอบ", "newuser", StrongPassword, StrongPassword + "x");

            AssertMessage(MessageBoxIcon.Warning, "ไม่ตรงกัน");
            Assert.AreEqual(2, UserCount());
        }

        [TestMethod, TestCategory("TC-REG-05")]
        public void Register_EmptyFullName_Rejected()
        {
            Register("   ", "newuser", StrongPassword);

            AssertMessage(MessageBoxIcon.Warning, "กรุณากรอกชื่อ-นามสกุล");
            Assert.AreEqual(2, UserCount());
        }

        [DataTestMethod, TestCategory("TC-REG-05")]
        [DataRow("")]
        [DataRow("ab")]
        [DataRow("มีภาษาไทย")]
        [DataRow("has space")]
        public void Register_InvalidUsername_Rejected(string username)
        {
            Register("ทดสอบ", username, StrongPassword);

            AssertMessage(MessageBoxIcon.Warning, "Username");
            Assert.AreEqual(2, UserCount());
        }

        [TestMethod]
        public void PasswordChecklist_UpdatesWhileTyping()
        {
            var form = Track(new RegisterForm());
            var rules = Ui.Get<PasswordRulesView>(form, "pwdRules");
            var box   = Ui.Get<TextBox>(form, "txtPassword");

            box.Text = "abc";
            Assert.IsFalse(rules.AllPassed);

            box.Text = StrongPassword;
            Assert.IsTrue(rules.AllPassed);
        }
    }
}
