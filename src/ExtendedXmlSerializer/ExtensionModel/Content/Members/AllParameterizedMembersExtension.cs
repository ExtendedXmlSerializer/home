using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class AllParameterizedMembersExtension : ISerializerExtension
	{
		public static AllParameterizedMembersExtension Default { get; } = new AllParameterizedMembersExtension();

		AllParameterizedMembersExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<TypeMembers>()
			            .RegisterWithDependencies<IMemberSpecifications, AllMemberSpecifications>()
			            .Register<IMemberSpecification, MemberSpecification>()
			            .RegisterInstance<IMemberLocators>(MemberLocators.Default)
			            .Register<IConstructorMembers, ConstructorMembers>()
			            .Register<IQueriedConstructors, QueriedConstructors>()
			            .Register<IParameterizedMembers, ParameterizedMembers>()
			            .Decorate<IMemberAccessors, ParameterizedMemberAccessors>()
			            .Decorate<IValidConstructorSpecification, ParameterizedConstructorSpecification>()
			            .Decorate<ITypeMembers, ParameterizedTypeMembers>()
			            .Decorate<IActivators, AllMembersParameterizedActivators>()
			            .Decorate<IInnerContentResult, ParameterizedResultHandler>()
			            .RegisterInstance(RuntimeSerializationExceptionMessage.Default);

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}