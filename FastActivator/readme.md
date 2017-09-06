https://blogs.msdn.microsoft.com/seteplia/2017/02/01/dissecting-the-new-constraint-in-c-a-perfect-example-of-a-leaky-abstraction/
``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507504 Hz, Resolution=285.1030 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2102.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2102.0


```
 |                             Method |      Mean |     Error |    StdDev |
 |----------------------------------- |----------:|----------:|----------:|
 |                       'new Node()' |  2.669 ns | 0.0307 ns | 0.0287 ns |
 |                 '() => new Node()' |  5.678 ns | 0.1744 ns | 0.1632 ns |
 |      'Create<T>() where T : new()' | 96.676 ns | 1.5484 ns | 1.4483 ns |
 |           'Compiled () => new T()' | 13.130 ns | 0.1188 ns | 0.1053 ns |
 |       FastActivator.Create<Node>() |  7.887 ns | 0.0785 ns | 0.0656 ns |
 |          FastActivator<T>.Create() |  4.237 ns | 0.0617 ns | 0.0515 ns |
 |     Activator.CreateInstance(type) | 45.885 ns | 0.3023 ns | 0.2827 ns |
 | FastActivator.CreateInstance(type) | 24.450 ns | 0.1977 ns | 0.1543 ns |


