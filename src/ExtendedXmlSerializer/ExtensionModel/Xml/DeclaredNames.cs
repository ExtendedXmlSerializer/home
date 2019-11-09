using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class DeclaredNames : INames
	{
		public static DeclaredNames Default { get; } = new DeclaredNames();

		DeclaredNames() {}

		public string Get(TypeInfo parameter) => parameter.GetCustomAttribute<XmlRootAttribute>()
		                                                  ?
		                                                  .ElementName
		                                                  .NullIfEmpty() ?? parameter
		                                                                    .GetCustomAttribute<XmlTypeAttribute
		                                                                    >(false)
		                                                                    ?
		                                                                    .TypeName
		                                                                    .NullIfEmpty();
	}
}