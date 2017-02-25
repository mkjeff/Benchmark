``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4790 CPU 3.60GHz, ProcessorCount=8
Frequency=3507501 Hz, Resolution=285.1033 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0


```
                         Method |        Mean |    StdDev |      Median |  Gen 0 | Allocated |
------------------------------- |------------ |---------- |------------ |------- |---------- |
                        NewGuid | 247.2260 ns | 1.5294 ns | 247.2297 ns |      - |      24 B |
                 SequentialGuid |  58.7613 ns | 2.9933 ns |  56.8840 ns | 0.0157 |      80 B |
 SuppressSecuritySequentialGuid |  50.5736 ns | 0.4364 ns |  50.4986 ns | 0.0157 |      80 B |
