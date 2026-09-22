using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using AppHub.Core.UI;

namespace AppHub.Launcher
{
    /// <summary>
    /// แผงแบรนด์ด้านซ้ายของหน้า Login / สมัครสมาชิก (วาดเองทั้งหมด)
    /// </summary>
    public class BrandPanel : Panel
    {
        private static readonly Color GradientEnd = Color.FromArgb(30, 64, 175);   // blue-800
        private static readonly Font  LogoFont     = new Font("Segoe UI", 18F);
        private static readonly Font  HeadlineFont = new Font("Segoe UI Semibold", 26F);
        private static readonly Font  TaglineFont  = new Font("Segoe UI", 11F);

        public string Headline { get; set; } = "AppHub";
        public string Tagline  { get; set; } = "ระบบจัดการข้อมูลลูกค้า\nครบ จบ ในที่เดียว";

        public BrandPanel()
        {
            this.Dock           = DockStyle.Left;
            this.Width          = 320;
            this.Tag            = "brand";
            this.DoubleBuffered = true;
            this.ResizeRedraw   = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using (var bg = new LinearGradientBrush(ClientRectangle, Theme.Primary, GradientEnd, 60F))
                g.FillRectangle(bg, ClientRectangle);

            // วงกลมตกแต่ง
            using (var soft = new SolidBrush(Color.FromArgb(28, Color.White)))
            {
                g.FillEllipse(soft, Width - 170, -90, 260, 260);
                g.FillEllipse(soft, -80, Height - 150, 220, 220);
            }

            // โลโก้
            var logo = new Rectangle(40, 56, 48, 48);
            using (var white = new SolidBrush(Color.White))
                g.FillEllipse(white, logo);
            TextRenderer.DrawText(g, "◆", LogoFont, logo, Theme.Primary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            TextRenderer.DrawText(g, Headline, HeadlineFont,
                new Point(34, 124), Color.White);
            TextRenderer.DrawText(g, Tagline, TaglineFont,
                new Rectangle(40, 178, Width - 70, 80), Color.FromArgb(219, 234, 254),
                TextFormatFlags.WordBreak);

            TextRenderer.DrawText(g, "© 2026 AppHub", Theme.Small,
                new Point(40, Height - 40), Color.FromArgb(191, 219, 254));
        }
    }
}
