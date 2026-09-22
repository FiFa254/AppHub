using System;
using System.Windows.Forms;

namespace AppHub.Core.UI
{
    /// <summary>
    /// Base form สำหรับ form ที่มี grid หลัก
    /// มี LblTitle (บน), DGV ในการ์ดสีขาว (กลาง), PnlFooter + LblCount (ล่าง) พร้อมใช้
    /// form ลูกเพิ่มได้แค่ toolbar/filter panel (Dock = Top) — ห้ามสร้าง grid/title ซ้ำ
    /// </summary>
    public class AppHubCRUDForm : AppHubForm
    {
        protected DataGridView  DGV;
        protected Label         LblTitle;
        protected Label         LblCount;
        protected Panel         PnlFooter;

        private Panel _gridHost;

        public AppHubCRUDForm()
        {
            BuildBaseLayout();
        }

        private void BuildBaseLayout()
        {
            LblTitle = new Label
            {
                Name      = "LblTitle",
                Dock      = DockStyle.Top,
                Height    = 52,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            DGV = new DataGridView
            {
                Name                  = "DGV",
                Dock                  = DockStyle.Fill,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly              = true,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect           = false,
                RowHeadersVisible     = false
            };

            // การ์ดสีขาวรอบ grid — เว้นขอบให้เห็นพื้นหลัง
            _gridHost = new Panel
            {
                Name      = "GridHost",
                Tag       = "card",
                Dock      = DockStyle.Fill,
                Padding   = new Padding(16, 12, 16, 0),
                BackColor = Theme.Background
            };
            _gridHost.Controls.Add(DGV);

            LblCount = new Label
            {
                AutoSize  = true,
                Location  = new System.Drawing.Point(18, 11),
                Font      = Theme.Body,
                ForeColor = Theme.TextMuted
            };

            // ปุ่มที่ form ลูกเพิ่มใน footer ให้ใช้ Dock = Right
            PnlFooter = new Panel
            {
                Name      = "PnlFooter",
                Tag       = "footer",
                Dock      = DockStyle.Bottom,
                Height    = 44,
                Padding   = new Padding(0, 6, 16, 6),
                BackColor = Theme.Background
            };
            PnlFooter.Controls.Add(LblCount);

            this.Controls.Add(_gridHost);
            this.Controls.Add(PnlFooter);
            this.Controls.Add(LblTitle);
            this.ClientSize = new System.Drawing.Size(900, 560);
        }

        protected override void OnLoad(EventArgs e)
        {
            // จัดลำดับ dock: title บนสุด → panel ของ form ลูก → การ์ด grid เต็มพื้นที่ที่เหลือ
            LblTitle.SendToBack();
            _gridHost.BringToFront();
            base.OnLoad(e);
        }

        /// <summary>
        /// อัปเดต label แสดงจำนวน rows
        /// </summary>
        protected void UpdateCount(int count)
            => LblCount.Text = $"ทั้งหมด {count} รายการ";

        /// <summary>
        /// ซ่อน column ตาม name
        /// </summary>
        protected void HideColumn(string columnName)
        {
            if (DGV.Columns[columnName] != null)
                DGV.Columns[columnName].Visible = false;
        }
    }
}
