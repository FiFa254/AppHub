using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using AppHub.Import;
using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;

namespace AppHub.Tests
{
    /// <summary>
    /// TC-I — Import Excel (ใช้ไฟล์จริงใน TestData\)
    /// </summary>
    [TestClass]
    public class ImportTests : UiTestBase
    {
        private string _tempFile;

        [TestInitialize]
        public void SignIn() => SignInAs("admin");

        [TestCleanup]
        public void DeleteTemp()
        {
            if (_tempFile != null && File.Exists(_tempFile)) File.Delete(_tempFile);
        }

        private ImportForm Browse(string path)
        {
            var form = Open<ImportForm>();
            OpenFilePath = path;
            Ui.Click(form, "btnBrowse_Click");
            return form;
        }

        private static DataTable Preview(ImportForm form) => Ui.Get<DataTable>(form, "_preview");
        private static bool ImportEnabled(ImportForm form) => Ui.Get<Button>(form, "btnImport").Enabled;
        private static int CustomerCount() => TestDb.Count("SELECT COUNT(*) FROM dbo.t_Customers");

        [TestMethod, TestCategory("TC-I-01")]
        public void ValidFile_ShowsPreview()
        {
            var form = Browse(TestDb.TestDataPath("test_customers.xlsx"));

            AssertNoError();
            Assert.AreEqual(10, Preview(form).Rows.Count);
            Assert.AreEqual("T001", Preview(form).Rows[0]["Customer_code"]);
            Assert.IsTrue(ImportEnabled(form));
            Assert.AreEqual(5, CustomerCount(), "Preview ต้องยังไม่บันทึกลง DB");
        }

        [TestMethod, TestCategory("TC-I-02")]
        public void NonExcelFile_ShowsWarning()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
            File.WriteAllText(_tempFile, "not excel");

            var form = Browse(_tempFile);

            AssertMessage(MessageBoxIcon.Warning, "กรุณาเลือกไฟล์ .xlsx");
            Assert.IsNull(Preview(form));
            Assert.IsFalse(ImportEnabled(form));
        }

        [TestMethod, TestCategory("TC-I-03")]
        public void WrongHeader_ShowsErrorWithColumn()
        {
            var form = Browse(TestDb.TestDataPath("test_wrong_header.xlsx"));

            var msg = AssertMessage(MessageBoxIcon.Error, "Header");
            StringAssert.Contains(msg.Text, "Customer_code");
            Assert.IsFalse(ImportEnabled(form));
        }

        [TestMethod, TestCategory("TC-I-04")]
        public void Import_InsertsAllRows()
        {
            var form = Browse(TestDb.TestDataPath("test_customers.xlsx"));

            Ui.Click(form, "btnImport_Click");

            AssertMessage(MessageBoxIcon.Information, "นำเข้าสำเร็จ 10 รายการ");
            Assert.AreEqual(15, CustomerCount());
            Assert.IsFalse(ImportEnabled(form));
        }

        [TestMethod, TestCategory("TC-I-05")]
        public void Import_SkipsDuplicates()
        {
            var form = Browse(TestDb.TestDataPath("test_customers_dup.xlsx"));

            Ui.Click(form, "btnImport_Click");

            var msg = AssertMessage(MessageBoxIcon.Information, "นำเข้าสำเร็จ 2 รายการ");
            StringAssert.Contains(msg.Text, "ข้าม (ซ้ำ) 3 รายการ");
            Assert.AreEqual(7, CustomerCount());
            Assert.AreEqual("สมชาย ใจดี",
                TestDb.Scalar("SELECT Full_name FROM dbo.t_Customers WHERE Customer_code = 'C001'"),
                "ข้อมูลเดิมต้องไม่ถูกเขียนทับ");
        }

        [TestMethod, TestCategory("TC-I-06")]
        public void EmptyFile_ShowsNoData()
        {
            var form = Browse(TestDb.TestDataPath("test_empty.xlsx"));

            AssertMessage(MessageBoxIcon.Warning, "ไม่พบข้อมูลในไฟล์");
            Assert.IsFalse(ImportEnabled(form));
        }

        [TestMethod, TestCategory("TC-I-07")]
        public void LargeFile_ImportsAllRows()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var pkg = new ExcelPackage())
            {
                var ws = pkg.Workbook.Worksheets.Add("Customers");
                string[] headers = { "Customer_code", "Full_name", "Phone", "Email", "Address" };
                for (int c = 0; c < headers.Length; c++) ws.Cells[1, c + 1].Value = headers[c];
                for (int r = 0; r < 150; r++)
                {
                    ws.Cells[r + 2, 1].Value = $"L{r:000}";
                    ws.Cells[r + 2, 2].Value = $"ลูกค้า {r}";
                    ws.Cells[r + 2, 3].Value = "080-000-0000";
                    ws.Cells[r + 2, 4].Value = $"l{r}@test.com";
                    ws.Cells[r + 2, 5].Value = "กรุงเทพ";
                }
                pkg.SaveAs(new FileInfo(_tempFile));
            }

            var form = Browse(_tempFile);
            Ui.Click(form, "btnImport_Click");

            AssertMessage(MessageBoxIcon.Information, "นำเข้าสำเร็จ 150 รายการ");
            Assert.AreEqual(155, CustomerCount());
        }

        [TestMethod]
        public void BadFileAfterGoodFile_ClearsOldPreview()
        {
            var form = Browse(TestDb.TestDataPath("test_customers.xlsx"));
            Assert.IsTrue(ImportEnabled(form));

            OpenFilePath = TestDb.TestDataPath("test_wrong_header.xlsx");
            Ui.Click(form, "btnBrowse_Click");

            Assert.IsNull(Preview(form));
            Assert.IsFalse(ImportEnabled(form), "ต้องกด Import ข้อมูลเก่าไม่ได้");
        }
    }
}
