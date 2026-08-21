using System;
using System.Windows.Forms;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class VacationTypeEditForm : Form
    {
        public VacationType VacationType { get; private set; }

        public VacationTypeEditForm()
        {
            InitializeComponent();
            VacationType = new VacationType();
        }

        public VacationTypeEditForm(VacationType vt)
        {
            InitializeComponent();
            VacationType = new VacationType { Id = vt.Id, Abbreviation = vt.Abbreviation, Name = vt.Name };
            txtAbbreviation.Text = vt.Abbreviation;
            txtName.Text = vt.Name;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAbbreviation.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Bitte füllen Sie alle Felder aus.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            VacationType.Abbreviation = txtAbbreviation.Text;
            VacationType.Name = txtName.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}