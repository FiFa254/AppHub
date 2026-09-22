namespace AppHub.Launcher
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlBrand    = new AppHub.Launcher.BrandPanel();
            this.lblHeading  = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin    = new System.Windows.Forms.Button();
            this.lnkRegister = new System.Windows.Forms.LinkLabel();
            this.btnCancel   = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // pnlBrand
            this.pnlBrand.Width = 320;

            // lblHeading
            this.lblHeading.AutoSize  = true;
            this.lblHeading.Location  = new System.Drawing.Point(356, 48);
            this.lblHeading.Font      = AppHub.Core.UI.Theme.Heading;
            this.lblHeading.ForeColor = AppHub.Core.UI.Theme.TextPrimary;
            this.lblHeading.Text      = "เข้าสู่ระบบ";

            // lblSubtitle
            this.lblSubtitle.AutoSize  = true;
            this.lblSubtitle.Location  = new System.Drawing.Point(360, 94);
            this.lblSubtitle.ForeColor = AppHub.Core.UI.Theme.TextMuted;
            this.lblSubtitle.Text      = "ยินดีต้อนรับ กรุณาเข้าสู่ระบบเพื่อใช้งาน";

            // lblUsername / txtUsername
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(360, 140);
            this.lblUsername.Font     = AppHub.Core.UI.Theme.BodyBold;
            this.lblUsername.Text     = "ชื่อผู้ใช้";

            this.txtUsername.Location = new System.Drawing.Point(360, 162);
            this.txtUsername.Size     = new System.Drawing.Size(340, 26);
            this.txtUsername.Font     = AppHub.Core.UI.Theme.Input;
            this.txtUsername.TabIndex = 0;

            // lblPassword / txtPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(360, 206);
            this.lblPassword.Font     = AppHub.Core.UI.Theme.BodyBold;
            this.lblPassword.Text     = "รหัสผ่าน";

            this.txtPassword.Location              = new System.Drawing.Point(360, 228);
            this.txtPassword.Size                  = new System.Drawing.Size(340, 26);
            this.txtPassword.Font                  = AppHub.Core.UI.Theme.Input;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.TabIndex              = 1;

            // btnLogin (primary — AcceptButton)
            this.btnLogin.Text     = "เข้าสู่ระบบ";
            this.btnLogin.Location = new System.Drawing.Point(360, 280);
            this.btnLogin.Size     = new System.Drawing.Size(340, 40);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Click   += new System.EventHandler(this.btnLogin_Click);

            // lnkRegister
            this.lnkRegister.AutoSize     = true;
            this.lnkRegister.Location     = new System.Drawing.Point(360, 334);
            this.lnkRegister.Text         = "ยังไม่มีบัญชี? สมัครสมาชิก";
            this.lnkRegister.TabIndex     = 3;
            this.lnkRegister.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkRegister_LinkClicked);

            // btnCancel (ghost)
            this.btnCancel.Tag      = "ghost";
            this.btnCancel.Text     = "ปิดโปรแกรม";
            this.btnCancel.Location = new System.Drawing.Point(600, 326);
            this.btnCancel.Size     = new System.Drawing.Size(100, 32);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Click   += new System.EventHandler(this.btnCancel_Click);

            // LoginForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(740, 420);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lnkRegister);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.pnlBrand);
            this.Font            = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.Name            = "LoginForm";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text            = "AppHub — Login";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private AppHub.Launcher.BrandPanel      pnlBrand;
        private System.Windows.Forms.Label      lblHeading;
        private System.Windows.Forms.Label      lblSubtitle;
        private System.Windows.Forms.Label      lblUsername;
        private System.Windows.Forms.TextBox    txtUsername;
        private System.Windows.Forms.Label      lblPassword;
        private System.Windows.Forms.TextBox    txtPassword;
        private System.Windows.Forms.Button     btnLogin;
        private System.Windows.Forms.LinkLabel  lnkRegister;
        private System.Windows.Forms.Button     btnCancel;
    }
}
