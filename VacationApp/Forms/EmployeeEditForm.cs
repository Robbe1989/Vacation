using System;
using System.Collections.Generic;
using System.Windows.Forms;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class EmployeeEditForm : Form
    {
        public Employee Employee { get; private set; }

        // Mapping Label -> numeric FTE
        private readonly Dictionary<string, double> _fteMap = new Dictionary<string, double>()
        {
            { "Vollzeit (100%)", 1.00 },
            { "90% (90%)", 0.90 },
            { "80% (80%)", 0.80 },
            { "75% (75%)", 0.75 },
            { "60% (60%)", 0.60 },
            { "Halbtags (50%)", 0.50 },
            { "Teilzeit 40% (40%)", 0.40 },
            { "20% (20%)", 0.20 }
        };

        public EmployeeEditForm(Employee? e = null)
        {
            InitializeComponent();

            // populate combo with labels
            foreach (var kv in _fteMap)
                cmbFte.Items.Add(kv.Key);

            if (e == null)
            {
                Employee = new Employee();
                Employee.StartDate = DateTime.Today; // Default, Eintritt wird nicht mehr erfasst
                cmbFte.SelectedItem = "Vollzeit (100%)";
            }
            else
            {
                Employee = e;
                txtName.Text = Employee.Name;
                txtEmail.Text = Employee.Email;
                txtDepartment.Text = Employee.Department;

                // set FTE selection by matching numeric value
                string match = null;
                foreach (var kv in _fteMap)
                {
                    if (Math.Abs(kv.Value - Employee.Fte) < 0.0001)
                    {
                        match = kv.Key;
                        break;
                    }
                }
                if (match != null && cmbFte.Items.Contains(match))
                    cmbFte.SelectedItem = match;
                else
                {
                    // if exact value not found, add a custom label and select it
                    var customLabel = $"{Employee.Fte:P0} ({Employee.Fte:0.00})";
                    if (!cmbFte.Items.Contains(customLabel))
                        cmbFte.Items.Insert(0, customLabel);
                    cmbFte.SelectedItem = customLabel;
                    // also add to map so OK can parse it
                    if (!_fteMap.ContainsKey(customLabel))
                        _fteMap[customLabel] = Employee.Fte;
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Employee.Name = txtName.Text.Trim();
            Employee.Email = txtEmail.Text.Trim();
            Employee.Department = txtDepartment.Text.Trim();
            // StartDate bleibt unverändert (oder Default bei neuem Mitarbeiter)

            var sel = cmbFte.SelectedItem?.ToString();
            if (sel != null && _fteMap.TryGetValue(sel, out var fteValue))
            {
                Employee.Fte = fteValue;
            }
            else
            {
                Employee.Fte = 1.0; // Fallback
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