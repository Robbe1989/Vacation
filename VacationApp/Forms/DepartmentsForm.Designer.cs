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

            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartments)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();

            // dgvDepartments - dock fill
            this.dgvDepartments.AllowUserToAddRows = false;
            this.dgvDepartments.AllowUserToDeleteRows = false;
            this.dgvDepartments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDepartments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colDeptId, this.colDeptName});
            this.dgvDepartments.Location = new System.Drawing.Point(12,12);
            this.dgvDepartments.MultiSelect = false;
            this.dgvDepartments.Name = "dgvDepartments";
            this.dgvDepartments.ReadOnly = true;
            this.dgvDepartments.RowHeadersVisible = false;
            this.dgvDepartments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDepartments.Size = new System.Drawing.Size(560,320);
            this.dgvDepartments.Dock = System.Windows.Forms.DockStyle.Fill;

            // columns
            this.colDeptId.HeaderText = "Id"; this.colDeptId.Name = "colDeptId"; this.colDeptId.Visible = false;
            this.colDeptName.HeaderText = "Name"; this.colDeptName.Name = "colDeptName"; this.colDeptName.Width = 500;

            // buttons panel
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Height = 48;
            this.panelButtons.Padding = new System.Windows.Forms.Padding(8);

            this.btnAdd.Text = "Hinzufügen"; this.btnAdd.Size = new System.Drawing.Size(90, 28); this.btnAdd.Location = new System.Drawing.Point(8, 8); this.btnAdd.Name = "btnAdd"; this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnEdit.Text = "Bearbeiten"; this.btnEdit.Size = new System.Drawing.Size(90, 28); this.btnEdit.Location = new System.Drawing.Point(106, 8); this.btnEdit.Name = "btnEdit"; this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            this.btnDelete.Text = "Löschen"; this.btnDelete.Size = new System.Drawing.Size(90, 28); this.btnDelete.Location = new System.Drawing.Point(204, 8); this.btnDelete.Name = "btnDelete"; this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.panelButtons.Controls.Add(this.btnAdd);
            this.panelButtons.Controls.Add(this.btnEdit);
            this.panelButtons.Controls.Add(this.btnDelete);

            // Form
            this.ClientSize = new System.Drawing.Size(584,380);
            this.Controls.Add(this.dgvDepartments);
            this.Controls.Add(this.panelButtons);
            this.Name = "DepartmentsForm";
            this.Text = "Abteilungen";

            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartments)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvDepartments;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeptId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeptName;

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnAdd, btnEdit, btnDelete;
    }
}