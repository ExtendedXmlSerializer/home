using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/// <summary>
	/// A collection of registrations.
	/// </summary>
	/// <typeparam name="T">The instance type to store.</typeparam>
	public sealed class Registrations<T> : IEnumerable<IRegistration>
	{
		readonly static Func<Type, bool> Specification = IsAssignableSpecification<T>.Default.IsSatisfiedBy;

		/// <inheritdoc />
		public Registrations() : this(new HashSet<T>()) {}

		/// <inheritdoc />
		public Registrations(ICollection<T> instances) : this(instances, new HashSet<Type>()) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="instances"></param>
		/// <param name="types"></param>
		public Registrations(ICollection<T> instances, ICollection<Type> types)
		{
			Instances = instances;
			Types     = types;
		}

		/// <summary>
		/// A list of instances.
		/// </summary>
		public ICollection<T> Instances { get; }

		/// <summary>
		/// A list of types.
		/// </summary>
		public ICollection<Type> Types { get; }

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <inheritdoc />
		public IEnumerator<IRegistration> GetEnumerator()
			=> Instances.Select(x => (IRegistration)new FixedRegistration<T>(x))
			            .Concat(Types.Where(Specification)
			                         .Select(x => new Registration<T>(x)))
			            .GetEnumerator();
	}
}