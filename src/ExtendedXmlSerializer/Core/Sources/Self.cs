using System;

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <summary>
	/// Convenience class used for accessing internal components.
	/// </summary>
	public static class Self
	{
		/// <summary>
		/// Returns an alteration that returns the instance that is passed to it.  Used for scenarios where a default value is
		/// needed where the instance passed in is the instance to return.
		/// </summary>
		/// <typeparam name="T">The type to return.</typeparam>
		/// <returns>An alteration that returns the instance that is passed into it.</returns>
		public static IAlteration<T> Instance<T>() => Self<T>.Default;

		/// <summary>
		/// Returns a delegate that returns the instance that is passed to it.  Used for scenarios where a default value is
		/// needed where the instance passed in is the instance to return.
		/// </summary>
		/// <typeparam name="T">The type to return.</typeparam>
		/// <returns>A delegate that returns the instance that is passed into it.</returns>
		public static Func<T, T> Of<T>() => Self<T>.Default.Get;
	}


	sealed class Self<T> : IAlteration<T>
	{
		public static Self<T> Default { get; } = new Self<T>();

		Self() {}

		public T Get(T parameter) => parameter;
	}
}