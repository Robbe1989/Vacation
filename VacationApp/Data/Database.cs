using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using VacationApp.Models;

namespace VacationApp.Data
{
    public static class Database
    {
        private static readonly string DbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vacation.db");

        private static string ConnectionString =>
            $"Data Source={DbFile};Version=3;Journal Mode=WAL;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        public static void Init()
        {
            try
            {
                var dir = Path.GetDirectoryName(DbFile);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using var conn = GetConnection();
                conn.Open();

                var createEmployees = @"
                    CREATE TABLE IF NOT EXISTS Employees (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Email TEXT,
                        Department TEXT,
                        StartDate TEXT,
                        VacationDays INTEGER NOT NULL DEFAULT 20
                    );";

                var createDepartments = @"
                    CREATE TABLE IF NOT EXISTS Departments (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL
                    );";

                var createVacationTypes = @"
                    CREATE TABLE IF NOT EXISTS VacationTypes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Abbreviation TEXT NOT NULL UNIQUE,
                        Name TEXT NOT NULL
                    );";

                var createVacations = @"
                    CREATE TABLE IF NOT EXISTS Vacations (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EmployeeId INTEGER NOT NULL,
                        StartDate TEXT NOT NULL,
                        EndDate TEXT NOT NULL,
                        VacationTypeId INTEGER,
                        Comment TEXT,
                        FOREIGN KEY(EmployeeId) REFERENCES Employees(Id),
                        FOREIGN KEY(VacationTypeId) REFERENCES VacationTypes(Id)
                    );";

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = createEmployees;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = createDepartments;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = createVacationTypes;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = createVacations;
                    cmd.ExecuteNonQuery();
                }

                // Stelle sicher, dass Standardurlaubstypen existieren
                EnsureDefaultVacationTypes(conn);
                
                // Migriere Farbe für VacationTypes falls nötig
                using (var checkCmd = conn.CreateCommand())
                {
                    checkCmd.CommandText = "PRAGMA table_info(VacationTypes);";
                    using var reader = checkCmd.ExecuteReader();
                    bool hasColorHex = false;
                    while (reader.Read())
                    {
                        var colName = reader["name"]?.ToString() ?? "";
                        if (string.Equals(colName, "ColorHex", StringComparison.OrdinalIgnoreCase))
                        {
                            hasColorHex = true;
                            break;
                        }
                    }
                    reader.Close();

                    if (!hasColorHex)
                    {
                        using var alter = conn.CreateCommand();
                        alter.CommandText = "ALTER TABLE VacationTypes ADD COLUMN ColorHex TEXT DEFAULT '#FFA500';";
                        alter.ExecuteNonQuery();
                    }
                }

                // Ensure VacationDays column exists (legacy migrations)
                using (var checkCmd = conn.CreateCommand())
                {
                    checkCmd.CommandText = "PRAGMA table_info(Employees);";
                    using var reader = checkCmd.ExecuteReader();
                    bool hasVacationDays = false;
                    while (reader.Read())
                    {
                        var colName = reader["name"]?.ToString() ?? "";
                        if (string.Equals(colName, "VacationDays", StringComparison.OrdinalIgnoreCase))
                        {
                            hasVacationDays = true;
                            break;
                        }
                    }
                    reader.Close();

                    if (!hasVacationDays)
                    {
                        using var alter = conn.CreateCommand();
                        alter.CommandText = "ALTER TABLE Employees ADD COLUMN VacationDays INTEGER NOT NULL DEFAULT 20;";
                        alter.ExecuteNonQuery();
                    }
                }

                // Migriere alte Vakationen falls nötig (VacationTypeId hinzufügen)
                using (var checkCmd = conn.CreateCommand())
                {
                    checkCmd.CommandText = "PRAGMA table_info(Vacations);";
                    using var reader = checkCmd.ExecuteReader();
                    bool hasVacationTypeId = false;
                    while (reader.Read())
                    {
                        var colName = reader["name"]?.ToString() ?? "";
                        if (string.Equals(colName, "VacationTypeId", StringComparison.OrdinalIgnoreCase))
                        {
                            hasVacationTypeId = true;
                            break;
                        }
                    }
                    reader.Close();

                    if (!hasVacationTypeId)
                    {
                        using var alter = conn.CreateCommand();
                        alter.CommandText = "ALTER TABLE Vacations ADD COLUMN VacationTypeId INTEGER DEFAULT 1;";
                        alter.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database_error.log");
                string errorMsg = $"Database.Init() Fehler:\n{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n{ex.Message}\n\nStackTrace:\n{ex.StackTrace}";
                try
                {
                    File.WriteAllText(logPath, errorMsg);
                }
                catch { }
                throw;
            }
        }

        // für Urlaubstypen
        private static void EnsureDefaultVacationTypes(SQLiteConnection conn)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT OR IGNORE INTO VacationTypes (Id, Abbreviation, Name, ColorHex) 
                    VALUES 
                        (1, 'U', 'Urlaub', '#87CEEB'),
                        (2, 'K', 'Krankheit', '#FFB6C6'),
                        (3, 'A', 'Abwesenheit', '#D3D3D3')
                ";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database_error.log");
                string errorMsg = $"EnsureDefaultVacationTypes() Fehler:\n{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n{ex.Message}\n\nStackTrace:\n{ex.StackTrace}";
                try
                {
                    File.WriteAllText(logPath, errorMsg);
                }
                catch { }
                throw;
            }
        }

        // Employees CRUD
        public static List<Employee> GetAllEmployees()
        {
            var list = new List<Employee>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Email, Department, StartDate, VacationDays FROM Employees ORDER BY Name;";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var e = new Employee
                {
                    Id = rdr.IsDBNull(0) ? 0 : Convert.ToInt32(rdr[0]),
                    Name = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                    Email = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                    Department = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                    StartDate = rdr.IsDBNull(4) ? DateTime.Today : DateTime.TryParse(rdr.GetString(4), out var dt) ? dt : DateTime.Today,
                    VacationDays = rdr.IsDBNull(5) ? 20 : Convert.ToInt32(rdr[5])
                };
                list.Add(e);
            }
            return list;
        }

        public static int AddEmployee(Employee e)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Employees (Name, Email, Department, StartDate, VacationDays)
                                VALUES (@name,@email,@dept,@start,@vacationDays);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@name", e.Name ?? "");
            cmd.Parameters.AddWithValue("@email", e.Email ?? "");
            cmd.Parameters.AddWithValue("@dept", e.Department ?? "");
            cmd.Parameters.AddWithValue("@start", e.StartDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@vacationDays", e.VacationDays);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public static void UpdateEmployee(Employee e)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Employees
                                SET Name=@name, Email=@email, Department=@dept, StartDate=@start, VacationDays=@vacationDays
                                WHERE Id=@id;";
            cmd.Parameters.AddWithValue("@name", e.Name ?? "");
            cmd.Parameters.AddWithValue("@email", e.Email ?? "");
            cmd.Parameters.AddWithValue("@dept", e.Department ?? "");
            cmd.Parameters.AddWithValue("@start", e.StartDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@vacationDays", e.VacationDays);
            cmd.Parameters.AddWithValue("@id", e.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteEmployee(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Employees WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // Departments CRUD
        public static List<Department> GetAllDepartments()
        {
            var list = new List<Department>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name FROM Departments ORDER BY Name;";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Department
                {
                    Id = rdr.IsDBNull(0) ? 0 : Convert.ToInt32(rdr[0]),
                    Name = rdr.IsDBNull(1) ? "" : rdr.GetString(1)
                });
            }
            return list;
        }

        public static int AddDepartment(Department d)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Departments (Name) VALUES (@name); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@name", d.Name ?? "");
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public static void UpdateDepartment(Department d)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Departments SET Name=@name WHERE Id=@id;";
            cmd.Parameters.AddWithValue("@name", d.Name ?? "");
            cmd.Parameters.AddWithValue("@id", d.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteDepartment(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Departments WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // Vacation Types CRUD
        public static List<VacationType> GetAllVacationTypes()
        {
            var list = new List<VacationType>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Abbreviation, Name, COALESCE(ColorHex, '#FFA500') FROM VacationTypes ORDER BY Name;";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new VacationType
                {
                    Id = rdr.IsDBNull(0) ? 0 : Convert.ToInt32(rdr[0]),
                    Abbreviation = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                    Name = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                    ColorHex = rdr.IsDBNull(3) ? "#FFA500" : rdr.GetString(3)
                });
            }
            return list;
        }

        public static int AddVacationType(VacationType vt)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO VacationTypes (Abbreviation, Name, ColorHex) VALUES (@abbr, @name, @color); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@abbr", vt.Abbreviation ?? "");
            cmd.Parameters.AddWithValue("@name", vt.Name ?? "");
            cmd.Parameters.AddWithValue("@color", vt.ColorHex ?? "#FFA500");
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public static void UpdateVacationType(VacationType vt)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE VacationTypes SET Abbreviation=@abbr, Name=@name, ColorHex=@color WHERE Id=@id;";
            cmd.Parameters.AddWithValue("@abbr", vt.Abbreviation ?? "");
            cmd.Parameters.AddWithValue("@name", vt.Name ?? "");
            cmd.Parameters.AddWithValue("@color", vt.ColorHex ?? "#FFA500");
            cmd.Parameters.AddWithValue("@id", vt.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteVacationType(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM VacationTypes WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // Vacations CRUD
        public static List<Vacation> GetVacationsForYear(int year)
        {
            var list = new List<Vacation>();
            var firstDay = new DateTime(year, 1, 1);
            var lastDay = new DateTime(year, 12, 31);
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, EmployeeId, StartDate, EndDate, VacationTypeId, Comment
                                FROM Vacations
                                WHERE date(StartDate) <= @last AND date(EndDate) >= @first
                                ORDER BY date(StartDate);";
            cmd.Parameters.AddWithValue("@first", firstDay.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@last", lastDay.ToString("yyyy-MM-dd"));
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var v = new Vacation
                {
                    Id = rdr.IsDBNull(0) ? 0 : Convert.ToInt32(rdr[0]),
                    EmployeeId = rdr.IsDBNull(1) ? 0 : Convert.ToInt32(rdr[1]),
                    StartDate = rdr.IsDBNull(2) ? DateTime.Today : DateTime.Parse(rdr.GetString(2)),
                    EndDate = rdr.IsDBNull(3) ? DateTime.Today : DateTime.Parse(rdr.GetString(3)),
                    VacationTypeId = rdr.IsDBNull(4) ? 1 : Convert.ToInt32(rdr[4]),
                    Comment = rdr.IsDBNull(5) ? "" : rdr.GetString(5)
                };
                list.Add(v);
            }
            return list;
        }

        public static List<Vacation> GetVacationsForEmployee(int employeeId, int year)
        {
            var list = new List<Vacation>();
            var firstDay = new DateTime(year, 1, 1);
            var lastDay = new DateTime(year, 12, 31);
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, EmployeeId, StartDate, EndDate, VacationTypeId, Comment
                                FROM Vacations
                                WHERE EmployeeId = @eid
                                  AND date(StartDate) <= @last
                                  AND date(EndDate) >= @first
                                ORDER BY date(StartDate);";
            cmd.Parameters.AddWithValue("@eid", employeeId);
            cmd.Parameters.AddWithValue("@first", firstDay.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@last", lastDay.ToString("yyyy-MM-dd"));
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var v = new Vacation
                {
                    Id = rdr.IsDBNull(0) ? 0 : Convert.ToInt32(rdr[0]),
                    EmployeeId = rdr.IsDBNull(1) ? 0 : Convert.ToInt32(rdr[1]),
                    StartDate = rdr.IsDBNull(2) ? DateTime.Today : DateTime.Parse(rdr.GetString(2)),
                    EndDate = rdr.IsDBNull(3) ? DateTime.Today : DateTime.Parse(rdr.GetString(3)),
                    VacationTypeId = rdr.IsDBNull(4) ? 1 : Convert.ToInt32(rdr[4]),
                    Comment = rdr.IsDBNull(5) ? "" : rdr.GetString(5)
                };
                list.Add(v);
            }
            return list;
        }

        public static int AddVacation(Vacation v)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Vacations (EmployeeId, StartDate, EndDate, VacationTypeId, Comment)
                                VALUES (@eid,@start,@end,@vtid,@comment);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@eid", v.EmployeeId);
            cmd.Parameters.AddWithValue("@start", v.StartDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@end", v.EndDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@vtid", v.VacationTypeId);
            cmd.Parameters.AddWithValue("@comment", v.Comment ?? "");
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public static void UpdateVacation(Vacation v)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Vacations
                                SET EmployeeId=@eid, StartDate=@start, EndDate=@end, VacationTypeId=@vtid, Comment=@comment
                                WHERE Id=@id;";
            cmd.Parameters.AddWithValue("@eid", v.EmployeeId);
            cmd.Parameters.AddWithValue("@start", v.StartDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@end", v.EndDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@vtid", v.VacationTypeId);
            cmd.Parameters.AddWithValue("@comment", v.Comment ?? "");
            cmd.Parameters.AddWithValue("@id", v.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteVacation(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Vacations WHERE Id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
