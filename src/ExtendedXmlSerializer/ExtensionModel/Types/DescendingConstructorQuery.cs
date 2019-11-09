using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class DescendingConstructorQuery : IAlteration<IEnumerable<ConstructorInfo>>
	{
		public static DescendingConstructorQuery Default { get; } = new DescendingConstructorQuery();

		DescendingConstructorQuery() {}

		public IEnumerable<ConstructorInfo> Get(IEnumerable<ConstructorInfo> parameter)
			=> parameter.OrderByDescending(c => c.GetParameters()
			                                     .Length);
	}
}