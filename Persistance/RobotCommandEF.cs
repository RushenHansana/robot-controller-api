using robot_controller_api.Models;
using Microsoft.EntityFrameworkCore;

namespace robot_controller_api.Persistence
{
    public class RobotCommandEF : IRobotCommandDataAccess
    {
        private readonly RobotContext _context;

        public RobotCommandEF(RobotContext context)
        {
            _context = context;
        }

        public int AddRobotCommand(RobotCommand command)
        {
            _context.RobotCommands.Add(command);
            _context.SaveChanges();
            return command.Id; // EF will auto-generate this if configured in DB
        }

        public void DeleteRobotCommand(int id)
        {
            var command = _context.RobotCommands.Find(id);
            if (command != null)
            {
                _context.RobotCommands.Remove(command);
                _context.SaveChanges();
            }
        }

        public List<RobotCommand> GetRobotCommands()
        {
            return _context.RobotCommands.ToList();
        }

        public RobotCommand UpdateRobotCommand(int id, RobotCommand updatedCommand)
        {
            var existing = _context.RobotCommands.Find(id);
            if (existing == null) return null!;

            existing.Name = updatedCommand.Name;
            existing.Description = updatedCommand.Description;
            existing.IsMoveCommand = updatedCommand.IsMoveCommand;
            existing.ModifiedDate = DateTime.Now;

            _context.SaveChanges();
            return existing;
        }
    }
}
