namespace VacationApp.Forms
{
    partial class VacationTypeEditForm
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.lblAbbreviation = new System.Windows.Forms.Label();
            this.txtAbbreviation = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();

            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // lblAbbreviation
            this.lblAbbreviation.AutoSize = true;
            this.lblAbbreviation.Location = new System.Drawing.Point(12, 15);
            this.lblAbbreviation.Text = "Abkürzung:";

            // txtAbbreviation
            this.txtAbbreviation.Location = new System.Drawing.Point(100, 12);
            this.txtAbbreviation.Size = new System.Drawing.Size(150, 20);
            this.txtAbbreviation.MaxLength = 5;
            this.txtAbbreviation.Name = "txtAbbreviation";

            // lblName
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 45);
            this.lblName.Text = "Bezeichnung:";

            // txtName
            this.txtName.Location = new System.Drawing.Point(100, 42);
            this.txtName.Size = new System.Drawing.Size(250, 20);
            this.txtName.Name = "txtName";

            // panelButtons
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Height = 48;
            this.panelButtons.Padding = new System.Windows.Forms.Padding(8);
            this.panelButtons.BackColor = System.Drawing.SystemColors.Control;

            // btnOK
            this.btnOK.Text = "OK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.Location = new System.Drawing.Point(8, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += this.btnOK_Click;

            // btnCancel
            this.btnCancel.Text = "Abbrechen";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.Location = new System.Drawing.Point(106, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Click += this.btnCancel_Click;

            this.panelButtons.Controls.Add(this.btnOK);
            this.panelButtons.Controls.Add(this.btnCancel);

            // VacationTypeEditForm
            this.ClientSize = new System.Drawing.Size(370, 130);
            this.Controls.Add(this.lblAbbreviation);
            this.Controls.Add(this.txtAbbreviation);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.panelButtons);
            this.Name = "VacationTypeEditForm";
            this.Text = "Urlaubstyp bearbeiten";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblAbbreviation;
        private System.Windows.Forms.TextBox txtAbbreviation;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnOK, btnCancel;
    }
}