using MySql.Data.MySqlClient;
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
        using (var conn = new MySqlConnection(_connectionString))
        using (var cmd = new MySqlCommand(query, conn))
        {
            AddParameters(cmd, parameters);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }
    }

    // For SELECT queries returning List<T>
    public List<T> ExecuteQuery<T>(string query, Dictionary<string, object> parameters = null) where T : new()
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

    // For single value (like COUNT(*))
    public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
    {
        using (var conn = new MySqlConnection(_connectionString))
        using (var cmd = new MySqlCommand(query, conn))
        {
            AddParameters(cmd, parameters);
            conn.Open();
            return cmd.ExecuteScalar();
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
