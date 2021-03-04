using System;
using System.Diagnostics.CodeAnalysis;
using VweCore.Geometry;

namespace VweCore.Abstractions
{
    public interface IEntity : IEquatable<IEntity>, IComparable<IEntity>
    {
        int Id { get; set; }

        Guid InternalId { get; set; }

        string DisplayString { get; }

        void SetAssociatedDiagramItem(object diagramItem);

        EntityOrderIndex OrderIndex { get; }

        bool TryGetAssociatedDiagramItem<TDiagramItem>([NotNullWhen(true)] out TDiagramItem? diagramItem)
            where TDiagramItem : class;

        TDiagramItem GetAssociatedDiagramItem<TDiagramItem>()
            where TDiagramItem : class;

        TDiagramItem DisconnectDiagramItem<TDiagramItem>()
            where TDiagramItem : class;

        LeveledRectangle GetBounds();
    }
}