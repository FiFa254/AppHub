using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AppHub.Core.UI
{
    /// <summary>
    /// จุดเดียวที่เปิด UI (MessageBox / dialog / เลือกไฟล์)
    /// ค่าปกติเรียก WinForms จริง — AppHub.Tests แทนที่เพื่อให้ test ไม่ค้างรอคนกด
    /// form ต้องเรียกผ่าน helper ของ AppHubForm เท่านั้น ห้ามเรียกตรง
    /// </summary>
    public static class UiServices
    {
        public static Func<IWin32Window, string, string, MessageBoxButtons, MessageBoxIcon, DialogResult> ShowMessage;

        /// <summary>dialog แบบ modal — ใช้เฉพาะตอนที่ไม่มี MDI container (เช่นหน้า Login)</summary>
        public static Func<Form, IWin32Window, DialogResult> ShowDialog;

        /// <summary>
        /// (dialog, MDI container, onClosed) — เปิด dialog เป็นหน้าต่างลูกใน container
        /// onClosed ได้ DialogResult ตอน dialog ปิด; container = null → เปิดแบบ modal แทน
        /// </summary>
        public static Action<Form, Form, Action<DialogResult>> ShowChild;

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

            ShowChild = ShowMdiChild;

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

        // ─── MDI dialog ──────────────────────────────────────────────────────
        private static void ShowMdiChild(Form dialog, Form container, Action<DialogResult> onClosed)
        {
            if (container == null)
            {
                using (dialog)
                    onClosed(dialog.ShowDialog());
                return;
            }

            // ปุ่มที่ตั้ง DialogResult ไว้ (เช่น ยกเลิก / Esc) ปิด form เองได้เฉพาะแบบ modal → สั่งปิดให้
            foreach (var button in AllControls(dialog).OfType<Button>()
                                                      .Where(b => b.DialogResult != DialogResult.None))
                button.Click += (s, e) => dialog.Close();

            // FormClosed ไม่เกิดถ้า dialog ถูก dispose ก่อนมี handle → ดัก Disposed ด้วย (เรียก onClosed ครั้งเดียว)
            bool closed = false;
            void Finish()
            {
                if (closed) return;
                closed = true;
                onClosed(dialog.DialogResult);
            }
            dialog.FormClosed   += (s, e) => Finish();
            dialog.Disposed     += (s, e) => Finish();
            DialogChrome.Apply(dialog);
            dialog.MdiParent     = container;
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location      = CenterIn(container, dialog.Size);
            dialog.Show();

            // ถ้ามีหน้าต่างลูกขยายเต็มอยู่ MDI จะขยาย dialog ตามด้วย
            if (dialog.WindowState != FormWindowState.Normal)
                dialog.WindowState = FormWindowState.Normal;
            dialog.Activate();
        }

        private static Point CenterIn(Form container, Size size)
        {
            var client = container.Controls.OfType<MdiClient>().FirstOrDefault()?.ClientSize
                         ?? container.ClientSize;
            return new Point(Math.Max(0, (client.Width  - size.Width)  / 2),
                             Math.Max(0, (client.Height - size.Height) / 2));
        }

        private static IEnumerable<Control> AllControls(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                yield return child;
                foreach (var nested in AllControls(child))
                    yield return nested;
            }
        }
    }
}
