using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using AppHub.Core;
using AppHub.Core.UI;
using OfficeOpenXml;

namespace AppHub.Import
{
    public partial class ImportForm : AppHubForm
    {
        private static readonly string[] RequiredColumns =
            { "Customer_code", "Full_name", "Phone", "Email", "Address" };

        private DataTable _preview;

        public ImportForm()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        // ─── Browse ───────────────────────────────────────────────────────────
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string path = PickOpenFile("Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                                       "เลือกไฟล์ Excel");
            if (path == null) return;

            ResetPreview();
            if (!string.Equals(Path.GetExtension(path), ".xlsx",
                    StringComparison.OrdinalIgnoreCase))
            {
                ShowWarning("กรุณาเลือกไฟล์ .xlsx");
                return;
            }

            txtFile.Text = path;
            LoadPreview(path);
        }

        // ─── Preview ──────────────────────────────────────────────────────────
        private void ResetPreview()
        {
            _preview              = null;
            dgvPreview.DataSource = null;
            txtFile.Clear();
            btnImport.Enabled     = false;
            lblInfo.Text          = "เลือกไฟล์ Excel เพื่อดูตัวอย่างก่อนนำเข้า";
        }

        private void LoadPreview(string path)
        {
            DataTable dt;
            try
            {
                dt = ReadExcel(path);
            }
            catch (Exception ex)
            {
                ShowError("อ่านไฟล์ไม่สำเร็จ:\n" + ex.Message);
                return;
            }

            if (dt.Rows.Count == 0)
            {
                ShowWarning("ไม่พบข้อมูลในไฟล์");
                lblInfo.Text = "ไม่พบข้อมูลในไฟล์";
                return;
            }

            _preview              = dt;
            dgvPreview.DataSource = _preview;
            lblInfo.Text          = $"พบข้อมูล {_preview.Rows.Count} แถว (ยังไม่ได้นำเข้า)";
            btnImport.Enabled     = true;
        }

        private static DataTable ReadExcel(string path)
        {
            var dt = new DataTable();
            using (var pkg = new ExcelPackage(new FileInfo(path)))
            {
                if (pkg.Workbook.Worksheets.Count == 0)
                    throw new Exception("ไฟล์ Excel ไม่มี worksheet");

                var ws = pkg.Workbook.Worksheets[0];
                if (ws.Dimension == null)
                    throw new Exception("ไฟล์ Excel ว่างเปล่า");

                for (int c = 1; c <= RequiredColumns.Length; c++)
                {
                    string header = ws.Cells[1, c].Text?.Trim();
                    if (!string.Equals(header, RequiredColumns[c - 1], StringComparison.OrdinalIgnoreCase))
                        throw new Exception(
                            $"Header คอลัมน์ที่ {c} ต้องเป็น \"{RequiredColumns[c - 1]}\" แต่พบ \"{header}\"");
                }

                foreach (var col in RequiredColumns) dt.Columns.Add(col);
                dt.Columns.Add("สถานะ");

                for (int r = 2; r <= ws.Dimension.End.Row; r++)
                {
                    if (string.IsNullOrWhiteSpace(ws.Cells[r, 1].Text)) continue;
                    var row = dt.NewRow();
                    for (int c = 1; c <= RequiredColumns.Length; c++)
                        row[c - 1] = ws.Cells[r, c].Text?.Trim() ?? "";
                    row["สถานะ"] = "รอนำเข้า";
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        // ─── Import ───────────────────────────────────────────────────────────
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (_preview == null || _preview.Rows.Count == 0) return;

            int inserted = 0, skipped = 0, invalid = 0;
            var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                SQL.Connect();

                var dtExist = SQL.ExecuteQuery("SELECT Customer_code FROM dbo.t_Customers", null);
                foreach (DataRow r in dtExist.Rows)
                    existing.Add(r[0].ToString());

                foreach (DataRow row in _preview.Rows)
                {
                    string code = row["Customer_code"].ToString();
                    if (existing.Contains(code))
                    {
                        row["สถานะ"] = "⚠️ ซ้ำ — ข้าม";
                        skipped++;
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(row["Full_name"].ToString()))
                    {
                        row["สถานะ"] = "❌ ไม่มีชื่อ — ข้าม";
                        invalid++;
                        continue;
                    }

                    SQL.ExecuteCommand(@"
                        INSERT INTO dbo.t_Customers
                            (Customer_code, Full_name, Phone, Email, Address)
                        VALUES (@code, @name, @phone, @email, @addr)",
                        new Dictionary<string, object>
                        {
                            { "@code",  code },
                            { "@name",  row["Full_name"].ToString() },
                            { "@phone", row["Phone"].ToString() },
                            { "@email", row["Email"].ToString() },
                            { "@addr",  row["Address"].ToString() },
                        });

                    existing.Add(code);
                    row["สถานะ"] = "✅ นำเข้าแล้ว";
                    inserted++;
                }
            }
            catch (Exception ex)
            {
                ShowError($"นำเข้าไม่สำเร็จ (นำเข้าไปแล้ว {inserted} แถว):\n" + ex.Message);
                return;
            }
            finally
            {
                SQL.Disconnect();
            }

            string summary = $"นำเข้าสำเร็จ {inserted} รายการ | ข้าม (ซ้ำ) {skipped} รายการ";
            if (invalid > 0) summary += $" | ข้อมูลไม่ครบ {invalid} รายการ";

            lblInfo.Text      = summary;
            btnImport.Enabled = false;
            ShowInfo(summary.Replace(" | ", "\n"), "Import สำเร็จ");
        }
    }
}
