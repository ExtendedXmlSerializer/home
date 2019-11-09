using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using JetBrains.Annotations;
using System.Reflection;
using Identity = ExtendedXmlSerializer.ContentModel.Identification.Identity;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class SchemaTypeExtension : ISerializerExtension
	{
		public static SchemaTypeExtension Default { get; } = new SchemaTypeExtension();

		SchemaTypeExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IClassification, Classification>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Classification : IClassification
		{
			readonly IClassification   _classification;
			readonly IReader<TypeInfo> _reader;

			[UsedImplicitly]
			public Classification(IClassification classification) : this(classification, Reader.Instance) {}

			public Classification(IClassification classification, IReader<TypeInfo> reader)
			{
				_classification = classification;
				_reader         = reader;
			}

			public TypeInfo Get(IFormatReader parameter)
			{
				var isSatisfiedBy = parameter.IsSatisfiedBy(SchemaType.Instance);
				return isSatisfiedBy
					       ? _reader.Get(parameter)
					       : _classification.Get(parameter);
			}
		}

		sealed class SchemaType : Identity
		{
			public static SchemaType Instance { get; } = new SchemaType();

			SchemaType() : base("type", "http://www.w3.org/2001/XMLSchema-instance") {}
		}

		sealed class Reader : TypedParsingReader
		{
			public static Reader Instance { get; } = new Reader();

			Reader() : base(SchemaType.Instance) {}
		}
	}
}