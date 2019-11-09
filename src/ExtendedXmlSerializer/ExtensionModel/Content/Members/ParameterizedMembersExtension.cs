using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedMembersExtension : ISerializerExtension
	{
		public static ParameterizedMembersExtension Default { get; } = new ParameterizedMembersExtension();

		ParameterizedMembersExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<TypeMembers>()
			            .RegisterWithDependencies<IMemberSpecifications, MemberSpecifications>()
			            .Register<IMemberSpecification, MemberSpecification>()
			            .RegisterInstance<IMemberLocators>(MemberLocators.Default)
			            .Register<IConstructorMembers, ConstructorMembers>()
			            .Register<IQueriedConstructors, QueriedConstructors>()
			            .Register<IParameterizedMembers, ParameterizedMembers>()
			            .Decorate<IMemberAccessors, ParameterizedMemberAccessors>()
			            .Decorate<IValidConstructorSpecification, ParameterizedConstructorSpecification>()
			            .Decorate<ITypeMembers, ParameterizedTypeMembers>()
			            .Decorate<IActivators, ParameterizedActivators>()
			            .Decorate<IInnerContentResult, ParameterizedResultHandler>()
			            .RegisterInstance(RuntimeSerializationExceptionMessage.Default);

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}