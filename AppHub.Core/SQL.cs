using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AppHub.Core
{
    /// <summary>
    /// Static SQL helper
    /// SQL.Connect() อยู่ใน try, SQL.Disconnect() อยู่ใน finally เสมอ
    /// </summary>
    public static class SQL
    {
        private static SqlConnection _conn;

        // ─── Connection ───────────────────────────────────────────────────────
        public static void Connect()
        {
            var setting = ConfigurationManager.ConnectionStrings["AppHubDB"];
            if (setting == null)
                throw new ConfigurationErrorsException(
                    "ไม่พบ connection string \"AppHubDB\" ใน App.config");

            Disconnect();   // กัน connection เก่าค้าง
            _conn = new SqlConnection(setting.ConnectionString);
            _conn.Open();
        }

        public static void Disconnect()
        {
            try
            {
                if (_conn != null)
                {
                    _conn.Close();
                    _conn.Dispose();
                    _conn = null;
                }
            }
            catch { /* ไม่ throw ใน Disconnect */ }
        }

        // ─── ExecuteQuery → DataTable ─────────────────────────────────────────
        public static DataTable ExecuteQuery(string sql,
            Dictionary<string, object> parameters = null)
        {
            using (var cmd = new SqlCommand(sql, _conn))
            {
                BindParameters(cmd, parameters);
                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd))
                    adapter.Fill(dt);
                return dt;
            }
        }

        // ─── ExecuteCommand → rows affected ──────────────────────────────────
        public static int ExecuteCommand(string sql,
            Dictionary<string, object> parameters = null)
        {
            using (var cmd = new SqlCommand(sql, _conn))
            {
                BindParameters(cmd, parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        // ─── ExecuteScalar → single value ────────────────────────────────────
        public static object ExecuteScalar(string sql,
            Dictionary<string, object> parameters = null)
        {
            using (var cmd = new SqlCommand(sql, _conn))
            {
                BindParameters(cmd, parameters);
                return cmd.ExecuteScalar();
            }
        }

        // ─── Private helper ───────────────────────────────────────────────────
        private static void BindParameters(SqlCommand cmd,
            Dictionary<string, object> parameters)
        {
            if (parameters == null) return;
            foreach (var kv in parameters)
                cmd.Parameters.AddWithValue(kv.Key, kv.Value ?? DBNull.Value);
        }
    }
}
