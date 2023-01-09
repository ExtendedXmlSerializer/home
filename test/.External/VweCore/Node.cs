using Light.GuardClauses;
using System;
using System.Collections.Generic;
using VweCore.Geometry;
using VweCore.Translations;

#pragma warning disable CA2227 // XML serialization requires settable collection properties

namespace VweCore
{
    public abstract class Node : Entity<Node>, IEquatable<Node>, IComparable<Node>
    {
        private List<NodeLink>? _nodeLinks;

        protected Node() : base(EntityOrderIndex.Nodes, TranslationKeys.EntityName_Node) { }

        public List<NodeLink> NodeLinks
        {
            get => _nodeLinks ??= new List<NodeLink>();
            set => _nodeLinks = value.MustNotBeNull();
        }

        public double DirectionalAngleInDegrees { get; set; }

        public double Radius { get; set; } = -1.0;

        public override LeveledRectangle GetBounds()
        {
            var position = GetAbsolutePosition();
            return new LeveledRectangle(position, position);
        }

        public NodeLink AddLinkedNode(Node node, int nodeLinkId, bool isBidirectional = true)
        {
            node = node.MustNotBeSameAs(this, nameof(node)).MustNotBeNull(nameof(node));
            if (HasLinkToOtherNode(node.Id))
                throw new ArgumentException($"{this} already has a link to {node}.", nameof(node));

            var nodeLink = new NodeLink
            {
                Id = nodeLinkId,
                Node1 = this,
                Node2 = node,
                Direction = isBidirectional ? NodeLinkDirection.Bidirectional : NodeLinkDirection.FromNode1ToNode2,
                DrivingDirectionFromNode1 = NodeLinkDrivingDirection.Straight,
                DrivingDirectionFromNode2 = isBidirectional ? NodeLinkDrivingDirection.Straight : null
            };
            NodeLinks.Add(nodeLink);
            node.NodeLinks.Add(nodeLink);
            return nodeLink;
        }

        public Node AddNodeLink(NodeLink nodeLink)
        {
            nodeLink = nodeLink.MustNotBeNull(nameof(nodeLink));
            if (!ReferenceEquals(this, nodeLink.Node1) && !ReferenceEquals(this, nodeLink.Node2))
                throw new ArgumentException($"None of the nodes of link {nodeLink} point to {this}, so this link cannot be added to the node.");

            NodeLinks.Add(nodeLink);
            return this;
        }

        public bool HasLinkToOtherNode(int otherNodeId)
        {
            if (_nodeLinks.IsNullOrEmpty())
                return false;

            foreach (var nodeLink in _nodeLinks)
            {
                if (nodeLink.TryGetOtherNode(this, out var other) && other.Id == otherNodeId)
                    return true;
            }

            return false;
        }

        public abstract Point2D GetAbsolutePosition();

        public double CalculateDistanceToOtherNode(Node other)
        {
            other = other.MustNotBeNull(nameof(other));
            return GetAbsolutePosition().CalculateMagnitude(other.GetAbsolutePosition());
        }
    }
}