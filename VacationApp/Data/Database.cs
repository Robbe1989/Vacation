using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using VacationApp.Models;

namespace VacationApp.Data
{
    public static class Database// Datei: VacationApp/Data/Database.cs
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
            // Ensure folder exists
            var dir = Path.GetDirectoryName(DbFile);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Open connection first, then create/alter tables
            using var conn = GetConnection();
            conn.Open();

            // Create Employees with UseFte included (safe for fresh DBs)
            var createEmployees = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT,
                    Department TEXT,
                    Fte REAL NOT NULL DEFAULT 1.0,
                    StartDate TEXT,
                    UseFte INTEGER NOT NULL DEFAULT 1
                );";

            var createDepartments = @"
                CREATE TABLE IF NOT EXISTS Departments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    UseFte INTEGER NOT NULL DEFAULT 1,
                    FteOptionsRaw TEXT
                );";

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = createEmployees;
                cmd.ExecuteNonQuery();

                cmd.CommandText = createDepartments;
                cmd.ExecuteNonQuery();
            }

            // If existing DB lacked UseFte in Employees, ensure column exists (safe no-op if present)
            using (var checkCmd = conn.CreateCommand())
            {
                checkCmd.CommandText = "PRAGMA table_info(Employees);";
                using var reader = checkCmd.ExecuteReader();
                bool hasUseFte = false;
                while (reader.Read())
                {
                    var colName = reader["name"]?.ToString() ?? "";
                    if (string.Equals(colName, "UseFte", StringComparison.OrdinalIgnoreCase))
                    {
                        hasUseFte = true;
                        break;
                    }
                }
                reader.Close();

                if (!hasUseFte)
                {
                    using var alter = conn.CreateCommand();
                    alter.CommandText = "ALTER TABLE Employees ADD COLUMN UseFte INTEGER NOT NULL DEFAULT 1;";
                    alter.ExecuteNonQuery();
                }
            }
        }

        // Employees CRUD
        public static List<Employee> GetAllEmployees()
        {
            var list = new List<Employee>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Email, Department, Fte, StartDate, UseFte FROM Employees ORDER BY Name;";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var e = new Employee();
                e.Id = rdr.IsDBNull(0) ? 0 : Convert.ToInt32(rdr[0]);
                e.Name = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
                e.Email = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
                e.Department = rdr.IsDBNull(3) ? "" : rdr.GetString(3);
                e.Fte = rdr.IsDBNull(4) ? 1.0 : Convert.ToDouble(rdr[4], CultureInfo.InvariantCulture);
                if (!rdr.IsDBNull(5))
                {
                    if (DateTime.TryParse(rdr.GetString(5), out var dt))
                        e.StartDate = dt;
                    else
                        e.StartDate = DateTime.Today;
                }
                else
                {
                    e.StartDate = DateTime.Today;
                }
                e.UseFte = !rdr.IsDBNull(6) && Convert.ToInt32(rdr[6]) != 0;
                list.Add(e);
            }
            return list;
        }

        public static int AddEmployee(Employee e)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Employees (Name, Email, Department, Fte, StartDate, UseFte)
                                VALUES (@name,@email,@dept,@fte,@start,@useFte);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@name", e.Name ?? "");
            cmd.Parameters.AddWithValue("@email", e.Email ?? "");
            cmd.Parameters.AddWithValue("@dept", e.Department ?? "");
            cmd.Parameters.AddWithValue("@fte", e.Fte);
            cmd.Parameters.AddWithValue("@start", e.StartDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@useFte", e.UseFte ? 1 : 0);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public static void UpdateEmployee(Employee e)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Employees
                                SET Name=@name, Email=@email, Department=@dept, Fte=@fte, StartDate=@start, UseFte=@useFte
                                WHERE Id=@id;";
            cmd.Parameters.AddWithValue("@name", e.Name ?? "");
            cmd.Parameters.AddWithValue("@email", e.Email ?? "");
            cmd.Parameters.AddWithValue("@dept", e.Department ?? "");
            cmd.Parameters.AddWithValue("@fte", e.Fte);
            cmd.Parameters.AddWithValue("@start", e.StartDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@useFte", e.UseFte ? 1 : 0);
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
            cmd.CommandText = "SELECT Id, Name, UseFte, FteOptionsRaw FROM Departments ORDER BY Name;";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Department
                {
                    Id = rdr.IsDBNull(0) ? 0 : Convert.ToInt32(rdr[0]),
                    Name = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                    UseFte = !rdr.IsDBNull(2) && Convert.ToInt32(rdr[2]) != 0,
                    FteOptionsRaw = rdr.IsDBNull(3) ? "" : rdr.GetString(3)
                });
            }
            return list;
        }

        public static int AddDepartment(Department d)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Departments (Name, UseFte, FteOptionsRaw) VALUES (@name,@useFte,@opts); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@name", d.Name ?? "");
            cmd.Parameters.AddWithValue("@useFte", d.UseFte ? 1 : 0);
            cmd.Parameters.AddWithValue("@opts", d.FteOptionsRaw ?? "");
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public static void UpdateDepartment(Department d)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Departments SET Name=@name, UseFte=@useFte, FteOptionsRaw=@opts WHERE Id=@id;";
            cmd.Parameters.AddWithValue("@name", d.Name ?? "");
            cmd.Parameters.AddWithValue("@useFte", d.UseFte ? 1 : 0);
            cmd.Parameters.AddWithValue("@opts", d.FteOptionsRaw ?? "");
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
    }
}
    {
        // Pfad zur DB-Datei (im App-Verzeichnis)
        private static readonly string DbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vacation.db");

        private static string ConnectionString =>
            $"Data Source={DbFile};Version=3;Journal Mode=WAL;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        public static void Init()
        {

// Nach Erstellen der Tabellen (innerhalb der offenen Verbindung)
using var checkCmd = conn.CreateCommand();
checkCmd.CommandText = "PRAGMA table_info(Employees);";
using var reader = checkCmd.ExecuteReader();
bool hasUseFte = false;
while (reader.Read())
{
    var colName = reader.GetString(reader.GetOrdinal("name"));
    if (string.Equals(colName, "UseFte", StringComparison.OrdinalIgnoreCase))
    {
        hasUseFte = true;
        break;
    }
}
reader.Close();

if (!hasUseFte)
{
    using var alter = conn.CreateCommand();
    alter.CommandText = "ALTER TABLE Employees ADD COLUMN UseFte INTEGER NOT NULL DEFAULT 1;";
    alter.ExecuteNonQuery();
}
            // ensure folder exists (normally base dir exists)
            var dir = Path.GetDirectoryName(DbFile);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var createEmployees = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT,
                    Department TEXT,
                    Fte REAL NOT NULL DEFAULT 1.0,
                    StartDate TEXT
                );";

            var createDepartments = @"
                CREATE TABLE IF NOT EXISTS Departments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    UseFte INTEGER NOT NULL DEFAULT 1,
                    FteOptionsRaw TEXT
                );";

            using var conn = GetConnection();
            conn.Open();
            using var t = conn.BeginTransaction();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = createEmployees;
                cmd.ExecuteNonQuery();

                cmd.CommandText = createDepartments;
                cmd.ExecuteNonQuery();
            }
            t.Commit();
        }

        // Employees CRUD (einfach)
        public static List<Employee> GetAllEmployees()
        {
            var list = new List<Employee>();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Email, Department, Fte, StartDate FROM Employees ORDER BY Name;";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
// Auszug beim Lesen
Employee e = new Employee
{
    Id = rdr.GetInt32(0),
    Name = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
    Email = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
    Department = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
    Fte = rdr.IsDBNull(4) ? 1.0 : rdr.GetDouble(4),
    StartDate = rdr.IsDBNull(5) ? DateTime.Today : DateTime.Parse(rdr.GetString(5)),
    UseFte = !rdr.IsDBNull(6) && rdr.GetInt32(6) != 0
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
            cmd.CommandText = @"INSERT INTO Employees (Name, Email, Department, Fte, StartDate, UseFte)
VALUES (@name,@email,@dept,@fte,@start,@useFte);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@name", e.Name ?? "");
            cmd.Parameters.AddWithValue("@email", e.Email ?? "");
            cmd.Parameters.AddWithValue("@dept", e.Department ?? "");
            cmd.Parameters.AddWithValue("@fte", e.Fte);
            cmd.Parameters.AddWithValue("@start", e.StartDate.ToString("yyyy-MM-dd"));
            var id = Convert.ToInt32(cmd.ExecuteScalar());

            return id;
        }

        public static void UpdateEmployee(Employee e)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Employees SET Name=@name, Email=@email, Department=@dept, Fte=@fte, StartDate=@start WHERE Id=@id;";
            cmd.Parameters.AddWithValue("@name", e.Name ?? "");
            cmd.Parameters.AddWithValue("@email", e.Email ?? "");
            cmd.Parameters.AddWithValue("@dept", e.Department ?? "");
            cmd.Parameters.AddWithValue("@fte", e.Fte);
            cmd.Parameters.AddWithValue("@start", e.StartDate.ToString("yyyy-MM-dd"));
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
            cmd.CommandText = "SELECT Id, Name, UseFte, FteOptionsRaw FROM Departments ORDER BY Name;";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Department
                {
                    Id = rdr.GetInt32(0),
                    Name = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                    UseFte = !rdr.IsDBNull(2) && rdr.GetInt32(2) != 0,
                    FteOptionsRaw = rdr.IsDBNull(3) ? "" : rdr.GetString(3)
                });
            }
            return list;
        }

        public static int AddDepartment(Department d)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Departments (Name, UseFte, FteOptionsRaw) VALUES (@name,@useFte,@opts); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@name", d.Name ?? "");
            cmd.Parameters.AddWithValue("@useFte", d.UseFte ? 1 : 0);
            cmd.Parameters.AddWithValue("@opts", d.FteOptionsRaw ?? "");
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public static void UpdateDepartment(Department d)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Departments SET Name=@name, UseFte=@useFte, FteOptionsRaw=@opts WHERE Id=@id;";
            cmd.Parameters.AddWithValue("@name", d.Name ?? "");
            cmd.Parameters.AddWithValue("@useFte", d.UseFte ? 1 : 0);
            cmd.Parameters.AddWithValue("@opts", d.FteOptionsRaw ?? "");
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
    }
}