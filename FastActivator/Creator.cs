using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using BenchmarkDotNet.Attributes;

namespace ConsoleApp1
{
    public class Creator
    {
        static T Create<T>() where T : Node, new() => new T();
        static Func<Node> NodeFactory => () => new Node();

        [Benchmark]
        public Node ConstructorCall() => new Node();

        [Benchmark]
        public Node FuncBasedFactory() => NodeFactory();

        [Benchmark]
        public static Node ActivatorCreateInstace() => (Node)Activator.CreateInstance(typeof(Node));

        [Benchmark]
        public Node FactoryWithNewConstraint() => Create<Node>();

        [Benchmark]
        public Node CompiledExpression() => CompiledExpressionMethod.Create<Node>();

        [Benchmark]
        public Node FastActivatorCreateInstance() => FastActivator.Create<Node>();

        [Benchmark]
        public Node FastActivator2CreateInstance() => FastActivator<Node>.Create();
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
    }

    public class Node
    {

    }
}
