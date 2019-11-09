using JetBrains.Annotations;
using System.Xml.Serialization;

namespace ExtendedXmlSerializer.Tests.TestObject
{
	public class TestClassInheritanceBase
	{
		public int Id { [UsedImplicitly] get; set; }

		public virtual void Init()
		{
			Id = 1;
		}
	}

	public class TestClassInheritance : TestClassInheritanceBase
	{
		public int Id2 { [UsedImplicitly] get; set; }

		public override void Init()
		{
			Id  = 2;
			Id2 = 3;
		}
	}

	public class TestClassInheritanceWithOrderBase
	{
		[XmlElement(Order = 2)]
		public int Id { get; set; }

		public virtual void Init()
		{
			Id = 1;
		}
	}

	public class TestClassInheritanceWithOrder : TestClassInheritanceWithOrderBase
	{
		public static TestClassInheritanceWithOrder Create()
		{
			var result = new TestClassInheritanceWithOrder();
			result.Init();
			return result;
		}

		[XmlElement(Order = 1)]
		public int Id2 { get; set; }

		public override void Init()
		{
			Id  = 2;
			Id2 = 3;
		}
	}

	public interface IInterfaceWithOrder
	{
		int Id { get; set; }
	}

	public class TestClassInterfaceInheritanceWithOrder : IInterfaceWithOrder
	{
		public int Id2 { get; set; }
		public int Id { get; set; }
	}
}