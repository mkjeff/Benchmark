```
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=2.1.300
  [Host]     : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
```

|                   Method |      Mean |     Error |    StdDev |  Gen 0 | Allocated |
|------------------------- |----------:|----------:|----------:|-------:|----------:|
|                  NewGuid |  84.73 ns | 0.1995 ns | 0.1866 ns |      - |       0 B |
| PureCsCodeSequentialGuid | 164.33 ns | 0.7201 ns | 0.6736 ns |      - |       0 B |
| FastUuidCreateSequential |  44.90 ns | 0.0914 ns | 0.0855 ns | 0.0127 |      40 B |
|               CombLegacy | 350.63 ns | 1.5187 ns | 1.4206 ns | 0.0429 |     136 B |
|                  CombSql | 298.40 ns | 0.8155 ns | 0.7628 ns | 0.0329 |     104 B |