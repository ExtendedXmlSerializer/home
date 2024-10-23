using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel;

sealed class GeneratedSubstitute : IAlteration<Type>
{
	public static GeneratedSubstitute Default { get; } = new();

	GeneratedSubstitute() : this(typeof(List<>), CollectionItemTypeLocator.Default) {}

	readonly Type                       _definition;
	readonly ICollectionItemTypeLocator _locator;

	public GeneratedSubstitute(Type definition, ICollectionItemTypeLocator locator)
	{
		_definition = definition;
		_locator    = locator;
	}

	public Type Get(Type parameter)
	{
		var arguments = _locator.Get(parameter);
		var result    = _definition.MakeGenericType(arguments);
		return result;
	}
}