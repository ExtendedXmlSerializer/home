using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Properties;
using JetBrains.Annotations;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class VariableTypeMemberContents : IMemberContents
	{
		readonly IVariableTypeMemberSpecifications _specifications;
		readonly IProperty<TypeInfo>               _type;
		readonly ISerializer                       _runtime;
		readonly IContents                         _contents;

		[UsedImplicitly]
		public VariableTypeMemberContents(ISerializer runtime, IContents contents)
			: this(VariableTypeMemberSpecifications.Default, ExplicitTypeProperty.Default, runtime, contents) {}

		// ReSharper disable once TooManyDependencies
		public VariableTypeMemberContents(IVariableTypeMemberSpecifications specifications, IProperty<TypeInfo> type,
		                                  ISerializer runtime, IContents contents)
		{
			_specifications = specifications;
			_type           = type;
			_runtime        = runtime;
			_contents       = contents;
		}

		public ISerializer Get(IMember parameter)
		{
			var contents      = _contents.Get(parameter.MemberType);
			var specification = _specifications.Get(parameter);
			var result =
				new Serializer(contents, new VariableTypedMemberWriter(specification, _runtime, contents, _type));
			return result;
		}
	}
}