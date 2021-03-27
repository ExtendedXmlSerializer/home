using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class AllowedTypesExtension : ISerializerExtension
	{
		public AllowedTypesExtension() : this(new HashSet<Type>(), new HashSet<Type>()) {}

		public AllowedTypesExtension(ISet<Type> allowed, ISet<Type> prohibited)
		{
			Allowed    = allowed;
			Prohibited = prohibited;
		}

		public ICollection<Type> Allowed { get; }

		public ICollection<Type> Prohibited { get; }

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var policy = Allowed.Any()
				             ? (ITypePolicy)new AllowedTypesPolicy(Allowed.ToArray())
				             : new ProhibitedTypePolicy(Prohibited.ToArray());

			return parameter.RegisterInstance(policy)
			                .Decorate<ISerializers, Serializers>()
			                .Decorate<IContents, Contents>();
		}

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Serializers : ISerializers
		{
			readonly ITypePolicy  _policy;
			readonly ISerializers _previous;

			public Serializers(ITypePolicy policy, ISerializers previous)
			{
				_policy   = policy;
				_previous = previous;
			}

			public ISerializer Get(TypeInfo parameter)
				=> _policy.IsSatisfiedBy(parameter) ? _previous.Get(parameter) : EmptySerializer.Instance;
		}

		sealed class Contents : IContents
		{
			readonly IContents   _previous;
			readonly ITypePolicy _policy;

			public Contents(IContents previous, ITypePolicy policy)
			{
				_previous = previous;
				_policy   = policy;
			}

			public ISerializer Get(TypeInfo parameter)
				=> _policy.IsSatisfiedBy(parameter) ? _previous.Get(parameter) : EmptySerializer.Instance;
		}

		sealed class EmptySerializer : ISerializer
		{
			public static EmptySerializer Instance { get; } = new EmptySerializer();

			EmptySerializer() {}

			public object Get(IFormatReader parameter) => null;

			public void Write(IFormatWriter writer, object instance) {}
		}

		interface ITypePolicy : ISpecification<Type> {}

		sealed class TypeComparerAdapter : IEqualityComparer<Type>
		{
			public static TypeComparerAdapter Default { get; } = new TypeComparerAdapter();

			TypeComparerAdapter() : this(Defaults.TypeComparer) {}

			readonly ITypeComparer _implementation;

			public TypeComparerAdapter(ITypeComparer implementation) => _implementation = implementation;

			public bool Equals(Type x, Type y) => _implementation.Equals(x.GetTypeInfo(), y.GetTypeInfo());

			public int GetHashCode(Type obj) => _implementation.GetHashCode(obj.GetTypeInfo());
		}

		sealed class AllowedTypesPolicy : ContainsSpecification<Type>, ITypePolicy
		{
			public AllowedTypesPolicy(params Type[] avoid) : this(TypeComparerAdapter.Default, avoid) {}

			public AllowedTypesPolicy(IEqualityComparer<Type> comparer, params Type[] avoid)
				: this(new HashSet<Type>(avoid, comparer)) {}

			public AllowedTypesPolicy(ICollection<Type> avoid) : base(avoid) {}
		}

		sealed class ProhibitedTypePolicy : InverseSpecification<Type>, ITypePolicy
		{
			public ProhibitedTypePolicy(params Type[] avoid) : this(TypeComparerAdapter.Default, avoid) {}

			public ProhibitedTypePolicy(IEqualityComparer<Type> comparer, params Type[] avoid)
				: this(new HashSet<Type>(avoid, comparer)) {}

			public ProhibitedTypePolicy(ICollection<Type> avoid) :
				base(new ContainsSpecification<Type>(avoid)) {}
		}
	}
}