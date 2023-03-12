using Light.GuardClauses;
using System.Diagnostics.CodeAnalysis;
// ReSharper disable all

namespace VweCore.Geometry
{
	public static class MapExtensions
    {
        public static bool TryFindIntersectingNodeLink(this Map map, StorageRow storageRow, [NotNullWhen(true)] out NodeLink? nodeLink, out LineSegment2D lineFromStorageRowToIntersection)
        {
            map = map.MustNotBeNull(nameof(map));
            storageRow = storageRow.MustNotBeNull(nameof(storageRow));

            nodeLink = null;
            lineFromStorageRowToIntersection = default;
            if (map.NodeLinks.Count == 0)
                return false;

            var perpendicularRay = storageRow.GetPerpendicularRay();
            var shortestDistance = default(double?);
            foreach (var link in map.NodeLinks)
            {
                if (!link.TryGetLineSegment(out var linkLineSegment))
                    continue;

                var intersection = perpendicularRay.LineEquation.CalculateIntersection(linkLineSegment.LineEquation);
                if (intersection == null || !perpendicularRay.IsPointOnRay(intersection.Value) || !linkLineSegment.IsPointOnLineSegment(intersection.Value))
                    continue;

                var distance = perpendicularRay.ReferencePoint.CalculateMagnitude(intersection.Value);
                if (shortestDistance != null && shortestDistance.Value < distance)
                    continue;

                shortestDistance = distance;
                nodeLink = link;
                lineFromStorageRowToIntersection = new LineSegment2D(perpendicularRay.ReferencePoint, intersection.Value);
            }

            return nodeLink != null;
        }
    }
}