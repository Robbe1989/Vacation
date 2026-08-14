// Datei: VacationApp/Forms/EmployeeEditForm.cs
using System;
using System.Globalization;
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

            // Load departments into cmbDepartment
            var depts = Database.GetAllDepartments();
            cmbDepartment.Items.Clear();
            foreach (var d in depts)
                cmbDepartment.Items.Add(d.Name);

            // if there are departments, select first by default
            if (cmbDepartment.Items.Count > 0)
                cmbDepartment.SelectedIndex = 0;

            // load default FTE options (will be replaced when department selected)
            LoadFteOptions(null);

            if (e == null)
            {
                Employee = new Employee();
                Employee.StartDate = DateTime.Today; // stored but not editable
            }
            else
            {
                Employee = e;
                txtName.Text = Employee.Name;
                txtEmail.Text = Employee.Email;

                if (!string.IsNullOrEmpty(Employee.Department) && cmbDepartment.Items.Contains(Employee.Department))
                    cmbDepartment.SelectedItem = Employee.Department;

                // try to select FTE matching value
                SelectFteByValue(Employee.Fte);
            }

            cmbDepartment.SelectedIndexChanged += CmbDepartment_SelectedIndexChanged;
        }

        private void CmbDepartment_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var sel = cmbDepartment.SelectedItem?.ToString();
            LoadFteOptions(sel);
        }

        private void LoadFteOptions(string? departmentName)
        {
            cmbFte.Items.Clear();

            Department? dept = null;
            if (!string.IsNullOrEmpty(departmentName))
            {
                dept = Database.GetAllDepartments().Find(x => string.Equals(x.Name, departmentName, StringComparison.OrdinalIgnoreCase));
            }

            if (dept != null)
            {
                if (!dept.UseFte)
                {
                    cmbFte.Items.Add("Vollzeit (100%)");
                    cmbFte.Enabled = false;
                    cmbFte.SelectedIndex = 0;
                    return;
                }

                foreach (var kv in dept.GetFteOptions())
                    cmbFte.Items.Add(kv.Label);

                cmbFte.Enabled = true;
                cmbFte.SelectedIndex = 0;
            }
            else
            {
                // defaults
                cmbFte.Items.Add("Vollzeit (100%)");
                cmbFte.Items.Add("Halbtags (50%)");
                cmbFte.Items.Add("Teilzeit 80% (80%)");
                cmbFte.Enabled = true;
                cmbFte.SelectedIndex = 0;
            }
        }

        private void SelectFteByValue(double value)
        {
            // try to find label with same value in current cmbFte items via departments
            foreach (var item in cmbFte.Items)
            {
                var label = item?.ToString();
                if (label != null && TryParseFteLabel(label, out var v) && Math.Abs(v - value) < 0.0001)
                {
                    cmbFte.SelectedItem = label;
                    return;
                }
            }
            // fallback leave first item
            if (cmbFte.Items.Count > 0)
                cmbFte.SelectedIndex = 0;
        }

        private bool TryParseFteLabel(string label, out double value)
        {
            // resolve from departments
            foreach (var d in Database.GetAllDepartments())
            {
                foreach (var kv in d.GetFteOptions())
                {
                    if (kv.Label == label)
                    {
                        value = kv.Value;
                        return true;
                    }
                }
            }
            value = 1.0;
            return false;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Employee.Name = txtName.Text.Trim();
            Employee.Email = txtEmail.Text.Trim();
            Employee.Department = cmbDepartment.SelectedItem?.ToString() ?? "";

            var sel = cmbFte.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(sel) && TryParseFteLabel(sel, out var fv))
                Employee.Fte = fv;
            else
                Employee.Fte = 1.0;

            DialogResult = DialogResult.OK;
            Close();
        }

        // Wichtig: diese Methode wurde in Designer als EventHandler eingetragen
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}