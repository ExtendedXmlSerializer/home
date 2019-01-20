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

using System;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using XmlReader = System.Xml.XmlReader;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class InstanceReader : IInstanceReader
	{
		readonly IExtendedXmlSerializer _serializer;
		readonly IInstances             _instances;

		public InstanceReader(IExtendedXmlSerializer serializer) : this(serializer, Instances.Default) {}

		public InstanceReader(IExtendedXmlSerializer serializer, IInstances instances)
		{
			_serializer = serializer;
			_instances  = instances;
		}

		public object Get(Existing parameter)
		{
			using (new Context(_instances, parameter.Reader))
			{
				_instances.Assign(parameter.Reader, parameter.Instance);

				var result = _serializer.Deserialize(parameter.Reader);
				return result;
			}
		}

		struct Context : IDisposable
		{
			readonly ITableSource<XmlReader, object> _table;
			readonly XmlReader                       _key;

			public Context(ITableSource<XmlReader, object> table, XmlReader key)
			{
				_table = table;
				_key   = key;
			}

			public void Dispose()
			{
				_table.Remove(_key);
			}
		}
	}
}