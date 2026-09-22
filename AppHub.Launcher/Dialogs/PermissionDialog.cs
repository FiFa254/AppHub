using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;

namespace AppHub.Launcher
{
    /// <summary>
    /// รายการ module จาก dbo.t_Modules (ใช้ร่วมกับ UserEditDialog / PermissionDialog)
    /// </summary>
    internal static class ModuleCatalog
    {
        public const int RowHeight = 26;

        /// <summary>
        /// โหลด module ทั้งหมด (Key = Module_code, Value = ข้อความที่แสดง)
        /// ต้องเรียกขณะ SQL.Connect() ยัง active อยู่
        /// </summary>
        public static List<KeyValuePair<string, string>> Load()
        {
            var dt = SQL.ExecuteQuery(@"
                SELECT Module_code, Module_name
                FROM   dbo.t_Modules
                ORDER  BY Sort_order, Module_code", null);

            var modules = new List<KeyValuePair<string, string>>();
            foreach (DataRow r in dt.Rows)
            {
                string code = r["Module_code"].ToString();
                modules.Add(new KeyValuePair<string, string>(code, $"{r["Module_name"]} ({code})"));
            }
            return modules;
        }

        /// <summary>
        /// สร้าง checkbox ของทุก module เรียงลงมาจากตำแหน่ง (x, y)
        /// </summary>
        public static List<CheckBox> CreateCheckBoxes(Control parent, int x, int y,
            IEnumerable<KeyValuePair<string, string>> modules, ICollection<string> checkedCodes)
        {
            var boxes = new List<CheckBox>();
            foreach (var m in modules)
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
                y += RowHeight;
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

        public PermissionDialog(string username,
            List<KeyValuePair<string, string>> modules, List<string> currentModules)
        {
            BuildUI(username, modules, currentModules);
        }

        private void BuildUI(string username,
            List<KeyValuePair<string, string>> modules, List<string> currentModules)
        {
            int buttonY = 40 + modules.Count * ModuleCatalog.RowHeight + 12;

            this.Text            = "กำหนดสิทธิ์ — " + username;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.ClientSize      = new System.Drawing.Size(300, buttonY + 44);
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;

            var lblHeader = new Label
            {
                Text     = $"สิทธิ์การใช้งาน module ของ \"{username}\"",
                AutoSize = true,
                Location = new System.Drawing.Point(12, 12)
            };
            this.Controls.Add(lblHeader);

            _chkModules = ModuleCatalog.CreateCheckBoxes(this, 24, 40, modules, currentModules);

            btnOk = new Button
            {
                Text         = "บันทึก",
                Location     = new System.Drawing.Point(110, buttonY),
                Size         = new System.Drawing.Size(80, 28),
                FlatStyle    = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };

            btnCancel = new Button
            {
                Text         = "ยกเลิก",
                Location     = new System.Drawing.Point(198, buttonY),
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
