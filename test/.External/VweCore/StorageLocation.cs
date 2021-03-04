using System;
using System.Collections.Generic;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
using VweCore.Abstractions;
using VweCore.Geometry;
using VweCore.Translations;

#pragma warning disable CA2227 // XML Serialization requires settable collection property

namespace VweCore
{
    public sealed class StorageLocation : Entity<StorageLocation>, IPositionable, ICustomDelete, IEquatable<StorageLocation>, IComparable<StorageLocation>, IRelativelyPositionable
    {
        private double _diameter;
        private List<MarkerPoint>? _markerPoints;

        public StorageLocation() : base(EntityOrderIndex.StorageLocations, TranslationKeys.EntityName_StorageLocation) { }

        public StorageRow? StorageRow { get; set; }

        public double Diameter
        {
            get => _diameter;
            set => _diameter = value.MustNotBeLessThan(0.0);
        }

        public List<MarkerPoint> MarkerPoints
        {
            get => _markerPoints ??= new List<MarkerPoint>();
            set => _markerPoints = value.MustNotBeNull();
        }

        public Point2D StorageRowOffset { get; set; }

        public string Type { get; set; } = "Foo";

        void ICustomDelete.DeleteFromMap(Map map) => DeleteFromMap(map);

        void ICustomDelete.RestoreBackToMap(Map map) => RestoreBackToMap(map);

        Point2D IPositionable.Position
        {
            get => GetAbsolutePosition();
            set => SetAbsolutePosition(value);
        }

        void IMovable.Move(Point2D moveVector) => Move(moveVector);

        public override LeveledRectangle GetBounds()
        {
            var radius = Diameter / 2;
            var radiusVector = new Point2D(radius, radius);
            return new LeveledRectangle(GetAbsolutePosition() - radiusVector, GetAbsolutePosition() + radiusVector);
        }

        Point2D IRelativelyPositionable.Offset
        {
            get => StorageRowOffset;
            set => StorageRowOffset = value;
        }

        public StorageLocation Clone(int id) =>
            new ()
            {
                Id = id,
                StorageRowOffset = StorageRowOffset,
                Diameter = Diameter
            };

        public Circle2D GetCircle()
        {
            if (StorageRow == null)
                Throw.InvalidOperation("The circle geometry cannot be determined when it is not attached to a Storage Row.");

            return new Circle2D(GetAbsolutePosition(), Diameter);
        }

        public Point2D GetAbsolutePosition()
        {
            if (StorageRow == null)
                Throw.InvalidOperation("The absolute position of a storage location cannot be determined when it is not attached to storage row.");

            return StorageRow.GetStorageLocationOffsetOrigin() + StorageRowOffset.RotateAroundOrigin(StorageRow.AngleInDegrees);
        }

        public StorageLocation SetAbsolutePosition(Point2D position)
        {
            if (StorageRow == null)
                Throw.InvalidOperation("The absolute position of a storage location cannot be set when it is not attached to a storage row.");

            StorageRowOffset = (position - StorageRow.GetStorageLocationOffsetOrigin()).RotateAroundOrigin(StorageRow.AngleInDegrees);
            return this;
        }

        public StorageLocation Move(Point2D moveVector)
        {
            if (StorageRow == null)
                Throw.InvalidOperation("A storage location cannot be set moved when it is not attached to a storage row.");

            StorageRowOffset += moveVector.RotateAroundOrigin(-StorageRow.AngleInDegrees);
            return this;
        }

        public StorageLocation DeleteFromMap(Map map)
        {
            map = map.MustNotBeNull(nameof(map));
            StorageRow?.StorageLocations.Remove(this);
            map.Remove(this);
            return this;
        }

        public StorageLocation RestoreBackToMap(Map map)
        {
            map = map.MustNotBeNull(nameof(map));
            StorageRow?.StorageLocations.Add(this);
            map.Add(this);
            return this;
        }

        public StorageLocation AddMarkerPoint(MarkerPoint markerPoint)
        {
            markerPoint = markerPoint.MustNotBeNull(nameof(markerPoint));

            markerPoint.StorageLocation = this;
            MarkerPoints.Add(markerPoint);
            return this;
        }

        public StorageLocation AddMarkerPointAndCalculateItsOffset(MarkerPoint markerPoint)
        {
            markerPoint = markerPoint.MustNotBeNull(nameof(markerPoint));

            markerPoint.StorageLocationOffset = DetermineOffsetForNewMarkerPoint();
            return AddMarkerPoint(markerPoint);
        }

        private Point2D DetermineOffsetForNewMarkerPoint()
        {
            if (MarkerPoints.Count == 0)
                return new Point2D(1, 0);
            if (MarkerPoints.Count == 1)
                return MarkerPoints[0].StorageLocationOffset * 2;

            var lastOffset = MarkerPoints[MarkerPoints.Count - 1].StorageLocationOffset;
            var secondToLastOffset = MarkerPoints[MarkerPoints.Count - 2].StorageLocationOffset;
            var difference = lastOffset - secondToLastOffset;
            return lastOffset + difference;
        }
    }
}