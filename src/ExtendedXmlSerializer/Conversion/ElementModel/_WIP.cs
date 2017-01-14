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

        protected ElementOptionBase(params IElementNameOption[] names)
            : this(AlwaysSpecification<MemberInfo>.Default, names) {}

        protected ElementOptionBase(ISpecification<TypeInfo> specification, params IElementNameOption[] names)
            : this(specification, new ElementNames(names)) {}

        protected ElementOptionBase(ISpecification<TypeInfo> specification) : this(specification, ElementNames.Default) {}

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

    public interface ICollectionElement : ICollectionElement<ICollectionItem> {}

    public interface ICollectionElement<out T> : IElement where T : ICollectionItem
    {
        T Item { get; }
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
    }

    public class CollectionElement : CollectionElementBase<ICollectionItem>, ICollectionElement
    {
        public CollectionElement(IElementName name, IMembers members, ICollectionItem item)
            : base(name, members, item) {}
    }

    public interface ICollectionItem : IElement
    {
        TypeInfo ElementType { get; }
    }

    public class CollectionItem : Element, ICollectionItem
    {
        public CollectionItem(TypeInfo elementType) : this(ItemProperty.Default, elementType) {}

        public CollectionItem(IElementName name, TypeInfo elementType) : base(name)
        {
            ElementType = elementType;
        }

        public TypeInfo ElementType { get; }
    }

    public interface IDictionaryItem : ICollectionItem
    {
        IDictionaryKeyElement Key { get; }

        IDictionaryValueElement Value { get; }
    }

    public class DictionaryItem : CollectionItem, IDictionaryItem
    {
        public static TypeInfo DictionaryEntryType { get; } = typeof(DictionaryEntry).GetTypeInfo();

        public DictionaryItem(IDictionaryKeyElement key, IDictionaryValueElement value) : base(DictionaryEntryType)
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

    class Elements : OptionSelector<TypeInfo, IElement>, IElementSelector
    {
        public static Elements Default { get; } = new Elements();

        Elements() : this(
            DictionaryElementOption.Default,
            ArrayElementOption.Default,
            CollectionElementOption.Default,
            ActivatedElementOption.Default) {}

        protected Elements(params IOption<TypeInfo, IElement>[] options) : base(options) {}
    }

    public abstract class CollectionElementOptionBase : ActivatedElementOptionBase
    {
        readonly private static ElementNames Names = new ElementNames(EnumerableNameOption.Default,
                                                                      DefaultElementNameOption.Default);

        private readonly ICollectionItemTypeLocator _locator;

        protected CollectionElementOptionBase(ISpecification<TypeInfo> specification)
            : this(CollectionItemTypeLocator.Default, specification, ElementMembers.Default) {}

        protected CollectionElementOptionBase(ICollectionItemTypeLocator locator,
                                              ISpecification<TypeInfo> specification, IElementMembers members)
            : base(specification, members, Names)
        {
            _locator = locator;
        }

        protected override IElement CreateElement(TypeInfo parameter, IElementName name, IMembers members)
            => Create(parameter, name, members, _locator.Get(parameter));

        protected abstract IElement Create(TypeInfo collectionType, IElementName name, IMembers members,
                                           TypeInfo elementType);
    }

    /*public class ElementOption : ElementOptionBase
    {
        public static ElementOption Default { get; } = new ElementOption();
        ElementOption() : base(DefaultElementNameOption.Default) {}

        protected override IElement Create(TypeInfo parameter, IElementName name) => new Element(name);
    }*/

    public class ArrayElementOption : ElementOptionBase
    {
        public static ArrayElementOption Default { get; } = new ArrayElementOption();
        private ArrayElementOption() : this(CollectionItemTypeLocator.Default) {}

        private readonly ICollectionItemTypeLocator _locator;

        public ArrayElementOption(ICollectionItemTypeLocator locator)
            : base(IsArraySpecification.Default, EnumerableNameOption.Default)
        {
            _locator = locator;
        }

        protected override IElement Create(TypeInfo parameter, IElementName name)
            => new ArrayElement(name, new CollectionItem(_locator.Get(parameter)));
    }

    public abstract class ActivatedElementOptionBase : ElementOptionBase
    {
        private readonly IElementMembers _members;

        protected ActivatedElementOptionBase(ISpecification<TypeInfo> specification, IElementMembers members,
                                             IElementNameSelector names)
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
        public static ActivatedElementOption Default { get; } = new ActivatedElementOption();
        ActivatedElementOption() : this(ElementMembers.Default, ElementNames.Default) {}

        public ActivatedElementOption(IElementMembers members, IElementNameSelector names)
            : base(IsActivatedTypeSpecification.Default, members, names) {}

        protected override IElement CreateElement(TypeInfo parameter, IElementName name, IMembers members)
            => new ActivatedElement(name, members);
    }

    public class CollectionElementOption : CollectionElementOptionBase
    {
        public static CollectionElementOption Default { get; } = new CollectionElementOption();
        CollectionElementOption() : base(IsCollectionTypeSpecification.Default) {}

        protected override IElement Create(TypeInfo collectionType, IElementName name, IMembers members,
                                           TypeInfo elementType)
            => new CollectionElement(name, members, new CollectionItem(elementType));
    }

    class DictionaryElementOption : CollectionElementOptionBase
    {
        public static DictionaryElementOption Default { get; } = new DictionaryElementOption();

        DictionaryElementOption() : this(DictionaryPairTypesLocator.Default) {}

        private readonly IDictionaryPairTypesLocator _locator;

        public DictionaryElementOption(IDictionaryPairTypesLocator locator)
            : base(IsDictionaryTypeSpecification.Default)
        {
            _locator = locator;
        }

        protected override IElement Create(TypeInfo collectionType, IElementName name, IMembers members,
                                           TypeInfo elementType)
        {
            var pair = _locator.Get(collectionType);
            var item = new DictionaryItem(new DictionaryKeyElement(pair.KeyType),
                                          new DictionaryValueElement(pair.ValueType));
            var result = new DictionaryElement(name, members, item);
            return result;
        }
    }
}