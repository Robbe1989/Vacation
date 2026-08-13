using System;
using System.Windows.Forms;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class EmployeeEditForm : Form
    {
        public Employee Employee { get; private set; }

        public EmployeeEditForm(Employee? e = null)
        {
            InitializeComponent();
            if (e == null)
            {
                Employee = new Employee();
            }
            else
            {
                Employee = e;
                txtName.Text = Employee.Name;
                txtEmail.Text = Employee.Email;
                txtDepartment.Text = Employee.Department;
                dtpStartDate.Value = Employee.StartDate;
                numFte.Value = (decimal)Employee.Fte;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Employee.Name = txtName.Text.Trim();
            Employee.Email = txtEmail.Text.Trim();
            Employee.Department = txtDepartment.Text.Trim();
            Employee.StartDate = dtpStartDate.Value.Date;
            Employee.Fte = (double)numFte.Value;
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
