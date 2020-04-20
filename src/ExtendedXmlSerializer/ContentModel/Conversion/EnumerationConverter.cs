using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

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
			       new Writer(enumerationType, new TableSource<string, string>(values).Get).Get) {}

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
				=> parameter != null ? (Enum)Enum.Parse(_enumerationType, _values(parameter) ?? parameter) : default;
		}

		sealed class Writer : IFormatter<Enum>
		{
			readonly Type _type;
			readonly Func<string, string> _source;

			public Writer(Type type, Func<string, string> source)
			{
				_type = type;
				_source = source;
			}

			public string Get(Enum parameter) => _source(parameter.ToString()) ?? Enum.Format(_type, parameter, "F");
		}
	}
}