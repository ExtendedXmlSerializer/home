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
using ExtendedXmlSerializer.Configuration;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	public sealed class AttachedPropertyConfiguration<TType, TValue> : IMemberConfiguration
	{
		readonly MemberConfiguration<TType, TValue> _configuration;

		public AttachedPropertyConfiguration(ITypeConfiguration declaringProperty,
		                                     MemberConfiguration<TType, TValue> configuration)
		{
			DeclaringProperty = declaringProperty;
			_configuration = configuration;
		}

		public ITypeConfiguration DeclaringProperty { get; }

		public MemberInfo Get() => _configuration.Get();

		public IConfigurationContainer Configuration => _configuration.Configuration;

		public IProperty<string> Name => _configuration.Name;

		public ITypeConfiguration Owner => ((IMemberConfiguration) _configuration).Owner;

		public IProperty<int> Order => _configuration.Order;
	}
}