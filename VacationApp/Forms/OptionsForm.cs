using System;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
            LoadMetrics();
        }

        private void LoadMetrics()
        {
            dgvMetrics.Rows.Clear();
            var list = Database.GetAllMetrics();
            foreach (var m in list)
            {
                var idx = dgvMetrics.Rows.Add(m.Key, m.DisplayName, m.Use);
                dgvMetrics.Rows[idx].Tag = m;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvMetrics.Rows)
            {
                var key = row.Cells["colMetricKey"].Value?.ToString() ?? "";
                var display = row.Cells["colMetricDisplay"].Value?.ToString() ?? key;
                var useVal = row.Cells["colMetricUse"].Value;
                var use = useVal != null && Convert.ToBoolean(useVal);

                if (string.IsNullOrEmpty(key)) continue;

                var m = new Metric { Key = key, DisplayName = display, Use = use };
                Database.UpdateMetric(m);
            }

            MessageBox.Show("Änderungen gespeichert.", "Optionen", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}