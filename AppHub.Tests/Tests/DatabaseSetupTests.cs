using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests
{
    /// <summary>
    /// ตรวจ setup.sql — schema, seed, และรันซ้ำได้
    /// </summary>
    [TestClass]
    public class DatabaseSetupTests : UiTestBase
    {
        [TestMethod]
        public void SetupScript_CanRunTwice()
        {
            TestDb.RunSetupScript();
            TestDb.RunSetupScript();

            Assert.AreEqual(2, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Users"));
            Assert.AreEqual(4, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Modules"));
            Assert.AreEqual(5, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Customers"));
            Assert.AreEqual(6, TestDb.Count("SELECT COUNT(*) FROM dbo.t_UserModule"));
        }

        [TestMethod]
        public void Users_HaveLockoutColumns()
        {
            Assert.AreEqual(0, TestDb.Count("SELECT SUM(Failed_login_count) FROM dbo.t_Users"));
            Assert.AreEqual(0, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Users WHERE Locked_until IS NOT NULL OR Last_login_date IS NOT NULL"));
        }

        [TestMethod]
        public void SeedPasswords_MatchAppSha256()
        {
            Assert.AreEqual(TestDb.Sha256("admin1234"),
                TestDb.Scalar("SELECT Password_hash FROM dbo.t_Users WHERE Username = 'admin'"));
            Assert.AreEqual(TestDb.Sha256("user1234"),
                TestDb.Scalar("SELECT Password_hash FROM dbo.t_Users WHERE Username = 'user1'"));
        }

        [TestMethod]
        public void UserModule_RejectsUnknownModuleCode()
        {
            var ex = Assert.ThrowsException<System.Data.SqlClient.SqlException>(() =>
                TestDb.Execute(
                    "INSERT INTO dbo.t_UserModule (User_id, Module_code) " +
                    "SELECT User_id, 'NOPE' FROM dbo.t_Users WHERE Username = 'user1'"));
            StringAssert.Contains(ex.Message, "FK_t_UserModule_Module");
        }

        [TestMethod]
        public void User1_HasCrudAndReportOnly()
        {
            var codes = TestDb.Query(
                "SELECT m.Module_code FROM dbo.t_UserModule m JOIN dbo.t_Users u ON u.User_id = m.User_id " +
                "WHERE u.Username = 'user1' ORDER BY m.Module_code");
            Assert.AreEqual(2, codes.Rows.Count);
            Assert.AreEqual("CRUD",   codes.Rows[0][0]);
            Assert.AreEqual("REPORT", codes.Rows[1][0]);
        }
    }
}
