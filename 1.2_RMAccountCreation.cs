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
    public partial class RMAccountCreation : Form
    {
        public RMAccountCreation()
        {
            InitializeComponent();
        }

        private void RMAccountCreation_Load(object sender, EventArgs e)
        {
            using (var context = new Session1Entities())
            {
                var getUserTypes = (from x in context.User_Type
                                    select x.userTypeName).Distinct().ToArray();
                cbUserType.Items.AddRange(getUserTypes);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            using (var context = new Session1Entities())
            {
                var checkUserID = (from x in context.Users
                                   where x.userId == txtUserID.Text
                                   select x).FirstOrDefault();
                if (txtUserID.TextLength < 8)
                {
                    MessageBox.Show("User ID must be 8 characters long or more!", "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (cbUserType.SelectedItem == null)
                {
                    MessageBox.Show("Please select User Type!", "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (checkUserID != null)
                {
                    MessageBox.Show("User ID has been taken!", "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtPassword.Text != txtReEnterPassword.Text)
                {
                    MessageBox.Show("Passwords do not match! Please check your passwords again!", "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var getUserTypeID = (from x in context.User_Type
                                         where x.userTypeName == cbUserType.SelectedItem.ToString()
                                         select x.userTypeId).FirstOrDefault();
                    context.Users.Add(new User()
                    {
                        userId = txtUserID.Text,
                        userName = txtUserName.Text,
                        userPw = txtReEnterPassword.Text,
                        userTypeIdFK = getUserTypeID
                    });
                    context.SaveChanges();
                    MessageBox.Show("Account created successfully!", "Create Account", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
