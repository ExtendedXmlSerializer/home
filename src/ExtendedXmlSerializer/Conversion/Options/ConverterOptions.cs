using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Collections;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Names;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Conversion.Xml.Converters;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;
using INames = ExtendedXmlSerialization.Conversion.Names.INames;

namespace ExtendedXmlSerialization.Conversion.Options
{
	class ConverterOptions : IConverterOptions
	{
		readonly static MemberSpecification<FieldInfo> Field =
			new MemberSpecification<FieldInfo>(FieldMemberSpecification.Default);

		readonly static MemberSpecification<PropertyInfo> Property =
			new MemberSpecification<PropertyInfo>(PropertyMemberSpecification.Default);

		readonly IConverters _converters;
		readonly INameProvider _names;
		readonly IActivators _activators;
		readonly IAddDelegates _add;
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;

		public ConverterOptions(IConverters converters, INames names)
			: this(converters, names, new Activators(), new CollectionItemTypeLocator(), new AddMethodLocator()) {}

		public ConverterOptions(IConverters converters, INames names, IActivators activators, ICollectionItemTypeLocator locator,
		                        IAddMethodLocator add)
			: this(converters, new CollectionItemNameProvider(locator, names), activators, new AddDelegates(locator, add), Property, Field) {}

		public ConverterOptions(IConverters converters, INameProvider names, IActivators activators, IAddDelegates add,
		                        ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
		{
			_converters = converters;
			_names = names;
			_activators = activators;
			_add = add;
			_property = property;
			_field = field;
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
			yield return new CollectionConverterOption(_converters, _names, _activators, _add);

			var members = new ConverterMembers(new MemberSelector(_converters, _add), _property, _field);
			yield return new ActivatedConverterOption(_activators, members);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}