using System;
using System.Windows.Forms;

namespace AppHub.Core.UI
{
    /// <summary>
    /// Base form สำหรับ form ที่มี grid หลัก
    /// มี LblTitle (บน), DGV (กลาง), PnlFooter + LblCount (ล่าง) พร้อมใช้
    /// form ลูกเพิ่มได้แค่ toolbar/filter panel (Dock = Top) — ห้ามสร้าง grid/title ซ้ำ
    /// </summary>
    public class AppHubCRUDForm : AppHubForm
    {
        protected DataGridView  DGV;
        protected Label         LblTitle;
        protected Label         LblCount;
        protected Panel         PnlFooter;

        public AppHubCRUDForm()
        {
            BuildBaseLayout();
        }

        private void BuildBaseLayout()
        {
            LblTitle = new Label
            {
                Dock      = DockStyle.Top,
                Font      = new System.Drawing.Font("Segoe UI", 12F,
                                System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(30, 60, 120),
                Height    = 40,
                Padding   = new Padding(8, 0, 0, 0),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            DGV = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly              = true,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect           = false,
                RowHeadersVisible     = false
            };

            LblCount = new Label
            {
                AutoSize  = true,
                Location  = new System.Drawing.Point(8, 10),
                Font      = new System.Drawing.Font("Segoe UI", 9F),
                ForeColor = System.Drawing.Color.Gray
            };

            // ปุ่มที่ form ลูกเพิ่มใน footer ให้ใช้ Dock = Right
            PnlFooter = new Panel
            {
                Dock    = DockStyle.Bottom,
                Height  = 36,
                Padding = new Padding(0, 4, 8, 4)
            };
            PnlFooter.Controls.Add(LblCount);

            this.Controls.Add(DGV);
            this.Controls.Add(PnlFooter);
            this.Controls.Add(LblTitle);
            this.ClientSize = new System.Drawing.Size(860, 540);
        }

        protected override void OnLoad(EventArgs e)
        {
            // จัดลำดับ dock: title บนสุด → panel ของ form ลูก → grid เต็มพื้นที่ที่เหลือ
            LblTitle.SendToBack();
            DGV.BringToFront();
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
