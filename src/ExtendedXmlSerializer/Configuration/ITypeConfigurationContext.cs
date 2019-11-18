using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// This is considered internal code and not to be used by external applications.
	/// </summary>
	public interface ITypeConfigurationContext : IContext, ISource<TypeInfo> {}
}