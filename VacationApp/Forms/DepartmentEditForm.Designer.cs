namespace VacationApp.Forms
{
    partial class DepartmentEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private void InitializeComponent()
        {
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // txtName
            this.txtName.Location = new System.Drawing.Point(110,12); this.txtName.Name = "txtName"; this.txtName.Size = new System.Drawing.Size(300,23);
            // label
            this.label1.AutoSize = true; this.label1.Location = new System.Drawing.Point(12,15); this.label1.Text = "Name:";
            // buttons
            this.btnOk.Location = new System.Drawing.Point(254,60); this.btnOk.Name = "btnOk"; this.btnOk.Size = new System.Drawing.Size(75,25); this.btnOk.Text = "OK"; this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            this.btnCancel.Location = new System.Drawing.Point(334,60); this.btnCancel.Name = "btnCancel"; this.btnCancel.Size = new System.Drawing.Size(75,25); this.btnCancel.Text = "Abbrechen"; this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // form
            this.ClientSize = new System.Drawing.Size(424,100);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Name = "DepartmentEditForm"; this.Text = "Abteilung bearbeiten";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOk, btnCancel;
    }
}