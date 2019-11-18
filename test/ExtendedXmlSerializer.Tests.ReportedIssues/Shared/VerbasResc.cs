using JetBrains.Annotations;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Tests.ReportedIssues.Shared
{
	public class VerbasResc
	{
		public List<DmDev> DmDev { [UsedImplicitly] get; set; }
	}
}