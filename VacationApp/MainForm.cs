using System;
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

                // Add day columns; set weekend default background
                for (int d = 0; d < daysInYear; d++)
                {
                    var date = firstOfYear.AddDays(d);
                    var col = new DataGridViewTextBoxColumn
                    {
                        Name = $"d{d + 1}",
                        HeaderText = date.Day.ToString(),
                        ReadOnly = true,
                        Width = DayColumnWidth,
                        ToolTipText = date.ToString("dd.MM.yyyy")
                    };

                    // Wenn Samstag oder Sonntag -> hellgrauer Default-Hintergrund
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        col.DefaultCellStyle = new DataGridViewCellStyle
                        {
                            BackColor = Color.FromArgb(240, 240, 240),
                            SelectionBackColor = Color.FromArgb(200, 200, 200)
                        };
                    }

                    dgvCalendar.Columns.Add(col);
                }

                // Total column
                var colTotal = new DataGridViewTextBoxColumn
                {
                    Name = "colTotal",
                    HeaderText = "Total",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 60
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
                            // Urlaubsmarkierung hat Vorrang vor Default-WE-Hintergrund
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
                                // explizit setzen, damit Urlaubs-Farbe sichtbar ist (überschreibt Default)
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

        // Zeichnet pro Monat ein Banner über den entsprechenden Tages-Spalten.
        // Die Tageszahlen werden als Tag-im-Monat (1..N) angezeigt.
        // Zusätzlich werden die Tage für Sa/So in der Header-Grafik hellgrau hinterlegt.
        private void PanelMonthHeader_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            try
            {
                g.Clear(panelMonthHeader.BackColor);

                // sicheren Jahrwert lesen
                int year;
                try { year = (int)nudYear.Value; }
                catch { year = DateTime.Now.Year; }
                if (year < 1 || year > DateTime.MaxValue.Year) year = DateTime.Now.Year;

                var firstOfYear = new DateTime(year, 1, 1);
                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

                // Layout: oberer Bereich für Monats-Banner, unterer Bereich für Tag/Zt
                int bannerHeight = Math.Max(36, panelMonthHeader.Height * 55 / 100);
                int dayHeaderHeight = panelMonthHeader.Height - bannerHeight;

                // Für jeden Monat: Bestimme sichtbare Spalten und zeichne Banner nur über diesen Spalten
                using var brushBanner = new SolidBrush(Color.FromArgb(255, 250, 205)); // pale yellow
                using var penBanner = new Pen(Color.LightGray);
                using var sfCenterTop = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                for (int month = 1; month <= 12; month++)
                {
                    DateTime monthStart;
                    DateTime monthEnd;
                    try
                    {
                        monthStart = new DateTime(year, month, 1);
                        monthEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    }
                    catch
                    {
                        continue;
                    }

                    int startIndex = (monthStart - firstOfYear).Days;
                    int endIndex = (monthEnd - firstOfYear).Days;
                    if (startIndex < 0) startIndex = 0;
                    if (endIndex >= daysInYear) endIndex = daysInYear - 1;
                    if (startIndex > endIndex) continue;

                    int colStart = 1 + startIndex;
                    int colEnd = 1 + endIndex;

                    Rectangle rectStart = Rectangle.Empty;
                    Rectangle rectEnd = Rectangle.Empty;
                    for (int c = colStart; c <= colEnd; c++)
                    {
                        try
                        {
                            var r = dgvCalendar.GetColumnDisplayRectangle(c, true);
                            if (r.Width > 0)
                            {
                                rectStart = r;
                                break;
                            }
                        }
                        catch { }
                    }
                    for (int c = colEnd; c >= colStart; c--)
                    {
                        try
                        {
                            var r = dgvCalendar.GetColumnDisplayRectangle(c, true);
                            if (r.Width > 0)
                            {
                                rectEnd = r;
                                break;
                            }
                        }
                        catch { }
                    }

                    if (rectStart.IsEmpty && rectEnd.IsEmpty) continue;

                    int xStart = rectStart.IsEmpty ? rectEnd.X : rectStart.X;
                    int xEnd = rectEnd.IsEmpty ? rectStart.Right : rectEnd.Right;
                    if (xEnd <= xStart) continue;

                    int width = xEnd - xStart;
                    var monthRect = new Rectangle(xStart, 0, Math.Min(width, panelMonthHeader.Width - xStart), bannerHeight - 1);
                    if (monthRect.Width <= 2) continue;

                    // draw banner background & border
                    g.FillRectangle(brushBanner, monthRect);
                    g.DrawRectangle(penBanner, monthRect);

                    // draw full month name centered in this banner
                    var monthName = new DateTime(year, month, 1).ToString("MMMM", System.Globalization.CultureInfo.CurrentCulture);
                    using var bigFont = new Font(this.Font.FontFamily, Math.Max(12f, this.Font.Size + 2f), FontStyle.Bold);
                    g.DrawString(monthName, bigFont, Brushes.Black, monthRect, sfCenterTop);
                }

                // draw day header background (below banners)
                using (var brushDayBg = new SolidBrush(Color.White))
                using (var penGrid = new Pen(Color.LightGray))
                {
                    var dayAreaRect = new Rectangle(0, bannerHeight, panelMonthHeader.Width, dayHeaderHeight);
                    g.FillRectangle(brushDayBg, dayAreaRect);
                    g.DrawLine(penGrid, 0, bannerHeight, panelMonthHeader.Width, bannerHeight);
                }

                // draw each visible day: day-of-month and weekday; weekends get light-gray background here too
                using (var smallFont = new Font(this.Font.FontFamily, Math.Max(8f, this.Font.Size - 1f)))
                using (var weekdayFont = new Font(this.Font.FontFamily, Math.Max(7f, this.Font.Size - 3f)))
                using (var penDotted = new Pen(Color.Gray))
                using (var brushWeekend = new SolidBrush(Color.FromArgb(240, 240, 240)))
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

                        // skip if not visible at all
                        if (rect.Width == 0 && rect.Right <= 0) continue;
                        if (rect.Width == 0 && rect.Left >= dgvCalendar.ClientSize.Width) continue;

                        int x = rect.X;
                        int w = rect.Width > 0 ? rect.Width : DayColumnWidth;

                        var cellRect = new Rectangle(x, bannerHeight, w, dayHeaderHeight);

                        // if weekend, fill background here (so header also shows weekend shading)
                        var date = firstOfYear.AddDays(d);
                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            g.FillRectangle(brushWeekend, cellRect);
                        }

                        // dotted vertical separator
                        g.DrawLine(penDotted, cellRect.Left, bannerHeight, cellRect.Left, bannerHeight + dayHeaderHeight);

                        var dayRect = new Rectangle(cellRect.Left, cellRect.Top + 2, cellRect.Width, (cellRect.Height / 2) - 2);
                        var weekdayRect = new Rectangle(cellRect.Left, cellRect.Top + (cellRect.Height / 2), cellRect.Width, (cellRect.Height / 2) - 2);

                        string dayText = date.Day.ToString("00"); // day-of-month (resets each month)
                        string weekdayShort = date.ToString("ddd", System.Globalization.CultureInfo.CurrentCulture);

                        g.DrawString(dayText, smallFont, Brushes.Black, dayRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                        g.DrawString(weekdayShort, weekdayFont, Brushes.DarkSlateGray, weekdayRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }

                    // draw rightmost separator for last visible day column
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
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                // defensive: avoid crashing paint handler
                System.Diagnostics.Debug.WriteLine("PanelMonthHeader_Paint error: " + ex);
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