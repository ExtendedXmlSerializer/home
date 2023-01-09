using System;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.References;

readonly struct ReferenceBoundary : IDisposable
{
	readonly Stack<object> _context;

	public ReferenceBoundary(Stack<object> context) => _context = context;

	public void Dispose()
	{
		_context.Pop();
	}
}