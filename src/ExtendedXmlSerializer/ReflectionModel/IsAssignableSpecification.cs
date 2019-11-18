using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// Specification that encapsulates the <see cref="Type.IsAssignableFrom"/> method.
	/// </summary>
	/// <typeparam name="T">The reference type used to call the `IsAssignableFrom`.</typeparam>
	public sealed class IsAssignableSpecification<T> : DelegatedSpecification<TypeInfo>
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static IsAssignableSpecification<T> Default { get; } = new IsAssignableSpecification<T>();

		IsAssignableSpecification() : base(IsAssignableSpecification.Delegates.Get(typeof(T).GetTypeInfo())) {}
	}

	/// <summary>
	/// Specification that encapsulates the <see cref="Type.IsAssignableFrom"/> method.
	/// </summary>
	public sealed class IsAssignableSpecification : DelegatedSpecification<TypeInfo>
	{
		/// <summary>
		/// Store of specification instances, keyed on type.
		/// </summary>
		public static IParameterizedSource<TypeInfo, ISpecification<TypeInfo>> Defaults { get; }
			= new ReferenceCache<TypeInfo, ISpecification<TypeInfo>>(x => new IsAssignableSpecification(x));

		/// <summary>
		/// Store of specification delegates, keyed on type.
		/// </summary>
		public static IParameterizedSource<TypeInfo, Func<TypeInfo, bool>> Delegates { get; }
			= new ReferenceCache<TypeInfo, Func<TypeInfo, bool>>(x => Defaults.Get(x).IsSatisfiedBy);

		IsAssignableSpecification(TypeInfo type) : base(type.IsAssignableFrom) {}
	}
}