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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using System.Reflection;
using XmlReader = System.Xml.XmlReader;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class ExistingInstanceExtension : ISerializerExtension
	{
		public static ExistingInstanceExtension Default { get; } = new ExistingInstanceExtension();

		ExistingInstanceExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IActivation, Activation>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Activation : IActivation
		{
			readonly IActivation _activation;

			public Activation(IActivation activation) => _activation = activation;

			public IReader Get(TypeInfo parameter) => new Reader(_activation.Get(parameter));

			sealed class Reader : IReader
			{
				readonly IReader    _reader;
				readonly IInstances _instances;

				public Reader(IReader reader) : this(reader, Instances.Default) {}

				public Reader(IReader reader, IInstances instances)
				{
					_reader    = reader;
					_instances = instances;
				}

				public object Get(IFormatReader parameter)
				{
					var key = parameter.Get()
					                   .AsValid<XmlReader>();

					var result = _instances.IsSatisfiedBy(key) ? _instances.Get(key) : _reader.Get(parameter);

					return result;
				}
			}
		}
	}


}