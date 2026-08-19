using System;
using System.Linq;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class VacationsForm : Form
    {
        private readonly int _year;
        public VacationsForm(int year)
        {
            InitializeComponent();
            _year = year;
            LoadVacations();
        }

        private void LoadVacations()
        {
            dgvVacations.Rows.Clear();
            var vacations = Database.GetVacationsForYear(_year);
            var employees = Database.GetAllEmployees().ToDictionary(x => x.Id, x => x.Name);
            foreach (var v in vacations)
            {
                var empName = employees.ContainsKey(v.EmployeeId) ? employees[v.EmployeeId] : v.EmployeeId.ToString();
                dgvVacations.Rows.Add(v.Id, empName, v.StartDate.ToString("yyyy-MM-dd"), v.EndDate.ToString("yyyy-MM-dd"), v.Days, v.Comment);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var dlg = new VacationEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Database.AddVacation(dlg.Vacation);
                LoadVacations();
            }
        }

        private Vacation? GetSelectedVacation()
        {
            if (dgvVacations.SelectedRows.Count == 0) return null;
            var row = dgvVacations.SelectedRows[0];
            if (row.Cells["colVacId"].Value == null) return null;
            int id = Convert.ToInt32(row.Cells["colVacId"].Value);
            var list = Database.GetVacationsForYear(_year);
            return list.FirstOrDefault(x => x.Id == id);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var sel = GetSelectedVacation();
            if (sel == null) return;
            using var dlg = new VacationEditForm(sel);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Database.UpdateVacation(dlg.Vacation);
                LoadVacations();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var sel = GetSelectedVacation();
            if (sel == null) return;
            if (MessageBox.Show("Urlaub wirklich löschen?", "Löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Database.DeleteVacation(sel.Id);
                LoadVacations();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadVacations();
    }
}