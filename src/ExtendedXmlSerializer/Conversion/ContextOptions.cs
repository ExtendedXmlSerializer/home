using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Model;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Conversion.Xml.Converters;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion
{
	class ContextOptions : IContextOptions
	{
		readonly static MemberSpecification<FieldInfo> Field =
			new MemberSpecification<FieldInfo>(FieldMemberSpecification.Default);

		readonly static MemberSpecification<PropertyInfo> Property =
			new MemberSpecification<PropertyInfo>(PropertyMemberSpecification.Default);

		readonly IContexts _contexts;
		readonly INames _names;
		readonly IActivators _activators;
		readonly ICollectionItemTypeLocator _locator;
		readonly IAddDelegates _add;
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;

		public ContextOptions(IContexts contexts, INames names)
			: this(contexts, names, new Activators(), new CollectionItemTypeLocator(), new AddMethodLocator()) {}

		public ContextOptions(IContexts contexts, INames names, IActivators activators, ICollectionItemTypeLocator locator,
		                      IAddMethodLocator add)
			: this(contexts, names, activators, locator, new AddDelegates(locator, add), Property, Field) {}

		public ContextOptions(IContexts contexts, INames names, IActivators activators, ICollectionItemTypeLocator locator,
		                      IAddDelegates add, ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
		{
			_contexts = contexts;
			_names = names;
			_activators = activators;
			_locator = locator;
			_add = add;
			_property = property;
			_field = field;
		}

		public IEnumerator<IContextOption> GetEnumerator()
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
			yield return
				new CollectionContextOption(_contexts, new CollectionItemNameProvider(_locator, _names), _activators, _add);

			var members = new ContextMembers(new MemberContextSelector(_contexts, _add), _property, _field);
			yield return new ActivatedContextOption(_activators, members);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}