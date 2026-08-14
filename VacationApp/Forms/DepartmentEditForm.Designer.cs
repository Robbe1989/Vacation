namespace VacationApp.Forms
{
    partial class DepartmentEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private void InitializeComponent()
        {
            this.txtName = new System.Windows.Forms.TextBox();
            this.chkUseFte = new System.Windows.Forms.CheckBox();
            this.txtFteOptions = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // txtName
            this.txtName.Location = new System.Drawing.Point(110,12); this.txtName.Name = "txtName"; this.txtName.Size = new System.Drawing.Size(300,23);
            // chkUseFte
            this.chkUseFte.Location = new System.Drawing.Point(110,41); this.chkUseFte.Text = "FTE aktiv";
            // txtFteOptions
            this.txtFteOptions.Location = new System.Drawing.Point(110,70); this.txtFteOptions.Multiline = true; this.txtFteOptions.Size = new System.Drawing.Size(300,120);
            // labels
            this.label1.AutoSize = true; this.label1.Location = new System.Drawing.Point(12,15); this.label1.Text = "Name:";
            this.label2.AutoSize = true; this.label2.Location = new System.Drawing.Point(12,73); this.label2.Text = "FTE‑Optionen (Label=Value zeilenweise):";
            // buttons
            this.btnOk.Location = new System.Drawing.Point(254,200); this.btnOk.Text = "OK"; this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            this.btnCancel.Location = new System.Drawing.Point(334,200); this.btnCancel.Text = "Abbrechen"; this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // form
            this.ClientSize = new System.Drawing.Size(424,240);
            this.Controls.Add(this.txtName); this.Controls.Add(this.chkUseFte); this.Controls.Add(this.txtFteOptions);
            this.Controls.Add(this.label1); this.Controls.Add(this.label2); this.Controls.Add(this.btnOk); this.Controls.Add(this.btnCancel);
            this.Name = "DepartmentEditForm"; this.Text = "Abteilung bearbeiten";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.CheckBox chkUseFte;
        private System.Windows.Forms.TextBox txtFteOptions;
        private System.Windows.Forms.Label label1, label2;
        private System.Windows.Forms.Button btnOk, btnCancel;
    }
}