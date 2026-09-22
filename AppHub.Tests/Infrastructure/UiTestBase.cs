using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHub.Tests.Infrastructure
{
    public sealed class ShownMessage
    {
        public string          Text  { get; set; }
        public string          Title { get; set; }
        public MessageBoxIcon  Icon  { get; set; }
        public override string ToString() => $"[{Icon}] {Title}: {Text}";
    }

    [TestClass]
    public static class AssemblySetup
    {
        [AssemblyInitialize]
        public static void Init(TestContext context) => TestDb.Recreate();

        [AssemblyCleanup]
        public static void Cleanup() => UiServices.Reset();
    }

    /// <summary>
    /// Base ของทุก test: ล้าง DB, ดัก MessageBox / dialog / file picker, ปิด form ที่เปิด
    /// </summary>
    public abstract class UiTestBase
    {
        protected readonly List<ShownMessage> Messages = new List<ShownMessage>();

        /// <summary>คำตอบของ Confirm (Yes/No) ตามลำดับ — ถ้าหมดจะตอบ Yes</summary>
        protected readonly Queue<bool> ConfirmAnswers = new Queue<bool>();

        /// <summary>handler ของ dialog ตามลำดับที่เปิด — เติมค่าแล้ว return DialogResult</summary>
        protected readonly Queue<Func<Form, DialogResult>> DialogHandlers = new Queue<Func<Form, DialogResult>>();

        protected string OpenFilePath;
        protected string SaveFilePath;

        private readonly List<Form> _forms = new List<Form>();

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void BaseInitialize()
        {
            TestDb.ResetData();
            AppSession.Clear();

            UiServices.ShowMessage = (owner, text, title, buttons, icon) =>
            {
                Messages.Add(new ShownMessage { Text = text, Title = title, Icon = icon });
                if (buttons == MessageBoxButtons.YesNo)
                {
                    bool yes = ConfirmAnswers.Count == 0 || ConfirmAnswers.Dequeue();
                    return yes ? DialogResult.Yes : DialogResult.No;
                }
                return DialogResult.OK;
            };
            UiServices.ShowDialog = (dialog, owner) =>
            {
                if (DialogHandlers.Count == 0)
                    Assert.Fail("มี dialog เปิดโดยไม่คาดไว้: " + dialog.Text);
                return DialogHandlers.Dequeue()(dialog);
            };
            UiServices.PickOpenFile = (owner, filter, title) => OpenFilePath;
            UiServices.PickSaveFile = (owner, filter, name) => SaveFilePath;
        }

        [TestCleanup]
        public void BaseCleanup()
        {
            foreach (var form in _forms) form.Dispose();
            _forms.Clear();
            UiServices.Reset();
            AppSession.Clear();
        }

        // ─── Forms ───────────────────────────────────────────────────────────
        /// <summary>สร้าง form + handle แล้วเรียก OnLoad (ไม่แสดงหน้าจอ)</summary>
        protected T Open<T>() where T : Form, new()
        {
            var form = Track(new T());
            var handle = form.Handle;
            Ui.Call(form, "OnLoad", EventArgs.Empty);
            return form;
        }

        protected T Track<T>(T form) where T : Form
        {
            _forms.Add(form);
            return form;
        }

        protected static DataGridView Grid(Form form) => Ui.Get<DataGridView>(form, "DGV");

        protected static int GridRowCount(Form form)
            => ((DataTable)Grid(form).DataSource).Rows.Count;

        /// <summary>ตั้ง AppSession เหมือน login สำเร็จ (อ่านจาก DB)</summary>
        protected static void SignInAs(string username)
        {
            var user = TestDb.Query(
                "SELECT User_id, Username, Full_name, Is_admin FROM dbo.t_Users WHERE Username = @u",
                TestDb.P("@u", username)).Rows[0];

            AppSession.UserId   = (int)user["User_id"];
            AppSession.Username = (string)user["Username"];
            AppSession.FullName = (string)user["Full_name"];
            AppSession.IsAdmin  = (bool)user["Is_admin"];

            var codes = TestDb.Query("SELECT Module_code FROM dbo.t_UserModule WHERE User_id = @id",
                    TestDb.P("@id", AppSession.UserId))
                .AsEnumerable().Select(r => (string)r["Module_code"]).ToList();
            AppSession.SetPermissions(codes);
        }

        // ─── Assertions ──────────────────────────────────────────────────────
        protected ShownMessage AssertMessage(MessageBoxIcon icon, string contains)
        {
            var match = Messages.LastOrDefault(m => m.Icon == icon && m.Text.Contains(contains));
            Assert.IsNotNull(match,
                $"ไม่พบ {icon} ที่มีข้อความ \"{contains}\" — ที่แสดงจริง: " +
                (Messages.Count == 0 ? "(ไม่มี)" : string.Join(" | ", Messages)));
            return match;
        }

        protected void AssertNoError()
        {
            var errors = Messages.Where(m => m.Icon == MessageBoxIcon.Error).ToList();
            Assert.AreEqual(0, errors.Count, "มี error: " + string.Join(" | ", errors));
        }
    }
}
