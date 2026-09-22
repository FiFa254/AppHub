using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AppHub.Core.UI;

namespace AppHub.Launcher
{
    /// <summary>
    /// Dialog เพิ่มผู้ใช้ใหม่ พร้อมเลือกสิทธิ์ module
    /// </summary>
    public class UserEditDialog : AppHubForm
    {
        private Label          lblUsername, lblPassword, lblConfirm, lblFullName, lblModules;
        private TextBox        txtUsername, txtPassword, txtConfirm, txtFullName;
        private CheckBox       chkAdmin;
        private List<CheckBox> _chkModules;
        private Button         btnOk, btnCancel;

        public string       Username        => txtUsername.Text.Trim();
        public string       Password        => txtPassword.Text;
        public string       FullName        => txtFullName.Text.Trim();
        public bool         IsAdmin         => chkAdmin.Checked;
        public List<string> SelectedModules => ModuleCatalog.GetChecked(_chkModules);

        public UserEditDialog()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text            = "เพิ่มผู้ใช้ใหม่";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.ClientSize      = new System.Drawing.Size(380, 360);
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;

            int lx = 12, tx = 130, w = 220, gap = 34, y = 16;

            lblUsername = MakeLabel("Username *",        lx, y); txtUsername = MakeBox(tx, y, w); y += gap;
            lblPassword = MakeLabel("Password *",        lx, y); txtPassword = MakeBox(tx, y, w); y += gap;
            lblConfirm  = MakeLabel("ยืนยัน Password *", lx, y); txtConfirm  = MakeBox(tx, y, w); y += gap;
            lblFullName = MakeLabel("ชื่อ-นามสกุล",       lx, y); txtFullName = MakeBox(tx, y, w); y += gap;

            txtPassword.UseSystemPasswordChar = true;
            txtConfirm.UseSystemPasswordChar  = true;

            chkAdmin = new CheckBox
            {
                Text     = "เป็น Admin (เข้าถึงทุก module + จัดการผู้ใช้)",
                AutoSize = true,
                Location = new System.Drawing.Point(tx, y)
            };
            y += gap;

            lblModules = MakeLabel("สิทธิ์ module", lx, y);

            this.Controls.AddRange(new Control[]
            {
                lblUsername, txtUsername, lblPassword, txtPassword, lblConfirm, txtConfirm,
                lblFullName, txtFullName, chkAdmin, lblModules
            });

            _chkModules = ModuleCatalog.CreateCheckBoxes(this, tx, y, null);
            y += ModuleCatalog.All.Length * 26 + 12;

            btnOk = new Button
            {
                Text         = "บันทึก",
                Location     = new System.Drawing.Point(tx, y),
                Size         = new System.Drawing.Size(80, 28),
                FlatStyle    = FlatStyle.Flat,
                BackColor    = System.Drawing.Color.FromArgb(220, 235, 255),
                DialogResult = DialogResult.None
            };
            btnOk.Click += BtnOk_Click;

            btnCancel = new Button
            {
                Text         = "ยกเลิก",
                Location     = new System.Drawing.Point(tx + 88, y),
                Size         = new System.Drawing.Size(80, 28),
                FlatStyle    = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                ShowWarning("กรุณากรอก Username และ Password");
                (string.IsNullOrWhiteSpace(txtUsername.Text) ? txtUsername : txtPassword).Focus();
                return;
            }
            if (txtPassword.Text != txtConfirm.Text)
            {
                ShowWarning("Password และ ยืนยัน Password ไม่ตรงกัน");
                txtConfirm.Clear();
                txtConfirm.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
                txtFullName.Text = txtUsername.Text.Trim();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private static Label MakeLabel(string text, int x, int y)
            => new Label { Text = text, Location = new System.Drawing.Point(x, y + 3), AutoSize = true };

        private static TextBox MakeBox(int x, int y, int w)
            => new TextBox { Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(w, 24) };
    }
}
