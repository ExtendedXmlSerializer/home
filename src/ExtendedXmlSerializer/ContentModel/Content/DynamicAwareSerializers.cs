using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class DynamicAwareSerializers : ISerializers
	{
		readonly ISpecification<TypeInfo> _specification;
		readonly ISerializers             _previous;

		public DynamicAwareSerializers(ISerializers previous) : this(IsAnonymousType.Default, previous) {}

		public DynamicAwareSerializers(ISpecification<TypeInfo> specification, ISerializers previous)
		{
			_specification = specification;
			_previous      = previous;
		}

		public ISerializer Get(TypeInfo parameter)
			=> _specification.IsSatisfiedBy(parameter)
				   ? throw new InvalidOperationException("Dynamic/anonymous types are not supported.")
				   : _previous.Get(parameter);
	}
}