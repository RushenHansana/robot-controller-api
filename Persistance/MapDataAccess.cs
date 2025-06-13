using Npgsql;

namespace robot_controller_api.Persistence;

public static class MapDataAccess
{
    private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=password;Database=sit331";

    public static List<Map> GetMaps()
    {
        var maps = new List<Map>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT * FROM map", conn);
        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            var map = new Map(
                (int)dr["id"],
                (int)dr["columns"],
                (int)dr["rows"],
                (string)dr["name"],
                (DateTime)dr["createddate"],
                (DateTime)dr["modifieddate"],
                dr["description"] as string
            );
            maps.Add(map);
        }

        return maps;
    }

    public static int InsertMap(Map map)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"INSERT INTO map (columns, rows, name, description, createddate, modifieddate) 
              VALUES (@columns, @rows, @name, @description, @createddate, @modifieddate) RETURNING id", conn);
        cmd.Parameters.AddWithValue("columns", map.Columns);
        cmd.Parameters.AddWithValue("rows", map.Rows);
        cmd.Parameters.AddWithValue("name", map.Name);
        cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("createddate", map.CreatedDate);
        cmd.Parameters.AddWithValue("modifieddate", map.ModifiedDate);

        return (int)cmd.ExecuteScalar();
    }

    public static void UpdateMap(int id, Map map)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"UPDATE map SET columns = @columns, rows = @rows, name = @name, 
              description = @description, modifieddate = @modifieddate 
              WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("columns", map.Columns);
        cmd.Parameters.AddWithValue("rows", map.Rows);
        cmd.Parameters.AddWithValue("name", map.Name);
        cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("modifieddate", DateTime.UtcNow); // Set modified date to current UTC time

        cmd.ExecuteNonQuery();
    }

    public static void DeleteMap(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand("DELETE FROM map WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }
}
