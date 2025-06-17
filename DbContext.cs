﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace whatsapp_clone_backend;    

public class DbContext
{
    private readonly string _connectionString;

    public DbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    // For INSERT, UPDATE, DELETE
    public int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
    {
        try
        {
            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                AddParameters(cmd, parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            return -1;
        }
    }

    // For SELECT queries returning List<T>
    public List<T> ExecuteQuery<T>(string query, Dictionary<string, object> parameters = null) where T : new()
    {
        try
        {
            var result = new List<T>();

            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                AddParameters(cmd, parameters);
                conn.Open();


                using (var reader = cmd.ExecuteReader())
                {
                    var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    while (reader.Read())
                    {
                        var obj = new T();
                        foreach (var prop in props)
                        {
                            if (!reader.HasColumn(prop.Name) || reader[prop.Name] is DBNull) continue;
                            prop.SetValue(obj, Convert.ChangeType(reader[prop.Name], prop.PropertyType));
                        }
                        result.Add(obj);
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }

    // For single value (like COUNT(*))
    public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
    {
        try
        {
            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                AddParameters(cmd, parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
    public bool ExecuteTransaction(List<(string query, Dictionary<string, object> parameters)> commands)
    {
        try
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var (query, parameters) in commands)
                        {
                            using (var cmd = new MySqlCommand(query, conn, transaction))
                            {
                                AddParameters(cmd, parameters);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Transaction failed: " + ex.Message);
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }


    private void AddParameters(MySqlCommand cmd, Dictionary<string, object> parameters)
    {
        if (parameters == null) return;
        foreach (var p in cmd.Parameters)
        {
            Console.WriteLine(((MySqlParameter)p).ParameterName + ": " + ((MySqlParameter)p).Value);
        }

        foreach (var kvp in parameters)
            cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
    }

}

public static class MySqlExtensions
{
    public static bool HasColumn(this MySqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }
}
