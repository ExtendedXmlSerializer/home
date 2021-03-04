using System.Linq;
using VweCore.Geometry;

namespace VweCore.Tests
{
    public static class TestMapFactory
    {
        public static Map CreateMapWithDefaultData()
        {
            var map = new Map
            {
                Id = 1,
                Description = "An example map",
                Level = 1
            }
                     .AddHallwayNodes()
                     .AddTagNodes()
                     .AddVirtualPoints()
                     .AddObstacles()
                     .AddReflectors()
                     .AddStorageRows()
                     .InterconnectHallwayNodesWithMarkerNodes();
            return map;
        }

        private static Map AddHallwayNodes(this Map map)
        {
            var node1 = new HallwayNode { Id = 1, Position = new Point2D(2, 1), DirectionalAngleInDegrees = 0, Radius = 1 };
            var node2 = new HallwayNode { Id = 2, Position = new Point2D(4, 1), DirectionalAngleInDegrees = 0, Radius = 1 };
            var node3 = new HallwayNode { Id = 3, Position = new Point2D(6, 1), DirectionalAngleInDegrees = 0, Radius = 1 };
            var node4 = new HallwayNode { Id = 4, Position = new Point2D(8, 1), DirectionalAngleInDegrees = 0, Radius = 1 };
            map.Add(node1);
            map.Add(node2);
            map.Add(node3);
            map.Add(node4);

            map.Add(map.CreateAttachedNodeLink(node1, node2));
            map.Add(map.CreateAttachedNodeLink(node2, node3));
            map.Add(map.CreateAttachedNodeLink(node3, node4));

            return map;
        }

        private static Map AddTagNodes(this Map map)
        {
            var node1 = new TagNode { Id = 5, RfId = 15, Position = new Point2D(2, 1), DirectionalAngleInDegrees = 0, Radius = 1 };
            var node2 = new TagNode { Id = 6, RfId = 16, Position = new Point2D(4, 1), DirectionalAngleInDegrees = 0, Radius = 1 };
            map.Add(node1);
            map.Add(node2);

            map.Add(map.CreateAttachedNodeLink(node1, node2));

            return map;
        }

        private static Map AddVirtualPoints(this Map map)
        {
            var node1 = new VirtualPoint() { Id = 1, Origin = VirtualPointOrigin.Position1 };
            var node2 = new VirtualPoint() { Id = 2, Origin = VirtualPointOrigin.Position2 };
            var node3 = new VirtualPoint() { Id = 3, Origin = VirtualPointOrigin.Position1 };
            var node4 = new VirtualPoint() { Id = 4, Origin = VirtualPointOrigin.Position2 };

            map.Add(node1);
            map.Add(node2);
            map.Add(node3);
            map.Add(node4);

            var tagNode = map.Nodes.First(n => n is TagNode);
            var nodeLink = tagNode.NodeLinks.First();

            nodeLink.AddVirtualPointAndCalculateItsOffset(node1);
            nodeLink.AddVirtualPointAndCalculateItsOffset(node2);
            nodeLink.AddVirtualPointAndCalculateItsOffset(node3);
            nodeLink.AddVirtualPointAndCalculateItsOffset(node4);

            return map;
        }

        private static Map AddObstacles(this Map map)
        {
            map.Add(new Obstacle { Id = 1, Point1 = new Point2D(0, 0), Point2 = new Point2D(0, 10) });
            map.Add(new Obstacle { Id = 2, Point1 = new Point2D(0, 0), Point2 = new Point2D(6, 0) });
            map.Add(new Obstacle { Id = 3, Point1 = new Point2D(6, 0), Point2 = new Point2D(6, 10) });
            return map;
        }

        private static Map AddReflectors(this Map map)
        {
            map.Add(new Reflector { Id = 1, Position = new Point2D(0, 3) });
            return map;
        }

        private static Map AddStorageRows(this Map map)
        {
            var storageRow = map.CreateStorageRow();
            storageRow.AngleInDegrees = 90;
            storageRow.Position = new Point2D(4, 5);
            map.Add(storageRow);
            foreach (var storageLocation in storageRow.StorageLocations)
            {
                map.Add(storageLocation);
                foreach (var markerPoint in storageLocation.MarkerPoints)
                {
                    map.Add(markerPoint);
                }
            }
            return map;
        }

        private static Map InterconnectHallwayNodesWithMarkerNodes(this Map map)
        {
            var storageRow = map.StorageRows[0];
            var hallwayNode1 = map.Nodes[0];
            var markerPointForFirstStorageLocation = storageRow.StorageLocations[0].MarkerPoints[0];
            map.Add(map.CreateAttachedNodeLink(hallwayNode1, markerPointForFirstStorageLocation));

            var hallwayNode2 = map.Nodes[2];
            var markerPointForSecondStorageLocation = storageRow.StorageLocations[1].MarkerPoints[0];
            map.Add(map.CreateAttachedNodeLink(hallwayNode2, markerPointForSecondStorageLocation));

            var markerPointForThirdStorageLocation = storageRow.StorageLocations[2].MarkerPoints[0];
            map.Add(map.CreateAttachedNodeLink(markerPointForSecondStorageLocation, markerPointForThirdStorageLocation));

            return map;
        }
    }
}