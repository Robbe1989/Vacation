using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using VacationApp.Data;
using VacationApp.Models;

namespace VacationApp
{
    public partial class MainForm : Form
    {
        private const int DayColumnWidth = 28;
        private Dictionary<int, VacationType> VacationTypesCache = new Dictionary<int, VacationType>();
        private List<HolidayRange> Holidays = new List<HolidayRange>();

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

            dgvCalendar.Scroll += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.ColumnWidthChanged += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.Resize += (s, e) => panelMonthHeader.Invalidate();
            dgvCalendar.ColumnDisplayIndexChanged += (s, e) => panelMonthHeader.Invalidate();
            panelMonthHeader.Paint += PanelMonthHeader_Paint;
            dgvCalendar.RowPostPaint += DgvCalendar_RowPostPaint;

            // Erstes Laden erst nach dem Anzeigen, damit das DGV Layout/Spaltenrechtecke hat
            this.Shown += (s, e) =>
            {
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        LoadCalendar((int)nudYear.Value);
                        dgvCalendar.ClearSelection();
                        panelMonthHeader.Invalidate();
                        
                        // Scrolle zum heutigen Monat
                        ScrollToMonth(DateTime.Today.Month);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler beim initialen Laden: " + ex.Message);
                    }
                }));
            };
        }

       
        private void ScrollToMonth(int month)
        {
            try
            {
                int year = (int)nudYear.Value;
                var firstOfYear = new DateTime(year, 1, 1);
                var monthDate = new DateTime(year, month, 1);
                
                // Berechne den Spaltenindex für den ersten Tag des Monats
                int dayIndex = (int)(monthDate - firstOfYear).TotalDays;
                int columnIndex = dayIndex + 1; // +1 wegen der Mitarbeiter-Spalte

                // Scrolle zur entsprechenden Spalte
                if (columnIndex >= 0 && columnIndex < dgvCalendar.Columns.Count)
                {
                    dgvCalendar.FirstDisplayedScrollingColumnIndex = columnIndex;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Scrollen zum Monat: " + ex.Message);
            }
        }

        private async void LoadCalendar(int year)
        {
            try
            {
                // Lade Ferien asynchron (ohne zu warten) - mit aktuellem Jahr
                _ = LoadHolidays(year);

                dgvCalendar.SuspendLayout();
                dgvCalendar.Columns.Clear();
                dgvCalendar.Rows.Clear();

                // Cache Urlaubstypen mit Farben
                VacationTypesCache.Clear();
                var vacationTypes = Database.GetAllVacationTypes();
                foreach (var vt in vacationTypes)
                {
                    VacationTypesCache[vt.Id] = vt;
                }

                var employees = Database.GetAllEmployees();
                var vacations = Database.GetVacationsForYear(year);

                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
                var firstOfYear = new DateTime(year, 1, 1);
                var today = DateTime.Today;

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

                    // Färbung der Spalte basierend auf Wochenende (nicht heutiger Tag in der Datenzeile)
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        // Wochenende: Grau
                        col.DefaultCellStyle = new DataGridViewCellStyle
                        {
                            BackColor = Color.FromArgb(220, 220, 220),
                            SelectionBackColor = Color.FromArgb(220, 220, 220),
                            SelectionForeColor = Color.Black
                        };
                    }
                    else
                    {
                        // Normale Wochentage
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

                    // Dictionary um Urlaubstypen pro Tag zu tracken
                    var dayVacations = new Dictionary<int, Vacation>();

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
                            if (i >= 0)
                                dayVacations[i] = v;
                        }
                    }

                    int total = 0;
                    for (int d = 0; d < daysInYear; d++)
                    {
                        if (dayVacations.ContainsKey(d))
                        {
                            // Zeige die Abkürzung des Urlaubstyps statt "●"
                            var vacation = dayVacations[d];
                            string abbreviation = "?";
                            if (VacationTypesCache.ContainsKey(vacation.VacationTypeId))
                            {
                                abbreviation = VacationTypesCache[vacation.VacationTypeId].Abbreviation;
                            }
                            values[1 + d] = abbreviation;
                            total++;
                        }
                        else 
                            values[1 + d] = "";
                    }
                    values[1 + daysInYear] = total > 0 ? total.ToString() : "";

                    int rowIndex = dgvCalendar.Rows.Add(values);

                    if (total > 0)
                    {
                        for (int d = 0; d < daysInYear; d++)
                        {
                            if (dayVacations.ContainsKey(d))
                            {
                                var cell = dgvCalendar.Rows[rowIndex].Cells[1 + d];
                                var vacation = dayVacations[d];
                                
                                // Farbe basierend auf VacationType
                                Color vacColor = Color.LightSalmon; // Default
                                if (VacationTypesCache.ContainsKey(vacation.VacationTypeId))
                                {
                                    vacColor = VacationTypesCache[vacation.VacationTypeId].GetColor();
                                }

                                cell.Style.BackColor = vacColor;
                                cell.Style.SelectionBackColor = vacColor;
                                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                cell.Style.Font = new Font(dgvCalendar.Font.FontFamily, dgvCalendar.Font.Size - 1);
                                
                                // Setze Textfarbe basierend auf Hintergrundfarbe (für bessere Lesbarkeit)
                                if (IsColorLight(vacColor))
                                    cell.Style.ForeColor = Color.Black;
                                else
                                    cell.Style.ForeColor = Color.White;
                            }
                        }
                    }
                }

                // Ferien-Reihe wird IMMER angezeigt (auch wenn noch keine Daten geladen wurden)
                object[] holidayValues = new object[1 + daysInYear + 1];
                holidayValues[0] = "Ferien Baden-Württemberg";

                for (int d = 0; d < daysInYear; d++)
                {
                    var date = firstOfYear.AddDays(d);
                    
                    // Suche nach Ferien, die diesen Tag abdecken
                    var holiday = Holidays.FirstOrDefault(h => date >= h.StartDate && date <= h.EndDate);
                    
                    if (holiday != null)
                    {
                        // Zeige Startdatum am ersten Tag der Ferien
                        if (date == holiday.StartDate)
                        {
                            holidayValues[1 + d] = holiday.StartDate.ToString("dd.MM");
                        }
                        // Zeige Enddatum am letzten Tag der Ferien
                        else if (date == holiday.EndDate)
                        {
                            holidayValues[1 + d] = holiday.EndDate.ToString("dd.MM");
                        }
                        // Andere Tage: einfach "F" für Ferien
                        else
                        {
                            holidayValues[1 + d] = "F";
                        }
                    }
                    else
                    {
                        holidayValues[1 + d] = "";
                    }
                }

                int holidayRowIndex = dgvCalendar.Rows.Add(holidayValues);

                // Formatiere die Ferien-Reihe in rot (nur wenn Ferien vorhanden)
                for (int d = 0; d < daysInYear; d++)
                {
                    if (!string.IsNullOrEmpty(holidayValues[1 + d].ToString()))
                    {
                        var cell = dgvCalendar.Rows[holidayRowIndex].Cells[1 + d];
                        cell.Style.BackColor = Color.Red;
                        cell.Style.SelectionBackColor = Color.Red;
                        cell.Style.ForeColor = Color.White;
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        cell.Style.Font = new Font(dgvCalendar.Font.FontFamily, dgvCalendar.Font.Size - 1);
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

        /// <summary>
        /// Prüft, ob eine Farbe hell oder dunkel ist
        /// </summary>
        private bool IsColorLight(Color color)
        {
            // Berechne Helligkeit: (R*299 + G*587 + B*114) / 1000
            double brightness = (color.R * 299 + color.G * 587 + color.B * 114) / 1000.0;
            return brightness > 128; // Wenn heller als 128, ist es eine helle Farbe
        }

        // Header: Monatsbanner (alternierend), Tagesspalten, Wochenend‑Shading; KW wurden entfernt.
        private void PanelMonthHeader_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
            try
            {
                g.Clear(panelMonthHeader.BackColor);

                int year;
                try { year = (int)nudYear.Value; }
                catch { year = DateTime.Now.Year; }
                if (year < 1 || year > DateTime.MaxValue.Year) year = DateTime.Now.Year;

                var firstOfYear = new DateTime(year, 1, 1);
                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
                var today = DateTime.Today;

                int bannerHeight = 35;
                int kwHeight = 20;
                int dayHeaderHeight = panelMonthHeader.Height - bannerHeight - kwHeight;

                var colorOdd = Color.FromArgb(255, 250, 205);
                var colorEven = Color.FromArgb(200, 235, 255);

                using var penBanner = new Pen(Color.FromArgb(180, 180, 180));
                using var sfCenterTop = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                // Monatsbanner (alternierend)
                for (int month = 1; month <= 12; month++)
                {
                    DateTime monthStart;
                    DateTime monthEnd;
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
                        try
                        {
                            var r = dgvCalendar.GetColumnDisplayRectangle(c, true);
                            if (r.Width > 0) { rectStart = r; break; }
                        }
                        catch { }
                    }
                    for (int c = colEnd; c >= colStart; c--)
                    {
                        try
                        {
                            var r = dgvCalendar.GetColumnDisplayRectangle(c, true);
                            if (r.Width > 0) { rectEnd = r; break; }
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

                    var fillColor = (month % 2 == 0) ? colorEven : colorOdd;
                    using var brushBanner = new SolidBrush(fillColor);

                    g.FillRectangle(brushBanner, monthRect);
                    g.DrawRectangle(penBanner, monthRect);
                    var monthName = new DateTime(year, month, 1).ToString("MMMM", CultureInfo.CurrentCulture);
                    using var bigFont = new Font(this.Font.FontFamily, Math.Max(12f, this.Font.Size + 2f), FontStyle.Bold);
                    g.DrawString(monthName, bigFont, Brushes.Black, monthRect, sfCenterTop);
                }

                // Tag-Header-Grund
                // Kalenderwochen
                using (var kwFont = new Font(this.Font.FontFamily, 8f, FontStyle.Bold))
                using (var kwBrush = new SolidBrush(Color.FromArgb(220, 220, 220)))
                using (var kwPen = new Pen(Color.Gray))
                {
                    int currentKw = -1;
                    int kwStartX = -1;

                    for (int d = 0; d < daysInYear; d++)
                    {
                        DateTime date = firstOfYear.AddDays(d);

                        int kw = ISOWeek.GetWeekOfYear(date);

                        Rectangle rect;
                        try
                        {
                            rect = dgvCalendar.GetColumnDisplayRectangle(1 + d, true);
                        }
                        catch
                        {
                            continue;
                        }

                        if (rect.Width <= 0)
                            continue;

                        if (currentKw == -1)
                        {
                            currentKw = kw;
                            kwStartX = rect.Left;
                        }

                        bool kwEnds =
                            d == daysInYear - 1 ||
                            ISOWeek.GetWeekOfYear(firstOfYear.AddDays(d + 1)) != currentKw;

                        if (kwEnds)
                        {
                            int width = rect.Right - kwStartX;

                            Rectangle kwRect = new Rectangle(
                                kwStartX,
                                bannerHeight,
                                width,
                                kwHeight);

                            g.FillRectangle(kwBrush, kwRect);

                            using var kwBorderPen = new Pen(Color.DimGray, 1.5f);

                            // obere Linie
                            g.DrawLine(
                                kwPen,
                                kwRect.Left,
                                kwRect.Top,
                                kwRect.Right - 1,
                                kwRect.Top);

                            // untere Linie
                            g.DrawLine(
                                kwPen,
                                kwRect.Left,
                                kwRect.Bottom - 1,
                                kwRect.Right - 1,
                                kwRect.Bottom - 1);

                            // linke dicke KW-Trennlinie
                            g.DrawLine(
                                kwBorderPen,
                                kwRect.Left,
                                bannerHeight,
                                kwRect.Left,
                                panelMonthHeader.Height);

                            // rechte dicke KW-Trennlinie
                            g.DrawLine(
                                kwBorderPen,
                                kwRect.Right - 1,
                                bannerHeight,
                                kwRect.Right - 1,
                                panelMonthHeader.Height);

                            g.DrawString(
                                $"KW {currentKw}",
                                kwFont,
                                Brushes.Black,
                                kwRect,
                                new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center
                                });

                            currentKw =
                                d < daysInYear - 1
                                ? ISOWeek.GetWeekOfYear(firstOfYear.AddDays(d + 1))
                                : -1;

                            kwStartX = rect.Right;
                        }
                    }
                }

                // Tag-Header-Grund
                using (var brushDayBg = new SolidBrush(Color.White))
                using (var penGrid = new Pen(Color.FromArgb(180, 180, 180)))
                {
                    var dayAreaRect = new Rectangle(
                        0,
                        bannerHeight + kwHeight,
                        panelMonthHeader.Width,
                        dayHeaderHeight);
                    g.FillRectangle(brushDayBg, dayAreaRect);
                    g.DrawLine(penGrid,
                        0,
                        bannerHeight + kwHeight,
                        panelMonthHeader.Width,
                        bannerHeight + kwHeight);
                }

                // Tage + Wochentage + Wochenendshading
                using (var smallFont = new Font(this.Font.FontFamily, Math.Max(8f, this.Font.Size - 1f)))
                using (var weekdayFont = new Font(this.Font.FontFamily, Math.Max(7f, this.Font.Size - 3f)))
                using (var penGridLines = new Pen(Color.Gray))
                using (var brushWeekend = new SolidBrush(Color.FromArgb(220, 220, 220)))
                using (var brushToday = new SolidBrush(Color.LimeGreen))
                {
                    for (int d = 0; d < daysInYear; d++)
                    {
                        int colIndex = 1 + d;
                        Rectangle rect;
                        try { rect = dgvCalendar.GetColumnDisplayRectangle(colIndex, true); }
                        catch { continue; }

                        if (rect.Width == 0 && rect.Right <= 0) continue;
                        if (rect.Width == 0 && rect.Left >= dgvCalendar.ClientSize.Width) continue;

                        int x = rect.Left;
                        int w = rect.Width > 0 ? rect.Width : DayColumnWidth;
                        var cellRect = new Rectangle(
                            x,
                            bannerHeight + kwHeight,
                            w,
                            dayHeaderHeight);

                        var date = firstOfYear.AddDays(d);
                        
                        // Färbe nur den heutigen Tag grün (in der Tageszeile des Headers)
                        if (date == today)
                        {
                            g.FillRectangle(brushToday, cellRect);
                        }
                        // Färbe Wochenenden grau
                        else if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            g.FillRectangle(brushWeekend, cellRect);
                        }

                        g.DrawRectangle(
                            penGridLines,
                            cellRect.Left,
                            cellRect.Top,
                            cellRect.Width - 1,
                            cellRect.Height - 1);

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
                                g.DrawLine(
                                    penGridLines,
                                    xRight - 1,
                                    bannerHeight,
                                    xRight - 1,
                                    panelMonthHeader.Height);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PanelMonthHeader_Paint error: " + ex);
            }
        }

        private void DgvCalendar_RowPostPaint(
            object? sender,
            DataGridViewRowPostPaintEventArgs e)
        {
            int year = (int)nudYear.Value;
            DateTime firstOfYear = new DateTime(year, 1, 1);
            int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

            using var kwPen = new Pen(Color.Black, 2f);

            for (int d = 0; d < daysInYear; d++)
            {
                DateTime date = firstOfYear.AddDays(d);

                bool isKwStart =
                    d == 0 ||
                    date.DayOfWeek == DayOfWeek.Monday;

                if (!isKwStart)
                    continue;

                int colIndex = d + 1;

                Rectangle rect;

                try
                {
                    rect = dgvCalendar.GetCellDisplayRectangle(
                        colIndex,
                        e.RowIndex,
                        true);
                }
                catch
                {
                    continue;
                }

                if (rect.Width <= 0)
                    continue;

                e.Graphics.DrawLine(
                    kwPen,
                    rect.Left,
                    rect.Top,
                    rect.Left,
                    rect.Bottom);
            }
        }

        private void DgvCalendar_Paint(object? sender, PaintEventArgs e)
        {
            int year = (int)nudYear.Value;
            DateTime firstOfYear = new DateTime(year, 1, 1);
            int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

            using var kwPen = new Pen(Color.Black, 2f);

            for (int d = 0; d < daysInYear; d++)
            {
                DateTime date = firstOfYear.AddDays(d);

                bool isKwStart =
                    d == 0 ||
                    date.DayOfWeek == DayOfWeek.Monday;

                if (!isKwStart)
                    continue;

                int colIndex = d + 1;

                Rectangle rect;

                try
                {
                    rect = dgvCalendar.GetColumnDisplayRectangle(
                        colIndex,
                        true);
                }
                catch
                {
                    continue;
                }

                if (rect.Width <= 0)
                    continue;

                e.Graphics.DrawLine(
                    kwPen,
                    rect.Left,
                    0,
                    rect.Left,
                    dgvCalendar.DisplayRectangle.Height);
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
            
            var menuVacationTypes = new ToolStripMenuItem("Urlaubstypen");
            menuVacationTypes.Click += (s, e) =>
            {
                using var f = new Forms.VacationTypesForm();
                f.ShowDialog(this);
                // Reload Calendar um neue Farben zu zeigen
                LoadCalendar((int)nudYear.Value);
            };
            menuOptions.DropDownItems.Add(menuVacationTypes);

            menu.Items.Add(menuOptions);
        }
    }
}
