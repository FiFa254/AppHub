using System;
using System.Windows.Forms;
using AppHub.Core.UI;

namespace AppHub.CRUD
{
    public class CustomerEditDialog : AppHubForm
    {
        private System.Windows.Forms.Label     lblCode, lblName, lblPhone, lblEmail, lblAddr;
        private System.Windows.Forms.TextBox   txtCode, txtName, txtPhone, txtEmail, txtAddr;
        private System.Windows.Forms.Button    btnOk, btnCancel;

        public string CustomerCode { get => txtCode.Text.Trim();  set => txtCode.Text  = value; }
        public string FullName     { get => txtName.Text.Trim();  set => txtName.Text  = value; }
        public string Phone        { get => txtPhone.Text.Trim(); set => txtPhone.Text = value; }
        public string Email        { get => txtEmail.Text.Trim(); set => txtEmail.Text = value; }
        public string Address      { get => txtAddr.Text.Trim();  set => txtAddr.Text  = value; }

        public CustomerEditDialog(string title)
        {
            BuildUI(title);
        }

        public void LockCode() => txtCode.ReadOnly = true;

        private void BuildUI(string title)
        {
            this.Text            = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.ClientSize      = new System.Drawing.Size(380, 280);
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;

            int lx = 12, tx = 110, w = 240, gap = 34, y = 16;

            lblCode  = MakeLabel("รหัสลูกค้า *",  lx, y);           txtCode  = MakeBox(tx, y, w);          y += gap;
            lblName  = MakeLabel("ชื่อ-นามสกุล *", lx, y);          txtName  = MakeBox(tx, y, w);          y += gap;
            lblPhone = MakeLabel("เบอร์โทร",       lx, y);           txtPhone = MakeBox(tx, y, w);          y += gap;
            lblEmail = MakeLabel("อีเมล",          lx, y);           txtEmail = MakeBox(tx, y, w);          y += gap;
            lblAddr  = MakeLabel("ที่อยู่",         lx, y);           txtAddr  = MakeBox(tx, y, w, 60);     y += 70;

            btnOk = new Button
            {
                Text      = "ตกลง",
                Location  = new System.Drawing.Point(tx, y),
                Size      = new System.Drawing.Size(80, 28),
                FlatStyle = FlatStyle.Flat,
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

            this.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                lblCode, txtCode, lblName, txtName, lblPhone, txtPhone,
                lblEmail, txtEmail, lblAddr, txtAddr, btnOk, btnCancel
            });

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                ShowWarning("กรุณากรอกรหัสลูกค้า");
                txtCode.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowWarning("กรุณากรอกชื่อ-นามสกุล");
                txtName.Focus();
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private static Label MakeLabel(string text, int x, int y)
            => new Label { Text = text, Location = new System.Drawing.Point(x, y + 3),
                           AutoSize = true };

        private static TextBox MakeBox(int x, int y, int w, int h = 22)
            => new TextBox { Location = new System.Drawing.Point(x, y),
                             Size = new System.Drawing.Size(w, h),
                             Multiline = h > 22 };
    }
}
