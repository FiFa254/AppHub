using System.Linq;
using AppHub.Core;
using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests
{
    /// <summary>
    /// TC-PW — กฎรหัสผ่าน / username (ไม่ใช้ DB)
    /// </summary>
    [TestClass]
    public class AccountRulesTests
    {
        [DataTestMethod, TestCategory("TC-PW-01")]
        [DataRow("Str0ng!Pass")]
        [DataRow("Abcdefg!")]           // 8 ตัวพอดี
        [DataRow("aB#aB#aB#aB#")]
        [DataRow("Pass word@2025")]     // มีช่องว่างได้
        public void ValidPasswords_Pass(string password)
        {
            Assert.IsNull(AccountRules.ValidatePassword(password));
        }

        [DataTestMethod, TestCategory("TC-PW-02")]
        [DataRow("Abcdef!",     "อย่างน้อย 8")]      // 7 ตัว
        [DataRow("abcdefg!",    "พิมพ์ใหญ่")]
        [DataRow("ABCDEFG!",    "พิมพ์เล็ก")]
        [DataRow("Abcdefgh1",   "อักขระพิเศษ")]
        [DataRow("",            "อย่างน้อย 8")]
        [DataRow("กขคงจฉชซ!",   "พิมพ์เล็ก")]        // อักษรไทยไม่นับเป็น a-z / A-Z
        public void WeakPasswords_ReportMissingRule(string password, string expectedRule)
        {
            string error = AccountRules.ValidatePassword(password);
            Assert.IsNotNull(error);
            StringAssert.Contains(error, expectedRule);
        }

        [TestMethod, TestCategory("TC-PW-02")]
        public void WeakPassword_ListsEveryMissingRule()
        {
            string error = AccountRules.ValidatePassword("abc");
            StringAssert.Contains(error, "อย่างน้อย 8");
            StringAssert.Contains(error, "พิมพ์ใหญ่");
            StringAssert.Contains(error, "อักขระพิเศษ");
            Assert.IsFalse(error.Contains("พิมพ์เล็ก"), "มีตัวพิมพ์เล็กแล้ว ไม่ควรฟ้อง");
        }

        [TestMethod]
        public void CheckPassword_ReturnsFourRules()
        {
            var rules = AccountRules.CheckPassword("Str0ng!Pass");
            Assert.AreEqual(4, rules.Count);
            Assert.IsTrue(rules.All(r => r.Value));
        }

        [DataTestMethod, TestCategory("TC-PW-03")]
        [DataRow("abc")]
        [DataRow("john.doe_99")]
        [DataRow("ABC")]
        public void ValidUsernames_Pass(string username)
        {
            Assert.IsNull(AccountRules.ValidateUsername(username));
        }

        [DataTestMethod, TestCategory("TC-PW-03")]
        [DataRow("")]
        [DataRow("ab")]
        [DataRow("john doe")]
        [DataRow("สมชาย")]
        [DataRow("user@mail")]
        public void InvalidUsernames_Fail(string username)
        {
            Assert.IsNotNull(AccountRules.ValidateUsername(username));
        }

        [TestMethod]
        public void HashPassword_MatchesSha256Hex()
        {
            Assert.AreEqual(TestDb.Sha256("admin1234"), AccountRules.HashPassword("admin1234"));
            Assert.AreEqual(64, AccountRules.HashPassword("x").Length);
        }
    }
}
