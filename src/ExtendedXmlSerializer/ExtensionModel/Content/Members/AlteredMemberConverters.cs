using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class AlteredMemberConverters : IMemberConverters
	{
		readonly static AssignedSpecification<IConverter> AssignedSpecification =
			AssignedSpecification<IConverter>.Default;

		readonly ISpecification<MemberInfo> _specification;
		readonly ISpecification<IConverter> _assigned;
		readonly IAlteration<IConverter>    _alteration;
		readonly IMemberConverters          _converters;

		public AlteredMemberConverters(ISpecification<MemberInfo> specification, IAlteration<IConverter> alteration,
		                               IMemberConverters converters)
			: this(specification, AssignedSpecification, alteration, converters) {}

		// ReSharper disable once TooManyDependencies
		public AlteredMemberConverters(ISpecification<MemberInfo> specification, ISpecification<IConverter> assigned,
		                               IAlteration<IConverter> alteration,
		                               IMemberConverters converters)
		{
			_specification = specification;
			_assigned      = assigned;
			_alteration    = alteration;
			_converters    = converters;
		}

		public IConverter Get(MemberInfo parameter)
		{
			var converter = _converters.Get(parameter);
			var result = _assigned.IsSatisfiedBy(converter) && _specification.IsSatisfiedBy(parameter)
				             ? _alteration.Get(converter)
				             : converter;
			return result;
		}
	}
}