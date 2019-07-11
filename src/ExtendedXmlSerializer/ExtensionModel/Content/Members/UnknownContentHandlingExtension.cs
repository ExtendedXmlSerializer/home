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
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class UnknownContentHandlingExtension : ISerializerExtension
	{
		readonly Action<IFormatReader> _action;

		public UnknownContentHandlingExtension(Action<IFormatReader> action) => _action = action;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(_action)
			            .Decorate<IInnerContentServices, Services>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Services : IInnerContentServices
		{
			readonly IInnerContentServices _services;
			readonly Action<IFormatReader> _missing;

			public Services(IInnerContentServices services, Action<IFormatReader> missing)
			{
				_services = services;
				_missing  = missing;
			}

			public bool IsSatisfiedBy(IInnerContent parameter) => _services.IsSatisfiedBy(parameter);

			public void Handle(IInnerContent contents, IMemberSerializer member)
			{
				_services.Handle(contents, member);
			}

			public void Handle(IListInnerContent contents, IReader reader)
			{
				_services.Handle(contents, reader);
			}

			public string Get(IFormatReader parameter) => _services.Get(parameter);

			public IReader Create(TypeInfo classification, IInnerContentHandler handler)
				=> _services.Create(classification, new Handler(handler, _missing));
		}

		sealed class Handler : IInnerContentHandler
		{
			readonly IInnerContentHandler  _handler;
			readonly Action<IFormatReader> _command;

			public Handler(IInnerContentHandler handler, Action<IFormatReader> command)
			{
				_handler = handler;
				_command = command;
			}

			public bool IsSatisfiedBy(IInnerContent parameter)
			{
				var result = _handler.IsSatisfiedBy(parameter);
				if (!result)
				{
					var reader = parameter.Get();
					if (reader.Identifier != "http://www.w3.org/2000/xmlns/")
					{
						_command(reader);
					}
				}

				return result;
			}
		}
	}
}