using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public sealed class MemberFormatExtension : ISerializerExtension
	{
		public MemberFormatExtension()
			: this(new Dictionary<MemberInfo, IAttributeSpecification>(MemberComparer.Default),
			       new HashSet<MemberInfo>(MemberComparer.Default)) {}

		public MemberFormatExtension(IDictionary<MemberInfo, IAttributeSpecification> specifications,
		                             ICollection<MemberInfo> registered)
		{
			Specifications = specifications;
			Registered     = registered;
		}

		public IDictionary<MemberInfo, IAttributeSpecification> Specifications { get; }

		public ICollection<MemberInfo> Registered { get; }

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var specification = new MemberConverterSpecification(new ContainsSpecification<MemberInfo>(Registered)
				                                                     .Or(IsDefinedSpecification<XmlAttributeAttribute>
					                                                         .Default)
			                                                    );
			var specifications = new ContentModel.Members.AttributeSpecifications(Specifications);
			return parameter.RegisterInstance<IAttributeSpecifications>(specifications)
			                .RegisterInstance<IMemberConverterSpecification>(specification)
			                .Register<IMemberConverters, MemberConverters>();
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}