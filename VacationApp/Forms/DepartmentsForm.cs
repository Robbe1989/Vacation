using System;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class DepartmentsForm : Form
    {
        public DepartmentsForm()
        {
            InitializeComponent();
            LoadDepartments();
        }

        private void LoadDepartments()
        {
            var list = Database.GetAllDepartments();
            dgvDepartments.Rows.Clear();
            foreach (var d in list)
            {
                var idx = dgvDepartments.Rows.Add(d.Id, d.Name, d.UseFte);
                dgvDepartments.Rows[idx].Tag = d;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var dlg = new DepartmentEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Database.AddDepartment(dlg.Department);
                LoadDepartments();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvDepartments.SelectedRows.Count == 0) return;
            var d = dgvDepartments.SelectedRows[0].Tag as Department;
            if (d == null) return;
            using var dlg = new DepartmentEditForm(d);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Database.UpdateDepartment(dlg.Department);
                LoadDepartments();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDepartments.SelectedRows.Count == 0) return;
            var d = dgvDepartments.SelectedRows[0].Tag as Department;
            if (d == null) return;
            if (MessageBox.Show($"Abteilung '{d.Name}' löschen?", "Löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Database.DeleteDepartment(d.Id);
                LoadDepartments();
            }
        }
    }
}