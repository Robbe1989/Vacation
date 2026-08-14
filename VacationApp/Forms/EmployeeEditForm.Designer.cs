namespace VacationApp.Forms
{
    partial class EmployeeEditForm
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.cmbDepartment = new System.Windows.Forms.ComboBox();
            this.cmbFte = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label(); // Name
            this.label2 = new System.Windows.Forms.Label(); // E-Mail
            this.label3 = new System.Windows.Forms.Label(); // Abteilung
            this.label5 = new System.Windows.Forms.Label(); // Vollzeitäquivalent
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            // txtName
            this.txtName.Location = new System.Drawing.Point(110, 12);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(280, 23);

            // txtEmail
            this.txtEmail.Location = new System.Drawing.Point(110, 41);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(280, 23);

            // cmbDepartment (neu)
            this.cmbDepartment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepartment.FormattingEnabled = true;
            this.cmbDepartment.Location = new System.Drawing.Point(110, 70);
            this.cmbDepartment.Name = "cmbDepartment";
            this.cmbDepartment.Size = new System.Drawing.Size(220, 23);

            // cmbFte
            this.cmbFte.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFte.FormattingEnabled = true;
            this.cmbFte.Location = new System.Drawing.Point(110, 99);
            this.cmbFte.Name = "cmbFte";
            this.cmbFte.Size = new System.Drawing.Size(180, 23);

            // labels
            this.label1.AutoSize = true; this.label1.Location = new System.Drawing.Point(12, 15); this.label1.Text = "Name:";
            this.label2.AutoSize = true; this.label2.Location = new System.Drawing.Point(12, 44); this.label2.Text = "E-Mail:";
            this.label3.AutoSize = true; this.label3.Location = new System.Drawing.Point(12, 73); this.label3.Text = "Abteilung:";
            this.label5.AutoSize = true; this.label5.Location = new System.Drawing.Point(12, 102); this.label5.Text = "Vollzeitäquivalent:";

            // btnOk
            this.btnOk.Location = new System.Drawing.Point(234, 140);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 25);
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(315, 140);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.Text = "Abbrechen";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // Form
            this.ClientSize = new System.Drawing.Size(402, 180);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.cmbDepartment);
            this.Controls.Add(this.cmbFte);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EmployeeEditForm";
            this.Text = "Mitarbeiter bearbeiten";
        }

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.ComboBox cmbDepartment;
        private System.Windows.Forms.ComboBox cmbFte;
        private System.Windows.Forms.Label label1, label2, label3, label5;
        private System.Windows.Forms.Button btnOk, btnCancel;
    }
}