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
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Type = System.Type;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class DeclaredExtension : DecoratedSource<object, Type>
	{
		public static DeclaredExtension Default { get; } = new DeclaredExtension();

		DeclaredExtension()
			: base(new InstanceMetadata<ExtensionAttribute, object, Type>()
				       .Guard(
				              x => $"Extension could not be located for {x.GetType()}.  Please ensure this type is decorated with the ExtensionAttribute that declares a type of an ISerializerExtension.")) {}
	}

	sealed class Properties : DecoratedCommand<object>
	{
		public static IParameterizedSource<IExtensionElements, Properties> Defaults { get; }
			= new ReferenceCache<IExtensionElements, Properties>(x => new Properties(x));

		public Properties(IExtensionElements elements) :
			base(
			     Initialize.Default
			               .In(DeclaredExtension.Default
			                                    .Out(TypeMetadataCoercer.Default)
			                                    .Out(YieldCoercer<TypeInfo>.Default)
			                                    .Out(ImmutableArrayCoercer<TypeInfo>.Default))
			               .Out()
			               .ToCommand()
			               .Fix(elements)
			               .To(A<object>.Default)
			               .ByParameter()
			    ) {}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ExtensionAttribute : Attribute, ISource<Type>
	{
		readonly Type _name;

		public ExtensionAttribute(Type name) => _name = name;

		public Type Get() => _name;
	}

	public class Property<THost, TValue> : ReferenceCache<THost, TValue>, IProperty<THost, TValue>
		where THost : class where TValue : class

	{
		public Property() : this(_ => default(TValue)) {}
		public Property(ConditionalWeakTable<THost, TValue>.CreateValueCallback callback) : base(callback) {}
	}

	public class StructureProperty<THost, TValue> : StructureCache<THost, TValue>, IProperty<THost, TValue>
		where TValue : struct where THost : class
	{
		public StructureProperty() : base(_ => default(TValue)) {}
	}

	public interface IProperty<THost, TValue> : ITableSource<THost, TValue> {}

	public interface IProperty<T> : IProperty<IConfigurationElement, T> {}

	public interface IMetadataProperty<T> : IProperty<IMetadata, T> {}

	/*public interface IMetadataConfigurationProperty<T> : IMetadataProperty<T>, IProperty<IMetadataConfiguration, T> {}*/

	public class MetadataTable<T> : DecoratedTable<IMetadata, T>, IProperty<ISource<IMetadata>, T>, IMetadataProperty<T>
	{
		readonly ITableSource<ISource<IMetadata>, T> _source;

		public MetadataTable(ITableSource<IMetadata, T> metadata) : this(metadata.In(SourceCoercer<IMetadata>.Default), metadata) {}
		public MetadataTable(ITableSource<ISource<IMetadata>, T> source, ITableSource<IMetadata, T> metadata) : base(metadata) => _source = source;

		public bool IsSatisfiedBy(ISource<IMetadata> parameter) => _source.IsSatisfiedBy(parameter);

		public T Get(ISource<IMetadata> parameter) => _source.Get(parameter);

		public void Execute(KeyValuePair<ISource<IMetadata>, T> parameter)
		{
			_source.Execute(Pairs.Create(parameter.Key, parameter.Value));
		}

		public bool Remove(ISource<IMetadata> key) => _source.Remove(key);
	}

	public class MetadataProperty<T> : MetadataTable<T> where T : class
	{
		public MetadataProperty() : base(new Property<IMetadata, T>()) {}
	}

	public class StructureMetadataProperty<T> : MetadataTable<T> where T : struct
	{
		public StructureMetadataProperty() : base(new StructureProperty<IMetadata, T>()) {}
	}

	public class Property<T> : Property<IConfigurationElement, T>, IProperty<T> where T : class
	{
		public Property() : this(_ => default(T)) {}

		public Property(ConditionalWeakTable<IConfigurationElement, T>.CreateValueCallback callback)
			: base(callback) {}
	}

	public class StructureProperty<T> : StructureProperty<IConfigurationElement, T>, IProperty<T> where T : struct {}

	/*public class AdaptedProperty<T> :/* DecoratedTable<IMetadata, T>,#1# /*IProperty<T>,#1# ITableSource<ISource<IMetadata>, T> where T : class

	{
		readonly ITableSource<ISource<IMetadata>, T> _adapter;

		public AdaptedProperty() : this(_ => default(T)) {}

		public AdaptedProperty(ConditionalWeakTable<IMetadata, T>.CreateValueCallback callback)
			: this(new ReferenceCache<IMetadata, T>(callback)) {}

		public AdaptedProperty(ITableSource<IMetadata, T> table) : this(table.In(SourceCoercer<IMetadata>.Default), table)
		{}

		public AdaptedProperty(ITableSource<ISource<IMetadata>, T> adapter, ITableSource<IMetadata, T> table)
			/*: base(table)#1# => _adapter = adapter;

		public bool IsSatisfiedBy(ISource<IMetadata> parameter) => _adapter.IsSatisfiedBy(parameter);

		public T Get(ISource<IMetadata> parameter) => _adapter.Get(parameter);

		public void Execute(KeyValuePair<ISource<IMetadata>, T> parameter)
		{
			_adapter.Execute(parameter);
		}

		public bool Remove(ISource<IMetadata> key) => _adapter.Remove(key);
	}*/
}