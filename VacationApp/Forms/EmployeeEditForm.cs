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

    // load departments...
    // Load default/initial FTE options...
    // Event hookup
    cmbDepartment.SelectedIndexChanged += CmbDepartment_SelectedIndexChanged;

    if (e == null)
    {
        Employee = new Employee();
        // default: UseFte true (oder wie gewünscht)
        chkUseFte.Checked = Employee.UseFte;
    }
    else
    {
        Employee = e;
        txtName.Text = Employee.Name;
        txtEmail.Text = Employee.Email;

        // select department if present
        if (!string.IsNullOrEmpty(Employee.Department) && cmbDepartment.Items.Contains(Employee.Department))
            cmbDepartment.SelectedItem = Employee.Department;

        // UseFte load
        chkUseFte.Checked = Employee.UseFte;

        // load FTE options for department and select matching value
        LoadFteOptions(Employee.Department);
        SelectFteByValue(Employee.Fte);
    }

    // ensure cmbFte enabled state matches checkbox
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
private void chkUseFte_CheckedChanged(object sender, EventArgs e)
{
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
        Employee.Fte = 1.0; // normalize / fallback
    }

    DialogResult = DialogResult.OK;
    Close();
}
private void chkUseFte_CheckedChanged(object sender, EventArgs e)
{
    // Aktiviert/Deaktiviert das FTE‑Dropdown entsprechend der Checkbox.
    // Wenn deaktiviert, wird das Dropdown ausgegraut und optional auf "Vollzeit (100%)" gesetzt.
    try
    {
        var use = chkUseFte.Checked;
        cmbFte.Enabled = use;

        if (!use)
        {
            // optional: setze die Anzeige auf Vollzeit, damit beim Speichern ein gültiger Wert vorhanden ist
            if (cmbFte.Items.Count > 0)
            {
                // versuche ein Label für Vollzeit zu finden, sonst erstes Item
                int idx = -1;
                for (int i = 0; i < cmbFte.Items.Count; i++)
                {
                    var s = cmbFte.Items[i]?.ToString() ?? "";
                    if (s.Contains("Vollzeit") || s.Contains("100%"))
                    {
                        idx = i;
                        break;
                    }
                }
                cmbFte.SelectedIndex = idx >= 0 ? idx : 0;
            }
        }
    }
    catch
    {
        // Falls Controls zur Laufzeit (z. B. im Designer) noch null sind, einfach ignorieren
    }
}
    }
}