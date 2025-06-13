using Npgsql;

namespace robot_controller_api.Persistence
{
    public static class RobotCommandDataAccess
    {
        // Connection string for your PostgreSQL database
        private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=password;Database=sit331";

        // Method to retrieve all robot commands
        public static List<RobotCommand> GetRobotCommands()
        {
            var robotCommands = new List<RobotCommand>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand", conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var robotCommand = new RobotCommand(
                    (int)dr["id"], // Convert id to int
                    (string)dr["name"],
                    (bool)dr["ismovecommand"],
                    (DateTime)dr["createddate"],
                    (DateTime)dr["modifieddate"],
                    dr["description"] as string // Use nullable if description is null
                );
                robotCommands.Add(robotCommand);
            }

            return robotCommands;
        }

        // Method to add a new robot command
        public static int AddRobotCommand(RobotCommand command)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO robotcommand (name, description, ismovecommand, createddate, modifieddate)
                VALUES (@name, @description, @ismovecommand, @createddate, @modifieddate)
                RETURNING id", conn);

            cmd.Parameters.AddWithValue("name", command.Name);
            cmd.Parameters.AddWithValue("description", (object?)command.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("ismovecommand", command.IsMoveCommand);
            cmd.Parameters.AddWithValue("createddate", command.CreatedDate);
            cmd.Parameters.AddWithValue("modifieddate", command.ModifiedDate);

            return (int)cmd.ExecuteScalar(); // Get the auto-generated ID
        }


        // Method to update an existing robot command
        public static void UpdateRobotCommand(int id, RobotCommand updatedCommand)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("UPDATE robotcommand SET name = @name, description = @description, ismovecommand = @ismovecommand, modifieddate = @modifieddate WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", updatedCommand.Name);
            cmd.Parameters.AddWithValue("description", (object)updatedCommand.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("ismovecommand", updatedCommand.IsMoveCommand);
            cmd.Parameters.AddWithValue("modifieddate", DateTime.UtcNow); // Set modified date to current UTC time

            cmd.ExecuteNonQuery();
        }

        // Method to delete a robot command by ID
        public static void DeleteRobotCommand(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("DELETE FROM robotcommand WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();
        }
    }
}
