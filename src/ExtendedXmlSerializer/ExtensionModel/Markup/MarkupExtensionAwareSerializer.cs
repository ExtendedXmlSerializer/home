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
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensionAwareSerializer : ISerializer
	{
		readonly IParser<MarkupExtensionParts> _parser;
		readonly IMarkupExtensionContainer _container;
		readonly System.IServiceProvider _provider;
		readonly ISerializer _serializer;

		public MarkupExtensionAwareSerializer(IMarkupExtensionContainer container, System.IServiceProvider provider,
		                                      ISerializer serializer)
			: this(MarkupExtensionParser.Default, container, provider, serializer) {}

		public MarkupExtensionAwareSerializer(IParser<MarkupExtensionParts> parser, IMarkupExtensionContainer container,
		                                      System.IServiceProvider provider,
		                                      ISerializer serializer)
		{
			_parser = parser;
			_container = container;
			_provider = provider;
			_serializer = serializer;
		}

		public object Get(IXmlReader parameter)
		{
			var candidate = Candidate(parameter);
			var parts = candidate.Instance as MarkupExtensionParts;
			var result = parts != null
				? _container.Get(parameter)
				            .Get(parts)
				            .ProvideValue(new Provider(_provider, parameter, parts))
				: candidate.Get();
			return result;
		}

		CandidateResult Candidate(IXmlReader parameter)
		{
			try
			{
				return new CandidateResult(_serializer.Get(parameter));
			}
			catch (Exception e)
			{
				return new CandidateResult(_parser.Get(parameter.Value()), e);
			}
		}

		struct CandidateResult : ISource<object>
		{
			readonly Exception _error;

			public CandidateResult(object instance, Exception error = null)
			{
				_error = error;
				Instance = instance;
			}

			public object Instance { get; }

			public object Get()
			{
				if (_error != null)
				{
					throw _error;
				}
				return Instance;
			}
		}

		sealed class Provider : System.IServiceProvider
		{
			readonly System.IServiceProvider _provider, _services;

			public Provider(System.IServiceProvider provider, params object[] services)
				: this(provider, new ServiceProvider(services)) {}

			public Provider(System.IServiceProvider provider, System.IServiceProvider services)
			{
				_provider = provider;
				_services = services;
			}

			public object GetService(Type serviceType)
				=> _services.GetService(serviceType) ?? _provider.GetService(serviceType);
		}

		public void Write(IXmlWriter writer, object instance) => _serializer.Write(writer, instance);
	}
}