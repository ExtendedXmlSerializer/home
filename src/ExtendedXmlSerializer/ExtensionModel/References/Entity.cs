using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Properties;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class Entity : IEntity
	{
		readonly static EntityIdentity EntityIdentity = EntityIdentity.Default;

		readonly IConverter        _converter;
		readonly IMemberSerializer _member;

		public Entity(IConverter converter, IMemberSerializer member)
		{
			_converter = converter;
			_member    = member;
		}

		public string Get(object parameter) => _converter.Format(_member.Access.Get(parameter));

		public object Get(IFormatReader parameter)
		{
			var contains = parameter.IsSatisfiedBy(_member.Profile);
			if (contains)
			{
				var result = _member.Get(parameter);
				parameter.Set();
				return result;
			}

			return null;
		}

		public object Reference(IFormatReader parameter)
			=> parameter.IsSatisfiedBy(EntityIdentity) ? _converter.Parse(parameter.Content()) : null;
	}
}