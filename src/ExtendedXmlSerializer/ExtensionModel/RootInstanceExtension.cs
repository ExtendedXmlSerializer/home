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

using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.References;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public sealed class RootInstanceExtension : ISerializerExtension
	{
		public static RootInstanceExtension Default { get; } = new RootInstanceExtension();
		RootInstanceExtension() {}

		public IServiceRepository Get(IServiceRepository parameter) => parameter
		                                                               .RegisterInstance<IRootInstances
		                                                               >(RootInstances.Default)
		                                                               .Decorate<ContentModel.Content.ISerializers,
			                                                               Serializers>()
		                                                               .Decorate(typeof(IContentWriters<>),
		                                                                         typeof(ContentWriters<>));

		sealed class ContentWriter<T> : IContentWriter<T>
		{
			readonly ISpecification<object> _conditions;
			readonly IContentWriter<T> _write;
			readonly IRootInstances _instances;

			public ContentWriter(ISpecification<object> conditions, IContentWriter<T> write, IRootInstances instances)
			{
				_conditions = conditions;
				_write = write;
				_instances = instances;
			}

			public void Execute(ContentModel.Writing<T> parameter)
			{
				var key = parameter.Writer.Get();
				if (_conditions.IsSatisfiedBy(key))
				{
					_instances.Assign(key, parameter.Instance);
					_write.Execute(parameter);
				}
			}
		}

		sealed class ContentWriters<T> : IContentWriters<T>
		{
			readonly Func<bool> _specification;
			readonly ISpecification<object> _conditions;
			readonly IContentWriters<T> _writers;
			readonly IRootInstances _instances;

			[UsedImplicitly]
			public ContentWriters(IContentWriters<T> writers, IRootInstances instances)
				: this(IsReferenceSpecification.Default.Build<T>(), new InstanceConditionalSpecification(), writers, instances) {}

			public ContentWriters(Func<bool> specification, ISpecification<object> conditions, IContentWriters<T> writers,
			                      IRootInstances instances)
			{
				_specification = specification;
				_conditions = conditions;
				_writers = writers;
				_instances = instances;
			}

			public IContentWriter<T> Get()
			{
				var write = _writers.Get();
				var result = _specification() ? new ContentWriter<T>(_conditions, write, _instances) : write;
				return result;
			}
		}

		sealed class Serializers : ContentModel.Content.ISerializers
		{
			readonly ISpecification<object> _conditions;
			readonly ISpecification<TypeInfo> _specification;
			readonly ContentModel.Content.ISerializers _writers;
			readonly IRootInstances _instances;

			[UsedImplicitly]
			public Serializers(ContentModel.Content.ISerializers writers, IRootInstances instances)
				: this(new InstanceConditionalSpecification(), IsReferenceSpecification.Default, writers, instances) {}

			public Serializers(ISpecification<object> conditions, ISpecification<TypeInfo> specification,
			                   ContentModel.Content.ISerializers writers, IRootInstances instances)
			{
				_conditions = conditions;
				_specification = specification;
				_writers = writers;
				_instances = instances;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var write = _writers.Get(parameter);
				var result = _specification.IsSatisfiedBy(parameter) ? new Serializer(_conditions, _instances, write) : write;
				return result;
			}
		}


		sealed class Serializer : ISerializer
		{
			readonly ISpecification<object> _conditions;
			readonly IRootInstances _instances;
			readonly ISerializer _container;

			public Serializer(ISpecification<object> conditions, IRootInstances instances, ISerializer container)
			{
				_conditions = conditions;
				_instances = instances;
				_container = container;
			}

			public object Get(IFormatReader parameter) => _container.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				var item = writer.Get();
				if (_conditions.IsSatisfiedBy(item))
				{
					_instances.Assign(item, instance);
				}

				_container.Write(writer, instance);
			}
		}


		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}