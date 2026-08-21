using System;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class VacationTypesForm : Form
    {
        public VacationTypesForm()
        {
            InitializeComponent();
            Database.Init();
            LoadVacationTypes();
        }

        private void LoadVacationTypes()
        {
            var list = Database.GetAllVacationTypes();
            dgvVacationTypes.Rows.Clear();
            foreach (var vt in list)
            {
                var idx = dgvVacationTypes.Rows.Add(vt.Id, vt.Abbreviation, vt.Name);
                dgvVacationTypes.Rows[idx].Tag = vt;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var dlg = new VacationTypeEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Database.AddVacationType(dlg.VacationType);
                LoadVacationTypes();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvVacationTypes.SelectedRows.Count == 0) return;
            var row = dgvVacationTypes.SelectedRows[0];
            if (!(row.Tag is VacationType vt)) return;

            using var dlg = new VacationTypeEditForm(vt);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Database.UpdateVacationType(dlg.VacationType);
                LoadVacationTypes();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvVacationTypes.SelectedRows.Count == 0) return;
            var row = dgvVacationTypes.SelectedRows[0];
            if (!(row.Tag is VacationType vt)) return;

            if (MessageBox.Show("Urlaubstyp wirklich löschen?", "Löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Database.DeleteVacationType(vt.Id);
                LoadVacationTypes();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}