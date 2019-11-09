using JetBrains.Annotations;
using System;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassTimeSpan
	{
		public void Init()
		{
			TimeSpan = TimeSpan.FromMilliseconds(1561);
		}

		public TimeSpan TimeSpan { [UsedImplicitly] get; set; }
	}
}