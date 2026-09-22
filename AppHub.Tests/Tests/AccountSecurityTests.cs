using System;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Launcher;
using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests
{
    /// <summary>
    /// TC-LOCK / TC-CPW / TC-RPW — ล็อกบัญชี, เปลี่ยนรหัสผ่าน, Admin รีเซ็ตรหัสผ่าน
    /// </summary>
    [TestClass]
    public class AccountSecurityTests : UiTestBase
    {
        private const string NewPassword = "N3w!Password";

        private LoginForm Login(string username, string password)
        {
            var form = Track(new LoginForm());
            Ui.Get<TextBox>(form, "txtUsername").Text = username;
            Ui.Get<TextBox>(form, "txtPassword").Text = password;
            Ui.Click(form, "btnLogin_Click");
            return form;
        }

        private static int FailedCount(string username)
            => TestDb.Count("SELECT Failed_login_count FROM dbo.t_Users WHERE Username = @u", TestDb.P("@u", username));

        private static object LockedUntil(string username)
            => TestDb.Scalar("SELECT Locked_until FROM dbo.t_Users WHERE Username = @u", TestDb.P("@u", username));

        private static DialogResult FillPasswordDialog(Form dialog, string current, string newPassword, string confirm = null)
        {
            var txtCurrent = Ui.Get<TextBox>(dialog, "txtCurrent");
            if (txtCurrent != null) txtCurrent.Text = current;
            Ui.Get<TextBox>(dialog, "txtNew").Text     = newPassword;
            Ui.Get<TextBox>(dialog, "txtConfirm").Text = confirm ?? newPassword;
            Ui.Click(dialog, "BtnOk_Click");
            return dialog.DialogResult;
        }

        // ─── Lockout ─────────────────────────────────────────────────────────
        [TestMethod, TestCategory("TC-LOCK-01")]
        public void WrongPassword_CountsFailures()
        {
            Login("user1", "wrong");
            Login("user1", "wrong");

            Assert.AreEqual(2, FailedCount("user1"));
            Assert.AreEqual(DBNull.Value, LockedUntil("user1"));
        }

        [TestMethod, TestCategory("TC-LOCK-02")]
        public void FiveWrongPasswords_LocksAccount()
        {
            for (int i = 0; i < AccountRules.MaxFailedLogins; i++)
                Login("user1", "wrong");

            AssertMessage(MessageBoxIcon.Error, "บัญชีถูกล็อกชั่วคราว");
            var until = (DateTime)LockedUntil("user1");
            Assert.IsTrue(until > DateTime.Now.AddMinutes(AccountRules.LockoutMinutes - 1));
        }

        [TestMethod, TestCategory("TC-LOCK-03")]
        public void LockedAccount_RejectsCorrectPassword()
        {
            for (int i = 0; i < AccountRules.MaxFailedLogins; i++)
                Login("user1", "wrong");
            Messages.Clear();

            var form = Login("user1", "user1234");

            Assert.AreNotEqual(DialogResult.OK, form.DialogResult);
            AssertMessage(MessageBoxIcon.Error, "บัญชีถูกล็อกชั่วคราว");
            Assert.IsFalse(AppSession.IsLoggedIn);
        }

        [TestMethod, TestCategory("TC-LOCK-04")]
        public void ExpiredLock_AllowsLoginAndResetsCounter()
        {
            TestDb.Execute("UPDATE dbo.t_Users SET Failed_login_count = 5, " +
                           "Locked_until = DATEADD(MINUTE, -1, GETDATE()) WHERE Username = 'user1'");

            var form = Login("user1", "user1234");

            Assert.AreEqual(DialogResult.OK, form.DialogResult);
            Assert.AreEqual(0, FailedCount("user1"));
            Assert.AreEqual(DBNull.Value, LockedUntil("user1"));
        }

        [TestMethod, TestCategory("TC-LOCK-04")]
        public void ExpiredLock_WrongPasswordStartsCountingAgain()
        {
            TestDb.Execute("UPDATE dbo.t_Users SET Failed_login_count = 5, " +
                           "Locked_until = DATEADD(MINUTE, -1, GETDATE()) WHERE Username = 'user1'");

            Login("user1", "wrong");

            Assert.AreEqual(1, FailedCount("user1"), "หมดเวลาล็อกแล้วต้องเริ่มนับ 1 ใหม่ ไม่ล็อกทันที");
            Assert.AreEqual(DBNull.Value, LockedUntil("user1"));
        }

        [TestMethod, TestCategory("TC-LOCK-05")]
        public void SuccessfulLogin_ResetsCounterAndRecordsLastLogin()
        {
            Login("user1", "wrong");
            Login("user1", "user1234");

            Assert.AreEqual(0, FailedCount("user1"));
            var last = (DateTime)TestDb.Scalar("SELECT Last_login_date FROM dbo.t_Users WHERE Username = 'user1'");
            Assert.IsTrue((DateTime.Now - last).TotalMinutes < 1);
        }

        [TestMethod]
        public void UnknownUser_DoesNotRevealLockout()
        {
            for (int i = 0; i < AccountRules.MaxFailedLogins + 1; i++)
                Login("ghost", "wrong");

            AssertMessage(MessageBoxIcon.Error, "Username หรือ Password ไม่ถูกต้อง");
            Assert.IsFalse(Messages.Exists(m => m.Text.Contains("ล็อก")));
        }

        // ─── Change own password ─────────────────────────────────────────────
        private MainForm SignedInMain(string username)
        {
            SignInAs(username);
            return Track(new MainForm());
        }

        [TestMethod, TestCategory("TC-CPW-01")]
        public void ChangePassword_Success_NewWorksOldDoesNot()
        {
            var main = SignedInMain("user1");
            DialogHandlers.Enqueue(d => FillPasswordDialog(d, "user1234", NewPassword));

            Ui.Click(main, "mnuChangePassword_Click");

            AssertMessage(MessageBoxIcon.Information, "เปลี่ยนรหัสผ่านสำเร็จ");
            Assert.AreNotEqual(DialogResult.OK, Login("user1", "user1234").DialogResult);
            Assert.AreEqual(DialogResult.OK, Login("user1", NewPassword).DialogResult);
        }

        [TestMethod, TestCategory("TC-CPW-02")]
        public void ChangePassword_WrongCurrent_ShowsError()
        {
            var main = SignedInMain("user1");
            DialogHandlers.Enqueue(d => FillPasswordDialog(d, "not-my-password", NewPassword));

            Ui.Click(main, "mnuChangePassword_Click");

            AssertMessage(MessageBoxIcon.Error, "รหัสผ่านปัจจุบันไม่ถูกต้อง");
            Assert.AreEqual(TestDb.Sha256("user1234"),
                TestDb.Scalar("SELECT Password_hash FROM dbo.t_Users WHERE Username = 'user1'"));
        }

        [TestMethod, TestCategory("TC-CPW-03")]
        public void ChangePassword_WeakNew_RejectedInDialog()
        {
            var main = SignedInMain("user1");
            DialogResult result = DialogResult.None;
            DialogHandlers.Enqueue(d => result = FillPasswordDialog(d, "user1234", "weakpass"));

            Ui.Click(main, "mnuChangePassword_Click");

            Assert.AreNotEqual(DialogResult.OK, result);
            AssertMessage(MessageBoxIcon.Warning, "ยังไม่ตรงเงื่อนไข");
        }

        [TestMethod, TestCategory("TC-CPW-03")]
        public void ChangePassword_SameAsCurrent_Rejected()
        {
            TestDb.Execute("UPDATE dbo.t_Users SET Password_hash = @h WHERE Username = 'user1'",
                TestDb.P("@h", TestDb.Sha256(NewPassword)));
            var main = SignedInMain("user1");
            DialogHandlers.Enqueue(d => FillPasswordDialog(d, NewPassword, NewPassword));

            Ui.Click(main, "mnuChangePassword_Click");

            AssertMessage(MessageBoxIcon.Warning, "ต้องไม่ซ้ำ");
        }

        [TestMethod, TestCategory("TC-CPW-03")]
        public void ChangePassword_ConfirmMismatch_Rejected()
        {
            var main = SignedInMain("user1");
            DialogHandlers.Enqueue(d => FillPasswordDialog(d, "user1234", NewPassword, NewPassword + "x"));

            Ui.Click(main, "mnuChangePassword_Click");

            AssertMessage(MessageBoxIcon.Warning, "ไม่ตรงกัน");
        }

        // ─── Admin reset ─────────────────────────────────────────────────────
        [TestMethod, TestCategory("TC-RPW-01")]
        public void AdminReset_SetsPasswordAndUnlocks()
        {
            TestDb.Execute("UPDATE dbo.t_Users SET Failed_login_count = 5, " +
                           "Locked_until = DATEADD(MINUTE, 15, GETDATE()) WHERE Username = 'user1'");
            SignInAs("admin");
            var form = Open<UserManagementForm>();
            Ui.SelectRow(Grid(form), "Username", "user1");
            bool hasCurrentField = true;
            DialogHandlers.Enqueue(d =>
            {
                hasCurrentField = Ui.Get<TextBox>(d, "txtCurrent") != null;
                return FillPasswordDialog(d, null, NewPassword);
            });

            Ui.Click(form, "btnResetPassword_Click");

            Assert.IsFalse(hasCurrentField, "Admin รีเซ็ตไม่ต้องรู้รหัสเดิม");
            AssertMessage(MessageBoxIcon.Information, "รีเซ็ตรหัสผ่าน");
            Assert.AreEqual(0, FailedCount("user1"));
            Assert.AreEqual(DBNull.Value, LockedUntil("user1"));
            AppSession.Clear();
            Assert.AreEqual(DialogResult.OK, Login("user1", NewPassword).DialogResult);
        }

        [TestMethod, TestCategory("TC-RPW-02")]
        public void AdminReset_WeakPassword_Rejected()
        {
            SignInAs("admin");
            var form = Open<UserManagementForm>();
            Ui.SelectRow(Grid(form), "Username", "user1");
            DialogHandlers.Enqueue(d => FillPasswordDialog(d, null, "short"));

            Ui.Click(form, "btnResetPassword_Click");

            AssertMessage(MessageBoxIcon.Warning, "ยังไม่ตรงเงื่อนไข");
            Assert.AreEqual(TestDb.Sha256("user1234"),
                TestDb.Scalar("SELECT Password_hash FROM dbo.t_Users WHERE Username = 'user1'"));
        }

        [TestMethod, TestCategory("TC-RPW-03")]
        public void AdminAddUser_WeakPassword_Rejected()
        {
            SignInAs("admin");
            var form = Open<UserManagementForm>();
            DialogHandlers.Enqueue(d =>
            {
                Ui.Get<TextBox>(d, "txtUsername").Text = "user9";
                Ui.Get<TextBox>(d, "txtPassword").Text = "test1234";
                Ui.Get<TextBox>(d, "txtConfirm").Text  = "test1234";
                Ui.Click(d, "BtnOk_Click");
                return d.DialogResult;
            });

            Ui.Click(form, "btnAdd_Click");

            AssertMessage(MessageBoxIcon.Warning, "ยังไม่ตรงเงื่อนไข");
            Assert.AreEqual(2, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Users"));
        }

        [TestMethod]
        public void UserGrid_ShowsLastLoginAndLockColumns()
        {
            SignInAs("admin");
            var form = Open<UserManagementForm>();

            Assert.IsNotNull(Grid(form).Columns["Last_login_date"]);
            Assert.IsNotNull(Grid(form).Columns["Locked_until"]);
        }
    }
}
