using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;

namespace AppHub.Launcher
{
    public partial class LoginForm : AppHubForm
    {
        private const string InvalidLoginMessage = "Username หรือ Password ไม่ถูกต้อง";

        public LoginForm()
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;
            this.CancelButton = btnCancel;
            SetPlaceholder(txtUsername, "กรอกชื่อผู้ใช้");
            SetPlaceholder(txtPassword, "กรอกรหัสผ่าน");
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

            string hash         = AccountRules.HashPassword(password);
            string errorMessage = null;

            try
            {
                SQL.Connect();

                var dt = SQL.ExecuteQuery(@"
                    SELECT User_id, Username, Full_name, Is_admin, Password_hash, Locked_until,
                           CASE WHEN Locked_until > GETDATE() THEN 1 ELSE 0 END AS Is_locked
                    FROM   dbo.t_Users
                    WHERE  Username  = @u
                      AND  Is_active = 1",
                    new Dictionary<string, object> { { "@u", username } });

                if (dt.Rows.Count == 0)
                {
                    errorMessage = InvalidLoginMessage;
                }
                else
                {
                    DataRow row    = dt.Rows[0];
                    int     userId = (int)row["User_id"];

                    if ((int)row["Is_locked"] == 1)
                    {
                        errorMessage = LockedMessage((DateTime)row["Locked_until"]);
                    }
                    else if (!string.Equals((string)row["Password_hash"], hash, StringComparison.OrdinalIgnoreCase))
                    {
                        errorMessage = RecordFailedLogin(userId);
                    }
                    else
                    {
                        SQL.ExecuteCommand(@"
                            UPDATE dbo.t_Users
                            SET    Failed_login_count = 0,
                                   Locked_until       = NULL,
                                   Last_login_date    = GETDATE()
                            WHERE  User_id = @id",
                            new Dictionary<string, object> { { "@id", userId } });

                        AppSession.UserId   = userId;
                        AppSession.Username = row["Username"].ToString();
                        AppSession.FullName = row["Full_name"].ToString();
                        AppSession.IsAdmin  = (bool)row["Is_admin"];

                        var dtPerm = SQL.ExecuteQuery(
                            "SELECT Module_code FROM dbo.t_UserModule WHERE User_id = @uid",
                            new Dictionary<string, object> { { "@uid", userId } });

                        var codes = new List<string>();
                        foreach (DataRow r in dtPerm.Rows)
                            codes.Add(r["Module_code"].ToString());
                        AppSession.SetPermissions(codes);
                    }
                }
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

            if (errorMessage != null)
            {
                ShowError(errorMessage, "เข้าสู่ระบบไม่สำเร็จ");
                txtPassword.Clear();
                txtPassword.Focus();
                return;
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

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var dlg = new RegisterForm())
            {
                if (ShowModal(dlg) != DialogResult.OK) return;
                txtUsername.Text = dlg.RegisteredUsername;
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        // ─── Lockout ──────────────────────────────────────────────────────────
        /// <summary>
        /// นับครั้งที่ login ผิด — ครบ MaxFailedLogins จะล็อก LockoutMinutes นาที
        /// ถ้าล็อกเดิมหมดเวลาแล้วเริ่มนับใหม่ ต้องเรียกขณะ SQL.Connect() active
        /// </summary>
        private static string RecordFailedLogin(int userId)
        {
            object lockedUntil = SQL.ExecuteScalar(@"
                UPDATE u
                SET    Failed_login_count = x.New_count,
                       Locked_until       = CASE WHEN x.New_count >= @max
                                                 THEN DATEADD(MINUTE, @mins, GETDATE()) END
                OUTPUT inserted.Locked_until
                FROM   dbo.t_Users u
                CROSS  APPLY (SELECT CASE WHEN u.Locked_until IS NOT NULL AND u.Locked_until <= GETDATE()
                                          THEN 1 ELSE u.Failed_login_count + 1 END AS New_count) x
                WHERE  u.User_id = @id",
                new Dictionary<string, object>
                {
                    { "@id",   userId },
                    { "@max",  AccountRules.MaxFailedLogins },
                    { "@mins", AccountRules.LockoutMinutes },
                });

            return lockedUntil is DateTime until ? LockedMessage(until) : InvalidLoginMessage;
        }

        private static string LockedMessage(DateTime until)
            => $"บัญชีถูกล็อกชั่วคราว เพราะใส่รหัสผ่านผิดครบ {AccountRules.MaxFailedLogins} ครั้ง\n" +
               $"ลองใหม่ได้หลังเวลา {until:HH:mm} น. หรือติดต่อผู้ดูแลระบบ";
    }
}
