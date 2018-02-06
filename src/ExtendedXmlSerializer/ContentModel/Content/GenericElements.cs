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

using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class GenericElements<T> : IElements<T>
	{
		readonly IIdentity _identity;
		readonly ImmutableArray<Type> _arguments;

		public GenericElements(IIdentities identities)
			: this(identities.Get(Support<T>.Key), Support<T>.Key.GetGenericArguments()
			                                                 .ToImmutableArray()) {}

		public GenericElements(IIdentity identity, ImmutableArray<Type> arguments)
		{
			_identity = identity;
			_arguments = arguments;
		}

		public IContentWriter<T> Get() => new GenericIdentity<T>(_identity, _arguments);
	}
}