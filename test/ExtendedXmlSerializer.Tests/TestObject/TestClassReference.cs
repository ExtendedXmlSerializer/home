// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nagórski
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

		public string Name { get; set; }

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