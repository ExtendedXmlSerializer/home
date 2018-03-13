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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue176
	{
		[Fact]
		public void Verify()
		{
			var test = new SerializableProject
			{
				Cases = new List<Case>
				{
					new SimpleCase
					{
						Name = "Simple",
						Number = 1,
						ModesCount = 2,
						Records = new List<LoadRecord>
						{
							new LoadRecord
							{
								Description = "DO NOT WORK :("
							}
						}
					},
					new CaseCombination()
					{
						Number = 2,
						Name = "A",
						label = "C"
					}
				}
			};
			var support = new ConfigurationContainer().ForTesting();

			var result = support.Cycle(test);

			var caseObj = result.Cases[0];
			caseObj.Should().BeOfType<SimpleCase>();

			// ShouldBeEquivalentTo doesn't work
			support.Cycle(test).ShouldBeEquivalentTo(test);
		}

		[Fact]
		public void VerifyOptimized()
		{
			var test = new SerializableProject
			{
				Cases = new List<Case>
				{
					new SimpleCase
					{
						Name       = "Simple",
						Number     = 1,
						ModesCount = 2,
						Records = new List<LoadRecord>
						{
							new LoadRecord
							{
								Description = "DO NOT WORK :("
							}
						}
					},
					new CaseCombination()
					{
						Number = 2,
						Name   = "A",
						label  = "C"
					}
				}
			};
			var support = new ConfigurationContainer()
			              .UseOptimizedNamespaces() //WITHOUT IT WORKS
			              .ForTesting();

			var result = support.Cycle(test);

			var caseObj = result.Cases[0];
			caseObj.Should().BeOfType<SimpleCase>();

			// ShouldBeEquivalentTo doesn't work
			support.Cycle(test).ShouldBeEquivalentTo(test);
		}

		public class Case
		{
			public int Number { get; set; }
			public string Name { get; set; }
		}

		public class SimpleCase : Case
		{
			public List<LoadRecord> Records { get; set; }

			public int ModesCount { get; set; }
		}

		public class CaseCombination : Case
		{
			public string label { get; set; }

		}

		public class LoadRecord
		{
			public string Description { get; set; }
		}

		public class SerializableProject
		{
			public List<Case> Cases { get; set; }
		}
	}
}