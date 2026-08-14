// name=VacationApp/Forms/VacationEditForm.Designer.cs
namespace VacationApp.Forms
{
    partial class VacationEditForm
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.labelEmployee = new System.Windows.Forms.Label();
            this.cmbEmployee = new System.Windows.Forms.ComboBox();
            this.labelStart = new System.Windows.Forms.Label();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.labelEnd = new System.Windows.Forms.Label();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.labelComment = new System.Windows.Forms.Label();
            this.txtComment = new System.Windows.Forms.TextBox();

            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.panelButtons.SuspendLayout();
            this.SuspendLayout();

            // labels and controls
            this.labelEmployee.AutoSize = true; this.labelEmployee.Location = new System.Drawing.Point(12, 12); this.labelEmployee.Text = "Mitarbeiter:";
            this.cmbEmployee.Location = new System.Drawing.Point(110, 8); this.cmbEmployee.Size = new System.Drawing.Size(300, 23); this.cmbEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.labelStart.AutoSize = true; this.labelStart.Location = new System.Drawing.Point(12, 46); this.labelStart.Text = "Start:";
            this.dtpStart.Location = new System.Drawing.Point(110, 42); this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            this.labelEnd.AutoSize = true; this.labelEnd.Location = new System.Drawing.Point(12, 80); this.labelEnd.Text = "Ende:";
            this.dtpEnd.Location = new System.Drawing.Point(110, 76); this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            this.labelComment.AutoSize = true; this.labelComment.Location = new System.Drawing.Point(12, 114); this.labelComment.Text = "Kommentar:";
            this.txtComment.Location = new System.Drawing.Point(110, 110); this.txtComment.Size = new System.Drawing.Size(300, 23);

            // panelButtons
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Height = 48;
            this.panelButtons.Padding = new System.Windows.Forms.Padding(8);

            this.btnOk.Text = "OK"; this.btnOk.Size = new System.Drawing.Size(75, 28); this.btnOk.Location = new System.Drawing.Point(254, 8); this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            this.btnCancel.Text = "Abbrechen"; this.btnCancel.Size = new System.Drawing.Size(90, 28); this.btnCancel.Location = new System.Drawing.Point(334, 8); this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.panelButtons.Controls.Add(this.btnOk);
            this.panelButtons.Controls.Add(this.btnCancel);

            // Form
            this.ClientSize = new System.Drawing.Size(430, 170);
            this.Controls.Add(this.cmbEmployee);
            this.Controls.Add(this.labelEmployee);
            this.Controls.Add(this.dtpStart);
            this.Controls.Add(this.labelStart);
            this.Controls.Add(this.dtpEnd);
            this.Controls.Add(this.labelEnd);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.labelComment);
            this.Controls.Add(this.panelButtons);
            this.Name = "VacationEditForm";
            this.Text = "Urlaub";

            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label labelEmployee;
        private System.Windows.Forms.ComboBox cmbEmployee;
        private System.Windows.Forms.Label labelStart;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.Label labelEnd;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.Label labelComment;
        private System.Windows.Forms.TextBox txtComment;

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnOk, btnCancel;
    }
}