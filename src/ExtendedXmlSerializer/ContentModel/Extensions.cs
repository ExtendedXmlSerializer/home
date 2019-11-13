using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel
{
	static class Extensions
	{
		public static IReader<T> CreateContents<T>(this IInnerContentServices @this, IInnerContentHandler parameter)
			=> new ReaderAdapter<T>(@this.Create(Support<T>.Metadata, parameter));

		public static TypeInfo GetClassification(this IClassification @this, IFormatReader parameter,
		                                         TypeInfo defaultValue = null)
		{
			var result = @this.Get(parameter) ?? defaultValue;
			if (result == null)
			{
				var name = IdentityFormatter.Default.Get(parameter);
				throw new InvalidOperationException(
				                                    $"An attempt was made to load a type with the fully qualified name of '{name}', but no type could be located with that name.");
			}

			return result;
		}
	}
}