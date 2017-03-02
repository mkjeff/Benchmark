``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i5-3570K CPU 3.40GHz, ProcessorCount=4
Frequency=3323587 Hz, Resolution=300.8797 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  DefaultJob : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0


```
                         Method |        Mean |    StdDev |  Gen 0 | Allocated |
------------------------------- |------------ |---------- |------- |---------- |
                        NewGuid |  98.2373 ns | 0.6335 ns | 0.0043 |      32 B |
                 SequentialGuid |  67.9856 ns | 0.4902 ns | 0.0325 |     112 B |
 SuppressSecuritySequentialGuid |  64.3827 ns | 1.0451 ns | 0.0325 |     112 B |
       PureCsCodeSequentialGuid | 395.1265 ns | 2.7922 ns | 0.0307 |     136 B |
