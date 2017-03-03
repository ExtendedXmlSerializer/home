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
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class Serializations : Serializations<Serialization>
	{
		public static Serializations Default { get; } = new Serializations();
		Serializations() {}
	}

	class Serializations<T> : ISerializations, ISerializerExtension where T : class, ISerializationContext
	{
		public ISerialization Get(IServiceProvider parameter) => parameter.Get<T>();

		public IServices Get(IServices parameter)
		{
			var serialization = new ConfiguredSerialization();
			var result = parameter.RegisterInstance<ISerialization>(serialization)
			                      .RegisterInstance(serialization)
			                      .Register<IMemberContent, MemberContent>()
			                      .Decorate<IMemberContent>((factory, content) => new RecursionGuardedMemberContent(content))
			                      .Register<T>();
			return result;
		}

		public void Execute(IServices parameter)
		{
			var configured = parameter.Get<ConfiguredSerialization>();
			var context = parameter.Get<T>();
			configured.Execute(context);
			parameter.RegisterInstance<ISerializationContext>(context);
		}
	}
}