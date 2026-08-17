using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp
{
    public partial class MainForm : Form
    {
        private const int DayColumnWidth = 20; // pixels per day column

        public MainForm()
        {
            InitializeComponent();
            AddMenu();
            Database.Init();

            // Hook events
            nudYear.ValueChanged += (s, e) => LoadCalendar((int)nudYear.Value);
            btnManageVacations.Click += (s, e) =>
            {
                using var f = new Forms.VacationsForm((int)nudYear.Value);
                f.ShowDialog(this);
                LoadCalendar((int)nudYear.Value);
            };

            nudYear.Value = DateTime.Now.Year;
            LoadCalendar((int)nudYear.Value);
        }

        private void LoadCalendar(int year)
        {
            try
            {
                dgvCalendar.SuspendLayout();
                dgvCalendar.Columns.Clear();
                dgvCalendar.Rows.Clear();

                var employees = Database.GetAllEmployees();
                var vacations = Database.GetVacationsForYear(year);

                // Number of days in year
                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
                var firstOfYear = new DateTime(year, 1, 1);

                // Add Name column (frozen)
                var colName = new DataGridViewTextBoxColumn
                {
                    Name = "colName",
                    HeaderText = "Mitarbeiter",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 200,
                    Frozen = true
                };
                dgvCalendar.Columns.Add(colName);

                // Add day columns
                for (int d = 0; d < daysInYear; d++)
                {
                    var date = firstOfYear.AddDays(d);
                    var col = new DataGridViewTextBoxColumn
                    {
                        Name = $"d{d + 1}",
                        HeaderText = date.Day.ToString(), // show day number
                        ReadOnly = true,
                        Width = DayColumnWidth,
                        ToolTipText = date.ToString("dd.MM.yyyy")
                    };
                    dgvCalendar.Columns.Add(col);
                }

                // Total column
                var colTotal = new DataGridViewTextBoxColumn
                {
                    Name = "colTotal",
                    HeaderText = "Total",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 60,
                    Frozen = false
                };
                dgvCalendar.Columns.Add(colTotal);

                // Fill rows
                foreach (var emp in employees)
                {
                    // prepare an array for row values (1 + daysInYear + 1)
                    object[] values = new object[1 + daysInYear + 1];
                    values[0] = emp.Name;

                    // boolean array for marking vacation days
                    var dayMarks = new bool[daysInYear];

                    // get vacations for this employee overlapping year
                    var vlist = vacations.Where(v => v.EmployeeId == emp.Id).ToList();
                    foreach (var v in vlist)
                    {
                        var s = v.StartDate < firstOfYear ? firstOfYear : v.StartDate;
                        var e = v.EndDate > firstOfYear.AddDays(daysInYear - 1) ? firstOfYear.AddDays(daysInYear - 1) : v.EndDate;
                        if (e < s) continue;
                        int startIndex = (s - firstOfYear).Days;
                        int endIndex = (e - firstOfYear).Days;
                        for (int i = startIndex; i <= endIndex && i < daysInYear; i++)
                        {
                            if (i >= 0) dayMarks[i] = true;
                        }
                    }

                    int total = 0;
                    // set day cell values as "X" (or empty)
                    for (int d = 0; d < daysInYear; d++)
                    {
                        if (dayMarks[d])
                        {
                            values[1 + d] = "X";
                            total++;
                        }
                        else
                        {
                            values[1 + d] = "";
                        }
                    }

                    values[1 + daysInYear] = total > 0 ? total.ToString() : "";

                    int rowIndex = dgvCalendar.Rows.Add(values);

                    // set styling for vacation cells (background color)
                    if (total > 0)
                    {
                        for (int d = 0; d < daysInYear; d++)
                        {
                            if (dayMarks[d])
                            {
                                var cell = dgvCalendar.Rows[rowIndex].Cells[1 + d];
                                cell.Style.BackColor = Color.LightSalmon;
                                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                cell.Style.Font = new Font(dgvCalendar.Font.FontFamily, dgvCalendar.Font.Size - 1);
                            }
                        }
                    }
                }

                // Freeze the name column so it stays visible
                if (dgvCalendar.Columns.Contains("colName"))
                    dgvCalendar.Columns["colName"].Frozen = true;

                // Improve header appearance: rotate header or keep as single numbers (we keep numbers)
                dgvCalendar.ResumeLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Tageskalenders: " + ex.Message);
            }
        }

        // AddMenu kept or adapted to existing logic (uses menuStrip1 from designer)
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