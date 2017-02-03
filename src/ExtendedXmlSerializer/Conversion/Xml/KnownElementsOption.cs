using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Collections;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Options;
using ExtendedXmlSerialization.Conversion.Xml.Converters;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public static class Defaults
	{
		public static ImmutableArray<IElement> Elements { get; } = new KnownElements().ToImmutableArray();
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

		// public IConverter Get(TypeInfo parameter)
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

		public ConverterOptions(IConverters converters) : this(converters, Elements.Default) {}

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
			: base(new MemberOption(converters), new ReadOnlyCollectionMemberOption(converters, XmlMemberAdorner.Default)) {}
	}

	public class MemberOption : Members.MemberOption
	{
		public MemberOption(IConverters converters) : base(converters, XmlVariableMemberAdorner.Default) {}
	}

	public class KnownElements : IEnumerable<IElement>
	{
		public virtual IEnumerator<IElement> GetEnumerator()
		{
			yield return new Element("boolean", typeof(bool));
			yield return new Element("char", typeof(char));
			yield return new Element("byte", typeof(sbyte));
			yield return new Element("unsignedByte", typeof(byte));
			yield return new Element("short", typeof(short));
			yield return new Element("unsignedShort", typeof(ushort));
			yield return new Element("int", typeof(int));
			yield return new Element("unsignedInt", typeof(uint));
			yield return new Element("long", typeof(long));
			yield return new Element("unsignedLong", typeof(ulong));
			yield return new Element("float", typeof(float));
			yield return new Element("double", typeof(double));
			yield return new Element("decimal", typeof(decimal));
			yield return new Element("dateTime", typeof(DateTime));
			yield return new Element("dateTimeOffset", typeof(DateTimeOffset));
			yield return new Element("string", typeof(string));
			yield return new Element("guid", typeof(Guid));
			yield return new Element("TimeSpan", typeof(TimeSpan));
			yield return new Element("Item", typeof(DictionaryEntry));
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
			yield return new GenericElementOption(_elements);
			yield return ElementOption.Default;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class KnownElementsOption : Conversion.Elements.KnownElementsOption
	{
		public static KnownElementsOption Default { get; } = new KnownElementsOption();
		KnownElementsOption() : base(Defaults.Elements) {}
	}
}