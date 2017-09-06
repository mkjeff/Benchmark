``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4790 CPU 3.60GHz, ProcessorCount=8
Frequency=3507504 Hz, Resolution=285.1030 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2102.0
  DefaultJob : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2102.0


```
                   Method |        Mean |    StdDev |  Gen 0 | Allocated |
------------------------- |------------ |---------- |------- |---------- |
                  NewGuid |  83.4747 ns | 1.0752 ns | 0.0040 |      32 B |
 PureCsCodeSequentialGuid | 280.8663 ns | 2.5649 ns | 0.0198 |     136 B |
     UuidCreateSequential |  55.3232 ns | 0.6649 ns | 0.0232 |     112 B |
 FastUuidCreateSequential |  47.5742 ns | 1.2835 ns | 0.0138 |      72 B |
