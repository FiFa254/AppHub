using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;

namespace AppHub.Launcher
{
    /// <summary>
    /// สมัครสมาชิก — สร้างบัญชีที่ยังไม่มีสิทธิ์ module ใด (Admin กำหนดสิทธิ์ภายหลัง)
    /// </summary>
    public partial class RegisterForm : AppHubForm
    {
        /// <summary>username ที่สมัครสำเร็จ (ให้หน้า Login กรอกให้)</summary>
        public string RegisteredUsername { get; private set; }

        public RegisterForm()
        {
            InitializeComponent();
            this.AcceptButton = btnRegister;
            this.CancelButton = btnCancel;
            SetPlaceholder(txtFullName, "เช่น สมชาย ใจดี");
            SetPlaceholder(txtUsername, "a-z, A-Z, 0-9, _ และ . (3-50 ตัว)");
        }

        // ─── Events ───────────────────────────────────────────────────────────
        private void txtPassword_TextChanged(object sender, EventArgs e)
            => pwdRules.ShowRules(txtPassword.Text);

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            txtConfirm.UseSystemPasswordChar  = !chkShowPassword.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // ─── Validate ─────────────────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowWarning("กรุณากรอกชื่อ-นามสกุล");
                txtFullName.Focus();
                return;
            }

            string usernameError = AccountRules.ValidateUsername(username);
            if (usernameError != null)
            {
                ShowWarning(usernameError);
                txtUsername.Focus();
                return;
            }

            string passwordError = AccountRules.ValidatePassword(password);
            if (passwordError != null)
            {
                ShowWarning(passwordError);
                txtPassword.Focus();
                return;
            }

            if (password != txtConfirm.Text)
            {
                ShowWarning("Password และ ยืนยัน Password ไม่ตรงกัน");
                txtConfirm.Clear();
                txtConfirm.Focus();
                return;
            }

            // ─── Save ─────────────────────────────────────────────────────────
            try
            {
                SQL.Connect();

                int exists = Convert.ToInt32(SQL.ExecuteScalar(
                    "SELECT COUNT(*) FROM dbo.t_Users WHERE Username = @u",
                    new Dictionary<string, object> { { "@u", username } }));
                if (exists > 0)
                {
                    ShowWarning($"Username \"{username}\" มีผู้ใช้แล้ว กรุณาเลือกชื่ออื่น");
                    txtUsername.Focus();
                    return;
                }

                SQL.ExecuteCommand(@"
                    INSERT INTO dbo.t_Users (Username, Password_hash, Full_name, Is_admin, Is_active)
                    VALUES (@u, @p, @f, 0, 1)",
                    new Dictionary<string, object>
                    {
                        { "@u", username },
                        { "@p", AccountRules.HashPassword(password) },
                        { "@f", fullName },
                    });
            }
            catch (Exception ex)
            {
                ShowError("สมัครสมาชิกไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }

            RegisteredUsername = username;
            ShowInfo("สมัครสมาชิกสำเร็จ\n\nเข้าสู่ระบบได้ทันที — ผู้ดูแลระบบจะกำหนดสิทธิ์ใช้งาน module ให้ภายหลัง");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
