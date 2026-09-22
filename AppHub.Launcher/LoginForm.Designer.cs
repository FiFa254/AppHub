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
            this.lblTitle    = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin    = new System.Windows.Forms.Button();
            this.btnCancel   = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(30, 60, 120);
            this.lblTitle.Height    = 56;
            this.lblTitle.Text      = "🔐 AppHub — เข้าสู่ระบบ";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblUsername
            this.lblUsername.Text     = "Username";
            this.lblUsername.Location = new System.Drawing.Point(30, 73);
            this.lblUsername.AutoSize = true;

            // txtUsername
            this.txtUsername.Location = new System.Drawing.Point(110, 70);
            this.txtUsername.Size     = new System.Drawing.Size(200, 24);
            this.txtUsername.TabIndex = 0;

            // lblPassword
            this.lblPassword.Text     = "Password";
            this.lblPassword.Location = new System.Drawing.Point(30, 107);
            this.lblPassword.AutoSize = true;

            // txtPassword
            this.txtPassword.Location              = new System.Drawing.Point(110, 104);
            this.txtPassword.Size                  = new System.Drawing.Size(200, 24);
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.TabIndex              = 1;

            // btnLogin
            this.btnLogin.Text      = "เข้าสู่ระบบ";
            this.btnLogin.Location  = new System.Drawing.Point(110, 146);
            this.btnLogin.Size      = new System.Drawing.Size(96, 30);
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(220, 235, 255);
            this.btnLogin.TabIndex  = 2;
            this.btnLogin.Click    += new System.EventHandler(this.btnLogin_Click);

            // btnCancel
            this.btnCancel.Text      = "ยกเลิก";
            this.btnCancel.Location  = new System.Drawing.Point(214, 146);
            this.btnCancel.Size      = new System.Drawing.Size(96, 30);
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.TabIndex  = 3;
            this.btnCancel.Click    += new System.EventHandler(this.btnCancel_Click);

            // LoginForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(350, 200);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.lblTitle);
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

        private System.Windows.Forms.Label    lblTitle;
        private System.Windows.Forms.Label    lblUsername;
        private System.Windows.Forms.TextBox  txtUsername;
        private System.Windows.Forms.Label    lblPassword;
        private System.Windows.Forms.TextBox  txtPassword;
        private System.Windows.Forms.Button   btnLogin;
        private System.Windows.Forms.Button   btnCancel;
    }
}
