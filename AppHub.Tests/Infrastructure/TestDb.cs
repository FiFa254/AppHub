using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AppHub.Tests.Infrastructure
{
    /// <summary>
    /// จัดการ DB สำหรับ test (AppHubDB_Test) — สร้างจาก setup.sql ตัวจริงของ solution
    /// </summary>
    internal static class TestDb
    {
        public const string DbName = "AppHubDB_Test";

        public static string ConnectionString
            => ConfigurationManager.ConnectionStrings["AppHubDB"].ConnectionString;

        private static string MasterConnectionString
            => new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = "master" }.ConnectionString;

        public static string SolutionDir { get; } = FindSolutionDir();

        public static string TestDataPath(string fileName)
            => Path.Combine(SolutionDir, "TestData", fileName);

        private static string FindSolutionDir()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "AppHub.sln")))
                dir = dir.Parent;
            if (dir == null)
                throw new InvalidOperationException("หา AppHub.sln ไม่เจอจาก " + AppDomain.CurrentDomain.BaseDirectory);
            return dir.FullName;
        }

        // ─── Setup ───────────────────────────────────────────────────────────
        public static void Recreate()
        {
            if (!ConnectionString.Contains(DbName))
                throw new InvalidOperationException("Test ต้องใช้ " + DbName + " เท่านั้น");

            SqlConnection.ClearAllPools();
            ExecuteOn(MasterConnectionString,
                $"IF DB_ID(N'{DbName}') IS NOT NULL " +
                $"BEGIN ALTER DATABASE [{DbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{DbName}]; END");
            RunSetupScript();
        }

        /// <summary>
        /// รัน setup.sql (แทนชื่อ DB เป็น AppHubDB_Test) — ทุก batch บน connection เดียว
        /// </summary>
        public static void RunSetupScript()
        {
            string script = File.ReadAllText(Path.Combine(SolutionDir, "setup.sql"), Encoding.UTF8)
                                .Replace("AppHubDB", DbName);

            using (var conn = new SqlConnection(MasterConnectionString))
            {
                conn.Open();
                foreach (string batch in Regex.Split(script, @"^\s*GO\s*$",
                             RegexOptions.Multiline | RegexOptions.IgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(batch)) continue;
                    using (var cmd = new SqlCommand(batch, conn))
                        cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// ล้างข้อมูลแล้ว seed ใหม่ — เรียกก่อนทุก test
        /// </summary>
        public static void ResetData()
        {
            Execute("DELETE dbo.t_ScanLog; DELETE dbo.t_UserModule; DELETE dbo.t_Customers; DELETE dbo.t_Users;");
            RunSetupScript();
        }

        // ─── Query helpers ───────────────────────────────────────────────────
        public static int Execute(string sql, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object Scalar(string sql, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static int Count(string sql, params SqlParameter[] parameters)
            => Convert.ToInt32(Scalar(sql, parameters));

        public static DataTable Query(string sql, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddRange(parameters);
                var dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public static SqlParameter P(string name, object value)
            => new SqlParameter(name, value ?? DBNull.Value);

        private static void ExecuteOn(string connectionString, string sql)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static string Sha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                var sb = new StringBuilder();
                foreach (byte b in sha.ComputeHash(Encoding.UTF8.GetBytes(input)))
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
