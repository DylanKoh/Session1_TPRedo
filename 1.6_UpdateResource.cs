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
    public partial class UpdateResource : Form
    {
        List<string> _list = new List<string>();
        int _resourceID = 0;
        public UpdateResource(int ResourceID)
        {
            InitializeComponent();
            _resourceID = ResourceID;
        }

        private void UpdateResource_Load(object sender, EventArgs e)
        {
            using (var context = new Session1Entities())
            {
                clbAllocatedSkills.Items.Clear();

                var getSkills = (from x in context.Skills
                                 select x.skillName).Distinct().ToArray();
                clbAllocatedSkills.Items.AddRange(getSkills);

                var getResource = (from x in context.Resources
                                   where x.resId == _resourceID
                                   select x).FirstOrDefault();
                lblResourceName.Text = getResource.resName;
                lblResourceType.Text = getResource.Resource_Type.resTypeName;
                nudAvailableQuantity.Value = getResource.remainingQuantity;
                var allocation = (from x in context.Resource_Allocation
                                  where x.resIdFK == _resourceID
                                  select x);
                foreach (var skill in allocation)
                {
                    _list.Add(skill.Skill.skillName);
                }
                CheckItems();
            }
        }
        private void CheckItems()
        {
            foreach (var item in _list)
            {
                var index = clbAllocatedSkills.Items.IndexOf(item);
                clbAllocatedSkills.SetItemChecked(index, true);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            (new ResourceManagement()).ShowDialog();
            this.Close();
        }
        private void nudAvailableQuantity_ValueChanged(object sender, EventArgs e)
        {
            if (nudAvailableQuantity.Value > 0)
            {
                clbAllocatedSkills.Enabled = true;
            }
            else
            {
                foreach (var item in _list)
                {
                    var index = clbAllocatedSkills.Items.IndexOf(item);
                    clbAllocatedSkills.SetItemChecked(index, false);
                }
                _list.Clear();
                clbAllocatedSkills.Enabled = false;
            }
        }

        private void clbAllocatedSkills_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (clbAllocatedSkills.SelectedItem != null)
            {
                if (e.NewValue == CheckState.Checked)
                {
                    if (!_list.Contains(clbAllocatedSkills.SelectedItem.ToString()))
                    {
                        _list.Add(clbAllocatedSkills.SelectedItem.ToString());
                    }
                }
                else
                {
                    if (_list.Contains(clbAllocatedSkills.SelectedItem.ToString()))
                    {
                        _list.Remove(clbAllocatedSkills.SelectedItem.ToString());
                    }
                }
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (var context = new Session1Entities())
            {
                var getAllocation = (from x in context.Resource_Allocation
                                     where x.resIdFK == _resourceID
                                     select x);
                foreach (var item in getAllocation)
                {
                    context.Resource_Allocation.Remove(item);
                }
                context.SaveChanges();
                var getResource = (from x in context.Resources
                                   where x.resId == _resourceID
                                   select x).FirstOrDefault();
                getResource.remainingQuantity = Convert.ToInt32(nudAvailableQuantity.Value);
                context.SaveChanges();
                foreach (var item in _list)
                {
                    var getSkillID = (from x in context.Skills
                                      where x.skillName == item
                                      select x.skillId).FirstOrDefault();
                    context.Resource_Allocation.Add(new Resource_Allocation() { resIdFK = _resourceID, skillIdFK = getSkillID });
                }
                context.SaveChanges();
                MessageBox.Show("Updated resource successfully!", "Update Resource", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                (new ResourceManagement()).ShowDialog();
                this.Close();
            }
        }
    }
}
