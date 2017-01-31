using System;
using System.Collections;
using System.Reflection;
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

		public ReadOnlyCollectionMemberOption(IConverters converters, IAddDelegates add)
			: this(converters, GetterFactory.Default, add) {}

		public ReadOnlyCollectionMemberOption(IConverters converters, IGetterFactory getter, IAddDelegates add)
			: base(Specification.Instance)
		{
			_converters = converters;
			_getter = getter;
			_add = add;
		}

		protected override IMemberConverter Create(IMemberName name)
		{
			var add = _add.Get(name.MemberType);
			if (add != null)
			{
				var getter = _getter.Get(name.Metadata);
				var result = new ReadOnlyCollectionMember(name, add, getter, _converters.Get(name.MemberType));
				return result;
			}
			return null;
		}

		public class ReadOnlyCollectionMember : MemberConverter
		{
			public ReadOnlyCollectionMember(IMemberName name, Action<object, object> add, Func<object, object> getter,
			                                IConverter context)
				: base(name, context, add, getter) {}

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