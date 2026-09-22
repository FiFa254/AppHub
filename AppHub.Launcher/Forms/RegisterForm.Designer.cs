namespace AppHub.Launcher
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlBrand        = new AppHub.Launcher.BrandPanel();
            this.lblHeading      = new System.Windows.Forms.Label();
            this.lblSubtitle     = new System.Windows.Forms.Label();
            this.lblFullName     = new System.Windows.Forms.Label();
            this.txtFullName     = new System.Windows.Forms.TextBox();
            this.lblUsername     = new System.Windows.Forms.Label();
            this.txtUsername     = new System.Windows.Forms.TextBox();
            this.lblPassword     = new System.Windows.Forms.Label();
            this.txtPassword     = new System.Windows.Forms.TextBox();
            this.pwdRules        = new AppHub.Launcher.PasswordRulesView();
            this.lblConfirm      = new System.Windows.Forms.Label();
            this.txtConfirm      = new System.Windows.Forms.TextBox();
            this.chkShowPassword = new System.Windows.Forms.CheckBox();
            this.btnRegister     = new System.Windows.Forms.Button();
            this.btnCancel       = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // pnlBrand
            this.pnlBrand.Width   = 300;
            this.pnlBrand.Tagline = "สมัครครั้งเดียว\nใช้งานได้ทุก module ที่ได้รับสิทธิ์";

            // lblHeading / lblSubtitle
            this.lblHeading.AutoSize  = true;
            this.lblHeading.Location  = new System.Drawing.Point(336, 32);
            this.lblHeading.Font      = AppHub.Core.UI.Theme.Heading;
            this.lblHeading.ForeColor = AppHub.Core.UI.Theme.TextPrimary;
            this.lblHeading.Text      = "สมัครสมาชิก";

            this.lblSubtitle.AutoSize  = true;
            this.lblSubtitle.Location  = new System.Drawing.Point(340, 78);
            this.lblSubtitle.ForeColor = AppHub.Core.UI.Theme.TextMuted;
            this.lblSubtitle.Text      = "สร้างบัญชีเพื่อเริ่มใช้งาน AppHub";

            // lblFullName / txtFullName
            this.lblFullName.AutoSize = true;
            this.lblFullName.Location = new System.Drawing.Point(340, 116);
            this.lblFullName.Font     = AppHub.Core.UI.Theme.BodyBold;
            this.lblFullName.Text     = "ชื่อ-นามสกุล *";

            this.txtFullName.Location = new System.Drawing.Point(340, 138);
            this.txtFullName.Size     = new System.Drawing.Size(360, 26);
            this.txtFullName.Font     = AppHub.Core.UI.Theme.Input;
            this.txtFullName.TabIndex = 0;

            // lblUsername / txtUsername
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(340, 178);
            this.lblUsername.Font     = AppHub.Core.UI.Theme.BodyBold;
            this.lblUsername.Text     = "ชื่อผู้ใช้ (Username) *";

            this.txtUsername.Location = new System.Drawing.Point(340, 200);
            this.txtUsername.Size     = new System.Drawing.Size(360, 26);
            this.txtUsername.Font     = AppHub.Core.UI.Theme.Input;
            this.txtUsername.TabIndex = 1;

            // lblPassword / txtPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(340, 240);
            this.lblPassword.Font     = AppHub.Core.UI.Theme.BodyBold;
            this.lblPassword.Text     = "รหัสผ่าน *";

            this.txtPassword.Location              = new System.Drawing.Point(340, 262);
            this.txtPassword.Size                  = new System.Drawing.Size(360, 26);
            this.txtPassword.Font                  = AppHub.Core.UI.Theme.Input;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.TabIndex              = 2;
            this.txtPassword.TextChanged          += new System.EventHandler(this.txtPassword_TextChanged);

            // pwdRules
            this.pwdRules.Location = new System.Drawing.Point(342, 296);
            this.pwdRules.Width    = 360;

            // lblConfirm / txtConfirm
            this.lblConfirm.AutoSize = true;
            this.lblConfirm.Location = new System.Drawing.Point(340, 382);
            this.lblConfirm.Font     = AppHub.Core.UI.Theme.BodyBold;
            this.lblConfirm.Text     = "ยืนยันรหัสผ่าน *";

            this.txtConfirm.Location              = new System.Drawing.Point(340, 404);
            this.txtConfirm.Size                  = new System.Drawing.Size(360, 26);
            this.txtConfirm.Font                  = AppHub.Core.UI.Theme.Input;
            this.txtConfirm.UseSystemPasswordChar = true;
            this.txtConfirm.TabIndex              = 3;

            // chkShowPassword
            this.chkShowPassword.AutoSize  = true;
            this.chkShowPassword.Location  = new System.Drawing.Point(340, 440);
            this.chkShowPassword.ForeColor = AppHub.Core.UI.Theme.TextMuted;
            this.chkShowPassword.Text      = "แสดงรหัสผ่าน";
            this.chkShowPassword.TabIndex  = 4;
            this.chkShowPassword.CheckedChanged += new System.EventHandler(this.chkShowPassword_CheckedChanged);

            // btnRegister (primary — AcceptButton)
            this.btnRegister.Text     = "สร้างบัญชี";
            this.btnRegister.Location = new System.Drawing.Point(340, 474);
            this.btnRegister.Size     = new System.Drawing.Size(360, 40);
            this.btnRegister.TabIndex = 5;
            this.btnRegister.Click   += new System.EventHandler(this.btnRegister_Click);

            // btnCancel (ghost)
            this.btnCancel.Tag      = "ghost";
            this.btnCancel.Text     = "← กลับไปหน้าเข้าสู่ระบบ";
            this.btnCancel.Location = new System.Drawing.Point(340, 520);
            this.btnCancel.Size     = new System.Drawing.Size(360, 32);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Click   += new System.EventHandler(this.btnCancel_Click);

            // RegisterForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(740, 576);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.chkShowPassword);
            this.Controls.Add(this.txtConfirm);
            this.Controls.Add(this.lblConfirm);
            this.Controls.Add(this.pwdRules);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtFullName);
            this.Controls.Add(this.lblFullName);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.pnlBrand);
            this.Font            = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.Name            = "RegisterForm";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text            = "AppHub — สมัครสมาชิก";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private AppHub.Launcher.BrandPanel              pnlBrand;
        private System.Windows.Forms.Label              lblHeading;
        private System.Windows.Forms.Label              lblSubtitle;
        private System.Windows.Forms.Label              lblFullName;
        private System.Windows.Forms.TextBox            txtFullName;
        private System.Windows.Forms.Label              lblUsername;
        private System.Windows.Forms.TextBox            txtUsername;
        private System.Windows.Forms.Label              lblPassword;
        private System.Windows.Forms.TextBox            txtPassword;
        private AppHub.Launcher.PasswordRulesView       pwdRules;
        private System.Windows.Forms.Label              lblConfirm;
        private System.Windows.Forms.TextBox            txtConfirm;
        private System.Windows.Forms.CheckBox           chkShowPassword;
        private System.Windows.Forms.Button             btnRegister;
        private System.Windows.Forms.Button             btnCancel;
    }
}
