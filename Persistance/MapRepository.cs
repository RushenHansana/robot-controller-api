using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class MapRepository : IMapDataAccess, IRepository
    {
        private IRepository _repo => this;

        // Get all maps
        public List<Map> GetMaps()
        {
            var maps = _repo.ExecuteReader<Map>("SELECT * FROM public.maps");
            return maps;
        }

        // Insert a new map
        public int InsertMap(Map map)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("columns", map.Columns),
                new("rows", map.Rows),
                new("name", map.Name),
                new("description", (object?)map.Description ?? DBNull.Value),
                new("created_date", map.CreatedDate),
                new("modified_date", map.ModifiedDate)
            };

            var result = _repo.ExecuteReader<Map>(
                @"INSERT INTO map (columns, rows, name, description, created_date, modified_date)
                  VALUES (@columns, @rows, @name, @description, @created_date, @modified_date)
                  RETURNING *;",
                sqlParams
            ).Single();

            return result.Id;
        }

        // Update a map
        public Map UpdateMap(int id, Map map)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("id", id),
                new("columns", map.Columns),
                new("rows", map.Rows),
                new("name", map.Name),
                new("description", (object?)map.Description ?? DBNull.Value),
                new("modified_date", DateTime.UtcNow)
            };

            var result=_repo.ExecuteReader<Map>(
                @"UPDATE maps 
                  SET columns = @columns, rows = @rows, name = @name, 
                      description = @description, modified_date = @modified_date 
                  WHERE id = @id 
                  RETURNING *;",
                sqlParams
            ).Single();
            return result;
        }

        // Delete a map
        public void DeleteMap(int id)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("id", id)
            };

            _repo.ExecuteReader<Map>(
                "DELETE FROM maps WHERE id = @id RETURNING *;",
                sqlParams
            );
        }
    }
}
