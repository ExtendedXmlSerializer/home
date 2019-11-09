using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public sealed class Registrations<T> : IEnumerable<IRegistration>
	{
		readonly static Func<Type, bool> Specification = IsAssignableSpecification<T>.Default.IsSatisfiedBy;

		public Registrations() : this(new HashSet<T>(), new HashSet<Type>()) {}

		public Registrations(ICollection<T> instances, ICollection<Type> types)
		{
			Instances = instances;
			Types     = types;
		}

		public ICollection<T> Instances { get; }
		public ICollection<Type> Types { get; }

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<IRegistration> GetEnumerator()
			=> Instances.Select(x => (IRegistration)new FixedRegistration<T>(x))
			            .Concat(Types.Where(Specification)
			                         .Select(x => new Registration<T>(x)))
			            .GetEnumerator();
	}
}