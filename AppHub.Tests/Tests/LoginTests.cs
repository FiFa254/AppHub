using System.Windows.Forms;
using AppHub.Core;
using AppHub.Launcher;
using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests
{
    /// <summary>
    /// TC-L — Login / Authentication (+ สิทธิ์ใน MainForm)
    /// </summary>
    [TestClass]
    public class LoginTests : UiTestBase
    {
        private LoginForm Login(string username, string password)
        {
            var form = Track(new LoginForm());
            Ui.Get<TextBox>(form, "txtUsername").Text = username;
            Ui.Get<TextBox>(form, "txtPassword").Text = password;
            Ui.Click(form, "btnLogin_Click");
            return form;
        }

        private MainForm OpenMain() => Track(new MainForm());

        private static bool Enabled(MainForm main, string item)
        {
            object target = Ui.Get<object>(main, item);
            return target is ToolStripItem menu ? menu.Enabled : ((Control)target).Enabled;
        }

        /// <summary>ตั้งให้แสดงไหม (ไม่ขึ้นกับว่า form แสดงบนจออยู่หรือเปล่า)</summary>
        private static bool Available(MainForm main, string item)
        {
            object target = Ui.Get<object>(main, item);
            return target is ToolStripItem menu ? menu.Available : Ui.IsSetVisible((Control)target);
        }

        [TestMethod, TestCategory("TC-L-01")]
        public void Admin_Login_SeesAllModulesAndUserManagement()
        {
            var form = Login("admin", "admin1234");

            Assert.AreEqual(DialogResult.OK, form.DialogResult);
            Assert.IsTrue(AppSession.IsAdmin);
            Assert.AreEqual("admin", AppSession.Username);

            var main = OpenMain();
            foreach (var item in new[] { "mnuCRUD", "mnuImport", "mnuReport", "mnuScan",
                                         "navCRUD", "navImport", "navReport", "navScan" })
                Assert.IsTrue(Enabled(main, item), item);
            Assert.IsTrue(Available(main, "mnuUserManagement"));
            Assert.IsTrue(Available(main, "navUsers"));
        }

        [TestMethod, TestCategory("TC-L-02")]
        public void User1_Login_SeesOnlyCrudAndReport()
        {
            var form = Login("user1", "user1234");
            Assert.AreEqual(DialogResult.OK, form.DialogResult);
            Assert.IsFalse(AppSession.IsAdmin);

            var main = OpenMain();
            Assert.IsTrue(Enabled(main, "mnuCRUD"));
            Assert.IsTrue(Enabled(main, "mnuReport"));
            Assert.IsFalse(Enabled(main, "mnuImport"));
            Assert.IsFalse(Enabled(main, "mnuScan"));
            Assert.IsFalse(Enabled(main, "navImport"));
            Assert.IsFalse(Enabled(main, "navScan"));
        }

        [TestMethod, TestCategory("TC-L-03")]
        public void WrongPassword_ShowsError()
        {
            var form = Login("admin", "wrongpass");

            Assert.AreNotEqual(DialogResult.OK, form.DialogResult);
            AssertMessage(MessageBoxIcon.Error, "Username หรือ Password ไม่ถูกต้อง");
            Assert.IsFalse(AppSession.IsLoggedIn);
        }

        [TestMethod, TestCategory("TC-L-04")]
        public void UnknownUser_ShowsSameError()
        {
            var form = Login("ghost", "anything");

            Assert.AreNotEqual(DialogResult.OK, form.DialogResult);
            AssertMessage(MessageBoxIcon.Error, "Username หรือ Password ไม่ถูกต้อง");
        }

        [TestMethod, TestCategory("TC-L-05")]
        public void EmptyFields_ShowsWarning()
        {
            var form = Login("", "");

            Assert.AreNotEqual(DialogResult.OK, form.DialogResult);
            AssertMessage(MessageBoxIcon.Warning, "กรุณากรอก Username และ Password");
        }

        [TestMethod, TestCategory("TC-L-06")]
        public void Logout_ClosesMainAndSessionCanBeCleared()
        {
            Login("admin", "admin1234");
            var main = OpenMain();
            ConfirmAnswers.Enqueue(true);

            Ui.Click(main, "mnuLogout_Click");

            Assert.IsTrue(main.IsLogout);
            // Program.Main เรียก AppSession.Clear() หลัง MainForm ปิดด้วย Logout
            AppSession.Clear();
            Assert.IsFalse(AppSession.IsLoggedIn);
            Assert.IsFalse(AppSession.HasPermission("CRUD"));
        }

        [TestMethod, TestCategory("TC-L-06")]
        public void Logout_Cancelled_StaysLoggedIn()
        {
            Login("admin", "admin1234");
            var main = OpenMain();
            ConfirmAnswers.Enqueue(false);

            Ui.Click(main, "mnuLogout_Click");

            Assert.IsFalse(main.IsLogout);
        }

        [TestMethod, TestCategory("TC-L-07")]
        public void InactiveUser_CannotLogin()
        {
            TestDb.Execute("UPDATE dbo.t_Users SET Is_active = 0 WHERE Username = 'user1'");

            var form = Login("user1", "user1234");

            Assert.AreNotEqual(DialogResult.OK, form.DialogResult);
            AssertMessage(MessageBoxIcon.Error, "Username หรือ Password ไม่ถูกต้อง");
        }

        [TestMethod, TestCategory("TC-U-09")]
        public void User1_DoesNotSeeUserManagement()
        {
            Login("user1", "user1234");
            var main = OpenMain();

            Assert.IsFalse(Available(main, "mnuUserManagement"));
            Assert.IsFalse(Available(main, "navUsers"));
        }

        [TestMethod]
        public void OpeningModuleWithoutPermission_IsBlocked()
        {
            Login("user1", "user1234");
            var main = OpenMain();

            Ui.Click(main, "mnuScan_Click");

            AssertMessage(MessageBoxIcon.Warning, "ไม่มีสิทธิ์");
            Assert.AreEqual(0, main.MdiChildren.Length);
        }
    }
}
