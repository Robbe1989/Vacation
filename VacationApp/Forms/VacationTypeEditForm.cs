using System;
using System.Drawing;
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
            panelColor.BackColor = VacationType.GetColor();
        }

        public VacationTypeEditForm(VacationType vt)
        {
            InitializeComponent();
            VacationType = new VacationType 
            { 
                Id = vt.Id, 
                Abbreviation = vt.Abbreviation, 
                Name = vt.Name,
                ColorHex = vt.ColorHex
            };
            txtAbbreviation.Text = vt.Abbreviation;
            txtName.Text = vt.Name;
            panelColor.BackColor = VacationType.GetColor();
        }

        private void btnColorPicker_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog())
            {
                dlg.Color = panelColor.BackColor;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    panelColor.BackColor = dlg.Color;
                    VacationType.ColorHex = ColorTranslator.ToHtml(dlg.Color);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAbbreviation.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Bitte füllen Sie alle Felder aus.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            VacationType.Abbreviation = txtAbbreviation.Text.Trim();
            VacationType.Name = txtName.Text.Trim();
            VacationType.ColorHex = ColorTranslator.ToHtml(panelColor.BackColor);
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