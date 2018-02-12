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
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ExtensionModel.Services;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ConditionalContentDecoration<TSpecification, T> : IAlteration<IServiceRepository>
		where T : IContents where TSpecification : ISpecification<TypeInfo>
	{
		public static ConditionalContentDecoration<TSpecification, T> Default { get; } =
			new ConditionalContentDecoration<TSpecification, T>();

		ConditionalContentDecoration() {}

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var key = Support<TSpecification>.Key;
			var repository = key.IsClass &&
			                 !parameter.AvailableServices.Contains(key)
				                 ? parameter.RegisterWithDependencies<TSpecification>()
				                 : parameter;
			return repository.RegisterWithDependencies<T>()
			                 .Decorate<IContents>(Create);
		}

		static IContents Create(IServiceProvider arg1, IContents arg2)
			=> new ConditionalContents(arg1.Get<TSpecification>(), arg1.Get<T>(), arg2);
	}

	sealed class ConditionalContentDecoration<T> : DecorateAlteration<IContents, T, TypeInfo, ISerializer>
		where T : IContents
	{
		public ConditionalContentDecoration(ISpecification<TypeInfo> specification)
			: base(new Factory(specification).Create) {}

		sealed class Factory
		{
			readonly ISpecification<TypeInfo> _specification;

			public Factory(ISpecification<TypeInfo> specification) => _specification = specification;

			public IContents Create(IContents element, T arg2) => new ConditionalContents(_specification, arg2, element);
		}
	}
}