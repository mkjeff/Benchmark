https://blogs.msdn.microsoft.com/seteplia/2017/02/01/dissecting-the-new-constraint-in-c-a-perfect-example-of-a-leaky-abstraction/
```
 BenchmarkDotNet=v0.10.9, OS=Windows 10.0.17134
Processor=Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), ProcessorCount=4
.NET Core SDK=2.1.300
  [Host]     : .NET Core 2.0.7 (Framework 4.6.26328.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.7 (Framework 4.6.26328.01), 64bit RyuJIT
```

|                                      Method |       Mean |     Error |    StdDev |
|-------------------------------------------- |-----------:|----------:|----------:|
|                                      T.ctor |  2.7530 ns | 0.0295 ns | 0.0276 ns |
|                              ValueType.ctor |  0.0322 ns | 0.0047 ns | 0.0044 ns |
|                                 'Func< T >' |  7.9970 ns | 0.0603 ns | 0.0564 ns |
|           'Activator.CreateInstance< T >()' | 64.7436 ns | 0.3496 ns | 0.3099 ns |
|   'Activator.CreateInstance< ValueType >()' | 59.8260 ns | 0.1650 ns | 0.1544 ns |
|          'Lambda.Compiled( () => new T() )' |  7.8450 ns | 0.0886 ns | 0.0829 ns |
|               'FastActivator.Create< T >()' |  9.2276 ns | 0.0359 ns | 0.0335 ns |
|               'FastActivator< T >.Create()' |  8.3186 ns | 0.0870 ns | 0.0814 ns |
|       'FastActivator< ValueType >.Create()' |  4.9491 ns | 0.0117 ns | 0.0110 ns |
|          'Activator.CreateInstance( type )' | 54.4961 ns | 0.1137 ns | 0.1063 ns |
|      'FastActivator.CreateInstance( type )' | 33.9466 ns | 0.1133 ns | 0.1060 ns |
|     'Activator.CreateInstance( valueType )' | 54.2365 ns | 0.0630 ns | 0.0526 ns |
| 'FastActivator.CreateInstance( valueType )' | 34.7169 ns | 0.1330 ns | 0.1244 ns |

```
 BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=2.1.300
  [Host]     : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
```
|                                      Method |       Mean |     Error |    StdDev |
|-------------------------------------------- |-----------:|----------:|----------:|
|                                      T.ctor |  3.3465 ns | 0.0338 ns | 0.0316 ns |
|                              ValueType.ctor |  0.1277 ns | 0.0048 ns | 0.0045 ns |
|                                 'Func< T >' |  8.1282 ns | 0.0482 ns | 0.0451 ns |
|           'Activator.CreateInstance< T >()' | 55.8059 ns | 0.1316 ns | 0.1231 ns |
|   'Activator.CreateInstance< ValueType >()' | 50.4040 ns | 0.1352 ns | 0.1265 ns |
|          'Lambda.Compiled( () => new T() )' |  7.9554 ns | 0.0388 ns | 0.0363 ns |
|               'FastActivator.Create< T >()' |  9.0998 ns | 0.0378 ns | 0.0353 ns |
|               'FastActivator< T >.Create()' |  7.9425 ns | 0.0343 ns | 0.0304 ns |
|       'FastActivator< ValueType >.Create()' |  5.2632 ns | 0.0144 ns | 0.0135 ns |
|          'Activator.CreateInstance( type )' | 49.0652 ns | 0.1467 ns | 0.1373 ns |
|      'FastActivator.CreateInstance( type )' | 36.1067 ns | 0.1220 ns | 0.1141 ns |
|     'Activator.CreateInstance( valueType )' | 48.6543 ns | 0.1332 ns | 0.1246 ns |
| 'FastActivator.CreateInstance( valueType )' | 37.4961 ns | 0.1175 ns | 0.1099 ns |