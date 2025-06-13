using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    /// <summary>
    /// Retrieves all maps.
    /// </summary>
    /// <returns>A list of all map objects.</returns>
    [HttpGet, Authorize(Policy = "UserOnly")]
    public IEnumerable<Map> GetAllMaps()
    {
        return MapDataAccess.GetMaps();
    }

    /// <summary>
    /// Retrieves only square maps (where Rows == Columns).
    /// </summary>
    /// <returns>A list of square maps.</returns>
    [HttpGet("square"), Authorize(Policy = "UserOnly")]
    public IEnumerable<Map> GetSquareMapsOnly()
    {
        return MapDataAccess.GetMaps().Where(m => m.Columns == m.Rows);
    }

    /// <summary>
    /// Retrieves a map by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the map to retrieve.</param>
    /// <returns>The map object if found; otherwise, NotFound.</returns>
    /// <response code="200">Returns the requested map.</response>
    /// <response code="404">If no map is found with the specified ID.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}"), Authorize(Policy = "UserOnly")]
    public IActionResult GetMapById(int id)
    {
        var map = MapDataAccess.GetMaps().FirstOrDefault(m => m.Id == id);
        if (map == null)
        {
            return NotFound();
        }
        return Ok(map);
    }

    /// <summary>
    /// Adds a new map.
    /// </summary>
    /// <param name="newMap">The map object to add.</param>
    /// <returns>The created map with its generated ID.</returns>
    /// <response code="201">If the map is created successfully.</response>
    /// <response code="400">If the map data is invalid.</response>
    /// <response code="409">If a map with the same name already exists.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost, Authorize(Policy = "AdminOnly")]
    public IActionResult AddMap(Map newMap)
    {
        if (newMap == null || string.IsNullOrWhiteSpace(newMap.Name))
        {
            return BadRequest("Invalid map data.");
        }
        if (MapDataAccess.GetMaps().Any(m => m.Name.Equals(newMap.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return Conflict("A map with the same name already exists.");
        }
        if (newMap.Columns <= 0 || newMap.Rows <= 0)
        {
            return BadRequest("Invalid map dimensions.");
        }

        newMap.CreatedDate = DateTime.UtcNow;
        newMap.ModifiedDate = DateTime.UtcNow;
        var newId = MapDataAccess.InsertMap(newMap);
        newMap.Id = newId;

        return CreatedAtRoute("GetMap", new { id = newId }, newMap);
    }

    /// <summary>
    /// Updates an existing map.
    /// </summary>
    /// <param name="id">The ID of the map to update.</param>
    /// <param name="updatedMap">The new map data.</param>
    /// <returns>The updated map if successful.</returns>
    /// <response code="200">If the map is updated successfully.</response>
    /// <response code="400">If the update data is invalid.</response>
    /// <response code="404">If the map does not exist.</response>
    /// <response code="409">If a map with the same name already exists.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateMap(int id, Map updatedMap)
    {
        if (updatedMap == null || string.IsNullOrWhiteSpace(updatedMap.Name))
        {
            return BadRequest("Invalid map data.");
        }

        var maps = MapDataAccess.GetMaps();
        var map = maps.FirstOrDefault(m => m.Id == id);
        if (map == null)
        {
            return NotFound();
        }

        if (maps.Any(m => m.Name.Equals(updatedMap.Name, StringComparison.OrdinalIgnoreCase) && m.Id != id))
        {
            return Conflict("A map with the same name already exists.");
        }

        if (updatedMap.Columns <= 0 || updatedMap.Rows <= 0)
        {
            return BadRequest("Invalid map dimensions.");
        }

        MapDataAccess.UpdateMap(id, updatedMap);
        return Ok(map);
    }

    /// <summary>
    /// Deletes a map by its ID.
    /// </summary>
    /// <param name="id">The ID of the map to delete.</param>
    /// <returns>No content if the deletion is successful.</returns>
    /// <response code="204">If the map was deleted.</response>
    /// <response code="404">If the map was not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteMap(int id)
    {
        var maps = MapDataAccess.GetMaps();
        var map = maps.FirstOrDefault(m => m.Id == id);
        if (map == null)
        {
            return NotFound();
        }
        MapDataAccess.DeleteMap(id);
        return NoContent();
    }

    /// <summary>
    /// Checks if a coordinate is within the bounds of the specified map.
    /// </summary>
    /// <param name="id">The ID of the map.</param>
    /// <param name="x">The x-coordinate (column index).</param>
    /// <param name="y">The y-coordinate (row index).</param>
    /// <returns>True if the coordinate is within the map bounds; otherwise, false.</returns>
    /// <response code="200">Returns true or false based on coordinate validity.</response>
    /// <response code="400">If the coordinates are invalid (e.g., negative).</response>
    /// <response code="404">If the map does not exist.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}/{x}/{y}"), Authorize(Policy = "UserOnly")]
    public IActionResult CheckCoordinate(int id, int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return BadRequest("Invalid coordinate values.");
        }

        var maps = MapDataAccess.GetMaps();
        var map = maps.FirstOrDefault(m => m.Id == id);
        if (map == null)
        {
            return NotFound("Map not found.");
        }

        bool isOnMap = x < map.Columns && y < map.Rows;
        return Ok(isOnMap);
    }
}
