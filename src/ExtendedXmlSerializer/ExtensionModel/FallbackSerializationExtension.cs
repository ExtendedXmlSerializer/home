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
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public class FallbackSerializationExtension : ISerializerExtension
	{
		public static FallbackSerializationExtension Default { get; } = new FallbackSerializationExtension();

		FallbackSerializationExtension() : this(EmptySerializer.Instance) {}

		readonly IFallbackSerializer _fallback;

		public FallbackSerializationExtension(IFallbackSerializer fallback) => _fallback = fallback;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(_fallback)
			            .Register<RuntimeSerialization>()
			            .Decorate<IRuntimeSerialization, RuntimeSerialization>();

		public void Execute(IServices parameter) {}

		sealed class RuntimeSerialization : IRuntimeSerialization
		{
			readonly IRuntimeSerialization _serializers;
			readonly ISerializer           _fallback;

			public RuntimeSerialization(IRuntimeSerialization serializers, IFallbackSerializer fallback)
			{
				_serializers = serializers;
				_fallback    = fallback;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				if (parameter != null)
				{
					try
					{
						return _serializers.Get(parameter);
					}
					catch (InvalidOperationException) {}
				}

				return _fallback;
			}
		}

		public interface IFallbackSerializer : ISerializer {}

		sealed class EmptySerializer : IFallbackSerializer
		{
			public static EmptySerializer Instance { get; } = new EmptySerializer();

			EmptySerializer() {}

			public object Get(IFormatReader parameter) => null;

			public void Write(IFormatWriter writer, object instance) {}
		}
	}
}