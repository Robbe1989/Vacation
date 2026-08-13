namespace VacationApp.Forms
{
    partial class EmployeeEditForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) { components.Dispose(); } base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtDepartment = new System.Windows.Forms.TextBox();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.numFte = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numFte)).BeginInit();
            this.SuspendLayout();
            // txtName
            this.txtName.Location = new System.Drawing.Point(110, 12);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(280, 23);
            // txtEmail
            this.txtEmail.Location = new System.Drawing.Point(110, 41);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(280, 23);
            // txtDepartment
            this.txtDepartment.Location = new System.Drawing.Point(110, 70);
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.Size = new System.Drawing.Size(280, 23);
            // dtpStartDate
            this.dtpStartDate.Location = new System.Drawing.Point(110, 99);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(200, 23);
            // numFte
            this.numFte.DecimalPlaces = 2;
            this.numFte.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            this.numFte.Location = new System.Drawing.Point(110, 128);
            this.numFte.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            this.numFte.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            this.numFte.Name = "numFte";
            this.numFte.Size = new System.Drawing.Size(80, 23);
            this.numFte.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // labels
            this.label1.AutoSize = true; this.label1.Location = new System.Drawing.Point(12, 15); this.label1.Text = "Name:";
            this.label2.AutoSize = true; this.label2.Location = new System.Drawing.Point(12, 44); this.label2.Text = "E-Mail:";
            this.label3.AutoSize = true; this.label3.Location = new System.Drawing.Point(12, 73); this.label3.Text = "Abteilung:";
            this.label4.AutoSize = true; this.label4.Location = new System.Drawing.Point(12, 104); this.label4.Text = "Eintritt:";
            this.label5.AutoSize = true; this.label5.Location = new System.Drawing.Point(12, 130); this.label5.Text = "FTE:";
            // btnOk
            this.btnOk.Location = new System.Drawing.Point(234, 165); this.btnOk.Name = "btnOk"; this.btnOk.Size = new System.Drawing.Size(75, 25); this.btnOk.Text = "OK"; this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(315, 165); this.btnCancel.Name = "btnCancel"; this.btnCancel.Size = new System.Drawing.Size(75, 25); this.btnCancel.Text = "Abbrechen"; this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // Form
            this.ClientSize = new System.Drawing.Size(402, 202);
            this.Controls.Add(this.txtName); this.Controls.Add(this.txtEmail); this.Controls.Add(this.txtDepartment);
            this.Controls.Add(this.dtpStartDate); this.Controls.Add(this.numFte);
            this.Controls.Add(this.label1); this.Controls.Add(this.label2); this.Controls.Add(this.label3); this.Controls.Add(this.label4); this.Controls.Add(this.label5);
            this.Controls.Add(this.btnOk); this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EmployeeEditForm";
            this.Text = "Mitarbeiter bearbeiten";
            ((System.ComponentModel.ISupportInitialize)(this.numFte)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtDepartment;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.NumericUpDown numFte;
        private System.Windows.Forms.Label label1, label2, label3, label4, label5;
        private System.Windows.Forms.Button btnOk, btnCancel;
    }
}
