using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;

namespace AppHub.Launcher
{
    /// <summary>
    /// หน้าจัดการบัญชีผู้ใช้และสิทธิ์ — เฉพาะ Admin
    /// </summary>
    public partial class UserManagementForm : AppHubCRUDForm
    {
        public UserManagementForm()
        {
            InitializeComponent();
            LblTitle.Text = "จัดการผู้ใช้งาน";
            Theme.SetIcon(btnAdd,             Theme.Icons.Add);
            Theme.SetIcon(btnEditPermissions, Theme.Icons.Permission);
            Theme.SetIcon(btnToggleActive,    Theme.Icons.Power);
            Theme.SetIcon(btnResetPassword,   Theme.Icons.Key);
            Theme.SetIcon(btnRefresh,         Theme.Icons.Refresh);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadUsers();
        }

        // ─── Load ─────────────────────────────────────────────────────────────
        private void LoadUsers()
        {
            try
            {
                SQL.Connect();

                var dt = SQL.ExecuteQuery(@"
                    SELECT User_id, Username, Full_name, Is_admin, Is_active,
                           Last_login_date, Locked_until, Created_date
                    FROM   dbo.t_Users
                    ORDER  BY User_id", null);

                DGV.DataSource = dt;
                HideColumn("User_id");
                UpdateCount(dt.Rows.Count);
            }
            catch (Exception ex)
            {
                ShowError("โหลดข้อมูลไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadUsers();

        // ─── Add User ─────────────────────────────────────────────────────────
        private void btnAdd_Click(object sender, EventArgs e)
        {
            List<KeyValuePair<string, string>> modules;
            try
            {
                SQL.Connect();
                modules = ModuleCatalog.Load();
            }
            catch (Exception ex)
            {
                ShowError("โหลดรายการ module ไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }

            var dlg = new UserEditDialog(modules);
            ShowDialogChild(dlg, () => AddUser(dlg));
        }

        private void AddUser(UserEditDialog dlg)
        {
            try
            {
                SQL.Connect();

                int exists = Convert.ToInt32(SQL.ExecuteScalar(
                    "SELECT COUNT(*) FROM dbo.t_Users WHERE Username = @u",
                    new Dictionary<string, object> { { "@u", dlg.Username } }));
                if (exists > 0)
                {
                    ShowWarning($"Username \"{dlg.Username}\" มีอยู่ในระบบแล้ว");
                    return;
                }

                int newId = Convert.ToInt32(SQL.ExecuteScalar(@"
                    INSERT INTO dbo.t_Users (Username, Password_hash, Full_name, Is_admin, Is_active)
                    VALUES (@u, @p, @f, @a, 1);
                    SELECT SCOPE_IDENTITY();",
                    new Dictionary<string, object>
                    {
                        { "@u", dlg.Username },
                        { "@p", AccountRules.HashPassword(dlg.Password) },
                        { "@f", dlg.FullName },
                        { "@a", dlg.IsAdmin },
                    }));

                SavePermissions(newId, dlg.SelectedModules);
            }
            catch (Exception ex)
            {
                ShowError("เพิ่มผู้ใช้ไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }
            LoadUsers();
        }

        // ─── Edit Permissions ─────────────────────────────────────────────────
        private void btnEditPermissions_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null)
            {
                ShowWarning("กรุณาเลือกผู้ใช้");
                return;
            }

            int    userId   = Convert.ToInt32(DGV.CurrentRow.Cells["User_id"].Value);
            string username = DGV.CurrentRow.Cells["Username"].Value?.ToString();

            var current = new List<string>();
            List<KeyValuePair<string, string>> modules;
            try
            {
                SQL.Connect();
                modules = ModuleCatalog.Load();
                var dt = SQL.ExecuteQuery(
                    "SELECT Module_code FROM dbo.t_UserModule WHERE User_id = @uid",
                    new Dictionary<string, object> { { "@uid", userId } });
                foreach (DataRow r in dt.Rows)
                    current.Add(r["Module_code"].ToString());
            }
            catch (Exception ex)
            {
                ShowError("โหลดสิทธิ์ไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }

            var dlg = new PermissionDialog(username, modules, current);
            ShowDialogChild(dlg, () => UpdatePermissions(userId, dlg));
        }

        private void UpdatePermissions(int userId, PermissionDialog dlg)
        {
            try
            {
                SQL.Connect();
                SavePermissions(userId, dlg.SelectedModules);
            }
            catch (Exception ex)
            {
                ShowError("บันทึกสิทธิ์ไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }

            ShowInfo("บันทึกสิทธิ์สำเร็จ\n(มีผลเมื่อผู้ใช้ login ครั้งถัดไป)");
            LoadUsers();
        }

        private void DGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) btnEditPermissions_Click(sender, EventArgs.Empty);
        }

        // ─── Toggle Active ────────────────────────────────────────────────────
        private void btnToggleActive_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null)
            {
                ShowWarning("กรุณาเลือกผู้ใช้");
                return;
            }

            int    userId   = Convert.ToInt32(DGV.CurrentRow.Cells["User_id"].Value);
            bool   isActive = Convert.ToBoolean(DGV.CurrentRow.Cells["Is_active"].Value);
            string username = DGV.CurrentRow.Cells["Username"].Value?.ToString();
            string action   = isActive ? "ปิดใช้งาน" : "เปิดใช้งาน";

            if (isActive && userId == AppSession.UserId)
            {
                ShowWarning("ไม่สามารถปิดใช้งานบัญชีที่กำลัง login อยู่ได้");
                return;
            }

            if (!Confirm($"{action} user \"{username}\"?")) return;

            try
            {
                SQL.Connect();
                SQL.ExecuteCommand(
                    "UPDATE dbo.t_Users SET Is_active = @v WHERE User_id = @id",
                    new Dictionary<string, object> { { "@v", !isActive }, { "@id", userId } });
            }
            catch (Exception ex)
            {
                ShowError($"{action}ไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }

            LoadUsers();
        }

        // ─── Reset Password (ปลดล็อกด้วย) ─────────────────────────────────────
        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null)
            {
                ShowWarning("กรุณาเลือกผู้ใช้");
                return;
            }

            int    userId   = Convert.ToInt32(DGV.CurrentRow.Cells["User_id"].Value);
            string username = DGV.CurrentRow.Cells["Username"].Value?.ToString();

            var dlg = new ChangePasswordDialog(username, requireCurrent: false);
            ShowDialogChild(dlg, () => ResetPassword(userId, username, dlg.NewPassword));
        }

        private void ResetPassword(int userId, string username, string newPassword)
        {
            try
            {
                SQL.Connect();
                SQL.ExecuteCommand(@"
                    UPDATE dbo.t_Users
                    SET    Password_hash      = @p,
                           Failed_login_count = 0,
                           Locked_until       = NULL
                    WHERE  User_id = @id",
                    new Dictionary<string, object>
                    {
                        { "@p",  AccountRules.HashPassword(newPassword) },
                        { "@id", userId },
                    });
            }
            catch (Exception ex)
            {
                ShowError("รีเซ็ตรหัสผ่านไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }

            ShowInfo($"รีเซ็ตรหัสผ่านของ \"{username}\" สำเร็จ (ปลดล็อกบัญชีแล้ว)");
            LoadUsers();
        }

        // ─── Helpers ──────────────────────────────────────────────────────────
        /// <summary>ต้องเรียกขณะ SQL.Connect() ยัง active อยู่</summary>
        private static void SavePermissions(int userId, List<string> modules)
        {
            SQL.ExecuteCommand("DELETE FROM dbo.t_UserModule WHERE User_id = @uid",
                new Dictionary<string, object> { { "@uid", userId } });

            foreach (string code in modules)
            {
                SQL.ExecuteCommand(
                    "INSERT INTO dbo.t_UserModule (User_id, Module_code) VALUES (@uid, @code)",
                    new Dictionary<string, object> { { "@uid", userId }, { "@code", code } });
            }
        }

    }
}
