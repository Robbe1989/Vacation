// name=VacationApp/Forms/VacationEditForm.cs
using System;
using System.Linq;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class VacationEditForm : Form
    {
        public Vacation Vacation { get; private set; }
        private readonly int _year;

        public VacationEditForm(Vacation v = null, int year = 0)
        {
            InitializeComponent();
            _year = year == 0 ? DateTime.Now.Year : year;

            // load employees into combo
            var employees = Database.GetAllEmployees();
            cmbEmployee.Items.Clear();
            foreach (var e in employees)
                cmbEmployee.Items.Add(new ComboboxItem { Text = e.Name, Value = e.Id });

            if (v == null)
            {
                Vacation = new Vacation { StartDate = new DateTime(_year, 1, 1), EndDate = new DateTime(_year, 1, 1) };
                if (cmbEmployee.Items.Count > 0) cmbEmployee.SelectedIndex = 0;
            }
            else
            {
                Vacation = v;
                var item = cmbEmployee.Items.OfType<ComboboxItem>().FirstOrDefault(x => (int)x.Value == v.EmployeeId);
                if (item != null) cmbEmployee.SelectedItem = item;
                dtpStart.Value = Vacation.StartDate;
                dtpEnd.Value = Vacation.EndDate;
                txtComment.Text = Vacation.Comment;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!(cmbEmployee.SelectedItem is ComboboxItem ci))
            {
                MessageBox.Show("Bitte einen Mitarbeiter wählen.");
                return;
            }

            var s = dtpStart.Value.Date;
            var en = dtpEnd.Value.Date;
            if (en < s)
            {
                MessageBox.Show("Ende darf nicht vor dem Start liegen.");
                return;
            }

            Vacation.EmployeeId = (int)ci.Value;
            Vacation.StartDate = s;
            Vacation.EndDate = en;
            Vacation.Comment = txtComment.Text ?? "";

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        // small helper for combo items
        private class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() => Text;
        }
    }
}