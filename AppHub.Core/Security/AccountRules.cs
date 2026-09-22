using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AppHub.Core
{
    /// <summary>
    /// กฎของบัญชีผู้ใช้ — username, รหัสผ่าน, การ hash, การล็อกเมื่อ login ผิด
    /// ใช้ร่วมกันทุกหน้าที่สร้าง/เปลี่ยนรหัสผ่าน
    /// </summary>
    public static class AccountRules
    {
        public const int PasswordMinLength  = 8;
        public const int PasswordMaxLength  = 128;
        public const int MaxFailedLogins    = 5;
        public const int LockoutMinutes     = 15;

        private static readonly Regex UsernamePattern = new Regex(@"^[A-Za-z0-9_.]{3,50}$");

        /// <summary>
        /// เงื่อนไขรหัสผ่านแต่ละข้อ (ข้อความ, ผ่านหรือไม่) — ใช้แสดง checklist บนหน้าจอ
        /// </summary>
        public static List<KeyValuePair<string, bool>> CheckPassword(string password)
        {
            password = password ?? "";
            return new List<KeyValuePair<string, bool>>
            {
                Rule($"อย่างน้อย {PasswordMinLength} ตัวอักษร",
                     password.Length >= PasswordMinLength && password.Length <= PasswordMaxLength),
                Rule("มีตัวพิมพ์เล็ก (a-z)",        password.Any(c => c >= 'a' && c <= 'z')),
                Rule("มีตัวพิมพ์ใหญ่ (A-Z)",        password.Any(c => c >= 'A' && c <= 'Z')),
                Rule("มีอักขระพิเศษ (เช่น ! @ # $)", password.Any(IsSpecial)),
            };
        }

        /// <summary>
        /// null = รหัสผ่านใช้ได้, ไม่ใช่ null = ข้อความบอกว่าขาดอะไร
        /// </summary>
        public static string ValidatePassword(string password)
        {
            var failed = CheckPassword(password).Where(r => !r.Value).Select(r => "• " + r.Key).ToList();
            return failed.Count == 0
                ? null
                : "รหัสผ่านยังไม่ตรงเงื่อนไข:\n" + string.Join("\n", failed);
        }

        /// <summary>
        /// null = username ใช้ได้
        /// </summary>
        public static string ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return "กรุณากรอก Username";
            if (!UsernamePattern.IsMatch(username.Trim()))
                return "Username ต้องยาว 3-50 ตัว ใช้ได้เฉพาะ a-z, A-Z, 0-9, _ และ .";
            return null;
        }

        /// <summary>
        /// SHA-256 hex (lowercase) — ตรงกับที่เก็บใน t_Users.Password_hash
        /// </summary>
        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var sb = new StringBuilder();
                foreach (byte b in sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? "")))
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private static bool IsSpecial(char c)
            => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c);

        private static KeyValuePair<string, bool> Rule(string text, bool passed)
            => new KeyValuePair<string, bool>(text, passed);
    }
}
