using System.Windows.Forms;
using AppHub.Scan;
using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests
{
    /// <summary>
    /// TC-S — Scan / Input
    /// </summary>
    [TestClass]
    public class ScanTests : UiTestBase
    {
        [TestInitialize]
        public void SignIn() => SignInAs("admin");

        private ScanForm Scan(string code, string note = "")
        {
            var form = Open<ScanForm>();
            ScanAgain(form, code, note);
            return form;
        }

        private static void ScanAgain(ScanForm form, string code, string note = "")
        {
            Ui.Get<TextBox>(form, "txtCode").Text = code;
            Ui.Get<TextBox>(form, "txtNote").Text = note;
            Ui.Click(form, "btnSave_Click");
        }

        private static int LogCount() => TestDb.Count("SELECT COUNT(*) FROM dbo.t_ScanLog");

        [TestMethod, TestCategory("TC-S-01")]
        public void Save_WritesLogAndShowsSuccess()
        {
            var form = Scan("C001", "รับของแล้ว");

            AssertMessage(MessageBoxIcon.Information, "บันทึกสำเร็จ");
            var row = TestDb.Query("SELECT Customer_code, Scanned_by, Note FROM dbo.t_ScanLog").Rows[0];
            Assert.AreEqual("C001", row["Customer_code"]);
            Assert.AreEqual("admin", row["Scanned_by"]);
            Assert.AreEqual("รับของแล้ว", row["Note"]);
            Assert.AreEqual("", Ui.Get<TextBox>(form, "txtCode").Text, "บันทึกแล้วต้องล้างช่อง");
            Assert.AreEqual(1, ((System.Data.DataTable)Ui.Get<DataGridView>(form, "dgvLog").DataSource).Rows.Count);
        }

        [TestMethod, TestCategory("TC-S-01")]
        public void Save_LowercaseCode_IsNormalized()
        {
            Scan("c002");

            AssertMessage(MessageBoxIcon.Information, "บันทึกสำเร็จ");
            Assert.AreEqual("C002", TestDb.Scalar("SELECT Customer_code FROM dbo.t_ScanLog"));
        }

        [TestMethod, TestCategory("TC-S-02")]
        public void EmptyCode_ShowsWarning()
        {
            Scan("   ");

            AssertMessage(MessageBoxIcon.Warning, "กรุณากรอก Customer Code");
            Assert.AreEqual(0, LogCount());
        }

        [TestMethod]
        public void UnknownCode_NotSaved()
        {
            var form = Scan("ZZZ999");

            Assert.AreEqual(0, LogCount());
            StringAssert.Contains(Ui.Get<Label>(form, "lblStatus").Text, "ไม่พบรหัส ZZZ999");
        }

        [TestMethod, TestCategory("TC-S-03")]
        public void SameCodeTwice_BothSaved()
        {
            var form = Scan("C001");
            ScanAgain(form, "C001");

            Assert.AreEqual(2, LogCount());
        }

        [TestMethod, TestCategory("TC-S-04")]
        public void Clear_EmptiesFields()
        {
            var form = Open<ScanForm>();
            Ui.Get<TextBox>(form, "txtCode").Text = "C001";
            Ui.Get<TextBox>(form, "txtNote").Text = "note";

            Ui.Click(form, "btnClear_Click");

            Assert.AreEqual("", Ui.Get<TextBox>(form, "txtCode").Text);
            Assert.AreEqual("", Ui.Get<TextBox>(form, "txtNote").Text);
            Assert.AreEqual(0, LogCount());
        }

        [TestMethod, TestCategory("TC-S-05")]
        public void EnterKey_TriggersSave()
        {
            var form = Open<ScanForm>();

            Assert.AreSame(Ui.Get<Button>(form, "btnSave"), form.AcceptButton);
        }
    }
}
