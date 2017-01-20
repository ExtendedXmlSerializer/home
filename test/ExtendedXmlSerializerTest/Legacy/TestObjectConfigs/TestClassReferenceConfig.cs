// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Xml.Linq;
using ExtendedXmlSerialization.Legacy;
using ExtendedXmlSerialization.Test.TestObject;
#pragma warning disable 618

namespace ExtendedXmlSerialization.Test.Legacy.TestObjectConfigs
{
	public class TestClassReferenceConfig : ExtendedXmlSerializerConfig<TestClassReference>
	{
		public TestClassReferenceConfig()
		{
			ObjectReference( obj => obj.Id );
		}
	}

	public class TestClassConcreteReferenceConfig : ExtendedXmlSerializerConfig<TestClassConcreteReference>
	{
		public TestClassConcreteReferenceConfig()
		{
			ObjectReference( obj => obj.Id );
		}
	}

	public class InterfaceReferenceConfig : ExtendedXmlSerializerConfig<IReference>
	{
		public InterfaceReferenceConfig()
		{
			ObjectReference( obj => obj.Id );
		}
	}

	public class TestClassInheritanceWithMigrationsBaseConfig : ExtendedXmlSerializerConfig<TestClassInheritanceWithMigrationsBase>
	{
		public TestClassInheritanceWithMigrationsBaseConfig()
		{
			AddMigration(MigrationBase);
		}

		public static void MigrationBase(XElement xElement)
		{
			xElement.Element("Property").Name = "ChangedProperty";
		}
	}
	public class TestClassInheritanceWithMigrationsAConfig : ExtendedXmlSerializerConfig<TestClassInheritanceWithMigrationsA>
	{
		public TestClassInheritanceWithMigrationsAConfig()
		{
			AddMigration(Migration);
		}

		private void Migration(XElement xElement)
		{
			TestClassInheritanceWithMigrationsBaseConfig.MigrationBase(xElement);
			xElement.Element("OtherProperty").Name = "OtherChangedProperty";
		}
	}
	public class TestClassInheritanceWithMigrationsBConfig : ExtendedXmlSerializerConfig<TestClassInheritanceWithMigrationsB>
	{
		public TestClassInheritanceWithMigrationsBConfig()
		{
			AddMigration(Migration);
		}

		private void Migration(XElement xElement)
		{
			TestClassInheritanceWithMigrationsBaseConfig.MigrationBase(xElement);
		}
	}
}