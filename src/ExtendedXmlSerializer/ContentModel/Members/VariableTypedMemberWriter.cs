using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class VariableTypedMemberWriter : IWriter
	{
		readonly ISpecification<Type> _type;
		readonly ISerializer          _runtime;
		readonly IWriter              _body;
		readonly IProperty<TypeInfo>  _property;

		public VariableTypedMemberWriter(Type type, ISerializer runtime, IWriter body)
			: this(VariableTypeSpecification.Defaults.Get(type), runtime, body) {}

		public VariableTypedMemberWriter(ISpecification<Type> type, ISerializer runtime, IWriter body)
			: this(type, runtime, body, ExplicitTypeProperty.Default) {}

		// ReSharper disable once TooManyDependencies
		public VariableTypedMemberWriter(ISpecification<Type> type, ISerializer runtime, IWriter body,
		                                 IProperty<TypeInfo> property)
		{
			_type     = type;
			_runtime  = runtime;
			_body     = body;
			_property = property;
		}

		public void Write(IFormatWriter writer, object instance)
		{
			var type = instance?.GetType();
			if (type != null)
			{
				if (_type.IsSatisfiedBy(type))
				{
					_property.Write(writer, type.GetTypeInfo());
					_runtime.Write(writer, instance);
				}
				else
				{
					_body.Write(writer, instance);
				}
			}
			else
			{
				writer.Content(null);
			}
		}
	}
}