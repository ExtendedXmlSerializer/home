using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class RuntimeSerializationExceptionMessage
		: DelegatedSource<TypeInfo, string>, IRuntimeSerializationExceptionMessage
	{
		public static IRuntimeSerializationExceptionMessage Default { get; }
			= new RuntimeSerializationExceptionMessage();

		RuntimeSerializationExceptionMessage() :
			base(x => @"Parameterized Content is enabled on the container.  By default, the type must satisfy the following rules if a public parameterless constructor is not found:

- Each member must not already be marked as an explicit contract
- Must be a public fields / property.
- Any public fields (spit) must be readonly
- Any public properties must have a get but not a set (on the public API, at least)
- There must be exactly one interesting constructor, with parameters that are a case-insensitive match for each field/property in some order (i.e. there must be an obvious 1:1 mapping between members and constructor parameter names)

More information can be found here: https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/222") {}
	}
}