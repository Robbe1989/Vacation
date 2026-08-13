using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using VacationApp.Models;

namespace VacationApp.Data
{
    public static class Database
    {
        private static string DbFile => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vacation.db");
        private static string ConnectionString => $"Data Source={DbFile};Version=3;";

        public static void Init()
        {
            bool exists = System.IO.File.Exists(DbFile);
            if (!exists)
            {
                SQLiteConnection.CreateFile(DbFile);
            }

            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            string createTableSql = @"
CREATE TABLE IF NOT EXISTS Employees (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Email TEXT,
    Department TEXT,
    StartDate TEXT,
    Fte REAL
);";
            using var cmd = new SQLiteCommand(createTableSql, conn);
            cmd.ExecuteNonQuery();
        }

        public static List<Employee> GetAllEmployees()
        {
            var list = new List<Employee>();
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, Name, Email, Department, StartDate, Fte FROM Employees ORDER BY Name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Employee
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = Convert.ToString(reader["Name"]) ?? "",
                    Email = Convert.ToString(reader["Email"]) ?? "",
                    Department = Convert.ToString(reader["Department"]) ?? "",
                    StartDate = DateTime.TryParse(Convert.ToString(reader["StartDate"]), out var dt) ? dt : DateTime.Today,
                    Fte = reader["Fte"] != DBNull.Value ? Convert.ToDouble(reader["Fte"]) : 1.0
                });
            }
            return list;
        }

        public static int AddEmployee(Employee e)
        {
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("INSERT INTO Employees (Name, Email, Department, StartDate, Fte) VALUES (@Name, @Email, @Department, @StartDate, @Fte); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@Name", e.Name);
            cmd.Parameters.AddWithValue("@Email", e.Email);
            cmd.Parameters.AddWithValue("@Department", e.Department);
            cmd.Parameters.AddWithValue("@StartDate", e.StartDate.ToString("o"));
            cmd.Parameters.AddWithValue("@Fte", e.Fte);
            var id = cmd.ExecuteScalar();
            return Convert.ToInt32(id);
        }

        public static void UpdateEmployee(Employee e)
        {
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE Employees SET Name=@Name, Email=@Email, Department=@Department, StartDate=@StartDate, Fte=@Fte WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Name", e.Name);
            cmd.Parameters.AddWithValue("@Email", e.Email);
            cmd.Parameters.AddWithValue("@Department", e.Department);
            cmd.Parameters.AddWithValue("@StartDate", e.StartDate.ToString("o"));
            cmd.Parameters.AddWithValue("@Fte", e.Fte);
            cmd.Parameters.AddWithValue("@Id", e.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteEmployee(int id)
        {
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM Employees WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
