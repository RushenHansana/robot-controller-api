namespace robot_controller_api;

public class Map
{
    /// Implement <see cref="Map"> here following the task sheet requirements
    public int Id { get; set; }
    public int Columns { get; set; }
    public int Rows { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public Map(int id, int columns, int rows, string name, DateTime createdDate, DateTime modifiedDate, string? description = null)
    {
        Id = id;
        Columns = columns;
        Rows = rows;
        Name = name;
        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
        Description = description;
    }

}
