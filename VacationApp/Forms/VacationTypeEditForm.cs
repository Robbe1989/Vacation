using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class VacationTypeEditForm : Form
    {
        public VacationType VacationType { get; private set; }
        private bool isUpdatingControls = false; // Flag um rekursive Updates zu verhindern

        public VacationTypeEditForm()
        {
            InitializeComponent();
            VacationType = new VacationType();
            panelColor.BackColor = VacationType.GetColor();
            txtColorHex.Text = VacationType.ColorHex;
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
            txtColorHex.Text = vt.ColorHex;
            panelColor.BackColor = VacationType.GetColor();
        }

        private void btnColorPicker_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog())
            {
                dlg.Color = panelColor.BackColor;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    isUpdatingControls = true;
                    panelColor.BackColor = dlg.Color;
                    
                    // Konvertiere Color zu HEX - immer im Format #RRGGBB
                    string hexColor = ConvertColorToHex(dlg.Color);
                    
                    VacationType.ColorHex = hexColor;
                    txtColorHex.Text = hexColor;
                    isUpdatingControls = false;
                }
            }
        }

        /// <summary>
        /// Konvertiert ein Color-Objekt in einen HEX-String im Format #RRGGBB
        /// </summary>
        private string ConvertColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        private void txtColorHex_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingControls) return;

            string hexInput = txtColorHex.Text.Trim();

            // Validierung: Must be valid hex color (format: #RRGGBB or RRGGBB)
            if (!IsValidHexColor(hexInput))
            {
                // Ungültige Eingabe - Panel rot färben als visuelles Feedback
                panelColor.BackColor = Color.LightCoral;
                return;
            }

            try
            {
                // Sicherstellen, dass # am Anfang ist
                if (!hexInput.StartsWith("#"))
                    hexInput = "#" + hexInput;

                Color newColor = ColorTranslator.FromHtml(hexInput);
                isUpdatingControls = true;
                panelColor.BackColor = newColor;
                VacationType.ColorHex = hexInput;
                // Normalisiere HEX-Input (z.B. #abc -> #AABBCC, aber speichere groß)
                txtColorHex.Text = hexInput.ToUpper();
                isUpdatingControls = false;
            }
            catch
            {
                panelColor.BackColor = Color.LightCoral;
            }
        }

        private bool IsValidHexColor(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Erlaubt: #RRGGBB oder RRGGBB (mit oder ohne #)
            string pattern = @"^#?([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
            return Regex.IsMatch(input, pattern);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAbbreviation.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Bitte füllen Sie alle Felder aus.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string hexInput = txtColorHex.Text.Trim();
            if (!IsValidHexColor(hexInput))
            {
                MessageBox.Show("Ungültige HEX-Farbe. Format: #RRGGBB (z.B. #FF5733)", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sicherstellen, dass # am Anfang ist
            if (!hexInput.StartsWith("#"))
                hexInput = "#" + hexInput;

            VacationType.Abbreviation = txtAbbreviation.Text.Trim();
            VacationType.Name = txtName.Text.Trim();
            VacationType.ColorHex = hexInput.ToUpper();
            
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