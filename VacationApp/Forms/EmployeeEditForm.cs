using System;
using System.Globalization;
using System.Linq;
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

            // Load departments into a ComboBox for department selection (ensure you have cmbDepartment in designer)
            var depts = Database.GetAllDepartments();
            cmbDepartment.Items.Clear();
            foreach (var d in depts)
                cmbDepartment.Items.Add(d.Name);

            // if employees use department values, hook up event
            cmbDepartment.SelectedIndexChanged += CmbDepartment_SelectedIndexChanged;

            // default FTE options fallback
            LoadFteOptions(null);

            if (e == null)
            {
                Employee = new Employee();
                Employee.StartDate = DateTime.Today; // still in DB but not editable
                cmbFte.SelectedIndex = 0;
            }
            else
            {
                Employee = e;
                txtName.Text = Employee.Name;
                txtEmail.Text = Employee.Email;
                txtDepartment.Text = Employee.Department;
                // select department dropdown if present
                if (!string.IsNullOrEmpty(Employee.Department) && cmbDepartment.Items.Contains(Employee.Department))
                    cmbDepartment.SelectedItem = Employee.Department;
                else if (cmbDepartment.Items.Count > 0)
                    cmbDepartment.SelectedIndex = 0;

                // select FTE by numeric match
                var found = false;
                for (int i = 0; i < cmbFte.Items.Count; i++)
                {
                    var item = cmbFte.Items[i].ToString();
                    if (item != null)
                    {
                        // map label to value if custom map exists
                        if (TryParseFteLabel(item, out double v) && Math.Abs(v - Employee.Fte) < 0.0001)
                        {
                            cmbFte.SelectedIndex = i;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found && cmbFte.Items.Count > 0)
                    cmbFte.SelectedIndex = 0;
            }
        }

        // Helper: load FTE options for departmentName (null = defaults)
        private void LoadFteOptions(string? departmentName)
        {
            cmbFte.Items.Clear();

            Department? dept = null;
            if (!string.IsNullOrEmpty(departmentName))
            {
                var list = Database.GetAllDepartments();
                dept = list.Find(x => string.Equals(x.Name, departmentName, StringComparison.OrdinalIgnoreCase));
            }

            if (dept != null)
            {
                if (!dept.UseFte)
                {
                    cmbFte.Items.Add("Vollzeit (100%)"); // only default
                    cmbFte.Enabled = false;
                    return;
                }
                foreach (var kv in dept.GetFteOptions())
                {
                    cmbFte.Items.Add($"{kv.Label}"); // label only; parse later using dept mapping stored in DB
                }
                cmbFte.Enabled = true;
            }
            else
            {
                // fallback defaults
                cmbFte.Items.Add("Vollzeit (100%)");
                cmbFte.Items.Add("Halbtags (50%)");
                cmbFte.Items.Add("Teilzeit 80% (80%)");
                cmbFte.Enabled = true;
            }
        }

        private bool TryParseFteLabel(string label, out double value)
        {
            // Try to resolve value: search departments for exact label
            var depts = Database.GetAllDepartments();
            foreach (var d in depts)
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
            // fallback parsing: try to extract numeric in parentheses or percent
            value = 1.0;
            return false;
        }

        private void CmbDepartment_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var sel = cmbDepartment.SelectedItem?.ToString();
            LoadFteOptions(sel);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Employee.Name = txtName.Text.Trim();
            Employee.Email = txtEmail.Text.Trim();
            Employee.Department = cmbDepartment.SelectedItem?.ToString() ?? txtDepartment.Text.Trim();

            // determine numeric fte from selected label
            var sel = cmbFte.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(sel) && TryParseFteLabel(sel, out var fv))
                Employee.Fte = fv;
            else
                Employee.Fte = 1.0;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}