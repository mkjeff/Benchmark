https://blogs.msdn.microsoft.com/seteplia/2017/02/01/dissecting-the-new-constraint-in-c-a-perfect-example-of-a-leaky-abstraction/
``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i5-3570K CPU 3.40GHz, ProcessorCount=4
Frequency=3323587 Hz, Resolution=300.8797 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  DefaultJob : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0

Allocated=24 B  

```
                       Method |       Mean |    StdErr |    StdDev |  Gen 0 |
----------------------------- |----------- |---------- |---------- |------- |
              ConstructorCall |  4.6303 ns | 0.0528 ns | 0.2044 ns | 0.0072 |
             FuncBasedFactory |  7.8399 ns | 0.1165 ns | 0.5825 ns | 0.0068 |
       ActivatorCreateInstace | 52.6771 ns | 0.0608 ns | 0.2354 ns | 0.0045 |
     FactoryWithNewConstraint | 63.0015 ns | 0.0604 ns | 0.2179 ns | 0.0041 |
           CompiledExpression | 16.3605 ns | 0.0286 ns | 0.1107 ns | 0.0068 |
  FastActivatorCreateInstance | 10.3308 ns | 0.0360 ns | 0.1395 ns | 0.0073 |
 FastActivator2CreateInstance |  4.8204 ns | 0.0083 ns | 0.0312 ns | 0.0072 |

