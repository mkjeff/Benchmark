using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection.Emit;
using BenchmarkDotNet.Attributes;

namespace ConsoleApp1
{
    public class Creator
    {
        private static readonly Type type = typeof(Node);
        private static readonly Type valueType = typeof(Guid);
        static T Create<T>() where T : new() => new T();
        static Func<Node> NodeFactory => () => new Node();

        [Benchmark(Description = "new Node()")]
        public Node ConstructorCall() => new Node();

        [Benchmark(Description = "() => new Node()")]
        public Node FuncBasedFactory() => NodeFactory();

        [Benchmark(Description = "Activator.CreateInstance<T>()")]
        public Node CreateInstanceWithNewConstraint() => Create<Node>();

        [Benchmark(Description = "Activator.CreateInstance<ValueType>()")]
        public Guid CreateBoxedValueWithNewConstraint() => Create<Guid>();

        [Benchmark(Description = "Compiled( () => new T() )")]
        public Node CompiledExpression() => CompiledExpressionMethod.Create<Node>();

        [Benchmark(Description = "FastActivator.Create<T>()")]
        public Node FastActivatorCreateInstance() => FastActivator.Create<Node>();

        [Benchmark(Description = "FastActivator<T>.Create()")]
        public Node FastActivator2CreateInstance() => FastActivator<Node>.Create();

        [Benchmark(Description = "FastActivator<ValueType>.Create()")]
        public Guid FastActivator2CreateBoxedValue() => FastActivator<Guid>.Create();

        [Benchmark(Description = "Activator.CreateInstance(type)")]
        public object ActivatorCreateInstanceNonGeneric() => Activator.CreateInstance(type);

        [Benchmark(Description = "FastActivator.CreateInstance(type)")]
        public object FastActivatorCreateInstanceNonGeneric() => FastActivator.CreateInstance(type);

        [Benchmark(Description = "Activator.CreateInstance(valueType)")]
        public object ActivatorCreateBoxedValueNonGeneric() => Activator.CreateInstance(valueType);

        [Benchmark(Description = "FastActivator.CreateInstance(valueType)")]
        public object FastActivatorCreateBoxedValueNonGeneric() => FastActivator.CreateInstance(valueType);
    }

    public static class CompiledExpressionMethod
    {
        public static T Create<T>() where T : new()
        {
            return FastActivatorImpl<T>.NewFunction();
        }

        private static class FastActivatorImpl<T> where T : new()
        {
            // Compiler translates 'new T()' into Expression.New()
            private static readonly Expression<Func<T>> NewExpression = () => new T();

            // Compiling expression into the delegate
            public static readonly Func<T> NewFunction = NewExpression.Compile();
        }
    }

    public static class FastActivator
    {
        public static T Create<T>() where T : new()
        {
            return FastActivatorImpl<T>.Create();
        }

        private static readonly ConcurrentDictionary<Type, Func<object>> FactoryDictionary
            = new ConcurrentDictionary<Type, Func<object>>(1,10);

        public static object CreateInstance(Type type) =>
            FactoryDictionary.GetOrAdd(type,
                key => key.IsValueType
                    ? (Func<object>)typeof(DynamicModuleLambdaCompiler)
                        .GetMethod(nameof(DynamicModuleLambdaCompiler.GenerateValueTypeFactory))
                        .Invoke(null, new object[] { key })
                    : (Func<object>)typeof(DynamicModuleLambdaCompiler)
                        .GetMethod(nameof(DynamicModuleLambdaCompiler.GenerateFactory))
                        .MakeGenericMethod(key)
                        .Invoke(null, null))();

        private static class FastActivatorImpl<T> where T : new()
        {
            public static readonly Func<T> Create =
                DynamicModuleLambdaCompiler.GenerateFactory<T>();
        }
    }

    public static class FastActivator<T> where T : new()
    {
        /// <summary>
        /// Extremely fast generic factory method that returns an instance
        /// of the type <typeparam name="T"/>.
        /// </summary>
        public static readonly Func<T> Create =
            DynamicModuleLambdaCompiler.GenerateFactory<T>();
    }

    public static class DynamicModuleLambdaCompiler
    {
        public static Func<T> GenerateFactory<T>() where T : new()
        {
            Expression<Func<T>> expr = () => new T();
            NewExpression newExpr = (NewExpression)expr.Body;

            var method = new DynamicMethod(
                name: "lambda",
                returnType: newExpr.Type,
                parameterTypes: new Type[0],
                m: typeof(DynamicModuleLambdaCompiler).Module,
                skipVisibility: true);

            ILGenerator ilGen = method.GetILGenerator();
            // Constructor for value types could be null
            if (newExpr.Constructor != null)
            {
                ilGen.Emit(OpCodes.Newobj, newExpr.Constructor);
            }
            else
            {
                LocalBuilder temp = ilGen.DeclareLocal(newExpr.Type);
                ilGen.Emit(OpCodes.Ldloca, temp);
                ilGen.Emit(OpCodes.Initobj, newExpr.Type);
                ilGen.Emit(OpCodes.Ldloc, temp);
            }

            ilGen.Emit(OpCodes.Ret);

            return (Func<T>)method.CreateDelegate(typeof(Func<T>));
        }

        public static Func<object> GenerateValueTypeFactory(Type valueType)
        {
            var method = new DynamicMethod(
                name: "lambda",
                returnType: typeof(object),
                parameterTypes: new Type[0],
                m: typeof(DynamicModuleLambdaCompiler).Module,
                skipVisibility: true);

            ILGenerator ilGen = method.GetILGenerator();

            LocalBuilder temp = ilGen.DeclareLocal(valueType);
            ilGen.Emit(OpCodes.Ldloca, temp);
            ilGen.Emit(OpCodes.Initobj, valueType);
            ilGen.Emit(OpCodes.Ldloc, temp);
            ilGen.Emit(OpCodes.Box, valueType);

            ilGen.Emit(OpCodes.Ret);

            return (Func<object>)method.CreateDelegate(typeof(Func<object>));
        }
    }

    public class Node
    {

    }
}
