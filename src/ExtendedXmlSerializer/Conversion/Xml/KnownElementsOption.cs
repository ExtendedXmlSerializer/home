using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Conversion.Collections;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Options;
using ExtendedXmlSerialization.Conversion.Xml.Converters;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public static class Defaults
	{
		public static ImmutableArray<IElement> Elements { get; } = KnownElements.Default.ToImmutableArray();
	}

	class Roots : WeakCacheBase<TypeInfo, IConverter>, IRoots
	{
		public static Roots Default { get; } = new Roots();
		Roots() : this(Elements.Default, DefaultConverters.Default) {}

		readonly IElements _elements;
		readonly IConverters _converters;

		public Roots(IElements elements, IConverters converters)
		{
			_elements = elements;
			_converters = converters;
		}

		protected override IConverter Create(TypeInfo parameter)
			=> new Root(_elements.Get(parameter), _converters.Get(parameter));
	}

	public class DefaultConverters : Conversion.Converters
	{
		public static DefaultConverters Default { get; } = new DefaultConverters();
		DefaultConverters() : base(x => new ConverterOptions(x)) {}
	}

	class ConverterOptions : IConverterOptions
	{
		readonly IConverters _converters;
		readonly IElements _elements;
		readonly IAliasProvider _alias;

		public ConverterOptions(IConverters converters) : this(converters, Elements.Default, TypeAliasProvider.Default) {}

		public ConverterOptions(IConverters converters, IElements elements, IAliasProvider alias)
		{
			_converters = converters;
			_elements = elements;
			_alias = alias;
		}

		public IEnumerator<IConverterOption> GetEnumerator()
		{
			yield return BooleanTypeConverter.Default;
			yield return CharacterTypeConverter.Default;
			yield return ByteTypeConverter.Default;
			yield return UnsignedByteTypeConverter.Default;
			yield return ShortTypeConverter.Default;
			yield return UnsignedShortTypeConverter.Default;
			yield return IntegerTypeConverter.Default;
			yield return UnsignedIntegerTypeConverter.Default;
			yield return LongTypeConverter.Default;
			yield return UnsignedLongTypeConverter.Default;
			yield return FloatTypeConverter.Default;
			yield return DoubleTypeConverter.Default;
			yield return DecimalTypeConverter.Default;
			yield return EnumerationTypeConverter.Default;
			yield return DateTimeTypeConverter.Default;
			yield return DateTimeOffsetTypeConverter.Default;
			yield return StringTypeConverter.Default;
			yield return GuidTypeConverter.Default;
			yield return TimeSpanTypeConverter.Default;

			// yield return new DictionaryContext();
			yield return new CollectionOption(_converters, _elements, _alias);

			yield return new MemberedConverterOption(new TypeMembers(_converters));
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}


	class MemberSelector : OptionSelector<MemberInformation, IMember>, IMemberSelector
	{
		public MemberSelector(IConverters converters)
			: base(new MemberOption(converters), new ReadOnlyCollectionMemberOption(converters, MemberElementProvider.Default)) {}
	}

	class MemberElementProvider : IMemberElementProvider
	{
		public static MemberElementProvider Default { get; } = new MemberElementProvider();
		MemberElementProvider() : this(MemberAliasProvider.Default) {}

		readonly IAliasProvider _alias;

		public MemberElementProvider(IAliasProvider alias)
		{
			_alias = alias;
		}

		public IElement Get(MemberInformation parameter)
			=> new XmlElement(_alias.Get(parameter.Metadata) ?? parameter.Metadata.Name, parameter.MemberType, null);
	}

	public class MemberOption : Members.MemberOption
	{
		readonly ISpecification<TypeInfo> _specification;

		public MemberOption(IConverters converters) : this(FixedTypeSpecification.Default, converters) {}

		public MemberOption(ISpecification<TypeInfo> specification, IConverters converters)
			: base(converters, MemberElementProvider.Default)
		{
			_specification = specification;
		}

		protected override IMember CreateMember(IElement element, Action<object, object> setter, Func<object, object> getter, IConverter body)
		{
			var converter = _specification.IsSatisfiedBy(element.Classification)
				? body
				: new DecoratedConverter(body, new VariableTypeEmitter(element, body));
			var result = base.CreateMember(element, setter, getter, converter);
			return result;
		}
	}

	public class KnownElements : IEnumerable<IElement>
	{
		public static KnownElements Default { get; } = new KnownElements();
		KnownElements() : this(Namespaces.Default.Get(typeof(object).GetTypeInfo()).Namespace.NamespaceName) {}

		readonly string _ns;

		public KnownElements(string @namespace)
		{
			_ns = @namespace;
		}

		public virtual IEnumerator<IElement> GetEnumerator()
		{
			yield return new XmlElement("boolean", typeof(bool), _ns);
			yield return new XmlElement("char", typeof(char), _ns);
			yield return new XmlElement("byte", typeof(sbyte), _ns);
			yield return new XmlElement("unsignedByte", typeof(byte), _ns);
			yield return new XmlElement("short", typeof(short), _ns);
			yield return new XmlElement("unsignedShort", typeof(ushort), _ns);
			yield return new XmlElement("int", typeof(int), _ns);
			yield return new XmlElement("unsignedInt", typeof(uint), _ns);
			yield return new XmlElement("long", typeof(long), _ns);
			yield return new XmlElement("unsignedLong", typeof(ulong), _ns);
			yield return new XmlElement("float", typeof(float), _ns);
			yield return new XmlElement("double", typeof(double), _ns);
			yield return new XmlElement("decimal", typeof(decimal), _ns);
			yield return new XmlElement("dateTime", typeof(DateTime), _ns);
			yield return new XmlElement("dateTimeOffset", typeof(DateTimeOffset), _ns);
			yield return new XmlElement("string", typeof(string), _ns);
			yield return new XmlElement("guid", typeof(Guid), _ns);
			yield return new XmlElement("TimeSpan", typeof(TimeSpan), _ns);
			yield return new XmlElement("Item", typeof(DictionaryEntry), _ns);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class Elements : Conversion.Elements.Elements
	{
		public static Elements Default { get; } = new Elements();
		Elements() : base(x => new ElementOptions(x)) {}
	}

	public class ElementOptions : IElementOptions
	{
		readonly IElements _elements;

		public ElementOptions(IElements elements)
		{
			_elements = elements;
		}

		public IEnumerator<IElementOption> GetEnumerator()
		{
			yield return KnownElementsOption.Default;
			yield return new GenericElementOption(new GenericElementProvider(_elements));
			yield return ElementOption.Default;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class KnownElementsOption : Conversion.Elements.KnownElementsOption
	{
		public static KnownElementsOption Default { get; } = new KnownElementsOption();
		KnownElementsOption() : base(Defaults.Elements) {}
	}

	public class ElementOption : ElementOptionBase
	{
		public static ElementOption Default { get; } = new ElementOption();
		ElementOption() : this(Provider.Implementation) {}

		public ElementOption(IElementProvider provider) : base(provider.Get) {}

		class Provider : ElementProviderBase
		{
			public static Provider Implementation { get; } = new Provider();
			Provider() : this(Namespaces.Default, TypeAliasProvider.Default, TypeFormatter.Default) {}

			readonly INamespaces _namespaces;

			public Provider(INamespaces namespaces, IAliasProvider alias, ITypeFormatter formatter)
				: base(alias, formatter)
			{
				_namespaces = namespaces;
			}

			public override IElement Create(string displayName, TypeInfo classification)
				=> new XmlElement(displayName, classification, _namespaces.Get(classification).Namespace.NamespaceName);
		}
	}

	class GenericElementProvider : GenericElementProviderBase
	{
		readonly INamespaces _namespaces;

		public GenericElementProvider(IElements elements) : this(Namespaces.Default, elements) {}

		public GenericElementProvider(INamespaces namespaces, IElements elements)
			: base(elements.Get, TypeAliasProvider.Default)
		{
			_namespaces = namespaces;
		}

		protected override IElement Create(string displayName, TypeInfo classification,
		                                   ImmutableArray<IElement> arguments)
			=>
				new GenericXmlElement(displayName, classification, _namespaces.Get(classification).Namespace.NamespaceName,
				                      arguments);
	}

	class TypeAliasProvider : AliasProviderBase<TypeInfo>
	{
		public static TypeAliasProvider Default { get; } = new TypeAliasProvider();
		TypeAliasProvider() {}

		protected override string GetItem(TypeInfo parameter) => parameter.GetCustomAttribute<XmlRootAttribute>()?.ElementName;
	}
}