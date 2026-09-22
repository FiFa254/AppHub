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

        /// <summary>
        /// แสดง error message มาตรฐาน
        /// </summary>
        protected void ShowError(string message, string title = "Error")
            => MessageBox.Show(this, message, title,
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        /// <summary>
        /// แสดง warning message
        /// </summary>
        protected void ShowWarning(string message, string title = "แจ้งเตือน")
            => MessageBox.Show(this, message, title,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

        /// <summary>
        /// แสดงข้อความแจ้งผลสำเร็จ
        /// </summary>
        protected void ShowInfo(string message, string title = "สำเร็จ")
            => MessageBox.Show(this, message, title,
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        /// <summary>
        /// ถามยืนยัน Yes/No
        /// </summary>
        protected bool Confirm(string message, string title = "ยืนยัน")
            => MessageBox.Show(this, message, title,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

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
