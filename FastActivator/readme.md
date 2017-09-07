https://blogs.msdn.microsoft.com/seteplia/2017/02/01/dissecting-the-new-constraint-in-c-a-perfect-example-of-a-leaky-abstraction/
``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507504 Hz, Resolution=285.1030 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2102.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2102.0


```
 |                                      Method |       Mean |     Error |    StdDev |
 |-------------------------------------------- |-----------:|----------:|----------:|
 |                                      T.ctor |  2.3635 ns | 0.1294 ns | 0.1271 ns |
 |                              ValueType.ctor |  0.5673 ns | 0.0167 ns | 0.0156 ns |
 |                                 'Func< T >' |  5.2759 ns | 0.0804 ns | 0.0671 ns |
 |           'Activator.CreateInstance< T >()' | 96.5594 ns | 1.9640 ns | 2.3379 ns |
 |   'Activator.CreateInstance< ValueType >()' | 43.8359 ns | 0.2702 ns | 0.2396 ns |
 |          'Lambda.Compiled( () => new T() )' | 11.9943 ns | 0.0515 ns | 0.0430 ns |
 |               'FastActivator.Create< T >()' |  7.6542 ns | 0.2170 ns | 0.2030 ns |
 |               'FastActivator< T >.Create()' |  3.6735 ns | 0.0824 ns | 0.0771 ns |
 |       'FastActivator< ValueType >.Create()' |  1.9258 ns | 0.0139 ns | 0.0123 ns |
 |          'Activator.CreateInstance( type )' | 44.9902 ns | 0.3897 ns | 0.3454 ns |
 |      'FastActivator.CreateInstance( type )' | 24.1745 ns | 0.1248 ns | 0.1167 ns |
 |     'Activator.CreateInstance( valueType )' | 41.0276 ns | 1.0820 ns | 1.2881 ns |
 | 'FastActivator.CreateInstance( valueType )' | 24.7827 ns | 0.1211 ns | 0.1133 ns |
