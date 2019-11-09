using System;
using System.Collections.Immutable;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Properties;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class GenericIdentity<T> : IWriter<T>
	{
		readonly IProperty<ImmutableArray<Type>> _property;
		readonly IWriter<T>                      _start;
		readonly ImmutableArray<Type>            _arguments;

		public GenericIdentity(IIdentity identity, ImmutableArray<Type> arguments) : this(new Identity<T>(identity),
		                                                                                  arguments) {}

		public GenericIdentity(IWriter<T> start, ImmutableArray<Type> arguments)
			: this(start, ArgumentsTypeProperty.Default, arguments) {}

		public GenericIdentity(IWriter<T> start, IProperty<ImmutableArray<Type>> property,
		                       ImmutableArray<Type> arguments)
		{
			_start     = start;
			_property  = property;
			_arguments = arguments;
		}

		public void Write(IFormatWriter writer, T instance)
		{
			_start.Write(writer, instance);
			_property.Write(writer, _arguments);
		}
	}
}