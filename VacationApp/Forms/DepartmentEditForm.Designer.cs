namespace VacationApp.Forms
{
    partial class DepartmentEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private void InitializeComponent()
        {
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.panelButtons.SuspendLayout();
            this.SuspendLayout();

            // label + textbox
            this.label1.AutoSize = true; this.label1.Location = new System.Drawing.Point(12,15); this.label1.Text = "Name:";
            this.txtName.Location = new System.Drawing.Point(110,12); this.txtName.Name = "txtName"; this.txtName.Size = new System.Drawing.Size(300,23);
            this.txtName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // buttons panel
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Height = 48;
            this.panelButtons.Padding = new System.Windows.Forms.Padding(8);

            this.btnOk.Text = "OK"; this.btnOk.Size = new System.Drawing.Size(75, 28); this.btnOk.Location = new System.Drawing.Point(254, 8); this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            this.btnCancel.Text = "Abbrechen"; this.btnCancel.Size = new System.Drawing.Size(90, 28); this.btnCancel.Location = new System.Drawing.Point(334, 8); this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.panelButtons.Controls.Add(this.btnOk);
            this.panelButtons.Controls.Add(this.btnCancel);

            // Form
            this.ClientSize = new System.Drawing.Size(424,100);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelButtons);
            this.Name = "DepartmentEditForm"; this.Text = "Abteilung bearbeiten";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnOk, btnCancel;
    }
}