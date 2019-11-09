using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class Names : Cache<TypeInfo, string>, INames
	{
		public Names(IParameterizedSource<TypeInfo, string> source) : base(source.Get) {}
	}
}