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

            // Sync events for month header redraw
            dgvCalendar.Scroll += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.ColumnWidthChanged += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.Resize += (s, e) => panelMonthHeader.Invalidate();
            panelMonthHeader.Paint += PanelMonthHeader_Paint;

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

                dgvCalendar.ResumeLayout();

                // redraw month header
                panelMonthHeader.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Tageskalenders: " + ex.Message);
            }
        }

        // Paint month header: draw month spans aligned with dgvCalendar day columns
        private void PanelMonthHeader_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(panelMonthHeader.BackColor);

            int year = (int)nudYear.Value;
            var firstOfYear = new DateTime(year, 1, 1);
            int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

            using var brush = new SolidBrush(Color.FromArgb(230, 230, 230));
            using var pen = new Pen(Color.Gray);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            for (int month = 1; month <= 12; month++)
            {
                var monthStart = new DateTime(year, month, 1);
                var monthEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                // clamp to year range
                if (monthEnd.Year != year) monthEnd = new DateTime(year, 12, 31);
                if (monthStart.Year != year) monthStart = new DateTime(year, 1, 1);

                int startIndex = (monthStart - firstOfYear).Days;
                int endIndex = (monthEnd - firstOfYear).Days;
                if (startIndex < 0) startIndex = 0;
                if (endIndex >= daysInYear) endIndex = daysInYear - 1;
                if (startIndex > endIndex) continue;

                // get display rectangle in dgv coordinates for first and last day columns
                // column index in dgvCalendar = 1 + dayIndex
                int colStart = 1 + startIndex;
                int colEnd = 1 + endIndex;

                var rectStart = dgvCalendar.GetColumnDisplayRectangle(colStart, true);
                var rectEnd = dgvCalendar.GetColumnDisplayRectangle(colEnd, true);

                // if both columns are not visible (e.g., scrolled out), skip drawing
                if (rectStart.Width == 0 && rectEnd.Width == 0)
                    continue;

                // convert dgv point to panel coordinates
                var pointScreen = dgvCalendar.PointToScreen(new Point(rectStart.X, rectStart.Y));
                var panelPoint = panelMonthHeader.PointToClient(pointScreen);
                var pointScreenEnd = dgvCalendar.PointToScreen(new Point(rectEnd.Right, rectEnd.Y));
                var panelPointEnd = panelMonthHeader.PointToClient(pointScreenEnd);

                int x = panelPoint.X;
                int width = panelPointEnd.X - panelPoint.X;
                if (width <= 2) width = rectStart.Width; // fallback

                var r = new Rectangle(x, 0, Math.Max(0, width), panelMonthHeader.Height - 1);
                // draw background & border
                g.FillRectangle(brush, r);
                g.DrawRectangle(pen, r);

                // month name centered
                var monthName = new DateTime(year, month, 1).ToString("MMM", System.Globalization.CultureInfo.CurrentCulture);
                g.DrawString(monthName, this.Font, Brushes.Black, r, sf);
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