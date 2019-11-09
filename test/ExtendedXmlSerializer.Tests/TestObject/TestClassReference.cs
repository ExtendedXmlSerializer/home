using JetBrains.Annotations;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassReferenceWithDictionary
	{
		public IReference Parent { get; set; }

		public Dictionary<int, IReference> All { get; set; }
	}

	public class TestClassReferenceWithList
	{
		public IReference Parent { get; set; }

		public List<IReference> All { get; set; }
	}

	public interface IReference
	{
		int Id { get; set; }
	}

	public class TestClassReference : IReference
	{
		public int Id { get; set; }

		public string Name { [UsedImplicitly] get; set; }

		public IReference CyclicReference { get; set; }
		public IReference ObjectA { get; set; }

		public IReference ReferenceToObjectA { get; set; }

		public List<IReference> Lists { get; set; }
	}

	public class TestClassConcreteReferenceWithDictionary
	{
		public TestClassConcreteReference Parent { get; set; }

		public Dictionary<int, TestClassConcreteReference> All { get; set; }
	}

	public class TestClassConcreteReferenceWithList
	{
		public TestClassConcreteReference Parent { get; set; }

		public List<TestClassConcreteReference> All { get; set; }
	}

	public class TestClassConcreteReference : IReference
	{
		public int Id { get; set; }
		public TestClassConcreteReference CyclicReference { get; set; }
		public TestClassConcreteReference ObjectA { get; set; }

		public TestClassConcreteReference ReferenceToObjectA { get; set; }

		public List<TestClassConcreteReference> Lists { get; set; }
	}
}