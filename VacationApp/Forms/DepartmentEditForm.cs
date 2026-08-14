using System;
using System.Windows.Forms;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class DepartmentEditForm : Form
    {
        public Department Department { get; private set; }

        public DepartmentEditForm(Department? d = null)
        {
            InitializeComponent();
            if (d == null)
            {
                Department = new Department();
            }
            else
            {
                Department = d;
                txtName.Text = d.Name;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Department.Name = txtName.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}