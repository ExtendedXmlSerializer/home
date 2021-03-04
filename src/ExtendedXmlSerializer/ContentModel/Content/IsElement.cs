using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class IsElement : ISpecification<IFormatReader>
	{
		public static IsElement Default { get; } = new IsElement();

		IsElement() : this(MemberProperty.Default.Get) {}

		readonly Func<IFormatReader, bool> _member;

		public IsElement(Func<IFormatReader, bool> member) => _member = member;

		public bool IsSatisfiedBy(IFormatReader parameter)
		{
			var reader = parameter.Get().To<XmlReader>();
			switch (reader.NodeType)
			{
				case XmlNodeType.Attribute:
					var name   = parameter.Name;
					var result = _member(parameter);
					if (result)
					{
						reader.MoveToAttribute(name);
					}

					return result;
			}

			return true;
		}
	}
}