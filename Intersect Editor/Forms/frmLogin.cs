using System;
using System.Windows.Forms;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            Globals.GameSocket = new Network();
            tmrSocket.Enabled = true;
            
        }

        private void tmrSocket_Tick(object sender, EventArgs e)
        {
            Globals.GameSocket.Update();
            var statusString = "Connecting to server...";
            if (Globals.GameSocket.IsConnected)
            {
                statusString = "Connected to server.";
            }
            else if (Globals.GameSocket.IsConnecting)
            {

            }
            else
            {
                statusString = "Failed to connect, retrying in " + ((Globals.GameSocket.ReconnectTime - Environment.TickCount)/1000).ToString("0") + " seconds.";
            }
            Globals.LoginForm.lblStatus.Text = statusString;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Globals.GameSocket.IsConnected) return;
            if (txtUsername.Text.Trim().Length > 0 && txtPassword.Text.Trim().Length > 0)
            {
                PacketSender.SendLogin(txtUsername.Text.Trim(), txtPassword.Text.Trim());
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar != 13) return;
            e.Handled = true;
            if (txtUsername.Text.Trim().Length > 0 && txtPassword.Text.Trim().Length > 0)
            {
                PacketSender.SendLogin(txtUsername.Text.Trim(), txtPassword.Text.Trim());
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Exit();
        }
    }


}
