using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class VariableTypeIdentity : IWriter
	{
		readonly ISpecification<Type> _specification;
		readonly IWriter<object>      _start;
		readonly RuntimeElement       _runtime;

		public VariableTypeIdentity(Type definition, IIdentity identity, RuntimeElement runtime)
			: this(VariableTypeSpecification.Defaults.Get(definition), new Identity<object>(identity), runtime) {}

		public VariableTypeIdentity(ISpecification<Type> specification, IWriter<object> start, RuntimeElement runtime)
		{
			_specification = specification;
			_start         = start;
			_runtime       = runtime;
		}

		public void Write(IFormatWriter writer, object instance)
		{
			var type = instance.GetType();
			if (_specification.IsSatisfiedBy(type))
			{
				_runtime.Get(type.GetTypeInfo())
				        .Write(writer, instance);
			}
			else
			{
				_start.Write(writer, instance);
			}
		}
	}
}