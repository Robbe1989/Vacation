using System;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class EmployeesForm : Form
    {
        public EmployeesForm()
        {
            InitializeComponent();
            Database.Init();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            var list = Database.GetAllEmployees();
            dgvEmployees.Rows.Clear();
            foreach (var e in list)
            {
                var idx = dgvEmployees.Rows.Add(e.Id, e.Name, e.Email, e.Department, e.Fte.ToString("0.00"), e.UseFte);
                dgvEmployees.Rows[idx].Tag = e;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var dlg = new EmployeeEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Database.AddEmployee(dlg.Employee);
                LoadEmployees();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0) return;
            var row = dgvEmployees.SelectedRows[0];
            if (!(row.Tag is Employee emp)) return;

            using var dlg = new EmployeeEditForm(emp);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Database.UpdateEmployee(dlg.Employee);
                LoadEmployees();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0) return;
            var row = dgvEmployees.SelectedRows[0];
            if (!(row.Tag is Employee emp)) return;

            if (MessageBox.Show("Mitarbeiter wirklich löschen?", "Löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Database.DeleteEmployee(emp.Id);
                LoadEmployees();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadEmployees();
    }
}