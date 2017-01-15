using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Legacy;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.ElementModel
{
    public class ElementNameOption<T> : ElementNameOptionBase
    {
        public static ElementNameOption<T> Default { get; } = new ElementNameOption<T>();

        ElementNameOption()
            : base(
                IsAssignableSpecification<T>.Default.Adapt(), x => new ElementName(x.ToTypeInfo())) {}
    }

    public class KnownElementNames : ElementNameOptionBase
    {
        public KnownElementNames(IEnumerable<IElementName> elements)
            : this(elements.ToDictionary(x => (MemberInfo) x.Classification)) {}

        public KnownElementNames(IDictionary<MemberInfo, IElementName> names)
            : base(new DelegatedSpecification<MemberInfo>(names.ContainsKey), names.TryGet) {}
    }

    public class DefaultElementNameOption : ElementNameOptionBase
    {
        public static DefaultElementNameOption Default { get; } = new DefaultElementNameOption();
        DefaultElementNameOption() : base(ElementNameProvider.Default.Get) {}
    }

    /*public class MemberNameOption : ElementNameOptionBase
    {
        public static MemberNameOption Default { get; } = new MemberNameOption();
        MemberNameOption() : base(MemberElementNameProvider.Default.Get) {}
    }*/

    public class EnumerableNameOption : ElementNameOptionBase
    {
        public static EnumerableNameOption Default { get; } = new EnumerableNameOption();
        EnumerableNameOption() : this(LegacyEnumerableElementNameProvider.Default.Get) {}

        public EnumerableNameOption(Func<MemberInfo, IElementName> source)
            : base(Specification.Instance, source) {}

        sealed class Specification : ISpecification<MemberInfo>
        {
            readonly private static TypeInfo TypeInfo = typeof(IEnumerable).GetTypeInfo();

            public static Specification Instance { get; } = new Specification();
            Specification() {}

            public bool IsSatisfiedBy(MemberInfo parameter)
            {
                var type = parameter.ToTypeInfo();
                var result = type.IsArray || type.IsGenericType && TypeInfo.IsAssignableFrom(type);
                return result;
            }
        }
    }

    public interface IElementOption : IOption<TypeInfo, IElement> {}

    public abstract class ElementOptionBase : OptionBase<TypeInfo, IElement>, IElementOption
    {
        private readonly IElementNameSelector _names;

        protected ElementOptionBase(IElementNameSelector names) : this(AlwaysSpecification<MemberInfo>.Default, names) {}

        protected ElementOptionBase(ISpecification<TypeInfo> specification, IElementNameSelector names)
            : base(specification)
        {
            _names = names;
        }

        public override IElement Get(TypeInfo parameter) => Create(parameter, _names.Get(parameter));

        protected abstract IElement Create(TypeInfo parameter, IElementName name);
    }

    public interface IDeclaredTypeElement : IElement
    {
        TypeInfo DeclaredType { get; }
    }

    public interface ICollectionElement : IElement
    {
        ICollectionItem Item { get; }
    }

    public interface ICollectionElement<out T> : ICollectionElement where T : ICollectionItem
    {
        new T Item { get; }
    }

    public interface IActivatedElement : IMemberedElement {}

    public interface IMemberedElement : IElement
    {
        IMembers Members { get; }
    }

    public class ActivatedElement : Element, IActivatedElement
    {
        public ActivatedElement(IElementName name, IMembers members) : base(name)
        {
            Members = members;
        }

        public IMembers Members { get; }
    }

    public interface IArrayElement : ICollectionElement {}

    public class ArrayElement : Element, IArrayElement
    {
        public ArrayElement(IElementName name, ICollectionItem item) : base(name)
        {
            Item = item;
        }

        public ICollectionItem Item { get; }
    }

    public class CollectionElementBase<T> : ActivatedElement, ICollectionElement<T> where T : ICollectionItem
    {
        public CollectionElementBase(IElementName name, IMembers members, T item) : base(name, members)
        {
            Item = item;
        }

        public T Item { get; }

        ICollectionItem ICollectionElement.Item => Item;
    }

    public class CollectionElement : CollectionElementBase<ICollectionItem>, ICollectionElement
    {
        public CollectionElement(IElementName name, IMembers members, ICollectionItem item)
            : base(name, members, item) {}
    }

    public interface ICollectionItem : IDeclaredTypeElement {}

    public class CollectionItem : DeclaredTypeElementBase, ICollectionItem
    {
        public CollectionItem(IElementName name, TypeInfo elementType) : base(name, elementType) {}
    }

    public interface IDictionaryItem : ICollectionItem
    {
        IDictionaryKeyElement Key { get; }

        IDictionaryValueElement Value { get; }
    }

    public class DictionaryItem : CollectionItem, IDictionaryItem
    {
        public static TypeInfo DictionaryEntryType { get; } = typeof(DictionaryEntry).GetTypeInfo();

        public DictionaryItem(IDictionaryKeyElement key, IDictionaryValueElement value)
            : base(ItemProperty.Default, DictionaryEntryType)
        {
            Value = value;
            Key = key;
        }

        public IDictionaryKeyElement Key { get; }
        public IDictionaryValueElement Value { get; }
    }

    public interface IDictionaryKeyElement : IDeclaredTypeElement {}

    public interface IDictionaryValueElement : IDeclaredTypeElement {}

    public interface IElementSelector : ISelector<TypeInfo, IElement> {}

    public interface IElementNameProvider : IParameterizedSource<MemberInfo, IElementName> {}

    class Elements : Selector<TypeInfo, IElement>, IElementSelector
    {
        public static Elements Default { get; } = new Elements();
        Elements() : this(ElementNames.Default, ElementMembers.Default) {}

        protected Elements(IElementNameSelector names, IElementMembers members) : this(
            new DictionaryElementOption(names, members),
            new ArrayElementOption(names),
            new CollectionElementOption(names, members),
            new ActivatedElementOption(names, members),
            new ElementOption(names)) {}

        protected Elements(params IOption<TypeInfo, IElement>[] options) : base(options) {}
    }

    public abstract class CollectionElementOptionBase : ActivatedElementOptionBase
    {
        private readonly ICollectionItemFactory _items;

        protected CollectionElementOptionBase(ISpecification<TypeInfo> specification,
                                              IElementNameSelector names, IElementMembers members)
            : this(specification, names, members, new CollectionItemFactory(names)) {}

        protected CollectionElementOptionBase(ISpecification<TypeInfo> specification,
                                              IElementNameSelector names, IElementMembers members,
                                              ICollectionItemFactory items)
            : base(specification, names, members)
        {
            _items = items;
        }

        protected override IElement CreateElement(TypeInfo parameter, IElementName name, IMembers members)
            => Create(parameter, name, members, _items.Get(parameter));

        protected abstract IElement Create(TypeInfo collectionType, IElementName name, IMembers members,
                                           ICollectionItem item);
    }

    public class ElementOption : ElementOptionBase
    {
        public ElementOption(IElementNameSelector names) : base(names) {}

        protected override IElement Create(TypeInfo parameter, IElementName name) => new Element(name);
    }

    public class ArrayElementOption : ElementOptionBase
    {
        private readonly ICollectionItemFactory _items;

        public ArrayElementOption(IElementNameSelector names) : this(names, new CollectionItemFactory(names)) {}

        public ArrayElementOption(IElementNameSelector names, ICollectionItemFactory items)
            : base(IsArraySpecification.Default, names)
        {
            _items = items;
        }

        protected override IElement Create(TypeInfo parameter, IElementName name)
            => new ArrayElement(name, _items.Get(parameter));
    }

    public interface ICollectionItemFactory : IParameterizedSource<TypeInfo, ICollectionItem> {}

    class CollectionItemFactory : ICollectionItemFactory
    {
        private readonly ICollectionItemTypeLocator _locator;
        private readonly IElementNameSelector _names;
        public CollectionItemFactory(IElementNameSelector names) : this(CollectionItemTypeLocator.Default, names) {}

        public CollectionItemFactory(ICollectionItemTypeLocator locator, IElementNameSelector names)
        {
            _locator = locator;
            _names = names;
        }

        public ICollectionItem Get(TypeInfo parameter)
        {
            var elementType = _locator.Get(parameter);
            var name = _names.Get(elementType);
            var result = new CollectionItem(name, elementType);
            return result;
        }
    }

    public abstract class ActivatedElementOptionBase : ElementOptionBase
    {
        private readonly IElementMembers _members;

        protected ActivatedElementOptionBase(ISpecification<TypeInfo> specification, IElementNameSelector names,
                                             IElementMembers members)
            : base(specification, names)
        {
            _members = members;
        }

        protected override IElement Create(TypeInfo parameter, IElementName name)
            => CreateElement(parameter, name, _members.Get(parameter));

        protected abstract IElement CreateElement(TypeInfo parameter, IElementName name, IMembers members);
    }

    public class ActivatedElementOption : ActivatedElementOptionBase
    {
        /*public static ActivatedElementOption Default { get; } = new ActivatedElementOption();
        ActivatedElementOption() : this(ElementMembers.Default, ElementNames.Default) {}*/

        public ActivatedElementOption(IElementNameSelector names, IElementMembers members)
            : base(IsActivatedTypeSpecification.Default, names, members) {}

        protected override IElement CreateElement(TypeInfo parameter, IElementName name, IMembers members)
            => new ActivatedElement(name, members);
    }

    public class CollectionElementOption : CollectionElementOptionBase
    {
        public CollectionElementOption(IElementNameSelector names, IElementMembers members)
            : base(IsCollectionTypeSpecification.Default, names, members) {}

        protected override IElement Create(TypeInfo collectionType, IElementName name, IMembers members,
                                           ICollectionItem item)
            => new CollectionElement(name, members, item);
    }

    class DictionaryElementOption : CollectionElementOptionBase
    {
        private readonly IDictionaryPairTypesLocator _locator;

        public DictionaryElementOption(IElementNameSelector names, IElementMembers members)
            : this(names, members, DictionaryPairTypesLocator.Default) {}

        public DictionaryElementOption(IElementNameSelector names, IElementMembers members,
                                       IDictionaryPairTypesLocator locator)
            : base(IsDictionaryTypeSpecification.Default, names, members)
        {
            _locator = locator;
        }

        protected override IElement Create(TypeInfo collectionType, IElementName name, IMembers members,
                                           ICollectionItem elementType)
        {
            var pair = _locator.Get(collectionType);
            var item = new DictionaryItem(new DictionaryKeyElement(pair.KeyType),
                                          new DictionaryValueElement(pair.ValueType));
            var result = new DictionaryElement(name, members, item);
            return result;
        }
    }
}