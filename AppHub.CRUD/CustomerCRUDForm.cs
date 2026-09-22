using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;

namespace AppHub.CRUD
{
    public partial class CustomerCRUDForm : AppHubCRUDForm
    {
        public CustomerCRUDForm()
        {
            InitializeComponent();
            LblTitle.Text = "📦 จัดการข้อมูลลูกค้า";
            SetPlaceholder(txtSearch, "ค้นหา ชื่อ หรือ รหัส...");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadData();
        }

        // ─── Load ─────────────────────────────────────────────────────────────
        private void LoadData(string filter = "")
        {
            try
            {
                SQL.Connect();

                string sql = @"
                    SELECT Customer_id, Customer_code, Full_name, Phone, Email, Address,
                           Created_date, Updated_date
                    FROM   dbo.t_Customers";

                var p = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    sql += " WHERE Full_name LIKE @f OR Customer_code LIKE @f";
                    p["@f"] = "%" + filter.Trim() + "%";
                }
                sql += " ORDER BY Customer_id";

                var dt = SQL.ExecuteQuery(sql, p.Count > 0 ? p : null);
                DGV.DataSource = dt;
                HideColumn("Customer_id");
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

        // ─── Search ───────────────────────────────────────────────────────────
        private void btnSearch_Click(object sender, EventArgs e) => LoadData(txtSearch.Text);
        private void btnClear_Click(object sender, EventArgs e)  { txtSearch.Clear(); LoadData(); }
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            LoadData(txtSearch.Text);
        }

        // ─── Add ──────────────────────────────────────────────────────────────
        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var dlg = new CustomerEditDialog("เพิ่ม Customer ใหม่"))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    SQL.Connect();

                    int exists = Convert.ToInt32(SQL.ExecuteScalar(
                        "SELECT COUNT(*) FROM dbo.t_Customers WHERE Customer_code = @code",
                        new Dictionary<string, object> { { "@code", dlg.CustomerCode } }));
                    if (exists > 0)
                    {
                        ShowWarning($"รหัสลูกค้า \"{dlg.CustomerCode}\" มีอยู่ในระบบแล้ว");
                        return;
                    }

                    SQL.ExecuteCommand(@"
                        INSERT INTO dbo.t_Customers
                            (Customer_code, Full_name, Phone, Email, Address)
                        VALUES (@code, @name, @phone, @email, @addr)",
                        new Dictionary<string, object>
                        {
                            { "@code",  dlg.CustomerCode },
                            { "@name",  dlg.FullName },
                            { "@phone", dlg.Phone },
                            { "@email", dlg.Email },
                            { "@addr",  dlg.Address },
                        });
                }
                catch (Exception ex)
                {
                    ShowError("เพิ่มไม่สำเร็จ:\n" + ex.Message);
                    return;
                }
                finally
                {
                    SQL.Disconnect();
                }
                LoadData(txtSearch.Text);
            }
        }

        // ─── Edit ─────────────────────────────────────────────────────────────
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null)
            {
                ShowWarning("กรุณาเลือกรายการที่ต้องการแก้ไข");
                return;
            }

            var row = DGV.CurrentRow;
            int id  = Convert.ToInt32(row.Cells["Customer_id"].Value);

            using (var dlg = new CustomerEditDialog("แก้ไข Customer"))
            {
                dlg.CustomerCode = row.Cells["Customer_code"].Value?.ToString();
                dlg.FullName     = row.Cells["Full_name"].Value?.ToString();
                dlg.Phone        = row.Cells["Phone"].Value?.ToString();
                dlg.Email        = row.Cells["Email"].Value?.ToString();
                dlg.Address      = row.Cells["Address"].Value?.ToString();
                dlg.LockCode();

                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    SQL.Connect();
                    SQL.ExecuteCommand(@"
                        UPDATE dbo.t_Customers
                        SET    Full_name    = @name,
                               Phone        = @phone,
                               Email        = @email,
                               Address      = @addr,
                               Updated_date = GETDATE()
                        WHERE  Customer_id  = @id",
                        new Dictionary<string, object>
                        {
                            { "@name",  dlg.FullName },
                            { "@phone", dlg.Phone },
                            { "@email", dlg.Email },
                            { "@addr",  dlg.Address },
                            { "@id",    id },
                        });
                }
                catch (Exception ex)
                {
                    ShowError("แก้ไขไม่สำเร็จ:\n" + ex.Message);
                    return;
                }
                finally
                {
                    SQL.Disconnect();
                }
                LoadData(txtSearch.Text);
            }
        }

        // ─── Delete ───────────────────────────────────────────────────────────
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null)
            {
                ShowWarning("กรุณาเลือกรายการที่ต้องการลบ");
                return;
            }

            int    id   = Convert.ToInt32(DGV.CurrentRow.Cells["Customer_id"].Value);
            string name = DGV.CurrentRow.Cells["Full_name"].Value?.ToString();

            if (!Confirm($"ลบ \"{name}\" ออกจากระบบ?", "ยืนยันลบ")) return;

            try
            {
                SQL.Connect();
                SQL.ExecuteCommand(
                    "DELETE FROM dbo.t_Customers WHERE Customer_id = @id",
                    new Dictionary<string, object> { { "@id", id } });
            }
            catch (Exception ex)
            {
                ShowError("ลบไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }
            LoadData(txtSearch.Text);
        }

        // ─── Double-click = Edit ──────────────────────────────────────────────
        private void DGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) btnEdit_Click(sender, EventArgs.Empty);
        }
    }
}
