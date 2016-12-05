/*namespace ExtendedXmlSerialization.Common
{
	/// <summary>
	/// Attribution: https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Serialization/DefaultSerializationBinder.cs
	/// </summary>
	class DefaultSerializationBinder : SerializationBinder
	{
		readonly static ConcurrentDictionary<TypeNameKey, Type> Types = new ConcurrentDictionary<TypeNameKey, Type>();
		readonly private static Func<TypeNameKey, Type> GetValue = GetTypeFromTypeNameKey;

		public static DefaultSerializationBinder Default { get; } = new DefaultSerializationBinder();
		DefaultSerializationBinder() {}

		public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = serializedType.GetTypeInfo().Assembly.FullName;
			typeName = serializedType.FullName;
		}

		public override Type BindToType(string assemblyName, string typeName) => Types.GetOrAdd(new TypeNameKey(assemblyName, typeName), GetValue);

		private static Type GetTypeFromTypeNameKey(TypeNameKey typeNameKey) => FromType(typeNameKey) ?? Load(typeNameKey);

		static Type FromType(TypeNameKey typeNameKey)
		{
			var assemblyName = typeNameKey.AssemblyName;
			var typeName = typeNameKey.TypeName;
			var result = assemblyName == null ? Type.GetType(typeName) : null;
			return result;
		}

		private static Type Load(TypeNameKey key)
		{
			var assembly1 = Assembly.Load(new AssemblyName(key.AssemblyName)) ?? Find(key.AssemblyName);
			if (assembly1 != null)
			{
				var type = assembly1.GetType(key.TypeName);
				if (type != null)
				{
					return type;
				}
				throw new InvalidOperationException(
					      $"Could not find type '{key.TypeName}' in assembly '{key.AssemblyName}'.");
			}
			throw new InvalidOperationException(
				      $"Could not load assembly '{key.AssemblyName}'.");
		}

		private static Assembly Find(string assemblyName)
		{
			foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly2.FullName == assemblyName)
				{
					return assembly2;
				}
			}
			return null;
		}

		struct TypeNameKey : IEquatable<TypeNameKey>
		{
			public TypeNameKey(string assemblyName, string typeName)
			{
				AssemblyName = assemblyName;
				TypeName = typeName;
			}

			public string AssemblyName { get; }
			public string TypeName { get; }

			public override int GetHashCode() => (AssemblyName?.GetHashCode() ?? 0) ^
			                                     (TypeName?.GetHashCode() ?? 0);

			public override bool Equals(object obj) => obj is TypeNameKey && Equals((TypeNameKey) obj);

			public bool Equals(TypeNameKey other) => AssemblyName == other.AssemblyName && TypeName == other.TypeName;
		}
	}
}*/