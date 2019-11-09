using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	class CircularReferenceEnabledSerialization : ISerializers
	{
		readonly ISerializers _context;

		public CircularReferenceEnabledSerialization(ISerializers context) => _context = context;

		public ISerializer Get(TypeInfo parameter) => new Container(_context.Get(parameter));

		sealed class Container : ISerializer
		{
			readonly ISerializer _serializer;

			public Container(ISerializer serializer) => _serializer = serializer;

			public object Get(IFormatReader parameter) => _serializer.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				try
				{
					_serializer.Write(writer, instance);
				}
				catch (CircularReferencesDetectedException e)
				{
					e.Writer.Write(writer, instance);
				}
			}
		}
	}
}