using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	class WellKnownAliases : Dictionary<Type, string>
	{
		public static WellKnownAliases Default { get; } = new WellKnownAliases();
		WellKnownAliases() : base(new Dictionary<Type, string>
		                           {
			                           {typeof(bool), "boolean"},
			                           {typeof(char), "char"},
			                           {typeof(sbyte), "byte"},
			                           {typeof(byte), "unsignedByte"},
			                           {typeof(short), "short"},
			                           {typeof(ushort), "unsignedShort"},
			                           {typeof(int), "int"},
			                           {typeof(uint), "unsignedInt"},
			                           {typeof(long), "long"},
			                           {typeof(ulong), "unsignedLong"},
			                           {typeof(float), "float"},
			                           {typeof(double), "double"},
			                           {typeof(decimal), "decimal"},
			                           {typeof(DateTime), "dateTime"},
			                           {typeof(DateTimeOffset), "dateTimeOffset"},
			                           {typeof(string), "string"},
			                           {typeof(Guid), "guid"},
			                           {typeof(TimeSpan), "TimeSpan"},
			                           {typeof(DictionaryEntry), "Item"}
		                           }) {}
	}

	class TypeAliasProvider : AliasProviderBase<TypeInfo>
	{
		public static TypeAliasProvider Default { get; } = new TypeAliasProvider();
		TypeAliasProvider() : this(WellKnownAliases.Default) {}

		readonly IDictionary<Type, string> _names;

		public TypeAliasProvider(IDictionary<Type, string> names)
		{
			_names = names;
		}

		protected override string GetItem(TypeInfo parameter)
			=> _names.TryGet(parameter.AsType()) ?? parameter.GetCustomAttribute<XmlRootAttribute>()?.ElementName;
	}
}