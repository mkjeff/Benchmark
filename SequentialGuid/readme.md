``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i5-3570K CPU 3.40GHz, ProcessorCount=4
Frequency=3323587 Hz, Resolution=300.8797 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  DefaultJob : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0


```
                   Method |        Mean |    StdDev |  Gen 0 | Allocated |
------------------------- |------------ |---------- |------- |---------- |
                  NewGuid |  93.0952 ns | 0.0734 ns | 0.0070 |      32 B |
 PureCsCodeSequentialGuid | 367.4043 ns | 0.3482 ns | 0.0317 |     136 B |
     UuidCreateSequential |  54.9215 ns | 0.0659 ns | 0.0323 |     112 B |
 FastUuidCreateSequential |  46.7796 ns | 0.0906 ns | 0.0195 |      72 B |
