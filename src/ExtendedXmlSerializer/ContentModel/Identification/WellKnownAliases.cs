using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	class WellKnownAliases : ReadOnlyDictionary<Type, string>
	{
		public static IReadOnlyDictionary<Type, string> Default { get; } = new WellKnownAliases();

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
}