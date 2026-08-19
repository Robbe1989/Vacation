using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp
{
    public partial class MainForm : Form
    {
        private const int DayColumnWidth = 28;
        private const int HeaderMinHeight = 80;

        public MainForm()
        {
            InitializeComponent();
            AddMenu();
            Database.Init();

            // Keine blaue Auswahl
            dgvCalendar.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvCalendar.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvCalendar.ClearSelection();

            // Events
            nudYear.ValueChanged += (s, e) => LoadCalendar((int)nudYear.Value);
            btnManageVacations.Click += (s, e) =>
            {
                using var f = new Forms.VacationsForm((int)nudYear.Value);
                f.ShowDialog(this);
                LoadCalendar((int)nudYear.Value);
            };

            // Header redraw when DGV changes
            dgvCalendar.Scroll += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.ColumnWidthChanged += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.Resize += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.ColumnDisplayIndexChanged += (s, e) => panelMonthHeader.Invalidate();
            panelMonthHeader.Paint += PanelMonthHeader_Paint;

            // Ensure header visible and load calendar after initial layout
            this.Shown += async (s, e) =>
            {
                try
                {
                    panelMonthHeader.BringToFront();
                }
                catch { }

                // small delay so WinForms finishes layout and DGV has display rectangles
                await System.Threading.Tasks.Task.Delay(80);

                EnsureHeaderMinHeight();

                try
                {
                    LoadCalendar((int)nudYear.Value);
                    dgvCalendar.ClearSelection();
                    panelMonthHeader.Invalidate();
                    System.Diagnostics.Debug.WriteLine($"[Startup] Columns={dgvCalendar.Columns.Count}, Rows={dgvCalendar.Rows.Count}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim initialen Laden: " + ex.Message);
                }
            };

            // Make Resize/Maximize robust: enforce header height and redraw after layout
            this.Resize += (s, e) =>
            {
                EnsureHeaderMinHeight();
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        dgvCalendar.PerformLayout();
                        panelMonthHeader.Invalidate();
                    }
                    catch { }
                }));
            };
            this.SizeChanged += (s, e) =>
            {
                EnsureHeaderMinHeight();
                panelMonthHeader.Invalidate();
            };
        }

        private void EnsureHeaderMinHeight()
        {
            if (panelMonthHeader == null) return;
            if (panelMonthHeader.Height < HeaderMinHeight)
                panelMonthHeader.Height = HeaderMinHeight;
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

                System.Diagnostics.Debug.WriteLine($"[LoadCalendar] Mitarbeiter: {employees.Count}, Urlaube: {vacations.Count}");

                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
                var firstOfYear = new DateTime(year, 1, 1);

                // Name column (frozen)
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

                // Day columns
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

                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        col.DefaultCellStyle = new DataGridViewCellStyle
                        {
                            BackColor = Color.FromArgb(240, 240, 240),
                            SelectionBackColor = Color.FromArgb(240, 240, 240),
                            SelectionForeColor = Color.Black
                        };
                    }
                    else
                    {
                        col.DefaultCellStyle.SelectionBackColor = Color.White;
                        col.DefaultCellStyle.SelectionForeColor = Color.Black;
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

                // Fill rows for employees
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
                            if (i >= 0) dayMarks[i] = true;
                    }

                    int total = 0;
                    for (int d = 0; d < daysInYear; d++)
                    {
                        if (dayMarks[d])
                        {
                            values[1 + d] = "●";
                            total++;
                        }
                        else values[1 + d] = "";
                    }

                    values[1 + daysInYear] = total > 0 ? total.ToString() : "";
                    int rowIndex = dgvCalendar.Rows.Add(values);

                    // Color vacation cells
                    if (total > 0)
                    {
                        for (int d = 0; d < daysInYear; d++)
                        {
                            if (dayMarks[d])
                            {
                                var cell = dgvCalendar.Rows[rowIndex].Cells[1 + d];
                                var vacColor = Color.LightSalmon;
                                cell.Style.BackColor = vacColor;
                                cell.Style.SelectionBackColor = vacColor;
                                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                cell.Style.Font = new Font(dgvCalendar.Font.FontFamily, dgvCalendar.Font.Size - 1);
                            }
                        }
                    }
                }

                if (dgvCalendar.Columns.Contains("colName"))
                    dgvCalendar.Columns["colName"].Frozen = true;

                dgvCalendar.ResumeLayout();

                dgvCalendar.ClearSelection();
                panelMonthHeader.Invalidate();
                
                System.Diagnostics.Debug.WriteLine($"[LoadCalendar] Fertig - Zeilen: {dgvCalendar.Rows.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Tageskalenders: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"[LoadCalendar ERROR] {ex}");
            }
        }

        private void PanelMonthHeader_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            try
            {
                g.Clear(panelMonthHeader.BackColor);

                if (panelMonthHeader.Height < 40)
                {
                    using var b = new SolidBrush(Color.FromArgb(255, 250, 205));
                    g.FillRectangle(b, panelMonthHeader.ClientRectangle);
                    return;
                }

                int year;
                try { year = (int)nudYear.Value; }
                catch { year = DateTime.Now.Year; }
                if (year < 1 || year > DateTime.MaxValue.Year) year = DateTime.Now.Year;

                var firstOfYear = new DateTime(year, 1, 1);
                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

                // layout: banner, week-row, day-header
                int bannerHeight = Math.Max(36, panelMonthHeader.Height * 45 / 100);
                int weekRowHeight = Math.Max(18, panelMonthHeader.Height * 16 / 100);
                int dayHeaderHeight = panelMonthHeader.Height - bannerHeight - weekRowHeight;
                if (dayHeaderHeight < 12) dayHeaderHeight = 12;

                var colorOdd = Color.FromArgb(255, 250, 205);
                var colorEven = Color.FromArgb(200, 235, 255);

                using var penBanner = new Pen(Color.LightGray);
                using var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                // draw month banners
                for (int month = 1; month <= 12; month++)
                {
                    DateTime monthStart, monthEnd;
                    try
                    {
                        monthStart = new DateTime(year, month, 1);
                        monthEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    }
                    catch { continue; }

                    int startIndex = (monthStart - firstOfYear).Days;
                    int endIndex = (monthEnd - firstOfYear).Days;
                    if (startIndex < 0) startIndex = 0;
                    if (endIndex >= daysInYear) endIndex = daysInYear - 1;
                    if (startIndex > endIndex) continue;

                    int colStart = 1 + startIndex;
                    int colEnd = 1 + endIndex;

                    Rectangle rectStart = Rectangle.Empty, rectEnd = Rectangle.Empty;
                    for (int c = colStart; c <= colEnd; c++)
                    {
                        try { var r = dgvCalendar.GetColumnDisplayRectangle(c, true); if (r.Width > 0) { rectStart = r; break; } }
                        catch { }
                    }
                    for (int c = colEnd; c >= colStart; c--)
                    {
                        try { var r = dgvCalendar.GetColumnDisplayRectangle(c, true); if (r.Width > 0) { rectEnd = r; break; } }
                        catch { }
                    }
                    if (rectStart.IsEmpty && rectEnd.IsEmpty) continue;

                    int xStart = rectStart.IsEmpty ? rectEnd.X : rectStart.X;
                    int xEnd = rectEnd.IsEmpty ? rectStart.Right : rectEnd.Right;
                    if (xEnd <= xStart) continue;

                    int width = xEnd - xStart;
                    var monthRect = new Rectangle(xStart, 0, Math.Min(width, panelMonthHeader.Width - xStart), bannerHeight - 1);
                    if (monthRect.Width <= 2) continue;

                    var fillColor = (month % 2 == 0) ? colorEven : colorOdd;
                    using var brushBanner = new SolidBrush(fillColor);
                    g.FillRectangle(brushBanner, monthRect);
                    g.DrawRectangle(penBanner, monthRect);

                    var monthName = new DateTime(year, month, 1).ToString("MMMM", CultureInfo.CurrentCulture);
                    using var bigFont = new Font(this.Font.FontFamily, Math.Max(12f, this.Font.Size + 2f), FontStyle.Bold);
                    g.DrawString(monthName, bigFont, Brushes.Black, monthRect, sfCenter);
                }

                // KW row background
                using (var brushWeekBg = new SolidBrush(Color.FromArgb(245, 245, 245)))
                using (var penWeek = new Pen(Color.LightGray))
                {
                    var weekAreaRect = new Rectangle(0, bannerHeight, panelMonthHeader.Width, weekRowHeight);
                    g.FillRectangle(brushWeekBg, weekAreaRect);
                    g.DrawLine(penWeek, 0, bannerHeight + weekRowHeight - 1, panelMonthHeader.Width, bannerHeight + weekRowHeight - 1);
                }

                // day header background
                using (var brushDayBg = new SolidBrush(Color.White))
                using (var penGrid = new Pen(Color.LightGray))
                {
                    var dayAreaRect = new Rectangle(0, bannerHeight + weekRowHeight, panelMonthHeader.Width, dayHeaderHeight);
                    g.FillRectangle(brushDayBg, dayAreaRect);
                    g.DrawLine(penGrid, 0, bannerHeight + weekRowHeight, panelMonthHeader.Width, bannerHeight + weekRowHeight);
                }

                // draw days + weekday labels + weekend shading
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
                        try { rect = dgvCalendar.GetColumnDisplayRectangle(colIndex, true); }
                        catch { continue; }

                        if (rect.Width == 0 && rect.Right <= 0) continue;
                        if (rect.Width == 0 && rect.Left >= dgvCalendar.ClientSize.Width) continue;

                        int x = rect.X;
                        int w = rect.Width > 0 ? rect.Width : DayColumnWidth;
                        var cellRect = new Rectangle(x, bannerHeight + weekRowHeight, w, dayHeaderHeight);

                        var date = firstOfYear.AddDays(d);
                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                            g.FillRectangle(brushWeekend, cellRect);

                        g.DrawLine(penDotted, cellRect.Left, bannerHeight + weekRowHeight, cellRect.Left, bannerHeight + weekRowHeight + dayHeaderHeight);

                        var dayRect = new Rectangle(cellRect.Left, cellRect.Top + 2, cellRect.Width, (cellRect.Height / 2) - 2);
                        var weekdayRect = new Rectangle(cellRect.Left, cellRect.Top + (cellRect.Height / 2), cellRect.Width, (cellRect.Height / 2) - 2);

                        string dayText = date.Day.ToString("00");
                        string weekdayShort = date.ToString("ddd", CultureInfo.CurrentCulture);

                        g.DrawString(dayText, smallFont, Brushes.Black, dayRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                        g.DrawString(weekdayShort, weekdayFont, Brushes.DarkSlateGray, weekdayRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }

                    int lastDayCol = dgvCalendar.Columns.Count - 2;
                    if (lastDayCol >= 1)
                    {
                        try
                        {
                            var lastRect = dgvCalendar.GetColumnDisplayRectangle(lastDayCol, true);
                            if (lastRect.Width > 0)
                            {
                                int xRight = lastRect.Right;
                                g.DrawLine(penDotted, xRight, bannerHeight + weekRowHeight, xRight, bannerHeight + weekRowHeight + dayHeaderHeight);
                            }
                        }
                        catch { }
                    }
                }

                // KW numbers centered over Mon..Sun (Monday start)
                using var weekFont = new Font(this.Font.FontFamily,
                                              Math.Max(9f, this.Font.Size - 1f),
                                              FontStyle.Bold);

                var drawnWeeks = new HashSet<string>();

                for (int d = 0; d < daysInYear; d++)
                {
                    var date = firstOfYear.AddDays(d);

                    int kw = ISOWeek.GetWeekOfYear(date);
                    int isoYear = ISOWeek.GetYear(date);

                    string weekKey = $"{isoYear}-{kw}";

                    if (!drawnWeeks.Add(weekKey))
                        continue;

                    var weekDays = Enumerable.Range(0, daysInYear)
                        .Where(i =>
                        {
                            var dt = firstOfYear.AddDays(i);
                            return ISOWeek.GetWeekOfYear(dt) == kw &&
                                   ISOWeek.GetYear(dt) == isoYear;
                        })
                        .ToList();

                    if (!weekDays.Any())
                        continue;

                    int firstDay = weekDays.First();
                    int lastDay = weekDays.Last();

                    Rectangle rectStart = dgvCalendar.GetColumnDisplayRectangle(firstDay + 1, true);
                    Rectangle rectEnd = dgvCalendar.GetColumnDisplayRectangle(lastDay + 1, true);

                    if (rectStart.Width <= 0 && rectEnd.Width <= 0)
                        continue;

                    var weekRect = new Rectangle(
                        rectStart.X,
                        bannerHeight,
                        rectEnd.Right - rectStart.X,
                        weekRowHeight);

                    using var weekBorderPen = new Pen(Color.DimGray, 2);

                    g.DrawLine(
                        weekBorderPen,
                        weekRect.Right,
                        bannerHeight,
                        weekRect.Right,
                        bannerHeight + weekRowHeight + dayHeaderHeight);

                    if (weekRect.Width > 4)
                    {
                        g.DrawString(
                            kw.ToString(),
                            weekFont,
                            Brushes.Black,
                            weekRect,
                            new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PanelMonthHeader_Paint error: " + ex);
            }
        }

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
