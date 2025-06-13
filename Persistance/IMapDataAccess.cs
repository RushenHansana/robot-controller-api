namespace robot_controller_api.Persistence
{
    public interface IMapDataAccess
    {
        void DeleteMap(int id);
        List<Map> GetMaps();
        int InsertMap(Map map);
        Map UpdateMap(int id, Map map);
    }
}