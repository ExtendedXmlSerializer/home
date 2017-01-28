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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.ElementModel.Names;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ElementModel.Options
{
	public interface IElementContext : IClassification
	{
		void Emit(IEmitter emitter, object instance);
	}

	public interface IEmitter : IServiceProvider
	{
		IDisposable Emit(IName name);

		void Write(string text);
	}

	public class XmlEmitter : IEmitter
	{
		readonly XmlWriter _writer;
		readonly INamespaces _namespaces;
		readonly IDisposable _finish;

		public XmlEmitter(INamespaces namespaces, XmlWriter writer)
			: this(namespaces, writer, new DelegatedDisposable(writer.WriteEndElement)) {}

		public XmlEmitter(INamespaces namespaces, XmlWriter writer, IDisposable finish)
		{
			_writer = writer;
			_namespaces = namespaces;
			_finish = finish;
		}

		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}

		public IDisposable Emit(IName name)
		{
			if (name is IMemberName)
			{
				_writer.WriteStartElement(name.DisplayName);

				/*var type = instance.GetType().GetTypeInfo();
				if (!context.Container.Exact(type))
				{
					context.Write(TypeProperty.Default, type);
				}*/
			}
			else
			{
				_writer.WriteStartElement(name.DisplayName, _namespaces.Get(name.Classification).Namespace.NamespaceName);
			}
			return _finish;
		}

		public void Write(string text) => _writer.WriteString(text);
	}

	/*public interface IElementContext<in T>
	{
		void Emit(IEmitter emitter, T instance);
	}*/

	public interface IContextOption : IOption<TypeInfo, IElementContext> {}

	public interface IContextOptions : IAlteration<IContexts> {}

	public class Serializer : Contexts, IExtendedXmlSerializer
	{
		readonly INameProvider<IName> _names;
		readonly INamespaces _namespaces;
		public Serializer() : this(new CollectionItemTypeLocator(), new AddMethodLocator(), new Names(), new Namespaces()) {}

		public Serializer(ICollectionItemTypeLocator locator, IAddMethodLocator add, INameProvider<IName> names,
		                  INamespaces namespaces)
			: base(new ContextOptions(names, locator, new AddDelegates(locator, add)))
		{
			_names = names;
			_namespaces = namespaces;
		}

		public void Serialize(Stream stream, object instance)
		{
			using (var writer = XmlWriter.Create(stream))
			{
				var emitter = new XmlEmitter(_namespaces, writer);
				var typeInfo = instance.GetType().GetTypeInfo();
				var context = new RootContext(_names.Get(typeInfo), Get(typeInfo));
				context.Emit(emitter, instance);
			}
		}

		public object Deserialize(Stream stream)
		{
			throw new NotImplementedException();
		}
	}

	class ContextOptions : IContextOptions
	{
		readonly INameProvider<IName> _names;
		readonly ICollectionItemTypeLocator _locator;
		readonly IAddDelegates _add;
		/*readonly ISpecification<TypeInfo> _specification;*/
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;

		/*public ContextOptions(ICollectionItemTypeLocator locator, IAddDelegates add, /*IEnumerable<TypeInfo> types,#1#
		                      ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
			: this(locator, add, /*new Specification(types),#1# property, field) {}*/

		public ContextOptions(INameProvider<IName> names, ICollectionItemTypeLocator locator, IAddDelegates add)
			: this(names, locator, add, new MemberSpecification<PropertyInfo>(PropertyMemberSpecification.Default),
			       new MemberSpecification<FieldInfo>(FieldMemberSpecification.Default)) {}

		public ContextOptions(INameProvider<IName> names, ICollectionItemTypeLocator locator, IAddDelegates add,
		                      /*ISpecification<TypeInfo> specification,*/
		                      ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
		{
			_names = names;
			_locator = locator;
			_add = add;
			/*_specification = specification;*/
			_property = property;
			_field = field;
		}

		public IContexts Get(IContexts parameter) => new Contexts(CreateOptions(parameter).ToArray());

		IEnumerable<IContextOption> CreateOptions(IContexts parameter)
		{
			//yield return new RootOption(parameter);

			var activators = new Activators();
			var members = new ContextMembers(new MemberContextSelector(parameter, _add), _property, _field);
			yield return IntegerTypeConverter.Default;
			yield return StringTypeConverter.Default;
			yield return new MemberedContextOption(members);
			yield return new CollectionContextOption(parameter, _locator, activators, _add);
		}

		/*sealed class Specification : AnySpecification<TypeInfo>
		{
			public Specification(IEnumerable<TypeInfo> known, params TypeInfo[] except)
				: base(
					new AnySpecification<TypeInfo>(new ContainsSpecification<TypeInfo>(known.Except(except).ToArray()),
					                               IsAssignableSpecification<Enum>.Default)) {}
		}*/

		class Contexts : Selector<TypeInfo, IElementContext>, IContexts
		{
			public Contexts(params IOption<TypeInfo, IElementContext>[] options) : base(options) {}

			/*protected override IElementContext Create(TypeInfo parameter)
			{
				return base.Create(parameter);
			}*/
		}
	}

	public interface IContexts : ISelector<TypeInfo, IElementContext> {}

	public class Contexts : IContexts
	{
		readonly IParameterizedSource<TypeInfo, IElementContext> _source;

		public Contexts(IContextOptions options)
		{
			_source = options.Get(this);
		}

		public IElementContext Get(TypeInfo parameter) => _source.Get(parameter);
	}

	abstract class ContainerContextOptionBase : ContainerContextOptionBase<IName>
	{
		protected ContainerContextOptionBase(ISpecification<TypeInfo> specification, INameProvider<IName> name)
			: base(specification, name) {}
	}

	class Names : ElementModel.Names.Names, INameProvider<IName>
	{
		public Names() : this(ElementModel.Names.Defaults.Names) {}
		public Names(ImmutableArray<IName> names) : base(names) {}
	}

	abstract class ContainerContextOptionBase<T> : ContextOptionBase where T : IName
	{
		readonly INameProvider<T> _name;

		protected ContainerContextOptionBase(ISpecification<TypeInfo> specification, INameProvider<T> name)
			: base(specification)
		{
			_name = name;
		}

		public override IElementContext Get(TypeInfo parameter) => Create(_name.Get(parameter));

		protected abstract IElementContext Create(T name);
	}

	class StringTypeConverter : ValueContextOption<string>
	{
		readonly static Func<string, string> Self = Self<string>.Default.Get;

		public static StringTypeConverter Default { get; } = new StringTypeConverter();
		StringTypeConverter() : base(Self, Self) {}
	}

	class IntegerTypeConverter : ValueContextOption<int>
	{
		public static IntegerTypeConverter Default { get; } = new IntegerTypeConverter();
		IntegerTypeConverter() : base(XmlConvert.ToInt32, XmlConvert.ToString) {}
	}

	abstract class ContextOptionBase : OptionBase<TypeInfo, IElementContext>, IContextOption
	{
		/*protected ContextOptionBase() : this(AlwaysSpecification<TypeInfo>.Default) {}*/

		protected ContextOptionBase(ISpecification<TypeInfo> specification) : base(specification) {}
	}

	class ValueContextOption<T> : FixedContextOption
	{
		public ValueContextOption(Func<string, T> deserialize, Func<T, string> serialize)
			: this(TypeEqualitySpecification<T>.Default, deserialize, serialize) {}

		public ValueContextOption(ISpecification<TypeInfo> specification, Func<string, T> deserialize,
		                          Func<T, string> serialize)
			: base(specification, new ValueContext<T>(deserialize, serialize)) {}
	}

	class FixedContextOption : FixedOption<TypeInfo, IElementContext>, IContextOption
	{
		public FixedContextOption(ISpecification<TypeInfo> specification, IElementContext context)
			: base(specification, context) {}
	}

	class ValueContext<T> : ElementContextBase<T>
	{
		readonly static TypeInfo Type = typeof(T).GetTypeInfo();

		readonly Func<string, T> _deserialize;
		readonly Func<T, string> _serialize;

		public ValueContext(Func<string, T> deserialize, Func<T, string> serialize) : base(Type)
		{
			_deserialize = deserialize;
			_serialize = serialize;
		}

		public override void Emit(IEmitter emitter, T instance) => emitter.Write(_serialize(instance));
	}

	public interface IContextMembers : IParameterizedSource<TypeInfo, IMembers> {}

	public interface IMembers : IEnumerable<IMemberContext>, IParameterizedSource<string, IMemberContext> {}

	public interface IMemberContext : IElementContext, IName {}

	class CollectionContextOption : ContainerContextOptionBase<ICollectionName>
	{
		readonly IContexts _contexts;
		readonly IActivators _activators;
		readonly IAddDelegates _add;

		public CollectionContextOption(IContexts contexts, ICollectionItemTypeLocator locator, IActivators activators,
		                               IAddDelegates add)
			: base(IsCollectionTypeSpecification.Default, new CollectionNameProvider(locator))
		{
			_contexts = contexts;
			_activators = activators;
			_add = add;
		}

		protected override IElementContext Create(ICollectionName name)
			=> new EnumerableContext(new CollectionItemContext(_contexts, name), _activators, _add);
	}

	class CollectionNameProvider : NameProviderBase<ICollectionName>
	{
		readonly ICollectionItemTypeLocator _locator;

		public CollectionNameProvider(ICollectionItemTypeLocator locator)
			: this(locator, new EnumerableTypeFormatter(locator)) {}

		public CollectionNameProvider(ICollectionItemTypeLocator locator, ITypeFormatter formatter) : base(formatter)
		{
			_locator = locator;
		}

		public override ICollectionName Create(string displayName, TypeInfo classification)
			=> new CollectionName(displayName, classification, _locator.Get(classification));
	}

	class CollectionItemContext : NamedContext<ICollectionName>
	{
		readonly IContexts _contexts;

		public CollectionItemContext(IContexts contexts, ICollectionName name)
			: this(contexts, name, contexts.Get(name.ElementType)) {}

		public CollectionItemContext(IContexts contexts, ICollectionName name, IElementContext body) : base(name, body)
		{
			_contexts = contexts;
		}

		public override void Emit(IEmitter emitter, object instance)
		{
			var type = instance.GetType();
			var actual = type.GetTypeInfo();
			if (Equals(actual, Name.ElementType))
			{
				base.Emit(emitter, instance);
			}
			else
			{
				_contexts.Get(actual).Emit(emitter, instance);
			}
		}
	}

	class DictionaryContext : EnumerableContext<IDictionary>
	{
		public DictionaryContext(IElementContext item, IActivators activators, IAddDelegates add, TypeInfo classification)
			: base(item, activators, add, classification) {}

		protected override IEnumerator Get(IDictionary instance) => instance.GetEnumerator();
	}

	class EnumerableContext : EnumerableContext<IEnumerable>
	{
		public EnumerableContext(IElementContext item, IActivators activators, IAddDelegates add)
			: base(item, activators, add) {}
	}

	class EnumerableContext<T> : ElementContextBase<T> where T : IEnumerable
	{
		readonly IElementContext _item;
		readonly IActivators _activators;
		readonly IAddDelegates _add;

		public EnumerableContext(IElementContext item, IActivators activators, IAddDelegates add)
			: this(item, activators, add, item.Classification) {}

		public EnumerableContext(IElementContext item, IActivators activators, IAddDelegates add, TypeInfo classification)
			: base(classification)
		{
			_item = item;
			_activators = activators;
			_add = add;
		}

		protected virtual IEnumerator Get(T instance) => instance.GetEnumerator();

		public override void Emit(IEmitter emitter, T instance)
		{
			var enumerator = Get(instance);
			while (enumerator.MoveNext())
			{
				_item.Emit(emitter, enumerator.Current);
			}
		}
	}

	class MemberedContextOption : ContextOptionBase
	{
		readonly IContextMembers _contexts;

		public MemberedContextOption(IContextMembers contexts) : base(IsActivatedTypeSpecification.Default)
		{
			_contexts = contexts;
		}

		public override IElementContext Get(TypeInfo parameter) => new MemberedContext(_contexts, parameter);
	}

	class MemberedContext : ContextBase
	{
		readonly IContextMembers _contexts;
		readonly IMembers _members;

		public MemberedContext(IContextMembers contexts, TypeInfo classification)
			: this(contexts, contexts.Get(classification), classification) {}

		public MemberedContext(IContextMembers contexts, IMembers members, TypeInfo classification) : base(classification)
		{
			_contexts = contexts;
			_members = members;
		}

		public override void Emit(IEmitter emitter, object instance)
		{
			var type = instance.GetType();
			var typeInfo = type.GetTypeInfo();
			var same = type == Classification.AsType();
			var members = same ? _members : _contexts.Get(typeInfo);
			foreach (var member in members)
			{
				member.Emit(emitter, instance);
			}
		}
	}

	public class Members : IMembers
	{
		readonly ImmutableArray<IMemberContext> _members;
		readonly IDictionary<string, IMemberContext> _lookup;

		/*public Members(TypeInfo classification, params IMember[] members) : this(classification, members.AsEnumerable()) {}*/

		public Members(IEnumerable<IMemberContext> members) : this(members.ToImmutableArray()) {}

		public Members(ImmutableArray<IMemberContext> members) : this(members, members.ToDictionary(x => x.DisplayName)) {}

		public Members(ImmutableArray<IMemberContext> members, IDictionary<string, IMemberContext> lookup)
		{
			_members = members;
			_lookup = lookup;
		}

		public IMemberContext Get(string parameter) => _lookup.TryGet(parameter);

		public IEnumerator<IMemberContext> GetEnumerator()
		{
			foreach (var element in _members)
			{
				yield return element;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class ReadOnlyCollectionMemberOption : MemberOptionBase
	{
		readonly IContexts _contexts;
		readonly IGetterFactory _getter;
		readonly IAddDelegates _add;

		public ReadOnlyCollectionMemberOption(IContexts contexts, IAddDelegates add)
			: this(contexts, GetterFactory.Default, add) {}

		public ReadOnlyCollectionMemberOption(IContexts contexts, IGetterFactory getter, IAddDelegates add)
			: base(Specification.Instance)
		{
			_contexts = contexts;
			_getter = getter;
			_add = add;
		}

		protected override IMemberContext Create(IMemberName name)
		{
			var add = _add.Get(name.MemberType);
			if (add != null)
			{
				var getter = _getter.Get(name.Metadata);
				var result = new ReadOnlyCollectionMember(name, add, getter, _contexts.Get(name.MemberType));
				return result;
			}
			return null;
		}

		public class ReadOnlyCollectionMember : MemberContext
		{
			public ReadOnlyCollectionMember(IMemberName name, Action<object, object> add, Func<object, object> getter,
			                                IElementContext context)
				: base(name, context, add, getter) {}

			protected override void Assign(object instance, object value)
			{
				var collection = Get(instance);
				foreach (var element in value.AsValid<IEnumerable>())
				{
					base.Assign(collection, element);
				}
			}
		}


		sealed class Specification : ISpecification<MemberInformation>
		{
			public static Specification Instance { get; } = new Specification();
			Specification() : this(IsCollectionTypeSpecification.Default) {}
			readonly ISpecification<TypeInfo> _specification;


			Specification(ISpecification<TypeInfo> specification)
			{
				_specification = specification;
			}

			public bool IsSatisfiedBy(MemberInformation parameter) => _specification.IsSatisfiedBy(parameter.MemberType);
		}
	}


	class MemberContextSelector : OptionSelector<MemberInformation, IMemberContext>, IMemberContextSelector
	{
		public MemberContextSelector(IContexts contexts, IAddDelegates add)
			: this(new MemberOption(contexts), new ReadOnlyCollectionMemberOption(contexts, add)) {}

		public MemberContextSelector(params IOption<MemberInformation, IMemberContext>[] options) : base(options) {}
	}

	public interface IMemberOption : IOption<MemberInformation, IMemberContext> {}

	public abstract class MemberOptionBase : OptionBase<MemberInformation, IMemberContext>, IMemberOption
	{
		readonly IParameterizedSource<MemberInformation, IMemberName> _provider;

		protected MemberOptionBase(ISpecification<MemberInformation> specification)
			: this(specification, MemberNameProvider.Default) {}

		protected MemberOptionBase(ISpecification<MemberInformation> specification,
		                           IParameterizedSource<MemberInformation, IMemberName> provider)
			: base(specification)
		{
			_provider = provider;
		}

		public override IMemberContext Get(MemberInformation parameter) => Create(_provider.Get(parameter));

		protected abstract IMemberContext Create(IMemberName name);
	}

	public class MemberNameProvider : IParameterizedSource<MemberInformation, IMemberName>
	{
		readonly IAliasProvider _alias;
		public static MemberNameProvider Default { get; } = new MemberNameProvider();
		MemberNameProvider() : this(MemberAliasProvider.Default) {}

		public MemberNameProvider(IAliasProvider alias)
		{
			_alias = alias;
		}

		public IMemberName Get(MemberInformation parameter)
			=>
				new MemberName(_alias.Get(parameter.Metadata) ?? parameter.Metadata.Name, parameter.Metadata, parameter.MemberType);
	}

	public interface IMemberName : IName
	{
		MemberInfo Metadata { get; }

		TypeInfo MemberType { get; }
	}

	public interface ICollectionName : IName
	{
		TypeInfo ElementType { get; }
	}

	class CollectionName : Name, ICollectionName
	{
		public CollectionName(string displayName, TypeInfo classification, TypeInfo elementType)
			: base(displayName, classification)
		{
			ElementType = elementType;
		}

		public TypeInfo ElementType { get; }
	}

	class MemberName : Name, IMemberName
	{
		public MemberName(string displayName, MemberInfo metadata, TypeInfo memberType)
			: base(displayName, metadata.DeclaringType)
		{
			Metadata = metadata;
			MemberType = memberType;
		}

		public MemberInfo Metadata { get; }
		public TypeInfo MemberType { get; }
	}

	public class MemberOption : MemberOptionBase
	{
		readonly IGetterFactory _getter;
		readonly ISetterFactory _setter;
		readonly IContexts _contexts;

		public MemberOption(IContexts contexts) : this(contexts, GetterFactory.Default, SetterFactory.Default) {}

		public MemberOption(IContexts contexts, IGetterFactory getter, ISetterFactory setter)
			: base(new DelegatedSpecification<MemberInformation>(x => x.Assignable))
		{
			_getter = getter;
			_setter = setter;
			_contexts = contexts;
		}

		protected override IMemberContext Create(IMemberName name)
		{
			var metadata = name.Metadata;
			var getter = _getter.Get(metadata);
			var setter = _setter.Get(metadata);
			var result = new MemberContext(name, _contexts.Get(name.MemberType), setter, getter);
			return result;
		}
	}

	/*public abstract class MemberContextBase : NamedContext, IMemberContext
	{
		protected MemberContextBase(IName name, IElementContext body) : this(name.DisplayName, name, body) {}

		protected MemberContextBase(string displayName, IName name, IElementContext body) : base(name, body)
		{
			DisplayName = displayName;
		}

		/*public abstract object Get(object instance);
		public abstract void Assign(object instance, object value);#1#
		public string DisplayName { get; }

		public override void Emit(IEmitter emitter, object instance)
		{
			base.Emit(emitter, instance);
		}
	}*/


	public class MemberContext : NamedContext<IMemberName>, IMemberContext
	{
		readonly Action<object, object> _setter;
		readonly Func<object, object> _getter;

		/*public MemberContext(MemberInfo metadata, Action<object, object> setter, Func<object, object> getter,
		                     IElementContext context)
			: this(metadata.Name, metadata, setter, getter, context) {}*/

		public MemberContext(IMemberName name, IElementContext body, Action<object, object> setter,
		                     Func<object, object> getter) : this(name.DisplayName, name, body, setter, getter) {}

		public MemberContext(string displayName, IMemberName name, IElementContext body, Action<object, object> setter,
		                     Func<object, object> getter) : base(name, body)
		{
			DisplayName = displayName;
			_setter = setter;
			_getter = getter;
		}

		public override void Emit(IEmitter emitter, object instance)
		{
			var value = Get(instance);
			if (value != null)
			{
				base.Emit(emitter, value);
			}
		}

		public string DisplayName { get; }

		protected virtual object Get(object instance) => _getter(instance);

		protected virtual void Assign(object instance, object value)
		{
			if (value != null)
			{
				_setter(instance, value);
			}
		}
	}


	public interface IMemberContextSelector : ISelector<MemberInformation, IMemberContext> {}

	public sealed class ContextMembers : CacheBase<TypeInfo, IMembers>, IContextMembers
	{
		readonly IMemberContextSelector _selector;
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;

		public ContextMembers(IMemberContextSelector selector, ISpecification<PropertyInfo> property,
		                      ISpecification<FieldInfo> field)
		{
			_selector = selector;
			_property = property;
			_field = field;
		}

		protected override IMembers Create(TypeInfo parameter) =>
			new Members(Yield(parameter).OrderBy(x => x.Sort).Select(x => x.Member));

		IEnumerable<Sorting> Yield(TypeInfo parameter)
		{
			foreach (var property in parameter.GetProperties())
			{
				if (_property.IsSatisfiedBy(property))
				{
					var sorting = Create(property, property.PropertyType, property.CanWrite);
					if (sorting != null)
					{
						yield return sorting.Value;
					}
				}
			}

			foreach (var field in parameter.GetFields())
			{
				if (_field.IsSatisfiedBy(field))
				{
					var sorting = Create(field, field.FieldType, !field.IsInitOnly);
					if (sorting != null)
					{
						yield return sorting.Value;
					}
				}
			}
		}

		Sorting? Create(MemberInfo metadata, Type memberType, bool assignable)
		{
			var sort = new Sort(metadata.GetCustomAttribute<XmlElementAttribute>(false)?.Order, metadata.MetadataToken);
			var information = new MemberInformation(metadata, memberType.GetTypeInfo().AccountForNullable(), assignable);
			var element = _selector.Get(information);
			var result = element != null ? (Sorting?) new Sorting(element, sort) : null;
			return result;
		}

		struct Sorting
		{
			public Sorting(IMemberContext member, Sort sort)
			{
				Member = member;
				Sort = sort;
			}

			public IMemberContext Member { get; }
			public Sort Sort { get; }
		}
	}


/*
	public static class Extensions
	{
		public static IMembers Load(this IContextMembers @this, IMembers members, TypeInfo instanceType)
			=> members.Exact(instanceType) ? members : @this.Get(instanceType);
	}
*/

	class RootOption : ContainerContextOptionBase
	{
		readonly IContexts _contexts;

		public RootOption(IContexts contexts, INameProvider<IName> names) : base(AlwaysSpecification<TypeInfo>.Default, names)
		{
			_contexts = contexts;
		}

		protected override IElementContext Create(IName name) => new RootContext(name, _contexts.Get(name.Classification));
	}

	public class RootContext : NamedContext
	{
		public RootContext(IName name, IElementContext body) : base(name, body) {}
	}

	public abstract class ContextBase : IElementContext
	{
		protected ContextBase(TypeInfo classification)
		{
			Classification = classification;
		}

		public abstract void Emit(IEmitter emitter, object instance);
		public TypeInfo Classification { get; }
	}

	public abstract class ElementContextBase<T> : ContextBase
	{
		protected ElementContextBase(TypeInfo classification) : base(classification) {}

		public override void Emit(IEmitter emitter, object instance) => Emit(emitter, (T) instance);

		public abstract void Emit(IEmitter emitter, T instance);
	}

	public class NamedContext : NamedContext<IName>
	{
		public NamedContext(IName name, IElementContext body) : base(name, body) {}
		public NamedContext(IName name, IElementContext body, TypeInfo classification) : base(name, body, classification) {}
	}

	public class NamedContext<T> : DecoratedContext where T : IName
	{
		public NamedContext(T name, IElementContext body) : this(name, body, body.Classification) {}

		public NamedContext(T name, IElementContext body, TypeInfo classification) : base(body, classification)
		{
			Name = name;
		}

		protected T Name { get; }

		public override void Emit(IEmitter emitter, object instance)
		{
			using (emitter.Emit(Name))
			{
				base.Emit(emitter, instance);
			}
		}
	}

	public class DecoratedContext : ContextBase
	{
		readonly IElementContext _context;

		public DecoratedContext(IElementContext context) : this(context, context.Classification) {}

		public DecoratedContext(IElementContext context, TypeInfo classification) : base(classification)
		{
			_context = context;
		}

		public override void Emit(IEmitter emitter, object instance) => _context.Emit(emitter, instance);
	}
}