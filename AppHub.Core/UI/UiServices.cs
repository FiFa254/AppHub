using System;
using System.Windows.Forms;

namespace AppHub.Core.UI
{
    /// <summary>
    /// จุดเดียวที่เปิด UI แบบ modal (MessageBox / dialog / เลือกไฟล์)
    /// ค่าปกติเรียก WinForms จริง — AppHub.Tests แทนที่เพื่อให้ test ไม่ค้างรอคนกด
    /// form ต้องเรียกผ่าน helper ของ AppHubForm เท่านั้น ห้ามเรียกตรง
    /// </summary>
    public static class UiServices
    {
        public static Func<IWin32Window, string, string, MessageBoxButtons, MessageBoxIcon, DialogResult> ShowMessage;
        public static Func<Form, IWin32Window, DialogResult> ShowDialog;

        /// <summary>(owner, filter, title) → path ที่เลือก หรือ null ถ้ายกเลิก</summary>
        public static Func<IWin32Window, string, string, string> PickOpenFile;

        /// <summary>(owner, filter, default file name) → path ที่เลือก หรือ null ถ้ายกเลิก</summary>
        public static Func<IWin32Window, string, string, string> PickSaveFile;

        static UiServices()
        {
            Reset();
        }

        /// <summary>
        /// กลับไปใช้ WinForms จริง
        /// </summary>
        public static void Reset()
        {
            ShowMessage = (owner, message, title, buttons, icon) =>
                MessageBox.Show(owner, message, title, buttons, icon);

            ShowDialog = (dialog, owner) => dialog.ShowDialog(owner);

            PickOpenFile = (owner, filter, title) =>
            {
                using (var dlg = new OpenFileDialog { Filter = filter, Title = title })
                    return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.FileName : null;
            };

            PickSaveFile = (owner, filter, fileName) =>
            {
                using (var dlg = new SaveFileDialog { Filter = filter, FileName = fileName })
                    return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.FileName : null;
            };
        }
    }
}
