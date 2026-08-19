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
                        try { var r = dgvCalendar.GetColumnDisplayRectangle(c, true*
