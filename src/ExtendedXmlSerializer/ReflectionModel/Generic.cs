using System;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class Generic<T> : GenericAdapterBase<Func<T>>, IGeneric<T>
	{
		readonly static IParameterizedSource<TypeInfo, Func<T>> Activator =
			new GenericActivators<Func<T>>(GenericSingleton.Default).ReferenceCache();

		public Generic(Type definition) : this(definition, Activator) {}

		public Generic(Type definition, IParameterizedSource<TypeInfo, Func<T>> source) : base(definition, source) {}
	}

	class Generic<T1, T> : GenericAdapterBase<Func<T1, T>>, IGeneric<T1, T>
	{
		readonly static IParameterizedSource<TypeInfo, Func<T1, T>> Activator
			= new GenericActivators<Func<T1, T>>(typeof(T1)).ReferenceCache();

		public Generic(Type definition) : this(definition, Activator) {}

		public Generic(Type definition, IParameterizedSource<TypeInfo, Func<T1, T>> source) :
			base(definition, source) {}
	}

	class Generic<T1, T2, T> : GenericAdapterBase<Func<T1, T2, T>>, IGeneric<T1, T2, T>
	{
		readonly static IParameterizedSource<TypeInfo, Func<T1, T2, T>> Activator
			= new GenericActivators<Func<T1, T2, T>>(typeof(T1), typeof(T2)).ReferenceCache();

		public Generic(Type definition) : this(definition, Activator) {}

		public Generic(Type definition, IParameterizedSource<TypeInfo, Func<T1, T2, T>> source)
			: base(definition, source) {}
	}

	class Generic<T1, T2, T3, T> : GenericAdapterBase<Func<T1, T2, T3, T>>, IGeneric<T1, T2, T3, T>
	{
		readonly static IParameterizedSource<TypeInfo, Func<T1, T2, T3, T>> Activator
			= new GenericActivators<Func<T1, T2, T3, T>>(typeof(T1), typeof(T2), typeof(T3)).ReferenceCache();

		public Generic(Type definition) : this(definition, Activator) {}

		public Generic(Type definition, IParameterizedSource<TypeInfo, Func<T1, T2, T3, T>> source)
			: base(definition, source) {}
	}

	class Generic<T1, T2, T3, T4, T> : GenericAdapterBase<Func<T1, T2, T3, T4, T>>, IGeneric<T1, T2, T3, T4, T>
	{
		readonly static IParameterizedSource<TypeInfo, Func<T1, T2, T3, T4, T>> Activator
			= new GenericActivators<Func<T1, T2, T3, T4, T>>(typeof(T1), typeof(T2), typeof(T3), typeof(T4))
				.ReferenceCache();

		public Generic(Type definition) : this(definition, Activator) {}

		public Generic(Type definition, IParameterizedSource<TypeInfo, Func<T1, T2, T3, T4, T>> source)
			: base(definition, source) {}
	}
}