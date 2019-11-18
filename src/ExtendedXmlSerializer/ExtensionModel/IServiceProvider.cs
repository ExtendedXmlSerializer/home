using System;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/// <summary>
	/// A general purpose component that is intended for service location.
	/// </summary>
	public interface IServiceProvider : System.IServiceProvider
	{
		/// <summary>
		/// Gets a component found with the requested type.
		/// </summary>
		/// <param name="serviceType">The requested type.</param>
		/// <returns>The instance found with the requested type.</returns>
		object GetInstance(Type serviceType);

		/// <summary>
		/// Gets a component found with the requested type and argument values.
		/// </summary>
		/// <param name="serviceType">The requested type.</param>
		/// <param name="arguments">A set of arguments to provide for construction.</param>
		/// <returns>The instance found with the requested information.</returns>
		object GetInstance(Type serviceType, object[] arguments);

		/// <summary>
		/// Gets a component found with the requested type, name, and argument values.
		/// </summary>
		/// <param name="serviceType">The requested type.</param>
		/// <param name="serviceName">The unique name for service location.</param>
		/// <param name="arguments">A set of arguments to provide for construction.</param>
		/// <returns>The instance found with the requested information.</returns>
		object GetInstance(Type serviceType, string serviceName, object[] arguments);

		/// <summary>
		/// Gets a component found with the requested type and name.
		/// </summary>
		/// <param name="serviceType">The requested type.</param>
		/// <param name="serviceName">The unique name for service location.</param>
		/// <returns>The instance found with the requested information.</returns>
		object GetInstance(Type serviceType, string serviceName);

		/// <summary>
		/// Tries to get an instance with the requested type.
		/// </summary>
		/// <param name="serviceType">The requested type.</param>
		/// <returns>The instance found with the requested information.</returns>
		object TryGetInstance(Type serviceType);

		/// <summary>
		/// Tries to get an instance with the requested type and name.
		/// </summary>
		/// <param name="serviceType">The requested type.</param>
		/// <param name="serviceName">The unique name for service location.</param>
		/// <returns>The instance found with the requested information.</returns>
		object TryGetInstance(Type serviceType, string serviceName);

		/// <summary>
		/// Gets all instance that are registered with the requested type.
		/// </summary>
		/// <param name="serviceType">The requested type.</param>
		/// <returns>All located instances registered with the requested type.</returns>
		IEnumerable<object> GetAllInstances(Type serviceType);

		/// <summary>
		/// Instantiates a new isntance of the requested type.
		/// </summary>
		/// <param name="serviceType">The requested type.</param>
		/// <returns>A new instance.</returns>
		object Create(Type serviceType);
	}
}