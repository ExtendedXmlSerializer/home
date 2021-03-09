using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Specifications;
using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class Contains : ISpecification<(IFormatReader Reader, IIdentity Identity)>
	{
		public static Contains Default { get; } = new Contains();

		Contains() {}

		public bool IsSatisfiedBy((IFormatReader Reader, IIdentity Identity) parameter)
		{
			var (format, identity) = parameter;

			var reader = format.Get().To<XmlReader>();
			var name   = reader.NodeType == XmlNodeType.Attribute ? format.Name : null;
			var result = format.IsSatisfiedBy(identity);
			if (name != null)
			{
				reader.MoveToAttribute(name);
			}

			return result;
		}
	}
}