namespace robot_controller_api.Persistence{
    public interface IRobotCommandDataAccess
    {
        int AddRobotCommand(RobotCommand command);
        void DeleteRobotCommand(int id);
        List<RobotCommand> GetRobotCommands();
        RobotCommand UpdateRobotCommand(int id, RobotCommand updatedCommand);
    }
}