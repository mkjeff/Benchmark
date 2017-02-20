https://blogs.msdn.microsoft.com/seteplia/2017/02/01/dissecting-the-new-constraint-in-c-a-perfect-example-of-a-leaky-abstraction/
``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i5-3570K CPU 3.40GHz, ProcessorCount=4
Frequency=3323583 Hz, Resolution=300.8801 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  DefaultJob : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0

Allocated=24 B  

```
                       Method |       Mean |    StdDev |  Gen 0 |
----------------------------- |----------- |---------- |------- |
              ConstructorCall |  2.3797 ns | 0.0200 ns | 0.0072 |
             FuncBasedFactory |  5.9234 ns | 0.0356 ns | 0.0072 |
       ActivatorCreateInstace | 53.0249 ns | 0.1824 ns | 0.0045 |
     FactoryWithNewConstraint | 59.4535 ns | 0.0519 ns | 0.0042 |
           CompiledExpression | 13.5515 ns | 0.0715 ns | 0.0068 |
  FastActivatorCreateInstance |  7.8671 ns | 0.0134 ns | 0.0072 |
 FastActivator2CreateInstance |  4.7599 ns | 0.0531 ns | 0.0072 |
