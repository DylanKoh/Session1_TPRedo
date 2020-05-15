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
    public partial class AddNewResource : Form
    {
        List<string> _list = new List<string>();
        public AddNewResource()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            (new ResourceManagement()).ShowDialog();
            this.Close();
        }

        private void AddNewResource_Load(object sender, EventArgs e)
        {
            using (var context = new Session1Entities())
            {
                cbResourceType.Items.Clear();
                clbAllocatedSkills.Items.Clear();
                var getResourceTypes = (from x in context.Resource_Type
                                        select x.resTypeName).Distinct().ToArray();
                cbResourceType.Items.AddRange(getResourceTypes);

                var getSkills = (from x in context.Skills
                                 select x.skillName).Distinct().ToArray();
                clbAllocatedSkills.Items.AddRange(getSkills);
            }
        }

        private void nudAvailableQuantity_ValueChanged(object sender, EventArgs e)
        {
            if (nudAvailableQuantity.Value > 0)
            {
                clbAllocatedSkills.Enabled = true;
            }
            else
            {
                clbAllocatedSkills.Enabled = false;
            }
        }

        private void clbAllocatedSkills_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                _list.Add(clbAllocatedSkills.SelectedItem.ToString());
            }
            else
            {
                _list.Remove(clbAllocatedSkills.SelectedItem.ToString());
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var context = new Session1Entities())
            {
                var checkResource = (from x in context.Resources
                                     where x.resName == txtResourceName.Text
                                     select x).FirstOrDefault();
                if (checkResource != null)
                {

                }
                else if (cbResourceType.SelectedItem == null)
                {

                }
                else
                {
                    var getResourceTypeID = (from x in context.Resource_Type
                                             where x.resTypeName == cbResourceType.SelectedItem.ToString()
                                             select x.resTypeId).FirstOrDefault();
                    context.Resources.Add(new Resource()
                    {
                        resName = txtResourceName.Text,
                        remainingQuantity = Convert.ToInt32(nudAvailableQuantity.Value),
                        resTypeIdFK = getResourceTypeID
                    });
                    context.SaveChanges();
                    var getNewID = (from x in context.Resources
                                    where x.resName == txtResourceName.Text
                                    select x.resId).FirstOrDefault();
                    foreach (var item in _list)
                    {
                        var getSkillID = (from x in context.Skills
                                          where x.skillName == item
                                          select x.skillId).FirstOrDefault();
                        context.Resource_Allocation.Add(new Resource_Allocation() { resIdFK = getNewID, skillIdFK = getSkillID });
                    }
                    context.SaveChanges();
                    MessageBox.Show("Successfully added new resource!", "Add Resource", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtResourceName.Text = string.Empty;
                    nudAvailableQuantity.Value = 0;
                    AddNewResource_Load(null, null);
                }
            }
        }
    }
}
