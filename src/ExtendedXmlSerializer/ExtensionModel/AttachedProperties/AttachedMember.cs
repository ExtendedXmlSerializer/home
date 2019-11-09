using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	sealed class AttachedMember : IMember, IProperty
	{
		readonly static Delimiter Separator = DefaultClrDelimiters.Default.Separator;

		readonly IIdentity _type;
		readonly IMember   _member;
		readonly IProperty _property;
		readonly string    _separator;

		public AttachedMember(IIdentity type, IMember member, IProperty property)
			: this(type, member, property, property.Get(), Separator) {}

		// ReSharper disable once TooManyDependencies
		public AttachedMember(IIdentity type, IMember member, IProperty property, TypeInfo memberType, string separator)
		{
			_type      = type;
			_member    = member;
			_property  = property;
			MemberType = memberType;
			_separator = separator;
		}

		public string Identifier => _type.Identifier;

		public string Name => $"{_type.Name}{_separator}{_member.Name}";

		public MemberInfo Metadata => _property.Metadata;

		public TypeInfo MemberType { get; }

		public bool IsWritable => _member.IsWritable;

		public int Order => _member.Order;

		public TypeInfo Get() => _property.Get();

		public bool IsSatisfiedBy(TypeInfo parameter) => _property.IsSatisfiedBy(parameter);

		public bool IsSatisfiedBy(object parameter) => _property.IsSatisfiedBy((TypeInfo)parameter);

		public object Get(object parameter) => _property.Get(parameter);

		public void Assign(object key, object value) => _property.Assign(key, value);

		public bool Remove(object key) => _property.Remove(key);

		PropertyInfo IProperty.Metadata => _property.Metadata;
	}
}