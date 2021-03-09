namespace VweCore.Abstractions
{
    public interface IRotatable : IEntity
    {
        double AngleInDegrees { get; }

        void RotateToAbsoluteAngle(double angleInDegrees);

        void Rotate(double angleInDegrees);
    }
}