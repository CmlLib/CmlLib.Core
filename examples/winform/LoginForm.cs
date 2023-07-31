using CmlLib.Core.Auth;

namespace CmlLibWinFormSample
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
        }

        private void btnAutoLogin_Click(object sender, EventArgs e)
        {
            
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

        }

        private void btnSignout_Click(object sender, EventArgs e)
        {

        }

        private void btnInvalidate_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteToken_Click(object sender, EventArgs e)
        {

        }

        private void btnOfflineLogin_Click(object sender, EventArgs e)
        {
            UpdateSession(MSession.GetOfflineSession(txtUsername.Text));
        }

        private void UpdateSession(MSession session)
        {
            // Success to login!

            var mainForm = new MainForm(session);
            mainForm.FormClosed += (s, e) => this.Close();
            mainForm.Show();
            this.Hide();
        }
    }
}
