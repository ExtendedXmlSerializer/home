using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
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
		Roots() : this(Emitters.Default, DefaultConverters.Default) {}

		readonly IEmitters _emitters;
		readonly IConverters _converters;

		public Roots(IEmitters emitters, IConverters converters)
		{
			_emitters = emitters;
			_converters = converters;
		}

		protected override IConverter Create(TypeInfo parameter)
			=> new Root(_emitters.Get(parameter), _converters.Get(parameter));
	}

	public class DefaultConverters : Conversion.Converters
	{
		public static DefaultConverters Default { get; } = new DefaultConverters();
		DefaultConverters() : base(x => new ConverterOptions(x)) {}
	}

	class ConverterOptions : IConverterOptions
	{
		readonly IConverters _converters;

		public ConverterOptions(IConverters converters)
		{
			_converters = converters;
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
			yield return new CollectionOption(_converters);

			yield return new MemberedConverterOption(new TypeMembers(_converters));
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}


	class MemberSelector : OptionSelector<MemberInformation, IMember>, IMemberSelector
	{
		public MemberSelector(IConverters converters)
			: base(new MemberOption(converters), new ReadOnlyCollectionMemberOption(converters)) {}
	}


	public class EmitterOption : EmitterOptionBase
	{
		public static EmitterOption Default { get; } = new EmitterOption();
		EmitterOption() {}

		public override IEmitter Create(XName name, TypeInfo classification)
			=> new StartElement(name.LocalName, name.NamespaceName);
	}
}