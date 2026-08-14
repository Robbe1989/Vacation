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

            // Ensure DB initialized (safe)
            Database.Init();

            // Load departments into cmbDepartment
            var depts = Database.GetAllDepartments();
            cmbDepartment.Items.Clear();
            foreach (var d in depts)
                cmbDepartment.Items.Add(d.Name);

            if (cmbDepartment.Items.Count > 0)
                cmbDepartment.SelectedIndex = 0;

            cmbDepartment.SelectedIndexChanged += CmbDepartment_SelectedIndexChanged;

            // load initial FTE options
            LoadFteOptions(cmbDepartment.SelectedItem?.ToString());

            if (e == null)
            {
                Employee = new Employee();
                chkUseFte.Checked = Employee.UseFte;
            }
            else
            {
                Employee = e;
                txtName.Text = Employee.Name;
                txtEmail.Text = Employee.Email;

                if (!string.IsNullOrEmpty(Employee.Department) && cmbDepartment.Items.Contains(Employee.Department))
                    cmbDepartment.SelectedItem = Employee.Department;

                chkUseFte.Checked = Employee.UseFte;
                LoadFteOptions(Employee.Department);
                SelectFteByValue(Employee.Fte);
            }

            // Respect global metric switch: if 'fte' metric disabled, hide VZÄ controls
            var globalUseFte = Database.GetMetricUse("fte");
            if (!globalUseFte)
            {
                chkUseFte.Visible = false;
                cmbFte.Visible = false;
                label5.Visible = false;
                // ensure Employee.UseFte is false so saving doesn't use VZÄ
                if (Employee != null) Employee.UseFte = false;
            }

            cmbFte.Enabled = chkUseFte.Checked;
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
                if (cmbFte.Items.Count > 0) cmbFte.SelectedIndex = 0;
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
            foreach (var item in cmbFte.Items)
            {
                var label = item?.ToString();
                if (label != null && TryParseFteLabel(label, out var v) && Math.Abs(v - value) < 0.0001)
                {
                    cmbFte.SelectedItem = label;
                    return;
                }
            }
            if (cmbFte.Items.Count > 0)
                cmbFte.SelectedIndex = 0;
        }

        private bool TryParseFteLabel(string label, out double value)
        {
            // search department options
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

            // fallback parsing: percent or decimal inside parentheses
            value = 1.0;
            var pIdx = label.IndexOf('%');
            if (pIdx > 0)
            {
                var start = label.LastIndexOf(' ', pIdx) + 1;
                var num = label.Substring(start, pIdx - start).Trim(' ', '(', ')');
                if (double.TryParse(num, NumberStyles.Any, CultureInfo.InvariantCulture, out var pct))
                {
                    value = pct / 100.0;
                    return true;
                }
            }

            var open = label.IndexOf('(');
            var close = label.IndexOf(')');
            if (open >= 0 && close > open)
            {
                var inner = label.Substring(open + 1, close - open - 1);
                if (double.TryParse(inner, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec))
                {
                    value = dec;
                    return true;
                }
            }

            return false;
        }

        private void chkUseFte_CheckedChanged(object sender, EventArgs e)
        {
            if (cmbFte != null && chkUseFte != null)
                cmbFte.Enabled = chkUseFte.Checked;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Employee.Name = txtName.Text.Trim();
            Employee.Email = txtEmail.Text.Trim();
            Employee.Department = cmbDepartment.SelectedItem?.ToString() ?? "";

            Employee.UseFte = chkUseFte.Checked;

            if (Employee.UseFte)
            {
                var sel = cmbFte.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(sel) && TryParseFteLabel(sel, out var fv))
                    Employee.Fte = fv;
                else
                    Employee.Fte = 1.0;
            }
            else
            {
                Employee.Fte = 1.0;
            }

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