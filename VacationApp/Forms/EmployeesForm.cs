using System;
using System.Collections.Generic;
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
                dgvEmployees.Rows.Add(e.Id, e.Name, e.Email, e.Department, e.StartDate.ToShortDateString(), e.Fte);
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
            var emp = new Employee
            {
                Id = Convert.ToInt32(row.Cells["colId"].Value),
                Name = Convert.ToString(row.Cells["colName"].Value) ?? "",
                Email = Convert.ToString(row.Cells["colEmail"].Value) ?? "",
                Department = Convert.ToString(row.Cells["colDepartment"].Value) ?? "",
                StartDate = DateTime.TryParse(Convert.ToString(row.Cells["colStartDate"].Value), out var dt) ? dt : DateTime.Today,
                Fte = Convert.ToDouble(row.Cells["colFte"].Value)
            };
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
            int id = Convert.ToInt32(row.Cells["colId"].Value);
            if (MessageBox.Show("Mitarbeiter wirklich löschen?", "Löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Database.DeleteEmployee(id);
                LoadEmployees();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadEmployees();
    }
}
