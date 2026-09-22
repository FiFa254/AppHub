using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AppHub.Core.UI
{
    /// <summary>
    /// กรอบ dialog แบบใหม่ (ไม่ใช้ title bar ของ Windows ที่ดูเก่าใน MDI)
    /// หัวสีขาว + ชื่อ + ปุ่มปิด, ลากย้ายได้, ขอบ 1px
    /// </summary>
    public static class DialogChrome
    {
        public const int HeaderHeight = 48;

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION        = 0x2;

        [DllImport("user32.dll")] private static extern bool   ReleaseCapture();
        [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public static void Apply(Form dialog)
        {
            var client = dialog.ClientSize;
            dialog.FormBorderStyle = FormBorderStyle.None;

            // layout ของ dialog เป็นตำแหน่งคงที่ → เลื่อนทุก control ลงใต้หัว (+ขอบ 1px)
            foreach (Control c in dialog.Controls)
                c.Location = new Point(c.Left + 1, c.Top + HeaderHeight);
            dialog.ClientSize = new Size(client.Width + 2, client.Height + HeaderHeight + 1);

            var header = new Panel
            {
                Name      = "DialogHeader",
                Tag       = "chrome",
                Location  = new Point(1, 1),
                Size      = new Size(dialog.ClientSize.Width - 2, HeaderHeight - 1),
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Theme.Surface
            };

            var title = new Label
            {
                Name      = "DialogTitle",
                Text      = dialog.Text,
                Font      = new Font("Segoe UI Semibold", 10.5F),
                ForeColor = Theme.TextPrimary,
                Location  = new Point(16, 0),
                Size      = new Size(header.Width - 64, header.Height - 1),   // เว้นเส้นใต้หัว
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            var close = new Button
            {
                Name     = "DialogClose",
                Tag      = Theme.RoleIcon,
                Size     = new Size(36, 32),
                Location = new Point(header.Width - 44, (header.Height - 32) / 2),
                Anchor   = AnchorStyles.Top | AnchorStyles.Right,
                TabStop  = false
            };
            Theme.SetIcon(close, Theme.Icons.Close);
            close.Click += (s, e) =>
            {
                dialog.DialogResult = DialogResult.Cancel;
                dialog.Close();
            };

            header.Controls.Add(close);
            header.Controls.Add(title);
            header.Paint     += DrawHeaderLine;
            header.MouseDown += (s, e) => BeginDrag(dialog, e);
            title.MouseDown  += (s, e) => BeginDrag(dialog, e);
            dialog.TextChanged += (s, e) => title.Text = dialog.Text;

            dialog.Controls.Add(header);
            dialog.Paint += DrawBorder;
        }

        private static void BeginDrag(Form dialog, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !dialog.IsHandleCreated) return;
            ReleaseCapture();
            SendMessage(dialog.Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
        }

        private static void DrawHeaderLine(object sender, PaintEventArgs e)
        {
            var c = (Control)sender;
            using (var pen = new Pen(Theme.Border))
                e.Graphics.DrawLine(pen, 0, c.Height - 1, c.Width, c.Height - 1);
        }

        private static void DrawBorder(object sender, PaintEventArgs e)
        {
            var f = (Form)sender;
            using (var pen = new Pen(Color.FromArgb(203, 213, 225)))   // slate-300
                e.Graphics.DrawRectangle(pen, 0, 0, f.ClientSize.Width - 1, f.ClientSize.Height - 1);
        }
    }
}
