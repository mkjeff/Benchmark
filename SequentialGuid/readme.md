``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4790 CPU 3.60GHz, ProcessorCount=8
Frequency=3507501 Hz, Resolution=285.1033 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0


```
                         Method |        Mean |    StdDev |  Gen 0 | Allocated |
------------------------------- |------------ |---------- |------- |---------- |
                        NewGuid | 265.6954 ns | 8.2995 ns |      - |      24 B |
                 SequentialGuid |  59.7019 ns | 1.0308 ns | 0.0156 |      80 B |
 SuppressSecuritySequentialGuid |  52.2618 ns | 0.2585 ns | 0.0157 |      80 B |
       PureCsCodeSequentialGuid | 664.3561 ns | 7.1017 ns |      - |      88 B |
