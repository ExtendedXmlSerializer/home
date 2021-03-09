namespace VweCore.Abstractions
{
    public interface ICustomDelete
    {
        void DeleteFromMap(Map map);

        void RestoreBackToMap(Map map);
    }
}