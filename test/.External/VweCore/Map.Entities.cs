// ReSharper disable all
using Light.GuardClauses;
using System;
using System.Collections.Generic;
using VweCore.Abstractions;
using VweCore.Geometry;

namespace VweCore
{
	public sealed partial class Map
    {
        public T Add<T>(T entity)
            where T : class, IEntity
        {
            entity = entity.MustNotBeNull(nameof(entity));

            switch (entity)
            {
                case Node node:
                    AddAndSort(Nodes, node);
                    HomeNode = HomeNode is null && !(node is TagNode) ? node : HomeNode;
                    break;
                case Reflector reflector:
                    AddAndSort(Reflectors, reflector);
                    break;
                case Obstacle obstacle:
                    AddAndSort(Obstacles, obstacle);
                    break;
                case StorageLocation storageLocation:
                    AddAndSort(StorageLocations, storageLocation);
                    break;
                case StorageRow storageRow:
                    AddAndSort(StorageRows, storageRow);
                    break;
                case VirtualPoint virtualPoint:
                    AddAndSort(VirtualPoints, virtualPoint);
                    break;
                case NodeLink nodeLink:
                    AddAndSort(NodeLinks, nodeLink);
                    break;
                default:
                    throw new ArgumentException($"Unknown entity {entity} cannot be added to the map", nameof(entity));
            }

            return entity;
        }

        public bool Remove(IEntity entity)
        {
            entity = entity.MustNotBeNull(nameof(entity));

            switch (entity)
            {
                case Node node:
                    if (node.Equals(HomeNode))
                        HomeNode = null;
                    return Nodes.Remove(node);
                case Reflector reflector:
                    return Reflectors.Remove(reflector);
                case Obstacle obstacle:
                    return Obstacles.Remove(obstacle);
                case StorageRow storageRow:
                    return StorageRows.Remove(storageRow);
                case StorageLocation storageLocation:
                    return StorageLocations.Remove(storageLocation);
                case VirtualPoint virtualPoint:
                    return VirtualPoints.Remove(virtualPoint);
                case NodeLink nodeLink:
                    return NodeLinks.Remove(nodeLink);
                default:
                    return false;
            }
        }

        private static void AddAndSort<T>(List<T> entities, T entity)
            where T : class, IEntity
        {
            entities.Add(entity);
            entities.Sort();
        }

        public Obstacle CreateObstacle(int? obstacleId = null)
        {
            obstacleId ??= GetNextId(Obstacles);
            return new Obstacle
            {
                Id = obstacleId.Value,
                Point1 = new Point2D(0, 2),
                Point2 = new Point2D(0, -2)
            };
        }

        public Obstacle CloneObstacle(Obstacle obstacle, int minimumId = 1)
        {
            obstacle = obstacle.MustNotBeNull(nameof(obstacle));
            var id = GetNextId(Obstacles, minimumId);
            return obstacle.Clone(id);
        }

        public Reflector CreateReflector(int? reflectorId = null)
        {
            reflectorId ??= GetNextId(Reflectors);
            return new Reflector { Id = reflectorId.Value };
        }

        public Reflector CloneReflector(Reflector reflector)
        {
            reflector = reflector.MustNotBeNull(nameof(reflector));
            var id = GetNextId(Reflectors);
            return reflector.Clone(id);
        }

        public HallwayNode CreateHallwayNode(int? nodeId = null)
        {
            nodeId ??= GetNextId(Nodes);
            return new HallwayNode { Id = nodeId.Value };
        }

        public HallwayNode CloneHallwayNode(HallwayNode hallwayNode)
        {
            hallwayNode = hallwayNode.MustNotBeNull(nameof(hallwayNode));
            var id = GetNextId(Nodes);
            return hallwayNode.Clone(id);
        }

        public NodeLink CreateAttachedNodeLink(Node node1, Node node2, int? nodeLinkId = null, bool isBidirectional = true)
        {
            node1 = node1.MustNotBeNull(nameof(node1));
            node2 = node2.MustNotBeNull(nameof(node2));

            nodeLinkId ??= GetNextId(NodeLinks);
            return node1.AddLinkedNode(node2, nodeLinkId.Value, isBidirectional);
        }

        public NodeLink CreateDetachedNodeLink(int? nodeLinkId = null)
        {
            nodeLinkId ??= GetNextId(NodeLinks);
            return new NodeLink
            {
                Id = nodeLinkId.Value,
                Direction = NodeLinkDirection.Bidirectional,
                DrivingDirectionFromNode1 = NodeLinkDrivingDirection.Straight,
                DrivingDirectionFromNode2 = NodeLinkDrivingDirection.Straight,
                Point1 = new Point2D(-1, 0),
                Point2 = new Point2D(1, 0)
            };
        }

        public NodeLink CreateNodeLinkWithSingleAttachedNode(Node node1, Point2D point2, int? nodeLinkId = null)
        {
            node1 = node1.MustNotBeNull(nameof(node1));

            nodeLinkId ??= GetNextId(NodeLinks);
            return new NodeLink
            {
                Id = nodeLinkId.Value,
                Direction = NodeLinkDirection.Bidirectional,
                DrivingDirectionFromNode1 = NodeLinkDrivingDirection.Straight,
                DrivingDirectionFromNode2 = NodeLinkDrivingDirection.Straight,
                Node1 = node1,
                Point2 = point2
            };
        }

        public NodeLink CloneNodeLink(NodeLink nodeLink)
        {
            nodeLink = nodeLink.MustNotBeNull(nameof(nodeLink));
            var id = GetNextId(NodeLinks);
            return nodeLink.Clone(id);
        }

        public StorageRow CreateStorageRow(int? storageRowId = null)
        {
            storageRowId ??= GetNextId(StorageRows);
            var storageLocationIdProvider = GetIdProvider(StorageLocations);
            var markerPointIdProvider = GetIdProvider(Nodes);
            return new StorageRow
            {
                Id = storageRowId.Value,
                Width = 6,
                Height = 2
            }
                  .AddStorageLocation(CreateStorageLocation(new Point2D(-1, 0), new Point2D(1, 0).RotateAroundOrigin(-30), storageLocationIdProvider.GetNextId(), markerPointIdProvider.GetNextId()))
                  .AddStorageLocation(CreateStorageLocation(new Point2D(-3, 0), storageLocationId: storageLocationIdProvider.GetNextId(), markerPointId: markerPointIdProvider.GetNextId()))
                  .AddStorageLocation(CreateStorageLocation(new Point2D(-5, 0), storageLocationId: storageLocationIdProvider.GetNextId(), markerPointId: markerPointIdProvider.GetNextId()))
                  .RotateToAbsoluteAngle(270);
        }

        public StorageRow CloneStorageRow(StorageRow storageRow)
        {
            storageRow = storageRow.MustNotBeNull(nameof(storageRow));
            var nextId = GetNextId(StorageRows);
            return storageRow.Clone(nextId);
        }

        public StorageLocation CreateStorageLocation(Point2D? locationOffset = null, Point2D? markerPointOffset = null, int? storageLocationId = null, int? markerPointId = null)
        {
            storageLocationId ??= GetNextId(StorageLocations);
            return new StorageLocation
            {
                Id = storageLocationId.Value,
                StorageRowOffset = locationOffset ?? default,
                Diameter = 1.5
            }
               .AddMarkerPoint(CreateMarkerPoint(markerPointOffset, markerPointId));
        }

        public StorageLocation CloneStorageLocation(StorageLocation storageLocation)
        {
            storageLocation = storageLocation.MustNotBeNull(nameof(storageLocation));
            var id = GetNextId(StorageLocations);
            return storageLocation.Clone(id);
        }

        public MarkerPoint CreateMarkerPoint(Point2D? offset = null, int? markerPointId = null)
        {
            markerPointId ??= GetNextId(Nodes);
            return new MarkerPoint
            {
                Id = markerPointId.Value,
                StorageLocationOffset = offset ?? new Point2D(1, 0)
            };
        }

        public MarkerPoint CloneMarkerPoint(MarkerPoint markerPoint)
        {
            markerPoint = markerPoint.MustNotBeNull(nameof(markerPoint));
            var id = GetNextId(Nodes);
            return markerPoint.Clone(id);
        }

        public TagNode CreateTagNode(int? nodeId = null)
        {
            nodeId ??= GetNextId(Nodes);
            return new TagNode { Id = nodeId.Value };
        }

        public TagNode CloneTagNode(TagNode tagNode)
        {
            tagNode = tagNode.MustNotBeNull(nameof(tagNode));
            var id = GetNextId(Nodes);
            return tagNode.Clone(id);
        }

        public VirtualPoint CreateVirtualPoint(NodeLink nodeLink, int? virtualPointId = null, double? offset = null)
        {
            nodeLink = nodeLink.MustNotBeNull(nameof(nodeLink));

            virtualPointId ??= GetNextId(VirtualPoints);
            return new VirtualPoint
            {
                Id = virtualPointId.Value,
                NodeLink = nodeLink,
                Origin = VirtualPointOrigin.Position1,
                Offset = offset ?? nodeLink.DetermineOffsetForNewVirtualPoint(VirtualPointOrigin.Position1)
            };
        }

        public VirtualPoint CloneVirtualPoint(VirtualPoint virtualPoint)
        {
            virtualPoint = virtualPoint.MustNotBeNull(nameof(virtualPoint));
            var id = GetNextId(VirtualPoints);
            return virtualPoint.Clone(id);
        }

        public static EntityIdProvider<T> GetIdProvider<T>(List<T> entities)
            where T : class, IEntity =>
            new EntityIdProvider<T>(entities);

        private static int GetNextId<T>(List<T> entities, int minimumId = 1)
            where T : class, IEntity
        {
            minimumId.MustNotBeLessThan(1, nameof(minimumId));

            if (entities.Count == 0)
                return minimumId;

            if (entities.Count == 1)
                return entities[0].Id == minimumId ? minimumId + 1 : minimumId;

            var previousId = 0;
            var currentId = 0;
            for (var i = 0; i < entities.Count; i++)
            {
                currentId = entities[i].Id;

                // First check if the current ID is less than the minimum ID
                // or if there is no gap between the current and the previous ID.
                // If these cases apply, then continue searching
                if (currentId < minimumId || previousId + 1 == currentId)
                    goto ContinueSearching;

                // If there is a gap, then return 'previousId + 1' if minimum ID allows it
                if (previousId + 1 >= minimumId)
                    return previousId + 1;

                // If 'previousId + 1' didn't work, check if the minimum ID is within the gap
                if (minimumId < currentId)
                    return minimumId;

                // If this wouldn't work, then we have to continue searching
                ContinueSearching:
                previousId = currentId;
            }

            // If nothing could be found, return either the next free ID after the last element or the minimum ID
            return Math.Max(minimumId, currentId + 1);
        }

#pragma warning disable CA1034 // This struct needs to access the private method Map.GetNextId and must thus be nested
        public ref struct EntityIdProvider<T>
#pragma warning restore CA1034
            where T : class, IEntity
        {
            public EntityIdProvider(List<T> entities)
            {
                Entities = entities;
                LastId = 0;
            }

            private int LastId { get; set; }

            private List<T> Entities { get; }

            public int GetNextId() => LastId = Map.GetNextId(Entities, LastId + 1);
        }
    }
}