using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AppHub.Core.UI
{
    /// <summary>
    /// Design system ของทั้งแอป — สี, ฟอนต์, สไตล์ control
    /// AppHubForm เรียก Theme.Apply ตอน OnLoad ให้อัตโนมัติ
    /// ห้ามตั้งสีปุ่ม / grid ใน Designer — กำหนดบทบาทปุ่มด้วย Tag แทน
    /// </summary>
    public static class Theme
    {
        // ─── Palette (Slate + Blue) ──────────────────────────────────────────
        public static readonly Color Background   = Hex(0xF1F5F9);   // พื้นหลังหน้าจอ
        public static readonly Color Surface      = Color.White;      // การ์ด / toolbar / dialog
        public static readonly Color SurfaceAlt   = Hex(0xF8FAFC);   // หัวตาราง / แถวสลับ
        public static readonly Color Border       = Hex(0xE2E8F0);
        public static readonly Color BorderStrong = Hex(0xCBD5E1);
        public static readonly Color TextPrimary  = Hex(0x0F172A);
        public static readonly Color TextMuted    = Hex(0x64748B);
        public static readonly Color Primary      = Hex(0x2563EB);
        public static readonly Color PrimaryHover = Hex(0x1D4ED8);
        public static readonly Color PrimaryDark  = Hex(0x1E3A8A);
        public static readonly Color PrimarySoft  = Hex(0xDBEAFE);
        public static readonly Color Danger       = Hex(0xDC2626);
        public static readonly Color DangerHover  = Hex(0xB91C1C);
        public static readonly Color DangerSoft   = Hex(0xFEE2E2);
        public static readonly Color Success      = Hex(0x16A34A);
        public static readonly Color Sidebar      = Hex(0x0F172A);
        public static readonly Color SidebarHover = Hex(0x1E293B);
        public static readonly Color SidebarActive = Hex(0x1E3A8A);
        public static readonly Color SidebarText  = Hex(0xCBD5E1);
        public static readonly Color SidebarMuted = Hex(0x94A3B8);

        // ─── Typography ──────────────────────────────────────────────────────
        public static readonly Font Body     = new Font("Segoe UI", 9F);
        public static readonly Font BodyBold = new Font("Segoe UI Semibold", 9F);
        public static readonly Font Small    = new Font("Segoe UI", 8.25F);
        public static readonly Font Input    = new Font("Segoe UI", 10.5F);
        public static readonly Font Nav      = new Font("Segoe UI", 10F);
        public static readonly Font Title    = new Font("Segoe UI Semibold", 14F);
        public static readonly Font Heading  = new Font("Segoe UI Semibold", 20F);

        // ─── Button roles (ตั้งผ่าน Button.Tag) ──────────────────────────────
        public const string RolePrimary   = "primary";    // ปุ่มหลักของหน้า (AcceptButton เป็น primary อัตโนมัติ)
        public const string RoleSecondary = "secondary";  // ค่าเริ่มต้น
        public const string RoleDanger    = "danger";     // ลบ / ทำลายข้อมูล
        public const string RoleGhost     = "ghost";      // ปุ่มรองแบบไม่มีกรอบ
        public const string RoleNav       = "nav";        // ปุ่มเมนูใน sidebar
        public const string RoleIcon      = "icon";       // ปุ่มไอคอนอย่างเดียว (ปิด ฯลฯ)

        // ─── Icons (Segoe MDL2 Assets — มีใน Windows 10/11, ห้ามใช้ emoji เพราะ GDI วาดเป็นกล่อง) ──
        public static class Icons
        {
            public const char Customers  = '\uE779';
            public const char Import     = '\uE8B5';
            public const char Report     = '\uE9D2';
            public const char Scan       = '\uED14';
            public const char Users      = '\uE716';
            public const char Permission = '\uE7EF';
            public const char Key        = '\uE192';
            public const char Password   = '\uE8D7';
            public const char Power      = '\uE7E8';
            public const char Search     = '\uE721';
            public const char Add        = '\uE710';
            public const char Edit       = '\uE70F';
            public const char Delete     = '\uE74D';
            public const char Refresh    = '\uE72C';
            public const char Save       = '\uE74E';
            public const char Close      = '\uE8BB';
            public const char Clear      = '\uE711';
            public const char OpenFile   = '\uE8E5';
            public const char Export     = '\uE898';
        }

        private const string IconFontName = "Segoe MDL2 Assets";
        private static readonly Dictionary<string, Bitmap> IconCache = new Dictionary<string, Bitmap>();
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Button, object> ButtonIcons =
            new System.Runtime.CompilerServices.ConditionalWeakTable<Button, object>();

        /// <summary>
        /// ไอคอนเป็นรูป (เว้นขวา 6px ให้ห่างจากข้อความ) — cache ตาม glyph/สี/ขนาด
        /// </summary>
        public static Bitmap Icon(char glyph, Color color, int size = 16, int gap = 6)
        {
            string key = $"{(int)glyph}|{color.ToArgb()}|{size}|{gap}";
            if (IconCache.TryGetValue(key, out var cached)) return cached;

            var bmp = new Bitmap(size + gap, size);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new Font(IconFontName, size * 0.72F, GraphicsUnit.Pixel))
            using (var brush = new SolidBrush(color))
            using (var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                g.DrawString(glyph.ToString(), font, brush, new RectangleF(0, 0, size, size), fmt);
            }
            IconCache[key] = bmp;
            return bmp;
        }

        /// <summary>ผูกไอคอนกับปุ่ม — สีไอคอนตามสีตัวอักษรของบทบาทปุ่ม (ตอน Apply)</summary>
        public static void SetIcon(Button b, char glyph)
        {
            ButtonIcons.Remove(b);
            ButtonIcons.Add(b, glyph);
            RefreshIcon(b);
        }

        /// <summary>วาดไอคอนใหม่ตาม ForeColor ปัจจุบัน (เช่นหลังเปลี่ยนสีเมนูที่เลือก)</summary>
        public static void RefreshIcon(Button b)
        {
            if (!ButtonIcons.TryGetValue(b, out object glyph)) return;
            bool nav = b.Tag as string == RoleNav;
            int gap = nav ? 14 : (string.IsNullOrEmpty(b.Text) ? 0 : 6);
            b.Image             = Icon((char)glyph, b.ForeColor, nav ? 18 : 16, gap);
            b.TextImageRelation = TextImageRelation.ImageBeforeText;
            b.ImageAlign        = nav ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
            if (!nav) b.TextAlign = ContentAlignment.MiddleCenter;
        }

        /// <summary>หัวคอลัมน์ภาษาไทยของชื่อคอลัมน์ใน DB</summary>
        public static readonly Dictionary<string, string> ColumnCaptions =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Customer_code",   "รหัสลูกค้า" },
                { "Full_name",       "ชื่อ-นามสกุล" },
                { "Phone",           "เบอร์โทร" },
                { "Email",           "อีเมล" },
                { "Address",         "ที่อยู่" },
                { "Created_date",    "วันที่สร้าง" },
                { "Updated_date",    "แก้ไขล่าสุด" },
                { "Username",        "ชื่อผู้ใช้" },
                { "Is_admin",        "Admin" },
                { "Is_active",       "ใช้งาน" },
                { "Last_login_date", "เข้าใช้ล่าสุด" },
                { "Locked_until",    "ล็อกถึง" },
                { "Scanned_by",      "ผู้สแกน" },
                { "Scan_date",       "เวลาสแกน" },
                { "Note",            "หมายเหตุ" },
            };

        public const string DateTimeFormat = "dd/MM/yyyy HH:mm";

        // ─── Apply ───────────────────────────────────────────────────────────
        /// <summary>
        /// ใส่สไตล์ให้ทั้ง form: dialog = พื้นขาว, หน้าทั่วไป = พื้นเทาอ่อน
        /// </summary>
        public static void ApplyForm(Form form)
        {
            bool isDialog  = form.FormBorderStyle == FormBorderStyle.FixedDialog
                          || form.Controls.ContainsKey("DialogHeader");      // dialog ที่ใส่ DialogChrome แล้ว
            form.BackColor = isDialog ? Surface : Background;
            form.ForeColor = TextPrimary;
            Apply(form);
        }

        /// <summary>ใส่สไตล์ให้ control ลูกทุกตัว (recursive)</summary>
        public static void Apply(Control root)
        {
            foreach (Control child in root.Controls)
            {
                ApplyOne(child);
                if (!(child is DataGridView))
                    Apply(child);
            }
        }

        private static void ApplyOne(Control c)
        {
            switch (c)
            {
                case Button b:
                    StyleButton(b, ResolveRole(b));
                    break;
                case DataGridView g:
                    StyleGrid(g);
                    break;
                case TextBox t:
                    t.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case LinkLabel link:
                    StyleLink(link);
                    break;
                case Label l when l.Name == "lblTitle" || l.Name == "LblTitle":
                    StyleTitle(l);
                    break;
                case ToolStrip strip:                       // MenuStrip / StatusStrip ด้วย
                    StyleStrip(strip);
                    break;
                case Panel p when p.Dock == DockStyle.Top && p.Tag == null:
                    StyleToolbar(p);
                    break;
            }
        }

        private static string ResolveRole(Button b)
        {
            if (b.Tag is string role) return role;
            return b.FindForm()?.AcceptButton == b ? RolePrimary : RoleSecondary;
        }

        // ─── Controls ────────────────────────────────────────────────────────
        public static void StyleButton(Button b, string role)
        {
            b.FlatStyle               = FlatStyle.Flat;
            b.UseVisualStyleBackColor = false;
            b.Cursor                  = Cursors.Hand;
            b.FlatAppearance.BorderSize = 0;

            switch (role)
            {
                case RolePrimary:
                    Colorize(b, Primary, Color.White, PrimaryHover, PrimaryDark);
                    b.Font = BodyBold;
                    break;
                case RoleDanger:
                    Colorize(b, Surface, Danger, DangerSoft, DangerSoft);
                    b.Font = BodyBold;
                    b.FlatAppearance.BorderSize  = 1;
                    b.FlatAppearance.BorderColor = Danger;
                    break;
                case RoleGhost:
                    Colorize(b, b.Parent?.BackColor ?? Surface, Primary, PrimarySoft, PrimarySoft);
                    break;
                case RoleIcon:
                    Colorize(b, b.Parent?.BackColor ?? Surface, TextMuted, SurfaceAlt, Border);
                    break;
                case RoleNav:
                    Colorize(b, Sidebar, SidebarText, SidebarHover, SidebarHover);
                    b.Font      = Nav;
                    b.TextAlign = ContentAlignment.MiddleLeft;
                    b.Padding   = new Padding(18, 0, 0, 0);
                    break;
                default:
                    Colorize(b, Surface, TextPrimary, SurfaceAlt, Border);
                    b.FlatAppearance.BorderSize  = 1;
                    b.FlatAppearance.BorderColor = BorderStrong;
                    break;
            }
            RefreshIcon(b);
        }

        private static void Colorize(Button b, Color back, Color fore, Color hover, Color down)
        {
            b.BackColor = back;
            b.ForeColor = fore;
            b.FlatAppearance.MouseOverBackColor = hover;
            b.FlatAppearance.MouseDownBackColor = down;
        }

        public static void StyleGrid(DataGridView g)
        {
            g.BorderStyle                 = BorderStyle.None;
            g.BackgroundColor             = Surface;
            g.GridColor                   = Border;
            g.CellBorderStyle             = DataGridViewCellBorderStyle.SingleHorizontal;
            g.ColumnHeadersBorderStyle    = DataGridViewHeaderBorderStyle.None;
            g.EnableHeadersVisualStyles   = false;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight         = 42;
            g.RowTemplate.Height          = 40;
            g.AllowUserToResizeRows       = false;
            g.RowHeadersVisible           = false;

            var header = g.ColumnHeadersDefaultCellStyle;
            header.BackColor          = SurfaceAlt;
            header.ForeColor          = TextMuted;
            header.SelectionBackColor = SurfaceAlt;
            header.SelectionForeColor = TextMuted;
            header.Font               = BodyBold;
            header.Padding            = new Padding(10, 0, 10, 0);
            header.Alignment          = DataGridViewContentAlignment.MiddleLeft;

            var cell = g.DefaultCellStyle;
            cell.BackColor          = Surface;
            cell.ForeColor          = TextPrimary;
            cell.SelectionBackColor = PrimarySoft;
            cell.SelectionForeColor = PrimaryDark;
            cell.Font               = Body;
            cell.Padding            = new Padding(10, 0, 10, 0);

            g.AlternatingRowsDefaultCellStyle.BackColor = SurfaceAlt;

            g.DataBindingComplete -= OnGridBound;
            g.DataBindingComplete += OnGridBound;
            ApplyCaptions(g);
        }

        private static void OnGridBound(object sender, DataGridViewBindingCompleteEventArgs e)
            => ApplyCaptions((DataGridView)sender);

        /// <summary>หัวคอลัมน์ภาษาไทย + รูปแบบวันที่</summary>
        public static void ApplyCaptions(DataGridView g)
        {
            foreach (DataGridViewColumn col in g.Columns)
            {
                string key = string.IsNullOrEmpty(col.DataPropertyName) ? col.Name : col.DataPropertyName;
                if (ColumnCaptions.TryGetValue(key, out string caption))
                    col.HeaderText = caption;
                if (col.ValueType == typeof(DateTime))
                    col.DefaultCellStyle.Format = DateTimeFormat;
            }
        }

        public static void StyleTitle(Label l)
        {
            l.Font      = Title;
            l.ForeColor = TextPrimary;
            l.BackColor = Surface;
            l.Padding   = new Padding(16, 0, 0, 0);
            l.Height    = 52;
            l.TextAlign = ContentAlignment.MiddleLeft;
        }

        public static void StyleToolbar(Panel p)
        {
            p.BackColor = Surface;
            p.Paint -= DrawBottomBorder;
            p.Paint += DrawBottomBorder;
        }

        public static void StyleLink(LinkLabel link)
        {
            link.LinkColor       = Primary;
            link.ActiveLinkColor = PrimaryHover;
            link.VisitedLinkColor = Primary;
            link.LinkBehavior    = LinkBehavior.HoverUnderline;
        }

        public static void StyleStrip(ToolStrip strip)
        {
            strip.RenderMode = ToolStripRenderMode.Professional;
            strip.Renderer   = new ToolStripProfessionalRenderer(new FlatColorTable()) { RoundedEdges = false };
            strip.BackColor  = Surface;
            strip.ForeColor  = TextPrimary;
        }

        private static void DrawBottomBorder(object sender, PaintEventArgs e)
        {
            var c = (Control)sender;
            using (var pen = new Pen(Border))
                e.Graphics.DrawLine(pen, 0, c.Height - 1, c.Width, c.Height - 1);
        }

        private static Color Hex(int rgb) => Color.FromArgb((rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);

        /// <summary>สีเมนูแบบแบน ไม่มี gradient</summary>
        private sealed class FlatColorTable : ProfessionalColorTable
        {
            public override Color MenuStripGradientBegin          => Surface;
            public override Color MenuStripGradientEnd            => Surface;
            public override Color ToolStripGradientBegin          => Surface;
            public override Color ToolStripGradientMiddle         => Surface;
            public override Color ToolStripGradientEnd            => Surface;
            public override Color ToolStripBorder                 => Border;
            public override Color ToolStripDropDownBackground     => Surface;
            public override Color ImageMarginGradientBegin        => Surface;
            public override Color ImageMarginGradientMiddle       => Surface;
            public override Color ImageMarginGradientEnd          => Surface;
            public override Color MenuBorder                      => Border;
            public override Color MenuItemBorder                  => PrimarySoft;
            public override Color MenuItemSelected                => PrimarySoft;
            public override Color MenuItemSelectedGradientBegin   => PrimarySoft;
            public override Color MenuItemSelectedGradientEnd     => PrimarySoft;
            public override Color MenuItemPressedGradientBegin    => SurfaceAlt;
            public override Color MenuItemPressedGradientMiddle   => SurfaceAlt;
            public override Color MenuItemPressedGradientEnd      => SurfaceAlt;
            public override Color SeparatorDark                   => Border;
            public override Color SeparatorLight                  => Surface;
            public override Color StatusStripGradientBegin        => Surface;
            public override Color StatusStripGradientEnd          => Surface;
        }
    }
}
