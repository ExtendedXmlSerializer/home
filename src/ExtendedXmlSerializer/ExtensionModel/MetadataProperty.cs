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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core.Sources;
using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/*public interface IProperties<T> : IParameterizedSource<IMetadataElement, IProperty<T>> {}*/

	public interface IProperty<T> : ITableSource<IConfigurationElement, T> {}

	public class Property<T> : ReferenceCache<IConfigurationElement, T>, IProperty<T> where T : class
	{
		public Property() : this(_ => default(T)) {}

		public Property(ConditionalWeakTable<IConfigurationElement, T>.CreateValueCallback callback) : base(callback) {}
	}

	public class StructureProperty<T> : StructureCache<IConfigurationElement, T>, IProperty<T> where T : struct
	{
		public StructureProperty() : base(_ => default(T)) {}
	}
}