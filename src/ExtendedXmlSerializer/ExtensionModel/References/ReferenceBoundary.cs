using System;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.References;

readonly struct ReferenceBoundary : IDisposable
{
	readonly Stack<object> _context;

	public ReferenceBoundary(Stack<object> context, object subject)
	{
		Subject  = subject;
		_context = context;
	}

	public object Subject { get; }

	public void Dispose()
	{
		_context.Push(ReferenceCompleted.Default);
	}
}