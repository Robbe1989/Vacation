using System;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class EmployeeEditForm : Form
    {
        public Employee Employee { get; private set; }

        public EmployeeEditForm(Employee? e = null)
        {
            InitializeComponent();

            Database.Init();

            // Load departments
            var depts = Database.GetAllDepartments();
            cmbDepartment.Items.Clear();
            foreach (var d in depts)
                cmbDepartment.Items.Add(d.Name);

            if (cmbDepartment.Items.Count > 0)
                cmbDepartment.SelectedIndex = 0;

            if (e == null)
            {
                Employee = new Employee();
                nudVacationDays.Value = Employee.VacationDays;
            }
            else
            {
                Employee = e;
                txtName.Text = Employee.Name;
                txtEmail.Text = Employee.Email;
                if (!string.IsNullOrEmpty(Employee.Department) && cmbDepartment.Items.Contains(Employee.Department))
                    cmbDepartment.SelectedItem = Employee.Department;
                nudVacationDays.Value = Math.Max(nudVacationDays.Minimum, Math.Min(nudVacationDays.Maximum, Employee.VacationDays));
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Employee.Name = txtName.Text.Trim();
            Employee.Email = txtEmail.Text.Trim();
            Employee.Department = cmbDepartment.SelectedItem?.ToString() ?? "";
            Employee.VacationDays = (int)nudVacationDays.Value;

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