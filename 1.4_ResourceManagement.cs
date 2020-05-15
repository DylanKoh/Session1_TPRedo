using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Session1_TPRedo
{
    public partial class ResourceManagement : Form
    {
        public ResourceManagement()
        {
            InitializeComponent();

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            (new RMLogin()).ShowDialog();
            this.Close();
        }

        private void ResourceManagement_Load(object sender, EventArgs e)
        {
            cbSkill.Items.Clear();
            cbType.Items.Clear();
            cbSkill.Items.Add("All");
            cbType.Items.Add("All");
            using (var context = new Session1Entities())
            {

                var getTypes = (from x in context.Resource_Type
                                select x.resTypeName).Distinct().ToArray();
                cbType.Items.AddRange(getTypes);


                var getSkills = (from x in context.Skills
                                 select x.skillName).Distinct().ToArray();
                cbSkill.Items.AddRange(getSkills);

            }
            LoadDGV();
        }

        private void LoadDGV()
        {
            using (var context = new Session1Entities())
            {
                var getResources = (from x in context.Resources
                                    orderby x.remainingQuantity descending
                                    select x);
                foreach (var Resources in getResources)
                {
                    var newRow = new List<string>()
                    {
                        Resources.resId.ToString(), Resources.resName, Resources.Resource_Type.resTypeName,
                        context.Resource_Allocation.Where(x => x.resIdFK == Resources.resId).Select(x => x).Count().ToString()
                    };
                    var getAllocation = (from x in context.Resource_Allocation
                                         where x.resIdFK == Resources.resId
                                         select x);
                    string allocationString = "Nil";
                    foreach (var skill in getAllocation)
                    {
                        if (allocationString == "Nil")
                        {
                            allocationString = skill.Skill.skillName;
                        }
                        else
                        {
                            allocationString += $", {skill.Skill.skillName}";
                        }
                    }
                    newRow.Add(allocationString);

                    if (Resources.remainingQuantity > 5)
                    {
                        newRow.Add("Sufficient");
                    }
                    else if (Resources.remainingQuantity <= 5 && Resources.remainingQuantity >= 1)
                    {
                        newRow.Add("Low Stock");
                    }
                    else
                    {
                        newRow.Add("Not Available");
                    }
                    dataGridView1.Rows.Add(newRow.ToArray());
                }
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (dataGridView1.Rows[row.Index].Cells[5].Value.ToString() == "Not Available")
                {
                    dataGridView1.Rows[row.Index].DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbType.SelectedItem.ToString() != "All" && (cbSkill.SelectedItem == null || cbSkill.SelectedItem.ToString() == "All"))
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (dataGridView1.Rows[row.Index].Cells[2].Value.ToString() != cbType.SelectedItem.ToString())
                    {
                        dataGridView1.Rows.RemoveAt(row.Index);
                    }
                }
            }
            else if (cbType.SelectedItem.ToString() != "All" && (cbSkill.SelectedItem == null || cbSkill.SelectedItem.ToString() != "All"))
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (dataGridView1.Rows[row.Index].Cells[2].Value.ToString() != cbType.SelectedItem.ToString() || !dataGridView1.Rows[row.Index].Cells[4].Value.ToString().Contains(cbType.SelectedItem.ToString()))
                    {
                        dataGridView1.Rows.RemoveAt(row.Index);
                    }
                }
            }
            else if (cbType.SelectedItem.ToString() == "All" && (cbSkill.SelectedItem == null || cbSkill.SelectedItem.ToString() != "All"))
            {
                dataGridView1.Rows.Clear();
                LoadDGV();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!dataGridView1.Rows[row.Index].Cells[4].Value.ToString().Contains(cbType.SelectedItem.ToString()))
                    {
                        dataGridView1.Rows.RemoveAt(row.Index);
                    }
                }
            }
            else
            {
                dataGridView1.Rows.Clear();
                LoadDGV();
            }
        }

        private void cbSkill_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSkill.SelectedItem.ToString() != "All" && (cbType.SelectedItem == null || cbType.SelectedItem.ToString() == "All"))
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!dataGridView1.Rows[row.Index].Cells[4].Value.ToString().Contains(cbType.SelectedItem.ToString()))
                    {
                        dataGridView1.Rows.RemoveAt(row.Index);
                    }
                }
            }
            else if (cbSkill.SelectedItem.ToString() != "All" && (cbType.SelectedItem == null || cbType.SelectedItem.ToString() != "All"))
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (dataGridView1.Rows[row.Index].Cells[2].Value.ToString() != cbType.SelectedItem.ToString() || !dataGridView1.Rows[row.Index].Cells[4].Value.ToString().Contains(cbType.SelectedItem.ToString()))
                    {
                        dataGridView1.Rows.RemoveAt(row.Index);
                    }
                }
            }
            else if (cbSkill.SelectedItem.ToString() == "All" && (cbType.SelectedItem == null || cbType.SelectedItem.ToString() != "All"))
            {
                dataGridView1.Rows.Clear();
                LoadDGV();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (dataGridView1.Rows[row.Index].Cells[2].Value.ToString() != cbType.SelectedItem.ToString())
                    {
                        dataGridView1.Rows.RemoveAt(row.Index);
                    }
                }
            }
            else
            {
                dataGridView1.Rows.Clear();
                LoadDGV();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a resource to delete!", "Delete Resource", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var response = MessageBox.Show("Are you sure you want to delete this resource?", "Delete Resource", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (response == DialogResult.Yes)
                {
                    var resourceID = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                    using (var context = new Session1Entities())
                    {
                        var allocation = (from x in context.Resource_Allocation
                                          where x.resIdFK == resourceID
                                          select x);
                        foreach (var item in allocation)
                        {
                            context.Resource_Allocation.Remove(item);
                        }
                        context.SaveChanges();

                        var resourceDelete = (from x in context.Resources
                                              where x.resId == resourceID
                                              select x).FirstOrDefault();
                        context.Resources.Remove(resourceDelete);
                        context.SaveChanges();
                    }
                    MessageBox.Show("Resource deleted successfully!", "Delete Resource", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.Hide();
            (new AddNewResource()).ShowDialog();
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a resource to edit!", "Update Resource", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var getResourceID = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                this.Hide();
                (new UpdateResource(getResourceID)).ShowDialog();
                this.Close();
            }
            
        }
    }
}
