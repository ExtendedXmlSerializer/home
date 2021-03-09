namespace VweCore.Geometry
{
    public interface ICurrentPointSource
    {
        bool TryGetCurrentPoint(out Point2D currentPoint);
    }
}