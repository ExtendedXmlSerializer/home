```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4820K CPU 3.70GHz, ProcessorCount=8
Frequency=3613275 ticks, Resolution=276.7572 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=64-bit DEBUG [AttachedDebugger]
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1586.0

Type=ExtendedXmlSerializerTest  Mode=Throughput  

```
                            Method |        Median |     StdDev |
---------------------------------- |-------------- |----------- |
   SerializationClassWithPrimitive | 2,671.8025 us | 46.6631 us |
 DeserializationClassWithPrimitive |   132.6964 us |  1.1534 us |
