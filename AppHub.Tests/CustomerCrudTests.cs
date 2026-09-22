using System;
using System.Windows.Forms;
using AppHub.CRUD;
using AppHub.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests
{
    /// <summary>
    /// TC-C — Customer Management
    /// </summary>
    [TestClass]
    public class CustomerCrudTests : UiTestBase
    {
        [TestInitialize]
        public void SignIn() => SignInAs("user1");

        private static DialogResult FillCustomerDialog(Form dialog, string code, string name,
            string phone = "", string email = "", string address = "")
        {
            var txtCode = Ui.Get<TextBox>(dialog, "txtCode");
            if (!txtCode.ReadOnly) txtCode.Text = code;
            Ui.Get<TextBox>(dialog, "txtName").Text  = name;
            Ui.Get<TextBox>(dialog, "txtPhone").Text = phone;
            Ui.Get<TextBox>(dialog, "txtEmail").Text = email;
            Ui.Get<TextBox>(dialog, "txtAddr").Text  = address;
            Ui.Click(dialog, "BtnOk_Click");
            return dialog.DialogResult;
        }

        private static int CustomerCount() => TestDb.Count("SELECT COUNT(*) FROM dbo.t_Customers");

        [TestMethod, TestCategory("TC-C-01")]
        public void Open_ShowsAllCustomers()
        {
            var form = Open<CustomerCRUDForm>();

            Assert.AreEqual(5, GridRowCount(form));
            Assert.IsFalse(Grid(form).Columns["Customer_id"].Visible);
            StringAssert.Contains(Ui.Get<Label>(form, "LblCount").Text, "5");
            AssertNoError();
        }

        [TestMethod, TestCategory("TC-C-02")]
        public void AddCustomer_AppearsInGrid()
        {
            var form = Open<CustomerCRUDForm>();
            DialogHandlers.Enqueue(d => FillCustomerDialog(d, "C099", "ทดสอบ", "080-000"));

            Ui.Click(form, "btnAdd_Click");

            AssertNoError();
            Assert.AreEqual(6, GridRowCount(form));
            Assert.AreEqual("ทดสอบ", TestDb.Scalar("SELECT Full_name FROM dbo.t_Customers WHERE Customer_code = 'C099'"));
        }

        [TestMethod, TestCategory("TC-C-03")]
        public void AddCustomer_DuplicateCode_ShowsWarning()
        {
            var form = Open<CustomerCRUDForm>();
            DialogHandlers.Enqueue(d => FillCustomerDialog(d, "C001", "ซ้ำ"));

            Ui.Click(form, "btnAdd_Click");

            AssertMessage(MessageBoxIcon.Warning, "มีอยู่ในระบบแล้ว");
            Assert.AreEqual(5, CustomerCount());
        }

        [TestMethod, TestCategory("TC-C-04")]
        public void AddCustomer_EmptyCode_ValidatesBeforeSave()
        {
            var form = Open<CustomerCRUDForm>();
            DialogHandlers.Enqueue(d => FillCustomerDialog(d, "", "ไม่มีรหัส"));

            Ui.Click(form, "btnAdd_Click");

            AssertMessage(MessageBoxIcon.Warning, "กรุณากรอกรหัสลูกค้า");
            Assert.AreEqual(5, CustomerCount());
        }

        [TestMethod, TestCategory("TC-C-04")]
        public void AddCustomer_EmptyName_ValidatesBeforeSave()
        {
            var form = Open<CustomerCRUDForm>();
            DialogHandlers.Enqueue(d => FillCustomerDialog(d, "C098", ""));

            Ui.Click(form, "btnAdd_Click");

            AssertMessage(MessageBoxIcon.Warning, "กรุณากรอกชื่อ-นามสกุล");
            Assert.AreEqual(5, CustomerCount());
        }

        [TestMethod, TestCategory("TC-C-05")]
        public void EditCustomer_UpdatesPhoneAndUpdatedDate()
        {
            var form = Open<CustomerCRUDForm>();
            Ui.SelectRow(Grid(form), "Customer_code", "C002");
            bool codeLocked = false;
            DialogHandlers.Enqueue(d =>
            {
                codeLocked = Ui.Get<TextBox>(d, "txtCode").ReadOnly;
                Assert.AreEqual("สมหญิง รักงาน", Ui.Get<TextBox>(d, "txtName").Text);
                return FillCustomerDialog(d, null, "สมหญิง รักงาน", "099-999-9999");
            });

            Ui.Click(form, "btnEdit_Click");

            AssertNoError();
            Assert.IsTrue(codeLocked, "รหัสลูกค้าต้องแก้ไม่ได้ตอน edit");
            var row = TestDb.Query("SELECT Phone, Updated_date FROM dbo.t_Customers WHERE Customer_code = 'C002'").Rows[0];
            Assert.AreEqual("099-999-9999", row["Phone"]);
            Assert.AreNotEqual(DBNull.Value, row["Updated_date"]);
        }

        [TestMethod, TestCategory("TC-C-06")]
        public void DeleteCustomer_ConfirmYes_Removes()
        {
            var form = Open<CustomerCRUDForm>();
            Ui.SelectRow(Grid(form), "Customer_code", "C005");
            ConfirmAnswers.Enqueue(true);

            Ui.Click(form, "btnDelete_Click");

            Assert.AreEqual(4, GridRowCount(form));
            Assert.AreEqual(0, TestDb.Count("SELECT COUNT(*) FROM dbo.t_Customers WHERE Customer_code = 'C005'"));
        }

        [TestMethod, TestCategory("TC-C-07")]
        public void DeleteCustomer_ConfirmNo_NoChange()
        {
            var form = Open<CustomerCRUDForm>();
            Ui.SelectRow(Grid(form), "Customer_code", "C005");
            ConfirmAnswers.Enqueue(false);

            Ui.Click(form, "btnDelete_Click");

            Assert.AreEqual(5, CustomerCount());
        }

        [TestMethod, TestCategory("TC-C-08")]
        public void Search_FiltersByPartialName()
        {
            var form = Open<CustomerCRUDForm>();
            Ui.Get<TextBox>(form, "txtSearch").Text = "สมห";

            Ui.Click(form, "btnSearch_Click");

            Assert.AreEqual(1, GridRowCount(form));
            Assert.AreEqual("C002", Grid(form).Rows[0].Cells["Customer_code"].Value);

            Ui.Click(form, "btnClear_Click");
            Assert.AreEqual(5, GridRowCount(form));
        }

        [TestMethod]
        public void Search_ByCode()
        {
            var form = Open<CustomerCRUDForm>();
            Ui.Get<TextBox>(form, "txtSearch").Text = "C00";

            Ui.Click(form, "btnSearch_Click");

            Assert.AreEqual(5, GridRowCount(form));
        }
    }
}
