using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class EnumerationConverter : Converter<Enum>
	{
		public EnumerationConverter(Type enumerationType)
			: this(enumerationType,
			       enumerationType.GetTypeInfo()
			                      .GetMembers()
			                      .Where(x => x.MemberType == MemberTypes.Field)
			                      .ToDictionary(x => x.Name, x => x.GetCustomAttribute<XmlEnumAttribute>()
			                                                       ?.Name ?? x.Name)) {}

		public EnumerationConverter(Type enumerationType, IDictionary<string, string> values)
			: base(new Reader(enumerationType, new TableSource<string, string>(values.ToDictionary(x => x.Value, x => x.Key)).Get).Get,
			       new Writer(new TableSource<string, string>(values).Get).Get) {}

		sealed class Reader : IParameterizedSource<string, Enum>
		{
			readonly Type                 _enumerationType;
			readonly Func<string, string> _values;

			public Reader(Type enumerationType, Func<string, string> values)
			{
				_enumerationType = enumerationType;
				_values          = values;
			}

			public Enum Get(string parameter)
				=> parameter != null ? (Enum)Enum.Parse(_enumerationType, _values(parameter)) : default;
		}

		sealed class Writer : IFormatter<Enum>
		{
			readonly Func<string, string> _source;

			public Writer(Func<string, string> source) => _source = source;

			public string Get(Enum parameter) => _source(parameter.ToString());
		}
	}
}