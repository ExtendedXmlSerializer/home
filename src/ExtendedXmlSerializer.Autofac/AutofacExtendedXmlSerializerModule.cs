using Autofac;

namespace ExtendedXmlSerialization.Autofac
{
    public class AutofacExtendedXmlSerializerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacSerializationToolsFactory>()
                .AsSelf()
                .As<ISerializationToolsFactory>()
                .SingleInstance();

            builder.RegisterType<ExtendedXmlSerializer>().UsingConstructor(typeof(ISerializationToolsFactory))
                .AsSelf()
                .As<IExtendedXmlSerializer>()
                .SingleInstance();
        }
    }
}
