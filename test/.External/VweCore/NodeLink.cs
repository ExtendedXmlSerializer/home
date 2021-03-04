using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
using VweCore.Abstractions;
using VweCore.Geometry;
using VweCore.Translations;

#pragma warning disable CA2227 // XML Serialization requires settable collection property

namespace VweCore
{
    public sealed class NodeLink : Entity<NodeLink>, IMovable, IRelativelyRotatable, ILineSegment, ICustomDelete, IEquatable<NodeLink>, IComparable<NodeLink>
    {
        private List<VirtualPoint>? _virtualPoints;

        public NodeLink() : base(EntityOrderIndex.NodeLinks, TranslationKeys.EntityName_NodeLink) { }

        public Point2D? Point1 { get; set; }

        public Point2D? Point2 { get; set; }

        public Node? Node1 { get; set; }

        public Node? Node2 { get; set; }

        public NodeLinkDirection Direction { get; set; } = NodeLinkDirection.Bidirectional;

        public NodeLinkDrivingDirection? DrivingDirectionFromNode1 { get; set; } = NodeLinkDrivingDirection.Straight;

        public NodeLinkDrivingDirection? DrivingDirectionFromNode2 { get; set; } = NodeLinkDrivingDirection.Straight;

        public double ActualLength { get; set; } = 1.0;

        public List<VirtualPoint> VirtualPoints
        {
            get => _virtualPoints ??= new List<VirtualPoint>();
            set => _virtualPoints = value.MustNotBeNull();
        }

        void ICustomDelete.DeleteFromMap(Map map) => DeleteFromMap(map);

        void ICustomDelete.RestoreBackToMap(Map map) => RestoreBackToMap(map);

        Point2D ILineSegment.Point1
        {
            get
            {
                if (Point1 == null)
                    throw new InvalidOperationException($"Point1 of NodeLink \"{this}\" can only be retrieved when it is not attached at Node1.");
                return Point1.Value;
            }

            set
            {
                if (Point1 == null)
                    throw new InvalidOperationException($"Point1 of NodeLink \"{this}\" can only be set when it is not attached at Node1.");
                Point1 = value;
            }
        }

        Point2D ILineSegment.Point2
        {
            get
            {
                if (Point2 == null)
                    throw new InvalidOperationException($"Point2 of NodeLink \"{this}\" can only be retrieved when it is not attached at Node2.");
                return Point2.Value;
            }

            set
            {
                if (Point2 == null)
                    throw new InvalidOperationException($"Point2 of NodeLink \"{this}\" can only be set when it is not attached at Node2.");
                Point2 = value;
            }
        }

        public void Move(Point2D moveVector)
        {
            if (Point1.HasValue)
                Point1 = Point1.Value + moveVector;

            if (Point2.HasValue)
                Point2 = Point2.Value + moveVector;
        }

        public override LeveledRectangle GetBounds()
        {
            var position1 = GetPosition1();
            var position2 = GetPosition2();
            return new LeveledRectangle(position1, position2);
        }

        void IRelativelyRotatable.RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint) =>
            RotateAroundReferencePoint(angleInDegrees, referencePoint);

        public NodeLink DeleteFromMap(Map map)
        {
            map = map.MustNotBeNull(nameof(map));

            Node1?.NodeLinks.Remove(this);
            Node2?.NodeLinks.Remove(this);
            map.Remove(this);
            return this;
        }

        public NodeLink RestoreBackToMap(Map map)
        {
            map = map.MustNotBeNull(nameof(map));

            Node1?.NodeLinks.Add(this);
            Node2?.NodeLinks.Add(this);
            map.Add(this);
            return this;
        }

        public bool CheckIfBothNodesAreConnected() => Node1 != null && Node2 != null;

        public bool CheckIfAnyNodeIsConnected() => Node1 != null || Node2 != null;

        public Point2D GetPosition1()
        {
            var point = Node1?.GetAbsolutePosition() ?? Point1;
            if (point.HasValue)
                return point.Value;

            throw new InvalidStateException($"The node link is in an invalid state because neither {nameof(Node1)} nor {nameof(Point1)} are set.");
        }

        public Point2D GetPosition2()
        {
            var point = Node2?.GetAbsolutePosition() ?? Point2;
            if (point.HasValue)
                return point.Value;

            throw new InvalidStateException($"The node link is in an invalid state because neither {nameof(Node2)} nor {nameof(Point2)} are set.");
        }

        public bool CheckIfNodeIsConnectedToOtherNode(Node node)
        {
            if (Node1 == null || Node2 == null)
                return false;

            if (Node1.Equals(node))
                return Direction != NodeLinkDirection.FromNode2ToNode1;
            if (Node2.Equals(node))
                return Direction != NodeLinkDirection.FromNode1ToNode2;

            return false;
        }

        public bool TryGetOtherNode(Node node, [NotNullWhen(true)] out Node? otherNode)
        {
            if (!CheckIfBothNodesAreConnected())
                goto NodeNotFound;

            if (Node1!.Equals(node))
            {
                otherNode = Node2!;
                return true;
            }

            if (Node2!.Equals(node))
            {
                otherNode = Node1;
                return true;
            }

            NodeNotFound:
            otherNode = null;
            return false;
        }

        public bool TryGetLineSegment(out LineSegment2D lineSegment)
        {
            if (!CheckIfBothNodesAreConnected())
            {
                lineSegment = default;
                return false;
            }

            lineSegment = new LineSegment2D(Node1!.GetAbsolutePosition(), Node2!.GetAbsolutePosition());
            return true;
        }

        public void SetNode1(Node? node, Point2D fallbackPoint)
        {
            Node1?.NodeLinks.Remove(this);
            if (node == null)
            {
                Node1 = null;
                Point1 = fallbackPoint;
                return;
            }

            Point1 = null;
            Node1 = node;
            Node1.NodeLinks.Add(this);
        }

        public void SetNode2(Node? node, Point2D fallbackPoint)
        {
            Node2?.NodeLinks.Remove(this);

            if (node == null)
            {
                Node2 = null;
                Point2 = fallbackPoint;
                return;
            }

            Point2 = null;
            Node2 = node;
            Node2.NodeLinks.Add(this);
        }

        public override string ToString()
        {
            if (CheckIfBothNodesAreConnected())
            {
                return Direction switch
                {
                    NodeLinkDirection.FromNode1ToNode2 => Translator.Instance.TranslateAndFormat(TranslationKeys.NodeLink_DirectionFromNodeToNode, Node1, Node2),
                    NodeLinkDirection.FromNode2ToNode1 => Translator.Instance.TranslateAndFormat(TranslationKeys.NodeLink_DirectionFromNodeToNode, Node2, Node1),
                    _ => Translator.Instance.TranslateAndFormat(TranslationKeys.NodeLink_BidirectionalNodeLinkBetween, Node1, Node2)
                };
            }

            if (Point1 == null || Point2 == null)
                return Translator.Instance.GetTranslation(TranslationKeys.NodeLink_InvalidNodeLink);

            return Translator.Instance.TranslateAndFormat(TranslationKeys.NodeLink_DisconnectedNodeLink, Point1, Point2);
        }

        public NodeLink RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint)
        {
            if (Node1 == null)
            {
                if (Point1 == null)
                    throw new InvalidStateException($"Node Link {Id} is in an invalid state as both Node1 and Point1 are not set.");
                Point1 = Point1.Value.RotateAroundReferencePoint(angleInDegrees, referencePoint);
            }

            if (Node2 == null)
            {
                if (Point2 == null)
                    throw new InvalidStateException($"Node Link {Id} is in an invalid state as both Node2 and Point2 are not set.");

                Point2 = Point2.Value.RotateAroundReferencePoint(angleInDegrees, referencePoint);
            }

            return this;
        }

        public NodeLink Clone(int id) =>
            new ()
            {
                Id = id,
                Direction = Direction,
                DrivingDirectionFromNode1 = DrivingDirectionFromNode1,
                DrivingDirectionFromNode2 = DrivingDirectionFromNode2,
                ActualLength = ActualLength,
                Point1 = Point1,
                Point2 = Point2
            };

        public NodeLink AddVirtualPoint(VirtualPoint virtualPoint)
        {
            virtualPoint = virtualPoint.MustNotBeNull(nameof(virtualPoint));

            virtualPoint.NodeLink = this;
            VirtualPoints.Add(virtualPoint);
            return this;
        }

        public NodeLink AddVirtualPointAndCalculateItsOffset(VirtualPoint virtualPoint)
        {
            virtualPoint = virtualPoint.MustNotBeNull(nameof(virtualPoint));

            virtualPoint.Offset = DetermineOffsetForNewVirtualPoint(virtualPoint.Origin);
            return AddVirtualPoint(virtualPoint);
        }

        public double CalculateLength()
        {
            if (!TryGetLineSegment(out var lineSegment))
                lineSegment = new LineSegment2D(GetPosition1(), GetPosition2());

            return lineSegment.CalculateLength();
        }

        public double DetermineOffsetForNewVirtualPoint(VirtualPointOrigin origin)
        {
            var length = CalculateLength();

            if (VirtualPoints.Count == 0)
                return length / 2.0;

            if (VirtualPoints.Count == 1)
                return length / 10.0 * 6.0;

            var lastPoint = VirtualPoints[VirtualPoints.Count - 1];
            var secondToLastPoint = VirtualPoints[VirtualPoints.Count - 2];

            var lastOffset = lastPoint.Origin == VirtualPointOrigin.Position1 ? lastPoint.Offset : length - lastPoint.Offset;
            var secondToLastOffset = secondToLastPoint.Origin == VirtualPointOrigin.Position1 ? secondToLastPoint.Offset : length - secondToLastPoint.Offset;
            var step = lastOffset - secondToLastOffset;
            var nextOffset = lastOffset + step;
            nextOffset = nextOffset.IsGreaterThanOrApproximatelyEqualTo(length)
                ? nextOffset - length
                : nextOffset.IsLessThanOrApproximatelyEqualTo(0.0)
                    ? length + nextOffset
                    : nextOffset;

            return origin == VirtualPointOrigin.Position1 ? nextOffset : length - nextOffset;
        }
    }
}