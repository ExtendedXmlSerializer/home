using System;
using System.Reflection;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	sealed class MemberAdapter : Identity, IMemberAdapter
	{
		readonly ISpecification<object> _emit;
		readonly Func<object, object> _get;
		readonly Action<object, object> _set;

		public MemberAdapter(ISpecification<object> emit, string name, MemberInfo metadata, TypeInfo memberType,
		                     bool isWritable, Func<object, object> get, Action<object, object> set) : base(name, string.Empty)
		{
			_emit = emit;
			_get = get;
			_set = set;
			Metadata = metadata;
			MemberType = memberType;
			IsWritable = isWritable;
		}

		public MemberInfo Metadata { get; }
		public TypeInfo MemberType { get; }
		public bool IsWritable { get; }
		public object Get(object instance) => _get(instance);

		public void Assign(object instance, object value)
		{
			if (_emit.IsSatisfiedBy(value))
			{
				_set(instance, value);
			}
		}
	}
}