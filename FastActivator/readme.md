https://blogs.msdn.microsoft.com/seteplia/2017/02/01/dissecting-the-new-constraint-in-c-a-perfect-example-of-a-leaky-abstraction/
``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507504 Hz, Resolution=285.1030 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2102.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2102.0


```
 |                                  Method |      Mean |     Error |    StdDev |
 |---------------------------------------- |----------:|----------:|----------:|
 |                            'new Node()' |  2.687 ns | 0.0788 ns | 0.0737 ns |
 |                      '() => new Node()' |  5.537 ns | 0.0932 ns | 0.0826 ns |
 |           Activator.CreateInstance<T>() | 96.256 ns | 1.9479 ns | 1.8220 ns |
 |   Activator.CreateInstance<ValueType>() | 43.744 ns | 0.2975 ns | 0.2783 ns |
 |             'Compiled( () => new T() )' | 12.333 ns | 0.0776 ns | 0.0688 ns |
 |               FastActivator.Create<T>() |  8.022 ns | 0.0539 ns | 0.0450 ns |
 |               FastActivator<T>.Create() |  4.037 ns | 0.0225 ns | 0.0176 ns |
 |       FastActivator<ValueType>.Create() |  1.849 ns | 0.0124 ns | 0.0116 ns |
 |          Activator.CreateInstance(type) | 45.783 ns | 1.4076 ns | 1.3824 ns |
 |      FastActivator.CreateInstance(type) | 24.523 ns | 0.1111 ns | 0.0868 ns |
 |     Activator.CreateInstance(valueType) | 38.564 ns | 0.2420 ns | 0.2020 ns |
 | FastActivator.CreateInstance(valueType) | 25.277 ns | 0.1312 ns | 0.1163 ns |
