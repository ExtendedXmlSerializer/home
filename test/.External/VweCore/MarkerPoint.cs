using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
using VweCore.Abstractions;
using VweCore.Geometry;

namespace VweCore
{
    public sealed class MarkerPoint : Node, IPositionable, ICustomDelete, IRelativelyPositionable
    {
        public StorageLocation? StorageLocation { get; set; }

        public Point2D StorageLocationOffset { get; set; }

        public double ApproachAngleInDegrees
        {
            get
            {
                if (StorageLocationOffset == Point2D.Zero || StorageLocation?.StorageRow == null)
                    return 0.0;

                var xAxisVector = new Point2D(1.0, 0.0);
                var angleInDegrees = StorageLocationOffset.CalculateAngleInDegreesBetweenVectors(xAxisVector);
                var normalizedStorageRowAngle = StorageLocation.StorageRow.AngleInDegrees.NormalizeAngleInDegrees();
                if (Tolerance.IsApproximately(angleInDegrees, 0.0))
                    return normalizedStorageRowAngle;

                if (StorageLocationOffset.Y.IsGreaterThanOrApproximatelyEqualTo(0.0))
                    angleInDegrees += normalizedStorageRowAngle;
                else
                    angleInDegrees = normalizedStorageRowAngle - angleInDegrees;

                return angleInDegrees.NormalizeAngleInDegrees();
            }
        }

        void ICustomDelete.DeleteFromMap(Map map) => DeleteFromMap(map);

        void ICustomDelete.RestoreBackToMap(Map map) => RestoreBackToMap(map);

        void IMovable.Move(Point2D moveVector) => Move(moveVector);

        Point2D IPositionable.Position
        {
            get => GetAbsolutePosition();
            set => SetAbsolutePosition(value);
        }

        Point2D IRelativelyPositionable.Offset
        {
            get => StorageLocationOffset;
            set => StorageLocationOffset = value;
        }

        public MarkerPoint DeleteFromMap(Map map)
        {
            map = map.MustNotBeNull(nameof(map));
            StorageLocation?.MarkerPoints.Remove(this);
            map.Remove(this);
            return this;
        }

        public MarkerPoint RestoreBackToMap(Map map)
        {
            map = map.MustNotBeNull(nameof(map));
            StorageLocation?.MarkerPoints.Add(this);
            map.Add(this);
            return this;
        }

        public MarkerPoint Move(Point2D moveVector)
        {
            if (StorageLocation?.StorageRow == null)
                Throw.InvalidOperation("A marker point cannot be moved when it is not attached to a storage location and row.");

            StorageLocationOffset += moveVector.RotateAroundOrigin(-StorageLocation.StorageRow.AngleInDegrees);
            return this;
        }

        public MarkerPoint SetAbsolutePosition(Point2D position)
        {
            if (StorageLocation?.StorageRow == null)
                Throw.InvalidOperation("The absolute position of a marker point cannot be set when it is not attached to a storage location and row.");

            StorageLocationOffset = (position - StorageLocation.GetAbsolutePosition()).RotateAroundOrigin(-StorageLocation.StorageRow.AngleInDegrees);
            return this;
        }

        public override Point2D GetAbsolutePosition()
        {
            if (StorageLocation?.StorageRow == null)
                Throw.InvalidOperation("The absolute position of a marker point cannot be determined when it is not attached to a storage location and row.");

            return StorageLocation.GetAbsolutePosition() + StorageLocationOffset.RotateAroundOrigin(StorageLocation.StorageRow.AngleInDegrees);
        }

        public MarkerPoint Clone(int id) =>
            new MarkerPoint
            {
                Id = id,
                DirectionalAngleInDegrees = DirectionalAngleInDegrees,
                Radius = Radius,
                StorageLocationOffset = StorageLocationOffset
            };

        public override string ToString() => $"{TypeIdentifier} {Id} ({GetAbsolutePosition()})";
    }
}