private void LoadCalendar(int year)
{
    try
    {
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