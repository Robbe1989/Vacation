using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using VacationApp.Models;

public static partial class Database
{
    // Ergänze in Init() die Tabelle:
    public static void Init()
    {
        // vorhandene Init-Logik (erzeugt Employees etc.)
        using var conn = GetConnection();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Departments (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                UseFte INTEGER NOT NULL DEFAULT 1,
                FteOptionsRaw TEXT
            );";
        cmd.ExecuteNonQuery();

        // ... restliche Init
    }

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
                Name = rdr.GetString(1),
                UseFte = rdr.GetInt32(2) != 0,
                FteOptionsRaw = rdr.IsDBNull(3) ? "" : rdr.GetString(3)
            });
        }
        return list;
    }

    public static Department? GetDepartmentById(int id)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, UseFte, FteOptionsRaw FROM Departments WHERE Id = @id LIMIT 1;";
        cmd.Parameters.AddWithValue("@id", id);
        using var rdr = cmd.ExecuteReader();
        if (rdr.Read())
        {
            return new Department
            {
                Id = rdr.GetInt32(0),
                Name = rdr.GetString(1),
                UseFte = rdr.GetInt32(2) != 0,
                FteOptionsRaw = rdr.IsDBNull(3) ? "" : rdr.GetString(3)
            };
        }
        return null;
    }

    public static int AddDepartment(Department d)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Departments (Name, UseFte, FteOptionsRaw) VALUES (@name, @useFte, @opts); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@name", d.Name);
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
        cmd.Parameters.AddWithValue("@name", d.Name);
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