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
            this.labelVacationType = new System.Windows.Forms.Label();
            this.cmbVacationType = new System.Windows.Forms.ComboBox();
            this.labelComment = new System.Windows.Forms.Label();
            this.txtComment = new System.Windows.Forms.TextBox();

            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.panelButtons.SuspendLayout();
            this.SuspendLayout();

            // labelEmployee
            this.labelEmployee.AutoSize = true;
            this.labelEmployee.Location = new System.Drawing.Point(12, 12);
            this.labelEmployee.Name = "labelEmployee";
            this.labelEmployee.Size = new System.Drawing.Size(85, 15);
            this.labelEmployee.TabIndex = 0;
            this.labelEmployee.Text = "Mitarbeiter:";

            // cmbEmployee
            this.cmbEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmployee.FormattingEnabled = true;
            this.cmbEmployee.Location = new System.Drawing.Point(110, 8);
            this.cmbEmployee.Name = "cmbEmployee";
            this.cmbEmployee.Size = new System.Drawing.Size(300, 23);
            this.cmbEmployee.TabIndex = 1;

            // labelStart
            this.labelStart.AutoSize = true;
            this.labelStart.Location = new System.Drawing.Point(12, 46);
            this.labelStart.Name = "labelStart";
            this.labelStart.Size = new System.Drawing.Size(40, 15);
            this.labelStart.TabIndex = 2;
            this.labelStart.Text = "Start:";

            // dtpStart
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStart.Location = new System.Drawing.Point(110, 42);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(300, 23);
            this.dtpStart.TabIndex = 3;

            // labelEnd
            this.labelEnd.AutoSize = true;
            this.labelEnd.Location = new System.Drawing.Point(12, 80);
            this.labelEnd.Name = "labelEnd";
            this.labelEnd.Size = new System.Drawing.Size(41, 15);
            this.labelEnd.TabIndex = 4;
            this.labelEnd.Text = "Ende:";

            // dtpEnd
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEnd.Location = new System.Drawing.Point(110, 76);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(300, 23);
            this.dtpEnd.TabIndex = 5;

            // labelVacationType
            this.labelVacationType.AutoSize = true;
            this.labelVacationType.Location = new System.Drawing.Point(12, 114);
            this.labelVacationType.Name = "labelVacationType";
            this.labelVacationType.Size = new System.Drawing.Size(81, 15);
            this.labelVacationType.TabIndex = 6;
            this.labelVacationType.Text = "Urlaubstyp:";

            // cmbVacationType
            this.cmbVacationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVacationType.FormattingEnabled = true;
            this.cmbVacationType.Location = new System.Drawing.Point(110, 110);
            this.cmbVacationType.Name = "cmbVacationType";
            this.cmbVacationType.Size = new System.Drawing.Size(300, 23);
            this.cmbVacationType.TabIndex = 7;

            // labelComment
            this.labelComment.AutoSize = true;
            this.labelComment.Location = new System.Drawing.Point(12, 148);
            this.labelComment.Name = "labelComment";
            this.labelComment.Size = new System.Drawing.Size(85, 15);
            this.labelComment.TabIndex = 8;
            this.labelComment.Text = "Kommentar:";

            // txtComment
            this.txtComment.Location = new System.Drawing.Point(110, 144);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(300, 60);
            this.txtComment.TabIndex = 9;

            // panelButtons
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnOk);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Height = 48;
            this.panelButtons.Location = new System.Drawing.Point(0, 212);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Padding = new System.Windows.Forms.Padding(8);
            this.panelButtons.Size = new System.Drawing.Size(430, 48);
            this.panelButtons.TabIndex = 10;

            // btnOk
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(254, 8);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);

            // btnCancel
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(334, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Abbrechen";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // VacationEditForm
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(430, 260);
            this.Controls.Add(this.labelEmployee);
            this.Controls.Add(this.cmbEmployee);
            this.Controls.Add(this.labelStart);
            this.Controls.Add(this.dtpStart);
            this.Controls.Add(this.labelEnd);
            this.Controls.Add(this.dtpEnd);
            this.Controls.Add(this.labelVacationType);
            this.Controls.Add(this.cmbVacationType);
            this.Controls.Add(this.labelComment);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.panelButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VacationEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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
        private System.Windows.Forms.Label labelVacationType;
        private System.Windows.Forms.ComboBox cmbVacationType;
        private System.Windows.Forms.Label labelComment;
        private System.Windows.Forms.TextBox txtComment;

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnOk, btnCancel;
    }
}