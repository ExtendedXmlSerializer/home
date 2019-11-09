using System;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class ItemTypeProperty : Property<TypeInfo>
	{
		public static ItemTypeProperty Default { get; } = new ItemTypeProperty();

		ItemTypeProperty() : this(new FrameworkIdentity("item")) {}

		public ItemTypeProperty(IIdentity identity)
			: base(new Reader(new TypedParsingReader(identity)), new TypedFormattingWriter(identity), identity) {}

		sealed class Reader : IReader<TypeInfo>
		{
			readonly IReader<TypeInfo>              _reader;
			readonly IProperty<ImmutableArray<int>> _maps;

			public Reader(IReader<TypeInfo> reader) : this(reader, MapProperty.Default) {}

			public Reader(IReader<TypeInfo> reader, IProperty<ImmutableArray<int>> maps)
			{
				_reader = reader;
				_maps   = maps;
			}

			public TypeInfo Get(IFormatReader parameter)
			{
				var element = _reader.Get(parameter);
				var result = element != null
					             ? TypeInfo(parameter, element)
						             .GetTypeInfo()
					             : null;
				return result;
			}

			Type TypeInfo(IFormatReader parameter, TypeInfo element)
			{
				if (parameter.IsSatisfiedBy(_maps))
				{
					var maps = _maps.Get(parameter);
					if (maps.Length > 1)
					{
						return element.MakeArrayType(maps.Length);
					}
				}

				return element.MakeArrayType();
			}
		}
	}
}