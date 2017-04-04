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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceSerializer : ISerializer
	{
		readonly IReferenceEncounters _encounters;
		readonly IReader _reader;
		readonly IWriter _writer;

		public ReferenceSerializer(IReferenceEncounters encounters, IReader reader, IWriter writer)
		{
			_encounters = encounters;
			_reader = reader;
			_writer = writer;
		}

		public void Write(IXmlWriter writer, object instance)
		{
			var encounters = _encounters.Get(writer);
			var identifier = encounters.Get(instance);
			if (identifier != null)
			{
				var first = encounters.IsSatisfiedBy(instance);
				var entity = identifier.Value.Entity;
				if (entity != null)
				{
					if (!first)
					{
						EntityIdentity.Default.Write(writer, entity.Get(instance));
					}
				}
				else
				{
					var property = first
						? (IProperty<uint?>) IdentityIdentity.Default
						: ContentModel.Properties.ReferenceIdentity.Default;
					property.Write(writer, identifier.Value.UniqueId);
				}

				if (!first)
				{
					return;
				}
			}
			_writer.Write(writer, instance);
		}

		public object Get(IContentAdapter parameter) => _reader.Get(parameter);
	}
}