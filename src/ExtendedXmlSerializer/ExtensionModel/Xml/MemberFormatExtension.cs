using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// Default extension used to determine whether to format a given member as an Xml attribute or as an Xml element.
	/// </summary>
	public sealed class MemberFormatExtension : ISerializerExtension
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public MemberFormatExtension()
			: this(new Dictionary<MemberInfo, IAttributeSpecification>(MemberComparer.Default),
			       new HashSet<MemberInfo>(MemberComparer.Default)) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="specifications">Registry of specifications.</param>
		/// <param name="registered">A collection of registered members.</param>
		public MemberFormatExtension(IDictionary<MemberInfo, IAttributeSpecification> specifications,
		                             ICollection<MemberInfo> registered)
		{
			Specifications = specifications;
			Registered     = registered;
		}

		/// <summary>
		/// Registry of specifications.
		/// </summary>
		public IDictionary<MemberInfo, IAttributeSpecification> Specifications { get; }

		/// <summary>
		/// A collection of registered members.
		/// </summary>
		public ICollection<MemberInfo> Registered { get; }

		/// <inheritdoc />
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