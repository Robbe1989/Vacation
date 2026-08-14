// name=VacationApp/MainForm.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            AddMenu(); // previous menu logic
            Database.Init();

            // Hook events
            nudYear.ValueChanged += (s, e) => LoadCalendar((int)nudYear.Value);
            btnManageVacations.Click += (s, e) =>
            {
                using var f = new Forms.VacationsForm((int)nudYear.Value);
                f.ShowDialog(this);
                LoadCalendar((int)nudYear.Value);
            };

            // initial load
            nudYear.Value = DateTime.Now.Year;
            LoadCalendar((int)nudYear.Value);
        }

        private void LoadCalendar(int year)
        {
            try
            {
                dgvCalendar.Columns.Clear();
                var employees = Database.GetAllEmployees();
                var vacations = Database.GetVacationsForYear(year);

                // Columns: Name, Jan..Dec, Total
                var colName = new DataGridViewTextBoxColumn() { Name = "colName", HeaderText = "Mitarbeiter", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
                dgvCalendar.Columns.Add(colName);

                for (int m = 1; m <= 12; m++)
                {
                    var monthName = new DateTime(year, m, 1).ToString("MMM", System.Globalization.CultureInfo.CurrentCulture);
                    dgvCalendar.Columns.Add(new DataGridViewTextBoxColumn() { Name = $"m{m}", HeaderText = monthName, ReadOnly = true, Width = 60 });
                }

                dgvCalendar.Columns.Add(new DataGridViewTextBoxColumn() { Name = "colTotal", HeaderText = "Total", ReadOnly = true, Width = 60 });

                dgvCalendar.Rows.Clear();

                foreach (var emp in employees)
                {
                    var rowValues = new List<object>();
                    rowValues.Add(emp.Name);

                    int total = 0;
                    for (int m = 1; m <= 12; m++)
                    {
                        var first = new DateTime(year, m, 1);
                        var last = new DateTime(year, m, DateTime.DaysInMonth(year, m));
                        int daysInMonth = 0;
                        foreach (var v in vacations.Where(x => x.EmployeeId == emp.Id))
                        {
                            var s = v.StartDate < first ? first : v.StartDate;
                            var e = v.EndDate > last ? last : v.EndDate;
                            if (e >= s)
                            {
                                daysInMonth += (e - s).Days + 1;
                            }
                        }
                        rowValues.Add(daysInMonth > 0 ? daysInMonth.ToString() : "");
                        total += daysInMonth;
                    }

                    rowValues.Add(total > 0 ? total.ToString() : "");
                    dgvCalendar.Rows.Add(rowValues.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Kalenders: " + ex.Message);
            }
        }

        // AddMenu kept as before (simple)
        private void AddMenu()
        {
            var menu = this.menuStrip1;
            menu.Items.Clear();

            var menuMitarbeiter = new ToolStripMenuItem("Mitarbeiter");
            var menuOpen = new ToolStripMenuItem("Verwalten");
            menuOpen.Click += (s, e) =>
            {
                using var f = new Forms.EmployeesForm();
                f.ShowDialog(this);
                LoadCalendar((int)nudYear.Value);
            };
            menuMitarbeiter.DropDownItems.Add(menuOpen);
            menu.Items.Add(menuMitarbeiter);

            var menuOptions = new ToolStripMenuItem("Optionen");
            var menuDepartments = new ToolStripMenuItem("Abteilungen");
            menuDepartments.Click += (s, e) =>
            {
                using var f = new Forms.DepartmentsForm();
                f.ShowDialog(this);
            };
            menuOptions.DropDownItems.Add(menuDepartments);
            menu.Items.Add(menuOptions);
        }
    }
}