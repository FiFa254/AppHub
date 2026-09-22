using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;

namespace AppHub.Scan
{
    public partial class ScanForm : AppHubForm
    {
        public ScanForm()
        {
            InitializeComponent();
            Theme.SetIcon(btnSave,    Theme.Icons.Save);
            Theme.SetIcon(btnRefresh, Theme.Icons.Refresh);
            SetPlaceholder(txtCode, "Scan หรือพิมพ์รหัส...");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadHistory();
            this.ActiveControl = txtCode;
        }

        // ─── Scan / Save (Enter = AcceptButton) ───────────────────────────────
        private void btnSave_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(code))
            {
                ShowWarning("กรุณากรอก Customer Code");
                txtCode.Focus();
                return;
            }

            string name = null;
            try
            {
                SQL.Connect();

                var dtCust = SQL.ExecuteQuery(
                    "SELECT Full_name FROM dbo.t_Customers WHERE Customer_code = @c",
                    new Dictionary<string, object> { { "@c", code } });

                if (dtCust.Rows.Count == 0)
                {
                    lblStatus.Text      = $"✘ ไม่พบรหัส {code} ในระบบ";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    txtCode.SelectAll();
                    txtCode.Focus();
                    return;
                }

                name = dtCust.Rows[0][0]?.ToString();

                SQL.ExecuteCommand(@"
                    INSERT INTO dbo.t_ScanLog (Customer_code, Scanned_by, Note)
                    VALUES (@code, @user, @note)",
                    new Dictionary<string, object>
                    {
                        { "@code", code },
                        { "@user", AppSession.Username },
                        { "@note", txtNote.Text.Trim() }
                    });
            }
            catch (Exception ex)
            {
                ShowError("บันทึกไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }

            lblStatus.Text      = $"✔ บันทึกสำเร็จ — {code} : {name}";
            lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
            txtCode.Clear();
            txtNote.Clear();
            LoadHistory();

            ShowInfo($"บันทึกสำเร็จ\n{code} : {name}");
            txtCode.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtCode.Clear();
            txtNote.Clear();
            lblStatus.Text      = "พร้อม scan...";
            lblStatus.ForeColor = System.Drawing.Color.Gray;
            txtCode.Focus();
        }

        // ─── History ──────────────────────────────────────────────────────────
        private void LoadHistory()
        {
            try
            {
                SQL.Connect();

                var dt = SQL.ExecuteQuery(@"
                    SELECT TOP 50
                           sl.Log_id,
                           sl.Customer_code,
                           c.Full_name,
                           sl.Scanned_by,
                           sl.Scan_date,
                           sl.Note
                    FROM   dbo.t_ScanLog sl
                    LEFT JOIN dbo.t_Customers c ON c.Customer_code = sl.Customer_code
                    ORDER BY sl.Scan_date DESC, sl.Log_id DESC", null);

                dgvLog.DataSource = dt;
                if (dgvLog.Columns["Log_id"] != null)
                    dgvLog.Columns["Log_id"].Visible = false;
            }
            catch (Exception ex)
            {
                ShowError("โหลดประวัติไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadHistory();
    }
}
