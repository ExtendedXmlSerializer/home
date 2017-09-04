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


using ExtendedXmlSerializer.Configuration;
using System.Reflection;

// ReSharper disable UnusedTypeParameter

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	public sealed class AttachedPropertyConfiguration<TType, TValue> : IMemberConfiguration
	{
		readonly IMemberConfiguration _configuration;

		public AttachedPropertyConfiguration(IMemberConfiguration configuration) => _configuration = configuration;

		public IRootContext Root => _configuration.Root;

		public IContext Parent => _configuration.Parent;

		public MemberInfo Get() => _configuration.Get();

		public IMemberConfiguration Name(string name) => _configuration.Name(name);

		public IMemberConfiguration Order(int order) => _configuration.Order(order);
	}
}