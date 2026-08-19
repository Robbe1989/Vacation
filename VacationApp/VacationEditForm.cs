using System;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class VacationEditForm : Form
    {
        public Vacation Vacation { get; private set; }

        public VacationEditForm(Vacation? v = null)
        {
            InitializeComponent();

            Database.Init();

            // Load employees
            var emps = Database.GetAllEmployees();
            cmbEmployee.Items.Clear();
            foreach (var e in emps)
                cmbEmployee.Items.Add(e.Name);

            if (cmbEmployee.Items.Count > 0)
                cmbEmployee.SelectedIndex = 0;

            if (v == null)
            {
                Vacation = new Vacation();
                dtpStart.Value = DateTime.Today;
                dtpEnd.Value = DateTime.Today;
            }
            else
            {
                Vacation = v;
                var emp = Database.GetAllEmployees().Find(x => x.Id == v.EmployeeId);
                if (emp != null && cmbEmployee.Items.Contains(emp.Name))
                    cmbEmployee.SelectedItem = emp.Name;
                dtpStart.Value = v.StartDate;
                dtpEnd.Value = v.EndDate;
                txtComment.Text = v.Comment;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var emp = Database.GetAllEmployees().Find(x => x.Name == cmbEmployee.SelectedItem?.ToString());
            if (emp == null)
            {
                MessageBox.Show("Mitarbeiter auswählen!");
                return;
            }

            Vacation.EmployeeId = emp.Id;
            Vacation.StartDate = dtpStart.Value;
            Vacation.EndDate = dtpEnd.Value;
            Vacation.Comment = txtComment.Text.Trim();

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