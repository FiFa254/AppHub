using System;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;

namespace AppHub.Launcher
{
    /// <summary>
    /// ตั้งรหัสผ่านใหม่ — ผู้ใช้เปลี่ยนเอง (requireCurrent = true)
    /// หรือ Admin รีเซ็ตให้ (requireCurrent = false)
    /// ตรวจแค่เงื่อนไขรหัสผ่าน — การเช็ครหัสเดิมกับ DB ทำที่ผู้เรียก
    /// </summary>
    public class ChangePasswordDialog : AppHubForm
    {
        private readonly bool _requireCurrent;

        private TextBox           txtCurrent, txtNew, txtConfirm;
        private PasswordRulesView pwdRules;
        private Button            btnOk, btnCancel;

        public string CurrentPassword => txtCurrent?.Text ?? "";
        public string NewPassword     => txtNew.Text;

        public ChangePasswordDialog(string username, bool requireCurrent)
        {
            _requireCurrent = requireCurrent;
            BuildUI(username);
        }

        private void BuildUI(string username)
        {
            this.Text            = (_requireCurrent ? "เปลี่ยนรหัสผ่าน — " : "รีเซ็ตรหัสผ่าน — ") + username;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;

            int lx = 12, tx = 150, w = 210, gap = 34, y = 16;

            if (_requireCurrent)
            {
                AddLabel("รหัสผ่านปัจจุบัน *", lx, y);
                txtCurrent = AddBox(tx, y, w);
                y += gap;
            }

            AddLabel("รหัสผ่านใหม่ *", lx, y);
            txtNew = AddBox(tx, y, w);
            txtNew.TextChanged += (s, e) => pwdRules.ShowRules(txtNew.Text);
            y += 30;

            pwdRules = new PasswordRulesView { Location = new System.Drawing.Point(tx, y), Width = w };
            this.Controls.Add(pwdRules);
            y += pwdRules.Height + 8;

            AddLabel("ยืนยันรหัสผ่านใหม่ *", lx, y);
            txtConfirm = AddBox(tx, y, w);
            y += gap + 6;

            btnOk = new Button
            {
                Text      = "บันทึก",
                Location  = new System.Drawing.Point(tx, y),
                Size      = new System.Drawing.Size(80, 28),
                FlatStyle = FlatStyle.Flat,
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
            this.ClientSize   = new System.Drawing.Size(tx + w + 16, y + 44);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (_requireCurrent && string.IsNullOrEmpty(txtCurrent.Text))
            {
                ShowWarning("กรุณากรอกรหัสผ่านปัจจุบัน");
                txtCurrent.Focus();
                return;
            }

            string error = AccountRules.ValidatePassword(txtNew.Text);
            if (error != null)
            {
                ShowWarning(error);
                txtNew.Focus();
                return;
            }

            if (txtNew.Text != txtConfirm.Text)
            {
                ShowWarning("รหัสผ่านใหม่ และ ยืนยันรหัสผ่าน ไม่ตรงกัน");
                txtConfirm.Clear();
                txtConfirm.Focus();
                return;
            }

            if (_requireCurrent && txtNew.Text == txtCurrent.Text)
            {
                ShowWarning("รหัสผ่านใหม่ต้องไม่ซ้ำกับรหัสผ่านปัจจุบัน");
                txtNew.Focus();
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void AddLabel(string text, int x, int y)
            => this.Controls.Add(new Label { Text = text, Location = new System.Drawing.Point(x, y + 3), AutoSize = true });

        private TextBox AddBox(int x, int y, int w)
        {
            var box = new TextBox
            {
                Location              = new System.Drawing.Point(x, y),
                Size                  = new System.Drawing.Size(w, 24),
                UseSystemPasswordChar = true
            };
            this.Controls.Add(box);
            return box;
        }
    }
}
