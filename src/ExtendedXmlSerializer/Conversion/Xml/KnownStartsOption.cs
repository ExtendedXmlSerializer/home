using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Conversion.Collections;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Options;
using ExtendedXmlSerialization.Conversion.Xml.Converters;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	

	class Roots : WeakCacheBase<TypeInfo, IConverter>, IRoots
	{
		public static Roots Default { get; } = new Roots();
		Roots() : this(Elements.Elements.Default, DefaultConverters.Default) {}

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
		
		public ConverterOptions(IConverters converters) : this(converters, Elements.Elements.Default) {}

		public ConverterOptions(IConverters converters, IElements elements)
		{
			_converters = converters;
			_elements = elements;
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
			yield return new CollectionOption(_converters, _elements);

			yield return new MemberedConverterOption(new TypeMembers(_converters));
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}


	class MemberSelector : OptionSelector<MemberInformation, IMember>, IMemberSelector
	{
		public MemberSelector(IConverters converters)
			: base(new MemberOption(converters), new ReadOnlyCollectionMemberOption(converters, MemberAliasProvider.Default)) {}
	}

	/*class MemberElementProvider : IMemberElementProvider
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
	}*/

	public class KnownElements : IEnumerable<IXmlElement>
	{
		public static KnownElements Default { get; } = new KnownElements();
		KnownElements() : this(Namespaces.Default.Get(typeof(object).GetTypeInfo()).Namespace.NamespaceName) {}

		readonly string _ns;

		public KnownElements(string @namespace)
		{
			_ns = @namespace;
		}

		public virtual IEnumerator<IXmlElement> GetEnumerator()
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

	public class ElementOptions : IElementOptions
	{
		readonly IElements _elements;

		public ElementOptions(IElements elements)
		{
			_elements = elements;
		}

		public IEnumerator<IStartOption> GetEnumerator()
		{
			yield return KnownStartsOption.Default;
			yield return new GenericStartOption(new GenericStartElementProvider(_elements));
			yield return StartOption.Default;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	

	public class StartOption : StartOptionBase
	{
		public static StartOption Default { get; } = new StartOption();
		StartOption() : base(StartElementProvider.Default.Get) {}
	}

	class TypeAliasProvider : AliasProviderBase<TypeInfo>
	{
		public static TypeAliasProvider Default { get; } = new TypeAliasProvider();
		TypeAliasProvider() {}

		protected override string GetItem(TypeInfo parameter) => parameter.GetCustomAttribute<XmlRootAttribute>()?.ElementName;
	}
}