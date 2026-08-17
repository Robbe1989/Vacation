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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler beim initialen Laden: " + ex.Message);
                    }
                }));
            };
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

        // Header: Monatsbanner (alternierend), KW-Zeile (Zahlen, Montag als Start), Tagesspalten, Wochenend‑Shading.
        private void PanelMonthHeader_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            try
            {
                g.Clear(panelMonthHeader.BackColor);

                int year;
                try { year = (int)nudYear.Value; }
                catch { year = DateTime.Now.Year; }
                if (year < 1 || year > DateTime.MaxValue.Year) year = DateTime.Now.Year;

                var firstOfYear = new DateTime(year, 1, 1);
                int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

                // Layout: banner + weekRow + dayHeader
                int bannerHeight = Math.Max(36, panelMonthHeader.Height * 45 / 100);
                int weekRowHeight = Math.Max(18, panelMonthHeader.Height * 16 / 100);
                int dayHeaderHeight = panelMonthHeader.Height - bannerHeight - weekRowHeight;
                if (dayHeaderHeight < 12) dayHeaderHeight = 12;

                var colorOdd = Color.FromArgb(255, 250, 205);
                var colorEven = Color.FromArgb(200, 235, 255);

                using var penBanner = new Pen(Color.LightGray);
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
                            if (r.Width > 0) { rectEnd*
