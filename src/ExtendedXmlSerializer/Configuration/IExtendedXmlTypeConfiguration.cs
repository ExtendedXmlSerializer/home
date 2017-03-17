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
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Configuration
{
	public interface IProperty<T> : ISource<T>
	{
		void Assign(T value);
	}

	class Property<T> : FixedSource<TypeInfo, T>, IProperty<T>
	{
		readonly ITypedTable<T> _table;
		readonly TypeInfo _type;

		public Property(ITypedTable<T> table, TypeInfo type) : base(table, type)
		{
			_table = table;
			_type = type;
		}

		public void Assign(T value) => _table.Assign(_type, value);
	}

	public interface IExtendedXmlTypeConfiguration : ISource<TypeInfo>
	{
		IProperty<string> Name { get; }

		IExtendedXmlMemberConfiguration Member(string name);

		/*int Version { get; }
		*/
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
		
		*/
	}
}