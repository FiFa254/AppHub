using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AppHub.Report;
using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests
{
    /// <summary>
    /// TC-R — Report + Export CSV
    /// </summary>
    [TestClass]
    public class ReportTests : UiTestBase
    {
        [TestInitialize]
        public void SignIn() => SignInAs("user1");

        [TestCleanup]
        public void DeleteExport()
        {
            if (SaveFilePath != null && File.Exists(SaveFilePath)) File.Delete(SaveFilePath);
        }

        private static void SetDateFilter(ReportForm form, DateTime from, DateTime to)
        {
            Ui.Get<CheckBox>(form, "chkDateFilter").Checked = true;
            Ui.Get<DateTimePicker>(form, "dtpFrom").Value   = from;
            Ui.Get<DateTimePicker>(form, "dtpTo").Value     = to;
        }

        [TestMethod, TestCategory("TC-R-01")]
        public void Open_ShowsAllCustomers()
        {
            var form = Open<ReportForm>();

            Assert.AreEqual(5, GridRowCount(form));
            AssertNoError();
            Assert.AreEqual(0, Messages.Count, "เปิดครั้งแรกไม่ควรมี popup");
        }

        [TestMethod, TestCategory("TC-R-02")]
        public void FilterByName()
        {
            var form = Open<ReportForm>();
            Ui.Get<TextBox>(form, "txtSearch").Text = "สมชาย";

            Ui.Click(form, "btnSearch_Click");

            Assert.AreEqual(1, GridRowCount(form));
            Assert.AreEqual("C001", Grid(form).Rows[0].Cells["Customer_code"].Value);
        }

        [TestMethod, TestCategory("TC-R-03")]
        public void FilterByDateFrom_ExcludesOlder()
        {
            TestDb.Execute("UPDATE dbo.t_Customers SET Created_date = '2020-01-15' WHERE Customer_code = 'C001'");
            var form = Open<ReportForm>();
            SetDateFilter(form, DateTime.Today.AddDays(-1), DateTime.Today);

            Ui.Click(form, "btnSearch_Click");

            Assert.AreEqual(4, GridRowCount(form));
        }

        [TestMethod, TestCategory("TC-R-04")]
        public void FilterByDateTo_ExcludesNewer()
        {
            TestDb.Execute("UPDATE dbo.t_Customers SET Created_date = '2020-01-15' WHERE Customer_code = 'C001'");
            var form = Open<ReportForm>();
            SetDateFilter(form, new DateTime(2020, 1, 1), new DateTime(2020, 1, 15));

            Ui.Click(form, "btnSearch_Click");

            Assert.AreEqual(1, GridRowCount(form), "วันที่สิ้นสุดต้องรวมวันนั้นด้วย");
            Assert.AreEqual("C001", Grid(form).Rows[0].Cells["Customer_code"].Value);
        }

        [TestMethod]
        public void DateRange_FromAfterTo_ShowsWarning()
        {
            var form = Open<ReportForm>();
            SetDateFilter(form, DateTime.Today, DateTime.Today.AddDays(-5));

            Ui.Click(form, "btnSearch_Click");

            AssertMessage(MessageBoxIcon.Warning, "วันที่เริ่มต้นต้องไม่เกินวันที่สิ้นสุด");
        }

        [TestMethod, TestCategory("TC-R-05")]
        public void NoMatch_ShowsNotFound()
        {
            var form = Open<ReportForm>();
            Ui.Get<TextBox>(form, "txtSearch").Text = "ไม่มีชื่อนี้แน่นอน";

            Ui.Click(form, "btnSearch_Click");

            Assert.AreEqual(0, GridRowCount(form));
            AssertMessage(MessageBoxIcon.Warning, "ไม่พบข้อมูล");
        }

        [TestMethod, TestCategory("TC-R-06")]
        public void ExportCsv_WritesUtf8WithBomAndAllRows()
        {
            var form = Open<ReportForm>();
            SaveFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csv");

            Ui.Click(form, "btnExport_Click");

            AssertMessage(MessageBoxIcon.Information, "Export สำเร็จ");
            byte[] bytes = File.ReadAllBytes(SaveFilePath);
            CollectionAssert.AreEqual(new byte[] { 0xEF, 0xBB, 0xBF }, bytes.Take(3).ToArray(), "ต้องมี UTF-8 BOM ให้ Excel อ่านไทยได้");

            string[] lines = File.ReadAllLines(SaveFilePath, Encoding.UTF8);
            Assert.AreEqual(6, lines.Length, "header + 5 แถว");
            StringAssert.StartsWith(lines[0], "\"Customer_code\",\"Full_name\"");
            Assert.IsTrue(lines.Any(l => l.Contains("\"สมชาย ใจดี\"")));
        }

        [TestMethod]
        public void ExportCsv_Cancelled_WritesNothing()
        {
            var form = Open<ReportForm>();
            SaveFilePath = null;

            Ui.Click(form, "btnExport_Click");

            Assert.AreEqual(0, Messages.Count);
        }

        [TestMethod, TestCategory("TC-R-07")]
        public void ClearFilter_ShowsAllAgain()
        {
            var form = Open<ReportForm>();
            Ui.Get<TextBox>(form, "txtSearch").Text = "สมชาย";
            Ui.Click(form, "btnSearch_Click");
            Assert.AreEqual(1, GridRowCount(form));

            Ui.Click(form, "btnClear_Click");

            Assert.AreEqual(5, GridRowCount(form));
            Assert.AreEqual("", Ui.Get<TextBox>(form, "txtSearch").Text);
            Assert.IsFalse(Ui.Get<CheckBox>(form, "chkDateFilter").Checked);
        }
    }
}
