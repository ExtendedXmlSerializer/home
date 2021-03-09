using System;
using System.Diagnostics.CodeAnalysis;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
using Light.GuardClauses.FrameworkExtensions;
using VweCore.Abstractions;
using VweCore.Geometry;
using VweCore.Translations;

#pragma warning disable CA1036 // We will not add operator overrides for IComparable<IEntity> to this type because it should not be used like a primitive value (e.g. int)

namespace VweCore
{
    public abstract class Entity<T> : IEntity
        where T : Entity<T>
    {
        private object? _associatedDiagramItem;
        private int _id;
        private string? _displayString;

        protected Entity(EntityOrderIndex orderIndex, string typeIdentifierTranslationKey)
        {
            OrderIndex = orderIndex.MustNotBe(EntityOrderIndex.TemporaryItems, nameof(orderIndex));
            _id = -1;
            TypeIdentifier = Translator.Instance.GetTranslation(
                typeIdentifierTranslationKey.MustNotBeNullOrWhiteSpace(nameof(typeIdentifierTranslationKey)));
        }

        protected string TypeIdentifier { get; }

        public int Id
        {
            get => _id;
            set
            {
                if (_id == value)
                    return;

                _id = value;
                _displayString = CreateDisplayString();
            }
        }

        public Guid InternalId { get; set; } = Guid.NewGuid();

        public EntityOrderIndex OrderIndex { get; }

        public abstract LeveledRectangle GetBounds();

        public void SetAssociatedDiagramItem(object diagramItem) =>
            _associatedDiagramItem = diagramItem.MustNotBeNull(nameof(diagramItem));

        public bool TryGetAssociatedDiagramItem<TDiagramItem>([NotNullWhen(true)] out TDiagramItem? diagramItem)
            where TDiagramItem : class
        {
            if (_associatedDiagramItem is TDiagramItem item)
            {
                diagramItem = item;
                return true;
            }

            diagramItem = null;
            return false;
        }

        public TDiagramItem GetAssociatedDiagramItem<TDiagramItem>()
            where TDiagramItem : class
        {
            if (!TryGetAssociatedDiagramItem<TDiagramItem>(out var diagramItem))
                ThrowNoAssociatedDiagramItem<TDiagramItem>();

            return diagramItem;
        }

        public string DisplayString => _displayString ??= CreateDisplayString();

        public TDiagramItem DisconnectDiagramItem<TDiagramItem>()
            where TDiagramItem : class
        {
            if (!TryGetAssociatedDiagramItem<TDiagramItem>(out var item))
                ThrowNoAssociatedDiagramItem<TDiagramItem>();

            _associatedDiagramItem = null;
            return item;
        }

        public int CompareTo(IEntity other)
        {
            other = other.MustNotBeNull(nameof(other));
            var orderIndexResult = ((int) OrderIndex).CompareTo((int) other.OrderIndex);
            return orderIndexResult != 0 ? orderIndexResult : Id.CompareTo(other.Id);
        }

        public bool Equals(IEntity? other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is null)
                return false;
            return InternalId == other.InternalId;
        }

        public bool Equals(T? other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is null)
                return false;

            return InternalId == other.InternalId;
        }

        [DoesNotReturn]
        private void ThrowNoAssociatedDiagramItem<TDiagramItem>()
            where TDiagramItem : class =>
            Throw.InvalidOperation($"The entity \"{this}\" is not associated with a diagram item of type \"{typeof(TDiagramItem)}\", but actually with {_associatedDiagramItem.ToStringOrNull()}.");

        public int CompareTo(T? other) => Id.CompareTo(other.MustNotBeNull(nameof(other)).Id);

        public override bool Equals(object obj) => obj is T entity && Equals(entity);

        // ReSharper disable once NonReadonlyMemberInGetHashCode -- Internal ID is only set once after construction or deserialization, and cannot be altered after that. This is due to limitations in ExtendedXmlSerializer
        public override int GetHashCode() => InternalId.GetHashCode();

        public override string ToString() => DisplayString;

        private string CreateDisplayString() => $"{TypeIdentifier} {Id}";
    }
}