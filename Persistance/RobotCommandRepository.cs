using Npgsql;
using robot_controller_api.Models;
namespace robot_controller_api.Persistence
{
    public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
    {
        private IRepository _repo => this;

        // Get all robot commands
        public List<RobotCommand> GetRobotCommands()
        {
            var commands = _repo.ExecuteReader<RobotCommand>("SELECT * FROM public.robot_commands");
            return commands;
        }

        // Add a new robot command
        public int AddRobotCommand(RobotCommand command)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("name", command.Name),
                new("description", (object?)command.Description ?? DBNull.Value),
                new("is_move_command", command.IsMoveCommand),
                new("created_date", command.CreatedDate),
                new("modified_date", command.ModifiedDate)
            };

            var result = _repo.ExecuteReader<RobotCommand>(
                @"INSERT INTO robot_commands (name, description, is_move_command, created_date, modified_date)
                  VALUES (@name, @description, @is_move_command, @created_date, @modified_date)
                  RETURNING *;",
                sqlParams
            ).Single();

            return result.Id;
        }

        // Update an existing robot command
        public RobotCommand UpdateRobotCommand(int id, RobotCommand updatedCommand)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("id", id),
                new("name", updatedCommand.Name),
                new("description", (object?)updatedCommand.Description ?? DBNull.Value),
                new("is_move_command", updatedCommand.IsMoveCommand),
                new("modified_date", DateTime.UtcNow)
            };

            var result=_repo.ExecuteReader<RobotCommand>(
                @"UPDATE robot_commands 
                  SET name = @name, description = @description, 
                      is_move_command = @is_move_command, modified_date = @modified_date 
                  WHERE id = @id 
                  RETURNING *;",
                sqlParams
            ).Single();
            return result;
        }

        // Delete a robot command
        public void DeleteRobotCommand(int id)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("id", id)
            };

            _repo.ExecuteReader<RobotCommand>(
                "DELETE FROM robot_commands WHERE id = @id RETURNING *;",
                sqlParams
            );
        }
    }
}
