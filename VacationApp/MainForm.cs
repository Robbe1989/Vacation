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
        // Breite pro Tagenspalte
        private const int DayColumnWidth = 28;

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

            // Sync events for header redraw
            dgvCalendar.Scroll += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.ColumnWidthChanged += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.Resize += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.ColumnDisplayIndexChanged += (s, e) => panelMonthHeader.Invalidate();
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
                        HeaderText = date.Day.ToString(), // not shown
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
                    object[] values = new object[1 + daysInYear + 1];
                    values[0] = emp.Name;

                    var dayMarks = new bool[daysInYear];

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
                    for (int d = 0; d < daysInYear; d++)
                    {
                        if (dayMarks[d])
                        {
                            values[1 + d] = "●"; // filled dot for visibility
                            total++;
                        }
                        else
                        {
                            values[1 + d] = "";
                        }
                    }

                    values[1 + daysInYear] = total > 0 ? total.ToString() : "";

                    int rowIndex = dgvCalendar.Rows.Add(values);

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

                // Freeze name column
                if (dgvCalendar.Columns.Contains("colName"))
                    dgvCalendar.Columns["colName"].Frozen = true;

                dgvCalendar.ResumeLayout();

                panelMonthHeader.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Tageskalenders: " + ex.Message);
            }
        }

        // Draw big month banner + day numbers + weekday abbreviations + dotted separators
        private void PanelMonthHeader_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(panelMonthHeader.BackColor);

            int year = (int)nudYear.Value;
            var firstOfYear = new DateTime(year, 1, 1);
            int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

            // layout sizes
            int bannerHeight = Math.Max(40, panelMonthHeader.Height * 60 / 100); // upper 60% for month title
            int dayHeaderHeight = panelMonthHeader.Height - bannerHeight; // lower area for day numbers

            // draw month banner background (full width) - pale yellow
            using (var brushBanner = new SolidBrush(Color.FromArgb(255, 250, 205))) // lemonchiffon-like
            using (var penBanner = new Pen(Color.LightGray))
            {
                var bannerRect = new Rectangle(0, 0, panelMonthHeader.Width, bannerHeight);
                g.FillRectangle(brushBanner, bannerRect);
                g.DrawRectangle(penBanner, 0, 0, bannerRect.Width - 1, bannerRect.Height - 1);
            }

            // Draw the month name centered (for the month currently visible in the leftmost visible day)
            // We will draw the full year months across the width, but also draw the current visible month's big name centered relative to visible day area.
            // Find the first fully/partially visible day column to determine visible month
            string bigMonthName = new DateTime(year, (int)nudYear.Value, 1).ToString("MMMM"); // fallback
            // Better: find the month that the leftmost visible day belongs to
            int firstVisibleDayIndex = -1;
            for (int col = 1; col < dgvCalendar.Columns.Count - 1; col++) // skip name col and total
            {
                var r = dgvCalendar.GetColumnDisplayRectangle(col, true);
                if (r.Width > 0)
                {
                    // day index:
                    firstVisibleDayIndex = col - 1; // dayIndex = col - 1 (since col 1 = day 1)
                    break;
                }
            }
            if (firstVisibleDayIndex >= 0)
            {
                var d = firstOfYear.AddDays(firstVisibleDayIndex);
                bigMonthName = d.ToString("MMMM", System.Globalization.CultureInfo.CurrentCulture);
            }

            using (var bigFont = new Font(this.Font.FontFamily, Math.Max(18f, this.Font.Size + 6f), FontStyle.Bold))
            using (var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                var bannerRect = new Rectangle(0, 0, panelMonthHeader.Width, bannerHeight);
                g.DrawString(bigMonthName, bigFont, Brushes.Black, bannerRect, sfCenter);
            }

            // draw day header background (white) and small borders
            using (var brushDayBg = new SolidBrush(Color.White))
            using (var penGrid = new Pen(Color.LightGray))
            {
                var dayAreaRect = new Rectangle(0, bannerHeight, panelMonthHeader.Width, dayHeaderHeight);
                g.FillRectangle(brushDayBg, dayAreaRect);
                // top border line
                g.DrawLine(penGrid, 0, bannerHeight, panelMonthHeader.Width, bannerHeight);
            }

            // draw each visible day: day number (dd) and weekday (Mo/Di/...)
            using (var smallFont = new Font(this.Font.FontFamily, Math.Max(8f, this.Font.Size - 1f)))
            using (var weekdayFont = new Font(this.Font.FontFamily, Math.Max(7f, this.Font.Size - 3f)))
            using (var penDotted = new Pen(Color.Gray))
            {
                penDotted.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                StringFormat sfDay = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                StringFormat sfWeek = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

                for (int d = 0; d < daysInYear; d++)
                {
                    int colIndex = 1 + d; // column index in dgvCalendar
                    var rect = dgvCalendar.GetColumnDisplayRectangle(colIndex, true);
                    if (rect.Width == 0 && rect.Right <= 0) continue; // not visible and left of viewport
                    if (rect.Width == 0 && rect.Left >= dgvCalendar.ClientSize.Width) continue; // not visible and right of viewport

                    // compute panel X relative (dgvCalendar and panelMonthHeader are left-aligned)
                    int x = rect.X;
                    int w = rect.Width;
                    if (w <= 0) w = DayColumnWidth; // fallback

                    // cell rect in panel coordinates
                    var cellRect = new Rectangle(x, bannerHeight, w, dayHeaderHeight);

                    // draw vertical dotted separator on left boundary (except if x==0 or overlap with name column area)
                    var sepX = cellRect.Left;
                    // Only draw separator if inside visible area
                    g.DrawLine(penDotted, sepX, bannerHeight, sepX, bannerHeight + dayHeaderHeight);

                    // draw day number (top of small area)
                    var dayRect = new Rectangle(cellRect.Left, cellRect.Top + 2, cellRect.Width, (cellRect.Height / 2) - 2);
                    var weekdayRect = new Rectangle(cellRect.Left, cellRect.Top + (cellRect.Height / 2), cellRect.Width, (cellRect.Height / 2) - 2);

                    string dayText = (d + 1).ToString("00");
                    string weekText = firstOfYear.AddDays(d).ToString("dd").Length > 0 ? firstOfYear.AddDays(d).ToString("ddd", System.Globalization.CultureInfo.CurrentCulture) : "";

                    // Draw day number
                    g.DrawString(dayText, smallFont, Brushes.Black, dayRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                    // Draw weekday (short, e.g., Mo, Di)
                    var weekdayShort = firstOfYear.AddDays(d).ToString("ddd", System.Globalization.CultureInfo.CurrentCulture);
                    g.DrawString(weekdayShort, weekdayFont, Brushes.DarkSlateGray, weekdayRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }

                // rightmost separator line
                var lastVisible = dgvCalendar.GetColumnDisplayRectangle(dgvCalendar.Columns.Count - 2, true); // last day col
                if (lastVisible.Width > 0)
                {
                    int xRight = lastVisible.Right;
                    g.DrawLine(penDotted, xRight, bannerHeight, xRight, bannerHeight + dayHeaderHeight);
                }
            }
        }

        // AddMenu kept or adapted to existing logic
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