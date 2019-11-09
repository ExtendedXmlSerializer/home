using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypeFormatter : DecoratedSource<TypeInfo, string>, ITypeFormatter
	{
		public TypeFormatter(INames names) : base(names.Or(TypeNameFormatter.Default)) {}
	}
}