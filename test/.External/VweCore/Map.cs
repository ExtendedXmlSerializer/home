using System;
using System.Collections.Generic;
using Light.GuardClauses;

#pragma warning disable CA2227 // XML serialization requires settable collection properties

namespace VweCore
{
    public sealed partial class Map
    {
        private List<NodeLink>? _nodeLinks;
        private List<Node>? _nodes;
        private List<Obstacle>? _obstacles;
        private List<Reflector>? _reflectors;
        private List<StorageRow>? _storageRows;
        private List<StorageLocation>? _storageLocations;
        private List<VirtualPoint>? _virtualPoints;

        public int Id { get; set; } = 1;

        public string? Description { get; set; } = "Generated with KS Control Visual Warehouse Editor";

        public int Level { get; set; } = 1;

        public int? Version { get; set; } = 5;

        public Node? HomeNode { get; set; }

        public double DefaultMaxRadius { get; set; } = 1.0;

        public double DefaultRadius { get; set; } = 1.0;

        public CurveType CurveType { get; set; } = CurveType.Spline4;

        public Guid CurrentVersion { get; set; } = Guid.NewGuid();

        public string? Name { get; set; } = "New Map";

        public List<Node> Nodes
        {
            get => _nodes ??= new List<Node>();
            set => _nodes = value.MustNotBeNull();
        }

        public List<Reflector> Reflectors
        {
            get => _reflectors ??= new List<Reflector>();
            set => _reflectors = value.MustNotBeNull();
        }

        public List<Obstacle> Obstacles
        {
            get => _obstacles ??= new List<Obstacle>();
            set => _obstacles = value.MustNotBeNull();
        }

        public List<StorageRow> StorageRows
        {
            get => _storageRows ??= new List<StorageRow>();
            set => _storageRows = value.MustNotBeNull();
        }

        public List<StorageLocation> StorageLocations
        {
            get => _storageLocations ??= new List<StorageLocation>();
            set => _storageLocations = value.MustNotBeNull();
        }

        public List<NodeLink> NodeLinks
        {
            get => _nodeLinks ??= new List<NodeLink>();
            set => _nodeLinks = value.MustNotBeNull();
        }

        public List<VirtualPoint> VirtualPoints
        {
            get => _virtualPoints ??= new List<VirtualPoint>();
            set => _virtualPoints = value.MustNotBeNull();
        }

        public void SetNewVersion() => CurrentVersion = Guid.NewGuid();
    }
}