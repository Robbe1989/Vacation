namespace VacationApp.Forms
{
    partial class EmployeesForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) { components.Dispose(); } base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.dgvEmployees = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDepartment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            this.SuspendLayout();
            // dgvEmployees
            this.dgvEmployees.AllowUserToAddRows = false;
            this.dgvEmployees.AllowUserToDeleteRows = false;
            this.dgvEmployees.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEmployees.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId, this.colName, this.colEmail, this.colDepartment, this.colFte});
            this.dgvEmployees.Location = new System.Drawing.Point(12, 12);
            this.dgvEmployees.MultiSelect = false;
            this.dgvEmployees.Name = "dgvEmployees";
            this.dgvEmployees.ReadOnly = true;
            this.dgvEmployees.RowHeadersVisible = false;
            this.dgvEmployees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployees.Size = new System.Drawing.Size(760, 380);
            // Columns
            this.colId.HeaderText = "Id"; this.colId.Name = "colId"; this.colId.Visible = false;
            this.colName.HeaderText = "Name"; this.colName.Name = "colName"; this.colName.Width = 220;
            this.colEmail.HeaderText = "E-Mail"; this.colEmail.Name = "colEmail"; this.colEmail.Width = 180;
            this.colDepartment.HeaderText = "Abteilung"; this.colDepartment.Name = "colDepartment"; this.colDepartment.Width = 120;
            this.colFte.HeaderText = "FTE"; this.colFte.Name = "colFte"; this.colFte.Width = 60;
            // Buttons
            this.btnAdd.Location = new System.Drawing.Point(12, 405); this.btnAdd.Name = "btnAdd"; this.btnAdd.Size = new System.Drawing.Size(90, 28); this.btnAdd.Text = "Hinzufügen"; this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnEdit.Location = new System.Drawing.Point(108, 405); this.btnEdit.Name = "btnEdit"; this.btnEdit.Size = new System.Drawing.Size(90, 28); this.btnEdit.Text = "Bearbeiten"; this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            this.btnDelete.Location = new System.Drawing.Point(204, 405); this.btnDelete.Name = "btnDelete"; this.btnDelete.Size = new System.Drawing.Size(90, 28); this.btnDelete.Text = "Löschen"; this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnRefresh.Location = new System.Drawing.Point(300, 405); this.btnRefresh.Name = "btnRefresh"; this.btnRefresh.Size = new System.Drawing.Size(90, 28); this.btnRefresh.Text = "Aktualisieren"; this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // Form
            this.ClientSize = new System.Drawing.Size(784, 445);
            this.Controls.Add(this.dgvEmployees); this.Controls.Add(this.btnAdd); this.Controls.Add(this.btnEdit); this.Controls.Add(this.btnDelete); this.Controls.Add(this.btnRefresh);
            this.Name = "EmployeesForm"; this.Text = "Mitarbeiter";
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvEmployees;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDepartment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFte;
        private System.Windows.Forms.Button btnAdd, btnEdit, btnDelete, btnRefresh;
    }
}