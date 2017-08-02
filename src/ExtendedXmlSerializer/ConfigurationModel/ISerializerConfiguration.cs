using System;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.ConfigurationModel
{
	public interface IConfigurationContext : ICreateContext
	{
		ITypeConfiguration With(Type type);
	}

	abstract class ContextBase : ICreateContext
	{
		readonly ICreateContext _parent;

		protected ContextBase(ICreateContext parent) => _parent = parent;

		public IExtendedXmlSerializer Create()
		{
			throw new NotImplementedException();
		}
	}

	class ConfigurationContext : IConfigurationContext
	{
		public IExtendedXmlSerializer Create()
		{
			throw new NotImplementedException();
		}

		public ITypeConfiguration With(Type type)
		{
			throw new NotImplementedException();
		}
	}

	public interface ITypeContext : ICreateContext
	{

	}

	public interface ICreateContext
	{
		IExtendedXmlSerializer Create();
	}
}