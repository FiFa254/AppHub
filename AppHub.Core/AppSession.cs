using System;
using System.Collections.Generic;

namespace AppHub.Core
{
    /// <summary>
    /// เก็บข้อมูลผู้ใช้ที่ login อยู่ ใช้ร่วมกันทุก module
    /// </summary>
    public static class AppSession
    {
        private static readonly HashSet<string> _permissions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static int    UserId   { get; set; }
        public static string Username { get; set; }
        public static string FullName { get; set; }
        public static bool   IsAdmin  { get; set; }

        public static bool IsLoggedIn => UserId > 0;

        /// <summary>
        /// ตั้งค่า module ที่ user มีสิทธิ์ (แทนที่ของเดิมทั้งหมด)
        /// </summary>
        public static void SetPermissions(List<string> codes)
        {
            _permissions.Clear();
            if (codes == null) return;
            foreach (string code in codes)
            {
                if (!string.IsNullOrWhiteSpace(code))
                    _permissions.Add(code.Trim());
            }
        }

        /// <summary>
        /// ตรวจสิทธิ์ module — Admin เข้าถึงได้ทุก module
        /// </summary>
        public static bool HasPermission(string moduleCode)
            => IsAdmin || _permissions.Contains(moduleCode);

        /// <summary>
        /// ล้าง session ตอน logout
        /// </summary>
        public static void Clear()
        {
            UserId   = 0;
            Username = null;
            FullName = null;
            IsAdmin  = false;
            _permissions.Clear();
        }
    }
}
