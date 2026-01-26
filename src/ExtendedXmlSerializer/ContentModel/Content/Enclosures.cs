using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ExtendedXmlSerializer.ContentModel.Content;

sealed class Enclosures : IEnclosures
{
    public Enclosure<T> Get<T>(IWriter<T> start, IWriter<T> body)
    {
        if (typeof(T) == typeof(object)) 
            return new Enclosure((IWriter<object>)start, (IWriter<object>)body) as Enclosure<T>;
        return new Enclosure<T>(start, body); 
    }

    public Enclosure<T> Get<T>(IWriter<T> start, IWriter<T> body, IWriter<T> finish)
    {
        if (typeof(T) == typeof(object))
            return new Enclosure((IWriter<object>)start, (IWriter<object>)body, (IWriter<object>)finish) as Enclosure<T>;
        return new Enclosure<T>(start, body, finish);
    }
}
