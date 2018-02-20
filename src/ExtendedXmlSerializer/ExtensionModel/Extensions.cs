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
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public interface IExtend<out T> : IParameterizedSource<IExtensionElements, T> where T : class, ISerializerExtension { }

	interface IExtensions<out T> : IParameterizedSource<IExtensionElements, T> where T : ISerializerExtension {}

	sealed class Extensions<T> : IExtensions<T> where T : class, ISerializerExtension
	{
		public static Extensions<T> Default { get; } = new Extensions<T>();
		Extensions() : this(Support<T>.NewOrSingleton) {}

		readonly Func<T> _create;

		public Extensions(Func<T> create) => _create = create;

		public T Get(IExtensionElements parameter)
		{
			var existing = parameter.OfType<T>().FirstOrDefault();
			if (existing == null)
			{
				var result = _create();
				parameter.Add.Execute(result);
				return result;
			}

			return existing;
		}
	}

	public class TypeContainerAttribute : Attribute, ISource<Type>
	{
		readonly Type _serializerType;

		public TypeContainerAttribute(Type serializerType) => _serializerType = serializerType;

		public Type Get() => _serializerType;
	}


	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class DependencyAttribute : TypeContainerAttribute
	{
		public DependencyAttribute(Type serializerType) : base(serializerType) {}
	}


	sealed class ExtensionsWithDependencies<T> : IExtensions<T> where T : class, ISerializerExtension
	{
		public static ExtensionsWithDependencies<T> Default { get; } = new ExtensionsWithDependencies<T>();
		ExtensionsWithDependencies() : this(Extensions<T>.Default.Get,
											DeclaredDependencies<T>.Default
											                       .YieldMetadata()
											                       .Select(Initialize.Default.Get)
											                       .Select(x => x.ToInstanceCommand())
											                       .Fold()) {}

		readonly Func<IExtensionElements, T> _extensions;
		readonly ICommand<IExtensionElements> _initialize;

		public ExtensionsWithDependencies(Func<IExtensionElements, T> extensions, ICommand<IExtensionElements> initialize)
		{
			_extensions = extensions;
			_initialize = initialize;
		}

		public T Get(IExtensionElements parameter)
		{
			_initialize.Execute(parameter);
			var result = _extensions(parameter);
			return result;
		}
	}


	sealed class Initialize : Generic<ICommand<IExtensionElements>>
	{
		public static Initialize Default { get; } = new Initialize();
		Initialize() : base(typeof(Initialize<>)) {}
	}

	sealed class Initialize<T> : ICommand<IExtensionElements> where T : class, ISerializerExtension
	{
		[UsedImplicitly]
		public static Initialize<T> Default { get; } = new Initialize<T>();
		Initialize() : this(new ConditionalSpecification<IExtensionElements>(), Extend<T>.Default) { }

		readonly ISpecification<IExtensionElements> _specification;
		readonly IExtend<T> _extend;

		public Initialize(ISpecification<IExtensionElements> specification, IExtend<T> extend)
		{
			_specification = specification;
			_extend = extend;
		}

		public void Execute(IExtensionElements parameter)
		{
			if (_specification.IsSatisfiedBy(parameter))
			{
				_extend.Get(parameter);
			}
		}
	}

	sealed class DeclaredDependencies<T> : Items<Type> where T : ISerializerExtension
	{
		public static DeclaredDependencies<T> Default { get; } = new DeclaredDependencies<T>();
		DeclaredDependencies() : base(DependencyAttributes.Default.Get(typeof(T))) {}
	}

	sealed class DependencyAttributes : TypeMetadataValues<DependencyAttribute, Type>
	{
		public static DependencyAttributes Default { get; } = new DependencyAttributes();
		DependencyAttributes() {}
	}

/*
	sealed class DeclaredDependencies : ItemsBase<Type>
	{
		readonly Type _root;
		readonly Func<MemberInfo, IEnumerable<Type>> _select;

		public DeclaredDependencies(Type root) : this(root, TypesFrom<DependencyAttribute>.Default.Get) {}

		public DeclaredDependencies(Type root, Func<MemberInfo, IEnumerable<Type>> select)
		{
			_root = root;
			_select = @select;
		}

		public override IEnumerator<Type> GetEnumerator() => Select(_select(_root), _ => true).GetEnumerator();

		IEnumerable<Type> Select(IEnumerable<Type> parameter, Func<Type, bool> specification)
		{
			var current = parameter.Fixed();
			var associated = current.SelectMany(_select)
			                        .Where(specification)
			                        .Fixed();
			var updated = current.Union(associated)
			                     .Fixed();
			var others = associated.Any()
				             ? Select(associated, new DelegatedSpecification<Type>(specification).And(new ContainsSpecification<Type>(updated).Inverse())
				                                                                                 .IsSatisfiedBy)
				             : Enumerable.Empty<Type>();
			var result = updated.Union(others);
			return result;
		}
	}
*/


	sealed class Extend<T> : ReferenceCache<IExtensionElements, T>, IExtend<T> where T : class, ISerializerExtension
	{
		public static Extend<T> Default { get; } = new Extend<T>();
		Extend() : this(ExtensionsWithDependencies<T>.Default.Get) { }

		public Extend(ConditionalWeakTable<IExtensionElements, T>.CreateValueCallback callback) : base(callback) { }
	}
}