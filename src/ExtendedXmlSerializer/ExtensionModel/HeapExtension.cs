using System;
using System.Collections.Generic;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ISerializer = ExtendedXmlSerializer.ContentModel.ISerializer;

namespace ExtendedXmlSerializer.ExtensionModel;

sealed class HeapExtension : ISerializerExtension, IEnclosures
{
    readonly Coordinator _coordinator;

    public IServiceRepository Get(IServiceRepository parameter)
        => parameter.Register<IReferenceView, ReferenceViewUsingHeap>()
                    .RegisterInstance<IEnclosures>(this)
                    .Decorate<IFormatReaders, FormatReaders>()
                    .Decorate<IMemberSerializers>((_, serializers) => new InterceptorProvider(serializers, _coordinator))
                    ;

    public IWriter<T> WrapWriter<T>(IWriter<T> writer) => new SpecificInterceptor<T>(_coordinator, writer);
    public IReader WrapReader<T>(IReader reader) => new SpecificInterceptor<T>(_coordinator, reader);

    public Enclosure<T> Get<T>(IWriter<T> start, IWriter<T> body)
    {
        return Get(start, body, EndCurrentElement<T>.Default);
    }

    public Enclosure<T> Get<T>(IWriter<T> start, IWriter<T> body, IWriter<T> finish)
    {
        start = WrapWriter<T>(start);
        body = WrapWriter<T>(body);
        finish = WrapWriter<T>(finish);

        if (typeof(T) == typeof(object))
            return new Enclosure((IWriter<object>)start, (IWriter<object>)body, (IWriter<object>)finish) as Enclosure<T>;
        return new Enclosure<T>(start, body, finish);
    }

    void ICommand<IServices>.Execute(IServices parameter) {}

    public HeapExtension() : this(true) { }

    public HeapExtension(bool prioritizeMemory)
    {
        _coordinator = new Coordinator(prioritizeMemory);
    }

    public sealed class InterceptorProvider : IMemberSerializers
    {
        readonly IMemberSerializers _serializers;
        readonly Coordinator _coordinator;

        public InterceptorProvider(IMemberSerializers serializers, Coordinator coordinator)
        {
            _serializers = serializers;
            _coordinator = coordinator;
        }

        public IMemberSerializer Get(IMember parameter)
        {
            var origin = _serializers.Get(parameter);
            if (origin is PropertyMemberSerializer or IRuntimeSerializer)
                return origin;
            return new MemeberIntercetpor(_coordinator, origin);
        }
    }

    public class Interceptor : ISerializer
    {
        protected readonly IWriter _writer;
        protected readonly IReader _reader;
        protected readonly Coordinator _coordinator;
        protected readonly Dictionary<(IRegressableReader, ReaderPosition), (ReaderPosition position, object result)> _readCache;

        public Interceptor(Coordinator coordinator, IWriter writer, IReader reader)
        {
            _writer = writer;
            _reader = reader;
            _coordinator = coordinator;
            _readCache = new();
        }

        public object Get(IFormatReader reader)
        {
            if (_reader == null)
                throw new NullReferenceException($"{nameof(_reader)} is null.");

            if (reader is not IRegressableReader { IsRegressable: true } regressable)
                throw new InvalidOperationException("The reader do not seems to be able to regress.");

            if (_readCache.TryGetValue((regressable, regressable.GetPosition()), out var cached))
            {
                regressable.SetPosition(cached.position);
                return cached.result;
            }

            _coordinator.AddRead(this, regressable);
            
            if (!_coordinator.armed)
                throw new TraversalStackException();

            _coordinator.armed = false;

            object result = null;
            try
            {
                while (_coordinator.NextReader(ref result));
            }
            finally
            {
                _coordinator.Reset();
            }

            return result;
        }

        public object NextGet(IFormatReader reader) => _reader.Get(reader);

        public void StashRead(IRegressableReader reader, ReaderPosition final, object result)
        {
            _readCache.Add((reader, reader.GetPosition()), (final, result));
        }

        public void Write(IFormatWriter writer, object instance)
        {
            if (_writer == null)
                throw new NullReferenceException($"{nameof(_writer)} is null.");

            _coordinator.AddWrite(this, instance, writer);
            
            if (!_coordinator.armed) return;
            _coordinator.armed = false;

            try
            {
                while (_coordinator.NextWrite());
            }
            finally
            {
                _coordinator.Reset();
            }
        }

        public void NextWrite(IFormatWriter writer, object instance) => _writer.Write(writer, instance);

        public void ClearCache()
        {
            _readCache.Clear();
        }
    }

    public sealed class SpecificInterceptor<T> : Interceptor, IWriter<T>
    {
        public SpecificInterceptor(Coordinator coordinator, IWriter<T> writer, IReader reader)
            : base(coordinator, writer.Adapt(), reader) { }

        public SpecificInterceptor(Coordinator coordinator, IWriter<T> writer)
            : base(coordinator, writer.Adapt(), null) { }

        public SpecificInterceptor(Coordinator coordinator, IReader reader)
            : base(coordinator, null, reader) { }

        public void Write(IFormatWriter writer, T instance) => base.Write(writer, instance);
    }

    public sealed class MemeberIntercetpor : Interceptor, IMemberSerializer
    {
        readonly IMemberSerializer _serializer;

        public MemeberIntercetpor(Coordinator coordinator, IMemberSerializer serializer)
            : base(coordinator, serializer, serializer)
        {
            _serializer = serializer;
        }

        public IMember Profile => _serializer.Profile;
        public IMemberAccess Access => _serializer.Access;
    }

    public sealed class Coordinator
    {
        public bool armed;
        public bool prioritizeMemory;

        int writeCurrentIndex;
        readonly List<WeakReference<Interceptor>> _interceptor;
        readonly List<WriterCall> _writeCalls;
        readonly Stack<ReaderCall> _readCalls;

        public Coordinator() : this(true) { }

        public Coordinator(bool prioritizeMemory)
        {
            _writeCalls = new List<WriterCall>();
            _readCalls = new Stack<ReaderCall>();
            _interceptor = new List<WeakReference<Interceptor>>();
            armed = true;
            this.prioritizeMemory = prioritizeMemory;
        }

        public void AddWrite(Interceptor interceptor, object instance, IFormatWriter parameter)
        {
            var call = new WriterCall(interceptor, instance, parameter);
            _writeCalls.Insert(writeCurrentIndex, call);
        }

        public void AddRead(Interceptor interceptor, IRegressableReader reader)
        {
            var positon = reader.GetPosition();
            var call = new ReaderCall(interceptor, reader, positon);
            _readCalls.Push(call);
        }

        public bool NextWrite()
        {
            if (_writeCalls.Count == 0) return false;

            writeCurrentIndex = _writeCalls.Count - 1;
            var current = _writeCalls[writeCurrentIndex];
            _writeCalls.RemoveAt(writeCurrentIndex);

            current.writer.NextWrite(current.parameter, current.instance);
            return true;
        }

        public bool NextReader(ref object result)
        {
            if (_readCalls.Count == 0) return false;

            var current = _readCalls.Peek();
            
            if (!current.reader.SetPosition(current.position))
                throw new InvalidOperationException("Unable to setback the reader positon.");

            try
            {
                result = current.getter.NextGet(current.reader);
            }
            catch (TraversalStackException) { return true; }
            catch (Exception e) when (LastInnerException(e) is TraversalStackException) { return true; }

            var endPosition = current.reader.GetPosition();

            if (!current.reader.SetPosition(current.position))
                throw new InvalidOperationException("Unable to setback the reader positon.");

            _readCalls.Pop();
            current.getter.StashRead(current.reader, endPosition, result);

            if (!current.reader.SetPosition(endPosition))
                throw new InvalidOperationException("Unable to setback the reader positon.");

            return true;
        }

        public void Reset()
        {
            armed = true;
            writeCurrentIndex = 0;
            _writeCalls.Clear();
            if (prioritizeMemory)
                _writeCalls.TrimExcess();
            _readCalls.Clear();
            if (prioritizeMemory)
                _readCalls.TrimExcess();
            for (int i = _interceptor.Count - 1; i >= 0; i--)
            {
                if (_interceptor[i].TryGetTarget(out var interceptor))
                    interceptor.ClearCache();
                else
                    _interceptor.RemoveAt(i);
            }
        }

        public static Exception LastInnerException(Exception ex)
        {
            Exception last = ex;
            while (last.InnerException != null)
            {
                last = last.InnerException;
            }
            return last;
        }

        public readonly struct WriterCall
        {
            public readonly Interceptor writer;
            public readonly object instance;
            public readonly IFormatWriter parameter;

            public WriterCall(Interceptor writer, object instance, IFormatWriter parameter)
            {
                this.writer = writer;
                this.instance = instance;
                this.parameter = parameter;
            }
        }

        public readonly struct ReaderCall
        {
            public readonly Interceptor getter;
            public readonly IRegressableReader reader;
            public readonly Xml.ReaderPosition position;

            public ReaderCall(Interceptor getter, IRegressableReader parameter, Xml.ReaderPosition position)
            {
                this.getter = getter;
                this.reader = parameter;
                this.position = position;
            }
        }
    }

    public sealed class TraversalStackException : Exception
    {
        public TraversalStackException() : base("This exception is not ment to be catched.") { }
    }

    public sealed class FormatReaders : IFormatReaders
    {
		readonly IFormatReaderContexts _read;

		public FormatReaders(IFormatReaderContexts read) => _read = read;

		public IFormatReader Get(System.Xml.XmlReader parameter)
		{
			switch (parameter.MoveToContent())
			{
				case System.Xml.XmlNodeType.Element:
					var result = new RegressableXmlReader(_read.Get(parameter), parameter);
					return result;
				default:
					throw new
						InvalidOperationException($"Could not locate the content from the Xml reader '{parameter}.'");
			}
		}
	}
}