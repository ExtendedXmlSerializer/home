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

namespace ExtendedXmlSerialization.Conversion.Names
{
	public interface INameProvider : INameProvider<IName> {}

	public interface INameProvider<out T> : IParameterizedSource<TypeInfo, T> where T : IName { }
	
	public abstract class NameProviderBase : NameProviderBase<IName>, INameProvider
	{
		protected NameProviderBase(ITypeFormatter formatter) : base(formatter) {}
		protected NameProviderBase(IAliasProvider alias, ITypeFormatter formatter) : base(alias, formatter) {}
	}

	public abstract class NameProviderBase<T> : INameProvider<T> where T : IName
	{
		readonly IAliasProvider _alias;
		readonly ITypeFormatter _formatter;

		protected NameProviderBase(ITypeFormatter formatter) : this(TypeAliasProvider.Default, formatter) {}

		protected NameProviderBase(IAliasProvider alias, ITypeFormatter formatter)
		{
			_alias = alias;
			_formatter = formatter;
		}

		public T Get(TypeInfo parameter) => Create(_alias.Get(parameter) ?? _formatter.Get(parameter), parameter);

		public abstract T Create(string displayName, TypeInfo classification);
	}
}