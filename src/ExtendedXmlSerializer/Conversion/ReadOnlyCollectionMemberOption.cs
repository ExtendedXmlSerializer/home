using System;
using System.Collections;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Model;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion
{
	public class ReadOnlyCollectionMemberOption : MemberOptionBase
	{
		readonly IContexts _contexts;
		readonly IGetterFactory _getter;
		readonly IAddDelegates _add;

		public ReadOnlyCollectionMemberOption(IContexts contexts, IAddDelegates add)
			: this(contexts, GetterFactory.Default, add) {}

		public ReadOnlyCollectionMemberOption(IContexts contexts, IGetterFactory getter, IAddDelegates add)
			: base(Specification.Instance)
		{
			_contexts = contexts;
			_getter = getter;
			_add = add;
		}

		protected override IMemberContext Create(IMemberName name)
		{
			var add = _add.Get(name.MemberType);
			if (add != null)
			{
				var getter = _getter.Get(name.Metadata);
				var result = new ReadOnlyCollectionMember(name, add, getter, _contexts.Get(name.MemberType));
				return result;
			}
			return null;
		}

		public class ReadOnlyCollectionMember : MemberContext
		{
			public ReadOnlyCollectionMember(IMemberName name, Action<object, object> add, Func<object, object> getter,
			                                IElementContext context)
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