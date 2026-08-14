using System;
using System.Globalization;
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
            // FTE-Options (strings, wir parsen beim OK)
            cmbFte.Items.AddRange(new object[] { "1.00", "0.90", "0.80", "0.75", "0.60", "0.50", "0.40", "0.20" });

            if (e == null)
            {
                Employee = new Employee();
                cmbFte.SelectedItem = "1.00";
            }
            else
            {
                Employee = e;
                txtName.Text = Employee.Name;
                txtEmail.Text = Employee.Email;
                txtDepartment.Text = Employee.Department;
                dtpStartDate.Value = Employee.StartDate;
                var ftestr = Employee.Fte.ToString("0.00", CultureInfo.InvariantCulture);
                if (cmbFte.Items.Contains(ftestr))
                    cmbFte.SelectedItem = ftestr;
                else
                {
                    // falls ungewöhnlicher Wert, hängt ihn an und wählt ihn
                    cmbFte.Items.Insert(0, ftestr);
                    cmbFte.SelectedItem = ftestr;
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Employee.Name = txtName.Text.Trim();
            Employee.Email = txtEmail.Text.Trim();
            Employee.Department = txtDepartment.Text.Trim();
            Employee.StartDate = dtpStartDate.Value.Date;

            // FTE aus ComboBox parsen (InvariantCulture)
            var sel = cmbFte.SelectedItem?.ToString() ?? "1.00";
            if (!double.TryParse(sel, NumberStyles.Any, CultureInfo.InvariantCulture, out var fte))
            {
                // fallback
                fte = 1.0;
            }
            Employee.Fte = fte;

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