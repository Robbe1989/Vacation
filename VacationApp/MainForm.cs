// name=VacationApp/MainForm.cs
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
                            values[1 + d] = "●";
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

        // Robust month header paint — guards against invalid DateTime values.
        private void PanelMonthHeader_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            try
            {
                g.Clear(panelMonthHeader.BackColor);

                // read year safely
                int year;
                try
                {
                    year = (int)nudYear.Value;
                }
                catch
                {
                    year = DateTime.Now.Year;
                }

                // validate year range for DateTime
                if (year < 1 || year > DateTime.MaxValue.Year)
                    year = DateTime.Now.Year;

                var firstOfYear = new DateTime(year, 1, 1);
                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

                // layout sizes
                int bannerHeight = Math.Max(40, panelMonthHeader.Height * 60 / 100);
                int dayHeaderHeight = panelMonthHeader.Height - bannerHeight;

                // draw month banner background (pale yellow)
                using (var brushBanner = new SolidBrush(Color.FromArgb(255, 250, 205)))
                using (var penBanner = new Pen(Color.LightGray))
                {
                    var bannerRect = new Rectangle(0, 0, panelMonthHeader.Width, bannerHeight);
                    g.FillRectangle(brushBanner, bannerRect);
                    g.DrawRectangle(penBanner, 0, 0, bannerRect.Width - 1, bannerRect.Height - 1);
                }

                // Determine leftmost visible day index safely (0-based)
                int firstVisibleDayIndex = 0; // default to day 1
                bool found = false;
                for (int col = 1; col <= dgvCalendar.Columns.Count - 2; col++) // skip name and total columns
                {
                    try
                    {
                        var r = dgvCalendar.GetColumnDisplayRectangle(col, true);
                        if (r.Width > 0 && r.Right > 0)
                        {
                            firstVisibleDayIndex = col - 1; // because day columns start at index 1
                            found = true;
                            break;
                        }
                    }
                    catch
                    {
                        // ignore and continue
                    }
                }
                if (!found)
                    firstVisibleDayIndex = 0;

                // choose big month name from leftmost visible day (clamped)
                if (firstVisibleDayIndex < 0) firstVisibleDayIndex = 0;
                if (firstVisibleDayIndex >= daysInYear) firstVisibleDayIndex = daysInYear - 1;
                var bigMonthName = firstOfYear.AddDays(firstVisibleDayIndex).ToString("MMMM", System.Globalization.CultureInfo.CurrentCulture);

                using (var bigFont = new Font(this.Font.FontFamily, Math.Max(18f, this.Font.Size + 6f), FontStyle.Bold))
                using (var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    var bannerRect = new Rectangle(0, 0, panelMonthHeader.Width, bannerHeight);
                    g.DrawString(bigMonthName, bigFont, Brushes.Black, bannerRect, sfCenter);
                }

                // draw day header background
                using (var brushDayBg = new SolidBrush(Color.White))
                using (var penGrid = new Pen(Color.LightGray))
                {
                    var dayAreaRect = new Rectangle(0, bannerHeight, panelMonthHeader.Width, dayHeaderHeight);
                    g.FillRectangle(brushDayBg, dayAreaRect);
                    g.DrawLine(penGrid, 0, bannerHeight, panelMonthHeader.Width, bannerHeight);
                }

                // draw day cells: number + weekday, dotted separators
                using (var smallFont = new Font(this.Font.FontFamily, Math.Max(8f, this.Font.Size - 1f)))
                using (var weekdayFont = new Font(this.Font.FontFamily, Math.Max(7f, this.Font.Size - 3f)))
                using (var penDotted = new Pen(Color.Gray))
                {
                    penDotted.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                    for (int d = 0; d < daysInYear; d++)
                    {
                        int colIndex = 1 + d;
                        Rectangle rect;
                        try
                        {
                            rect = dgvCalendar.GetColumnDisplayRectangle(colIndex, true);
                        }
                        catch
                        {
                            continue;
                        }

                        // skip fully not visible columns
                        if (rect.Width == 0 && rect.Right <= 0) continue;
                        if (rect.Width == 0 && rect.Left >= dgvCalendar.ClientSize.Width) continue;

                        int x = rect.X;
                        int w = rect.Width > 0 ? rect.Width : DayColumnWidth;

                        var cellRect = new Rectangle(x, bannerHeight, w, dayHeaderHeight);
                        // dotted vertical separator at left
                        g.DrawLine(penDotted, cellRect.Left, bannerHeight, cellRect.Left, bannerHeight + dayHeaderHeight);

                        var dayRect = new Rectangle(cellRect.Left, cellRect.Top + 2, cellRect.Width, (cellRect.Height / 2) - 2);
                        var weekdayRect = new Rectangle(cellRect.Left, cellRect.Top + (cellRect.Height / 2), cellRect.Width, (cellRect.Height / 2) - 2);

                        string dayText = (d + 1).ToString("00");
                        string weekdayShort = firstOfYear.AddDays(d).ToString("ddd", System.Globalization.CultureInfo.CurrentCulture);

                        g.DrawString(dayText, smallFont, Brushes.Black, dayRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                        g.DrawString(weekdayShort, weekdayFont, Brushes.DarkSlateGray, weekdayRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }

                    // rightmost separator (optional)
                    int lastDayCol = dgvCalendar.Columns.Count - 2;
                    if (lastDayCol >= 1)
                    {
                        try
                        {
                            var lastRect = dgvCalendar.GetColumnDisplayRectangle(lastDayCol, true);
                            if (lastRect.Width > 0)
                            {
                                int xRight = lastRect.Right;
                                g.DrawLine(penDotted, xRight, bannerHeight, xRight, bannerHeight + dayHeaderHeight);
                            }
                        }
                        catch { /* ignore */ }
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // defensive: nothing to draw if DateTime parameters bad
            }
            catch (Exception ex)
            {
                // log or ignore; avoid crashing paint handler
                System.Diagnostics.Debug.WriteLine("PanelMonthHeader_Paint error: " + ex);
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