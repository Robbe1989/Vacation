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
            var dir = Path.GetDirectoryName(DbFile);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using var conn = GetConnection();
            conn.Open();

            // Employees now includes VacationDays
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

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = createEmployees;
                cmd.ExecuteNonQuery();

                cmd.CommandText = createDepartments;
                cmd.ExecuteNonQuery();
            }

            // Ensure VacationDays column exists in legacy DBs (safe no-op if present)
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
    }
}