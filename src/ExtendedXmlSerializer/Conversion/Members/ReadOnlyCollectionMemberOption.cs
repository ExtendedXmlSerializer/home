using System;
using System.Collections;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class ReadOnlyCollectionMemberOption : MemberOptionBase
	{
		readonly IAddDelegates _add;

		public ReadOnlyCollectionMemberOption(IConverters converters)
			: this(converters, MemberAliasProvider.Default, AddDelegates.Default) {}

		public ReadOnlyCollectionMemberOption(IConverters converters, IAliasProvider alias, IAddDelegates add)
			: base(Specification.Instance, converters, alias)
		{
			_add = add;
		}

		protected override IMember Create(string displayName, TypeInfo classification, Func<object, object> getter,
		                                  IConverter body, MemberInfo metadata)
		{
			var add = _add.Get(classification);
			var result = add != null ? new ReadOnlyCollectionMember(displayName, add, getter, body) : null;
			return result;
		}

		class ReadOnlyCollectionMember : Member
		{
			public ReadOnlyCollectionMember(string displayName, Action<object, object> add, Func<object, object> getter,
			                                IConverter context) : base(displayName, add, getter, context) {}

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