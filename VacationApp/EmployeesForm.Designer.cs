namespace VacationApp.Forms
{
    partial class EmployeesForm
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.dgvEmployees = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDepartment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVacationDays = new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();

            // dgvEmployees - dock fill so it resizes with the form
            this.dgvEmployees.AllowUserToAddRows = false;
            this.dgvEmployees.AllowUserToDeleteRows = false;
            this.dgvEmployees.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEmployees.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId, this.colName, this.colEmail, this.colDepartment, this.colVacationDays});
            this.dgvEmployees.Location = new System.Drawing.Point(12, 12);
            this.dgvEmployees.MultiSelect = false;
            this.dgvEmployees.Name = "dgvEmployees";
            this.dgvEmployees.ReadOnly = true;
            this.dgvEmployees.RowHeadersVisible = false;
            this.dgvEmployees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployees.Size = new System.Drawing.Size(760, 380);
            this.dgvEmployees.Dock = System.Windows.Forms.DockStyle.Fill;

            // Columns
            this.colId.HeaderText = "Id"; this.colId.Name = "colId"; this.colId.Visible = false;
            this.colName.HeaderText = "Name"; this.colName.Name = "colName"; this.colName.Width = 200;
            this.colEmail.HeaderText = "E-Mail"; this.colEmail.Name = "colEmail"; this.colEmail.Width = 180;
            this.colDepartment.HeaderText = "Abteilung"; this.colDepartment.Name = "colDepartment"; this.colDepartment.Width = 160;
            this.colVacationDays.HeaderText = "Urlaubstage"; this.colVacationDays.Name = "colVacationDays"; this.colVacationDays.Width = 80;

            // panelButtons at bottom - dock bottom so it stays at bottom
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Height = 48;
            this.panelButtons.Padding = new System.Windows.Forms.Padding(8);
            this.panelButtons.BackColor = System.Drawing.SystemColors.Control;

            // Buttons (anchored left inside the panel)
            this.btnAdd.Text = "Hinzufügen"; this.btnAdd.Size = new System.Drawing.Size(90, 28); this.btnAdd.Location = new System.Drawing.Point(8, 8); this.btnAdd.Name = "btnAdd"; this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnEdit.Text = "Bearbeiten"; this.btnEdit.Size = new System.Drawing.Size(90, 28); this.btnEdit.Location = new System.Drawing.Point(106, 8); this.btnEdit.Name = "btnEdit"; this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            this.btnDelete.Text = "Löschen"; this.btnDelete.Size = new System.Drawing.Size(90, 28); this.btnDelete.Location = new System.Drawing.Point(204, 8); this.btnDelete.Name = "btnDelete"; this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnRefresh.Text = "Aktualisieren"; this.btnRefresh.Size = new System.Drawing.Size(100, 28); this.btnRefresh.Location = new System.Drawing.Point(302, 8); this.btnRefresh.Name = "btnRefresh"; this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            this.panelButtons.Controls.Add(this.btnAdd);
            this.panelButtons.Controls.Add(this.btnEdit);
            this.panelButtons.Controls.Add(this.btnDelete);
            this.panelButtons.Controls.Add(this.btnRefresh);

            // Form - Controls.Add order: dgv fills client area, panelButtons docked bottom
            this.ClientSize = new System.Drawing.Size(784, 445);
            this.Controls.Add(this.dgvEmployees);
            this.Controls.Add(this.panelButtons);
            this.Name = "EmployeesForm";
            this.Text = "Mitarbeiter";

            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvEmployees;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDepartment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVacationDays;

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnAdd, btnEdit, btnDelete, btnRefresh;
    }
}