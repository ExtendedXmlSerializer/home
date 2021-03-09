namespace VweCore.Geometry
{
    public interface ICurrentPointObserver
    {
        void CurrentPointChanged(Point2D newPoint);
    }
}