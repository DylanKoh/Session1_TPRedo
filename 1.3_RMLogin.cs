using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Session1_TPRedo
{
    public partial class RMLogin : Form
    {
        public RMLogin()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            (new MainMenu()).ShowDialog();
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using (var context = new Session1Entities())
            {
                var getUser = (from x in context.Users
                               where x.userId == txtUserID.Text
                               select x).FirstOrDefault();
                if (getUser == null)
                {
                    MessageBox.Show("User does not exist!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (getUser.userPw != txtPassword.Text)
                {
                    MessageBox.Show("Password is incorrect!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    if (getUser.userTypeIdFK == 1)
                    {
                        MessageBox.Show($"Welcome {getUser.userName}!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide();
                        (new ResourceManagement()).ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Sorry, your account is not permitted to use this application!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }
    }
}
