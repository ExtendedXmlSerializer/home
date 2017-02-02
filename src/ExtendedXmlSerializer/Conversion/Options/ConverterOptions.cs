using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Collections;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Conversion.Xml.Converters;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Options
{
	class ConverterOptions : IConverterOptions
	{
		readonly static MemberSpecification<FieldInfo> Field =
			new MemberSpecification<FieldInfo>(FieldMemberSpecification.Default);

		readonly static MemberSpecification<PropertyInfo> Property =
			new MemberSpecification<PropertyInfo>(PropertyMemberSpecification.Default);

		readonly IConverters _converters;
		readonly IElementProvider _elements;
		readonly IMemberAdorner _adorner;
		readonly IActivators _activators;
		readonly IAddDelegates _add;
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;

		public ConverterOptions(IConverters converters, IElements elements, IMemberAdorner adorner)
			: this(converters, elements, adorner, new Activators(), new CollectionItemTypeLocator(), new AddMethodLocator()) {}

		public ConverterOptions(IConverters converters, IElements elements, IMemberAdorner adorner, IActivators activators,
		                        ICollectionItemTypeLocator locator, IAddMethodLocator add)
			: this(
				converters, new CollectionItemElementProvider(locator, elements), adorner, activators,
				new AddDelegates(locator, add), Property, Field) {}

		public ConverterOptions(IConverters converters, IElementProvider elements, IMemberAdorner adorner,
		                        IActivators activators, IAddDelegates add,
		                        ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
		{
			_converters = converters;
			_elements = elements;
			_adorner = adorner;
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
			yield return new CollectionOption(_converters, _elements, _activators, _add);

			var members = new TypeMembers(new MemberSelector(_converters, _add, _adorner), _property, _field);
			yield return new MemberedConverterOption(_activators, members);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}