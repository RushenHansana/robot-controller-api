using Npgsql;
using robot_controller_api.Models;
namespace robot_controller_api.Persistence;



public class MapADO : IMapDataAccess
{
    private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=password;Database=sit331";

    public List<Map> GetMaps()
    {
        var maps = new List<Map>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT * FROM maps", conn);
        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            var map = new Map(
                (int)dr["id"],
                (int)dr["columns"],
                (int)dr["rows"],
                (string)dr["name"],
                (DateTime)dr["created_date"],
                (DateTime)dr["modified_date"],
                dr["description"] as string
            );
            maps.Add(map);
        }

        return maps;
    }

    public int InsertMap(Map map)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"INSERT INTO maps (columns, rows, name, description, created_date, modified_date) 
              VALUES (@columns, @rows, @name, @description, @created_date, @modified_date) RETURNING id", conn);
        cmd.Parameters.AddWithValue("columns", map.Columns);
        cmd.Parameters.AddWithValue("rows", map.Rows);
        cmd.Parameters.AddWithValue("name", map.Name);
        cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("created_date", map.CreatedDate);
        cmd.Parameters.AddWithValue("modified_date", map.ModifiedDate);

        return (int)cmd.ExecuteScalar();
    }

    public Map UpdateMap(int id, Map map)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"UPDATE maps 
            SET columns = @columns, 
                rows = @rows, 
                name = @name, 
                description = @description, 
                modified_date = @modified_date 
            WHERE id = @id 
            RETURNING *;", conn);

        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("columns", map.Columns);
        cmd.Parameters.AddWithValue("rows", map.Rows);
        cmd.Parameters.AddWithValue("name", map.Name);
        cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("modified_date", DateTime.UtcNow);

        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new Map(
                id,
                (int)reader["columns"],
                (int)reader["rows"],
                (string)reader["name"],
                (DateTime)reader["created_date"],
                (DateTime)reader["modified_date"],
                reader["description"] as string
            );
        }

        throw new Exception("Map update failed; no record was returned.");
    }


    public void DeleteMap(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand("DELETE FROM maps WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }
}
