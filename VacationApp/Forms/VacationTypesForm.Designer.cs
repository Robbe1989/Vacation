namespace VacationApp.Forms
{
    partial class VacationTypesForm
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.dgvVacationTypes = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAbbreviation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColor = new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvVacationTypes)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();

            // dgvVacationTypes
            this.dgvVacationTypes.AllowUserToAddRows = false;
            this.dgvVacationTypes.AllowUserToDeleteRows = false;
            this.dgvVacationTypes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId, this.colAbbreviation, this.colName, this.colColor});
            this.dgvVacationTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvVacationTypes.Location = new System.Drawing.Point(0, 0);
            this.dgvVacationTypes.MultiSelect = false;
            this.dgvVacationTypes.Name = "dgvVacationTypes";
            this.dgvVacationTypes.ReadOnly = true;
            this.dgvVacationTypes.RowHeadersVisible = false;
            this.dgvVacationTypes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVacationTypes.Size = new System.Drawing.Size(600, 350);

            // Columns
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.Visible = false;

            this.colAbbreviation.HeaderText = "Abkürzung";
            this.colAbbreviation.Name = "colAbbreviation";
            this.colAbbreviation.Width = 80;

            this.colName.HeaderText = "Bezeichnung";
            this.colName.Name = "colName";
            this.colName.Width = 200;

            this.colColor.HeaderText = "Farbe";
            this.colColor.Name = "colColor";
            this.colColor.Width = 80;

            // panelButtons
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Height = 48;
            this.panelButtons.Padding = new System.Windows.Forms.Padding(8);
            this.panelButtons.BackColor = System.Drawing.SystemColors.Control;

            // Buttons
            this.btnAdd.Text = "Hinzufügen";
            this.btnAdd.Size = new System.Drawing.Size(90, 28);
            this.btnAdd.Location = new System.Drawing.Point(8, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Click += this.btnAdd_Click;

            this.btnEdit.Text = "Bearbeiten";
            this.btnEdit.Size = new System.Drawing.Size(90, 28);
            this.btnEdit.Location = new System.Drawing.Point(106, 8);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Click += this.btnEdit_Click;

            this.btnDelete.Text = "Löschen";
            this.btnDelete.Size = new System.Drawing.Size(90, 28);
            this.btnDelete.Location = new System.Drawing.Point(204, 8);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Click += this.btnDelete_Click;

            this.btnClose.Text = "Schließen";
            this.btnClose.Size = new System.Drawing.Size(90, 28);
            this.btnClose.Location = new System.Drawing.Point(302, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Click += this.btnClose_Click;

            this.panelButtons.Controls.Add(this.btnAdd);
            this.panelButtons.Controls.Add(this.btnEdit);
            this.panelButtons.Controls.Add(this.btnDelete);
            this.panelButtons.Controls.Add(this.btnClose);

            // VacationTypesForm
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.dgvVacationTypes);
            this.Controls.Add(this.panelButtons);
            this.Name = "VacationTypesForm";
            this.Text = "Urlaubstypen";

            ((System.ComponentModel.ISupportInitialize)(this.dgvVacationTypes)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvVacationTypes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAbbreviation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColor;

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnAdd, btnEdit, btnDelete, btnClose;
    }
}