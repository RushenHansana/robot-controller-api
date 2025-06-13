using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>A list of all users.</returns>
    [HttpGet, Authorize(Policy = "AdminOnly")]
    public IEnumerable<UserModel> GetAllUsers()
    {
        return UserDataAccess.GetUsers();
    }

    /// <summary>
    /// Retrieves only admin users.
    /// </summary>
    /// <returns>A list of users where Role is "admin".</returns>
    [HttpGet("admin"), Authorize(Policy = "AdminOnly")]
    public IEnumerable<UserModel> GetAdminUsersOnly()
    {
        return UserDataAccess.GetUsers().Where(u => u.Role == "Admin");
    }

    /// <summary>
    /// Retrieves a specific user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The user if found; otherwise, NotFound.</returns>
    /// <response code="200">Returns the user.</response>
    /// <response code="404">If the user is not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult GetUserById(int id)
    {
        var user = UserDataAccess.GetUsers().FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // this for Add a new user should be used to register new users req and res body are usermodel
    /// <summary>
    /// Creates a new user.
    /// /// </summary>
    /// <param name="newUser">The user to add.</param>
    /// <returns>The newly created user with a URI to retrieve it.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///    POST /api/users
    ///   {
    ///       "email": "
    ///      "firstname": "John",
    ///      "lastname": "Doe",
    ///      "passwordhash": "hashedpassword",
    ///      "description": "A sample user",
    ///      "role": "user",
    ///      "createddate": "2023-10-01T00:00:00Z",
    ///     "modifieddate": "2023-10-01T00:00:00Z"
    ///   }
    /// </remarks>
    /// <response code="201">Returns the created user.</response>
    /// <response code="400">If the user data is invalid.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost, Authorize(Policy = "AdminOnly")]
    public IActionResult CreateUser([FromBody] UserModel newUser)
    {
        if (newUser == null || string.IsNullOrWhiteSpace(newUser.Email) || string.IsNullOrWhiteSpace(newUser.FirstName) || string.IsNullOrWhiteSpace(newUser.LastName) || string.IsNullOrWhiteSpace(newUser.PasswordHash))
        {
            return BadRequest("Invalid user data.");
        }
        if (UserDataAccess.GetUsers().Any(u => u.Email == newUser.Email))
        {
            return Conflict("A user with this email already exists.");
        }

        var hasher = new PasswordHasher<UserModel>();
        string hashedPassword = hasher.HashPassword(newUser, newUser.PasswordHash); // Note: PasswordHash contains raw password initially
        newUser.PasswordHash = hashedPassword;
        newUser.CreatedDate = DateTime.UtcNow;
        newUser.ModifiedDate = DateTime.UtcNow;
        var userId = UserDataAccess.AddUser(newUser);
        var createdUser = UserDataAccess.GetUsers().FirstOrDefault(u => u.Id == userId);

        return CreatedAtRoute("GetUser", new { id = createdUser.Id }, createdUser);
    }

    // this is for Disregard password and email change here; it should only update other properties and req, res body are usermodel, none respectively
    /// <summary>
    /// Updates an existing except user email and password.
    /// /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="updatedUser">The updated user data.</param>
    /// <returns>The updated user if successful.</returns>
    /// <response code="204">No content if the user is updated successfully.</response>
    /// <response code="400">If the update data is invalid.</response>
    /// <response code="404">If the user is not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}"),Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateUser(int id, [FromBody] UserModel updatedUser)
    {
        if (updatedUser == null || string.IsNullOrWhiteSpace(updatedUser.FirstName) || string.IsNullOrWhiteSpace(updatedUser.LastName))
        {
            return BadRequest("Invalid user data.");
        }

        var existingUser = UserDataAccess.GetUsers().FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
        {
            return NotFound();
        }

        // Update the user properties except email and password
        existingUser.FirstName = updatedUser.FirstName;
        existingUser.LastName = updatedUser.LastName;
        existingUser.Description = updatedUser.Description;
        existingUser.Role = updatedUser.Role;
        existingUser.ModifiedDate = DateTime.UtcNow;

        UserDataAccess.UpdateUser(id, existingUser);
        return Ok(existingUser);
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">No content if the user is deleted successfully.</response>
    /// <response code="404">If the user is not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPatch("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteUser(int id)
    {
        var user = UserDataAccess.GetUsers().FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        UserDataAccess.DeleteUser(id);
        return NoContent();
    }

    /// <summary>
    /// Updates user with email and password.
    /// /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="updatedUserLogin">The updated user data.</param>
    /// <returns>No content if successful.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///   PATCH /api/users/{id}
    ///  {
    ///      "email": "
    ///     "password": "newpassword"
    ///  }
    /// </remarks>
    /// <response code="204">No content if the user is updated successfully.</response>
    /// <response code="400">If the update data is invalid.</response>
    /// <response code="404">If the user is not found.</response> 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPatch("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateUserLogin(int id, [FromBody] LoginModel updatedUserLogin)
    {
        if (updatedUserLogin == null || string.IsNullOrWhiteSpace(updatedUserLogin.Email) || string.IsNullOrWhiteSpace(updatedUserLogin.Password))
        {
            return BadRequest("Invalid user data.");
        }

        var existingUser = UserDataAccess.GetUsers().FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
        {
            return NotFound();
        }

        // Update the user properties
        var hasher = new PasswordHasher<UserModel>();
        existingUser.PasswordHash = hasher.HashPassword(existingUser, updatedUserLogin.Password);
        existingUser.Email = updatedUserLogin.Email;
        existingUser.ModifiedDate = DateTime.UtcNow;

        UserDataAccess.UpdateUser(id, existingUser);
        return NoContent();
    }
    

}