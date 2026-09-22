using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AppHub.Core.UI;

namespace AppHub.Launcher
{
    /// <summary>
    /// รายการ module ทั้งหมดที่กำหนดสิทธิ์ได้ (ใช้ร่วมกับ UserEditDialog)
    /// </summary>
    internal static class ModuleCatalog
    {
        public static readonly KeyValuePair<string, string>[] All =
        {
            new KeyValuePair<string, string>("CRUD",   "จัดการลูกค้า (CRUD)"),
            new KeyValuePair<string, string>("IMPORT", "นำเข้า Excel (IMPORT)"),
            new KeyValuePair<string, string>("REPORT", "รายงาน (REPORT)"),
            new KeyValuePair<string, string>("SCAN",   "Scan บัตร (SCAN)"),
        };

        /// <summary>
        /// สร้าง checkbox ของทุก module เรียงลงมาจากตำแหน่ง (x, y)
        /// </summary>
        public static List<CheckBox> CreateCheckBoxes(Control parent, int x, int y,
            ICollection<string> checkedCodes)
        {
            var boxes = new List<CheckBox>();
            foreach (var m in All)
            {
                var chk = new CheckBox
                {
                    Text     = m.Value,
                    Tag      = m.Key,
                    AutoSize = true,
                    Location = new System.Drawing.Point(x, y),
                    Checked  = checkedCodes != null && checkedCodes.Contains(m.Key)
                };
                parent.Controls.Add(chk);
                boxes.Add(chk);
                y += 26;
            }
            return boxes;
        }

        public static List<string> GetChecked(IEnumerable<CheckBox> boxes)
        {
            var codes = new List<string>();
            foreach (var chk in boxes)
                if (chk.Checked) codes.Add((string)chk.Tag);
            return codes;
        }
    }

    /// <summary>
    /// กำหนดสิทธิ์ module ให้ user
    /// </summary>
    public class PermissionDialog : AppHubForm
    {
        private List<CheckBox> _chkModules;
        private Button btnOk, btnCancel;

        public List<string> SelectedModules => ModuleCatalog.GetChecked(_chkModules);

        public PermissionDialog(string username, List<string> currentModules)
        {
            BuildUI(username, currentModules);
        }

        private void BuildUI(string username, List<string> currentModules)
        {
            this.Text            = "กำหนดสิทธิ์ — " + username;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.ClientSize      = new System.Drawing.Size(300, 200);
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;

            var lblHeader = new Label
            {
                Text     = $"สิทธิ์การใช้งาน module ของ \"{username}\"",
                AutoSize = true,
                Location = new System.Drawing.Point(12, 12)
            };
            this.Controls.Add(lblHeader);

            _chkModules = ModuleCatalog.CreateCheckBoxes(this, 24, 40, currentModules);

            btnOk = new Button
            {
                Text         = "บันทึก",
                Location     = new System.Drawing.Point(110, 156),
                Size         = new System.Drawing.Size(80, 28),
                FlatStyle    = FlatStyle.Flat,
                BackColor    = System.Drawing.Color.FromArgb(220, 235, 255),
                DialogResult = DialogResult.OK
            };

            btnCancel = new Button
            {
                Text         = "ยกเลิก",
                Location     = new System.Drawing.Point(198, 156),
                Size         = new System.Drawing.Size(80, 28),
                FlatStyle    = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
    }
}
