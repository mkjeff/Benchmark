using System;
using BenchmarkDotNet.Attributes;

namespace ConsoleApp1
{
    public class CreatorTest
    {
        private static readonly Type type = typeof(Node);
        private static readonly Type valueType = typeof(Guid);
        static T Create<T>() where T : new() => new T();
        static Func<Node> NodeFactory => () => new Node();

        [Benchmark(Description = "T.ctor")]
        public Node Constructor() => new Node();

        [Benchmark(Description = "ValueType.ctor")]
        public Guid ConstructorCall() => new Guid();

        [Benchmark(Description = "Func< T >")]
        public Node FuncBasedFactory() => NodeFactory();

        [Benchmark(Description = "Activator.CreateInstance< T >()")]
        public Node CreateInstanceWithNewConstraint() => Create<Node>();

        [Benchmark(Description = "Activator.CreateInstance< ValueType >()")]
        public Guid CreateBoxedValueWithNewConstraint() => Create<Guid>();

        [Benchmark(Description = "Lambda.Compiled( () => new T() )")]
        public Node CompiledExpression() => CompiledExpressionMethod.Create<Node>();

        [Benchmark(Description = "FastActivator.Create< T >()")]
        public Node FastActivatorCreateInstance() => FastActivator.Create<Node>();

        [Benchmark(Description = "FastActivator< T >.Create()")]
        public Node FastActivator2CreateInstance() => FastActivator<Node>.Create();

        [Benchmark(Description = "FastActivator< ValueType >.Create()")]
        public Guid FastActivator2CreateBoxedValue() => FastActivator<Guid>.Create();

        [Benchmark(Description = "Activator.CreateInstance( type )")]
        public object ActivatorCreateInstanceNonGeneric() => Activator.CreateInstance(type);

        [Benchmark(Description = "FastActivator.CreateInstance( type )")]
        public object FastActivatorCreateInstanceNonGeneric() => FastActivator.CreateInstance(type);

        [Benchmark(Description = "Activator.CreateInstance( valueType )")]
        public object ActivatorCreateBoxedValueNonGeneric() => Activator.CreateInstance(valueType);

        [Benchmark(Description = "FastActivator.CreateInstance( valueType )")]
        public object FastActivatorCreateBoxedValueNonGeneric() => FastActivator.CreateInstance(valueType);
    }

    public class Node
    {

    }
}
