using robot_controller_api.Models;
using Microsoft.EntityFrameworkCore;

namespace robot_controller_api.Persistence;

public class MapEF : IMapDataAccess
{
    private readonly RobotContext _context;

    public MapEF(RobotContext context)
    {
        _context = context;
    }

    public List<Map> GetMaps()
    {
        return _context.Maps.ToList();
    }

    public int InsertMap(Map map)
    {
        _context.Maps.Add(map);
        _context.SaveChanges();
        return map.Id; // EF will populate the ID after SaveChanges if DB is configured correctly
    }

    public Map UpdateMap(int id, Map map)
    {
        var existingMap = _context.Maps.Find(id);
        if (existingMap == null) return null!;

        existingMap.Name = map.Name;
        existingMap.Description = map.Description;
        existingMap.Columns = map.Columns;
        existingMap.Rows = map.Rows;
        existingMap.ModifiedDate = DateTime.Now;

        _context.SaveChanges();
        return existingMap;
    }

    public void DeleteMap(int id)
    {
        var map = _context.Maps.Find(id);
        if (map != null)
        {
            _context.Maps.Remove(map);
            _context.SaveChanges();
        }
    }
}
