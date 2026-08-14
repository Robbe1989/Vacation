namespace VacationApp.Forms
{
    partial class DepartmentsForm
    {
        private System.ComponentModel.IContainer components = null;
        private void InitializeComponent()
        {
            this.dgvDepartments = new System.Windows.Forms.DataGridView();
            this.colDeptId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDeptName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDeptUseFte = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartments)).BeginInit();
            this.SuspendLayout();
            // dgv
            this.dgvDepartments.AllowUserToAddRows = false;
            this.dgvDepartments.AllowUserToDeleteRows = false;
            this.dgvDepartments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDepartments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colDeptId, this.colDeptName, this.colDeptUseFte});
            this.dgvDepartments.Location = new System.Drawing.Point(12,12);
            this.dgvDepartments.Name = "dgvDepartments";
            this.dgvDepartments.ReadOnly = true;
            this.dgvDepartments.RowHeadersVisible = false;
            this.dgvDepartments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDepartments.Size = new System.Drawing.Size(560,320);
            // columns
            this.colDeptId.HeaderText = "Id"; this.colDeptId.Name = "colDeptId"; this.colDeptId.Visible = false;
            this.colDeptName.HeaderText = "Name"; this.colDeptName.Name = "colDeptName"; this.colDeptName.Width = 350;
            this.colDeptUseFte.HeaderText = "VZÄ aktiv"; this.colDeptUseFte.Name = "colDeptUseFte"; this.colDeptUseFte.Width = 80;
            // buttons
            this.btnAdd.Location = new System.Drawing.Point(12,340); this.btnAdd.Name = "btnAdd"; this.btnAdd.Size = new System.Drawing.Size(90,28); this.btnAdd.Text = "Hinzufügen"; this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnEdit.Location = new System.Drawing.Point(108,340); this.btnEdit.Name = "btnEdit"; this.btnEdit.Size = new System.Drawing.Size(90,28); this.btnEdit.Text = "Bearbeiten"; this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            this.btnDelete.Location = new System.Drawing.Point(204,340); this.btnDelete.Name = "btnDelete"; this.btnDelete.Size = new System.Drawing.Size(90,28); this.btnDelete.Text = "Löschen"; this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // form
            this.ClientSize = new System.Drawing.Size(584,380);
            this.Controls.Add(this.dgvDepartments); this.Controls.Add(this.btnAdd); this.Controls.Add(this.btnEdit); this.Controls.Add(this.btnDelete);
            this.Name = "DepartmentsForm"; this.Text = "Abteilungen";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartments)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvDepartments;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeptId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeptName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colDeptUseFte;
        private System.Windows.Forms.Button btnAdd, btnEdit, btnDelete;
    }
}