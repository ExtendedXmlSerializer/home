using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel
{
	class ConditionalWriter : IWriter
	{
		readonly ISpecification<object> _specification;
		readonly IWriter<object>        _writer;

		public ConditionalWriter(ISpecification<object> specification, IWriter<object> writer)
		{
			_specification = specification;
			_writer        = writer;
		}

		public void Write(IFormatWriter writer, object instance)
		{
			if (_specification.IsSatisfiedBy(instance))
			{
				_writer.Write(writer, instance);
			}
		}
	}
}