using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace VacationApp.Forms
{
    partial class EmployeeEditForm
    {
        private IContainer components = null;

        private void InitializeComponent()
        {
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();

            this.labelName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();

            this.labelEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();

            this.labelDepartment = new System.Windows.Forms.Label();
            this.cmbDepartment = new System.Windows.Forms.ComboBox();

            this.labelVacation = new System.Windows.Forms.Label();
            this.nudVacationDays = new System.Windows.Forms.NumericUpDown();

            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.nudVacationDays)).BeginInit();
            this.tableLayout.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();

            // tableLayout - 2 columns (labels, controls)
            this.tableLayout.ColumnCount = 2;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout.RowCount = 4;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayout.Location = new System.Drawing.Point(8, 8);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.Size = new System.Drawing.Size(386, 128);
            this.tableLayout.Padding = new System.Windows.Forms.Padding(4);

            // Name
            this.labelName.Text = "Name:";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelName.Dock = DockStyle.Fill;
            this.tableLayout.Controls.Add(this.labelName, 0, 0);

            this.txtName.Dock = DockStyle.Fill;
            this.tableLayout.Controls.Add(this.txtName, 1, 0);

            // Email
            this.labelEmail.Text = "E-Mail:";
            this.labelEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelEmail.Dock = DockStyle.Fill;
            this.tableLayout.Controls.Add(this.labelEmail, 0, 1);

            this.txtEmail.Dock = DockStyle.Fill;
            this.tableLayout.Controls.Add(this.txtEmail, 1, 1);

            // Department
            this.labelDepartment.Text = "Abteilung:";
            this.labelDepartment.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDepartment.Dock = DockStyle.Fill;
            this.tableLayout.Controls.Add(this.labelDepartment, 0, 2);

            this.cmbDepartment.Dock = DockStyle.Fill;
            this.cmbDepartment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tableLayout.Controls.Add(this.cmbDepartment, 1, 2);

            // VacationDays
            this.labelVacation.Text = "Urlaubstage:";
            this.labelVacation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelVacation.Dock = DockStyle.Fill;
            this.tableLayout.Controls.Add(this.labelVacation, 0, 3);

            this.nudVacationDays.Dock = DockStyle.Left; // keep compact but move with form
            this.nudVacationDays.Minimum = 0;
            this.nudVacationDays.Maximum = 365;
            this.nudVacationDays.Value = 20;
            this.tableLayout.Controls.Add(this.nudVacationDays, 1, 3);

            // Buttons panel dock bottom
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Height = 48;
            this.panelButtons.Padding = new Padding(8);

            this.btnOk.Text = "OK"; this.btnOk.Size = new System.Drawing.Size(75, 28); this.btnOk.Location = new System.Drawing.Point(220, 8); this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            this.btnCancel.Text = "Abbrechen"; this.btnCancel.Size = new System.Drawing.Size(90, 28); this.btnCancel.Location = new System.Drawing.Point(300, 8); this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.panelButtons.Controls.Add(this.btnOk);
            this.panelButtons.Controls.Add(this.btnCancel);

            // Form
            this.ClientSize = new System.Drawing.Size(402, 180);
            this.Controls.Add(this.tableLayout);
            this.Controls.Add(this.panelButtons);
            this.Padding = new Padding(4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "EmployeeEditForm";
            this.Text = "Mitarbeiter bearbeiten";

            ((System.ComponentModel.ISupportInitialize)(this.nudVacationDays)).EndInit();
            this.tableLayout.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.Label labelName, labelEmail, labelDepartment, labelVacation;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.ComboBox cmbDepartment;
        private System.Windows.Forms.NumericUpDown nudVacationDays;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnOk, btnCancel;
    }
}