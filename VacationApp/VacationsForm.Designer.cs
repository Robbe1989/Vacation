// name=VacationApp/Forms/VacationsForm.Designer.cs
namespace VacationApp.Forms
{
    partial class VacationsForm
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.dgvVacations = new System.Windows.Forms.DataGridView();
            this.colVacId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVacEmployee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVacStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVacEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVacDays = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVacComment = new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvVacations)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();

            // dgvVacations
            this.dgvVacations.AllowUserToAddRows = false;
            this.dgvVacations.AllowUserToDeleteRows = false;
            this.dgvVacations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colVacId, this.colVacEmployee, this.colVacStart, this.colVacEnd, this.colVacDays, this.colVacComment});
            this.dgvVacations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvVacations.MultiSelect = false;
            this.dgvVacations.Name = "dgvVacations";
            this.dgvVacations.ReadOnly = true;
            this.dgvVacations.RowHeadersVisible = false;
            this.dgvVacations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            this.colVacId.HeaderText = "Id"; this.colVacId.Name = "colVacId"; this.colVacId.Visible = false;
            this.colVacEmployee.HeaderText = "Mitarbeiter"; this.colVacEmployee.Name = "colVacEmployee"; this.colVacEmployee.Width = 200;
            this.colVacStart.HeaderText = "Start"; this.colVacStart.Name = "colVacStart"; this.colVacStart.Width = 90;
            this.colVacEnd.HeaderText = "Ende"; this.colVacEnd.Name = "colVacEnd"; this.colVacEnd.Width = 90;
            this.colVacDays.HeaderText = "Tage"; this.colVacDays.Name = "colVacDays"; this.colVacDays.Width = 60;
            this.colVacComment.HeaderText = "Kommentar"; this.colVacComment.Name = "colVacComment"; this.colVacComment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;

            // panelButtons
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Height = 48;
            this.panelButtons.Padding = new System.Windows.Forms.Padding(8);

            this.btnAdd.Text = "Hinzufügen"; this.btnAdd.Size = new System.Drawing.Size(90, 28); this.btnAdd.Location = new System.Drawing.Point(8, 8); this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnEdit.Text = "Bearbeiten"; this.btnEdit.Size = new System.Drawing.Size(90, 28); this.btnEdit.Location = new System.Drawing.Point(106, 8); this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            this.btnDelete.Text = "Löschen"; this.btnDelete.Size = new System.Drawing.Size(90, 28); this.btnDelete.Location = new System.Drawing.Point(204, 8); this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnRefresh.Text = "Aktualisieren"; this.btnRefresh.Size = new System.Drawing.Size(100, 28); this.btnRefresh.Location = new System.Drawing.Point(302, 8); this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            this.panelButtons.Controls.Add(this.btnAdd);
            this.panelButtons.Controls.Add(this.btnEdit);
            this.panelButtons.Controls.Add(this.btnDelete);
            this.panelButtons.Controls.Add(this.btnRefresh);

            // Form
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvVacations);
            this.Controls.Add(this.panelButtons);
            this.Name = "VacationsForm";
            this.Text = "Urlaube verwalten";

            ((System.ComponentModel.ISupportInitialize)(this.dgvVacations)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvVacations;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVacId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVacEmployee;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVacStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVacEnd;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVacDays;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVacComment;

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnAdd, btnEdit, btnDelete, btnRefresh;
    }
}