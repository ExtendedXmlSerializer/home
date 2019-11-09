using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Configuration
{
	public interface ITypeConfigurationContext : IContext, ISource<TypeInfo> {}
}