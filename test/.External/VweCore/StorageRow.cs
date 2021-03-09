using System;
using System.Collections.Generic;
using Light.GuardClauses;
using VweCore.Abstractions;
using VweCore.Geometry;
using VweCore.Translations;

#pragma warning disable CA2227 // XML Serialization requires settable collection property

namespace VweCore
{
    public sealed class StorageRow : Entity<StorageRow>, IPositionable, IRotatable, IRelativelyRotatable, IEquatable<StorageRow>, IComparable<StorageRow>
    {
        private double _storageLocationPadding = 0.25;
        private List<StorageLocation>? _storageLocations;

        public StorageRow() : base(EntityOrderIndex.StorageRows, TranslationKeys.EntityName_StorageRow) { }

        public List<StorageLocation> StorageLocations
        {
            get => _storageLocations ??= new List<StorageLocation>();
            set => _storageLocations = value.MustNotBeNull();
        }

        public double Width { get; set; }

        public double Height { get; set; }

        public double StorageLocationPadding
        {
            get => _storageLocationPadding;
            set => _storageLocationPadding = value.MustNotBeLessThan(0.0);
        }

        public Point2D Position { get; set; }

        void IMovable.Move(Point2D moveVector) => Move(moveVector);

        public override LeveledRectangle GetBounds()
        {
            var halfWidth = Width / 2.0;
            var halfHeight = Height / 2.0;
            var point1 = (Position + new Point2D(-halfWidth, -halfHeight)).RotateAroundReferencePoint(AngleInDegrees, Position);
            var point2 = (Position + new Point2D(halfWidth, halfHeight)).RotateAroundReferencePoint(AngleInDegrees, Position);
            var bounds = new LeveledRectangle(point1, point2);
            var point3 = (Position + new Point2D(-halfWidth, halfHeight)).RotateAroundReferencePoint(AngleInDegrees, Position);
            var point4 = (Position + new Point2D(halfWidth, -halfHeight)).RotateAroundReferencePoint(AngleInDegrees, Position);
            return bounds.Extend(new LeveledRectangle(point3, point4));
        }

        void IRelativelyRotatable.RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint) =>
            RotateAroundReferencePoint(angleInDegrees, referencePoint);

        public double AngleInDegrees { get; set; }

        void IRotatable.RotateToAbsoluteAngle(double angleInDegrees) => RotateToAbsoluteAngle(angleInDegrees);

        void IRotatable.Rotate(double angleInDegrees) => RotateToAbsoluteAngle(AngleInDegrees + angleInDegrees);

        public Point2D GetStorageLocationOffsetOrigin() => new Point2D(Width / 2, 0).RotateAroundOrigin(AngleInDegrees) + Position;

        public StorageRow Clone(int id) =>
            new StorageRow
            {
                Id = id,
                StorageLocationPadding = StorageLocationPadding,
                Position = Position,
                AngleInDegrees = AngleInDegrees,
                Width = Width,
                Height = Height
            };

        public Ray2D GetPerpendicularRay()
        {
            var referencePoint = new Point2D(Position.X + (Width / 2), Position.Y).RotateAroundReferencePoint(AngleInDegrees, Position);
            return new Ray2D(referencePoint, AngleInDegrees);
        }

        public StorageRow Move(Point2D moveVector)
        {
            Position += moveVector;
            return this;
        }

        public StorageRow RotateToAbsoluteAngle(double newAngleInDegrees)
        {
            AngleInDegrees = newAngleInDegrees;
            return this;
        }

        public StorageRow AddStorageLocation(StorageLocation storageLocation)
        {
            storageLocation = storageLocation.MustNotBeNull(nameof(storageLocation));

            storageLocation.StorageRow = this;
            StorageLocations.Add(storageLocation);
            return this;
        }

        public StorageRow AddStorageLocationAndAdjustWidthIfNecessary(StorageLocation storageLocation)
        {
            storageLocation = storageLocation.MustNotBeNull(nameof(storageLocation));

            var (newMinimumWidth, newStraightOffset) = CalculateMinimumWidthAndOffsetForNewStorageLocation(storageLocation);
            storageLocation.StorageRowOffset = new Point2D(newStraightOffset, 0);
            AddStorageLocation(storageLocation);
            if (Width < newMinimumWidth)
                Width = newMinimumWidth;
            return this;
        }

        public Rectangle2D GetRectangle() => new Rectangle2D(Position, Width, Height, AngleInDegrees);

        private (double minimumWidth, double newStraingOffset) CalculateMinimumWidthAndOffsetForNewStorageLocation(StorageLocation storageLocation)
        {
            var minimumWidth = 0.0;
            for (var i = 0; i < StorageLocations.Count; i++)
            {
                minimumWidth += StorageLocations[i].Diameter + StorageLocationPadding + StorageLocationPadding;
            }

            var offsetForNewLocation = -(minimumWidth + StorageLocationPadding + (storageLocation.Diameter / 2));
            minimumWidth += storageLocation.Diameter + StorageLocationPadding + StorageLocationPadding;
            return (minimumWidth, offsetForNewLocation);
        }

        public StorageRow RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint)
        {
            Position = Position.RotateAroundReferencePoint(angleInDegrees, referencePoint);
            AngleInDegrees += angleInDegrees;
            return this;
        }
    }
}