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

using System;
using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Conversion.Read;

namespace ExtendedXmlSerialization.ElementModel.Members
{
	public class EncryptedMemberConverter : DecoratedConverter
	{
		readonly IPropertyEncryption _encryption;

		public EncryptedMemberConverter(IPropertyEncryption encryption, IConverter member) : base(member)
		{
			_encryption = encryption;
		}

		public override object Read(IReadContext context) => base.Read(new DecryptedContext(_encryption, context));

		sealed class DecryptedContext : IReadContext
		{
			readonly IPropertyEncryption _encryption;
			readonly IReadContext _context;

			public DecryptedContext(IPropertyEncryption encryption, IReadContext context)
			{
				_encryption = encryption;
				_context = context;
			}

			public object GetService(Type serviceType) => _context.GetService(serviceType);

			public IContainerElement Container => _context.Container;

			public IElement Element => _context.Element;

			public IEnumerator<IReadMemberContext> GetEnumerator() => _context.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public void Add(object service) => _context.Add(service);

			public IEnumerable<IReadContext> Items() => _context.Items();

			public string Read() => _encryption.Decrypt(_context.Read());

			public string this[IName name] => _context[name];
		}
	}
}