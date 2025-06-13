using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class RobotCommandADO : IRobotCommandDataAccess
    {
        // Connection string for your PostgreSQL database
        private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=password;Database=sit331";

        // Method to retrieve all robot commands
        public List<RobotCommand> GetRobotCommands()
        {
            var robotCommands = new List<RobotCommand>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM robot_commands", conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var robotCommand = new RobotCommand(
                    (int)dr["id"], // Convert id to int
                    (string)dr["name"],
                    (bool)dr["is_move_command"],
                    (DateTime)dr["created_date"],
                    (DateTime)dr["modified_date"],
                    dr["description"] as string // Use nullable if description is null
                );
                robotCommands.Add(robotCommand);
            }

            return robotCommands;
        }

        // Method to add a new robot command
        public int AddRobotCommand(RobotCommand command)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO robot_commands (name, description, is_move_command, created_date, modified_date)
                VALUES (@name, @description, @is_move_command, @created_date, @modified_date)
                RETURNING id", conn);

            cmd.Parameters.AddWithValue("name", command.Name);
            cmd.Parameters.AddWithValue("description", (object?)command.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("is_move_command", command.IsMoveCommand);
            cmd.Parameters.AddWithValue("created_date", command.CreatedDate);
            cmd.Parameters.AddWithValue("modified_date", command.ModifiedDate);

            return (int)cmd.ExecuteScalar(); // Get the auto-generated ID
        }


        // Method to update an existing robot command
        public RobotCommand UpdateRobotCommand(int id, RobotCommand updatedCommand)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("UPDATE robot_commands SET name = @name, description = @description, is_move_command = @is_move_command, modified_date = @modified_date WHERE id = @id RETURNING *;", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", updatedCommand.Name);
            cmd.Parameters.AddWithValue("description", (object)updatedCommand.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("is_move_command", updatedCommand.IsMoveCommand);
            cmd.Parameters.AddWithValue("modified_date", DateTime.UtcNow); // Set modified date to current UTC time

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new RobotCommand(
                    (int)reader["id"],
                    (string)reader["name"],
                    (bool)reader["is_move_command"],
                    (DateTime)reader["created_date"],
                    (DateTime)reader["modified_date"],
                    reader["description"] as string
                );
            }
            throw new Exception("Map update failed; no record was returned.");
            
        }

        // Method to delete a robot command by ID
        public void DeleteRobotCommand(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("DELETE FROM robot_commands WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();
        }
    }
}
