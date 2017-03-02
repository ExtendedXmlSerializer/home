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
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class ConfigureServicesCommand : ICommand<IServices>
	{
		readonly static Serializations<Serialization> Serializations = Serializations<Serialization>.Default;

		readonly ISerializations _serializations;
		readonly IReadOnlyList<ISerializerExtension> _extensions;

		public ConfigureServicesCommand(IReadOnlyList<ISerializerExtension> extensions) : this(Serializations, extensions) {}

		public ConfigureServicesCommand(ISerializations serializations, IReadOnlyList<ISerializerExtension> extensions)
		{
			_serializations = serializations;
			_extensions = extensions;
		}

		public void Execute(IServices parameter)
		{
			var serialization = new AssignedSerialization();
			var provider = _extensions.Appending(_serializations.Get(serialization))
			                          .Alter(parameter);

			serialization.Execute(_serializations.Get(provider));
		}
	}
}