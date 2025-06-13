using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    /// <summary>
    /// Retrieves all robot commands.
    /// </summary>
    /// <returns>A list of all robot commands.</returns>
    [HttpGet, Authorize(Policy = "UserOnly")]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        return RobotCommandDataAccess.GetRobotCommands();
    }

    /// <summary>
    /// Retrieves only move commands.
    /// </summary>
    /// <returns>A list of robot commands where IsMoveCommand is true.</returns>
    [HttpGet("move"), Authorize(Policy = "UserOnly")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly()
    {
        return RobotCommandDataAccess.GetRobotCommands().Where(c => c.IsMoveCommand);
    }

    /// <summary>
    /// Retrieves a specific robot command by its ID.
    /// </summary>
    /// <param name="id">The ID of the robot command to retrieve.</param>
    /// <returns>The robot command if found; otherwise, NotFound.</returns>
    /// <response code="200">Returns the robot command.</response>
    /// <response code="404">If the robot command is not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}"), Authorize(Policy = "UserOnly")]
    public IActionResult GetRobotCommandById(int id)
    {
        var command = RobotCommandDataAccess.GetRobotCommands().FirstOrDefault(c => c.Id == id);
        if (command == null)
        {
            return NotFound();
        }
        return Ok(command);
    }

    /// <summary>
    /// Creates a new robot command.
    /// </summary>
    /// <param name="newCommand">The robot command to add.</param>
    /// <returns>The newly created robot command with a URI to retrieve it.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/robot-commands
    ///     {
    ///         "name": "DANCE",
    ///         "isMoveCommand": true,
    ///         "description": "Salsa on the Moon"
    ///     }
    /// </remarks>
    /// <response code="201">Returns the newly created robot command.</response>
    /// <response code="400">If the command is null or invalid.</response>
    /// <response code="409">If a command with the same name already exists.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null || string.IsNullOrWhiteSpace(newCommand.Name))
        {
            return BadRequest("Invalid command data.");
        }

        if (RobotCommandDataAccess.GetRobotCommands().Any(c => c.Name.Equals(newCommand.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return Conflict("A command with the same name already exists.");
        }

        newCommand.CreatedDate = DateTime.UtcNow;
        newCommand.ModifiedDate = DateTime.UtcNow;

        var newId = RobotCommandDataAccess.AddRobotCommand(newCommand);
        newCommand.Id = newId;
        return CreatedAtRoute("GetRobotCommand", new { id = newId }, newCommand);
    }

    /// <summary>
    /// Updates an existing robot command.
    /// </summary>
    /// <param name="id">The ID of the command to update.</param>
    /// <param name="updatedCommand">The updated robot command data.</param>
    /// <returns>The updated command if successful.</returns>
    /// <response code="200">Returns the updated command.</response>
    /// <response code="400">If the update data is invalid.</response>
    /// <response code="404">If the command with the given ID does not exist.</response>
    /// <response code="409">If another command with the same name already exists.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        if (updatedCommand == null || string.IsNullOrWhiteSpace(updatedCommand.Name))
        {
            return BadRequest("Invalid command data.");
        }

        var commands = RobotCommandDataAccess.GetRobotCommands();
        var existingCommand = commands.FirstOrDefault(c => c.Id == id);
        if (existingCommand == null)
        {
            return NotFound();
        }

        if (commands.Any(c => c.Name.Equals(updatedCommand.Name, StringComparison.OrdinalIgnoreCase) && c.Id != id))
        {
            return Conflict("A command with the same name already exists.");
        }

        RobotCommandDataAccess.UpdateRobotCommand(id, updatedCommand);
        return Ok(updatedCommand);
    }

    /// <summary>
    /// Deletes a robot command by its ID.
    /// </summary>
    /// <param name="id">The ID of the robot command to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the deletion is successful.</response>
    /// <response code="404">If the command does not exist.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteRobotCommand(int id)
    {
        var command = RobotCommandDataAccess.GetRobotCommands().FirstOrDefault(c => c.Id == id);
        if (command == null)
        {
            return NotFound("Command not found.");
        }

        RobotCommandDataAccess.DeleteRobotCommand(id);
        return NoContent();
    }
}
