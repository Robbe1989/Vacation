# create_employees_crud.ps1
Set-StrictMode -Version Latest

function Write-TextFile($path, $content) {
    $dir = Split-Path $path
    if ($dir -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $content | Out-File -FilePath $path -Encoding UTF8 -Force
}

# Employee model
Write-TextFile "VacationApp/Models/Employee.cs" @'
using System;

namespace VacationApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Today;
        public double Fte { get; set; } = 1.0;
    }
}
'@

# Database helper (SQLite)
Write-TextFile "VacationApp/Data/Database.cs" @'
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using VacationApp.Models;

namespace VacationApp.Data
{
    public static class Database
    {
        private static string DbFile => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vacation.db");
        private static string ConnectionString => $"Data Source={DbFile};Version=3;";

        public static void Init()
        {
            bool exists = System.IO.File.Exists(DbFile);
            if (!exists)
            {
                SQLiteConnection.CreateFile(DbFile);
            }

            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            string createTableSql = @"
CREATE TABLE IF NOT EXISTS Employees (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Email TEXT,
    Department TEXT,
    StartDate TEXT,
    Fte REAL
);";
            using var cmd = new SQLiteCommand(createTableSql, conn);
            cmd.ExecuteNonQuery();
        }

        public static List<Employee> GetAllEmployees()
        {
            var list = new List<Employee>();
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, Name, Email, Department, StartDate, Fte FROM Employees ORDER BY Name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Employee
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = Convert.ToString(reader["Name"]) ?? "",
                    Email = Convert.ToString(reader["Email"]) ?? "",
                    Department = Convert.ToString(reader["Department"]) ?? "",
                    StartDate = DateTime.TryParse(Convert.ToString(reader["StartDate"]), out var dt) ? dt : DateTime.Today,
                    Fte = reader["Fte"] != DBNull.Value ? Convert.ToDouble(reader["Fte"]) : 1.0
                });
            }
            return list;
        }

        public static int AddEmployee(Employee e)
        {
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("INSERT INTO Employees (Name, Email, Department, StartDate, Fte) VALUES (@Name, @Email, @Department, @StartDate, @Fte); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@Name", e.Name);
            cmd.Parameters.AddWithValue("@Email", e.Email);
            cmd.Parameters.AddWithValue("@Department", e.Department);
            cmd.Parameters.AddWithValue("@StartDate", e.StartDate.ToString("o"));
            cmd.Parameters.AddWithValue("@Fte", e.Fte);
            var id = cmd.ExecuteScalar();
            return Convert.ToInt32(id);
        }

        public static void UpdateEmployee(Employee e)
        {
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE Employees SET Name=@Name, Email=@Email, Department=@Department, StartDate=@StartDate, Fte=@Fte WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Name", e.Name);
            cmd.Parameters.AddWithValue("@Email", e.Email);
            cmd.Parameters.AddWithValue("@Department", e.Department);
            cmd.Parameters.AddWithValue("@StartDate", e.StartDate.ToString("o"));
            cmd.Parameters.AddWithValue("@Fte", e.Fte);
            cmd.Parameters.AddWithValue("@Id", e.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteEmployee(int id)
        {
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM Employees WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
'@

# EmployeeEditForm (code)
Write-TextFile "VacationApp/Forms/EmployeeEditForm.cs" @'
using System;
using System.Windows.Forms;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class EmployeeEditForm : Form
    {
        public Employee Employee { get; private set; }

        public EmployeeEditForm(Employee? e = null)
        {
            InitializeComponent();
            if (e == null)
            {
                Employee = new Employee();
            }
            else
            {
                Employee = e;
                txtName.Text = Employee.Name;
                txtEmail.Text = Employee.Email;
                txtDepartment.Text = Employee.Department;
                dtpStartDate.Value = Employee.StartDate;
                numFte.Value = (decimal)Employee.Fte;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Employee.Name = txtName.Text.Trim();
            Employee.Email = txtEmail.Text.Trim();
            Employee.Department = txtDepartment.Text.Trim();
            Employee.StartDate = dtpStartDate.Value.Date;
            Employee.Fte = (double)numFte.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
'@

# EmployeeEditForm Designer
Write-TextFile "VacationApp/Forms/EmployeeEditForm.Designer.cs" @'
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
'@

# EmployeesForm (code)
Write-TextFile "VacationApp/Forms/EmployeesForm.cs" @'
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp.Forms
{
    public partial class EmployeesForm : Form
    {
        public EmployeesForm()
        {
            InitializeComponent();
            Database.Init();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            var list = Database.GetAllEmployees();
            dgvEmployees.Rows.Clear();
            foreach (var e in list)
            {
                dgvEmployees.Rows.Add(e.Id, e.Name, e.Email, e.Department, e.StartDate.ToShortDateString(), e.Fte);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var dlg = new EmployeeEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Database.AddEmployee(dlg.Employee);
                LoadEmployees();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0) return;
            var row = dgvEmployees.SelectedRows[0];
            var emp = new Employee
            {
                Id = Convert.ToInt32(row.Cells["colId"].Value),
                Name = Convert.ToString(row.Cells["colName"].Value) ?? "",
                Email = Convert.ToString(row.Cells["colEmail"].Value) ?? "",
                Department = Convert.ToString(row.Cells["colDepartment"].Value) ?? "",
                StartDate = DateTime.TryParse(Convert.ToString(row.Cells["colStartDate"].Value), out var dt) ? dt : DateTime.Today,
                Fte = Convert.ToDouble(row.Cells["colFte"].Value)
            };
            using var dlg = new EmployeeEditForm(emp);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Database.UpdateEmployee(dlg.Employee);
                LoadEmployees();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0) return;
            var row = dgvEmployees.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["colId"].Value);
            if (MessageBox.Show("Mitarbeiter wirklich löschen?", "Löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Database.DeleteEmployee(id);
                LoadEmployees();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadEmployees();
    }
}
'@

# EmployeesForm Designer
Write-TextFile "VacationApp/Forms/EmployeesForm.Designer.cs" @'
namespace VacationApp.Forms
{
    partial class EmployeesForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) { components.Dispose(); } base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.dgvEmployees = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDepartment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStartDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            this.SuspendLayout();
            // dgvEmployees
            this.dgvEmployees.AllowUserToAddRows = false;
            this.dgvEmployees.AllowUserToDeleteRows = false;
            this.dgvEmployees.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEmployees.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId, this.colName, this.colEmail, this.colDepartment, this.colStartDate, this.colFte});
            this.dgvEmployees.Location = new System.Drawing.Point(12, 12);
            this.dgvEmployees.MultiSelect = false;
            this.dgvEmployees.Name = "dgvEmployees";
            this.dgvEmployees.ReadOnly = true;
            this.dgvEmployees.RowHeadersVisible = false;
            this.dgvEmployees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployees.Size = new System.Drawing.Size(760, 380);
            // Columns
            this.colId.HeaderText = "Id"; this.colId.Name = "colId"; this.colId.Visible = false;
            this.colName.HeaderText = "Name"; this.colName.Name = "colName"; this.colName.Width = 220;
            this.colEmail.HeaderText = "E-Mail"; this.colEmail.Name = "colEmail"; this.colEmail.Width = 180;
            this.colDepartment.HeaderText = "Abteilung"; this.colDepartment.Name = "colDepartment"; this.colDepartment.Width = 120;
            this.colStartDate.HeaderText = "Eintritt"; this.colStartDate.Name = "colStartDate"; this.colStartDate.Width = 90;
            this.colFte.HeaderText = "FTE"; this.colFte.Name = "colFte"; this.colFte.Width = 60;
            // Buttons
            this.btnAdd.Location = new System.Drawing.Point(12, 405); this.btnAdd.Name = "btnAdd"; this.btnAdd.Size = new System.Drawing.Size(90, 28); this.btnAdd.Text = "Hinzufügen"; this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnEdit.Location = new System.Drawing.Point(108, 405); this.btnEdit.Name = "btnEdit"; this.btnEdit.Size = new System.Drawing.Size(90, 28); this.btnEdit.Text = "Bearbeiten"; this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            this.btnDelete.Location = new System.Drawing.Point(204, 405); this.btnDelete.Name = "btnDelete"; this.btnDelete.Size = new System.Drawing.Size(90, 28); this.btnDelete.Text = "Löschen"; this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnRefresh.Location = new System.Drawing.Point(300, 405); this.btnRefresh.Name = "btnRefresh"; this.btnRefresh.Size = new System.Drawing.Size(90, 28); this.btnRefresh.Text = "Aktualisieren"; this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // Form
            this.ClientSize = new System.Drawing.Size(784, 445);
            this.Controls.Add(this.dgvEmployees); this.Controls.Add(this.btnAdd); this.Controls.Add(this.btnEdit); this.Controls.Add(this.btnDelete); this.Controls.Add(this.btnRefresh);
            this.Name = "EmployeesForm"; this.Text = "Mitarbeiter";
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvEmployees;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDepartment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStartDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFte

