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
		readonly IAddDelegates _add;

		public ReadOnlyCollectionMemberOption(IConverters converters, IMemberElementProvider provider) : this(converters, provider, AddDelegates.Default) {}

		public ReadOnlyCollectionMemberOption(IConverters converters, IMemberElementProvider provider, IAddDelegates add)
			: base(Specification.Instance, converters, provider)
		{
			_add = add;
		}
		protected override IMember Create(IElement element, Func<object, object> getter, IConverter body, MemberInfo metadata)
		{
			var add = _add.Get(element.Classification);
			var result = add != null ? new ReadOnlyCollectionMember(element, add, getter, body) : null;
			return result;
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