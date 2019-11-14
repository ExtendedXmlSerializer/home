using ExtendedXmlSerializer.ContentModel.Reflection;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	class TypeIdentity<T> : ITypeIdentity where T : Attribute
	{
		readonly Func<T, Key> _identity;

		public TypeIdentity(Func<T, Key> identity) => _identity = identity;

		public Key? Get(TypeInfo parameter)
			=> parameter.IsDefined(typeof(T)) ? (Key?)_identity(parameter.GetCustomAttribute<T>()) : null;
	}

	sealed class TypeIdentity : ITypeIdentity
	{
		public static TypeIdentity Default { get; } = new TypeIdentity();

		TypeIdentity() : this(XmlRootIdentity.Default, XmlTypeIdentity.Default) {}

		readonly ITypeIdentity _root, _type;
		public TypeIdentity(ITypeIdentity root, ITypeIdentity type)
		{
			_root  = root;
			_type  = type;
		}

		public Key? Get(TypeInfo parameter)
		{
			var root = _root.Get(parameter);
			var type = _type.Get(parameter);

			var result = root.HasValue || type.HasValue
				             ? new Key(root?.Name ?? type?.Name ?? parameter.Name,
				                       root?.Identifier ?? type?.Identifier)
				             : default;
			return result;
		}
	}
}