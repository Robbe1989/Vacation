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
        private const int HeaderMinHeight = 80; // Mindesthöhe für panelMonthHeader

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

            // Header repaint sync
            dgvCalendar.Scroll += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.ColumnWidthChanged += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.Resize += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.ColumnDisplayIndexChanged += (s, e) => panelMonthHeader.Invalidate();
            panelMonthHeader.Paint += PanelMonthHeader_Paint;

            // Wenn das Formular in Größe/State wechselt (z.B. Vollbild), neu layouten und redrawen
            this.Resize += (s, e) =>
            {
                // Erzwinge Mindesthöhe und Neuzeichnen nach dem Layout-Pass
                EnsureHeaderMinHeight();
                this.BeginInvoke(new Action(() =>
                {
                    // Force layout so dgv spalten display-rects stimmen
                    dgvCalendar.PerformLayout();
                    panelMonthHeader.Invalidate();
                }));
            };
            this.ResizeEnd += (s, e) =>
            {
                EnsureHeaderMinHeight();
                panelMonthHeader.Invalidate();
            };
            this.ClientSizeChanged += (s, e) =>
            {
                EnsureHeaderMinHeight();
                this.BeginInvoke(new Action(() => panelMonthHeader.Invalidate()));
            };
            // Wenn WindowState geändert (z.B. Maximized), ebenfalls neuzeichnen
            this.SizeChanged += (s, e) =>
            {
                EnsureHeaderMinHeight();
                this.BeginInvoke(new Action(() => panelMonthHeader.Invalidate()));
            };

            // Erstes Laden nach Form.Shown (sicher, dass DGV Layout hat)
            this.Shown += (s, e) =>
            {
                // bring header to front to avoid it being overlapped by other controls
                panelMonthHeader.BringToFront();
                // Use BeginInvoke so the call happens after the current paint/layout pass.
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        EnsureHeaderMinHeight();
                        LoadCalendar((int)nudYear.Value);
                        dgvCalendar.ClearSelection();
                        panelMonthHeader.Invalidate();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler beim initialen Laden: " + ex.Message);
                    }
                }));
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

                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
                var firstOfYear = new DateTime(year, 1, 1);

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

                var colTotal = new DataGridViewTextBoxColumn
                {
                    Name = "colTotal",
                    HeaderText = "Total",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 60
                };
                dgvCalendar.Columns.Add(colTotal);

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Tageskalenders: " + ex.Message);
            }
        }

        // Paint wie vorher, aber mit Schutz gegen zu kleine Höhen und ungültige Spaltenrects
        private void PanelMonthHeader_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            try
            {
                g.Clear(panelMonthHeader.BackColor);

                // fallback if header too small
                if (panelMonthHeader.Height < 40)
                {
                    // keep a minimal visual feedback
                    using var smallBrush = new SolidBrush(Color.FromArgb(255, 250, 205));
                    g.FillRectangle(smallBrush, new Rectangle(0, 0, panelMonthHeader.Width, panelMonthHeader.Height));
                    return;
                }

                int year;
                try { year = (int)nudYear.Value; }
                catch { year = DateTime.Now.Year; }
                if (year < 1 || year > DateTime.MaxValue.Year) year = DateTime.Now.Year;

                var firstOfYear = new DateTime(year, 1, 1);
                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

                // compute safe heights, clamped
                int totalHeight = Math.Max(HeaderMinHeight, panelMonthHeader.Height);
                int bannerHeight = Math.Clamp((int)(totalHeight * 0.45), 36, totalHeight - 36);
                int weekRowHeight = Math.Clamp((int)(totalHeight * 0.15), 14, 32);
                int dayHeaderHeight = totalHeight - bannerHeight - weekRowHeight;
                if (dayHeaderHeight < 12) dayHeaderHeight = 12;

                // fonts sized relative to available area but clamped
                float dayFontSize = Math.Max(7f, Math.Min(12f, dayHeaderHeight * 0.42f));
                float weekdayFontSize = Math.Max(7f, Math.Min(10f, dayHeaderHeight * 0.28f));
                float monthFontSize = Math.Max(12f, this.Font.Size + Math.Min(10f, bannerHeight * 0.18f));

                var colorOdd = Color.FromArgb(255, 250, 205);
                var colorEven = Color.FromArgb(200, 235, 255);

                using var penBanner = new Pen(Color.LightGray);
                using var sfCenterTop = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                // draw months (alternating color) - same logic but tolerant to missing rects
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

                    Rectangle rectStart = Rectangle.Empty;
                    Rectangle rectEnd = Rectangle.Empty;
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
                    using var bigFont = new Font(this.Font.FontFamily, monthFontSize, FontStyle.Bold);
                    g.DrawString(monthName, bigFont, Brushes.Black, monthRect, sfCenterTop);
                }

                // KW row background
                using (var brushWeekBg = new SolidBrush(Color.FromArgb(245, 245, 245)))
                using (var penWeek = new Pen(Color.LightGray))
                {
                    var weekAreaRect = new Rectangle(0, bannerHeight, panelMonthHeader.Width, weekRowHeight);
                    g.FillRectangle(brushWeekBg, weekAreaRect);
                    g.DrawLine(penWeek, 0, bannerHeight + weekRowHeight - 1, panelMonthHeader.Width, bannerHeight + weekRowHeight - 1);
                }

                // Day header background
                using (var brushDayBg = new SolidBrush(Color.White))
                using (var penGrid = new Pen(Color.LightGray))
                {
                    var dayAreaRect = new Rectangle(0, bannerHeight + weekRowHeight, panelMonthHeader.Width, dayHeaderHeight);
                    g.FillRectangle(brushDayBg, dayAreaRect);
                    g.DrawLine(penGrid, 0, bannerHeight + weekRowHeight, panelMonthHeader.Width, bannerHeight + weekRowHeight);
                }

                // Draw days & weekdays with dynamic fonts
                using (var dayFont = new Font(this.Font.FontFamily, dayFontSize, FontStyle.Regular))
                using (var weekdayFont = new Font(this.Font.FontFamily, weekdayFontSize, FontStyle.Regular))
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

                        var dayRect = new Rectangle(cellRect.Left, cellRect.Top + 2, cellRect.Width, (int)Math.Max(10, cellRect.Height * 0.55) - 4);
                        var weekdayRect = new Rectangle(cellRect.Left, cellRect.Top + (int)(cellRect.Height * 0.55), cellRect.Width, cellRect.Height - (int)(cellRect.Height * 0.55) - 2);

                        string dayText = date.Day.ToString("00");
                        string weekdayShort = date.ToString("ddd", CultureInfo.CurrentCulture);

                        var sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString(dayText, dayFont, Brushes.Black, dayRect, sfCenter);
                        g.DrawString(weekdayShort, weekdayFont, Brushes.DarkSlateGray, weekdayRect, sfCenter);
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

                // Draw week numbers as plain centered numbers spanning Mon..Sun
                var calendar = CultureInfo.CurrentCulture.Calendar;
                var weekRule = CalendarWeekRule.FirstFourDayWeek;
                var firstDayOfWeek = DayOfWeek.Monday;

                using var weekTextBrush = Brushes.Black;
                using var weekFont = new Font(this.Font.FontFamily, Math.Max(9f, dayFontSize - 0.5f), FontStyle.Bold);
                for (int d = 0; d < daysInYear; d++)
                {
                    var date = firstOfYear.AddDays(d);
                    if (date.DayOfWeek != DayOfWeek.Monday) continue;

                    int mondayIndex = d;
                    int sundayIndex = Math.Min(daysInYear - 1, d + 6);

                    int colStart = 1 + mondayIndex;
                    int colEnd = 1 + sundayIndex;

                    Rectangle rectStart = Rectangle.Empty;
                    Rectangle rectEnd = Rectangle.Empty;
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

                    var weekRect = new Rectangle(xStart, bannerHeight + 2, Math.Min(xEnd - xStart, panelMonthHeader.Width - xStart), Math.Max(12, weekRowHeight - 4));
                    if (weekRect.Width <= 4) continue;

                    int kw;
                    try { kw = calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek); }
                    catch { kw = ((date.DayOfYear + 6) / 7); }

                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(kw.ToString(), weekFont, weekTextBrush, weekRect, sf);
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

    // Hilfsmethoden (abgerundete Rechtecke falls noch verwendet)
    static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle bounds, int radius)
        {
            using var path = RoundedRectPath(bounds, radius);
            g.FillPath(brush, path);
        }

        public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle bounds, int radius)
        {
            using var path = RoundedRectPath(bounds, radius);
            g.DrawPath(pen, path);
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundedRectPath(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int d = radius * 2;
            path.StartFigure();
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}