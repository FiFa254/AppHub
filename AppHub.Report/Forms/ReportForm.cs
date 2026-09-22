using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;

namespace AppHub.Report
{
    public partial class ReportForm : AppHubCRUDForm
    {
        public ReportForm()
        {
            InitializeComponent();
            LblTitle.Text   = "รายงานข้อมูลลูกค้า";
            Theme.SetIcon(btnSearch, Theme.Icons.Search);
            Theme.SetIcon(btnExport, Theme.Icons.Export);
            dtpFrom.Value   = DateTime.Today.AddDays(-30);
            dtpTo.Value     = DateTime.Today;
            SetPlaceholder(txtSearch, "ค้นหา ชื่อ หรือ รหัส...");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadReport();
        }

        // ─── Load ─────────────────────────────────────────────────────────────
        private void LoadReport()
        {
            bool hasFilter = !string.IsNullOrWhiteSpace(txtSearch.Text) || chkDateFilter.Checked;

            if (chkDateFilter.Checked && dtpFrom.Value.Date > dtpTo.Value.Date)
            {
                ShowWarning("วันที่เริ่มต้นต้องไม่เกินวันที่สิ้นสุด");
                return;
            }

            int rowCount;
            try
            {
                SQL.Connect();

                string sql = @"
                    SELECT Customer_code, Full_name, Phone, Email, Address,
                           Created_date, Updated_date
                    FROM   dbo.t_Customers
                    WHERE  1=1";

                var p = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    sql += " AND (Full_name LIKE @f OR Customer_code LIKE @f)";
                    p["@f"] = "%" + txtSearch.Text.Trim() + "%";
                }

                if (chkDateFilter.Checked)
                {
                    sql += " AND CAST(Created_date AS DATE) BETWEEN @from AND @to";
                    p["@from"] = dtpFrom.Value.Date;
                    p["@to"]   = dtpTo.Value.Date;
                }

                sql += " ORDER BY Customer_code";

                var dt = SQL.ExecuteQuery(sql, p.Count > 0 ? p : null);
                DGV.DataSource = dt;
                rowCount = dt.Rows.Count;
                UpdateCount(rowCount);
            }
            catch (Exception ex)
            {
                ShowError("โหลดรายงานไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }

            if (hasFilter && rowCount == 0)
                ShowWarning("ไม่พบข้อมูล");
        }

        // ─── Events ───────────────────────────────────────────────────────────
        private void btnSearch_Click(object sender, EventArgs e) => LoadReport();
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            chkDateFilter.Checked = false;
            LoadReport();
        }
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            LoadReport();
        }
        private void chkDateFilter_CheckedChanged(object sender, EventArgs e)
        {
            dtpFrom.Enabled = dtpTo.Enabled = chkDateFilter.Checked;
        }

        // ─── Export CSV ───────────────────────────────────────────────────────
        private void btnExport_Click(object sender, EventArgs e)
        {
            var dt = DGV.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                ShowWarning("ไม่มีข้อมูลที่จะ export");
                return;
            }

            string path = PickSaveFile("CSV Files|*.csv",
                                       $"Customer_Report_{DateTime.Today:yyyyMMdd}.csv");
            if (path == null) return;

            try
            {
                var sb = new StringBuilder();
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (c > 0) sb.Append(',');
                    sb.Append('"').Append(dt.Columns[c].ColumnName).Append('"');
                }
                sb.AppendLine();
                foreach (DataRow row in dt.Rows)
                {
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        if (c > 0) sb.Append(',');
                        sb.Append('"').Append(FormatCell(row[c]).Replace("\"", "\"\"")).Append('"');
                    }
                    sb.AppendLine();
                }
                // UTF-8 มี BOM เพื่อให้ Excel อ่านภาษาไทยถูก
                File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
            }
            catch (Exception ex)
            {
                ShowError("Export ไม่สำเร็จ:\n" + ex.Message);
                return;
            }
            ShowInfo($"Export สำเร็จ:\n{path}");
        }

        private static string FormatCell(object value)
        {
            if (value == null || value == DBNull.Value) return "";
            if (value is DateTime d)
                return d.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            return value.ToString();
        }
    }
}
