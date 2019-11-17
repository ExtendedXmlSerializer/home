using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	/// <summary>
	/// A type-based version of <see cref="INames{T}"/>.
	/// </summary>
	public interface INames : INames<TypeInfo> {}

	/// <summary>
	/// A component that resolves a string when providing the metadata of a member.
	/// </summary>
	/// <typeparam name="T">The metadata type.</typeparam>
	public interface INames<in T> : IParameterizedSource<T, string> where T : MemberInfo {}
}