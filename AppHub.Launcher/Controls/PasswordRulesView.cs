using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AppHub.Core;

namespace AppHub.Launcher
{
    /// <summary>
    /// แสดงเงื่อนไขรหัสผ่านเป็น checklist (✔ / ✘) อัปเดตตามที่พิมพ์
    /// </summary>
    public class PasswordRulesView : Panel
    {
        private static readonly Color PassColor = AppHub.Core.UI.Theme.Success;
        private static readonly Color FailColor = AppHub.Core.UI.Theme.TextMuted;

        private readonly List<Label> _labels = new List<Label>();

        public PasswordRulesView()
        {
            var rules = AccountRules.CheckPassword("");
            for (int i = 0; i < rules.Count; i++)
            {
                var lbl = new Label
                {
                    AutoSize = true,
                    Location = new Point(0, i * 20),
                    Font     = new Font("Segoe UI", 8.5F)
                };
                _labels.Add(lbl);
                this.Controls.Add(lbl);
            }
            this.Height = rules.Count * 20;
            ShowRules("");
        }

        /// <summary>true = ผ่านทุกข้อ</summary>
        public bool AllPassed { get; private set; }

        public void ShowRules(string password)
        {
            var rules = AccountRules.CheckPassword(password);
            AllPassed = true;
            for (int i = 0; i < rules.Count; i++)
            {
                bool ok = rules[i].Value;
                _labels[i].Text      = (ok ? "✔ " : "✘ ") + rules[i].Key;
                _labels[i].ForeColor = ok ? PassColor : FailColor;
                AllPassed &= ok;
            }
        }
    }
}
