using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	/// <summary>
	/// Base converter used as a convenience for extension authors.
	/// </summary>
	/// <typeparam name="T">The type to convert.</typeparam>
	public abstract class ConverterBase<T> : DecoratedSpecification<TypeInfo>, IConverter<T>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		protected ConverterBase() : this(TypeEqualitySpecification<T>.Default) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="specification">The specification that determines whether the created converter handles the candidate
		/// type.</param>
		protected ConverterBase(ISpecification<TypeInfo> specification) : base(specification) {}

		/// <summary>
		/// Used to parse the provided text into a new instance of the converter type.
		/// </summary>
		/// <param name="data">The text from which to create a new instance.</param>
		/// <returns>An instance created from the provided text.</returns>
		public abstract T Parse(string data);

		/// <summary>
		/// Used to format the provided instance into its text representation.
		/// </summary>
		/// <param name="instance">The instance to convert into a text string.</param>
		/// <returns>The string representation of the provided instance.</returns>
		public abstract string Format(T instance);
	}
}