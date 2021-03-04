using System.Collections.Generic;
using Light.GuardClauses;
using VweCore.Abstractions;
using VweCore.Geometry;

namespace VweCore.FrameworkExtensions
{
    public static class CollectionExtensions
    {
        public static T Pop<T>(this List<T> list)
        {
            list = list.MustNotBeNull(nameof(list));
            Check.InvalidOperation(list.Count == 0, "A list must have at least one item in it before calling Pop, but it actually has zero items.");

            var lastIndex = list.Count - 1;
            var item = list[lastIndex];
            list.RemoveAt(lastIndex);
            return item;
        }

        public static void AddCapacityBound<T>(this List<T> list, T item)
        {
            list = list.MustNotBeNull(nameof(list));
            item.MustNotBeNullReference(nameof(item));

            if (list.Count == list.Capacity)
                list.RemoveAt(0);
            list.Add(item);
        }

        public static bool ContainsOnlyNodes(this List<IEntity> entities)
        {
            entities = entities.MustNotBeNull(nameof(entities));

            foreach (var entity in entities)
            {
                if (!(entity is Node))
                    return false;
            }

            return true;
        }

        public static LeveledRectangle CalculateBounds(this List<IEntity> entities)
        {
            entities = entities.MustNotBeNull(nameof(entities));
            if (entities.Count == 0)
                return LeveledRectangle.Empty;

            var bounds = entities[0].GetBounds();
            for (var i = 1; i < entities.Count; i++)
            {
                bounds = bounds.Extend(entities[i].GetBounds());
            }

            return bounds;
        }
    }
}