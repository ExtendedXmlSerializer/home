using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class TypeModelExtension : ISerializerExtension
	{
		public static TypeModelExtension Default { get; } = new TypeModelExtension();

		TypeModelExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<ITypedSortOrder, TypedSortOrder>()
			            .Register<IActivation, Activation>()
			            .Register<IActivators, DefaultActivators>()
			            .Register<IActivatingTypeSpecification, ActivatingTypeSpecification>()
			            .Register<IConstructorLocator, ConstructorLocator>()
			            .Register<IEnumeratorStore, EnumeratorStore>()
			            .Register<IDiscoveredTypes, DiscoveredTypes>()
			            .RegisterInstance<IDictionaryEnumerators>(DictionaryEnumerators.Default)
			            .RegisterInstance<IEnumerators>(Enumerators.Default)
			            .RegisterInstance<IConstructors>(Constructors.Default)
			            .RegisterInstance<IFields>(Fields.Default)
			            .RegisterInstance<IProperties>(Properties.Default)
			            .RegisterInstance<IValidConstructorSpecification>(ValidConstructorSpecification.Default);

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}