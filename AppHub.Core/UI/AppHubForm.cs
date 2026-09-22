using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AppHub.Core.UI
{
    /// <summary>
    /// Base form สำหรับทุก form ใน AppHub
    /// </summary>
    public class AppHubForm : Form
    {
        public AppHubForm()
        {
            this.Font            = new System.Drawing.Font("Segoe UI", 9F);
            this.AutoScaleMode   = AutoScaleMode.Font;
            this.StartPosition   = FormStartPosition.CenterParent;
        }

        protected override void OnLoad(EventArgs e)
        {
            Theme.ApplyForm(this);
            base.OnLoad(e);
        }

        /// <summary>
        /// แสดง error message มาตรฐาน
        /// </summary>
        protected void ShowError(string message, string title = "Error")
            => UiServices.ShowMessage(this, message, title,
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        /// <summary>
        /// แสดง warning message
        /// </summary>
        protected void ShowWarning(string message, string title = "แจ้งเตือน")
            => UiServices.ShowMessage(this, message, title,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

        /// <summary>
        /// แสดงข้อความแจ้งผลสำเร็จ
        /// </summary>
        protected void ShowInfo(string message, string title = "สำเร็จ")
            => UiServices.ShowMessage(this, message, title,
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        /// <summary>
        /// ถามยืนยัน Yes/No
        /// </summary>
        protected bool Confirm(string message, string title = "ยืนยัน")
            => UiServices.ShowMessage(this, message, title,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        /// <summary>
        /// เปิด dialog แบบ modal — ใช้เฉพาะ form ที่ไม่อยู่ใน MainForm (เช่นหน้า Login)
        /// form ใน MainForm ให้ใช้ ShowDialogChild
        /// </summary>
        protected DialogResult ShowModal(Form dialog)
            => UiServices.ShowDialog(dialog, this);

        /// <summary>
        /// dialog ที่เปิดค้างจาก form นี้ (form ถูกล็อกระหว่างนั้น) — MainForm ใช้ยก dialog ขึ้นมาเมื่อกลับมาที่หน้านี้
        /// </summary>
        public Form OpenDialog { get; private set; }

        /// <summary>
        /// ยก form นี้ขึ้นมาใน MainForm — ถ้ามี dialog ค้างอยู่ให้ dialog อยู่ข้างหน้า
        /// </summary>
        public void BringUp()
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;

            if (OpenDialog != null && !OpenDialog.IsDisposed)
            {
                BringToFront();              // form ที่ถูกล็อก activate ไม่ได้ → ยกด้วย z-order
                OpenDialog.Activate();
            }
            else
            {
                Activate();
            }
        }

        /// <summary>
        /// เปิด dialog เป็นหน้าต่างลูกใน MainForm (MDI) — onOk ทำงานหลังกด OK แล้ว dialog ปิด
        /// ระหว่างเปิด form นี้ถูก disable กันกดซ้ำ; ถ้าไม่มี MDI container จะเปิดแบบ modal แทน
        /// </summary>
        protected void ShowDialogChild(Form dialog, Action onOk)
        {
            Form container = IsMdiContainer ? this : MdiParent;
            bool lockOwner = !IsMdiContainer;
            if (lockOwner)
            {
                Enabled    = false;
                OpenDialog = dialog;
            }

            UiServices.ShowChild(dialog, container, result =>
            {
                if (OpenDialog == dialog) OpenDialog = null;
                if (lockOwner && !IsDisposed)
                {
                    Enabled = true;
                    Activate();
                }
                if (result == DialogResult.OK) onOk();
            });
        }

        /// <summary>
        /// เลือกไฟล์ที่จะเปิด → path หรือ null ถ้ายกเลิก
        /// </summary>
        protected string PickOpenFile(string filter, string title)
            => UiServices.PickOpenFile(this, filter, title);

        /// <summary>
        /// เลือกที่บันทึกไฟล์ → path หรือ null ถ้ายกเลิก
        /// </summary>
        protected string PickSaveFile(string filter, string fileName)
            => UiServices.PickSaveFile(this, filter, fileName);

        // ─── Placeholder (.NET Framework ไม่มี TextBox.PlaceholderText) ────────
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        /// <summary>
        /// แสดงข้อความจางๆ ใน TextBox ตอนยังว่าง
        /// </summary>
        protected static void SetPlaceholder(TextBox box, string text)
        {
            if (box.IsHandleCreated)
                SendMessage(box.Handle, EM_SETCUEBANNER, (IntPtr)1, text);
            else
                box.HandleCreated += (s, e) =>
                    SendMessage(box.Handle, EM_SETCUEBANNER, (IntPtr)1, text);
        }
    }
}
