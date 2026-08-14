namespace VacationApp.Forms
{
    partial class OptionsForm
    {
        private System.ComponentModel.IContainer components = null;
        private void InitializeComponent()
        {
            this.dgvMetrics = new System.Windows.Forms.DataGridView();
            this.colMetricKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMetricDisplay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMetricUse = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMetrics)).BeginInit();
            this.SuspendLayout();

            // dgvMetrics
            this.dgvMetrics.AllowUserToAddRows = false;
            this.dgvMetrics.AllowUserToDeleteRows = false;
            this.dgvMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMetrics.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colMetricKey, this.colMetricDisplay, this.colMetricUse});
            this.dgvMetrics.Location = new System.Drawing.Point(12,12);
            this.dgvMetrics.MultiSelect = false;
            this.dgvMetrics.Name = "dgvMetrics";
            this.dgvMetrics.RowHeadersVisible = false;
            this.dgvMetrics.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMetrics.Size = new System.Drawing.Size(460,260);

            // columns
            this.colMetricKey.HeaderText = "Key"; this.colMetricKey.Name = "colMetricKey"; this.colMetricKey.ReadOnly = true; this.colMetricKey.Width = 120;
            this.colMetricDisplay.HeaderText = "Bezeichnung"; this.colMetricDisplay.Name = "colMetricDisplay"; this.colMetricDisplay.Width = 240;
            this.colMetricUse.HeaderText = "Aktiv"; this.colMetricUse.Name = "colMetricUse"; this.colMetricUse.Width = 60;

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(397, 280);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75,28);
            this.btnSave.Text = "Speichern";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // Form
            this.ClientSize = new System.Drawing.Size(484,320);
            this.Controls.Add(this.dgvMetrics);
            this.Controls.Add(this.btnSave);
            this.Name = "OptionsForm";
            this.Text = "Optionen";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMetrics)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvMetrics;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMetricKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMetricDisplay;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colMetricUse;
        private System.Windows.Forms.Button btnSave;
    }
}