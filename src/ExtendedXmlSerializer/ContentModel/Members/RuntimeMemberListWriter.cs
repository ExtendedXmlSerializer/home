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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.ContentModel.Xml;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class RuntimeMemberListWriter : IWriter
	{
		readonly ImmutableArray<IRuntimeMember> _runtime;
		readonly ImmutableArray<Writer> _properties, _content;

		public RuntimeMemberListWriter(ImmutableArray<IRuntimeMember> runtime, ImmutableArray<IMember> members)
			: this(members.OfType<IPropertyMember>().ToImmutableArray(), runtime, members) {}

		RuntimeMemberListWriter(ImmutableArray<IPropertyMember> properties,
		                        ImmutableArray<IRuntimeMember> runtime, ImmutableArray<IMember> members)
			: this(properties.Select(x => new Writer(x, x.Adapter)).ToImmutableArray(),
			       runtime,
			       members.RemoveRange(properties.CastArray<IMember>())
			              .RemoveRange(runtime.CastArray<IMember>())
			              .Select(x => new Writer(x, x.Adapter))
			              .ToImmutableArray()) {}

		RuntimeMemberListWriter(ImmutableArray<Writer> properties, ImmutableArray<IRuntimeMember> runtime,
		                        ImmutableArray<Writer> content)
		{
			_properties = properties;
			_runtime = runtime;
			_content = content;
		}

		public void Write(IXmlWriter writer, object instance)
		{
			var runtime = _runtime.ToArray();
			var properties = _properties.ToArray();
			var contents = _content.ToArray();
			
			var runtimeProperties = Properties(instance).ToArray();

			foreach (var property in properties.Concat(runtimeProperties.Select(x => new Writer(x.Property, x.Adapter)))
			                                    .OrderBy(x => x.Adapter.Order))
			{
				var value = property.Implementation is IMember ? instance : property.Adapter.Get(instance);
				property.Implementation.Write(writer, value);
			}

			var runtimeContents = runtime.Except(runtimeProperties).ToArray();
			
			foreach (var content in contents.Concat(runtimeContents.Select(x => new Writer(x, x.Adapter)))
			                                .OrderBy(x => x.Adapter.Order))
			{
				var value = content.Implementation is IMember ? instance : content.Adapter.Get(instance);
				content.Implementation.Write(writer, value);
			}
		}

		IEnumerable<IRuntimeMember> Properties(object instance)
		{
			foreach (var member in _runtime)
			{
				if (member.IsSatisfiedBy(member.Adapter.Get(instance)))
				{
					yield return member;
				}
			}
		}

		struct Writer
		{
			public Writer(IWriter writer, IMemberAdapter adapter)
			{
				Implementation = writer;
				Adapter = adapter;
			}

			public IWriter Implementation { get; }
			public IMemberAdapter Adapter { get; }
		}
	}
}