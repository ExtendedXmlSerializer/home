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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class Element : IElement
	{
		readonly IIdentities _identities;
		readonly IGeneric<IIdentity, IWriter> _adapter;

		public Element(IIdentities identities) : this(identities, Adapter.Default) {}

		public Element(IIdentities identities, IGeneric<IIdentity, IWriter> adapter)
		{
			_identities = identities;
			_adapter = adapter;
		}

		public IWriter Get(TypeInfo parameter) => _adapter.Get(parameter)
		                                                  .Invoke(_identities.Get(parameter));

		sealed class Adapter : Generic<IIdentity, IWriter>
		{
			public static Adapter Default { get; } = new Adapter();
			Adapter() : base(typeof(Writer<>)) {}

			sealed class Writer<T> : GenericWriterAdapter<T>
			{
				public Writer(IIdentity identity) : base(new Identity<T>(identity)) {}
			}
		}
	}
}