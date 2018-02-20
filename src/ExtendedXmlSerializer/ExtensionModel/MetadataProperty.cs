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
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public interface IProperties<T> : IAlteration<IProperty<T>> {}

	sealed class DeclaredExtension<T> : DecoratedSource<IProperty<T>, Type>
	{
		public static DeclaredExtension<T> Default { get; } = new DeclaredExtension<T>();
		DeclaredExtension() : this(x => $"Extension could not be located for {x.GetType()}.  Please ensure this type is decorated with the ExtensionAttribute that declares a type of an ISerializerExtension.") {}

		public DeclaredExtension(Func<IProperty<T>, string> message) : base(DeclaredExtensions<T>.Default.Guard(message)) {}
	}

	sealed class Properties<T> : DelegatedAlteration<IProperty<T>>, IProperties<T>
	{
		public static IParameterizedSource<IExtend, IProperties<T>> Defaults { get; }
			= new ReferenceCache<IExtend, IProperties<T>>(x => new Properties<T>(x));

		public Properties(IExtend extend) : base(extend.In(DeclaredExtension<T>.Default.Out(TypeMetadataCoercer.Default)).Return) {}
	}

	sealed class DeclaredExtensions<T> : InstanceMetadata<ExtensionAttribute, IProperty<T>, Type>
	{
		public static DeclaredExtensions<T> Default { get; } = new DeclaredExtensions<T>();
		DeclaredExtensions() { }
	}


	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ExtensionAttribute : Attribute, ISource<Type>
	{
		readonly Type _name;

		public ExtensionAttribute(Type name) => _name = name;

		public Type Get() => _name;
	}

	public interface IProperty<T> : ITableSource<IMetadata, T> {}

	public class Property<T> : ReferenceCache<IMetadata, T>, IProperty<T> where T : class

	{
		public Property() : this(_ => default(T)) {}

		public Property(ConditionalWeakTable<IMetadata, T>.CreateValueCallback callback)
			: base(callback) {}
	}

	public class StructureProperty<T> : StructureCache<IMetadata, T>, IProperty<T> where T : struct
	{
		public StructureProperty() : base(_ => default(T)) {}
	}


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