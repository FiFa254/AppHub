using System;
using System.Windows.Forms;
using AppHub.Core;

namespace AppHub.Launcher
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // วน Login → MainForm จนกว่าผู้ใช้จะปิดโปรแกรม (Logout = กลับมา Login ใหม่)
            while (true)
            {
                using (var login = new LoginForm())
                {
                    if (login.ShowDialog() != DialogResult.OK) return;
                }

                var main = new MainForm();
                Application.Run(main);

                if (!main.IsLogout) return;
                AppSession.Clear();
            }
        }
    }
}
