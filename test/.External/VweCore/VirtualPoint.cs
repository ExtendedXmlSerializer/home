using System;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
using VweCore.Abstractions;
using VweCore.Geometry;
using VweCore.Translations;

namespace VweCore
{
    public sealed class VirtualPoint : Entity<VirtualPoint>, ICustomDelete, IEquatable<VirtualPoint>, IComparable<VirtualPoint>
    {
        public VirtualPoint() : base(EntityOrderIndex.VirtualPoints, TranslationKeys.EntityName_VirtualPoint) { }

        // A virtual point must at all times be associated with a node link.
        // This property is only nullable because of XML serialization.
        public NodeLink? NodeLink { get; set; }

        public VirtualPointOrigin Origin { get; set; }

        public double Offset { get; set; }

        public string Type { get; set; } = string.Empty;

        void ICustomDelete.DeleteFromMap(Map map) => DeleteFromMap(map);

        void ICustomDelete.RestoreBackToMap(Map map) => RestoreBackToMap(map);

        public VirtualPoint DeleteFromMap(Map map)
        {
            map = map.MustNotBeNull(nameof(map));
            NodeLink?.VirtualPoints.Remove(this);
            map.Remove(this);
            return this;
        }

        public VirtualPoint RestoreBackToMap(Map map)
        {
            map = map.MustNotBeNull(nameof(map));
            NodeLink?.VirtualPoints.Add(this);
            map.Add(this);
            return this;
        }

        public Point2D GetAbsolutePosition()
        {
            if (NodeLink == null)
                Throw.InvalidOperation("The absolute position of a virtual point cannot be determined when it is not attached to a node link.");

            if (!NodeLink.TryGetLineSegment(out var lineSegment))
                lineSegment = new LineSegment2D(NodeLink.GetPosition1(), NodeLink.GetPosition2());

            return lineSegment.GetPointByOffset(Offset, Origin == VirtualPointOrigin.Position1);
        }

        public override LeveledRectangle GetBounds()
        {
            var position = GetAbsolutePosition();
            return new LeveledRectangle(position, position);
        }

        public VirtualPoint Clone(int id) =>
           new ()
           {
               Id = id,
               Origin = Origin,
               Offset = Offset,
               Type = Type
           };

        public override string ToString() => $"{TypeIdentifier} {Id} ({GetAbsolutePosition()})";
    }
}