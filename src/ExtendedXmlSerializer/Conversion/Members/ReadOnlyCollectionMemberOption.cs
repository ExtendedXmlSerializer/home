using System;
using System.Collections;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class ReadOnlyCollectionMemberOption : MemberOptionBase
	{
		readonly IConverters _converters;
		readonly IGetterFactory _getter;
		readonly IAddDelegates _add;

		public ReadOnlyCollectionMemberOption(IConverters converters) : this(converters, GetterFactory.Default, AddDelegates.Default) {}

		public ReadOnlyCollectionMemberOption(IConverters converters, IGetterFactory getter, IAddDelegates add)
			: base(Specification.Instance)
		{
			_converters = converters;
			_getter = getter;
			_add = add;
		}

		protected override IMember Create(IElement element, MemberInfo metadata)
		{
			var add = _add.Get(element.Classification);
			if (add != null)
			{
				var getter = _getter.Get(metadata);
				var result = new ReadOnlyCollectionMember(element, add, getter, _converters.Get(element.Classification));
				return result;
			}
			return null;
		}

		class ReadOnlyCollectionMember : Member
		{
			public ReadOnlyCollectionMember(IElement element, Action<object, object> add, Func<object, object> getter,
			                                IConverter context) : base(element, add, getter, context) {}

			public override void Assign(object instance, object value)
			{
				var collection = Get(instance);
				foreach (var element in value.AsValid<IEnumerable>())
				{
					base.Assign(collection, element);
				}
			}
		}

		sealed class Specification : ISpecification<MemberInformation>
		{
			public static Specification Instance { get; } = new Specification();
			Specification() : this(IsCollectionTypeSpecification.Default) {}
			readonly ISpecification<TypeInfo> _specification;

			Specification(ISpecification<TypeInfo> specification)
			{
				_specification = specification;
			}

			public bool IsSatisfiedBy(MemberInformation parameter) => _specification.IsSatisfiedBy(parameter.MemberType);
		}
	}
}