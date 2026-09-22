using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;

namespace AppHub.Launcher
{
    public partial class LoginForm : AppHubForm
    {
        public LoginForm()
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;
        }

        // ─── Events ───────────────────────────────────────────────────────────
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowWarning("กรุณากรอก Username และ Password");
                return;
            }

            string hash = Sha256(password);

            try
            {
                SQL.Connect();

                // ตรวจสอบ user
                string sql = @"
                    SELECT User_id, Username, Full_name, Is_admin
                    FROM   dbo.t_Users
                    WHERE  Username      = @u
                      AND  Password_hash = @p
                      AND  Is_active     = 1";

                var dt = SQL.ExecuteQuery(sql, new Dictionary<string, object>
                {
                    { "@u", username },
                    { "@p", hash }
                });

                if (dt.Rows.Count == 0)
                {
                    ShowError("Username หรือ Password ไม่ถูกต้อง", "เข้าสู่ระบบไม่สำเร็จ");
                    txtPassword.Clear();
                    txtPassword.Focus();
                    return;
                }

                DataRow row = dt.Rows[0];
                AppSession.UserId   = (int)row["User_id"];
                AppSession.Username = row["Username"].ToString();
                AppSession.FullName = row["Full_name"].ToString();
                AppSession.IsAdmin  = (bool)row["Is_admin"];

                // โหลด permissions
                var dtPerm = SQL.ExecuteQuery(
                    "SELECT Module_code FROM dbo.t_UserModule WHERE User_id = @uid",
                    new Dictionary<string, object> { { "@uid", AppSession.UserId } });

                var codes = new List<string>();
                foreach (DataRow r in dtPerm.Rows)
                    codes.Add(r["Module_code"].ToString());

                AppSession.SetPermissions(codes);
            }
            catch (Exception ex)
            {
                ShowError("เชื่อมต่อฐานข้อมูลไม่ได้:\n" + ex.Message, "ข้อผิดพลาด");
                return;
            }
            finally
            {
                SQL.Disconnect();
            }

            // Program.Main จะเปิด MainForm ต่อ
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // ─── Helper ───────────────────────────────────────────────────────────
        private static string Sha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
