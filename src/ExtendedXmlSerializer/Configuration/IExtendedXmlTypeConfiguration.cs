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

using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Configuration
{
	public interface IExtendedXmlTypeConfiguration : ISource<TypeInfo>
	{
		IExtendedXmlMemberConfiguration Member(string name);

		/*int Version { get; }
		string Name { get; }*/
		/*void Map(Type targetType, XElement currentNode);
		object ReadObject(XElement element);
		void WriteObject(XmlWriter writer, object obj);
		
		bool IsCustomSerializer { get; set; }
		bool IsObjectReference { get; set; }
		Func<object, string> GetObjectId { get; set; }

		// IMemberConfigurator<T, TMember> Member<TMember>(Expression<Func<T, TMember>> member);
		/*IExtendedXmlTypeConfiguration<T> CustomSerializer(Action<XmlWriter, T> serializer, Func<XElement, T> deserialize);
		IExtendedXmlTypeConfiguration<T> CustomSerializer(IExtendedXmlCustomSerializer<T> serializer);

		IExtendedXmlTypeConfiguration<T> AddMigration(Action<XElement> migration);
		IExtendedXmlTypeConfiguration<T> AddMigration(IExtendedXmlTypeMigrator migrator);

		IExtendedXmlTypeConfiguration<T> EnableReferences();
		IExtendedXmlTypeConfiguration<T> Name(string name);
		*/
	}
}