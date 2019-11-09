using JetBrains.Annotations;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	struct MemberDescriptor : IEquatable<MemberDescriptor>
	{
		public static implicit operator MemberDescriptor(MemberInfo member) => MemberDescriptors.Default.Get(member);

		readonly int _code;

		public MemberDescriptor(PropertyInfo metadata)
			: this(metadata, metadata.PropertyType.GetTypeInfo(), metadata.CanWrite) {}

		public MemberDescriptor(PropertyInfo metadata, TypeInfo memberType)
			: this(metadata, Validate(metadata, metadata.PropertyType.GetTypeInfo(), memberType), metadata.CanWrite) {}

		public MemberDescriptor(FieldInfo metadata)
			: this(metadata, metadata.FieldType.GetTypeInfo(), !metadata.IsInitOnly) {}

		[UsedImplicitly]
		public MemberDescriptor(FieldInfo metadata, TypeInfo memberType)
			: this(metadata, Validate(metadata, metadata.FieldType.GetTypeInfo(), memberType), !metadata.IsInitOnly) {}

		public MemberDescriptor(TypeInfo type) : this(type, type, false) {}

		MemberDescriptor(MemberInfo metadata, TypeInfo memberType, bool writable)
			: this(metadata, memberType, writable, (metadata.GetHashCode() * 397) ^ memberType.GetHashCode()) {}

		// ReSharper disable once TooManyDependencies
		MemberDescriptor(MemberInfo metadata, TypeInfo memberType, bool writable, int code)
		{
			_code      = code;
			Metadata   = metadata;
			MemberType = memberType;
			Writable   = writable;
		}

		static TypeInfo Validate(MemberInfo member, TypeInfo defaultType, TypeInfo assigned)
		{
			if (!Equals(defaultType, assigned) && !defaultType.IsAssignableFrom(assigned))
			{
				throw new InvalidOperationException(
				                                    $"Cannot assign type '{assigned}' as a return type for '{member}'.  Ensure that the specified type can be assigned from '{defaultType}'");
			}

			return assigned;
		}

		public MemberInfo Metadata { get; }
		public TypeInfo MemberType { get; }
		public bool Writable { get; }

		public bool Equals(MemberDescriptor other)
		{
			var code = _code;
			return code.Equals(other._code);
		}

		public override bool Equals(object obj)
			=> !ReferenceEquals(null, obj) && obj is MemberDescriptor && Equals((MemberDescriptor)obj);

		public override int GetHashCode() => _code;

		public static bool operator ==(MemberDescriptor left, MemberDescriptor right) => left.Equals(right);

		public static bool operator !=(MemberDescriptor left, MemberDescriptor right) => !left.Equals(right);
	}
}